
using System;
using System.Reflection;
using Caf.Core.Caf.Utils.Reflection;

namespace Caf.DynamicWebApi.Attributes
{

   [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class|AttributeTargets.Method)]
    public class DynamicWebApiAttribute:Attribute
    {
       
       /// <summary>
       ///  support area webapi
       /// </summary>
       /// <value></value>

       public string AreaName{get;set;}
          /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsMetadataEnabled { get; set; }

        public DynamicWebApiAttribute(bool isEnabled = true)
        {
            IsEnabled = isEnabled;
            IsMetadataEnabled = true;
        }

        public virtual bool IsEnabledFor(Type type)
        {
            return IsEnabled;
        }

        public virtual bool IsEnabledFor(MethodInfo method)
        {
            return IsEnabled;
        }

        public virtual bool IsMetadataEnabledFor(Type type)
        {
            return IsMetadataEnabled;
        }

        public virtual bool IsMetadataEnabledFor(MethodInfo method)
        {
            return IsMetadataEnabled;
        }

        public static bool IsExplicitlyEnabledFor(Type type)
        {
            var dynamicWebApiServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<DynamicWebApiAttribute>();
            return dynamicWebApiServiceAttr != null && dynamicWebApiServiceAttr.IsEnabledFor(type);
        }

        public static bool IsExplicitlyDisabledFor(Type type)
        {
            var dynamicWebApiServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<DynamicWebApiAttribute>();
            return dynamicWebApiServiceAttr != null && !dynamicWebApiServiceAttr.IsEnabledFor(type);
        }

        public static bool IsMetadataExplicitlyEnabledFor(Type type)
        {
            var dynamicWebApiServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<DynamicWebApiAttribute>();
            return dynamicWebApiServiceAttr != null && dynamicWebApiServiceAttr.IsMetadataEnabledFor(type);
        }

        public static bool IsMetadataExplicitlyDisabledFor(Type type)
        {
            var dynamicWebApiServiceAttr = type.GetTypeInfo().GetSingleAttributeOrNull<DynamicWebApiAttribute>();
            return dynamicWebApiServiceAttr != null && !dynamicWebApiServiceAttr.IsMetadataEnabledFor(type);
        }

        public static bool IsMetadataExplicitlyDisabledFor(MethodInfo method)
        {
            var dynamicWebApiServiceAttr = method.GetSingleAttributeOrNull<DynamicWebApiAttribute>();
            return dynamicWebApiServiceAttr != null && !dynamicWebApiServiceAttr.IsMetadataEnabledFor(method);
        }

        public static bool IsMetadataExplicitlyEnabledFor(MethodInfo method)
        {
            var dynamicWebApiServiceAttr = method.GetSingleAttributeOrNull<DynamicWebApiAttribute>();
            return dynamicWebApiServiceAttr != null && dynamicWebApiServiceAttr.IsMetadataEnabledFor(method);
        }
    }
}