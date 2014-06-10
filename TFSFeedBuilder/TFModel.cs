using System;
using System.IO;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TFSFeedBuilder
{
    class TFModel
    {
        #region Private Members

        private TfsTeamProjectCollection tfs;
        private WorkItemStore wiStore;
        //TODO: make this a bit more dynamic?
        private string wiQuery = "Select [State], [Title], [Id] From WorkItems Where [Assigned to] = @Me Order By [Id] Asc";
        private WorkItemCollection queryResults;

        #endregion

        #region Public Members

        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        //string TFSPath = "http://tfstta.int.thomson.com:8080/tfs";
        public string TFSPath { get; set; }
        public string SavePath { get; set; }

        public string RssFeed { get; set; }

        #endregion

        #region Public Methods

        public NetworkCredential Creditials()
        {
            return new NetworkCredential(Username, Password, Domain);
        }

        public void ConnectToTFS()
        {
            // Connect to Team Foundation Server
            //     Server is the name of the server that is running the application tier for Team Foundation.
            //     Port is the port that Team Foundation uses. The default port is 8080.
            //     VDir is the virtual path to the Team Foundation application. The default path is tfs.
            var tfsUri = new Uri(TFSPath);

            var credentials = Creditials();

            // create the collection
            tfs = new TfsTeamProjectCollection(tfsUri, credentials);

            // check we are authenticated
            tfs.EnsureAuthenticated();
        }

        public void GetWorkItemStore()
        {
            // create the work item store
            wiStore = (WorkItemStore)tfs.GetService(typeof(WorkItemStore));
        }

        public void QueryStore()
        {
            queryResults = wiStore.Query(wiQuery);
        }


        public void BuildFeed()
        {
            //TODO: ACTUALLY GENERATE A RSS FEED HERE
            for (int i = 0; i < queryResults.Count; i++)
            {
                RssFeed += queryResults[i].Id + "\t" + queryResults[i].Title + "\n";
            }
        }

        public void OutputFile()
        {
            if (SavePath == null) return;

            //TODO: proper input mode?
            Stream fileStream = new FileStream(SavePath, FileMode.OpenOrCreate);
            var sw = new StreamWriter(fileStream);
            sw.Write(RssFeed);
            sw.Flush();
            sw.Close();
        }

        #endregion
    }
}
