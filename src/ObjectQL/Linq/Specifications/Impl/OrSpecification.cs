/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：OrSpecification
 * 命名空间：ObjectQL.Specifications
 * 文 件 名：OrSpecification
 * 创建时间：2016/10/19 15:05:17
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
    /// 
    /// </summary>
    public class OrSpecification<T> : CompositeSpecification<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public OrSpecification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        /// <summary>
        /// 
        /// </summary>
        public override Expression<Func<T, bool>> Expression => Left.Expression.Or(Right.Expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public override bool IsSatisfiedBy(T candidate)
        {
            return left.IsSatisfiedBy(candidate) || right.IsSatisfiedBy(candidate);
        }
    }
}
