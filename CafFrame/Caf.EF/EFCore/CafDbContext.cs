using AspectCore.Extensions.Reflection;
using Caf.Core.Caf.User;
using Caf.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.EF.EFCore
{
    public abstract class CafDbContext<TDbContext> : DbContext where TDbContext: DbContext
    {
        protected virtual object CurrentCreatorId { get { return null; } }
        //protected virtual object CreatorIdL { get { return null; } }
        private static readonly MethodInfo ConfigureModelMethodInfo = typeof(CafDbContext<TDbContext>).GetMethod("ConfigureModel",
                    BindingFlags.Instance | BindingFlags.NonPublic);
        protected CafDbContext(DbContextOptions<TDbContext> options)
    : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureModelMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder });
            }
        }

        /// <summary>
        /// 配置model设置
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="modelBuilder"></param>
        protected virtual void ConfigureModel<TEntity>(ModelBuilder modelBuilder)
    where TEntity : class
        {
            if (typeof(IHasSoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                modelBuilder.Entity<TEntity>().HasQueryFilter(o => ((IHasSoftDelete)o).IsDeleted == false);
            }
            //if(typeof(TEntity).GetInterfaces().Any(o => o.IsGenericType && (o.GetGenericTypeDefinition() == typeof(IHasCreatorWithReferenceTypeKey<>))))
            //{
            //    modelBuilder.Entity<TEntity>().Property("CreatorId").HasMaxLength(50);
            //}
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AutoSettingField();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            return result;
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AutoSettingField();
            var result =await base.SaveChangesAsync(acceptAllChangesOnSuccess,cancellationToken);
            return result;
        }
        private void AutoSettingField()
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        AutoSettingFieldForAdded(entry);
                        break;
                    case EntityState.Modified:
                        AutoSettingFieldForModified(entry);
                        break;
                    case EntityState.Deleted:
                        AutoSettingFieldForDeleted(entry);
                        break;
                }
            }
        }

        private void AutoSettingFieldForAdded(EntityEntry entry)
        {
            if(entry.Entity is IHasCreationTime)
            {
                ((IHasCreationTime)entry.Entity).CreationTime = DateTime.Now;
            }
            //存在反射性能问题，后期优化
            var entityType = entry.Entity.GetType();
            if (entityType.GetInterfaces().Any(o=>o.IsGenericType && (o.GetGenericTypeDefinition() == typeof(IHasCreator<>) || o.GetGenericTypeDefinition() == typeof(IHasCreatorWithReferenceTypeKey<>))))
            {
                if(entityType.GetProperties().First(o => o.Name == "CreatorId").GetReflector().GetValue(entry.Entity) == null)
                {
                    entityType.GetProperties().First(o => o.Name == "CreatorId").SetValue(entry.Entity, CurrentCreatorId);
                }
            }
        }
        private void AutoSettingFieldForModified(EntityEntry entry)
        {
            if(entry.Entity is IHasLastModificationTime)
            {
                ((IHasLastModificationTime)entry.Entity).LastModificationTime = DateTime.Now;
            }
            //存在反射性能问题，后期优化
            var entityType = entry.Entity.GetType();
            if (entityType.GetInterfaces().Any(o => o.IsGenericType && (o.GetGenericTypeDefinition() == typeof(IHasLastModifier<>) || o.GetGenericTypeDefinition() == typeof(IHasLastModifierWithReferenceTypeKey<>))))
            {
                if (entityType.GetProperties().First(o => o.Name == "LastModifierId").GetReflector().GetValue(entry.Entity) == null)
                {
                    entityType.GetProperties().First(o => o.Name == "LastModifierId").SetValue(entry.Entity, CurrentCreatorId);
                }
            }
        }

        private void AutoSettingFieldForDeleted(EntityEntry entry)
        {
            if (entry.Entity is IHasSoftDelete)
            {
                ((IHasSoftDelete)entry.Entity).IsDeleted = true;
                ((IHasSoftDelete)entry.Entity).DeletedTime = DateTime.Now;
                entry.State = EntityState.Modified;
            }
        }


    }
}
