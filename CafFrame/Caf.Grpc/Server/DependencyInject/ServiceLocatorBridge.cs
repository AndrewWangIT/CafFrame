using MagicOnion.Server;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Caf.Grpc.Server.Configuration;
using System.Threading;
using MagicOnion.Hosting;

namespace Caf.Grpc.Server.DependencyInject
{
    public class ServiceLocatorBridge : IServiceLocator
    {
        private readonly IServiceScopeFactory _serviceProvider;
        //private static AsyncLocal<IServiceProvider> dics = new AsyncLocal<IServiceProvider>();
        //public static IServiceProvider current => dics.Value;
        public ServiceLocatorBridge(IServiceScopeFactory serviceProvider, MagicOnionOptions magicOnionOptions)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceLocatorScope CreateScope()
        {
           //return new MicrosoftExtensionsServiceLocator(_serviceProvider.CreateScope().ServiceProvider).CreateScope();
            //throw new NotImplementedException();
            return null;
        }

        public T GetService<T>()
        {
            var bc = _serviceProvider.CreateScope().ServiceProvider;
            //dics.Value = bc;
            
            var aa= bc.GetRequiredService<T>();
            return aa;
        }

        public void Register<T>()
        {
            //_serviceProvider.AddSingleton()
            //if (!_iocManager.IsRegistered(typeof(T)))
            //{
            //    _iocManager.Register(typeof(T));
            //}
        }

        public void Register<T>(T singleton)
        {
            //_iocManager.Register(typeof(T));
        }
    }
}
