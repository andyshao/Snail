﻿using ApplicationCore.IServices;
using AutoMapper;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Snail.Cache;
using Snail.Core.Entity;
using Snail.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
namespace Snail.Web.Services
{
    public abstract class ServiceContextBaseService : IService
    {
        protected IEntityCacheManager entityCacheManager => serviceContext.entityCacheManager;
        protected IMapper mapper => serviceContext.mapper;
        protected IApplicationContext applicationContext => serviceContext.applicationContext;
        protected string currentUserId => serviceContext.applicationContext.GetCurrentUserId();
        public DbContext db => serviceContext.db;
        public IMemoryCache memoryCache => serviceContext.memoryCache;
        public ICapPublisher publisher => serviceContext.publisher;
        public ISnailCache cache => serviceContext.cache;
        public IServiceProvider serviceProvider => serviceContext.serviceProvider;
        public ServiceContext serviceContext;
  
        protected ServiceContextBaseService(ServiceContext serviceContext)
        {
            this.serviceContext = serviceContext;
        }

        /// <summary>
        /// 获取entity的缓存，用于TEntity会频繁修改，又需要自己控制cache刷新的情况（EntityCacheManager不适用的情况）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TEntityCache"></typeparam>
        /// <returns></returns>
        public List<TEntityCache> GetEntityCache<TEntity, TEntityCache>()
         where TEntity : class, IEntity
        {
            return serviceContext.GetEntityCache<TEntity, TEntityCache>();
        }

        public void ClearEntityCache<TEntity, TEntityCache>()
        {
            serviceContext.ClearEntityCache<TEntity, TEntityCache>();

        }

        public void ClearAllEntityCache()
        {
            serviceContext.ClearAllEntityCache();

        }
    }
}
