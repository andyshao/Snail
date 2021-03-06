﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Snail.Core.Entity
{
    /// <summary>
    /// 包含主键的entity
    /// </summary>
    /// <typeparam name="T">主键的类型</typeparam>
    public interface IEntityId<T>: IIdField<T>,IEntity
    {
    }
}
