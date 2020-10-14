using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Exceptions
{
    public class ProcessResultException
    {
        public ProcessResultException(Exception ex)
        {
            this.Message = ex.Message;
        }

        private string Message { get; set; }
    }
}
