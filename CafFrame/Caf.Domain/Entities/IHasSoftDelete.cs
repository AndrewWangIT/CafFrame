using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public interface IHasSoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedTime { get; set; }
    }
}
