/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：RelationContext
 * 命名空间：ObjectQL.Data
 * 文 件 名：RelationContext
 * 创建时间：2017/4/14 15:06:38
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
using System.Reflection;
using System.Text;
using ObjectQL.Mapping;
using ObjectQL.Data;

namespace ObjectQL.Linq
{
    /// <summary>
    ///     排序类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        ///     升序
        /// </summary>
        Asc = 0,

        /// <summary>
        ///     倒序
        /// </summary>
        Desc = 1
    }

    /// <summary>
    ///     关联上下文
    /// </summary>
    public class SqlBuilderContext<TFirst>
        where TFirst : class, new()
    {
        #region 私有

        private readonly object _lock = new object();
        private List<IDataParameter> _dataParameters;

        /// <summary>
        ///     查询委托
        /// </summary>
        private readonly Func<Expression<Func<TFirst, object>>, IEnumerable<TFirst>> _excuteSelectFunc;

        /// <summary>
        ///     统计委托
        /// </summary>
        private readonly Func<int> _excuteCount;

        /// <summary>
        ///     判断记录是否存在的委托
        /// </summary>
        private readonly Func<bool> _excuteExists;

        /// <summary>
        ///     数据查询命令的支持类
        /// </summary>
        private readonly ICommandBuildProvider _commandBuildProvider;

        /// <summary>
        ///     排序字段
        /// </summary>
        private IEnumerable<DbColumnInfo> _orderFields;

        /// <summary>
        ///     排序类型
        /// </summary>
        private OrderType _orderType;

        /// <summary>
        /// </summary>
        private EntityInfo _entityInfo;

        #endregion

        #region 保护的属性

        /// <summary>
        ///     需要加载的关联表属性与该表SQL构造器
        /// </summary>
        internal Dictionary<EntityPropertyInfo, SqlEntityClauseBuilder> LoadedPropertys { private set; get; }

        /// <summary>
        ///     需加载的关联表属性
        /// </summary>
        protected Dictionary<string, EntityPropertyInfo> LoadedMembers;

        /// <summary>
        ///     等值连接
        /// </summary>
        protected List<SqlJoinRelevance<TFirst>> InnerRelevances = new List<SqlJoinRelevance<TFirst>>();

        /// <summary>
        /// </summary>
        protected List<SqlJoinRelevance<TFirst>> ExistsRelevances;

        /// <summary>
        /// </summary>
        protected List<SqlJoinRelevance<TFirst>> NotExistsRelevances;

        /// <summary>
        ///     左连接或者右连接
        /// </summary>
        protected List<SqlJoinRelevance<TFirst>> LeftOrRightRelevances = new List<SqlJoinRelevance<TFirst>>();

        /// <summary>
        ///     关联后的Where筛选条件（需附加）
        /// </summary>
        protected List<string> JoinWhereList;

        /// <summary>
        ///     当前的索引
        /// </summary>
        protected int CurrentIndex { get; private set; }

        /// <summary>
        ///     当前关联的SqlClauseBuilder
        /// </summary>
        internal SqlEntityClauseBuilder Current { private set; get; }

        /// <summary>
        ///     跳过记录（如果不指定排序则默认使用主键升序排序）
        /// </summary>
        protected internal int Skip { set; get; }

        /// <summary>
        ///     获取记录（如果不指定排序则默认使用主键升序排序）
        /// </summary>
        protected internal int Take { set; get; }

        /// <summary>
        ///     属性与数据库的映射关系
        /// </summary>
        internal Dictionary<string, SelectField> PropertyNamesMap;

        /// <summary>
        ///     查询的主实体信息
        /// </summary>
        protected EntityInfo EntityInfo => _entityInfo ?? (_entityInfo = OrmContext.OrmProvider.GetEntityInfo<TFirst>());

        /// <summary>
        ///     内连接
        /// </summary>
        protected string InnerJoin
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in InnerRelevances)
                {
                    DataParameters.AddRange(item.JoinParameters);
                    sb.Append(item);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        ///     返回Exits条件的子语句
        /// </summary>
        protected string ExistsClause
        {
            get
            {
                List<string> existsClauseList = null;
                if (ExistsRelevances != null && ExistsRelevances.Any())
                {
                    existsClauseList = new List<string>();
                    foreach (var item in ExistsRelevances)
                    {
                        DataParameters.AddRange(item.JoinParameters);
                        existsClauseList.Add(item.ExistsClause);
                    }
                }
                if (NotExistsRelevances != null && NotExistsRelevances.Any())
                {
                    if (existsClauseList == null)
                        existsClauseList = new List<string>();
                    foreach (var item in NotExistsRelevances)
                    {
                        DataParameters.AddRange(item.JoinParameters);
                        existsClauseList.Add(item.NotExistsClause);
                    }
                }
                if (existsClauseList != null && existsClauseList.Any())
                    return string.Join(" AND ", existsClauseList);
                return string.Empty;
            }
        }

        /// <summary>
        ///     InnerJoin条件转换为Exists语句
        /// </summary>
        protected string InnerJoinExists
        {
            get
            {
                if (InnerRelevances == null || !InnerRelevances.Any())
                    return string.Empty;

                var sb = new StringBuilder();
                sb.Append(" EXISTS (SELECT * ");
                var fromClauseList = new List<string>();
                var whereClauseList = new List<string>();
                foreach (var item in InnerRelevances)
                {
                    DataParameters.AddRange(item.JoinParameters);
                    fromClauseList.Add(item.FromClause);
                    whereClauseList.Add(item.RelevanceClause);
                }
                if (fromClauseList.Any() && whereClauseList.Any())
                {
                    sb.Append($" FROM {string.Join(", ", fromClauseList)}");
                    sb.Append($" WHERE {string.Join(" AND ", whereClauseList)}");
                }
                sb.Append(")");

                var whereClause = WhereClause;
                var inner = string.IsNullOrEmpty(whereClause)
                    ? $"SELECT {EntityInfo.PrimaryKeyInfo.DbFieldInfo} FROM {Root.TableName} WHERE {sb} "
                    : $"SELECT {EntityInfo.PrimaryKeyInfo.DbFieldInfo} FROM {Root.TableName} {WhereClause} AND {sb}";
                inner =
                    $"(SELECT * FROM {Root.TableName} WHERE EXISTS  (SELECT {_entityInfo.PrimaryKeyInfo.DbFieldInfo} FROM ({inner}) AA WHERE {_entityInfo.PrimaryKeyInfo.DbFieldInfo}=AA.{_entityInfo.PrimaryKeyInfo.DbFieldInfo.ColumnName} GROUP BY {_entityInfo.PrimaryKeyInfo.DbFieldInfo}) {OrderByClause})";
                return inner;
            }
        }


        /// <summary>
        ///     对关联后的结果集进行筛选（一般出现在存在LEFT（RIGHT） JOIN的情况下）
        /// </summary>
        protected string ExtraWhereClause
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in LeftOrRightRelevances)
                {
                    DataParameters.AddRange(item.JoinParameters);
                    sb.Append(item);
                    var whereClause = item.WhereClause;
                    if (string.IsNullOrEmpty(whereClause)) continue;
                    if (JoinWhereList == null)
                        JoinWhereList = new List<string>();
                    if (!JoinWhereList.Contains(whereClause))
                        JoinWhereList.Add(whereClause);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        ///     附加的非等值连接的条件（注意Join On中的筛选条件与Where中的筛选条件的差别）
        /// </summary>
        protected string AppendNotInnerWhere
        {
            get
            {
                if (JoinWhereList != null)
                    return $" WHERE {string.Join("AND", JoinWhereList)}";
                return string.Empty;
            }
        }

        /// <summary>
        ///     等值连接的查询字段
        /// </summary>
        protected string InnerSelectClause
        {
            get
            {
                return string.Join(",", PropertyNamesMap.Where(item => item.Value.JoinType == JoinType.Inner)
                    .Select(item => item.Value.SelectClause));
            }
        }

        /// <summary>
        ///     查询字段
        /// </summary>
        protected string OtherSelectClause
        {
            get
            {
                return string.Join(",",
                    PropertyNamesMap.Where(item => item.Value.JoinType != JoinType.Inner)
                        .Select(item => item.Value.SelectClause));
            }
        }

        #endregion

        /// <summary>
        ///     查询的主表
        /// </summary>
        internal SqlClauseBuilder<TFirst> Root { get; }

        /// <summary>
        ///     SQL构造上下文
        /// </summary>
        public SqlBuilderContext(
            Func<Expression<Func<TFirst, object>>, IEnumerable<TFirst>> selectFunc = null,
            Func<int> countFunc = null,
            Func<bool> existsFunc = null)
        {
            var firstSqlClauseBuilder = new SqlClauseBuilder<TFirst> { Index = CurrentIndex };
            AddSelect(firstSqlClauseBuilder.SelectFields);
            Root = firstSqlClauseBuilder;
            Current = Root;
            _excuteSelectFunc = selectFunc;
            _excuteCount = countFunc;
            _excuteExists = existsFunc;
            _commandBuildProvider = OrmContext.DriverProviders.GetCommandBuildProvider<TFirst>();
        }

        #region internal 方法

        /// <summary>
        ///     附加OR条件，与之前的条件之间的或运算
        /// </summary>
        /// <param name="expression"></param>
        internal void AppendOrWhere(Expression<Func<TFirst, bool>> expression = null)
        {
            Root.AppendOrWhere(expression);
        }

        /// <summary>
        /// </summary>
        /// <param name="expression"></param>
        internal void AppendAndWhere(Expression<Func<TFirst, bool>> expression = null)
        {
            Root.AppendAndWhere(expression);
        }

        /// <summary>
        /// </summary>
        internal List<IDataParameter> DataParameters => _dataParameters ??
                                                        (_dataParameters = new List<IDataParameter>());

        /// <summary>
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        internal SqlJoinRelevance<TFirst, TFirst, TSecond> Join<TSecond>(
            Expression<Func<TFirst, object>> firstExpression,
            Expression<Func<TSecond, object>> secondExpression,
            JoinType type = JoinType.Inner)
            where TSecond : class, new()
        {
            lock (_lock)
            {
                var sqlClauseBuilder = new SqlClauseBuilder<TSecond>();
                var relevance = new SqlJoinRelevance<TFirst, TFirst, TSecond>(this, Root, sqlClauseBuilder, type)
                    .AddFieldRelevance(firstExpression, secondExpression);
                AddJoinRelevance(relevance);
                return relevance;
            }
        }

        internal SqlJoinRelevance<TFirst, TFirst, TSecond> Exists<TSecond>(
            Expression<Func<TFirst, object>> firstExpression,
            Expression<Func<TSecond, object>> secondExpression,
            Expression<Func<TSecond, bool>> expression = null,
            bool isExists = true)
            where TSecond : class, new()
        {
            lock (_lock)
            {
                var sqlClauseBuilder = new SqlClauseBuilder<TSecond>();
                var relevance = new SqlJoinRelevance<TFirst, TFirst, TSecond>(this, Root, sqlClauseBuilder)
                    .AddFieldRelevance(firstExpression, secondExpression);

                relevance.ToSqlBuilder.AppendAndWhere(expression);

                if (isExists)
                    AddExistsRelevance(relevance);
                else
                    AddNotExistsRelevance(relevance);
                return relevance;
            }
        }

        private Expression<Func<TFirst, object>> _selectExpression;

        private IEnumerable<DbColumnInfo> _selectedFileds;

        /// <summary>
        ///     重新设置了查询字段
        /// </summary>
        private bool _isResetFields = true;

        /// <summary>
        ///     查询的字段
        /// </summary>
        protected IEnumerable<DbColumnInfo> SelectedFileds
        {
            get
            {
                if (_isResetFields)
                {
                    _selectedFileds = OrmContext.OrmProvider.GetDbColumnInfo(_selectExpression);
                    if (!_selectedFileds.Contains(EntityInfo.PrimaryKeyInfo.DbFieldInfo))
                        _selectedFileds = _selectedFileds.Concat(new[] { EntityInfo.PrimaryKeyInfo.DbFieldInfo });
                }
                return _selectedFileds;
            }
        }

        /// <summary>
        ///     设置主表中只需要查询的属性字段
        /// </summary>
        /// <param name="expression">属性</param>
        internal IEnumerable<DbColumnInfo> SetSelect(Expression<Func<TFirst, object>> expression = null)
        {
            _selectExpression = expression;
            _isResetFields = true;
            return SelectedFileds;
        }

        #region 排序

        /// <summary>
        ///     设置排序
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderType"></param>
        internal void SetOrderBy(Expression<Func<TFirst, object>> expression, OrderType orderType = OrderType.Asc)
        {
            _orderFields = OrmContext.OrmProvider.GetDbColumnInfo(expression);
            SetOrderBy(_orderFields, orderType);
        }

        internal void SetOrderBy(IEnumerable<DbColumnInfo> orderByFields, OrderType orderType = OrderType.Asc)
        {
            if (orderByFields == null)
                return;
            _orderFields = orderByFields;
            SetOrderType(orderType);
        }


        /// <summary>
        /// </summary>
        /// <param name="orderType"></param>
        internal void SetOrderType(OrderType orderType)
        {
            _orderType = orderType;
        }

        /// <summary>
        /// </summary>
        internal string OrderByClause
        {
            get
            {
                if (_orderFields == null || !_orderFields.Any())
                    return string.Empty;

                var orderNames = _orderFields.Select(item =>
                {
                    return $"{item.TableName}.{item.ColumnName}";
                });
                var result = $"ORDER BY {string.Join(",", orderNames)} {_orderType}";
                return result;
            }
        }

        #endregion

        #endregion

        #region protected internal方法

        /// <summary>
        ///     增加新的关联关系
        /// </summary>
        /// <param name="relevance"></param>
        protected internal void AddJoinRelevance(SqlJoinRelevance<TFirst> relevance)
        {
            relevance.JoinedSqlBuilder.Index = ++CurrentIndex;
            Current = relevance.JoinedSqlBuilder;
            if (relevance.JoinType == JoinType.Inner)
            {
                InnerRelevances.Add(relevance);
            }
            else
            {
                LeftOrRightRelevances.Add(relevance);
                AddSelect(relevance.JoinedSqlBuilder.SelectFields, relevance.JoinedSqlBuilder.JoinType);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="relevance"></param>
        protected internal void AddExistsRelevance(SqlJoinRelevance<TFirst> relevance)
        {
            if (ExistsRelevances == null)
                ExistsRelevances = new List<SqlJoinRelevance<TFirst>>();
            ExistsRelevances.Add(relevance);
        }

        /// <summary>
        /// </summary>
        /// <param name="relevance"></param>
        protected internal void AddNotExistsRelevance(SqlJoinRelevance<TFirst> relevance)
        {
            if (NotExistsRelevances == null)
                NotExistsRelevances = new List<SqlJoinRelevance<TFirst>>();
            NotExistsRelevances.Add(relevance);
        }

        /// <summary>
        ///     增加需要查询的字段
        /// </summary>
        /// <param name="fields">需要查询的字段</param>
        /// <param name="joinType">查询字段所在表的连接方式的条件</param>
        internal void AddSelect(IEnumerable<SelectField> fields, JoinType joinType = JoinType.Inner)
        {
            if (fields == null)
                return;
            foreach (var item in fields)
            {
                if (PropertyNamesMap == null)
                    PropertyNamesMap = new Dictionary<string, SelectField>();
                if (PropertyNamesMap.Values.Any(x => x.FieldName == item.FieldName))
                    item.AliasName = item.FieldName + item.SqlBuilderIndex;

                if (PropertyNamesMap.Keys.Contains(item.Key)) continue;
                item.JoinType = joinType;
                PropertyNamesMap.Add(item.Key, item);
            }
        }

        /// <summary>
        ///     加载属性
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlClauseBuilder"></param>
        internal SqlBuilderContext<TFirst> AddLoadProperty<T>(Expression<Func<TFirst, object>> expression,
            SqlClauseBuilder<T> sqlClauseBuilder)
            where T : class, new()
        {
            var member = ((MemberExpression)expression.Body).Member;
            if (LoadedMembers != null && LoadedMembers.Keys.Contains(member.Name))
                return this;

            // sqlClauseBuilder
            var memeberPropertyInfo = InitMemberEntityProperty<TFirst>(member);

            if (memeberPropertyInfo.IsComplex)
            {
                if (LoadedPropertys == null)
                    LoadedPropertys = new Dictionary<EntityPropertyInfo, SqlEntityClauseBuilder>();
                if (LoadedMembers == null)
                    LoadedMembers = new Dictionary<string, EntityPropertyInfo>();

                LoadedPropertys.Add(memeberPropertyInfo, sqlClauseBuilder);
                sqlClauseBuilder.SetUsageLoadProperty(memeberPropertyInfo);
                AddSelect(sqlClauseBuilder.SelectFields, sqlClauseBuilder.JoinType);
            }
            // 回到根
            Current = Root;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal IEnumerable<TFirst> ExcuteSelect(Expression<Func<TFirst, object>> expression)
        {
            return _excuteSelectFunc?.Invoke(expression);
        }

        #endregion

        #region internal方法

        /// <summary>
        ///     记录数
        /// </summary>
        /// <returns></returns>
        internal int Count()
        {
            return _excuteCount();
        }

        /// <summary>
        ///     是否存在记录
        /// </summary>
        /// <returns></returns>
        internal bool Exists()
        {
            return _excuteExists();
        }

        /// <summary>
        ///     主表的WHERE条件
        /// </summary>
        internal string WhereClause
        {
            get
            {
                var where = BuildWhereClause(Root);
                if (!string.IsNullOrEmpty(ExistsClause) && !string.IsNullOrEmpty(where))
                    where = $"{where} AND {ExistsClause}";
                else if (!string.IsNullOrEmpty(ExistsClause))
                    where = $"{where} {ExistsClause}";

                if (!string.IsNullOrEmpty(where))
                    where = $"WHERE {where}";
                return where;
            }
        }

        /// <summary>
        ///     获取查询的字段名（AS后的别名）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal string GetSelectFieldName(string name)
        {
            if (!PropertyNamesMap.Keys.Contains(name))
                throw new Exception($"无法确定{name}的字段");
            return PropertyNamesMap[name].AliasName;
        }

        #endregion

        #region SQL命令

        /// <summary>
        ///     查询命令文本
        /// </summary>
        public string CommandText
        {
            get
            {
                string sql;
                if (Take > 0 && Skip >= 0)
                {
                    if (OrderByClause == string.Empty && EntityInfo != null)
                        SetOrderBy(new[] { EntityInfo.PrimaryKeyInfo.DbFieldInfo });
                    var innerSql = $"{InnerJoinExists}";
                    sql = $"SELECT {InnerSelectClause} FROM {innerSql} {Root.TableName} {InnerJoin}";
                    if (string.IsNullOrEmpty(innerSql))
                    {
                        sql = $"{sql} {WhereClause} {OrderByClause}";
                        sql = _commandBuildProvider.CreatePageSql(sql, Skip + 1, Take);
                    }
                    else
                    {
                        sql = _commandBuildProvider.CreatePageSql(sql, Skip + 1, Take);
                    }
                    sql = $"SELECT * FROM ({sql}) {Root.TableName} {OrderByClause}";
                }
                else
                {
                    sql = $"SELECT {InnerSelectClause} FROM {Root.TableName} {InnerJoin} {WhereClause} {OrderByClause}";
                }


                if (string.IsNullOrEmpty(ExtraWhereClause)) return sql;
                var selectFields = string.Join(",", SelectedFileds.Select(item => $"{item.TableName}.{item.ColumnName}"));
                // sql = $"SELECT {Root.TableName}.*, {OtherSelectClause} FROM ({sql}){Root.TableName} {ExtraWhereClause} {AppendNotInnerWhere}";
                //sql =
                //    $"SELECT {selectFields}, {OtherSelectClause} FROM ({sql}){Root.TableName} {ExtraWhereClause} {AppendNotInnerWhere} {OrderByClause}";
                //sql =
                sql = $"SELECT {Root.TableName}.*, {OtherSelectClause} FROM ({sql}){Root.TableName} {ExtraWhereClause} {AppendNotInnerWhere} {OrderByClause}";
                return sql;
            }
        }

        /// <summary>
        /// </summary>
        public string ExistsCommandText
        {
            get
            {
                var innerSql = $"{InnerJoinExists}";
                var sql = $"SELECT  {EntityInfo.PrimaryKeyInfo.DbFieldInfo.ColumnName} FROM {innerSql} {Root.TableName} ";
                if (string.IsNullOrEmpty(innerSql))
                    sql = $"{sql} {InnerJoin} {WhereClause}";
                return sql;
            }
        }

        /// <summary>
        /// </summary>
        public string CountCommandText
        {
            get
            {
                var innerSql = $"{InnerJoinExists}";
                var sql = $"SELECT COUNT(1) FROM {innerSql} {Root.TableName} ";
                if (string.IsNullOrEmpty(innerSql))
                    sql = $"{sql} {InnerJoin} {WhereClause}";
                return sql;
            }
        }

        #endregion

        /// <summary>
        ///     构建关联的Where条件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        internal string BuildWhereClause(SqlEntityClauseBuilder builder)
        {
            var sqlWhere = builder.WhereClause;
            foreach (var item in builder.DbParameters)
                if (DataParameters.All(x => x.ParameterName != item.ParameterName))
                    DataParameters.Add(item);
            return sqlWhere;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        protected EntityPropertyInfo InitMemberEntityProperty<T>(MemberInfo member)
            where T : class, new()
        {
            var firstEntityInfo = OrmContext.OrmProvider.GetEntityInfo<T>();
            var property = firstEntityInfo.PropertyInfos
                .FirstOrDefault(item => item.Key == member.Name);
            // 被加载的属性信息
            if (property.Value == null)
            {
                var propertyInfo = new EntityPropertyInfo(firstEntityInfo, member.Name);
                // todo: 禁止外部添加
                firstEntityInfo.PropertyInfos.TryAdd(member.Name, propertyInfo);
            }
            EntityPropertyInfo result;
            firstEntityInfo.PropertyInfos.TryGetValue(member.Name, out result);
            return result;
        }
    }
}
