/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ISpecification
 * 命名空间：ObjectQL.Data.Specifications
 * 文 件 名：ISpecification
 * 创建时间：2016/10/19 14:21:20
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Linq.Expressions;

namespace ObjectQL.Specifications
{

    /// <summary>
    /// 规约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsSatisfiedBy(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        ISpecification<T> And(ISpecification<T> other);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        ISpecification<T> AndNot(ISpecification<T> other);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        ISpecification<T> Or(ISpecification<T> other);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        ISpecification<T> OrNot(ISpecification<T> other);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ISpecification<T> Not(); 

        /// <summary>
        /// 
        /// </summary>
        Expression<Func<T, bool>> Expression { get; }
    }

}
