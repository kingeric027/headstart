using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.AnytimeDashboard.Models
{
    // See https://api.anytimefitness.com/Help/Api/GET-clubs for the origin of this model.
    public class AFClub
    {
        public string id { get; set; }
        public string clubGuid { get; set; }
        public string afNumber { get; set; }
        public string billingNumber { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string legalEntity { get; set; }
        public string primaryContactName { get; set; }
        public AFAddress address { get; set; }
        public Coordinates coordinates { get; set; }
        public AFLocationStatus status { get; set; } 
        public DateTime? openingDate { get; set; }
        public bool isDeleted { get; set; } // Not sure how/if to use this yet.

        public class AFAddress
        {
            public string city { get; set; }
            public string stateProvince { get; set; }
            public string postCode { get; set; }
            public string address { get; set; }
            public string address2 { get; set; }
            public string country { get; set; }
        }

        public class AFLocationStatus
        {
            public string id { get; set; }
            public string description { get; set; }
        }
    }
}
