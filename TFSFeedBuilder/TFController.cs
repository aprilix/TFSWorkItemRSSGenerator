using System.Windows.Controls;
using System.Windows.Data;

namespace TFSFeedBuilder
{
    class TFController
    {
        private TFView _view;
        private TFModel _model;

        public TFController(TFView view, TFModel model)
        {
            _view = view;
            _model = model;
        }

        public void Run()
        {
            string query = "Select [State], [Title] From WorkItems Where [Assigned to] = @Me Order By [State] Asc, [Changed Date] Desc";

            _model.ConnectToTFS();
            _model.GetWorkItemStore();
            var result = _model.QueryStore(query);
            //TODO: do more things
        }
    }
}
