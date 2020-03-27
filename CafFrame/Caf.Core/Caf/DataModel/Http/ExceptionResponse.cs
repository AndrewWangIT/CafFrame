using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DataModel.Http
{
    public class ExceptionResponse: ResponseBase
    {
        public string ExceptionDetail { get; set; }
    }
}
