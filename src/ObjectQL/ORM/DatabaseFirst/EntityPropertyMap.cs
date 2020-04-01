using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ObjectQL.Mapping
{
    public class EntityPropertyMap
    {
        public EntityPropertyMap(EntityPropertyInfo info)
        {
            Info = info;
        }

        internal EntityPropertyInfo Info { get; }

        /// <summary>
        /// 指定属性映射的数据库字段名称 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EntityPropertyMap HasColumnName(string name)
        {
            Info.HasColumn = true;
            Info.DbFieldInfo.ColumnName = name?.ToUpper();

            return this;
        }

        /// <summary>
        /// 标示为主键
        /// </summary>
        /// <returns></returns>T
        public EntityPropertyMap IsPrimaryKey()
        {
            Info.IsPrimary = true;
            Unique().NotNull();
            return this;
        }

        /// <summary>
        /// 默认值
        /// </summary> 
        /// <param name="defaultExpression"></param>
        /// <returns></returns>
        public EntityPropertyMap Default(object defaultExpression)
        {
            //propertyInfo.CheckNullFieldInfo();
            Info.DbFieldInfo.DefaultExpression = defaultExpression;
            return this;
        }

        /// <summary>
        /// 值唯一
        /// </summary>
        /// <returns></returns>
        public EntityPropertyMap Unique()
        {
            Info.IsUnique = true;
            return this;
        }

        /// <summary>
        /// 不能为空
        /// </summary>
        /// <returns></returns>
        public EntityPropertyMap NotNull()
        {
            Info.IsNotNull = true;
            return this;
        }


        /// <summary>
        /// 设置为只读字段，禁止UPDATE
        /// </summary>
        /// <returns></returns>
        public EntityPropertyMap ReadOnly()
        {
            Info.IsReadOnly = true;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EntityPropertyMap CheckNullFieldInfo()
        {
            return this;
        }
    }
}
