using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Caf.Core.Caf.Utils.Reflection;
using Caf.Core.Utils;
using Caf.DynamicWebApi.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Caf.DynamicWebApi {
    public class DynamicWebApiConvention : IApplicationModelConvention {
        public void Apply (ApplicationModel application) {
            foreach (var controller in application.Controllers) {
                var controllerType = controller.ControllerType.AsType ();
                var daynamicWebApiServiceAttr = ReflectionExt.GetSingleAttributeOrDefault<DynamicWebApiAttribute> (controllerType.GetTypeInfo ());
                if (ImplementsDynamicWebApiInterface (controllerType)) {
                    controller.ControllerName = controller.ControllerName.RemovePostFix(WebApiConsts.ControllerPostfixes.ToArray());
                    ConfigureApplicationService (controller, daynamicWebApiServiceAttr);
                } else {
                    if (daynamicWebApiServiceAttr != null && daynamicWebApiServiceAttr.IsEnabledFor (controllerType)) {
                        ConfigureApplicationService (controller, daynamicWebApiServiceAttr);
                    }
                }
            }
        }

        protected virtual void ConfigureArea (ControllerModel controller, DynamicWebApiAttribute attr) {
            Check.NotNull (attr, nameof (attr));
            if (!controller.RouteValues.ContainsKey ("area")) {
                if (!string.IsNullOrEmpty (attr.AreaName)) {
                    controller.RouteValues["area"] = attr.AreaName;
                }
            }
        }
        /// <summary>
        ///    判断是否实现了接口
        /// </summary>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        protected virtual bool ImplementsDynamicWebApiInterface (Type controllerType) {
            return typeof (IDynamicWebApiService).GetTypeInfo ().IsAssignableFrom (controllerType);

        }
        private void ConfigureApplicationService (ControllerModel controller, DynamicWebApiAttribute attr) {
            ConfigureApiExplorer (controller);
            ConfigureSelector (controller, attr);
            ConfigureParameters (controller);

        }
        private void ConfigureApiExplorer (ControllerModel controller) {
            if (controller.ApiExplorer.GroupName.IsNullOrEmpty ()) {
                controller.ApiExplorer.GroupName = controller.ControllerName;
            }
            if (!controller.ApiExplorer.IsVisible.HasValue) {
                var controllerType = controller.ControllerType.AsType ();
                var daynamicWebApiServiceAttr = ReflectionExt.GetSingleAttributeOrDefault<DynamicWebApiAttribute> (controllerType.GetTypeInfo ());
                if (daynamicWebApiServiceAttr != null) {
                    controller.ApiExplorer.IsVisible =
                        daynamicWebApiServiceAttr.IsEnabledFor (controllerType) &&
                        daynamicWebApiServiceAttr.IsMetadataEnabledFor (controllerType);
                } else {
                    controller.ApiExplorer.IsVisible = true;
                }
            }
            foreach (var action in controller.Actions) {
                ConfigureApiExplorer (action);
            }
        }
        protected virtual void ConfigureApiExplorer (ActionModel action) {
            if (action.ApiExplorer.IsVisible == null) {
              action.ApiExplorer.IsVisible=true;
            }
        }

        private void ConfigureSelector (ControllerModel controller, DynamicWebApiAttribute attr) {
            RemoveEmptySelectors (controller.Selectors);

            if (controller.Selectors.Any (s => s.AttributeRouteModel != null)) {
                return;
            }
            var areaName = string.Empty;
            if (attr != null) {
                areaName = attr.AreaName;
            }
            foreach (var action in controller.Actions) {
                ConfigureSelector (areaName, controller.ControllerName, action);
            }
        }
        private void ConfigureSelector (string areaName, string controllerName, ActionModel action) {
            var nonDynamicWebApiAttr = ReflectionExt.GetSingleAttributeOrDefault<NonDynamicWebApiAttribute> (action.ActionMethod);
            if (nonDynamicWebApiAttr != null)
                return;
            RemoveEmptySelectors (action.Selectors);
            if (action.Selectors.Count <= 0) {
                AddApplicationServiceSelector (areaName, controllerName, action);
            } else {
                NormalizeSelectorRoutes (areaName, controllerName, action);
            }

        }
        private void ConfigureParameters (ControllerModel controller) {
            foreach (var action in controller.Actions) {
                foreach (var parameter in action.Parameters) {
                    if (parameter.BindingInfo != null) {
                        continue;
                    }
                       if(!TypeHelper.IsPrimitiveExtendedIncludingNullable(parameter.ParameterInfo.ParameterType))
                       {
                           if(CanUseFormBodyBinding(action,parameter))
                           {
                             parameter.BindingInfo = BindingInfo.GetBindingInfo (new [] { new FromBodyAttribute () });

                           }
                       }
                    
                }
            }
        }
       
        private bool CanUseFormBodyBinding(ActionModel action, ParameterModel parameter)
        {
            if (WebApiConsts.FormBodyBindingIgnoredTypes.Any(t => t.IsAssignableFrom(parameter.ParameterInfo.ParameterType)))
            {
                return false;
            }

            foreach (var selector in action.Selectors)
            {
                if (selector.ActionConstraints == null)
                {
                    continue;
                }

                foreach (var actionConstraint in selector.ActionConstraints)
                {
                    
                    var httpMethodActionConstraint = actionConstraint as HttpMethodActionConstraint;
                    if (httpMethodActionConstraint == null)
                    {
                        continue;
                    }

                    if (httpMethodActionConstraint.HttpMethods.All(hm => hm.IsIn("GET", "DELETE", "TRACE", "HEAD")))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 移除空路由
        /// </summary>
        /// <param name="selectors"></param>
        private void RemoveEmptySelectors (IList<SelectorModel> selectors) {
            for (var i = selectors.Count - 1; i >= 0; i--) {
                var selector = selectors[i];
                if (selector.AttributeRouteModel == null &&
                    (selector.ActionConstraints == null || selector.ActionConstraints.Count <= 0) &&
                    (selector.EndpointMetadata == null || selector.EndpointMetadata.Count <= 0)) {
                    selectors.Remove (selector);
                }
            }
        }
        /// <summary>
        /// 生成路由
        /// </summary>
        /// <param name="action"></param>
        private void AddApplicationServiceSelector (string areaName, string controllerName, ActionModel action) {

            var httpMethod = HttpMethodHelper.GetConventionalVerbForMethodName (action.ActionName);
            action.ActionName = GetActualActionName (action.ActionName);
            var selector = new SelectorModel ();
            selector.AttributeRouteModel = new AttributeRouteModel (new RouteAttribute (CreateRouter (areaName, controllerName, action)));
            selector.ActionConstraints.Add (new HttpMethodActionConstraint (new [] { httpMethod }));
            action.Selectors.Add (selector);
        }
        /// <summary>
        ///   process action name
        /// </summary>
        /// <returns></returns>
        protected virtual string GetActualActionName (string actionName) {
            //remove post prefix
            actionName = actionName.RemovePostFix (WebApiConsts.ActionPostfixes.ToArray ());
            foreach (var prefix in WebApiConsts.actionPrefixes) {
                if (actionName.StartsWith (prefix)) {
                    actionName = actionName.Substring (prefix.Length);
                    break;
                }
            }
            return actionName;
        }
        private string CreateRouter (string areaName, string controllerName, ActionModel action) {
            var routeTemplate = new StringBuilder ();
            // {apiprefix}/{areaName}/{controllerName}/{actionName}
            routeTemplate.Append (WebApiConsts.DefaultApiPreFix);
            if (!string.IsNullOrEmpty (areaName))
                routeTemplate.Append (string.Format ("/{0}", areaName));
            routeTemplate.Append ($"/{controllerName}");
            // id 部分
            if (action.Parameters.Any (temp => temp.ParameterName == "id")) {
                routeTemplate.Append ("/{id}");
            }
            // Action 名称部分
            if (!string.IsNullOrEmpty (action.ActionName)) {
                routeTemplate.Append ($"/{action.ActionName}");
            }
            return routeTemplate.ToString ();
        }

        private void NormalizeSelectorRoutes (string areaName, string controllerName, ActionModel action) {

            action.ActionName = GetActualActionName (action.ActionName);
            var httpMethod = HttpMethodHelper.GetConventionalVerbForMethodName (action.ActionName);
            foreach (var selector in action.Selectors) {
                if (selector.AttributeRouteModel == null) {
                    selector.AttributeRouteModel = new AttributeRouteModel (new RouteAttribute (CreateRouter (areaName, controllerName, action)));
                }
                if (selector.ActionConstraints.OfType<HttpMethodActionConstraint> ().FirstOrDefault ()?.HttpMethods?.FirstOrDefault () == null) {
                    selector.ActionConstraints.Add (new HttpMethodActionConstraint (new [] { httpMethod }));
                }
            }
        }

    }
}