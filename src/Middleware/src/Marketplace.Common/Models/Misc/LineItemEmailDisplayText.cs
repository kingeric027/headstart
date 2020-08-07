using System.Collections.Generic;
using Marketplace.Models.Exceptions;
using ordercloud.integrations.library;

namespace Marketplace.Models
{
    public class LineItemEmailDisplayText
    {
        public string EmailSubject { get; set; }
        public string StatusChangeDetail { get; set; }
        public string StatusChangeDetail2 { get; set; }
    }
}
