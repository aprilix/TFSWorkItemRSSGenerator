using System;
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

        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string TFSPath { get; set; }

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

        public WorkItemCollection QueryStore(string query)
        {
            if (wiStore == null)
                return null;

            return wiStore.Query(query);
        }


        #endregion
    }
}
