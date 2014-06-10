using System.Windows;
using System.Windows.Controls;

namespace TFSFeedBuilder
{
    /// <summary>
    /// Interaction logic for TFView.xaml
    /// </summary>
    public partial class TFView
    {
        #region Private Members

        //BAD PROGRAMMER NO COOKIE
        private TFController controller;
        private TFModel model;

        #endregion


        public TFView()
        {
            InitializeComponent();

            //pwBox
            pwBox.PasswordChar = '*';
            pwBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            pwBox.VerticalContentAlignment = VerticalAlignment.Center;

            DoBadThings();
        }

        private void DoBadThings()
        {
            //Technically bad, but not seeing a lot of better alternatives when I'm lazy.
            model = new TFModel();
            controller = new TFController(this, model);
        }

        private void svBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //string tfsPath = "http://tfstta.int.thomson.com:8080/tfs";
            model.TFSPath = svBox.Text;
        }

        private void UnBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            model.Username = unBox.Text;
        }

        private void DnBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            model.Domain = dnBox.Text;
        }

        private void PwBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            model.Password = pwBox.Password;
        }

        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            controller.Run();
        }
    }
}
