using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public class History
	{
		public DateTimeOffset DateCreated { get; set; }
		public string CreatedByUserID { get; set; }
		public DateTimeOffset DateUpdated { get; set; }
		public string UpdatedByUserID { get; set; }
	}

	public static class HistoryBuilder
	{
		public static History OnCreate(VerifiedUserContext user)
		{
			return new History()
			{
				DateCreated = DateTimeOffset.Now,
				CreatedByUserID = user.UserID,
				DateUpdated = DateTimeOffset.Now,
				UpdatedByUserID = user.UserID
			};
		}

		public static History OnUpdate(History history, VerifiedUserContext user)
		{
			history.DateUpdated = DateTimeOffset.Now;
			history.UpdatedByUserID = user.UserID;
			return history;
		}
	}
}
