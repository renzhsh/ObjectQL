/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DbTableField
 * 命名空间：ObjectQL.Data
 * 文 件 名：DbTableField
 * 创建时间：2016/10/21 13:38:30
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/
using System;
namespace ObjectQL.Mapping
{
    /// <summary>
    /// 数据库字段
    /// </summary>
    public class DbColumnInfo
    {
        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { set; get; }

        /// <summary>
        /// 数据字段名称
        /// </summary>
        public string ColumnName { set; get; }

        /// <summary>
        /// 如果该值为空，对应的数据库的缺省表达式。
        /// </summary>
        public object DefaultExpression { get; set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityTypeName { set; get; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { set; get; }

        /// <summary>
        /// 转成字符串 格式：$"{TableName}.{FieldName}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{TableName}.{ColumnName}";
        }

        /// <summary>
        /// 转成字符串 格式：$"{tableAlias}.{FieldName}"
        /// </summary>
        /// <param name="tableAlias">表的别名</param>
        /// <returns></returns>
        //public string ToString(string tableAlias)
        //{
        //    return string.IsNullOrEmpty(tableAlias) ? $"{TableName}.{ColumnName}" : $"{tableAlias}.{ColumnName}";
        //}
    }
}
