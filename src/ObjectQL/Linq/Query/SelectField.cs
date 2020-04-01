/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：SelectField
 * 命名空间：ObjectQL.Data
 * 文 件 名：SelectField
 * 创建时间：2017/4/18 23:50:57
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectQL.Mapping;

namespace ObjectQL.Linq
{
    /// <summary>
    /// sqlBuilder的查询字段
    /// </summary>
    internal class SelectField
    {
        private SqlEntityClauseBuilder _sqlBuilder;

        /// <summary>
        /// sqlBuilder的查询字段
        /// </summary>
        /// <param name="sqlBuilder"></param>
        public SelectField(SqlEntityClauseBuilder sqlBuilder)
        {
            _sqlBuilder = sqlBuilder;
        }

        private DbColumnInfo _field;
        /// <summary>
        /// 字段
        /// </summary>
        [Obsolete]
        internal DbColumnInfo FieldInfo
        {
            set
            {
                _field = value;
                TableName = _field.TableName;
                AliasName = _field.ColumnName;
            }
            get
            {
                return _field;
            }
        }

        internal EntityPropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 字段所属的SqlBuilder在上下文中的索引
        /// </summary>
        protected internal int SqlBuilderIndex
        {
            get
            {
                return _sqlBuilder.Index;
            }
        }

        /// <summary>
        /// 数据库字段名称
        /// </summary>
        public string FieldName
        {
            get
            {
                return FieldInfo.ColumnName;
            }
        }

        /// <summary>
        /// 数据表名称
        /// </summary>
        protected string TableName
        {
            set; get;
        }

        /// <summary>
        /// 字段查询别名(默认与FileName相同)
        /// </summary>
        public string AliasName { set; get; }

        /// <summary>
        /// 字段所属的SqlBuilder的连接类型
        /// </summary>
        public JoinType JoinType { set; get; }

        /// <summary>
        /// 字段对应的Select子语句
        /// </summary>
        public string SelectClause
        {
            get
            {
                var result = $"{this._sqlBuilder.TableName}.{FieldInfo.ColumnName}";
                if (AliasName != FieldName)
                    return $"{result} as {AliasName}";
                return result;
            }
        }

        /// <summary>
        /// 唯一的KEY值
        /// </summary>
        public string Key
        {
            get
            {
                if (_sqlBuilder.Index == 0)
                {
                    return $"{FieldInfo.EntityTypeName}.{this.FieldInfo.PropertyName}";
                }
                if (_sqlBuilder.EntityPropertyInfo != null)
                {
                    return $"{FieldInfo.EntityTypeName}.{_sqlBuilder.EntityPropertyInfo.PropetyName}.{this.FieldInfo.PropertyName}";
                }
                return $"{FieldInfo.EntityTypeName}.{this.FieldInfo.PropertyName}";
            }
        }
    }
}
