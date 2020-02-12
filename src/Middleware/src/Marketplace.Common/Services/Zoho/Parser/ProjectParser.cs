using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of ProjectsApi.
    /// </summary>
    class ProjectParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoProjectsList getProjectsList(HttpResponseMessage responce)
        {
            var projectList = new ZohoProjectsList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("projects"))
            {
                var projectsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["projects"].ToString());
                foreach(var projectObj in projectsArray)
                {
                    var project = new ZohoProject();
                    project = JsonConvert.DeserializeObject<ZohoProject>(projectObj.ToString());
                    projectList.Add(project);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                projectList.ZohoPageContext = pageContext;
            }
            return projectList;
        }

        internal static ZohoProject getProject(HttpResponseMessage responce)
        {
            var project = new ZohoProject();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("project"))
            {
                project = JsonConvert.DeserializeObject<ZohoProject>(jsonObj["project"].ToString());
            }
            return project;
        }

        internal static ZohoTaskList gatTaskList(HttpResponseMessage responce)
        {
            var taskList = new ZohoTaskList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("task"))
            {
                var tasksArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["task"].ToString());
                foreach(var taskObj in tasksArray)
                {
                    var task = new ZohoProjectTask();
                    task = JsonConvert.DeserializeObject<ZohoProjectTask>(taskObj.ToString());
                    taskList.Add(task);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                taskList.ZohoPageContext = pageContext;
            }
            return taskList;
        }

        internal static ZohoProjectTask gettask(HttpResponseMessage responce)
        {
            var task = new ZohoProjectTask();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("task"))
            {
                task = JsonConvert.DeserializeObject<ZohoProjectTask>(jsonObj["task"].ToString());
            }
            return task;
        }

        internal static ZohoUserList getUserList(HttpResponseMessage responce)
        {
            var userList = new ZohoUserList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("users"))
            {
                var usersArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["users"].ToString());
                foreach(var userObj in usersArray)
                {
                    var user = new ZohoUser();
                    user = JsonConvert.DeserializeObject<ZohoUser>(userObj.ToString());
                    userList.Add(user);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                userList.ZohoPageContext = pageContext;
            }
            return userList;
        }

        internal static ZohoUser getUser(HttpResponseMessage responce)
        {
            var user = new ZohoUser();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("user"))
            {
                user = JsonConvert.DeserializeObject<ZohoUser>(jsonObj["user"].ToString());
            }
            return user;
        }

        internal static ZohoTimeEntryList getTimeEntrieslist(HttpResponseMessage responce)
        {
            var timeEntryList = new ZohoTimeEntryList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("time_entries"))
            {
                var timeEntriesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["time_entries"].ToString());
                foreach(var timeEntryObj in timeEntriesArray)
                {
                    var timeEntry = new ZohoTimeEntry();
                    timeEntry = JsonConvert.DeserializeObject<ZohoTimeEntry>(timeEntryObj.ToString());
                    timeEntryList.Add(timeEntry);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                timeEntryList.ZohoPageContext = pageContext;
            }
            return timeEntryList;
        }

        internal static ZohoTimeEntry getTimeEntry(HttpResponseMessage responce)
        {
            var timeEntry = new ZohoTimeEntry();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("time_entry"))
            {
                timeEntry = JsonConvert.DeserializeObject<ZohoTimeEntry>(jsonObj["time_entry"].ToString());
            }
            return timeEntry;
        }
    }
}
