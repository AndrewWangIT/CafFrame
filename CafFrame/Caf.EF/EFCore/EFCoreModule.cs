using Caf.Core.Module;
using Caf.Domain.Repository;
using Caf.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.EF.EFCore
{
    [UsingModule(typeof(UowModule))]
    public class EFCoreModule:CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton(typeof(IEFDbContextEntityMapping), typeof(EFDbContextEntityMapping));
            context.Services.AddScoped(typeof(IDbContextProvider<>),typeof(DbContextProvider<>));
            context.Services.AddScoped(typeof(ICafRepository<, >),typeof(CafEfCoreRepository<,>));
            context.Services.AddScoped(typeof(ICafRepository<>), typeof(CafEfCoreRepository<>));
        }
    }
}
