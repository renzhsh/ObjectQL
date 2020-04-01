/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IWhereClauseBuilder
 * 命名空间：ObjectQL.Data
 * 文 件 名：IWhereClauseBuilder
 * 创建时间：2016/10/19 16:04:33
 * 作    者：renzhsh
 * 说    明：用于创建生成SQL命令的Where子句的接口
 * 修改时间：
 * 修 改 人：
*************************************************************************************/


using System;
using System.Linq.Expressions;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTableObject"></typeparam>
    public interface IWhereClauseBuilder<TTableObject>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        WhereClauseBuildResult BuildWhereClause(Expression<Func<TTableObject, bool>> expression);
    }
}
