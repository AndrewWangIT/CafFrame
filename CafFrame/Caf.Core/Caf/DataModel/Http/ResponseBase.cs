using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DataModel.Http
{
    public class ResponseBase<T> : ResponseBase where  T:class
    {
        public T Data { get; set; }
    }

    public class ResponseBase
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
}
