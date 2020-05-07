using System;
using System.Collections.Generic;

namespace Caf.DynamicWebApi
{
    public class WebApiConsts
    {
        public static string DefaultHttpVerb { get; set; }

        public static string DefaultAreaName { get; set; } 
 
        public static string DefaultApiPreFix { get; set; }

        public static List<string> ControllerPostfixes { get; set; }
        public static List<string> ActionPostfixes { get; set; }

        public static List<Type> FormBodyBindingIgnoredTypes { get; set; }

        public static Dictionary<string,string> HttpVerbs { get; set; }

        public static string[] actionPrefixes{get;set;}=new [] {
                "GetAll",
                "GetList",
                "Get",
                "Post",
                "Create",
                "Add",
                "Insert",
                "Put",
                "Update",
                "Delete",
                "Remove",
                "Patch"
            };
    }
}