using Caf.Core.Module;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.Caf.File
{
    public static class FileExtensions
    {
        public static void ConfigFileOptions(this CafConfigurationContext cafConfigurationContext)
        {
            cafConfigurationContext.Services.Configure<AWSS3Options>(cafConfigurationContext.Configuration.GetSection(nameof(AWSS3Options)));
        }
    }
}
