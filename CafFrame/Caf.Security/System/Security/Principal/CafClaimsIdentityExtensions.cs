using Caf.Claims;
using Caf.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace System.Security.Principal
{
    public static class CafClaimsIdentityExtensions
    {
        public static Guid? FindUserId(this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(userIdOrNull.Value);
        }

        public static Guid? FindUserId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var userIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(userIdOrNull.Value);
        }

        public static Guid? FindTenantId( this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var tenantIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.TenantId);
            if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(tenantIdOrNull.Value);
        }

        public static Guid? FindTenantId( this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var tenantIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.TenantId);
            if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(tenantIdOrNull.Value);
        }

        public static string FindClientId( this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var clientIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.ClientId);
            if (clientIdOrNull == null || clientIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return clientIdOrNull.Value;
        }

        public static string FindClientId( this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var clientIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.ClientId);
            if (clientIdOrNull == null || clientIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return clientIdOrNull.Value;
        }

        public static Guid? FindEditionId( this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var editionIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.EditionId);
            if (editionIdOrNull == null || editionIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(editionIdOrNull.Value);
        }

        public static Guid? FindEditionId( this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var editionIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == CafClaimTypes.EditionId);
            if (editionIdOrNull == null || editionIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(editionIdOrNull.Value);
        }
    }
}
