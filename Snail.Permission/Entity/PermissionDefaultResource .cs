﻿using Snail.Core.Entity;
using Snail.Core.Permission;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snail.Permission.Entity
{
    [Table("Resource")]
    public class PermissionDefaultResource : DefaultBaseEntity, IResource
    {
        /// <summary>
        /// 资源键，如接口名，菜单名，唯一键
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 资源描述，如接口的名称、菜单的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父资源id
        /// </summary>
        public string ParentId { get; set; }

        public string GetKey()
        {
            return this.Id;
        }

        public string GetName()
        {
            return this.Name;
        }

        public string GetResourceCode()
        {
            return this.Code;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }
    }
}
