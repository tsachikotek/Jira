using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JiraHelper;
using WordHelper;
using System.ComponentModel;

namespace JiraAdapter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string file = @"C:\temp\test.doc";

        private BackgroundWorker _bgWorker = new BackgroundWorker();
        private int _workerState;
        private string _workingOn;

        public event PropertyChangedEventHandler PropertyChanged;

        public int WorkerState
        {
            get { return _workerState; }
            set
            {
                _workerState = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("WorkerState"));
            }

        }

        public string WorkingOn
        {
            get { return _workingOn; }
            set
            {
                _workingOn = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("WorkingOn"));
            }

        }

        public MainWindow()
        {
            InitializeComponent();

            //DataContext = this;
            //_bgWorker.DoWork += (s, e) =>
            //{
            //    for (int i = 0; i <= 100; i++)
            //    {
            //        System.Threading.Thread.Sleep(100);
            //        WorkerState = i;
            //    }
            //};

            //_bgWorker.RunWorkerAsync();           

        }

        

        private void ExportJiraIssue(JiraIssues issues, string filename)
        {
            wordCreate wordDoc = new wordCreate();

            int index = 0;

            wordDoc.Open();

            foreach (var jiraIssue in issues.issues)
            {

                //progressBar.Value = index;
                WorkingOn = "[" + index.ToString() + "/" + issues.issues.Count.ToString() + "] " + jiraIssue.key + " - " + jiraIssue.fields.summary;
                wordDoc.AddIssue(jiraIssue);
                index++;
                System.Threading.Thread.Sleep(100);
                WorkerState = index;
                //MessageBox.Show(index.ToString());
            }
            
            wordDoc.Save(filename);            
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            string jiraHome = ((TextBox)jiraUrl).Text;  //"https://jira.allot.com";
            string jiraUsername = ((TextBox)jiraUser).Text;  
            string jiraPassword = ((PasswordBox)jiraPass).Password.ToString();  
            string jiraQueryFolter = ((TextBox)jiraFilter).Text;  

            btnOpenFile.IsEnabled = false;

            JiraObject jira = new JiraObject(jiraHome, jiraUsername, jiraPassword);

            JiraIssues issues = jira.getJiraIssues(jiraQueryFolter);
            
            int max = issues.issues.Count;
            progressBar.Maximum = max;
            

            DataContext = this;
            _bgWorker.DoWork += (s, x) =>
            {
                WorkingOn = "STARTING...";
                ExportJiraIssue(issues, file);
                WorkingOn = "DONE!!!";
                MessageBox.Show("Done!!!\r\nFile: " + file);
            };

            btnOpenFile.IsEnabled = true;
            _bgWorker.RunWorkerAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(file);
        }
    }
}
