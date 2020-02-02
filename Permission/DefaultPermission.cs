﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Snail.Common;
using Snail.Common.Extenssions;
using Snail.Core;
using Snail.Core.Attributes;
using Snail.Core.Permission;
using Snail.Permission.Entity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Snail.Permission
{
    public class DefaultPermission : BasePermission
    {
        public static readonly string superAdminRoleName = "SuperAdmin";
        public DefaultPermission(IPermissionStore permissionStore, IOptionsMonitor<PermissionOptions> permissionOptions) : base(permissionStore, permissionOptions)
        {
        }

        public override bool HasPermission(string resourceKey, string userKey)
        {
            if (IsSuperAdmin(userKey))
            {
                return true;
            }
            return base.HasPermission(resourceKey, userKey);
        }

        public override string GenerateTokenStr(List<Claim> claims)
        {
            var expireTimeSpan = (_permissionOptions.ExpireTimeSpan == null || _permissionOptions.ExpireTimeSpan == TimeSpan.Zero) ? new TimeSpan(6, 0, 0) : _permissionOptions.ExpireTimeSpan;
            SigningCredentials creds;
            if (_permissionOptions.IsAsymmetric)
            {
                var key = new RsaSecurityKey(RSAHelper.GetRSAParametersFromFromPrivatePem(_permissionOptions.RsaPrivateKey));
                creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            }
            else
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_permissionOptions.SymmetricSecurityKey));
                creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            }
            var token = new JwtSecurityToken(_permissionOptions.Issuer, _permissionOptions.Audience, claims, DateTime.Now, DateTime.Now.Add(expireTimeSpan), creds);
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenStr;
        }

        /// <summary>
        /// 获取资源对象的code,已经适配如下类型:AuthorizationFilterContext,ControllerActionDescriptor,methodInfo
        /// 默认为className_methodName，或是resourceAttribute里设置的code
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override string GetRequestResourceCode(object obj)
        {
            if (obj is MethodInfo)
            {
                return GetResourceCode((MethodInfo)obj);
            }
            MethodInfo methodInfo;
            if (obj is AuthorizationFilterContext authorizationFilterContext)
            {
                if (authorizationFilterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    methodInfo = controllerActionDescriptor.MethodInfo;
                    return GetResourceCode(methodInfo);
                    //resourceCode = GetResourceCode(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
                }
            }
            if (obj is ControllerActionDescriptor controllerActionDescriptor1)
            {
                methodInfo = controllerActionDescriptor1.MethodInfo;
                return GetResourceCode(methodInfo);
                //resourceCode = GetResourceCode(controllerActionDescriptor1.ControllerName, controllerActionDescriptor1.ActionName);
            }

            if (obj is RouteEndpoint endpoint)
            {
                //.net core 3.1后，AuthorizationHandlerContext.Resource为endpoint
                methodInfo = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.MethodInfo;
                return GetResourceCode(methodInfo);

            }
            return string.Empty;
        }

        /// <summary>
        /// 初始化所有的权限资源。
        /// 所有有定义ResourceAttribute的方法都为权限资源，否则不是。要使方法受权限控制，必须做到如下两点：1、在方法上加ResourceAttribute，2、在controller或是action上加Authorize
        /// </summary>
        public override void InitResource()
        {
            var resources = new List<Resource>();
            if (_permissionOptions.ResourceAssemblies == null)
            {
                _permissionOptions.ResourceAssemblies = new List<Assembly>();
            }
            var existResources = _permissionStore.GetAllResource();
            _permissionOptions.ResourceAssemblies.Add(this.GetType().Assembly);
            _permissionOptions.ResourceAssemblies?.Distinct().ToList().ForEach(assembly =>
            {
                //对所有的controller类进行扫描
                assembly.GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList().ForEach(controller =>
                {
                    var controllerIsAdded = false;//父是否增加
                    var parentId = IdGenerator.Generate<string>();
                    var parentResource = controller.GetCustomAttribute<ResourceAttribute>();
                    controller.GetMethods().ToList().ForEach(method =>
                    {
                        if (method.IsDefined(typeof(ResourceAttribute), true))
                        {
                            var methodResource = method.GetCustomAttribute<ResourceAttribute>();
                            if (!controllerIsAdded)
                            {
                                // 增加父
                                resources.Add(new Resource
                                {
                                    Id = parentId,
                                    Code = parentResource?.ResourceCode??controller.Name,
                                    CreateTime = DateTime.Now,
                                    IsDeleted = false,
                                    Name = parentResource?.Description??controller.Name
                                });
                                controllerIsAdded = true;
                            }
                            // 增加子
                            resources.Add(new Resource
                            {
                                Id = IdGenerator.Generate<string>(),
                                Code = GetResourceCode(method),
                                CreateTime = DateTime.Now,
                                IsDeleted = false,
                                ParentId = parentId,
                                Name = methodResource?.Description??method.Name
                            });
                        }
                    });
                });
            });
            resources.ForEach(item =>
            {
                // 计算resource的id，如果已经存在，id为已经存在的resource的id
                var matchRs = existResources.FirstOrDefault(i => i.GetResourceCode() == item.Code);
                if (matchRs!=null)
                {
                    item.Id = matchRs.GetKey();
                }

                // 计算资源的父id，如果父存在，则子资源的父id为已经存在的父数据的id，否则为新的id
                if (!string.IsNullOrEmpty(item.ParentId))
                {
                    var pa = resources.FirstOrDefault(a => a.Id == item.ParentId);
                    var matchPa = existResources.FirstOrDefault(i => i.GetResourceCode() == pa?.Code);
                    if (matchPa!=null)
                    {
                        item.ParentId = matchPa.GetKey();
                    }
                }
                _permissionStore.SaveResource(item);
            });
        }

        private bool IsSuperAdmin(string userKey)
        {
            var superRole = _permissionStore.GetAllRole().FirstOrDefault(a => a.GetName().Equals(DefaultPermission.superAdminRoleName,StringComparison.OrdinalIgnoreCase));
            return _permissionStore.GetAllUserRole().Any(a => a.GetUserKey() == userKey && a.GetRoleKey() == superRole.GetKey());
        }

        /// <summary>
        /// 通过类名和方法名，获取
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private string GetResourceCode(MethodInfo methodInfo)
        {
            if (Attribute.IsDefined(methodInfo, typeof(ResourceAttribute)))
            {
                var attr = methodInfo.GetCustomAttribute<ResourceAttribute>();
                if (attr != null && !string.IsNullOrEmpty(attr.ResourceCode))
                {
                    return attr.ResourceCode;
                }
            }
            return $"{methodInfo.DeclaringType.Name.Replace("Controller", "")}_{methodInfo.Name}";
        }

    }
}
