using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.EF.EFCore
{
    public interface IEFDbContextEntityMapping
    {
        Type GetDbContextByEntity<TEntity>();
        void LoadMaping(Type dbContextType);
    }
}
