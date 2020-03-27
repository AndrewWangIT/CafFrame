using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caf.AutoMapper
{
    public class CafMapperOption
    {
        public List<Action<IMapperConfigurationExpression>> Configurators { get; } = new List<Action<IMapperConfigurationExpression>>();

        public List<Type> Profiles { get; set; }

        public void AddConfigurator(Action<IMapperConfigurationExpression> action)
        {
            Configurators.Add(action);
        }
        public void AddProfiles(Assembly assembly)
        {
            var profileTypes = assembly
                .GetTypes()
                .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && !type.IsGenericType);

            foreach (var profileType in profileTypes)
            {
                Configurators.Add(mapper => mapper.AddProfile(profileType));
            }
        }
    }
}
