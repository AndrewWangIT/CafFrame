using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DataModel.Http
{
    public class PagedResponse<T> where T : class
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public List<T> Datas { get; set; }
        public int Total { get; set; }
    }
}
