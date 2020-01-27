using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.FreightPop
{

    public class Response<TData>
    {
        public TData Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
