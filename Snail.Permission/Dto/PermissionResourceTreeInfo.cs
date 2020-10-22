﻿using Snail.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snail.Permission.Dto
{
    public class PermissionResourceTreeInfo : IIdField<string>
    {
        /// <summary>
        /// 资源id
        /// </summary>
        public string Id { get; set; }

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
        /// <summary>
        /// 子资源
        /// </summary>
        public List<PermissionResourceTreeInfo> Children { get; set; }
        public bool HasChildren { get { return Children != null && Children.Count > 0; } }

    }
}
