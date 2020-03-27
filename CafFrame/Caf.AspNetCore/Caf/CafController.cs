using AspectCore.Injector;
using AutoMapper;
using Caf.Core.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AspNetCore.Caf
{
    public class CafController: Controller
    {
        [FromContainer]
        public IStringLocalizer StringLocalizer { get; set; }
        [FromContainer]
        public IServiceProvider ServiceProvider { get; set; }

        public IMapper Mapper { get { return MapperWrapper.Value; } }
        [FromContainer]
        public IObjectWrapper<IMapper> MapperWrapper { get; set; }

        protected string L(string key)
        {
            return StringLocalizer[key].Value;
        }
        protected string L(object key)
        {
            return StringLocalizer[key.ToString()].Value;
        }
    }
}
