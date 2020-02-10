using System;
using System.Runtime.Serialization;
using Flurl.Http;

namespace Marketplace.Common.Services.Zoho
{

    [Serializable]
    public class ZohoException : Exception
    {
        private Exception ex;
        public string payloadId;

        public ZohoException()
        {
        }

        public ZohoException(Exception ex)
        {
            this.ex = ex;
        }

        public ZohoException(string message) : base(message)
        {
        }

        public ZohoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ZohoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}