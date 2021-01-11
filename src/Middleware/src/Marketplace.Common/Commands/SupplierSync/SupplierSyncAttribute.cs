using System;
using System.Collections.Generic;
using System.Text;

namespace Headstart.Common.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SupplierSyncAttribute : Attribute
    {
        public string SupplierID { get; set; }

        public SupplierSyncAttribute(string supplierID)
        {
            this.SupplierID = supplierID;
        }
    }
}
