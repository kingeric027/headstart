using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.tecra.Models
{
    public class TecraDocumentRequest
    {
        public string chiliurl { get; set; }
        public string environment { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string folder { get; set; }
    }
    public class TecraFrameParams
    {
        public string docid { get; set; }
        public string wsid { get; set; }
        public string vpid { get; set; }
        public string folder { get; set; }
        public string storeid { get; set; }
    }

}
