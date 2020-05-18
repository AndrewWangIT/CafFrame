using Caf.AppSetting.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AppSetting.DbContextService
{
    public class CafAppsettingDbContext: DbContext
    {
        public CafAppsettingDbContext(DbContextOptions<CafAppsettingDbContext> options) : base(options)
        {
        }

        public DbSet<AppSettingViewModel> AppSettingByCafs { get; set; } 

    }
}
