using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Caf.Core.JWT
{
    public static class JWTAuthExtensions
    {
        

        public static string CreateAccessToken(this CafJwtAuthConfiguration jWTAuthConfiguration, List<Claim> claims)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jWTAuthConfiguration.Issuer,
                audience: jWTAuthConfiguration.Audience,
                claims: claims,
                expires: now.Add(jWTAuthConfiguration.Expiration),
                signingCredentials: jWTAuthConfiguration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
