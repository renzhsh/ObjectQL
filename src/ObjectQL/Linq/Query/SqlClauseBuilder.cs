using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ObjectQL.Mapping;
using ObjectQL.Data;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    internal class SqlClauseBuilder<T> : SqlEntityClauseBuilder
        where T : class, new()
    {
        private List<string> _andWhere;
        private List<string> _orWhere;
        private List<string> _joinAndWhere;
        private List<string> _joinOrWhere;
        private string _originTable = string.Empty;
        // ReSharper disable once InconsistentNaming
        private List<WhereClauseBuildResult> _whereClauseBuildResults { set; get; }
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Expression<Func<T, object>> _selectExpression = null;
        private IEnumerable<IDataParameter> _dbParameters;
        /// <summary>
        /// true:无效的SELECT子语句，需要重新构建
        /// </summary>
        private bool _invalidSelect = true;

        /// <summary>
        /// true:无效的SQL自语句，需要重新构建
        /// </summary>
        protected bool InvalidSqlClause = false;

        /// <summary>
        /// 
        /// </summary>
        protected ICommandBuildProvider CommandBuilderProvider;

        /// <summary>
        /// SQL参数和参数值
        /// </summary>
        protected IEnumerable<KeyValuePair<string, object>> ParameterKeyValues
        {
            get
            {
                if (_whereClauseBuildResults != null)
                {
                    foreach (var kvItem in _whereClauseBuildResults)
                    {
                        if (kvItem.ParameterValueList == null || !kvItem.ParameterValueList.Any())
                            continue;

                        foreach (var item in kvItem.ParameterValueList)
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joinType"></param>
        public SqlClauseBuilder(JoinType joinType = JoinType.Inner)
        {
            JoinType = joinType;
            CommandBuilderProvider = OrmContext.DriverProviders.GetCommandBuildProvider<T>();
        }

        /// <summary>
        /// 当前SQL构造器处于上下文中的索引值
        /// </summary>
        internal override int Index { set; get; }

        /// <summary>
        /// 
        /// </summary>
        internal override string OriginTable
        {
            get
            {
                if (string.IsNullOrEmpty(_originTable))
                    _originTable = EntityInfo.TableInfo.TableName;
                return _originTable;
            }
        }

        internal override EntityInfo EntityInfo
        {
            get
            {
                return OrmContext.OrmProvider.GetEntityInfo<T>();
            }
        }



        private IEnumerable<SelectField> _selectFields;
        /// <summary>
        /// 
        /// </summary>
        internal override IEnumerable<SelectField> SelectFields
        {
            get
            {
                if (!_invalidSelect) return _selectFields;
                var selectDbFields = OrmContext.OrmProvider.GetDbColumnInfo(_selectExpression);
                _selectFields = selectDbFields.Select(item => new SelectField(this)
                {
                    FieldInfo = item
                });
                _invalidSelect = false;
                return _selectFields;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override string TableName
        {
            get
            {
                if (Index > 0)
                {
                    return $"{OriginTable}{Index}";
                }
                return $"{OriginTable}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string FromClause
        {
            get
            {
                return $"{OriginTable} {TableName}";
            }
        }

        /// <summary>
        /// Join On的筛选条件
        /// </summary>
        internal override string JoinOnWhereClause
        {
            get
            {
                string result = string.Empty;
                string joinOrClause = string.Empty;
                if (_joinAndWhere != null)
                    result = string.Join(" AND ", _joinAndWhere);

                if (_joinOrWhere != null)
                    joinOrClause = string.Join(" OR ", _joinOrWhere);
                if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(joinOrClause))
                    result = $"{result} OR ({joinOrClause})";
                else if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(joinOrClause))
                    result = joinOrClause;
                return result;
            }
        }

        /// <summary>
        /// 关联后的结果集的筛选条件
        /// </summary>
        internal override string WhereClause
        {
            get
            {
                string result = string.Empty;
                string orClause = string.Empty;
                if (_andWhere != null)
                    result = string.Join(" AND ", _andWhere);
                if (_orWhere != null)
                    orClause = string.Join(" OR ", _orWhere);
                if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(orClause))
                    result = $"({result}) OR ({orClause})";
                else if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(orClause))
                    result = orClause;
                return result;
            }
        }

        /// <summary>
        /// 数据库参数
        /// </summary>
        internal override IEnumerable<IDataParameter> DbParameters
        {
            get
            {
                if (_dbParameters == null)
                    _dbParameters = CommandBuilderProvider.GetDbParameters(ParameterKeyValues);
                return _dbParameters;
            }
        }

        /// <summary>
        /// 关联后的结果集的筛选条件
        /// </summary>
        /// <param name="expression"></param>
        internal void AppendAndWhere(Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
                return;

            var whereClauseBuildResult = OrmContext.OrmProvider.CreateWhereCommand(
                item => $"{TableName}.{GetSelectName(OrmContext.OrmProvider.GetEntityInfo<T>().GetDbColumnName(item))}",
                expression);
            if (string.IsNullOrEmpty(whereClauseBuildResult?.WhereClause))
                return;
            if (_andWhere == null)
                _andWhere = new List<string>();
            _andWhere.Add(whereClauseBuildResult.WhereClause);
            AddWhereClauseBuildResult(whereClauseBuildResult);
        }

        /// <summary>
        /// Join On的查询条件
        /// </summary>
        /// <param name="expression"></param>
        internal void JoinOnAndWhere(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                return;
            WhereClauseBuildResult whereClauseBuildResult = OrmContext.OrmProvider.CreateWhereCommand(
               (item) => $"{TableName}.{GetSelectName(OrmContext.OrmProvider.GetEntityInfo<T>().GetDbColumnName(item))}",
               expression);
            if (whereClauseBuildResult == null || string.IsNullOrEmpty(whereClauseBuildResult.WhereClause))
                return;
            if (_joinAndWhere == null)
                _joinAndWhere = new List<string>();
            _joinAndWhere.Add(whereClauseBuildResult.WhereClause);
            AddWhereClauseBuildResult(whereClauseBuildResult);
        }

        /// <summary>
        /// Join On的查询条件
        /// </summary>
        /// <param name="expression"></param>
        internal void JoinOnOrWhere(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                return;
            WhereClauseBuildResult whereClauseBuildResult = OrmContext.OrmProvider.CreateWhereCommand(
               (item) => $"{TableName}.{GetSelectName(OrmContext.OrmProvider.GetEntityInfo<T>().GetDbColumnName(item))}",
               expression);
            if (whereClauseBuildResult == null || string.IsNullOrEmpty(whereClauseBuildResult.WhereClause))
                return;
            if (_joinOrWhere == null)
                _joinOrWhere = new List<string>();
            _joinOrWhere.Add(whereClauseBuildResult.WhereClause);
            AddWhereClauseBuildResult(whereClauseBuildResult);
        }

        /// <summary>
        /// 关联后的结果集的筛选条件
        /// </summary>
        /// <param name="expression"></param>
        internal void AppendOrWhere(Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
                return;
            var whereClauseBuildResult = OrmContext.OrmProvider.CreateWhereCommand(
               (item) => $"{TableName}.{GetSelectName(OrmContext.OrmProvider.GetEntityInfo<T>().GetDbColumnName(item))}",
               expression);

            if (string.IsNullOrEmpty(whereClauseBuildResult?.WhereClause))
                return;
            if (_orWhere == null)
                _orWhere = new List<string>();
            _orWhere.Add(whereClauseBuildResult.WhereClause);
            AddWhereClauseBuildResult(whereClauseBuildResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClauseBuildResult"></param>
        protected void AddWhereClauseBuildResult(WhereClauseBuildResult whereClauseBuildResult)
        {
            if (whereClauseBuildResult == null)
                return;
            if (_whereClauseBuildResults == null)
                _whereClauseBuildResults = new List<WhereClauseBuildResult>();

            if (!string.IsNullOrEmpty(whereClauseBuildResult.WhereClause))
            {
                _whereClauseBuildResults.Add(whereClauseBuildResult);
                // var parameters = CommandBuilderProvider.GetDbParameters(whereClauseBuildResult.ParameterValues);
                // AddParameters(parameters);
            }
        }

        /// <summary>
        /// 获取查询的字段(重名的情况下会根据上下文生成字段别名)
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal string GetSelectName(string fieldName)
        {
            var query = SelectFields.Where(item => item.FieldName == fieldName);
            return query.Any() ? query.First().AliasName : string.Empty;
        }
    }
}
