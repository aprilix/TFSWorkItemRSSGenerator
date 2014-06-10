using System.ComponentModel;
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
        private string saveLoc;

        #endregion


        public TFView()
        {
            InitializeComponent();

            //pwBox
            pwBox.PasswordChar = '*';

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

            this.Close();
        }

        private void BtnBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = "Txt Files (*.xml)|*.xml",
                Title = "Save RSS Feed",
            };

            var result = dlg.ShowDialog();

            if (result != true)
                return;

            saveLoc = dlg.FileName;
            model.SavePath = saveLoc;

            //Set the lblSaveLoc field
            int fileNameLen = model.SavePath.Length;
            int subStart = 0;
            int maxLen = 30;
            lblSaveLoc.Content = "";

            if (fileNameLen >= maxLen)
            {
                subStart = fileNameLen - maxLen;
                lblSaveLoc.Content = "...";
            }

            lblSaveLoc.Content = lblSaveLoc.Content + model.SavePath.Substring(subStart, fileNameLen - subStart);
        }


        private void TFView_OnClosing(object sender, CancelEventArgs e)
        {
            //Could just use the model, but this handles the case of closing w/o touching any fields yet
            var serv = svBox.Text;
            var user = unBox.Text;
            var pw = pwBox.Password;
            var domain = dnBox.Text;

            if (!string.IsNullOrEmpty(serv) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pw)
                && !string.IsNullOrEmpty(domain) && !string.IsNullOrEmpty(saveLoc))
            {
                DAL.ClearTable(); //just in case; DB should only have 1 user in it
                DAL.AddUser(user, pw, domain, serv, saveLoc);
            }
        }
    }
}
