using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ObjectQL.Data;
using ObjectQL.Specifications;
using System.Transactions;
using ObjectQL.Linq;
using System.Collections.Concurrent;
using System.Linq;
using Jinhe;

namespace ObjectQL
{
    class UpdateOperation
    {
        public UpdateOperation(DataGateway gateway)
        {
            this.gateway = gateway;
        }

        DataGateway gateway { get; }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateCriteria"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public NonQueryResult Update<T>(UpdateCriteria<T> updateCriteria, Specification<T> specification)
            where T : class, new()
        {
            var tableName = OrmContext.OrmProvider.GetEntityInfo<T>().TableInfo.TableName;
            IDataParameter[] whereParameters = null;
            List<IDataParameter> parameters;
            var sql =
                $"UPDATE {tableName} SET {string.Join(",", CreateUpdateParameterNames(updateCriteria, out parameters))}";
            OrmContext.OrmProvider.AppendSqlWhereParameter(specification.Expression, ref sql, ref whereParameters);
            var db = OrmContext.DriverProviders.GetDataAccess<T>();
            parameters.AddRange(whereParameters);
            return db.ExecuteNonQuery(sql, parameters.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateCriteria"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> CreateUpdateParameterNames<T>(UpdateCriteria<T> updateCriteria, out List<IDataParameter> parameters)
            where T : class, new()
        {
            var entiyInfo = OrmContext.OrmProvider.GetEntityInfo<T>();
            if (entiyInfo.IsReadOnly)
                throw new Exception("不能更新更新一个视图对象");

            var updateParameters = new List<IDataParameter>();
            var commandBuildProvider = OrmContext.DriverProviders.GetCommandBuildProvider<T>();
            var updateParameterNameList = updateCriteria.Select(uc =>
            {
                var entityInfo = OrmContext.OrmProvider.GetEntityInfo<T>();
                var entityProperty = entityInfo.PropertyInfos.Where(item => item.Value.PropetyName == uc.Key)
                                                        .Select(item => item.Value).FirstOrDefault();
                if (string.IsNullOrEmpty(entityProperty?.DbFieldInfo.ColumnName))
                {
                    throw new ASoftException($"{typeof(T).FullName}：{uc.Key}没有映射数据库字段", RetCode.DB9002);
                }
                if (entityProperty.IsReadOnly)
                {
                    throw new ASoftException($"{typeof(T).FullName}：{uc.Key}是只读字段，不能更新");
                }
                var parameterName = $"{commandBuildProvider.ParameterPrefix}u_{uc.Key}";
                var result = $"{entityProperty.DbFieldInfo.ColumnName}={parameterName}";
                var parameter = OrmContext.OrmProvider.CreateParameter<T>(parameterName, uc.Value);

                updateParameters.Add(parameter);
                return result;
            });
            parameters = updateParameters;
            return updateParameterNameList;
        }
    }

    public static class UpdateExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateCriteria"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public static NonQueryResult Update<T>(this DataGateway gateway, UpdateCriteria<T> updateCriteria, Specification<T> specification)
            where T : class, new()
        {
            var op = new UpdateOperation(gateway);
            return op.Update(updateCriteria, specification);
        }

        /// <summary>
        ///     更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateCriteria"></param>
        /// <param name="expression"></param>
        public static NonQueryResult Update<T>(this DataGateway gateway, UpdateCriteria<T> updateCriteria, Expression<Func<T, bool>> expression)
            where T : class, new()
        {
            return gateway.Update(updateCriteria, (Specification<T>)expression);
        }

        /// <summary>
        /// 注意不支持oracle 11G以下版本的oracle数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateObjects"></param>
        public static void UpdateBatch<T>(this DataGateway gateway, IEnumerable<UpdateObject<T>> updateObjects)
            where T : class, new()
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var update in updateObjects)
                    gateway.Update(update.GetUpdateCriteria(), update.Where);
                transaction.Complete();
            }
        }
    }
}
