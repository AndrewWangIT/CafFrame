using AutoMapper;
using Caf.Core.DependencyInjection;
using Caf.Core.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Caf.AutoMapper
{
    public class CafAutoMapperModule:CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddObjectWrapper<IMapper>();
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<CafMapperOption>>().Value;

                var mapperConfiguration = new MapperConfiguration(mapper =>
                {
                    foreach (var config in options.Configurators)
                    {
                        config(mapper);
                    }
                });
                var mapper = new Mapper(mapperConfiguration);
                var mapperWrapper = context.ServiceProvider.GetRequiredService<IObjectWrapper<IMapper>>();
                mapperWrapper.Value = mapper;
            }
        }
    }
}
