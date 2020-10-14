using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Text;


namespace ordercloud.integrations.tecra.Models
{
    [SwaggerModel]
    public class TecraDocument
    {
        public string id { get; set; }
        public string name { get; set; }
        public int pages { get; set; }
    }
}
