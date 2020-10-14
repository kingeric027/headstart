using System.Collections.Generic;
using Marketplace.Models.Exceptions;
using ordercloud.integrations.library;

namespace Marketplace.Models
{
    public class EmailDisplayText
    {
        public string EmailSubject { get; set; }
        public string DynamicText { get; set; }
        public string DynamicText2 { get; set; }
    }
}
