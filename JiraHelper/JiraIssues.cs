using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;
using RestSharp;

namespace JiraHelper
{
    public class JiraObject
    {
        public Jira jiraConn { get; set; }

        public JiraObject(string jiraUrl, string jiraUsername, string jiraPassword)
        {
            //jiraConn = Jira.CreateRestClient("https://jira.allot.com", "tkotek@allot.com", "www222@@@");
            jiraConn = Jira.CreateRestClient(jiraUrl, jiraUsername, jiraPassword);
        }

        public JiraIssues getJiraIssues(string filter)
        {
            // create a connection to JIRA using the Rest client
            //var jiraConn = Jira.CreateRestClient("https://jira.allot.com", "tkotek@allot.com", "www222@@@");

            jiraConn.Issues.MaxIssuesPerRequest = int.MaxValue;
            var jiraRestClient = jiraConn.RestClient;

            RestRequest reqGetFilter = new RestRequest();
            //reqGetFilter.Resource = "https://jira.allot.com/rest/api/2/search?jql=filter=kotek";
            reqGetFilter.Resource = "https://jira.allot.com/rest/api/2/search";

            //reqGetFilter.Parameters.Add(new Parameter() { Name="jql", Value="filter=CI", Type=ParameterType.QueryString });
            reqGetFilter.Parameters.Add(new Parameter() { Name = "jql", Value = "filter=" + filter, Type = ParameterType.QueryString });
            reqGetFilter.Parameters.Add(new Parameter() { Name = "maxResults", Value = int.MaxValue, Type = ParameterType.QueryString });

            reqGetFilter.Method = Method.GET;

            var res = jiraRestClient.RestSharpClient.ExecuteAsGet(reqGetFilter, "Get");

            //JiraIssues

            var jiraIssues = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraIssues>(res.Content);

            //res.Content

            System.Console.WriteLine("Content: " + res.Content.ToString());


            // use LINQ syntax to retrieve issues
            //var jiraIssues = from i in jiraConn.Issues.Queryable
            //             where i.Assignee == "Rabi Rabi"
            //             orderby i.Created
            //             select i;

            System.Console.WriteLine("Issues Found: " + jiraIssues.issues.Count());

            foreach (var jIssue in jiraIssues.issues)
            {
                System.Console.WriteLine(jIssue.key + " -- " + jIssue.fields.description);
            }

            //System.Console.ReadLine();

            return jiraIssues;
        }
    }


    public class JiraIssues
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<Issue> issues { get; set; }
    }

    public class Issuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
    }

    public class AvatarUrls
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }

    public class Project
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public AvatarUrls avatarUrls { get; set; }
    }

    public class Resolution
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }

    public class Watches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }

    public class Priority
    {
        public string self { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class AvatarUrls2
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }

    public class Assignee
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls2 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class StatusCategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class Status
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public StatusCategory statusCategory { get; set; }
    }

    public class AvatarUrls3
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }

    public class Creator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls3 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class AvatarUrls4
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }

    public class Reporter
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls4 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Aggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Progress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class Fields
    {
        public Issuetype issuetype { get; set; }
        public object timespent { get; set; }
        public Project project { get; set; }
        public List<object> fixVersions { get; set; }
        public object aggregatetimespent { get; set; }
        public Resolution resolution { get; set; }
        public object customfield_11401 { get; set; }
        public object customfield_11203 { get; set; }
        public object customfield_11400 { get; set; }
        public object customfield_10104 { get; set; }
        public object customfield_10500 { get; set; }
        public double customfield_11314 { get; set; }
        public string customfield_10105 { get; set; }
        public object customfield_10501 { get; set; }
        public object customfield_11315 { get; set; }
        public object customfield_10502 { get; set; }
        public object customfield_11316 { get; set; }
        public object customfield_10503 { get; set; }
        public object customfield_11317 { get; set; }
        public object customfield_10504 { get; set; }
        public object customfield_11318 { get; set; }
        public DateTime? resolutiondate { get; set; }
        public int workratio { get; set; }
        public DateTime? lastViewed { get; set; }
        public Watches watches { get; set; }
        public DateTime created { get; set; }
        public string customfield_10100 { get; set; }
        public Priority priority { get; set; }
        public List<string> labels { get; set; }
        public object customfield_11313 { get; set; }
        public object customfield_11303 { get; set; }
        public object customfield_11307 { get; set; }
        public object timeestimate { get; set; }
        public object aggregatetimeoriginalestimate { get; set; }
        public List<object> versions { get; set; }
        public object customfield_11308 { get; set; }
        public object customfield_11309 { get; set; }
        public List<object> issuelinks { get; set; }
        public Assignee assignee { get; set; }
        public DateTime updated { get; set; }
        public Status status { get; set; }
        public List<object> components { get; set; }
        public string customfield_11140 { get; set; }
        public object timeoriginalestimate { get; set; }
        public string description { get; set; }
        public object customfield_11301 { get; set; }
        public object customfield_10203 { get; set; }
        public object aggregatetimeestimate { get; set; }
        public string summary { get; set; }
        public Creator creator { get; set; }
        public List<object> subtasks { get; set; }
        public Reporter reporter { get; set; }
        public Aggregateprogress aggregateprogress { get; set; }
        public object customfield_10200 { get; set; }
        public object customfield_10201 { get; set; }
        public object customfield_10202 { get; set; }
        public object customfield_11204 { get; set; }
        public object environment { get; set; }
        public object duedate { get; set; }
        public Progress progress { get; set; }
        public Votes votes { get; set; }
    }

    public class Issue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields fields { get; set; }
    }

   
}
