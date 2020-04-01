/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ExpressionFuncExtender
 * 命名空间：ObjectQL.Data.Specifications
 * 文 件 名：ExpressionFuncExtender
 * 创建时间：2016/10/19 14:32:41
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Specifications
{
    /// <summary>
    /// 表达式组合合并
    /// </summary>
    public static class ExpressionFuncExtender
    {
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// 组合表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        private static Expression<T> Compose<T>(this Expression<T> source, Func<Expression, Expression> converter)
        {
            var map = source.Parameters.ToDictionary(p => p);
            var body = ParameterRebinder.ReplaceParameters(map, source.Body);
            return Expression.Lambda<T>(converter(body), source.Parameters);
        }

        #region 公共方法
        /// <summary>
        /// 用AND组合两个给定的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first">第一个表达式</param>
        /// <param name="second">第二个表达式</param>
        /// <returns>组合表达式</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// 用OR组合两个给定的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first">第一个表达式</param>
        /// <param name="second">第二个表达式</param>
        /// <returns>组合表达式</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> source)
        {
            return source.Compose(Expression.Not);
        }
        #endregion
    }
}
