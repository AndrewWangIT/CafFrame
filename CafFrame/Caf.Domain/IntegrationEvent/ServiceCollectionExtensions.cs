using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Domain.IntegrationEvent
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection ServiceCollection;

        /// <summary>
        /// Adds and configures the consistence services for the consistency.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="CafOptions" />.</param>
        /// <returns>An <see cref="CafBuilder" /> for application services.</returns>
        public static void AddIntegrationEventBus(this IServiceCollection services, Action<IntegrationEventBusOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            ServiceCollection = services;
            services.TryAddSingleton<IConsumerRegister, ConsumerRegister>();

            //Processors
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, ConsumerRegister>());

            //Options and extension service
            var options = new IntegrationEventBusOptions();
            setupAction(options);
            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }
            services.Configure(setupAction);
            if(options.IConsumerErrHandler!=null)
            {
                services.Replace(ServiceDescriptor.Singleton(typeof(IConsumerErrHandler), options.IConsumerErrHandler));
            }
            //Startup and Hosted 
            services.AddSingleton<Bootstrapper>();
            services.AddHostedService<Bootstrapper>();
           
        }
    }
}
