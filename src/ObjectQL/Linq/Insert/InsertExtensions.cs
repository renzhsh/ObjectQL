using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using Jinhe;
using ObjectQL.Data;
using ObjectQL.Linq;

namespace ObjectQL
{
    class InsertOperation
    {
        public InsertOperation(DataGateway gateway)
        {
            this.gateway = gateway;
        }

        DataGateway gateway { get; }

        public void Insert<T>(T tableObject) where T : class, new()
        {
            var tableName = OrmContext.OrmProvider.GetEntityInfo<T>().TableInfo.TableName;
            var db = OrmContext.DriverProviders.GetDataAccess<T>();

            List<string> fieldNames;
            var parameters = CreateInsertDbParameters(tableObject, out fieldNames);
            var parameterNames = parameters.Select(item => item.ParameterName);
            var sql =
                $"INSERT INTO {tableName} ({string.Join(",", fieldNames)}) VALUES ({string.Join(",", parameterNames)})";
            db.ExecuteNonQuery(sql, parameters.ToArray());
        }

        /// <summary>
        /// 创建实体对象的数据库参数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象的实例</param>
        /// <param name="fields">有值的字段</param>
        /// <returns></returns>
        public virtual IEnumerable<IDataParameter> CreateInsertDbParameters<T>(T obj, out List<string> fields)
            where T : class, new()
        {
            var entiyInfo = OrmContext.OrmProvider.GetEntityInfo<T>();
            var parameters = new List<IDataParameter>();
            var commandBuildProvider = OrmContext.DriverProviders.GetCommandBuildProvider<T>();
            List<string> fieldList = null;
            entiyInfo.PropertyInfos.Where(item => item.Value.HasColumn)
                 .ToList()
                 .ForEach(p =>
                 {
                     fieldList = fieldList == null ? new List<string>() : fieldList;
                     // 属性名称
                     var propertyName = p.Value.PropetyName.ToLower();
                     // 数据库字段名称
                     var fieldName = p.Value.DbFieldInfo.ColumnName;
                     var parameterName = $"{commandBuildProvider.ParameterPrefix}{propertyName}";
                     IDataParameter parameter = null;
                     // 枚举
                     if (p.Value.IsEnum)
                     {
                         var myType = p.Value.PropertyType.Assembly.GetType(p.Value.PropertyType.FullName);
                         var myValue = (int)Enum.Parse(myType, p.Value.GetValue(obj, null).ToString());
                         parameter = OrmContext.OrmProvider.CreateParameter<T>(parameterName, myValue);
                     }
                     else if (p.Value.DbFieldInfo.DefaultExpression?.ToString().ToUpper() == "SYSDATE")
                     {
                         parameter = commandBuildProvider.MakeIn(parameterName, DateTime.Now);
                     }
                     else
                     {
                         var parameterValue = p.Value.GetValue(obj, null);
                         if (parameterValue != null)
                         {

                             parameter = OrmContext.OrmProvider.CreateParameter<T>(parameterName, parameterValue);
                         }
                     }
                     if (parameter == null) return;
                     fieldList.Add(fieldName);
                     parameters.Add(parameter);
                 });
            fields = fieldList;
            return parameters;
        }
    }


    public static class InsertExtensions
    {
        #region Insert操作

        /// <summary>
        ///     插入新的实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableObject"></param>
        public static void Insert<T>(this DataGateway gateway, T tableObject) where T : class, new()
        {
            var op = new InsertOperation(gateway);

            op.Insert(tableObject);
        }

        /// <summary>
        /// 注意不支持oracle 11G以下版本的oracle数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableObjects"></param>
        public static void InsertBatch<T>(this DataGateway gateway, IEnumerable<T> tableObjects) where T : class, new()
        {
            using (var transaction = new TransactionScope())
            {
                foreach (var tableObject in tableObjects)
                    gateway.Insert(tableObject);
                transaction.Complete();
            }
        }

        /// <summary>
        /// 完成事务提交
        /// </summary>
        [Obsolete]
        public static void Complete(this DataGateway gateway)
        {
            //collectors.Complete();
        }

        #endregion
    }
}
