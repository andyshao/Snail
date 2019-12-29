﻿using Snail.Core.IPermission;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Snail.Entity
{
    [Table("Role")]
    public class Role :BaseEntity,IRole
    {
        public string Name { get; set; }

        public string GetKey()
        {
            return this.Id;
        }

        public string GetName()
        {
            return this.Name;
        }
    }
}
