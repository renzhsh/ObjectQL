using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.Specifications;
using ObjectQL.Linq;

namespace ObjectQL
{
    public static class QueryExtensions
    {
        /// <summary>
        ///     Where查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IDataQueryable<T> Where<T>(this DataGateway gateway, Expression<Func<T, bool>> expression = null)
            where T : class, new()
        {
            IDataQueryable<T> result = new SqlDataQueryable<T>();
            result.AppendAndWhere(expression);
            return result;
        }

        /// <summary>
        /// SQL查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ISqlContext Query(this DataGateway gateway, string commandText, params object[] args)
        {
            SqlContext context = new SqlContext(Consts.BaseConnectStringName, commandText, args);
            return context;
        }

        /// <summary>
        ///     完整SQL的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法已经过期，使用ISqlContext Query(string commandText, params object[] args)方法替代", true)]
        public static IEnumerable<T> Query<T>(this DataGateway gateway, string commandText, params object[] args)
            where T : class, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("方法已经过期，使用ISqlContext Query(string commandText, params object[] args)方法替代", true)]
        public static IEnumerable<dynamic> QueryDynamicFrom<T>(this DataGateway gateway, string commandText, params object[] args)
            where T : class, new()
        {
            throw new NotImplementedException();
        }

    }
}
