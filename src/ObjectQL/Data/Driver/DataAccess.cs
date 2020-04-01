/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataAccess
 * 命名空间：ObjectQL.Data.Impl
 * 文 件 名：DataAccess
 * 创建时间：2016/10/19 10:48:08
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jinhe;

namespace ObjectQL.Data
{
    /// <summary>
    /// 用于数据库访问
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="P"></typeparam>
    public abstract class DataAccess<C, A, T, P> : IDataAccess
        where C : class, IDbConnection, new()
        where A : class, IDbDataAdapter
        where T : class, IDbTransaction
        where P : class, IDbDataParameter, new()
    {
        #region 私有字段
        private ConnectionSettings _connectSetting;
        #endregion

        #region 公共属性

        /// <summary>
        /// 所有表的名称
        /// </summary>
        public IEnumerable<string> Tables
        {
            get
            {
                List<string> result = new List<string>();
                using (IResultReader reader = ExecuteReader(this.CommandBuildProvider.AllTableSql))
                {
                    while (reader.Read())
                    {
                        result.Add(reader[0]?.ToString()?.Trim().ToUpper());
                    }
                    reader.Close();
                }
                return result;
            }
        }

        /// <summary>
        /// 数据库命令创建支持
        /// </summary>
        public abstract ICommandBuildProvider CommandBuildProvider { set; get; }

        /// <summary>
        /// 连接名
        /// </summary>
        public string ConnectionName
        {
            get
            {
                return this.ConnectionSetting.ConnectionName;
            }
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public ConnectionSettings ConnectionSetting
        {
            get
            {
                if (_connectSetting == null)
                    throw new Exception("未配置连接字符串");
                return _connectSetting;
            }
            set
            {
                _connectSetting = value;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 创建数据访问实例
        /// </summary>
        /// <param name="setting">数据连接配置</param>
        public DataAccess(ConnectionSettings setting)
        {
            _connectSetting = setting;
        }
        #endregion

        /// <summary>
        /// 创建数据连接
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            C conn = new C();
            conn.ConnectionString = this.ConnectionSetting.ConnectionString;
            return conn;
        }

        static CommandType GuessCommandType(string commandText)
        {
            return commandText.Trim().Contains(" ") ? CommandType.Text : CommandType.StoredProcedure;
        }

        /// <summary>
        /// 创建参数化语句（可以是子句）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="refParameters"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string CreateDbCommand(string text, ref List<IDataParameter> refParameters, params object[] args)
        {
            if (!args.Any())
                return text;
            Regex regex = new Regex(@"(?<=\{)\d+(?=\})", RegexOptions.IgnoreCase);
            var match = regex.Matches(text);
            if (match.Count == 0)
                return text;

            int argsLength = args.Length;
            if (args == null || argsLength == 0)
                return text;

            List<IDataParameter> parameters = new List<IDataParameter>();
            var sqlParamPrefix = $"P_{text.GetHashCode().ToString().Replace("-", "A")}";
            Dictionary<int, bool> dict = new Dictionary<int, bool>();
            for (var i = 0; i < match.Count; i++)
            {
                var index = Convert.ToInt32(match[i].Value);
                if (index >= argsLength && !dict.Keys.Contains(index))
                {
                    continue;
                }

                var paramValue = args[index];
                dict[index] = true;
                // 如果参数的值是数组或列表，则展开针对每个元素生成一个参数
                if (paramValue != null
                    && (paramValue.GetType().IsArray || paramValue is IEnumerable<object>))
                {
                    var paramValueList = (paramValue as IEnumerable<object>).ToArray();
                    var valueIndex = 0;
                    List<String> argNames = new List<string>();
                    foreach (var item in paramValueList)
                    {
                        String paramName = $"{sqlParamPrefix}_{index}_{valueIndex}";
                        string argName = CommandBuildProvider.ParameterPrefix + paramName;
                        var param = CommandBuildProvider.MakeIn(argName, item);
                        argNames.Add(argName);
                        refParameters.Add(param);
                        valueIndex++;
                    }
                    text = text.Replace("{" + index + "}", $"{string.Join(",", argNames)}");
                }
                else
                {
                    var paramName = $"{sqlParamPrefix}_{index}";
                    var param = this.CommandBuildProvider.MakeIn(paramName, args[index]);
                    text = text.Replace("{" + index + "}", $"{ CommandBuildProvider.ParameterPrefix}{paramName}");
                    refParameters.Add(param);
                }
            }
            return text;
            //text = $"({text})";
        }

        #region ExecuteReader:执行数据库命令，返回数据读取器

        /// <summary>
        /// 执行一个数据库命令，返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库文本，带{d}参数模板</param>
        /// <param name="args">{d}对应的值</param>
        /// <returns></returns>
        public IResultReader ExecuteReaderByArgs(string commandText, params object[] args)
        {
            List<IDataParameter> appendAndWhereParameter = new List<IDataParameter>();
            commandText = CreateDbCommand(commandText, ref appendAndWhereParameter, args);
            return ExecuteReader(commandText, appendAndWhereParameter?.ToArray());
        }

        /// <summary>
        /// 执行一个数据库命令，返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令参数</param>
        /// <returns></returns>
        public IResultReader ExecuteReader(string commandText, params IDataParameter[] commandParameters)
        {
            C connection = this.CreateConnection() as C;
            return ExecuteReader(connection, null, commandText, commandParameters);
        }

       

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            C connection = this.CreateConnection() as C;
            return ExecuteReader(connection, null, commandText, commandType, commandParameters, ConnOwnerShip.Internal);
        }

        /// <summary>
        /// 执行一个数据库命令，返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令参数</param>
        /// <returns></returns>
        public Task<IResultReader> ExecuteReaderAsync(string commandText, params IDataParameter[] commandParameters)
        {
            return Task.Factory.StartNew(() => ExecuteReader(commandText, commandParameters));
        }


        /// <summary>
        /// 执行一个数据库命令，返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库链接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令参数</param>
        /// <param name="connectionOwnership">数据库命令的连接类型</param>
        /// <returns></returns>
        protected virtual IResultReader ExecuteReader(C connection, IDbTransaction transaction, string commandText, IDataParameter[] commandParameters, ConnOwnerShip connectionOwnership = ConnOwnerShip.Internal)
        {
            IDbCommand cmd = CreateCommand(connection);
            DbTools.PrepareCommand(cmd, connection, transaction, CommandType.Text, commandText, commandParameters);
            try
            {
                // DbTools.WriteDbException(new Exception("没有错误"), cmd);
                //if (cmd is Oracle.ManagedDataAccess.Client.OracleCommand)
                //{
                //    (cmd as Oracle.ManagedDataAccess.Client.OracleCommand).BindByName = true;
                //}
                if (connectionOwnership == ConnOwnerShip.External)
                {
                    return new DataReader(cmd.ExecuteReader(), cmd);
                }
                else
                {
                    return new DataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection), cmd);
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                DbTools.WriteDbException(ex, cmd);
                throw ex;
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <param name="connectionOwnership">数据库命令的连接类型</param>
        /// <returns>返回一个数据读取器</returns>
        private DataReader ExecuteReader(C connection, IDbTransaction transaction, string commandText, CommandType commandType, IDbDataParameter[] commandParameters, ConnOwnerShip connectionOwnership)
        {
            IDbCommand cmd = connection.CreateCommand();
            DbTools.PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);
            try
            {
                if (connectionOwnership == ConnOwnerShip.External)
                {
                    return new DataReader(cmd.ExecuteReader(), cmd);
                }
                else
                {
                    return new DataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection), cmd);
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                DbTools.WriteDbException(ex, cmd);
                throw ex;
            }
        }
        #endregion 

        #region ExecuteScalar:执行数据库命令,返回结果的第一行第一列       
        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(string commandText, params IDataParameter[] commandParameters)
        {
            using (C conn = CreateConnection() as C)
            {
                return ExecuteScalar(conn, commandText, CommandType.Text, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteScalar(connection, commandText, GuessCommandType(commandText), commandParameters);
        } 

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C cn = CreateConnection() as C)
            {
                return ExecuteScalar(cn, commandText, commandType, commandParameters);
            }
        }

        private ScalerResult ExecuteScalar(C connection, string commandText, CommandType commandType, params IDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = CreateCommand(connection))
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                try
                {
                    return new ScalerResult(cmd.ExecuteScalar(), cmd);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="args"></param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalarByArgs(string commandText, params object[] args)
        {
            var commandParameters = new List<IDataParameter>();
            commandText = CreateDbCommand(commandText, ref commandParameters, args);
            using (C conn = CreateConnection() as C)
            {
                return ExecuteScalar(conn, commandText, CommandType.Text, commandParameters.ToArray());
            }
        }


        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令参数</param>
        /// <param name="commandParameters"></param>
        /// <returns>返回结果的第一行第一列</returns>
        public Task<ScalerResult> ExecuteScalarAsync(string commandText, params IDataParameter[] commandParameters)
        {
            return Task.Factory.StartNew(() =>
            {
                return ExecuteScalar(commandText, commandParameters);
            });
        }
        #endregion

        #region NonQueryResult:执行数据库命令,返回受影响的行数

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(string commandText, params IDataParameter[] commandParameters)
        {
            using (C cn = this.CreateConnection() as C)
            {
                return ExecuteNonQuery(cn, commandText, CommandType.Text, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C cn = this.CreateConnection() as C)
            {
                return ExecuteNonQuery(cn, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="args">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQueryByArgs(string commandText, params object[] args)
        {
            var commandParameters = new List<IDataParameter>();
            commandText = CreateDbCommand(commandText, ref commandParameters, args);
            using (C cn = this.CreateConnection() as C)
            {
                return ExecuteNonQuery(cn, commandText, CommandType.Text, commandParameters.ToArray());
            }
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public Task<NonQueryResult> ExecuteNonQueryAsync(string commandText, params IDataParameter[] commandParameters)
        {
            return Task.Factory.StartNew(() =>
            {
                return ExecuteNonQuery(commandText, commandParameters);
            });
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        private NonQueryResult ExecuteNonQuery(C connection, string commandText, CommandType commandType, params IDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = CreateCommand(connection))
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                //---继续 
                try
                {
                    var result = new NonQueryResult(cmd.ExecuteNonQuery(), cmd);
                    return result;
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }
        #endregion

        #region ExecuteTransaction:执行一系列的事务
        /// <summary>
        /// 执行一系列的事务
        /// </summary>
        /// <param name="commands">数据库命令</param>
        public IDbTransaction PreCommitTransaction(IList<DataCommand> commands)
        {
            C conn = this.CreateConnection() as C;
            return PreCommitTransaction(conn, commands);
        }

        /// <summary>
        /// 执行SQL事务
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        public IDbTransaction PreCommitTransaction(C connection, IList<DataCommand> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return null;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDbTransaction trans = connection.BeginTransaction();
            PreCommitTransaction(trans, commands);
            return trans;
        }

        /// <summary>
        /// 在一个事务上执行SQL语句.执行完后不提交,出错后也不回滚,需要手动处理
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commands">要执行的命令列表</param>
        public int PreCommitTransaction(IDbTransaction transaction, IList<DataCommand> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return 0;
            }
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                try
                {
                    int r = 0;
                    foreach (var command in commands)
                    {
                        DbTools.PrepareCommand(cmd, transaction.Connection, transaction, command.CommandType, command.CommandText, command.Parameters);
                        r = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    return r;
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        #endregion

        /// <summary>
        /// 创建数据库命令
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        protected virtual IDbCommand CreateCommand(C connection)
        {
            var cmd = connection.CreateCommand();
            return cmd;
        }


        #region 执行数据库命令,返回一个数据集

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(C connection, string commandText)
        {
            return ExecuteDataSet(connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(IDbTransaction transaction, string commandText)
        {
            return ExecuteDataSet(transaction, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataSet(commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataSet(connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataSet(transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C cn = this.CreateConnection() as C)
            {
                return ExecuteDataSet(cn, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                A da =  Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return new DataSetResult(ds, cmd);
                        }
                    }
                    throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                A da =  Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return new DataSetResult(ds, cmd);
                        }
                    }
                    throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        #endregion

        #region 执行数据库命令,返回一个数据表

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, CommandType.Text, null);
        }



        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, string commandText)
        {
            return ExecuteDataTable(connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, string commandText)
        {
            return ExecuteDataTable(transaction, commandText, CommandType.Text, null);
        } 

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataTable(commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataTable(connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataTable(transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C connection = this.CreateConnection() as C)
            {
                return ExecuteDataTable(connection, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                A da =  Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            DataTable dt = new DataTable();
                            (da as DbDataAdapter).Fill(dt);
                            return new DataTableResult(dt, cmd);
                        }
                    }
                    else
                    {
                        using (da as IDisposable)
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return new DataTableResult(ds.Tables[0], cmd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                A da =  Reflect.CreateInstance<A>();
                using (da as IDisposable)
                {
                    da.SelectCommand = cmd;
                    try
                    {
                        if (da is DbDataAdapter)
                        {
                            using (da as IDisposable)
                            {
                                DataTable dt = new DataTable();
                                (da as DbDataAdapter).Fill(dt);
                                return new DataTableResult(dt, cmd);
                            }
                        }
                        else
                        {
                            using (da as IDisposable)
                            {
                                DataSet ds = new DataSet();
                                da.Fill(ds);
                                return new DataTableResult(ds.Tables[0], cmd);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DbTools.WriteDbException(ex, cmd);
                        throw ex;
                    }
                }
            }
        }

          
        #endregion 执行数据库命令,返回一个数据表

        #region 参数管理
        /// <summary>
        /// 获取存储过程的参数
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>存储过程的参数数组</returns>
        protected abstract IDbDataParameter[] GetProcParameters(string procName);

        /// <summary>
        /// 创建线程安全的保存数据库命令参数的表.
        /// </summary>
        private Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 缓存数据库命令参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="ps">参数列表,如果参数不继承ICloneable,则不缓存</param>
        public IDbDataParameter[] SaveParameters(string key, params IDbDataParameter[] ps)
        {
            if (ps.Length > 0 && ps[0] is ICloneable)
            {
                parmCache[key] = ps;
            }
            return ps;
        }

        /// <summary>
        /// 从缓存中获取数据库命令参数
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>数据库命令参数</returns>
        public IDbDataParameter[] GrabParameters(string key)
        {
            if (parmCache.ContainsKey(key))
            {
                IDbDataParameter[] cachedParms = (IDbDataParameter[])parmCache[key];
                IDbDataParameter[] clonedParms = new P[cachedParms.Length];
                if (cachedParms[0] is ICloneable)
                {
                    for (int i = 0, j = cachedParms.Length; i < j; i++)
                    {
                        clonedParms[i] = (P)((ICloneable)cachedParms[i]).Clone();
                    }
                }
                else
                {
                    for (int i = 0, j = cachedParms.Length; i < j; i++)
                    {
                        clonedParms[i] = new P();
                        clonedParms[i].ParameterName = cachedParms[i].ParameterName;
                        clonedParms[i].DbType = cachedParms[i].DbType;
                        clonedParms[i].Direction = cachedParms[i].Direction;
                        clonedParms[i].Size = cachedParms[i].Size;
                        clonedParms[i].Precision = cachedParms[i].Precision;
                        clonedParms[i].Scale = cachedParms[i].Scale;
                    }
                }
                return clonedParms;
            }
            return null;
        }
        #endregion

        #region 存储过程

        /// <summary>
        /// 执行数据库存储过程,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteProcReader(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteReader(commandText, CommandType.StoredProcedure, pvs);
        }

        /// <summary>
        /// 执行数据库存储过程,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteProcScalar(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteScalar(commandText, CommandType.StoredProcedure, pvs);
        }

        /// <summary>
        /// 执行数据库存储过程,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteProcDataTable(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            if (pvs != null)
            {
                for (int i = 0; i < pvs.Length && i < ps.Length; i++)
                {
                    pvs[i].Value = ps[i];
                }
            }
            return ExecuteDataTable(commandText, CommandType.StoredProcedure, pvs);
        }

        /// <summary>
        /// 执行数据库存储过程,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteProcDataSet(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteDataSet(commandText, CommandType.StoredProcedure, pvs);
        }

        /// <summary>
        /// 执行数据库存储过程,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteProcNonQuery(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteNonQuery(commandText, CommandType.StoredProcedure, pvs);
        }

        #endregion
    }
}
