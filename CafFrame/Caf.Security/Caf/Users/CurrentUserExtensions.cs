using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Security.Users
{
    public static class CurrentUserExtensions
    {
        public static string FindClaimValue(this ICurrentUser currentUser, string claimType)
        {
            return currentUser.FindClaim(claimType)?.Value;
        }

        public static T FindClaimValue<T>(this ICurrentUser currentUser, string claimType)
            where T : struct
        {
            var value = currentUser.FindClaimValue(claimType);
            if (value == null)
            {
                return default;
            }

            return value.To<T>();
        }

        public static Guid GetId(this ICurrentUser currentUser)
        {

            return currentUser.Id.Value;
        }
    }
}
