/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ParameterRebinder
 * 命名空间：ObjectQL.Data.Specifications
 * 文 件 名：ParameterRebinder
 * 创建时间：2016/10/19 14:34:11
 * 作    者：renzhsh
 * 说    明：老外写的类
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectQL.Specifications
{
    /// <summary>
    /// 
    /// </summary>
    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        #region 构造函数
        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        #endregion

        internal static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}
