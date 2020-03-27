using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DataModel.Http
{
    public class PagedRepuestInput
    {
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
