using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using JiraHelper;
using WordHelper;

namespace WordHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string jiraUrl = "https://jira.allot.com";
            string jiraUsername = "tkotek@allot.com";
            string jiraPassword = "www222@@@";
            string jiraQueryFolter = "kotek";

            JiraObject jira = new JiraObject(jiraUrl, jiraUsername, jiraPassword);
            JiraIssues issues = jira.getJiraIssues(jiraQueryFolter);

            WordHelper.wordCreate word = new wordCreate();
            word.create(issues);
            
            System.Console.ReadLine();
        }

        
    }
}
