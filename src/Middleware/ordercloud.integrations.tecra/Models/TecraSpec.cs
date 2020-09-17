using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.tecra.Models
{
    public class TecraSpec
    {
        public string name { get; set; }
        public string dataType { get; set; }
        public string displayName { get; set; }
        public string displayValue { get; set; }
        public string required { get; set; }
        public string value { get; set; }
        public string visible { get; set; }
    }
}
