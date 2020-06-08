using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public interface IEntity
    {
    }
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        TKey Id { get; }
    }
}
