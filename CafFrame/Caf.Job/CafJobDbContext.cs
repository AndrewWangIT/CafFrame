using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.Job
{
    public class CafJobDbContext : DbContext
    {
        public CafJobDbContext(DbContextOptions<CafJobDbContext> options) : base(options)
        {
        }

    }
}
