using Caf.Claims;
using Caf.Core.DependencyInjection;
using Caf.Security.Claims;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Caf.Security.Users
{
    public class CurrentUser : ICurrentUser, ITransient
    {
        private static readonly Claim[] EmptyClaimsArray = new Claim[0];

        public virtual bool IsAuthenticated => Id.HasValue;

        public virtual Guid? Id => _principalAccessor.Principal?.FindUserId();

        public virtual string UserName => this.FindClaimValue(CafClaimTypes.UserName);

        public virtual string PhoneNumber => this.FindClaimValue(CafClaimTypes.PhoneNumber);

        public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(CafClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);

        public virtual string Email => this.FindClaimValue(CafClaimTypes.Email);

        public virtual string[] Roles => FindClaims(CafClaimTypes.Role).Select(c => c.Value).ToArray();

        private readonly ICurrentPrincipalAccessor _principalAccessor;

        public CurrentUser(ICurrentPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        public virtual Claim FindClaim(string claimType)
        {
            return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
        }

        public virtual Claim[] FindClaims(string claimType)
        {
            return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
        }

        public virtual Claim[] GetAllClaims()
        {
            return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
        }

        public virtual bool IsInRole(string roleName)
        {
            return FindClaims(CafClaimTypes.Role).Any(c => c.Value == roleName);
        }
    }
}
