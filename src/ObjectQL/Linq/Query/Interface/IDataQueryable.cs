/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IQuery
 * 命名空间：ObjectQL.Data
 * 文 件 名：IQuery
 * 创建时间：2016/10/19 15:35:47
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    public interface IDataQueryable<TFirst>:IQueryExecutor<TFirst>, IAggregateQuery
        where TFirst : class, new()
    {

        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <returns></returns>
        bool Exists();
          

        /// <summary>
        /// 执行排序
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IOrderQuery<TFirst> OrderBy(Expression<Func<TFirst, object>> expression);

        /// <summary>
        /// 指定需跳过的记录数
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        IOrderQuery<TFirst> Skip(int skip);

        /// <summary>
        /// 附加AND查询条件
        /// </summary> 
        /// <param name="expression"></param>
        /// <returns></returns>
        IDataQueryable<TFirst> AppendAndWhere(Expression<Func<TFirst, bool>> expression = null);

        /// <summary>
        /// 附加OR查询条件, 该方法在多表关联的情况下可能有问题，待测试
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        //[Obsolete("过时的方法：AppendOrWhere，正式版将移除本方法", false)]
        IDataQueryable<TFirst> AppendOrWhere(Expression<Func<TFirst, bool>> expression = null);


        /// <summary>
        /// 关联查询
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="joinType">默认使用InnerJoin关联查询</param>
        /// <returns></returns>
        SqlJoinRelevance<TFirst, TFirst, TSecond> Join<TSecond>(Expression<Func<TFirst, object>> firstExpression, Expression<Func<TSecond, object>> secondExpression, JoinType joinType = JoinType.Inner)
            where TSecond : class, new();

        /// <summary>
        /// LeftJoin关联
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param> 
        /// <returns></returns>
        SqlJoinRelevance<TFirst, TFirst, TSecond> LeftJoin<TSecond>(Expression<Func<TFirst, object>> firstExpression, Expression<Func<TSecond, object>> secondExpression)
            where TSecond : class, new();

        /// <summary>
        /// 实体的属性存在关联数据 - 关联实体（表）查询条件 
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        IDataQueryable<TFirst> Exists<TSecond>(Expression<Func<TFirst, object>> firstExpression,
           Expression<Func<TSecond, object>> secondExpression,
           Expression<Func<TSecond, bool>> expression = null)
             where TSecond : class, new();

        /// <summary>
        /// 实体的属性存在关联数据 - 关联实体（表）查询条件 
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        IDataQueryable<TFirst> NotExists<TSecond>(Expression<Func<TFirst, object>> firstExpression,
           Expression<Func<TSecond, object>> secondExpression,
           Expression<Func<TSecond, bool>> expression = null)
             where TSecond : class, new();

        #region 未实现接口
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="TSecond"></typeparam>
//        /// <param name="expression"></param>
//        /// <param name="joinType"></param>
//        /// <returns></returns>
//        IDataQueryable<TFirst> Join<TSecond>(Expression<Func<TFirst, TSecond, bool>> expression, JoinType joinType = JoinType.Inner)
//where TSecond : class, new();
        #endregion

        #region 过时函数
        /// <summary>
        /// 附加AND查询条件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法AppendAndWhere(string text, params object[] args)已经过时", true)]
        IDataQueryable<TFirst> AppendAndWhere(string text, params object[] args);

        /// <summary>
        /// 附加OR查询条件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法AppendOrWhere(string text, params object[] args)已经过时", true)]
        IDataQueryable<TFirst> AppendOrWhere(string text, params object[] args);
        #endregion
    }
}
