﻿using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Snail.Core.Attributes;
using Snail.Core.Default;
using Snail.Permission.Entity;
using System;
using System.Linq;
using System.Reflection;

namespace Snail.Web
{
    public partial class BaseAppDbContext : DbContext
    {
        #region 通用权限表
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<RoleResource> RoleResources { get; set; }
        public DbSet<Snail.Permission.Entity.Org> Orgs { get; set; }
        public DbSet<UserOrg> UserOrgs { get; set; }
        #endregion
        #region 公共表
        public DbSet<Snail.Web.Entities.Config> Configs { get; set; }
        public DbSet<Snail.FileStore.FileInfo> FileInfos { get; set; }
        #endregion

        private ICapPublisher _publisher;
        public BaseAppDbContext(DbContextOptions options, ICapPublisher publisher)
            : base(options)
        {
            _publisher = publisher;
        }

        public BaseAppDbContext(DbContextOptions options)
          : base(options)
        {
        }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 自动应用所有的IEntityTypeConfiguration配置
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public override int SaveChanges()
        {
            //统一在数据库上下文的操作前，触发缓存实体的数据清空。
            if (_publisher != null)
            {
                this.ChangeTracker.Entries().Where(a =>(a.State == EntityState.Added || a.State == EntityState.Modified || a.State == EntityState.Deleted) && Attribute.IsDefined(a.Entity.GetType(), typeof(EnableEntityCacheAttribute))).Select(a => a.Entity.GetType().Name).Distinct().ToList().ForEach(entityName =>
                {
                    _publisher.Publish(EntityCacheManager.EntityCacheEventName, new EntityChangeEvent { EntityName = entityName });
                });
            }

            return base.SaveChanges();
        }

        // 不用要SeedData会数据初始化，此方法会在每次migration时删除和创建数据
    }
}
