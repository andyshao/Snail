﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snail.Core;
using Snail.Core.Entity;

namespace Snail.Web.EntityTypeConfiguration
{
    public class BaseConfiguration
    {
        public void Config<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity:class
        {

            if (typeof(IEntityId<string>).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property("Id").HasMaxLength(50);
                builder.HasKey("Id");
            }
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property("IsDeleted").HasConversion<int>();
            }
            if (typeof(IAudit<string>).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property("Creater").HasMaxLength(50);
                builder.Property("Updater").HasMaxLength(50);
            }
        }
    }
}
