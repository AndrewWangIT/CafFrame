using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Caf.Security.Users
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        Guid? Id { get; }

        string UserName { get; }

        string PhoneNumber { get; }

        bool PhoneNumberVerified { get; }

        string Email { get; }

        string[] Roles { get; }

        Claim FindClaim(string claimType);

        Claim[] FindClaims(string claimType);
        Claim[] GetAllClaims();

        bool IsInRole(string roleName);
    }
}
