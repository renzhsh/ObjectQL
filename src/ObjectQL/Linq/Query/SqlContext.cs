/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：QueryContext
 * 命名空间：ObjectQL.Data
 * 文 件 名：QueryContext
 * 创建时间：2018/3/27 16:52:33
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;
using ObjectQL.Data;

namespace ObjectQL.Linq
{


    /// <summary>
    /// 
    /// </summary>
    public class SqlContext : ISqlContext, IPageSqlContext
    {
        private ModelReaderProvider _readerProvider;
        private ModelReaderProvider readerProvider => _readerProvider ?? (_readerProvider =
                                                  new ModelReaderProvider());

        private object[] _args;
        private string _commandText;


        /// <summary>
        /// 跳过记录（如果不指定排序则默认使用主键升序排序）
        /// </summary>
        private int _skip { set; get; }

        /// <summary>
        /// 获取记录（如果不指定排序则默认使用主键升序排序）
        /// </summary>
        private int _take { set; get; }

        private string _defaultConnectionName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName">数据连接的名称</param>
        /// <param name="sql">执行的SQL</param>
        /// <param name="args">参数</param>
        public SqlContext(string connectionName, string sql, params object[] args)
        {
            _defaultConnectionName = connectionName;
            _args = args;
            _commandText = sql;
        }

        /// <summary>
        /// </summary>
        public string CountCommandText
        {
            get
            {
                var sql = $"SELECT COUNT(1) FROM ({_commandText})";
                return sql;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string GetCommandText<T>()
             where T : class, new()
        {
            var sql = _commandText;
            ICommandBuildProvider commandBuildProvider = OrmContext.DriverProviders.GetCommandBuildProvider<T>();
            if (_take > 0 && _skip >= 0)
            {
                sql = commandBuildProvider.CreatePageSql(_commandText, _skip + 1, _take);
            }
            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IDataAccess GetDb<T>()
                        where T : class, new()
        {
            return OrmContext.DriverProviders.GetDataAccess<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        public IPageSqlContext Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public IPageSqlContext Take(int take)
        {
            _take = take;
            return this;
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <returns></returns>
        public NonQueryResult ExecuteNonQuery()
        {
            var db = OrmContext.DriverProviders.GetDataAccess(_defaultConnectionName);
            return db.ExecuteNonQueryByArgs(_commandText, _args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IResultSet<T> Get<T>()
            where T : class, new()
        {
            var rows = new List<T>();
            var command = GetCommandText<T>();
            var db = GetDb<T>();
            using (var reader = db.ExecuteReaderByArgs(command, _args))
            {
                while (reader.Read())
                {
                    var entity = new T();
                    readerProvider.SetObjectProperty(entity, reader);
                    rows.Add(entity);
                }
                reader.Close();
            }
            var result = new ResultSet<T>(db, rows);
            result.SetTotal(Count);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName">null时使用默认数据库连接</param>
        /// <returns></returns>
        public IResultSet<dynamic> Get(string connectionName = null)
        {
            connectionName = connectionName != null ? connectionName : _defaultConnectionName;
            var rows = new List<ExpandoObject>();
            var db = OrmContext.DriverProviders.GetDataAccess(connectionName);
            using (var reader = db.ExecuteReaderByArgs(_commandText, _args))
            {
                var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                while (reader.Read())
                {
                    var expando = new ExpandoObject() as IDictionary<string, object>;
                    foreach (var name in names)
                        expando[name] = reader[name];
                    rows.Add((ExpandoObject)expando);
                }
                reader.Close();
            }
            var result = new ResultSet<dynamic>(db, rows);
            result.SetTotal(Count);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public T GetScalar<T>(string connectionName = null)
        {
            connectionName = connectionName != null ? connectionName : _defaultConnectionName;
            var db = OrmContext.DriverProviders.GetDataAccess(connectionName);
            using (var sr = db.ExecuteScalarByArgs(_commandText, _args))
            {
                var type = typeof(T);
                if (type == typeof(bool))
                {
                    return (T)(object)(sr.BoolValue);
                }
                else if (type == typeof(DateTime))
                {
                    return (T)(object)(sr.DateTimeValue);
                }
                else if (type == typeof(decimal))
                {
                    return (T)(object)(sr.DecimalValue);
                }
                else if (type == typeof(float))
                {
                    return (T)(object)(sr.FloatValue);
                }
                else if (type == typeof(int))
                {
                    return (T)(object)(sr.IntValue);
                }
                else if (type == typeof(long))
                {
                    return (T)(object)(sr.LongValue);
                }
                else if (type == typeof(long))
                {
                    return (T)(object)(sr.LongValue);
                }
                else if (type == typeof(string))
                {
                    return (T)(object)(sr.StringValue);
                }
                else
                {
                    return (T)sr.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual int Count(IDataAccess db)
        {
            using (var sr = db.ExecuteScalarByArgs(CountCommandText, _args))
            {
                return sr.IntValue;
            }
        }
    }
}
