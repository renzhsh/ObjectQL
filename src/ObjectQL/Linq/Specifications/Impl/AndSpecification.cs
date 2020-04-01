/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：AndSpecification
 * 命名空间：ObjectQL.Specifications
 * 文 件 名：AndSpecification
 * 创建时间：2016/10/19 14:26:12
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
    public class AndSpecification<T> : CompositeSpecification<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public AndSpecification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        /// <summary>
        /// 
        /// </summary>
        public override Expression<Func<T, bool>> Expression => Left.Expression.And(Right.Expression);
    }
}
