using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.EF.EFCore
{
    public interface IDbContextProvider<EntityType> where EntityType:class
    {
        DbContext GetDbContext();
    }
}
