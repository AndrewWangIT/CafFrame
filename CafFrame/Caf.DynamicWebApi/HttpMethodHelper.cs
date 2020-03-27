using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Caf.Core;
using Caf.Core.Utils;

namespace Caf.DynamicWebApi
{

    
    public static class HttpMethodHelper
    {
        public  const string DefaultHttpVerb = "POST";

        public static Dictionary<string,string[]> ConventionPrefixes{get;set;}=new Dictionary<string, string[]>
        {
            {"GET", new[] {"GetList", "GetAll", "Get"}},
            {"PUT", new[] {"Put", "Update"}},
            {"DELETE", new[] {"Delete", "Remove"}},
            {"POST", new[] {"Create", "Add", "Insert", "Post"}},
            {"PATCH", new[] {"Patch"}}
        };
        /// <summary>
        ///  get http verb
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
         public static string GetConventionalVerbForMethodName(string methodName)
        {
            foreach (var conventionalPrefix in ConventionPrefixes)
            {
                if (conventionalPrefix.Value.Any(prefix => methodName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                {
                    return conventionalPrefix.Key;
                }
            }

            return DefaultHttpVerb;
        }
        /// <summary>
        /// remove metond prefix
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>

        public static string RemoveHttpMethodPrefix(string methodName,  string httpMethod)
        {
            Check.NotNull(methodName, nameof(methodName));
            Check.NotNull(httpMethod, nameof(httpMethod));

            var prefixes =  ConventionPrefixes.GetOrDefault(httpMethod);
            if (prefixes.IsNullOrEmpty())
            {
                return methodName;
            }

            return methodName.RemovePreFix(prefixes);
        }

        public static HttpMethod ConvertToHttpMethod(string httpMethod)
        {
            switch (httpMethod.ToUpperInvariant())
            {
                case "GET":
                    return HttpMethod.Get;
                case "POST":
                    return HttpMethod.Post;
                case "PUT":
                    return HttpMethod.Put;
                case "DELETE":
                    return HttpMethod.Delete;
                case "OPTIONS":
                    return HttpMethod.Options;
                case "TRACE":
                    return HttpMethod.Trace;
                case "HEAD":
                    return HttpMethod.Head;
                case "PATCH":
                    return new HttpMethod("PATCH");
                default:
                    throw new CafException("Unknown HTTP METHOD: " + httpMethod);
            }
        }
    }
}