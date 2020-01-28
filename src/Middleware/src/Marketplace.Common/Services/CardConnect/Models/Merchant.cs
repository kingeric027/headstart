using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.CardConnect.Models
{
    /// <summary>
    /// https://developer.cardconnect.com/cardconnect-api?lang=json#inquire-merchant
    /// </summary>
    public class Merchant
    {
        public string site { get; set; }
        public string cardproc { get; set; }
        public bool enabled { get; set; }
        public string merchid { get; set; }
    }

}
