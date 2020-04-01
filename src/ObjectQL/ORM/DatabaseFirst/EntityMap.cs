/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：EntityMap
 * 命名空间：ObjectQL.Data
 * 文 件 名：EntityMap
 * 创建时间：2016/10/20 21:32:41
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Linq.Expressions;

namespace ObjectQL.Mapping
{
    /// <summary>
    /// 实体映射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityMap<T> : EntityMap
        where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EntityPropertyMap Property<TResult>(Expression<Func<T, TResult>> expression)
        {
            return Property<T, TResult>(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEntityMap ToTable(string name)
        {
            return base.ToTable<T>(name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class EntityMap : IEntityMap
    {
        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Mapping();

        /// <summary>
        /// 已映射
        /// </summary>
        protected bool HasMapped => EntityInfo != null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EntityPropertyMap Property<T, TResult>(Expression<Func<T, TResult>> expression)
            where T : class
        {
            var me = (MemberExpression)(expression.Body);
            if (!HasMapped)
                throw new Exception($"{typeof(T).FullName}未指定关系对象映射信息");

            var info = EntityInfo.GetPropertyInfo(me.Member.Name);

            return new EntityPropertyMap(info);
        }

        /// <summary>
        /// 实体映射的数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEntityMap ToTable<T>(string name) where T : class
        {
            Type sourceType = typeof(T);
            EntityInfo = new EntityInfo<T>(name);
            return this;
        }

        /// <summary>
        /// 指定为视图对象，禁止insert和delete以及update
        /// </summary>
        /// <returns></returns>
        public IEntityMap ReadOnly<T>()
        {
            if (!HasMapped)
            {
                throw new Exception($"{typeof(T).FullName}未指定关系对象映射信息");
            }
            EntityInfo.IsReadOnly = true;
            return this;
        }
    }
}
