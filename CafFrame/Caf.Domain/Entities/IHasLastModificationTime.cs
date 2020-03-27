using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public interface IHasLastModificationTime
    {
        public DateTime? LastModificationTime { get; set; }
    }
}
