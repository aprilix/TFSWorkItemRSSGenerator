namespace TFSFeedBuilder
{
    class RssNode
    {
        //TODO: Serializability? C#'s built-in XML functionality?
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Link { get; private set; }

        public RssNode(string title, string desc, string link)
        {
            Title = title;
            Description = desc;
            Link = link;
        }
    }
}
