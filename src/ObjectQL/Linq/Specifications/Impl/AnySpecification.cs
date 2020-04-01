/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：AnySpecification
 * 命名空间：ObjectQL.Specifications
 * 文 件 名：AnySpecification
 * 创建时间：2016/10/19 15:08:30
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
    public class AnySpecification<T> : Specification<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public override Expression<Func<T, bool>> Expression => _ => true;
    }
}
