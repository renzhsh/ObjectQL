using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ObjectQL.Data;
using System.Linq.Expressions;

namespace ObjectQL
{
    public static class DeleteExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public static NonQueryResult Delete<T>(this DataGateway gateway, Expression<Func<T, bool>> expression) where T : class, new()
        {
            var tableName = OrmContext.OrmProvider.GetEntityInfo<T>().TableInfo.TableName;
            var sql = $"DELETE FROM {tableName}";
            IDataParameter[] parameters = null;
            OrmContext.OrmProvider.AppendSqlWhereParameter(expression, ref sql, ref parameters);
            var db = OrmContext.DriverProviders.GetDataAccess<T>();
            //Console.WriteLine(sql);
            //collectors.AddExecuteNonQuery(db, new DataCommand()
            //{
            //    CommandText = sql,
            //    Parameters = parameters?.ToArray(),
            //    CommandType = CommandType.Text
            //});
            return db.ExecuteNonQuery(sql, parameters);
        }
    }
}
