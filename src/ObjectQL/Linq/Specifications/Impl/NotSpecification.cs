/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：NotSpecification
 * 命名空间：ObjectQL.Specifications
 * 文 件 名：NotSpecification
 * 创建时间：2016/10/19 15:06:49
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
    public class NotSpecification<T> : Specification<T>
    {
        private ISpecification<T> other;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specification"></param>
        public NotSpecification(ISpecification<T> specification)
        {
            this.other = specification;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Expression<Func<T, bool>> Expression => this.other.Expression.Not();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public override bool IsSatisfiedBy(T candidate)
        {
            return !other.IsSatisfiedBy(candidate);
        }
    }
}
