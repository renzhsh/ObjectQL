/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：SqlRelevance
 * 命名空间：ObjectQL.Data
 * 文 件 名：SqlRelevance
 * 创建时间：2017/4/16 22:14:51
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ObjectQL.Mapping;

namespace ObjectQL.Linq
{
    internal class FieldRelevance
    {
        internal DbColumnInfo FromField { set; get; }

        internal DbColumnInfo ToField { set; get; }

        private string _key;

        internal string Key
        {
            get
            {
                _key = $"{FromField.ColumnName}_EQUEL_{ToField.ColumnName}";
                return _key;
            }
        }
    }

    /// <summary>
    ///     实体关联相关性
    /// </summary>
    /// <typeparam name="TRoot"></typeparam>
    public abstract class SqlJoinRelevance<TRoot>
        where TRoot : class, new()
    {
        /// <summary>
        ///     关联类型
        /// </summary>
        public JoinType JoinType { set; get; }

        /// <summary>
        ///     上下文
        /// </summary>
        internal SqlBuilderContext<TRoot> Context { set; get; }

        /// <summary>
        ///     实体SQL构造器
        /// </summary>
        internal SqlEntityClauseBuilder JoinedSqlBuilder { set; get; }

        /// <summary>
        ///     关联
        /// </summary>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        public SqlJoinRelevance<TRoot, TRoot, TRight> Join<TRight>(Expression<Func<TRoot, object>> firstExpression,
            Expression<Func<TRight, object>> secondExpression, JoinType joinType = JoinType.Inner)
            where TRight : class, new()
        {
            var result = Join(Context.Root, firstExpression, secondExpression, joinType);
            return result;
        }

        /// <summary>
        ///     关联
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="leftBuilder"></param>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        internal SqlJoinRelevance<TRoot, TFrom, TJoin> Join<TFrom, TJoin>(SqlClauseBuilder<TFrom> leftBuilder,
            Expression<Func<TFrom, object>> firstExpression, Expression<Func<TJoin, object>> secondExpression,
            JoinType joinType = JoinType.Inner)
            where TFrom : class, new()
            where TJoin : class, new()
        {
            var joinClauseBuilder = new SqlClauseBuilder<TJoin>(joinType);

            var fieldRelevance = CreateFieldRelevance(firstExpression, secondExpression);

            var result = new SqlJoinRelevance<TRoot, TFrom, TJoin>(Context, leftBuilder, joinClauseBuilder, joinType)
                .AddFieldRelevance(fieldRelevance);

            Context.AddJoinRelevance(result);
            return result;
        }


        /// <summary>
        /// </summary>
        /// <param name="leftExpression"></param>
        /// <param name="rightExpression"></param>
        /// <returns></returns>
        internal FieldRelevance CreateFieldRelevance<TFrom, TJoin>(Expression<Func<TFrom, object>> leftExpression,
            Expression<Func<TJoin, object>> rightExpression)
            where TFrom : class, new()
            where TJoin : class, new()
        {
            var fromField =OrmContext.OrmProvider.GetDbColumnInfo(leftExpression).First();
            var toField = OrmContext.OrmProvider.GetDbColumnInfo(rightExpression).First();
            if (fromField == null && toField == null)
                return null;

            var fieldRelevance = new FieldRelevance
            {
                FromField = fromField,
                ToField = toField
            };
            return fieldRelevance;
        }

        /// <summary>
        ///     查询数据库并返回结果集合
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public abstract IEnumerable<TRoot> Select(Expression<Func<TRoot, object>> expression = null);

        /// <summary>
        ///     参数
        /// </summary>
        public abstract IEnumerable<IDataParameter> JoinParameters { get; }

        /// <summary>
        ///     跳过的记录数
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        public abstract SqlJoinRelevance<TRoot> Skip(int skip);

        /// <summary>
        ///     获取记录数
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public abstract SqlJoinRelevance<TRoot> Take(int take);

        /// <summary>
        ///     升序排序字段
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual SqlJoinRelevance<TRoot> OrderBy(Expression<Func<TRoot, object>> expression)
        {
            Context.SetOrderBy(expression);
            return this;
        }

        /// <summary>
        ///     关联以及条件语句
        /// </summary>
        public abstract string RelevanceClause { get; }

        /// <summary>
        /// </summary>
        public abstract string ExistsClause { get; }

        ///
        public abstract string NotExistsClause { get; }

        /// <summary>
        /// </summary>
        public abstract string FromClause { get; }

        /// <summary>
        ///     倒序排序字段
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual SqlJoinRelevance<TRoot> OrderByDesc(Expression<Func<TRoot, object>> expression)
        {
            Context.SetOrderBy(expression, OrderType.Desc);
            return this;
        }

