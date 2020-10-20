﻿using Snail.Core.Entity;
using Snail.Core.Permission;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snail.Permission.Entity
{
    /// <summary>
    /// 机构
    /// </summary>
    [Table("Org")]

    public class PermissionDefaultOrg : DefaultBaseEntity, IOrg
    {
        public string ParentId { get; set; }
        public string Name { get; set; }

        public string GetKey()
        {
            return this.Id;
        }

        public string GetName()
        {
            return this.Name;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }
    }
}