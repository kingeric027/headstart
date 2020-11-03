using System;
using System.Collections.Generic;
using System.Text;
using Flurl.Http;

namespace Marketplace.Common.Exceptions
{
    public class ProcessResultException
    {
        public ProcessResultException(Exception ex)
        {
            this.Message = ex.Message;
            this.ResponseBody = ex.Message;
        }

        public ProcessResultException(FlurlHttpException ex)
        {
            this.Message = ex.Message;
            this.ResponseBody = ex.GetResponseJsonAsync().Result;
        }

        public string Message { get; set; }
        public dynamic ResponseBody { get; set; }
    }
}
