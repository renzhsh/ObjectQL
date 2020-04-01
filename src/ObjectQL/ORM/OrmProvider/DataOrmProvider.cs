/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataOrmProvider
 * 命名空间：ObjectQL.Data
 * 文 件 名：DataOrmProvider
 * 创建时间：2017/3/22 22:03:04
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Jinhe;
using ObjectQL.Data;
using ObjectQL.Linq;

namespace ObjectQL.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class DataOrmProvider
    {
        private static readonly ConcurrentDictionary<string, EntityInfo> EntityInfoCollection
         = new ConcurrentDictionary<string, EntityInfo>();

        /// <summary>
        /// 清除所有的实体映射关系
        /// </summary>
        public void Clear()
        {
            // 清理
            EntityInfoCollection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public void AddMapping(IEntityMap map)
        {
            // 实体类
            var entityType = map.EntityInfo.EntityType;
            if (EntityInfoCollection.Keys.Contains(entityType.FullName))
                throw new Exception($"{entityType}的映射关系被重复定义");
            EntityInfoCollection.TryAdd(entityType.FullName, map.EntityInfo);
        }

        public void AddEntityInfo(EntityInfo info)
        {
            if (EntityInfoCollection.Keys.Contains(info.EntityType.FullName))
                throw new Exception($"{info.EntityType.FullName}的映射关系被重复定义");
            EntityInfoCollection.TryAdd(info.EntityType.FullName, info);
        }

        /// <summary>
        /// 根据类名获取实体映射信息
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public EntityInfo GetEntityInfo(string fullName)
        {
            if (!EntityInfoCollection.Keys.Contains(fullName))
            {
                throw new ASoftException($"{fullName}的类型没有对应的Maping映射");
            }
            var result = EntityInfoCollection[fullName];
            return result;
        }

        /// <summary>
        /// 获取指定类型的实体关系映射信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public EntityInfo GetEntityInfo<T>()
        {
            return GetEntityInfo(typeof(T).FullName);
        }

        /// <summary>
        /// 获取表达式中指定的数据库字段
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="expression"></param>
        /// <returns></returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        internal IEnumerable<DbColumnInfo> GetDbColumnInfo<T>(Expression<Func<T, object>> expression = null)
            where T : class, new()
        {
            if (expression == null)
                return GetDbColumnInfo<T>();

            var entityInfo = GetEntityInfo<T>();
            var propertyNames = GetPropNames(expression);
            var query = entityInfo.PropertyInfos
                        .Where(item => propertyNames.Contains(item.Value.PropetyName))
                        .Select(item => new { DbField = item.Value.DbFieldInfo, index = propertyNames.IndexOf(item.Key) });
            var result = query.OrderBy(x => x.index).Select(item => item.DbField);
            Console.WriteLine("12312");
            return result;
        }

        /// <summary>
        /// 获取所有数据库字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal IEnumerable<DbColumnInfo> GetDbColumnInfo<T>()
            where T : class, new()
        {
            var entityInfo = GetEntityInfo<T>();
            var result = entityInfo.PropertyInfos.Where(item => item.Value.HasColumn)
                .Select(item => item.Value.DbFieldInfo);
            return result;
        }

        /// <summary>
        /// 根据表达式获取成员名称的字符串集合
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<string> GetPropNames<T>(Expression<Func<T, object>> expression)
        {
            List<string> propNames;
            switch (expression.Body.NodeType)
            {
                //item=>"*"
                case ExpressionType.Constant:
                    propNames = new[] { ((ConstantExpression)expression.Body).Value.ToString() }.ToList();
                    break;
                case ExpressionType.MemberAccess://item=>item.ID
                    propNames = new[] { ((MemberExpression)expression.Body).Member.Name }.ToList();
                    break;
                case ExpressionType.New://item=>new {item.ID, item.Name}
                    propNames = ((NewExpression)expression.Body).Members.Select(item => item.Name).ToList();
                    break;
                case ExpressionType.NewArrayInit:
                    var s = (expression.Body as NewArrayExpression).Expressions;
                    propNames = null;
                    break;
                case ExpressionType.Convert:
                    propNames = new[] { ((expression.Body as UnaryExpression)?.Operand as MemberExpression)?.Member.Name }.ToList();
                    break;
                default:
                    propNames = new List<string>();
                    break;
            }
            return propNames;
        }

        internal WhereClauseBuildResult CreateWhereCommand<T>(Func<string, string> fieldLoadFunc, Expression<Func<T, bool>> expression = null)
            where T : class, new()
        {
            if (expression == null) return null;
            var whereClauseBuilder = new WhereClauseBuilder<T>(fieldLoadFunc, OrmContext.DriverProviders.GetCommandBuildProvider<T>());
            var whereClauseBuildResult = whereClauseBuilder.BuildWhereClause(expression);
            return whereClauseBuildResult;
        }

        /// <summary>
        /// 附加SQL命令Where子语句的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">Where条件的表达式</param>
        /// <param name="sql">SQL文本</param>
        /// <param name="parameters">SQL参数</param>
        public void AppendSqlWhereParameter<T>(Expression<Func<T, bool>> expression, ref string sql, ref IDataParameter[] parameters)
            where T : class, new()
        {
            if (expression == null) return;
            var whereClauseBuildResult = new WhereClauseBuilder<T>(GetEntityInfo<T>().GetDbColumnName, OrmContext.DriverProviders.GetCommandBuildProvider<T>())
                .BuildWhereClause(expression);

            if (!string.IsNullOrEmpty(whereClauseBuildResult.WhereClause))
            {
                sql = $"{sql} WHERE {whereClauseBuildResult.WhereClause}";
            }
            var index = 0;
            var parameterCount = whereClauseBuildResult.ParameterValues.Count;
            parameters = new IDataParameter[parameterCount];
            foreach (var item in whereClauseBuildResult.ParameterValues)
            {
                parameters[index] = CreateParameter<T>(item.Key, item.Value);
                index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal IDataParameter CreateParameter<T>(string key, object value)
            where T : class, new()
        {
            var commandBuildProvider = OrmContext.DriverProviders.GetCommandBuildProvider<T>();
            IDataParameter result;
            if (value != null && value.GetType().IsEnum)
            {
                result = commandBuildProvider.MakeIn(key, (int)value);
            }
            else
            {
                result = commandBuildProvider.MakeIn(key, value);
            }
            return result;
        }
    }
}
