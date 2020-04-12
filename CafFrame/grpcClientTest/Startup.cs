using AspectCore.Injector;
using Caf.Core.DependencyInjection;
using Capgemini.Caf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace grpcClientTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; set; }
        public void ConfigureServices(IServiceCollection services)
        {
        services.ConfigureModule<GrpcTestMoudle>(Configuration);
            Services = services;
        }
        public void ConfigureContainer(IServiceContainer builder)
        {
            builder.BuildServiceProviderFromFactory(Services);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationRun();
        }
    }
}
