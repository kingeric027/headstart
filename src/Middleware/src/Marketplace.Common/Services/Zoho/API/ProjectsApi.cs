using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Parser;
using Marketplace.Common.Services.Zoho.Util;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.API
{
    /// <summary>
    /// Class ProjectsApi is used to <br></br>
    ///     Create a projects for the customer,<br></br>
    ///     Get the List of projects,tasks,users,time entries,comments or invoices created for the project,<br></br>
    ///     Get the details of the specified project,task,user or time entry,<br></br>
    ///     Mark the project as active or inactive,<br></br>
    ///     Clone the project,<br></br>
    ///     Add the task and assinging users to the project,<br></br>
    ///     Items time entries and start or stop the time tracking,<br></br>
    ///     Add or delete a comment,<br></br>
    ///     Update the details of the project,task,user or time entry,<br></br>
    ///     Delete the specified project,task,user or time entry.
    /// </summary>
    public class ProjectsApi:Api
    {
        static string baseAddress = baseurl + "/projects";
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsApi" /> class.
        /// </summary>
        /// <param name="auth_token">The auth_token is used for the authentication purpose.</param>
        /// <param name="organization_Id">The organization_ identifier is used to define the current working organisation.</param>
        public ProjectsApi(string auth_token, string organization_Id)
            : base(auth_token, organization_Id)
        {

        }
        /// <summary>
        /// List all projects with pagination.
        /// </summary>
        /// <param name="parameters">The parameters is the Dictionary object which contains the filters in the form of key, value pair to refine the list.<br></br>The possible filters are listed below<br></br>
        /// <table>
        /// <tr><td>filter_by</td><td>Filter projects by any status<br></br>Allowed Values: <i>FourOverStatus.All, FourOverStatus.Active</i> and <i>FourOverStatus.Inactive</i></td></tr>
        /// <tr><td>customer_id</td><td>Search projects by customer id.</td></tr>
        /// <tr><td>sort_column</td><td>Sort projects<br></br>Allowed Values: <i>project_name, customer_name, rate</i> and <i>created_time</i></td></tr>
        /// </table>
        /// </param>
        /// <returns>ZohoProjectsList object.</returns>
        public ZohoProjectsList GetProjects(Dictionary<object, object> parameters)
        {
            string url = baseAddress;
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return ProjectParser.getProjectsList(responce);
        }
        /// <summary>
        /// Get the details of a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <returns>ZohoProject object.</returns>
        public ZohoProject Get(string project_id)
        {
            string url = baseAddress+"/"+project_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.getProject(responce);
        }
        /// <summary>
        /// Creates a project for the customer.
        /// </summary>
        /// <param name="newZohoProjectInfo">The newZohoProjectInfo is the Projrct object with project_name,customer_id,billing_type,user_id,task_name as mandatory attributes.</param>
        /// <returns>ZohoProject object.</returns>
        public ZohoProject Create(ZohoProject newZohoProjectInfo)
        {
            string url = baseAddress;
            var json = JsonConvert.SerializeObject(newZohoProjectInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ProjectParser.getProject(responce);
        }
        /// <summary>
        /// Update details of a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="update_info">The update_info is the ZohoProject object which contains the updation information.</param>
        /// <returns>ZohoProject object.</returns>
        public ZohoProject Update(string project_id, ZohoProject update_info)
        {
            string url = baseAddress + "/" + project_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return ProjectParser.getProject(responce);
        }
        /// <summary>
        /// Deletes the existing project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <returns>System.String.<br></br>The success message is "The project has been deleted".</returns>
        public string Delete(string project_id)
        {
            string url = baseAddress + "/" + project_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return ProjectParser.getMessage(responce);
        }
        /// <summary>
        /// Marks project as active.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <returns>System.String.<br></br>The success message is "The selected Projects have been marked as active."</returns>
        public string Activate(string project_id)
        {
            string url = baseAddress + "/" + project_id + "/active";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ProjectParser.getMessage(responce);
        }
        /// <summary>
        /// Marks a project as inactive.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <returns>System.String.<br></br>The success message is "The selected projects have been marked as inactive."</returns>
        public string Inactivate(string project_id)
        {
            string url = baseAddress + "/" + project_id + "/inactive";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ProjectParser.getMessage(responce);
        }
        /// <summary>
        /// Clones the project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="clone_details">The clone_details is the ZohoProject object which contains the project details which has to be cloned with project_name as mandatory attribute.</param>
        /// <returns>ZohoProject object.</returns>
        public ZohoProject Clone(string project_id, ZohoProject clone_details)
        {
            string url = baseAddress + "/" + project_id + "/clone";
            var json = JsonConvert.SerializeObject(clone_details);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ProjectParser.getProject(responce);
        }
//--------------------------------------------------------------------------------------

        /// <summary>
        /// Get list of tasks added to a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="parameters">The parameters is the Dictionary object which contains the filters in the form of key,value pair to refine the list.<br></br>The possible filters are listed below<br></br>
        /// <table>
        /// <tr><td>sort_column</td><td>Sort tasks<br></br>Allowed Values: <i>task_name, billed_hours, log_time</i> and <i>un_billed_hours</i></td></tr>
        /// </table>
        /// </param>
        /// <returns>TasksList object.</returns>
        public ZohoTaskList GetTasks(string project_id, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + project_id + "/tasks";
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return ProjectParser.gatTaskList(responce);
        }

        /// <summary>
        /// Gets the details of a task.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="task_id">The task_id is the identifier of the task of specified project.</param>
        /// <returns>ZohoProjectTask object.</returns>
        public ZohoProjectTask GetATask(string project_id, string task_id)
        {
            string url = baseAddress + "/" + project_id + "/tasks/"+task_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.gettask(responce);
        }

        /// <summary>
        /// Adds a task to the project.
        /// </summary>
        /// <param name="project_id">The project_id is the idntifier of the project.</param>
        /// <param name="new_task_info">The new_task_info is the ZohoProjectTask object with task_name as mandatory attribute.</param>
        /// <returns>ZohoProjectTask object.</returns>
        public ZohoProjectTask AddATask(string project_id, ZohoProjectTask new_task_info)
        {
            string url = baseAddress + "/" + project_id + "/tasks";
            var json = JsonConvert.SerializeObject(new_task_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ProjectParser.gettask(responce);
        }

        /// <summary>
        /// Updates the details of a task.
        /// </summary>
        /// <param name="project_id">The project_id is the idntifier of the project.</param>
        /// <param name="task_id">The task_id is the identifier of the task of specified project.</param>
        /// <param name="update_info">The update_info is the ZohoProjectTask object which contains the updation information.</param>
        /// <returns>ZohoProjectTask object.</returns>
        public ZohoProjectTask UpdateTask(string project_id, string task_id, ZohoProjectTask update_info)
        {
            string url = baseAddress + "/" + project_id + "/tasks/"+task_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return ProjectParser.gettask(responce);
        }
        /// <summary>
        /// Deletes the task added to the project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="task_id">The task_id is the identifier of the task of specified project.</param>
        /// <returns>System.String.<br></br>The success message is "The task has been deleted."</returns>
        public string DeleteTask(string project_id, string task_id)
        {
            string url = baseAddress + "/" + project_id + "/tasks/"+task_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return ProjectParser.getMessage(responce);
        }
//------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get list of user associated with a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <returns>List of ZohoUser objects.</returns>
        public ZohoUserList GetUsers(string project_id)
        {
            string url = baseAddress + "/" + project_id + "/users";
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.getUserList(responce);
        }
        /// <summary>
        /// Get details of a user in a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="user_id">The user_id is the identifier of the user who is associated with the specified project.</param>
        /// <returns>ZohoUser object.</returns>
        public ZohoUser GetAUser(string project_id, string user_id)
        {
            string url = baseAddress + "/" + project_id + "/users/" + user_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.getUser(responce);
        }
        /// <summary>
        /// Assigns the users to a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="zohoUsersToAssign">The zohoUsersToAssign is the ZohoUsersToAssign object with user_id as mandatory attribute.</param>
        /// <returns>List of ZohoUser objects.</returns>
        public ZohoUserList AssignUsers(string project_id, ZohoUsersToAssign zohoUsersToAssign)
        {
            string url = baseAddress + "/" + project_id + "/users";
            var json = JsonConvert.SerializeObject(zohoUsersToAssign);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ProjectParser.getUserList(responce);
        }
        /// <summary>
        /// Invite and add user to the project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="zohoUserInfo">The zohoUserInfo is the ZohoUser object with user_name and email as mandatory attributes.</param>
        /// <returns>ZohoUser object.</returns>
        public ZohoUser InviteUser(string project_id, ZohoUser zohoUserInfo)
        {
            string url = baseAddress + "/" + project_id + "/users/invite";
            var json = JsonConvert.SerializeObject(zohoUserInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ProjectParser.getUser(responce);
        }
        /// <summary>
        /// Updates the details of a user.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="user_id">The user_id is the identifier of the user who is associated with the specified project.</param>
        /// <param name="update_info">The update_info is the ZohoUser object which contains the updation information.</param>
        /// <returns>ZohoUser object.</returns>
        public ZohoUser UpdateUser(string project_id, string user_id, ZohoUser update_info)
        {
            string url = baseAddress + "/" + project_id + "/users/" + user_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return ProjectParser.getUser(responce);
        }
        /// <summary>
        /// Remove user from the project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="user_id">The user_id is the identifier of the user who is associated with the specified project.</param>
        /// <returns>System.String.<br></br>The success message is "The staff has been removed"</returns>
        public string DeleteUser(string project_id, string user_id)
        {
            string url = baseAddress + "/" + project_id + "/users/" + user_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return ProjectParser.getMessage(responce);
        }
//------------------------------------------------------------------------------------------------------
        /// <summary>
        /// List all time entries with pagination.
        /// </summary>
        /// <param name="parameters">The parameters is the Dictionary object which contains the filters in the form of key,value pair to refine the list.<br></br>The possible filters are listed below<br></br>
        /// <table>
        /// <tr><td>from_date</td><td>Date from which the time entries logged to be fetched</td></tr>
        /// <tr><td>to_date</td><td>Date up to which the time entries logged to be fetched</td></tr>
        /// <tr><td>filter_by</td><td>Filter time entries by date and status.<br></br>Allowed Values: <i>Date.All, Date.Today, Date.ThisWeek, Date.ThisMonth, Date.ThisQuarter, Date.ThisYear, Date.PreviousDay, Date.PreviousWeek, Date.PreviousMonth, Date.PreviousQuarter, Date.PreviousYear, Date.CustomDate, FourOverStatus.Unbilled</i> and <i>FourOverStatus.Invoiced</i></td></tr>
        /// <tr><td>project_id</td><td>Search time entries by project_id.</td></tr>
        /// <tr><td>user_id</td><td>Search time entries by user_id.</td></tr>
        /// <tr><td>sort_column</td><td>Sort time entries.<br></br>Allowed Values: <i>project_name, task_name, user_name, log_date, timer_started_at</i> and <i>customer_name</i></td></tr>
        /// </table>
        /// </param>
        /// <returns>TimeEntrisList object.</returns>
        public ZohoTimeEntryList GetTimeEnries(Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/timeentries";
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return ProjectParser.getTimeEntrieslist(responce);
        }
        /// <summary>
        /// Get details of a time entry.
        /// </summary>
        /// <param name="time_entry_id">The time_entry_id is the identifier of the time entry.</param>
        /// <returns>ZohoTimeEntry object.</returns>
        public ZohoTimeEntry GetATimeEntry(string time_entry_id)
        {
            string url = baseAddress + "/timeentries/"+time_entry_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.getTimeEntry(responce);
        }
        /// <summary>
        /// Logs the time entries.
        /// </summary>
        /// <param name="zohoTimeEntryInfo">The zohoTimeEntryInfo is the ZohoTimeEntry object with project_id,task_id,user_id and log_date as mandatory attributes.</param>
        /// <returns>ZohoTimeEntry object.</returns>
        public ZohoTimeEntry LogTimeEntry(ZohoTimeEntry zohoTimeEntryInfo)
        {
            string url = baseAddress + "/timeentries";
            var json = JsonConvert.SerializeObject(zohoTimeEntryInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ProjectParser.getTimeEntry(responce);
        }
        /// <summary>
        /// Updates the logged time entry.
        /// </summary>
        /// <param name="time_entry_id">The time_entry_id is the identifier of the time entry.</param>
        /// <param name="update_info">The update_info is the ZohoTimeEntry object which is having the upadation information.</param>
        /// <returns>ZohoTimeEntry object.</returns>
        public ZohoTimeEntry UpdateTimeEntry(string time_entry_id, ZohoTimeEntry update_info)
        {
            string url = baseAddress + "/timeentries/"+time_entry_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return ProjectParser.getTimeEntry(responce);
        }
        /// <summary>
        /// Deletes the logged time entry.
        /// </summary>
        /// <param name="time_entry_id">The time_entry_id is the identifier of the time entry.</param>
        /// <returns>System.String.<br></br>The success message is "The time entry has been deleted" </returns>
        public string DeleteTimeEntry(string time_entry_id)
        {
            string url = baseAddress + "/timeentries/"+time_entry_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return ProjectParser.getMessage(responce);
        }
        /// <summary>
        /// Deletes the logged time entries.
        /// </summary>
        /// <param name="parameters">The parameters is the dictionary object which contains the following mandatory parameter in the key,value pair format.<br></br>
        /// <table><tr><td>time_entry_ids*</td><td>IDs of the time entries to be deleted.</td></tr></table>
        /// </param>
        /// <returns>System.String.<br></br>The success message is "The selected timesheet entries have been deleted"</returns>
        public string DeleteTimeEntries(Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/timeentries";
            var responce = ZohoHttpClient.delete(url, getQueryParameters(parameters));
            return ProjectParser.getMessage(responce);
        }
        /// <summary>
        /// Get current running timer.
        /// </summary>
        /// <returns>ZohoTimeEntry object.</returns>
        public ZohoTimeEntry GetTimer()
        {
            string url = baseAddress + "/timeentries/runningtimer/me";
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.getTimeEntry(responce);
        }
        /// <summary>
        /// Start tracking time spent.
        /// </summary>
        /// <param name="time_entry_id">The time_entry_id is the identifier of the time entry.</param>
        /// <returns>ZohoTimeEntry object.</returns>
        public ZohoTimeEntry StartTimer(string time_entry_id)
        {
            string url = baseAddress + "/timeentries/" + time_entry_id + "/timer/start";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ProjectParser.getTimeEntry(responce);
        }
        /// <summary>
        /// EchoStop tracking time, say taking a break or leaving.
        /// </summary>
        /// <returns>ZohoTimeEntry object.</returns>
        public ZohoTimeEntry StopTimer()
        {
            string url = baseAddress + "/timeentries/timer/stop";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ProjectParser.getTimeEntry(responce);
        }
 //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get comments for a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <returns>List of ZohoComment objects.</returns>
        public ZohoCommentList GetComments(string project_id)
        {
            string url = baseAddress + "/" + project_id + "/comments";
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return CreditNoteParser.getCommentList(responce);
        }
        /// <summary>
        /// Post comment to a project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="zohoCommentInfo">The zohoCommentInfo is the ZohoComment object with description as mandatory attribute.</param>
        /// <returns>ZohoComment object.</returns>
        public ZohoComment AddComment(string project_id, ZohoComment zohoCommentInfo)
        {
            string url = baseAddress + "/" + project_id + "/comments";
            var json = JsonConvert.SerializeObject(zohoCommentInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return CreditNoteParser.getComment(responce);
        }
        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="comment_id">The comment_id is the identifier of the comment for the specified project.</param>
        /// <returns>System.String.<br></br>The success message is "The comment has been deleted."</returns>
        public string DeleteComment(string project_id, string comment_id)
        {
            string url = baseAddress + "/" + project_id + "/comments/"+comment_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return ProjectParser.getMessage(responce);
        }
//------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Lists invoices created for this project.
        /// </summary>
        /// <param name="project_id">The project_id is the identifier of the project.</param>
        /// <param name="parameters">The parameters is the Dictionary object which contains the following filters in the form of key,value pair to refine the list.<br></br>
        /// <table>
        /// <tr><td>sort_column</td><td>Sort invoices raised.<br></br>Allowed Values: <i>invoice_number, date, total, balance</i> and <i>created_time</i></td></tr>
        /// </table>
        /// </param>
        /// <returns>ZohoInvoicesList.</returns>
        public ZohoInvoicesList GetInvoices(string project_id, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + project_id + "/invoices";
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return InvoiceParser.getInvoiceList(responce);
        }
    }
}
