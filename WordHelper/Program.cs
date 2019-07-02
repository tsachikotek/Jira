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
            WordHelper.wordCreate word = new wordCreate();

            string jiraUrl = "https://jira.aligntech.com";
            string jiraUsername = args[0];
            string jiraPassword = args[1];
            string jiraQueryFilter = args[2];

            word.log("REQUEST...");

            word.log("Jira Url: " + jiraUrl);
            word.log("Jira User: " + jiraUsername);
            word.log("Jira Password: ******");
            word.log("Jira Filter: " + jiraQueryFilter);

            word.log(string.Format("Link: {0}/rest/api/2/search?jql=filter={1}", jiraUrl, jiraQueryFilter));

            JiraObject jira = new JiraObject(jiraUrl, jiraUsername, jiraPassword);
            JiraIssues issues = jira.getJiraIssues(jiraQueryFilter);
                        
            word.create(issues);
            
            System.Console.ReadLine();
        }

        
    }   
}
