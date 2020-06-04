using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Commands.SupplierSync
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
