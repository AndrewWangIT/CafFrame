using Caf.AspNetCore.Caf.Cors;
using Caf.Core.JWT;
using Caf.Core.Module;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Caf.AspNetCore
{
    public static class CafConfigurationContextExtensions
    {
        public static void AddCafCors(this CafConfigurationContext context,Action<CafCorsOption> action)
        {
            CafCorsOption cafCorsOption = new CafCorsOption();
            action.Invoke(cafCorsOption);
            context.Services.AddObjectWrapper(cafCorsOption);
        }
        public static void AddCafJWTAuth(this CafConfigurationContext context, Action<CafJwtAuthConfiguration> action, Func<MessageReceivedContext, Task> onMessageReceived = null)
        {
            CafJwtAuthConfiguration jWTAuthConfiguration = new CafJwtAuthConfiguration();
            action.Invoke(jWTAuthConfiguration);
            context.Services.AddSingleton(jWTAuthConfiguration);
            context.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "CafJWTBearer";
                options.DefaultChallengeScheme = "CafJWTBearer";
            }).AddJwtBearer("CafJWTBearer", options =>
            {
                options.Audience = jWTAuthConfiguration.Audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jWTAuthConfiguration.SymmetricSecurityKey,

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = jWTAuthConfiguration.Issuer,

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = jWTAuthConfiguration.Audience,

                    // Validate the token expiry
                    ValidateLifetime = true,

                    // If you want to allow a certain amount of clock drift, set that here
                    ClockSkew = TimeSpan.Zero
                };

                if (onMessageReceived != null)
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = onMessageReceived
                    };
                }

            });
        }
    }
}
