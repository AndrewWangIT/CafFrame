using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public class AuditingEntity<TKey, UserKey> : BaseEntity<TKey>, IHasCreationTime, IHasCreator<UserKey>, IHasLastModificationTime, IHasLastModifier<UserKey> where UserKey:struct
    {
        public DateTime CreationTime { get; set; }
        public UserKey? CreatorId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public UserKey? LastModifierId { get; set; }
    }

    public class AuditingEntity<UserKey>: AuditingEntity<long, UserKey> where UserKey : struct
    {

    }
    public class AuditingEntity : AuditingEntity<long>
    {

    }


}
