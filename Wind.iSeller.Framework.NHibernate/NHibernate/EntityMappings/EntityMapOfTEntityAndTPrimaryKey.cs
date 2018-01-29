using System;
using FluentNHibernate.Mapping;
using Wind.iSeller.Framework.Core.Domain.Entities;

namespace Wind.iSeller.Framework.NHibernate.EntityMappings
{
    /// <summary>
    /// This class is base class to map entities to database tables.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Type of primary key of the entity</typeparam>
    public abstract class EntityMap<TEntity, TPrimaryKey> : ClassMap<TEntity> where TEntity : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        protected EntityMap(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) //TODO: Use code contracts?
            {
                throw new ArgumentNullException("tableName");
            }

            Table(tableName);

            //if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
            //    ApplyFilter<MustHaveTenantFilter>();
            //if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
            //    ApplyFilter<MayHaveTenantFilter>();
        }
    }
}