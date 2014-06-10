using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Security.Cryptography;
using System.Text;

namespace TFSFeedBuilder
{
    class DAL
    {
        #region Public Methods

        public static int FetchUsersCount()
        {
            int count = 0;

            string sqlSelectQuery = string.Format("SELECT COUNT(*) FROM {0}", Constants.UserTable);

            using (var con = new SqlCeConnection(Properties.Settings.Default.DataStoreConnectionString))
            {
                var cmd = new SqlCeCommand(sqlSelectQuery, con);
                try
                {
                    con.Open();
                    count = (Int32)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return count;
        }

        public static void AddUser(string username, string password, string domain, string tfsLocation, string saveLocation)
        {
            string selectCmd = string.Format("SELECT * FROM {0}", Constants.UserTable);

            using (var con = new SqlCeConnection(Properties.Settings.Default.DataStoreConnectionString))
            {
                con.Open();

                var adapter = new SqlCeDataAdapter(selectCmd, con);

                // ReSharper disable SuggestUseVarKeywordEvident -- Note: With the Data* series, I like using explicit typing.
                DataSet ds = new DataSet();
                // ReSharper restore SuggestUseVarKeywordEvident

                adapter.FillSchema(ds, SchemaType.Source, Constants.UserTable);
                adapter.Fill(ds, Constants.UserTable);

                // ReSharper disable UnusedVariable -- Note: this object is required to properly commit to the DB
                SqlCeCommandBuilder sqlCb = new SqlCeCommandBuilder(adapter);
                // ReSharper restore UnusedVariable

                var newRow = ds.Tables[Constants.UserTable].NewRow();
                var tempSalt = GenerateSalt();

                newRow[Constants.UserCol] = username;
                newRow[Constants.SaltCol] = tempSalt;
                //TODO: THIS IS STORING IN PLAINTEXT. BAD. USE GenerateSaltedHash(tempSalt, password);
                newRow[Constants.PassCol] = password; 
                newRow[Constants.DomainCol] = domain;
                newRow[Constants.TfsCol] = tfsLocation;
                newRow[Constants.SaveCol] = saveLocation;

                ds.Tables[Constants.UserTable].Rows.Add(newRow);
                adapter.Update(ds, Constants.UserTable);

                con.Close();
            }
        }

        public static DataRow FetchUser()
        {
            //could pass in a string "username" and make this more powerful, see below
            //string selectCmd = string.Format("SELECT * FROM {0} WHERE {1}={2}",
            //                                 userTable, userCol, username);

            string selectCmd = string.Format("SELECT * FROM {0}", Constants.UserTable);

            DataRow curRow;

            using (var con = new SqlCeConnection(Properties.Settings.Default.DataStoreConnectionString))
            {
                con.Open();

                var adapter = new SqlCeDataAdapter(selectCmd, con);

                DataSet ds = new DataSet();
                adapter.FillSchema(ds, SchemaType.Source, Constants.UserTable);
                adapter.Fill(ds, Constants.UserTable);

                //Return null if no match was found
                if (ds.Tables[Constants.UserTable].Rows.Count == 0)
                    return null;

                curRow = ds.Tables[Constants.UserTable].Rows[0];

                con.Close();
            }

            return curRow;
        }

        /// <summary>
        /// Clear all contents of the database
        /// </summary>
        public static void ClearTable()
        {
            // ReSharper disable UnusedVariable
            string sqlSelectQuery = string.Format("SELECT * FROM {0}", Constants.UserTable);
            // ReSharper restore UnusedVariable

            using (var con = new SqlCeConnection(Properties.Settings.Default.DataStoreConnectionString))
            {
                con.Open();

                var cmd = new SqlCeCommand(string.Format("DELETE FROM {0}", Constants.UserTable), con);

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }

        #endregion

        #region Security Methods

        public static string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[1024];

            rng.GetBytes(buffer);
            return BitConverter.ToString(buffer);
        }

        public static string GenerateSaltedHash(string salt, string password)
        {
            var saltB =Encoding.UTF8.GetBytes(salt);
            var passwordB = Encoding.UTF8.GetBytes(password);

            var hmacSHA1 = new HMACSHA1(saltB);
            var hash = hmacSHA1.ComputeHash(passwordB);
            return Convert.ToBase64String(hash);
        }

        #endregion

    }
}
