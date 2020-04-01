/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ExpressionSpecification
 * 命名空间：ObjectQL.Data.Specifications
 * 文 件 名：ExpressionSpecification
 * 创建时间：2016/10/19 14:15:52
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
    internal sealed class ExpressionSpecification<T> : Specification<T>
    {
        #region 私有字段
        private Expression<Func<T, bool>> expression;
        #endregion

        #region Ctor 
        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            this.expression = expression;
        }
        #endregion

        #region 公共方法
        public override Expression<Func<T, bool>> Expression => this.expression; 
        #endregion
    }
}
