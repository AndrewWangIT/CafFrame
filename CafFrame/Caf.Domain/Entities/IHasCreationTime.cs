using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public interface IHasCreationTime
    {
        /// <summary>
        /// Creation time.
        /// </summary>
        DateTime CreationTime { get; set; }
    }
}
