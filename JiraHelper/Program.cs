using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JiraHelper
{
    public class Program
    {
        static void Main(string[] args)
        {
            string jiraUrl = "https://jira.allot.com";
            string jiraUsername = "tkotek@allot.com";
            string jiraPassword = "www222@@@";
            string jiraQueryFolter = "kotek";

            JiraObject jira = new JiraObject(jiraUrl, jiraUsername, jiraPassword);
            jira.getJiraIssues(jiraQueryFolter);
            System.Console.ReadLine();
        }

        
    }
}
