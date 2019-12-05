using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.DevCenter
{
    public class Organization
    {
        public bool Active { get; set; }
        public string BuyerApiClientName { get; set; }
        public string BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPassword { get; set; }
        public string BuyerUserName { get; set; }
        public string Name { get; set; }
        public string SellerApiClientName { get; set; }
        public string SellerPassword { get; set; }
        public string SellerUserName { get; set; }
    }
}
