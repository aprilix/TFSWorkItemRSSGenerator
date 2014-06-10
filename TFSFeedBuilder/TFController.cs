namespace TFSFeedBuilder
{
    class TFController
    {
        private TFView _view;
        private TFModel _model;

        public TFController(TFView view, TFModel model)
        {
            //We pass in the view for if we ever wanted to do data binding...
            //But we don't.
            _view = view;
            _model = model;
        }

        public void Run()
        {
            //Five-step process to genning our feed.
            _model.ConnectToTFS();
            _model.GetWorkItemStore();
            _model.QueryStore();
            _model.BuildFeed();
            _model.OutputFile();
        }
    }
}
