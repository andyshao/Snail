﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snail.Core.Interface;
using Snail.Web;

namespace Snail.Web.Controllers
{
    /// <summary>
    /// 所有contoller继承此类re
    /// </summary>
    /// <remarks>
    /// 如果没有AuthorizeAttribute，HttpContext.User不会有用户
    /// </remarks>
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    [Authorize]
    public class DefaultBaseController : ControllerBase
    {
        protected SnailControllerContext controllerContext;
        protected IMapper mapper => controllerContext.mapper;
        protected IApplicationContext applicationContext => controllerContext.applicationContext;
        protected DbContext db => controllerContext.db;
        protected IEntityCacheManager entityCacheManager => controllerContext.entityCacheManager;
        protected string currentUserId => controllerContext.applicationContext.GetCurrentUserId();
        public DefaultBaseController(SnailControllerContext controllerContext)
        {
            this.controllerContext = controllerContext;
        }
    }
}
