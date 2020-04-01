/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：EntityInfo
 * 命名空间：ObjectQL.Data
 * 文 件 名：EntityInfo
 * 创建时间：2016/10/20 9:47:57
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jinhe;
using ObjectQL.Data;

namespace ObjectQL.Mapping
{
    public class EntityInfo<T> : EntityInfo where T : class
    {
        public EntityInfo(string tableName) : base(typeof(T), tableName) { }
    }

    /// <summary>
    /// 实体的关系映射信息
    /// </summary> 
    public class EntityInfo
    {
        private readonly Lazy<ConcurrentDictionary<string, EntityPropertyInfo>> _propertyInfoDictionary =
            new Lazy<ConcurrentDictionary<string, EntityPropertyInfo>>();

        public EntityInfo(Type entityType, string tableName)
        {
            EntityType = entityType;
            TableInfo = new DbTableInfo(tableName);
        }

        public DbTableInfo TableInfo { get; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// 指定为视图对象，禁止insert和delete以及update
        /// </summary>
        public bool IsReadOnly { set; get; }

        /// <summary>
        /// 实体主键信息
        /// </summary>
        public EntityPropertyInfo PrimaryKeyInfo
        {
            get
            {
                return PropertyInfos.Values.Where(item => item.IsPrimary).FirstOrDefault();
            }
        }

        /// <summary>
        /// 实体的属性信息
        /// </summary>
        public ConcurrentDictionary<string, EntityPropertyInfo> PropertyInfos
        {
            get
            {
                return _propertyInfoDictionary.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool Exists(string propertyName)
        {
            return GetPropertyInfo(propertyName) != null;
        }

        /// <summary>
        /// 获取指定名称的属性信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EntityPropertyInfo GetPropertyInfo(string fieldName)
        {
            EntityPropertyInfo result = null;
            if (!PropertyInfos.Keys.Contains(fieldName))
            {
                var entityPropertyInfo = new EntityPropertyInfo(this, fieldName);

                this.PropertyInfos.TryAdd(fieldName, entityPropertyInfo);
            }
            if (PropertyInfos.Keys.Contains(fieldName))
                result = PropertyInfos[fieldName];
            return result;
        }

        /// <summary>
        /// 获取属性映射的字段名称
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        [Obsolete]
        public String GetDbColumnName(string fieldName)
        {
            var property = GetPropertyInfo(fieldName);
            if (property == null)
                throw new ASoftException($"{EntityType.FullName}.{fieldName}没有数据关系映射");
            return property.DbFieldInfo.ColumnName;
        }
    }
}