        /// <summary>
        ///     升序
        /// </summary>
        /// <returns></returns>
        public virtual SqlJoinRelevance<TRoot> Asc()
        {
            Context.SetOrderType(OrderType.Asc);
            return this;
        }

        /// <summary>
        ///     倒序
        /// </summary>
        /// <returns></returns>
        public virtual SqlJoinRelevance<TRoot> Desc()
        {
            Context.SetOrderType(OrderType.Desc);
            return this;
        }

        /// <summary>
        ///     记录数
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return Context.Count();
        }

        /// <summary>
        ///     是否存在记录
        /// </summary>
        /// <returns></returns>
        public virtual bool Exists()
        {
            return Context.Exists();
        }

        /// <summary>
        ///     关联的查询筛选条件
        /// </summary>
        public abstract string WhereClause { get; }
    }

    /// <summary>
    /// </summary>
    public class SqlJoinRelevance<TRoot, TFirst, TSecond> : SqlJoinRelevance<TRoot>
        where TRoot : class, new()
        where TFirst : class, new()
        where TSecond : class, new()
    {
        /// <summary>
        /// </summary>
        private Dictionary<string, FieldRelevance> _fieldRelevances;

        /// <summary>
        /// </summary>
        internal SqlClauseBuilder<TFirst> FromSqlBuilder { get; }

        /// <summary>
        /// </summary>
        internal SqlClauseBuilder<TSecond> ToSqlBuilder { get; }

        /// <summary>
        ///     SQL参数
        /// </summary>
        public override IEnumerable<IDataParameter> JoinParameters
        {
            get
            {
                var result = ToSqlBuilder.DbParameters;
                return result;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fromBuilder"></param>
        /// <param name="toBuilder"></param>
        /// <param name="joinType"></param>
        internal SqlJoinRelevance(SqlBuilderContext<TRoot> context, SqlClauseBuilder<TFirst> fromBuilder,
            SqlClauseBuilder<TSecond> toBuilder, JoinType joinType = JoinType.Inner)
        {
            if (fromBuilder == null || toBuilder == null)
                throw new ArgumentException("fromBuilder 或者toBuilder为空");
            FromSqlBuilder = fromBuilder;
            toBuilder.JoinType = joinType;
            JoinedSqlBuilder = ToSqlBuilder = toBuilder;
            JoinType = joinType;
            Context = context;
        }

        /// <summary>
        ///     字段关联相关性
        /// </summary>
        /// <param name="leftExpression"></param>
        /// <param name="rightExpression"></param>
        /// <returns></returns>
        internal SqlJoinRelevance<TRoot, TFirst, TSecond> AddFieldRelevance(
            Expression<Func<TFirst, object>> leftExpression,
            Expression<Func<TSecond, object>> rightExpression)
        {
            var fieldRelevance = CreateFieldRelevance(leftExpression, rightExpression);
            AddFieldRelevance(fieldRelevance);
            return this;
        }

        /// <summary>
        ///     字段关联相关性
        /// </summary>
        /// <param name="fieldRelevance"></param>
        /// <returns></returns>
        internal SqlJoinRelevance<TRoot, TFirst, TSecond> AddFieldRelevance(FieldRelevance fieldRelevance)
        {
            if (_fieldRelevances == null)
                _fieldRelevances = new Dictionary<string, FieldRelevance>();
            if (!_fieldRelevances.Keys.Contains(fieldRelevance.Key))
                _fieldRelevances.Add(fieldRelevance.Key, fieldRelevance);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        public SqlJoinRelevance<TRoot, TSecond, TOther> JoinNext<TOther>(
            Expression<Func<TSecond, object>> firstExpression, Expression<Func<TOther, object>> secondExpression,
            JoinType joinType = JoinType.Inner)
            where TOther : class, new()
        {
            if (joinType == JoinType.Inner)
                joinType = ToSqlBuilder.JoinType;
            return Join(ToSqlBuilder, firstExpression, secondExpression, joinType);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        public new SqlJoinRelevance<TRoot, TRoot, TOther> Join<TOther>(Expression<Func<TRoot, object>> firstExpression,
            Expression<Func<TOther, object>> secondExpression, JoinType joinType = JoinType.Inner)
            where TOther : class, new()
        {
            return Join(Context.Root, firstExpression, secondExpression, joinType);
        }

        /// <summary>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlJoinRelevance<TRoot, TFirst, TSecond> Load(Expression<Func<TRoot, object>> expression)
        {
            Context.AddLoadProperty(expression, ToSqlBuilder);
            return this;
        }

        /// <summary>
        ///     跳过序列中指定数量的元素，然后返回剩余的元素(默认使用主键排序)
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        public override SqlJoinRelevance<TRoot> Skip(int skip)
        {
            Context.Skip = skip;
            return this;
        }

        /// <summary>
        ///     从序列的开头返回指定数量的连续元素(默认使用主键排序)
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public override SqlJoinRelevance<TRoot> Take(int take)
        {
            Context.Take = take;
            return this;
        }

        /// <summary>
        ///     查询并获取字段
        /// </summary>
        /// <param name="expression">查询的字段（使用后从数据库中将只获取这些字段，暂未支持）</param>
        /// <returns></returns>
        public IEnumerable<TRoot> ExcuteSelect(Expression<Func<TRoot, object>> expression)
        {
            return Context.ExcuteSelect(expression);
        }

        /// <summary>
        ///     查询并获取字段
        /// </summary>
        /// <param name="expression">查询的字段（使用后从数据库中将只获取这些字段，暂未支持）</param>
        /// <returns></returns>
        public override IEnumerable<TRoot> Select(Expression<Func<TRoot, object>> expression = null)
        {
            return ExcuteSelect(expression);
        }

        /// <summary>
        ///     转为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_fieldRelevances == null)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append($" {JoinType} JOIN {ToSqlBuilder.FromClause} ON ");
            sb.Append(RelevanceClause);
            return sb.ToString();
        }

        /// <summary>
        ///     Exists语句
        /// </summary>
        public override string ExistsClause
        {
            get
            {
                if (_fieldRelevances == null)
                    return string.Empty;
                var sb = new StringBuilder();
                sb.Append($" EXISTS (SELECT * FROM {ToSqlBuilder.FromClause} WHERE {RelevanceClause} ");
                if (!string.IsNullOrEmpty(WhereClause))
                    sb.Append($" AND {WhereClause}");
                sb.Append(")");
                return sb.ToString();
            }
        }

        /// <summary>
        ///     NotExists语句
        /// </summary>
        public override string NotExistsClause
        {
            get
            {
                if (_fieldRelevances == null)
                    return string.Empty;
                var sb = new StringBuilder();
                sb.Append($" NOT {ExistsClause}");
                return sb.ToString();
            }
        }

        /// <summary>
        ///     关联语句
        /// </summary>
        public override string RelevanceClause
        {
            get
            {
                var isFirst = true;
                var sb = new StringBuilder();
                foreach (var item in _fieldRelevances)
                {
                    if (!isFirst)
                        sb.Append(" AND ");
                    isFirst = false;
                    if (JoinType == JoinType.Inner && FromSqlBuilder.JoinType == JoinType.Inner)
                        sb.Append(
                            $"{FromSqlBuilder.TableName}.{item.Value.FromField.ColumnName}={ToSqlBuilder.TableName}.{item.Value.ToField.ColumnName}");
                    else if (JoinType != JoinType.Inner && FromSqlBuilder.JoinType == JoinType.Inner)
                        sb.Append(
                            $"{Context.Root.TableName}.{FromSqlBuilder.GetSelectName(item.Value.FromField.ColumnName)}={ToSqlBuilder.TableName}.{item.Value.ToField.ColumnName}");
                    else
                        sb.Append(
                            $"{FromSqlBuilder.TableName}.{FromSqlBuilder.GetSelectName(item.Value.FromField.ColumnName)}={ToSqlBuilder.TableName}.{item.Value.ToField.ColumnName}");
                }
                if (!string.IsNullOrEmpty(ToSqlBuilder.JoinOnWhereClause))
                    sb.Append($" AND {ToSqlBuilder.JoinOnWhereClause} ");
                return $"({sb})";
            }
        }

        /// <summary>
        /// </summary>
        public override string FromClause => ToSqlBuilder.FromClause;

        /// <summary>
        ///     过时的方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Obsolete("过时的方法FilterAndWhere", true)]
        public SqlJoinRelevance<TRoot, TFirst, TSecond> FilterAndWhere(
            Expression<Func<TSecond, bool>> expression = null)
        {
            ToSqlBuilder.AppendAndWhere(expression);
            return this;
        }

        /// <summary>
        ///     JOIN ON的筛选条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlJoinRelevance<TRoot, TFirst, TSecond> JoinOnAndWhere(Expression<Func<TSecond, bool>> expression)
        {
            ToSqlBuilder.JoinOnAndWhere(expression);
            return this;
        }

        /// <summary>
        ///     JOIN ON的筛选条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlJoinRelevance<TRoot, TFirst, TSecond> JoinOnOrWhere(Expression<Func<TSecond, bool>> expression)
        {
            ToSqlBuilder.JoinOnOrWhere(expression);
            return this;
        }

        /// <summary>
        ///     关联后的结果集的筛选条件
        /// </summary>
        public override string WhereClause => ToSqlBuilder.WhereClause;
    }
}
