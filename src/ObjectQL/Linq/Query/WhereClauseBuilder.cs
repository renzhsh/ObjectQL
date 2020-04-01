/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：WhereClauseBuilder
 * 命名空间：ObjectQL.Data
 * 文 件 名：WhereClauseBuilder
 * 创建时间：2016/10/19 16:03:08
 * 作    者：renzhsh
 * 说    明：用于创建生成SQL命令的Where子句的类
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Jinhe;
using ObjectQL.Data;

namespace ObjectQL.Linq
{
    /// <summary>
    /// </summary>
    public class WhereClauseBuilder<TTableObject> : ExpressionVisitor
        where TTableObject : class, new()
    {
        #region 常量

        /// <summary>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string WHERE_AND = "AND";

        /// <summary>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string WHERE_OR = "OR";

        /// <summary>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string WHERE_EQUAL = "=";

        /// <summary>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string WHERE_NOT = "NOT";

        /// <summary>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string WHERE_NOT_EQUAL = "<>";

        /// <summary>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string WHERE_LIKE = "LIKE";

        /// <summary>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected const string WHERE_IN = "IN";

        #endregion

        #region 私有字段

        private readonly StringBuilder _sb = new StringBuilder();
        private readonly Dictionary<string, object> _parameterValues = new Dictionary<string, object>();
        private bool _startsWith;
        private bool _endsWith;
        private bool _contains;

        private readonly ICommandBuildProvider _commandBuildProvider;

        /// <summary>
        ///     {Table}.{Field}
        /// </summary>
        private readonly Func<string, string> _getfieldFunc;

        #endregion

        #region 构造函数

        /// <summary>
        /// </summary>
        /// <param name="fieldLoadFunc"></param>
        /// <param name="commandBuildProvider"></param>
        public WhereClauseBuilder(Func<string, string> fieldLoadFunc, ICommandBuildProvider commandBuildProvider)
        {
            _getfieldFunc = fieldLoadFunc;
            _commandBuildProvider = commandBuildProvider;
            ParameterPrefix = _commandBuildProvider.ParameterPrefix;
        }

        #endregion

        #region 私有方法 

        private void Out(string s)
        {
            _sb.Append(s);
        }

        #endregion


        #region where操作关键字

        /// <summary>
        /// </summary>
        protected virtual string And => WHERE_AND;

        /// <summary>
        /// </summary>
        protected virtual string Or => WHERE_OR;

        /// <summary>
        /// </summary>
        protected virtual string Equal => WHERE_EQUAL;

        /// <summary>
        /// </summary>
        protected virtual string Not => WHERE_NOT;

        /// <summary>
        /// </summary>
        protected virtual string NotEqual => WHERE_NOT_EQUAL;

        /// <summary>
        /// </summary>
        protected virtual string Like => WHERE_LIKE;

        /// <summary>
        /// </summary>
        protected virtual string In => WHERE_IN;

        /// <summary>
        /// </summary>
        protected virtual char LikeSymbol => '%';

        #endregion

        #region 保护的方法

        #region Override Visit Methods

        /// <summary>
        /// 取反操作
        /// </summary>
        private bool _isNot;

        /// <summary>
        ///     访问 <see cref="System.Linq.Expressions.BinaryExpression" />.的子表达式
        /// </summary>
        /// <param name="node">表达式</param>
        /// <returns>如果有任何子表达式被修改则返回修改后的表达式；否则返回源表达式；</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            string operateWords;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    operateWords = "+";
                    break;
                case ExpressionType.AddChecked:
                    operateWords = "+";
                    break;
                case ExpressionType.AndAlso:
                    operateWords = And;
                    break;
                case ExpressionType.Divide:
                    operateWords = "/";
                    break;
                case ExpressionType.Equal:
                {
                    operateWords = Equal;
                    break;
                }
                case ExpressionType.GreaterThan:
                    operateWords = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    operateWords = ">=";
                    break;
                case ExpressionType.LessThan:
                    operateWords = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    operateWords = "<=";
                    break;
                case ExpressionType.Modulo:
                    operateWords = "%";
                    break;
                case ExpressionType.Multiply:
                    operateWords = "*";
                    break;
                case ExpressionType.MultiplyChecked:
                    operateWords = "*";
                    break;
                case ExpressionType.Not:
                    operateWords = Not;
                    break;
                case ExpressionType.NotEqual:
                    operateWords = NotEqual;
                    break;
                case ExpressionType.OrElse:
                    operateWords = Or;
                    break;
                case ExpressionType.Subtract:
                    operateWords = "-";
                    break;
                case ExpressionType.SubtractChecked:
                    operateWords = "-";
                    break;
                default:
                    throw new NotSupportedException($"Node type {node.NodeType} is not supported.");
            }
            Out("(");
            Visit(node.Left);
            var constantExpression = node.Right as ConstantExpression;
            if (constantExpression != null && node.Right.NodeType == ExpressionType.Constant &&
                constantExpression.Value == null && !_contains)
            {
                if (node.NodeType == ExpressionType.Equal)
                    Out(" is null ");
                else if (node.NodeType == ExpressionType.NotEqual)
                    Out(" is not null ");
            }
            else
            {
                var rightExpression = node.Right;
                var expression = rightExpression as ConstantExpression;
                if (expression != null && expression.Value == null)
                {
                    Out(" is null ");
                }
                else if (rightExpression is MemberExpression &&
                         Expression.Lambda(node.Right).Compile().DynamicInvoke() == null)
                {
                    Out(" is null ");
                }
                else
                {
                    Out(operateWords);
                    Out(" ");
                    if (rightExpression is UnaryExpression)
                    {
                        var expressionValue = Expression.Lambda(rightExpression).Compile().DynamicInvoke();
                        rightExpression = Expression.Constant(expressionValue);
                    }
                    Visit(rightExpression);
                }
            }
            Out(")");
            return node;
        }

        /// <summary>
        ///     访问 <see cref="System.Linq.Expressions.MemberExpression" />的成员
        /// </summary>
        /// <param name="node">表达式</param>
        /// <returns>如果有任何子表达式被修改则返回修改后的表达式；否则返回源表达式；</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType != null && (node.Member.DeclaringType == typeof(TTableObject) ||
                                                      typeof(TTableObject).IsSubclassOf(node.Member.DeclaringType)))
            {
                var expression = node.Expression as ConstantExpression;
                if (expression != null)
                {
                    var obj = expression.Value;
                    var propertyInfo = obj.GetType().GetProperty(node.Member.Name);
                    if (propertyInfo == null) return node;
                    var fieldValue = propertyInfo.GetValue(obj, null);
                    Expression constantExpr = Expression.Constant(fieldValue);
                    Visit(constantExpr);
                }
                else
                {
                    if (node.ToString().StartsWith("value("))
                    {
                        //node.NodeType == ExpressionType.v
                        var fieldValue = Expression.Lambda(node).Compile().DynamicInvoke();
                        Expression constantExpr = Expression.Constant(fieldValue);
                        Visit(constantExpr);
                    }
                    else
                    {
                        //string mappedFieldName = entityInfo.GetFieldName(node.Member.Name);
                        // Out($"{entityInfo.Table}.{mappedFieldName}");
                        Out(_getfieldFunc(node.Member.Name));
                    }
                }
            }
            else
            {
                var info = node.Member as FieldInfo;
                if (info != null && node.Expression.NodeType == ExpressionType.Constant)
                {
                    // 常量表达式
                    var ce = node.Expression as ConstantExpression;
                    var fi = info;
                    var fieldValue = fi.GetValue(ce?.Value);
                    // 如果是枚举
                    Expression constantExpr = fi.FieldType.IsEnum
                        ? Expression.Constant((int) fieldValue)
                        : Expression.Constant(fieldValue);

                    Visit(constantExpr);
                }
                else
                {
                    try
                    {
                        var fieldValue = Expression.Lambda(node).Compile().DynamicInvoke();
                        Expression constantExpr = Expression.Constant(fieldValue);
                        Visit(constantExpr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{node.Expression}为空值,{ex.Message}");
                    }
                }
                // throw new NotSupportedException($"Member type {node.Member.GetType().FullName} is not supported");
            }
            return node;
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            object v;
            // 条件没有指定字段
            if (node.Value is bool && _sb.Length == 0)
            {
                if (!(bool) node.Value)
                    Out("1<>1");
                return node;
            }
            if (node.Value == null && !_contains)
            {
                Out(" is null ");
                return node;
            }
            var paramName = $"{ParameterPrefix}{ObjectId.GetUniqueIdentifier(5)}";
            var paramNameAfter = string.Empty;
            var connectFixSymbol = _commandBuildProvider.ConnectFixSymbol;
            if (_startsWith && node.Value is string)
            {
                _startsWith = false;
                v = node.Value.ToString();
                paramNameAfter = $"{connectFixSymbol}'{LikeSymbol}'";
            }
            else if (_endsWith && node.Value is string)
            {
                _endsWith = false;
                Out($"'{LikeSymbol}'{connectFixSymbol}");
                v = node.Value.ToString();
            }
            else if (_contains && node.Value is string)
            {
                _contains = false;
                Out($"'{LikeSymbol}'{connectFixSymbol}");
                v = node.Value.ToString();
                paramNameAfter = $"{connectFixSymbol}'{LikeSymbol}'";
                // Out("%'");  + LikeSymbol
            }
            else if (_contains && (node.Value is Array || node.Value is IEnumerable<object>))
            {
                _contains = false;
                var value = node.Value;
                // var s = value.GetType().MakeArrayType();
                var valueType = value.GetType();
                if (valueType.IsIEnumerable())
                    CreateMultiParam(((IEnumerable<string>) value)?.ToArray());
                else if (valueType.IsArray)
                    CreateMultiParam(value as Array);
                else if (value is int[] || value is List<int>)
                    CreateMultiParam((value as IEnumerable<int>).ToArray());
                else if (value is long[] || value is List<long>)
                    CreateMultiParam((value as IEnumerable<long>).ToArray());
                else if (value is string[] || value is List<string>)
                    CreateMultiParam((value as IEnumerable<string>).ToArray());
                return node;
            }
            else if (_contains && node.Value == null)
            {
                _contains = false;
                Out("(null)");
                return node;
            }
            else if (_contains)
            {
                throw new Exception("不支持contains方法， 请使用Array");
            }
            else
            {
                v = node.Value;
            }
            if (!_parameterValues.ContainsKey(paramName))
                _parameterValues.Add(paramName, v);

            Out(paramName);
            Out(paramNameAfter);
            return node;
        }

        /// <summary>
        ///     访问 <see cref="System.Linq.Expressions.MethodCallExpression" /> 的成员
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Out("(");
            if (node.Arguments == null)
                throw new NotSupportedException("Argument length is not valid.");
            Expression expr;
            if (node.Method.Name == "Contains" && node.Object != null && node.Object.Type.IsListGenericType())
            {
                expr = node.Object;
            }
            else
            {
                expr = node.Arguments[0];
                Visit(node.Object);
            }

            switch (node.Method.Name)
            {
                case "StartsWith":
                    _startsWith = true; 
                    Out($" {Like} "); 
                    break;
                case "EndsWith":
                    _endsWith = true;
                    Out($" {Like} ");
                    break;
                case "Equals": 
                    Out($" {Equal} "); 
                    break;
                case "Contains":
                    _contains = true;
                    // 用于匹配 类似 packageIDs.Contains(x.PackageID) 
                    if (node.Object == null && node.Arguments.Count == 2
                        || node.Object != null && node.Object.Type.IsListGenericType())
                    {
                        Visit(node.Arguments.Last());
                        Out(_isNot ? $" {Not} {In} " : $" {In} ");
                    }
                    else
                    {
                        Out($" {Like} ");
                    } 
                    break;
                default:
                    throw new NotSupportedException($"Method {node.Method.Name}{node} is not supported.");
            }
            if (expr is ConstantExpression || expr is MemberExpression)
                Visit(expr);
            else
                throw new NotSupportedException();
            Out(")");
            return node;
        }

        #region 不支持的表达式

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBlock(BlockExpression node)
        {
            throw new NotSupportedException($"VisitBlock：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            throw new NotSupportedException($"VisitCatchBlock：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            throw new NotSupportedException($"VisitConditional：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            throw new NotSupportedException($"VisitDebugInfo：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitDefault(DefaultExpression node)
        {
            throw new NotSupportedException($"VisitDefault：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitDynamic(DynamicExpression node)
        {
            throw new NotSupportedException($"VisitDynamic：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            throw new NotSupportedException($"VisitElementInit：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitGoto(GotoExpression node)
        {
            throw new NotSupportedException($"Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitExtension(Expression node)
        {
            throw new NotSupportedException($"VisitExtension：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitIndex(IndexExpression node)
        {
            throw new NotSupportedException($"VisitIndex：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            throw new NotSupportedException($"VisitInvocation：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitLabel(LabelExpression node)
        {
            throw new NotSupportedException($"VisitLabel：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            throw new NotSupportedException($"VisitLabelTarget：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            throw new NotSupportedException($"VisitLambda：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitListInit(ListInitExpression node)
        {
            throw new NotSupportedException($"VisitListInit：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitLoop(LoopExpression node)
        {
            throw new NotSupportedException($"VisitLoop：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            throw new NotSupportedException($"VisitMemberAssignment：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            throw new NotSupportedException($"VisitMemberBinding：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            throw new NotSupportedException($"VisitMemberInit：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            throw new NotSupportedException(
                $"VisitMemberListBinding：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            throw new NotSupportedException(
                $"VisitMemberMemberBinding：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitNew(NewExpression node)
        {
            throw new NotSupportedException($"VisitNewExpression：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            throw new NotSupportedException($"VisitNewArray：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            throw new NotSupportedException($"VisitParameter：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            throw new NotSupportedException($"VisitRuntimeVariables：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            throw new NotSupportedException($"Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            throw new NotSupportedException($"VisitSwitchCase：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitTry(TryExpression node)
        {
            throw new NotSupportedException($"VisitTry：Node type {node.GetType().Name} is not supported.");
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            throw new NotSupportedException($"VisitTypeBinary：Node type {node.GetType().Name} is not supported.");
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            //Expression.Convert()
            // 
            if (node.NodeType == ExpressionType.Convert)
            { 
                Visit(node.Operand as MemberExpression);
                return (MemberExpression) node.Operand;
            }
            if (node.NodeType != ExpressionType.Not)
                throw new NotSupportedException($"VisitUnary：Node type {node.GetType().Name} is not supported.");
            _isNot = true;
            Visit(node.Operand as MethodCallExpression);
            return (MethodCallExpression)node.Operand;
        }

        #endregion

        #endregion

        #region 公共的属性

        /// <summary>
        ///     变量的前缀（和数据库有关）
        /// </summary>
        public string ParameterPrefix { get; }

        #endregion

        #region IWhereClauseBuilder<T>成员

        /// <summary>
        ///     根据指定的expression对象构造Where子句
        /// </summary>
        /// <param name="expression">expression对象</param>
        /// <returns>返回The <c>WhereClauseBuildResult</c>实例</returns>
        public WhereClauseBuildResult BuildWhereClause(Expression<Func<TTableObject, bool>> expression)
        {
            _sb.Clear();
            _parameterValues.Clear();
            Visit(expression.Body);
            var result = new WhereClauseBuildResult
            {
                ParameterValues = _parameterValues,
                WhereClause = _sb.ToString()
            };
            return result;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="list"></param>
        protected void CreateMultiParam(Array list)
        {
            if (list == null)
            {
                Out(" (null) ");
                return;
            }

            var parameters = new List<string>();
            for (int i = 0, j = list.Length; i < j; i++)
            {
                var item = list.GetValue(i);
                var parameter = $"{ParameterPrefix}{ObjectId.GetUniqueIdentifier(5)}";
                parameters.Add(parameter);
                _parameterValues.Add(parameter, item);
            }
            Out($"({string.Join(",", parameters)})");
        }
    }
}