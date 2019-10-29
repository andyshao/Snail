﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Snail.Permission.Core
{
    public interface IUserManager<TUser>
    {
        void Save(TUser user);
        void Delete(TUser user);
    }
}
