using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
namespace Caf.DynamicWebApi {
    public static class DynamicWebApiServiceExtensions {
        public static IServiceCollection AddDynamicWebApi (this IServiceCollection services, DynamicWebApiOptions options) {
            if (options == null) {
                throw new ArgumentException (nameof (options));
            }
            options.Valid ();
            WebApiConsts.DefaultAreaName = options.DefaultAreaName;
            WebApiConsts.DefaultHttpVerb = options.DefaultHttpVerb;
            WebApiConsts.DefaultApiPreFix = options.DefaultApiPrefix;
            WebApiConsts.ControllerPostfixes = options.RemoveControllerPostfixes;
            WebApiConsts.ActionPostfixes = options.RemoveActionPostfixes;
            WebApiConsts.FormBodyBindingIgnoredTypes = options.FormBodyBindingIgnoredTypes;
            var partManager = services.GetSingletonInstanceOrNull<ApplicationPartManager> ();
            if (partManager == null) {
                throw new InvalidOperationException ("\"AddDynamicWebApi\" must be after \"AddMvc\".");
            }
            // Add a custom controller checker
            partManager.FeatureProviders.Add (new DynamicWebApiControllerFeatureProvider ());

            services.Configure<MvcOptions> (o => {
                // Register Controller Routing Information Converter
                o.Conventions.Add (new DynamicWebApiConvention ());
            });             
            return services;
        }

        public static IServiceCollection AddDynamicWebApi (this IServiceCollection services) {
            return services.AddDynamicWebApi (new DynamicWebApiOptions ());
        }
    }
}