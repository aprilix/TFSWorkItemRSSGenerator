using System.Windows;

namespace TFSFeedBuilder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var curUserCount = 0; //DAL.FetchUsersCount();

            if (e.Args.Length == 0 || curUserCount == 0 || (e.Args.Length != 0 && e.Args[0] == "-r"))
            {
                //do things with a gui
                if (e.Args.Length != 0 && e.Args[0] == "-r")
                    DAL.ClearTable();

                new TFView().ShowDialog();
            }
            else if (e.Args.Length != 0 && e.Args[0] == "-f" && curUserCount != 0)
            {
                //do things without a GUI
                var model = new TFModel();

                var userRow = DAL.FetchUser();

                //TODO: use salting and hashing
                //The below only works if you are inputting a pw from the command line
                //var tempSalt = userRow[Constants.SaltCol];
                //var storedPw = userRow[Constants.DbUsersTable].Rows[0][Constants.DbPassword];

                //if (DAL.GenerateSaltedHash(tempSalt, password) != storedPw)
                //{
                //    //ERROR OUT
                //    return;
                //}

                model.Username = (string) userRow[Constants.UserCol];
                model.Password = (string) userRow[Constants.PassCol]; //TODO: BAD
                model.Domain = (string) userRow[Constants.DomainCol];
                model.TFSPath = (string) userRow[Constants.TfsCol];
                model.SavePath = (string) userRow[Constants.SaveCol];

                //Five-step process to genning our feed.
                model.ConnectToTFS();
                model.GetWorkItemStore();
                model.QueryStore();
                model.BuildFeed();
                model.OutputFile();
            }

            Shutdown();
        }
    }
}
