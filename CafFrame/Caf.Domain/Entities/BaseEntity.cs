using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Caf.Domain.Entities
{
    public abstract class BaseEntity<TKey>: IHasSoftDelete
    {
        [Key]
        public virtual TKey Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedTime { get; set; }
    }
    public abstract class BaseEntity:BaseEntity<long>
    {

    }

}
