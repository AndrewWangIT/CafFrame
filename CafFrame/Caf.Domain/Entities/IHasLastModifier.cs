using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public interface IHasLastModifier<UserKey> where UserKey:struct
    {
        UserKey? LastModifierId { get; set; }
    }

    public interface IHasLastModifierWithReferenceTypeKey<UserKey> where UserKey : class
    {
        UserKey? LastModifierId { get; set; }
    }
}
