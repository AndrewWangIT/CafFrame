using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public class AuditingEntityWithReferenceTypeUserKey<TKey, UserKey> : BaseEntity<TKey>, IHasCreationTime, IHasCreatorWithReferenceTypeKey<UserKey>, IHasLastModificationTime, IHasLastModifierWithReferenceTypeKey<UserKey> where UserKey : class
    {
        public DateTime CreationTime { get; set; }
        public UserKey? CreatorId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public UserKey? LastModifierId { get; set; }
    }
    public class AuditingEntityWithReferenceTypeUserKey<UserKey> : AuditingEntityWithReferenceTypeUserKey<long, UserKey> where UserKey : class
    {

    }
    public class AuditingEntityWithReferenceTypeUserKey : AuditingEntityWithReferenceTypeUserKey<string>
    {

    }
}
