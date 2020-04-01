/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IQueryExecutor
 * 命名空间：ObjectQL.Data
 * 文 件 名：IQueryExecutor
 * 创建时间：2018/2/5 13:18:10
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IQueryExecutor<E>:IAggregateQuery
        where E : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        SqlBuilderContext<E> BuilderContext { get; }

        /// <summary>
        /// 执行查询并获取记录
        /// </summary>
        /// <param name="expression">只返回表达式指定的成员（只针对主实体类型，不针对Load的属性）</param>
        /// <returns></returns>
        IEnumerable<E> Select(Expression<Func<E, object>> expression = null);

        /// <summary>
        /// 执行查询,
        /// </summary>
        /// <typeparam name="TResult">返回的Model类型</typeparam>
        /// <param name="selector">Select</param>
        /// <returns></returns>
        IEnumerable<TResult> SelectToModel<TResult>(Func<E, TResult> selector);

        /// <summary>
        /// 执行查询并获取记录
        /// </summary>
        /// <param name="total"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<E> Select(out int total, Expression<Func<E, object>> expression = null);

        /// <summary>
        /// 查询并返回列表
        /// </summary>
        /// <returns></returns>
        List<E> ToList();

        /// <summary>
        /// 用一个完整的语句查询并映射成实体类
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法已经过期", true)]
        IEnumerable<E> QueryAndSelect(string commandText, params object[] args);

        /// <summary>
        ///  用一个完整的语句查询并映射成动态类型
        /// </summary>
        /// <typeparam name="T">与T实体类同一个库中执行SQL</typeparam>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法已经过期", true)]
        IEnumerable<dynamic> QueryDynamicFrom(string commandText, params object[] args);
    }
}
