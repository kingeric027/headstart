using System;
using System.Collections.Generic;
using System.Text;

namespace Headstart.Common.Services.AnytimeDashboard.Models
{
	public class AFGetStaffResponse
	{
        public string id { get; set; } // auto-incremented I think, e.g. 9381355
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string type { get; set; } // "Owner", "Staff", "Manager", "Trainer", "Regional Manager"
        public string language { get; set; } // "en-US", 
        public string email { get; set; }
        public string username { get; set; } // seems to match email
        public bool isDeleted { get; set; }
        public DateTime? updated { get; set; } // Not sure if how to use this yet, but seems like it might be helpful
        public List<AFClub> clubs { get; set; }
        public AFClub homeClub { get; set; }
    }
}
