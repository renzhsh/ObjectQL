/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：MapRegi
 * 命名空间：ObjectQL.WeChat.App.Repositories
 * 文 件 名：MapRegi
 * 创建时间：2016/12/18 10:53:45
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using ObjectQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Linq
{

    /// <summary>
    ///  
    /// </summary>
    public class BinaryExpressionSqlWhereResult : SqlExpression
    {
        private bool _setedLeft = false;
        /// <summary>
        /// 右边
        /// </summary>
        private bool _setedRight = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public BinaryExpressionSqlWhereResult(string opt)
        {
            this.Operator = opt;
        }

        public BinaryExpressionSqlWhereResult()
        {

        }

        private string _left;
        /// <summary>
        /// 左边表达式
        /// </summary>
        public string Left
        {
            private set
            {
                _setedLeft = true;
                _left = value;
            }
            get
            {
                return _left;
            }
        }



        private object _right;
        /// <summary>
        /// 右边表达式
        /// </summary>
        public object Right
        {
            private set
            {
                _right = value;
                _setedRight = true;
            }
            get
            {
                return _right;
            }
        }

        /// <summary>
        /// 设置左边表达式
        /// </summary>
        /// <param name="left"></param>
        public void SetLeft(string left)
        {
            this.Left = left;
        }

        /// <summary>
        /// 设置右边表达式
        /// </summary>
        /// <param name="right"></param>
        public void SetRight(object right)
        {
            if (right == null || right == DBNull.Value)
            {
                if (this.Operator == WHERE_EQUAL)
                {
                    this.Operator = string.Empty;
                    this.Right = "IS NULL";
                }
                else if (this.Operator == WHERE_NOT_EQUAL)
                {
                    this.Operator = string.Empty;
                    this.Right = "IS NOT NULL";
                }
                else {
                    this.Right = null;
                }
            }
            else
            {
                this.Right = right;
            }
        }



        internal void SetOperator(string opt)
        {
            this.Operator = opt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Operator == WHERE_IN && (this.Right == null || this.Right == DBNull.Value))
                return "1<>1";
            return $"{this.Left} {this.Operator} {this.Right}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected bool CheckExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return false;
            var check = expression.Replace(" ", "")
                                  .Replace("(", "")
                                  .Replace(")", "");
            if (string.IsNullOrEmpty(check))
                return false;
            return true;
        }

        internal override void Append(object expression)
        {
            if (expression != null && expression is List<string>)
            {
                if ((expression as List<string>).Any())
                {
                    SetRight($"({string.Join(",", expression as List<string>)})"); 
                }
                return;
            }
            if (!_setedRight)
                SetRight(expression);
            else
                SetLeft(expression?.ToString());
        }
    }
}
