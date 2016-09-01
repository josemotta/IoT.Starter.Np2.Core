using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace FileClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string[] _remoteFiles = null;
        public string[] RemoteFiles
        {
            get
            {
                return _remoteFiles;
            }
            private set
            {
                _remoteFiles = value;
                OnPropertyChanged("RemoteFiles");
            }
        }

        private string _selectedRemoteFile = null;
        public string SelectedRemoteFile
        {
            get
            {
                return _selectedRemoteFile;
            }
            set
            {
                _selectedRemoteFile = value;
                OnPropertyChanged("SelectedRemoteFile");
            }
        }

        private string _remoteDirectory = "";
        public string RemoteDirectory
        {
            get
            {
                return _remoteDirectory;
            }
            set
            {
                _remoteDirectory = value;
                OnPropertyChanged("RemoteDirectory");
            }
        }

        private string[] _localFiles = null;
        public string[] LocalFiles
        {
            get
            {
                return _localFiles;
            }
            private set
            {
                _localFiles = value;
                OnPropertyChanged("LocalFiles");
            }
        }

        private string _selectedLocalFile = null;
        public string SelectedLocalFile
        {
            get
            {
                return _selectedLocalFile;
            }
            set
            {
                _selectedLocalFile = value;
                 OnPropertyChanged("SelectedLocalFile");
            }
        }

        private string _localDirectory = @"C:\Users\jo\Desktop\local";
        public string LocalDirectory
        {
            get
            {
                return _localDirectory;
            }
            set
            {
                _localDirectory = value;
                OnPropertyChanged("LocalDirectory");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshRemoteFiles();
            RefreshLocalFiles();
        }

        private void RefreshRemoteFiles()
        {
            string[] files = null;
            bool success = Client.List(RemoteDirectory, out files);
            if (success)
            {
                RemoteFiles = files;
            }
        }

        private void RefreshLocalFiles()
        {
            LocalFiles = Directory.GetFiles(LocalDirectory).Select(x => x.Substring(x.LastIndexOf('\\') + 1)).ToArray();
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            bool success = Client.Receive(RemoteDirectory + @"\" + SelectedRemoteFile, LocalDirectory + @"\" + SelectedRemoteFile);
            RefreshLocalFiles();
        }

        private void Put_Click(object sender, RoutedEventArgs e)
        {
            bool success = Client.Send(LocalDirectory + @"\" + SelectedLocalFile);
            RefreshRemoteFiles();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        private void ListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                bool success = Client.Delete(RemoteDirectory + @"\" + SelectedRemoteFile);
                RefreshRemoteFiles();
            }
        }
    }
}
