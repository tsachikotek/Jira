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
            string jiraUrl = "https://jira.aligntech.com";
            string jiraUsername = args[0];
            string jiraPassword = args[1];
            string jiraQueryFilter = args[2];

            Console.WriteLine("{0} {1} {2} {3}", jiraUrl, jiraUsername, jiraPassword, jiraQueryFilter);
            Console.WriteLine("ForTest: {0}/rest/api/2/search?jql=filter={1}", jiraUrl, jiraQueryFilter);

            JiraObject jira = new JiraObject(jiraUrl, jiraUsername, jiraPassword);
            JiraIssues issues = jira.getJiraIssues(jiraQueryFilter);

            WordHelper.wordCreate word = new wordCreate();
            word.create(issues);
            
            System.Console.ReadLine();
        }

        
    }   
}
