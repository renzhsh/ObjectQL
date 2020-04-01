/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：SqlFromTable
 * 命名空间：ObjectQL.Data
 * 文 件 名：FromTable
 * 创建时间：2017/4/16 14:41:55
 * 作    者：renzhsh
 * 说    明：单个实体的SQL生成器类(Clause)
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ObjectQL.Mapping;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class SqlEntityClauseBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        internal JoinType JoinType { set; get; }

        /// <summary>
        /// 
        /// </summary>
        internal abstract string OriginTable { get; }

        /// <summary>
        /// 
        /// </summary>
        internal abstract IEnumerable<SelectField> SelectFields { get; }

        /// <summary>
        /// 
        /// </summary>
        internal abstract int Index { set; get; }

        /// <summary>
        /// 
        /// </summary>
        internal abstract string TableName { get; }

        /// <summary>
        /// Join On筛选条件
        /// </summary>
        internal abstract string JoinOnWhereClause { get; }

        /// <summary>
        /// Where筛选条件
        /// </summary>
        internal abstract string WhereClause { get; }

        /// <summary>
        /// 
        /// </summary>
        internal abstract IEnumerable<IDataParameter> DbParameters { get; }

        internal EntityPropertyInfo EntityPropertyInfo { get; private set; }

        internal abstract EntityInfo EntityInfo { get;  }

        internal void SetUsageLoadProperty(EntityPropertyInfo propertyInfo)
        {
            EntityPropertyInfo = propertyInfo;
        }
    }
}