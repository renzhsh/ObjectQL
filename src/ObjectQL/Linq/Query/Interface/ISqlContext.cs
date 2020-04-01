using System.Collections.Generic;

namespace ObjectQL.Linq
{
    public interface ISqlContext
    {
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IResultSet<T> Get<T>() where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName">null时使用默认数据库连接</param>
        /// <returns></returns>
        IResultSet<dynamic> Get(string connectionName = null);

        /// <summary>
        /// 跳过的记录
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        IPageSqlContext Skip(int skip);



        /// <summary>
        /// 获取第一行第一列的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionName">null时使用默认数据库连接</param>
        /// <returns></returns>
        T GetScalar<T>(string connectionName = null);
    }
}