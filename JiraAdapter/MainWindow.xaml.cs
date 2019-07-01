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
using System.Windows.Threading;

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
                {
                    log("WorkingOn: " + _workingOn);
                    PropertyChanged(this, new PropertyChangedEventArgs("WorkingOn"));
                }
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
            Action<string> workMethod = (message) => log(message);

            try
            {
                int index = 1;
                log("OPENING...");
                wordDoc.Open(filename);
                log(filename + " IS OPEN");
                WorkingOn = "NUMBER OF ISSUES: " + issues.issues.Count.ToString();

                foreach (var jiraIssue in issues.issues)
                {                    
                    log("[" + index.ToString() + "/" + issues.issues.Count.ToString() + "] " + jiraIssue.key + " - " + jiraIssue.fields.summary);

                    wordDoc.AddIssue(jiraIssue);
                    index++;
                    WorkingOn = "[" + index.ToString() + "/" + issues.issues.Count.ToString() + "] " + jiraIssue.key + " - " + jiraIssue.fields.summary;
                    System.Threading.Thread.Sleep(100);
                    WorkerState = index;
                    //MessageBox.Show(index.ToString());                    
                }

                WorkingOn = "SAVING...";
                wordDoc.Save(filename);
                WorkingOn = "SAVED!";
            }
            catch (Exception exc)
            {   
                log(exc.Message);
                WorkingOn = exc.Message;
            }
        }

        private void log (string message)
        {
            logger.Dispatcher.BeginInvoke((Action)delegate () {
                int itemIndex = logger.Items.Add(message);
                logger.SelectedIndex = itemIndex;
                logger.ScrollIntoView(logger.SelectedItem);
            });
            
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            log("STARTING...");

            string jiraHome = ((TextBox)jiraUrl).Text;  //"https://jira.allot.com";
            string jiraUsername = ((TextBox)jiraUser).Text;  
            string jiraPassword = ((PasswordBox)jiraPass).Password.ToString();  
            string jiraQueryFolter = ((TextBox)jiraFilter).Text;  

            btnOpenFile.IsEnabled = false;

            WorkingOn = "CONNECTING TO JIRA...";
            JiraObject jira = new JiraObject(jiraHome, jiraUsername, jiraPassword);            
            JiraIssues issues = jira.getJiraIssues(jiraQueryFolter);
            WorkingOn = "RETRIEVED: " + issues.issues.Count + " issues";

            int max = issues.issues.Count;
            progressBar.Maximum = max;
            

            DataContext = this;

            _bgWorker = new BackgroundWorker(); //stopped the dowork being executed multiple times when button pressed again in the same session
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.WorkerSupportsCancellation = true;
            _bgWorker.DoWork += (s, x) =>
            {

                log("STARTING...");

                WorkingOn = "STARTING...";
                ExportJiraIssue(issues, file);

                log("DONE!!!");
                WorkingOn = "DONE!!!";
                //MessageBox.Show("Done!!!\r\nFile: " + file);
                WorkingOn = "COMPLETED!!!";
            };

            _bgWorker.ProgressChanged += worker_ProgressChanged;
            _bgWorker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _bgWorker.RunWorkerAsync();

            //_bgWorker.DoWork += (s, x) =>
            //{
            //    WorkingOn = "STARTING..." + WorkerState.ToString();
            //    ExportJiraIssue(issues, file);
            //    WorkingOn = "DONE!!!" + WorkerState.ToString();
            //    MessageBox.Show("Done!!!\r\nFile: " + file);
            //    WorkingOn = "COMPLETED!!!" + WorkerState.ToString();

            //};

            
            //_bgWorker.RunWorkerAsync();
            
        }
        
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            logger.Items.Add(e.ProgressPercentage);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnOpenFile.IsEnabled = true;
            _bgWorker.Dispose();
            logger.Items.Add("COMPLETED");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WorkingOn = "OPENNING...";
            logger.Items.Add("OPENNING: " +file);
            System.Diagnostics.Process.Start(file);
        }        
    }
}
