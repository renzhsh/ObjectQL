/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataQuaryable
 * 命名空间：ObjectQL.Data
 * 文 件 名：DataQuaryable
 * 创建时间：2016/10/21 13:13:16
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Jinhe;
using ObjectQL.Mapping;
using ObjectQL.Data;

namespace ObjectQL.Linq
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TFirst">主实体类型</typeparam>
    public class SqlDataQueryable<TFirst> : IDataQueryable<TFirst>, IOrderQuery<TFirst>
        where TFirst : class, new()
    {
        #region 私有字段  

        private readonly IDataAccess _db;
        private ModelReaderProvider _readerProvider;

        #endregion

        /// <summary>
        ///     数据读取支持对象
        /// </summary>
        private ModelReaderProvider ReaderProvider => _readerProvider ?? (_readerProvider =
                                                          new ModelReaderProvider(BuilderContext.GetSelectFieldName));

        #region 构造函数

        /// <summary>
        ///     SQL构造上下文
        /// </summary>
        public SqlBuilderContext<TFirst> BuilderContext { protected internal set; get; }

        /// <summary>
        /// </summary>
        public SqlDataQueryable()
        {
            BuilderContext = new SqlBuilderContext<TFirst>(Select, Count, Exists);
            _db = OrmContext.DriverProviders.GetDataAccess<TFirst>();
            OrmContext.DriverProviders.GetCommandBuildProvider<TFirst>();
        }

        #endregion

        #region 保护属性  

        /// <summary>
        /// </summary>
        protected string CommandText
        {
            get
            {
                var sql = BuilderContext.CommandText;
                return sql;
            }
        }

        /// <summary>
        ///     数据库参数
        /// </summary>
        protected IEnumerable<IDataParameter> DbParameters
        {
            get
            {
                var result = BuilderContext.DataParameters;
                return result;
            }
        }

        #endregion

        #region 附加查询条件 

        /// <summary>
        ///     附加Where And条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataQueryable<TFirst> AppendAndWhere(Expression<Func<TFirst, bool>> expression = null)
        {
            BuilderContext.AppendAndWhere(expression);
            return this;
        }

        /// <summary>
        ///     附加Where Or条件语句
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataQueryable<TFirst> AppendOrWhere(Expression<Func<TFirst, bool>> expression = null)
        {
            BuilderContext.AppendOrWhere(expression);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        public SqlJoinRelevance<TFirst, TFirst, TSecond> Join<TSecond>(Expression<Func<TFirst, object>> firstExpression,
            Expression<Func<TSecond, object>> secondExpression, JoinType joinType = JoinType.Inner)
            where TSecond : class, new()
        {
            var sqlClauseBuilder = BuilderContext
                .Join(firstExpression, secondExpression, joinType);
            return sqlClauseBuilder;
        }

        /// <summary>
        ///     LeftJoin关联
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <returns></returns>
        public SqlJoinRelevance<TFirst, TFirst, TSecond> LeftJoin<TSecond>(
            Expression<Func<TFirst, object>> firstExpression, Expression<Func<TSecond, object>> secondExpression)
            where TSecond : class, new()
        {
            return Join(firstExpression, secondExpression, JoinType.Left);
        }

        ///// <summary>
        ///// 关联
        ///// </summary>
        ///// <typeparam name="TSecond"></typeparam>
        ///// <param name="expression"></param>
        ///// <param name="joinType"></param>
        ///// <returns></returns>
        //public IDataQueryable<TFirst> Join<TSecond>(Expression<Func<TFirst, TSecond, bool>> expression, JoinType joinType = JoinType.Inner)
        //    where TSecond : class, new()
        //{
        //    return this;
        //} 

        #endregion

        #region 分页、排序

        /// <summary>
        ///     倒序排序
        /// </summary>
        public IOrderQuery<TFirst> Desc()
        {
            BuilderContext.SetOrderType(OrderType.Desc);
            return this;
        }

        /// <summary>
        ///     跳过序列中指定数量的元素，然后返回剩余的元素(默认使用主键排序)
        /// </summary>
        /// <param name="skip"></param>
        public IOrderQuery<TFirst> Skip(int skip)
        {
            BuilderContext.Skip = skip;
            return this;
        }

        /// <summary>
        ///     从序列的开头返回指定数量的连续元素(默认使用主键排序)
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public IQueryExecutor<TFirst> Take(int take)
        {
            BuilderContext.Take = take;
            return this;
        }

        #endregion

        #region 数据访问的公共方法

        /// <summary>
        ///     统计记录数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            using (var sr = _db.ExecuteScalar(BuilderContext.CountCommandText, DbParameters?.ToArray()))
            {
                return sr.IntValue;
            }
        }

        /// <summary>
        ///     是否存在
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            Debug.WriteLine(BuilderContext.ExistsCommandText);
            using (var sr = _db.ExecuteScalar(BuilderContext.ExistsCommandText, DbParameters?.ToArray()))
            {
                return !sr.IsDBNull && sr.Value != null;
            }
        }

        /// <summary>
        ///     排序
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IOrderQuery<TFirst> OrderBy(Expression<Func<TFirst, object>> expression)
        {
            BuilderContext.SetOrderBy(expression);
            return this;
        }

        #region 查询 
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual IEnumerable<TResult> SelectToModel<TResult>(Func<TFirst, TResult> selector)
        {
            var result = Select().Select(selector);
            return result;
        }

        /// <summary>
        ///     执行查询并返回实体
        /// </summary>
        /// <param name="expression">查询的字段（使用后从数据库中将只获取这些字段，暂未支持）</param>
        /// <returns></returns>
        public virtual IEnumerable<TFirst> Select(Expression<Func<TFirst, object>> expression = null)
        {
            var entityInfo = OrmContext.OrmProvider.GetEntityInfo<TFirst>();
            if (entityInfo.PrimaryKeyInfo == null)
                throw new ASoftException($"实体类型{typeof(TFirst)}未定义主键字段（属性）");
            if (expression != null)
                BuilderContext.SetSelect(expression);

            var entityPrimaryKeyField = entityInfo.PrimaryKeyInfo.DbFieldInfo;
            var result = new List<TFirst>();

            using (var reader = _db.ExecuteReader(CommandText, DbParameters?.ToArray()))
            {
                Dictionary<object, object> resultDict = null;
                // 主键字段名称 
                while (reader.Read())
                {
                    if (resultDict == null)
                        resultDict = new Dictionary<object, object>();

                    var entityPrimaryValue = reader[entityPrimaryKeyField.ColumnName];
                    var entityKey = $"{entityPrimaryKeyField.ColumnName}{entityPrimaryValue}";
                    TFirst entity;
                    if (!resultDict.Keys.Contains(entityKey))
                    {
                        entity = new TFirst();
                        ReaderProvider.SetObjectProperty(entity, reader);
                        resultDict[entityKey] = entity;
                        result.Add(entity);
                    }
                    else
                    {
                        entity = resultDict[entityKey] as TFirst;
                    }

                    // 加载属性的值
                    if (BuilderContext.LoadedPropertys == null) continue;
                    foreach (var item in BuilderContext.LoadedPropertys)
                    {
                        var entityProperty = item.Key;
                        var type = entityProperty.PropertyType;
                        if (type.IsIEnumerable() || type.IsListGenericType())
                        {
                            type = type.GetGenericArguments().FirstOrDefault();
                        }
                        if (type == null) continue;
                        var primaryKey = OrmContext.OrmProvider.GetEntityInfo(type.FullName).PrimaryKeyInfo;
                        if (primaryKey == null)
                            throw new Exceptions.NotSetPrimaryKeyException($"类型{type.FullName}的关系映射中没有标示主键字段");
                        // 属性字段的主键值，用于过滤重复的数据
                        var keyValue = ReaderProvider.GetPropertyValue(primaryKey, reader, $"{type.FullName}.{entityProperty.PropetyName}");
                        if (keyValue == null)
                            continue;

                        // 属性对象的标识
                        var entityPropertyKey = $"{primaryKey.DbFieldInfo.TableName}{primaryKey.DbFieldInfo.ColumnName}{keyValue}";

                        // 如果主表中已经取到了，则直接从entityDict
                        if (primaryKey.DbFieldInfo.TableName == entityInfo.TableInfo.TableName
                            && resultDict.ContainsKey($"{entityPrimaryKeyField.ColumnName}{keyValue}"))
                        {

                            // bug: entity ==> collection
                            var val = resultDict[$"{primaryKey.DbFieldInfo.ColumnName}{keyValue}"];
                            entityProperty.SetValue(entity, val, null);
                            continue;
                        }
                        if (!resultDict.ContainsKey(entityPropertyKey))
                        {
                            var obj = ReaderProvider.SetObjectProperty(entity, entityProperty, reader);
                            resultDict[entityPropertyKey] = obj;
                        }
                        else
                        {
                            var val = resultDict[entityPropertyKey];
                            entityProperty.SetValue(entity, val, null);
                        }
                    }
                }
                reader.Close();
            }


            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="total"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual IEnumerable<TFirst> Select(out int total, Expression<Func<TFirst, object>> expression = null)
        {
            total = Count();
            return Select(expression);
        }

        /// <summary>
        ///     自定义查询，用一个完整的语句查询并映射成实体类
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<TFirst> QueryAndSelect(string commandText, params object[] args)
        {
            var result = new List<TFirst>();
            using (var reader = _db.ExecuteReaderByArgs(commandText, args))
            {
                while (reader.Read())
                {
                    var entity = new TFirst();
                    ReaderProvider.SetObjectProperty(entity, reader);
                    result.Add(entity);
                }
                reader.Close();
            }
            return result;
        }



        /// <summary>
        ///     执行查询并返回列表
        /// </summary>
        /// <returns></returns>
        public List<TFirst> ToList()
        {
            return Select().ToList();
        }

        #endregion

        #endregion

        #region 关联、查询      

        /// <summary>
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataQueryable<TFirst> Exists<TSecond>(Expression<Func<TFirst, object>> firstExpression,
            Expression<Func<TSecond, object>> secondExpression,
            Expression<Func<TSecond, bool>> expression = null)
            where TSecond : class, new()
        {
            return Exists(firstExpression, secondExpression, expression, true);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IDataQueryable<TFirst> NotExists<TSecond>(Expression<Func<TFirst, object>> firstExpression,
            Expression<Func<TSecond, object>> secondExpression,
            Expression<Func<TSecond, bool>> expression = null)
            where TSecond : class, new()
        {
            return Exists(firstExpression, secondExpression, expression, false);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firstExpression"></param>
        /// <param name="secondExpression"></param>
        /// <param name="expression"></param>
        /// <param name="isExists"></param>
        /// <returns></returns>
        protected IDataQueryable<TFirst> Exists<TSecond>(Expression<Func<TFirst, object>> firstExpression,
            Expression<Func<TSecond, object>> secondExpression,
            Expression<Func<TSecond, bool>> expression = null,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            bool isExists = true)
            where TSecond : class, new()
        {
            BuilderContext.Exists(firstExpression, secondExpression, expression, isExists);
            return this;
        }

        #endregion

        #region Obsolete - 过时方法  

        /// <summary>
        ///     附加Where AND条件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法AppendAndWhere(string text, params object[] args)已经过时", true)]
        public IDataQueryable<TFirst> AppendAndWhere(string text, params object[] args)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     附加Where Or条件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法AppendOrWhere(string text, params object[] args)已经过时", true)]
        public IDataQueryable<TFirst> AppendOrWhere(string text, params object[] args)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> QueryDynamicFrom(string commandText, params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
