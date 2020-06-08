using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public class IntegrationHandlerDesc
    {
        public IntegrationHandlerDesc(Type handleType, Type integrationEventType,string groupId, MethodInfo handlerMethod)
        {
            HandlerType = handleType;
            IntegrationEventType = integrationEventType;
            GroupId = groupId;
            HandlerMethod = handlerMethod;
        }
        public Type HandlerType { get; set; }
        public Type IntegrationEventType { get; set; }         
        public string GroupId { get; set; }
        public MethodInfo HandlerMethod { get; set; }
    }
}
