/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：Specification
 * 命名空间：ObjectQL.Specifications
 * 文 件 名：Specification
 * 创建时间：2016/10/19 14:14:04
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
    /// 
    /// </summary>
    public abstract class Specification<T> : ISpecification<T>
    {
        /// <summary>
        /// 给定的对象是否满足规约
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>符合规约</returns>
        public virtual bool IsSatisfiedBy(T obj)
        {
            return this.Expression.Compile()(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ISpecification<T> And(ISpecification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ISpecification<T> Or(ISpecification<T> other)
        {
            return new OrSpecification<T>(this, other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ISpecification<T> AndNot(ISpecification<T> other)
        {
            return new AndNotSpecification<T>(this, other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ISpecification<T> OrNot(ISpecification<T> other)
        {
            return new OrNotSpecification<T>(this, other);
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract Expression<Func<T, bool>> Expression { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification) => specification.Expression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public static implicit operator Specification<T>(Expression<Func<T, bool>> expression) => new ExpressionSpecification<T>(expression);

    }
}
