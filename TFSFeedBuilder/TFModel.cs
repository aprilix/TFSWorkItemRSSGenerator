using System;
using System.Collections.Generic;
using System.Globalization;
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

        private const string coll = "DefaultCollection";
        private const string workItemUrl = "_workItems#id=";
        private const string triageUrl = "&triage=true&_a=edit";

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
            List<RssNode> feedList = new List<RssNode>();

            for (int i = 0; i < queryResults.Count; i++)
            {
                WorkItem curItem = queryResults[i];
                string url = buildUrl(curItem);
                feedList.Add(new RssNode(curItem.Id.ToString(CultureInfo.InvariantCulture), curItem.Title, url));
            }

            GenHeader();

            GenFeed(feedList);

            GenFooter();
        }

        public void OutputFile()
        {
            if (SavePath == null) return;

            Stream fileStream = new FileStream(SavePath, FileMode.Create);
            var sw = new StreamWriter(fileStream);
            sw.Write(RssFeed);
            sw.Flush();
            sw.Close();
        }

        #endregion

        #region Private Methods

        private string buildUrl(WorkItem curItem)
        {
            //NOTE: not sure if this URL is really dynamic enough?
            string result = TFSPath + "/" + coll + "/" + webify(curItem.Project.Name) + "/" +
                                            workItemUrl + curItem.Id.ToString(CultureInfo.InvariantCulture) + triageUrl;

            return result;
        }

        private string webify(string link)
        {
            return link.Replace(" ", "%20");
        }

        private void GenHeader()
        {
            RssFeed = "<?xml version=\"1.0\" ?>\n" +
                                 "<rss version=\"2.0\">\n" +
                                 "<channel>\n\n";
        }

        private void GenFeed(List<RssNode> nodeList)
        {
            if (nodeList == null || nodeList.Count == 0)
                return;

            foreach (var node in nodeList)
            {
                RssFeed += "<item>\n";
                RssFeed += string.Format("\t<title>{0}</title>\n", node.Title);
                RssFeed += string.Format("\t<description>{0}</description>\n", node.Description);
                RssFeed += string.Format("\t<link>{0}</link>\n", node.Link);
                RssFeed += "</item>\n";
            }
        }

        private void GenFooter()
        {
            RssFeed += "\n</channel>\n" +
                                 "</rss>";
        }
        #endregion
    }
}