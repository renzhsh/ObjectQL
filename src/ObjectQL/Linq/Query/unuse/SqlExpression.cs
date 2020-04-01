/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：SqlExpression
 * 命名空间：ObjectQL.Data
 * 文 件 名：SqlExpression
 * 创建时间：2017/4/26 14:27:12
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
using System.Xml.Linq;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SqlExpression
    {
        #region 常量
        /// <summary>
        /// 
        /// </summary>
        protected const string WHERE_AND = "AND";

        /// <summary>
        /// 
        /// </summary>
        protected const string WHERE_OR = "OR";
        /// <summary>
        /// 
        /// </summary>
        protected const string WHERE_EQUAL = "=";
        /// <summary>
        /// 
        /// </summary>
        protected const string WHERE_NOT = "NOT";
        /// <summary>
        /// 
        /// </summary>
        protected const string WHERE_NOT_EQUAL = "<>";
        /// <summary>
        /// 
        /// </summary>
        protected const string WHERE_LIKE = "LIKE";
        /// <summary>
        /// 
        /// </summary>
        protected const string WHERE_IN = "IN";

        /// <summary>
        /// 大于
        /// </summary>
        protected const string GREATER_THAN = ">";

        protected const string MODULO = "%";

        /// <summary>
        /// 大于等于
        /// </summary>
        protected const string GREATER_THAN_EQUAL = ">=";

        protected const string LESS_THAN = "<";

        protected const string LESS_THAN_OR_EQUAL = "<=";

        protected const string MULTIPLY = "*";
        #endregion 

        /// <summary>
        /// 
        /// </summary>
        public SqlExpression NextNode = null;

        /// <summary>
        /// 
        /// </summary>
        public SqlExpression PreNode = null;

        /// <summary>
        /// 
        /// </summary>
        public SqlExpression LeftNode = null;


        /// <summary>
        /// 
        /// </summary>
        public SqlExpression RightNode = null;

        /// <summary>
        /// 
        /// </summary>
        public SqlExpression ParentNode = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void AppendPre(SqlExpression node)
        {
            this.PreNode = node;
            this.PreNode.NextNode = this;
        }

        /// <summary>
        /// where操作关键字
        /// </summary>
        public string Operator { protected set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public void AppendNext(SqlExpression next)
        {

            this.NextNode = next;
            this.NextNode.PreNode = this;
        }

        public void AddLeftChildren(SqlExpression node)
        {
            this.LeftNode = node;
            node.ParentNode = this;
        }

        public void AddRightChildren(SqlExpression node)
        {
            this.RightNode = node;
            node.ParentNode = this;
        }

        /// <summary>
        /// 根据表达式获取WHERE关键字
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public static string GetKeywords(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.AndAlso:
                    return WHERE_AND;
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return WHERE_EQUAL;
                case ExpressionType.GreaterThan:
                    return GREATER_THAN;
                case ExpressionType.GreaterThanOrEqual:
                    return GREATER_THAN_EQUAL;
                case ExpressionType.LessThan:
                    return LESS_THAN;
                case ExpressionType.LessThanOrEqual:
                    return LESS_THAN_OR_EQUAL;
                case ExpressionType.Modulo:
                    return MODULO;
                case ExpressionType.Multiply:
                    return MULTIPLY;
                case ExpressionType.MultiplyChecked:
                    return MULTIPLY;
                case ExpressionType.Not:
                    return WHERE_NOT;
                case ExpressionType.NotEqual:
                    return WHERE_NOT_EQUAL;
                case ExpressionType.OrElse:
                    return WHERE_OR;
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.SubtractChecked:
                    return "-";
                default:
                    throw new NotSupportedException($"Node type {nodeType.ToString()} is not supported.");
            }
        }

        public virtual bool IsLoicalOperators
        {
            get
            {
                if ((this.Operator == WHERE_AND || this.Operator == WHERE_OR))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetKeywords(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "StartsWith": 
                    return WHERE_LIKE;
                case "EndsWith":
                    return WHERE_LIKE;
                case "Equals":
                    return WHERE_EQUAL;
                case "Contains":
                    if ((node.Object == null && node.Arguments.Count == 2)
                      || (node.Object != null && node.Object.Type.IsListGenericType()))
                    {
                        return WHERE_IN;
                    }
                    else
                    {
                        return WHERE_LIKE;
                    }
                default:
                    throw new NotSupportedException($"Method {node.Method.Name}{node} is not supported.");
            }
        }

        public bool IsStartWith {
            protected set; get;
        }
        internal abstract void Append(object expression);
    }
}
