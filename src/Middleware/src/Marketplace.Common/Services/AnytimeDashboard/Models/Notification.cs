using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Headstart.Common.Services.AnytimeDashboard.Models
{
	// https://api.anytimefitness.com/Help/Webhooks
	public class Notification
	{
		public NotificationChannel Channel { get; set; } // the type of entity that changed
		public NotificationAction Action { get; set; } // the CRUD action that was performed
		public string TimestampUtc { get; set; } // the time the action was performed
		public string JsonData { get; set; } // json formatted string

		public T GetData<T>() => JsonConvert.DeserializeObject<T>(JsonData);
	}

	public enum NotificationChannel { Club, Staff }
	public enum NotificationAction { Update, Delete, Create }
}
