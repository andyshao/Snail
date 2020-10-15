﻿using AutoMapper;
using Snail.Core.Interface;
using Snail.Web;

namespace Snail.Web.Controllers
{
    /// <summary>
    /// controller公共上下文，用于定义controller类的公共方法，属性等
    /// </summary>
    public class ControllerContext
    {
        public IMapper mapper;
        public IApplicationContext applicationContext;
        public BaseAppDbContext db;
        public IEntityCacheManager entityCacheManager;
        public ControllerContext(IMapper mapper,IApplicationContext applicationContext, BaseAppDbContext db, IEntityCacheManager entityCacheManager)
        {
            this.mapper = mapper;
            this.applicationContext = applicationContext;
            this.db = db;
            this.entityCacheManager = entityCacheManager;
        }
    }
}
