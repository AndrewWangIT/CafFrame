using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.Entities
{
    public interface IHasCreator<UserKey> where UserKey : struct
    {
        [CanBeNull]
        //UserKey 
        UserKey? CreatorId { get; set; }
    }

    public interface IHasCreatorWithReferenceTypeKey<UserKey> where UserKey : class
    {
        [CanBeNull]
        //UserKey 
        UserKey? CreatorId { get; set; }
    }
}
