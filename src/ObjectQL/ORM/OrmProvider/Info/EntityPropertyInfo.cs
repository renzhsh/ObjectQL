/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：EntityPropertyInfo
 * 命名空间：ObjectQL.Data
 * 文 件 名：EntityPropertyInfo
 * 创建时间：2016/10/20 9:50:00
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using ObjectQL.Data;
using ObjectQL.Linq;

namespace ObjectQL.Mapping
{
    /// <summary>
    /// 实体的属性信息
    /// </summary>
    public class EntityPropertyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityInfo"></param>
        /// <param name="propertyName"></param>
        public EntityPropertyInfo(EntityInfo entityInfo, string propertyName)
        {
            EntityInfo = entityInfo;
            PropertyInfo = entityInfo.EntityType.GetProperty(propertyName);

            DbFieldInfo = new DbColumnInfo()
            {
                TableName = EntityInfo.TableInfo.TableName,
                //FieldName = FieldInfo.Name,
                PropertyName = PropertyInfo.Name,
                EntityTypeName = EntityInfo.EntityType.FullName
            };
        }

        /// <summary>
        ///  实体信息
        /// </summary>
        internal EntityInfo EntityInfo { get; }

        /// <summary>
        /// 实体熟悉映射的数据库字段全名（含表名）
        /// </summary>
        internal DbColumnInfo DbFieldInfo { get; set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        protected PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType
        {
            get
            {
                return PropertyInfo.PropertyType;
            }
        }


        /// <summary>
        /// 是引用类型
        /// </summary>
        public bool IsComplex => !PropertyType.IsValueType
                                 && PropertyType.FullName != "System.String"
                                 && PropertyType.FullName != "System.Char[]"
                                 && PropertyType.FullName != "System.Byte[]";

        /// <summary>
        /// 是否是枚举类型
        /// </summary>
        public bool IsEnum => PropertyType.BaseType.FullName == "System.Enum";

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropetyName
        {
            get
            {
                return PropertyInfo.Name;
            }
        }

        /// <summary>
        /// 只读
        /// </summary>
        public bool IsReadOnly { internal set; get; }

        /// <summary>
        /// 不能为空
        /// </summary>
        public bool IsNotNull { internal set; get; }

        /// <summary>
        /// 唯一
        /// </summary>
        public bool IsUnique { internal set; get; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool IsPrimary { internal set; get; }

        /// <summary>
        /// 有字段
        /// </summary>
        public bool HasColumn { get; internal set; }

        public void SetValue(object obj, object value, object[] index)
        {
            PropertyInfo.SetValue(obj, value, index);
        }

        public object GetValue(object obj, object[] index)
        {
            return PropertyInfo.GetValue(obj, index);
        }


    }
}
