﻿using Snail.Core.IPermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snail.Entity
{
    public class RoleResource: BaseEntity,IRoleResource
    {
        public string RoleId { get; set; }
        public string ResourceId { get; set; }

        public string GetResourceKey()
        {
            return this.ResourceId;
        }

        public string GetRoleKey()
        {
            return this.RoleId;
        }
    }
}
