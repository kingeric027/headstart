using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.DevCenter
{
    public class AdminCompany
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public int OwnerDevID { get; set; }
        public object AutoForwardingUserID { get; set; }
    }
}
