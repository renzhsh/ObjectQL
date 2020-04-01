using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ObjectQL.Data
{
    /// <summary>
    /// 数据访问接口
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// 所有表的名称
        /// </summary>
        IEnumerable<string> Tables { get;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        ConnectionSettings ConnectionSetting { get; set; }

        /// <summary>
        /// 数据库命令创建支持
        /// </summary>
        ICommandBuildProvider CommandBuildProvider { set; get; }

        /// <summary>
        /// 连接名称
        /// </summary>
        string ConnectionName { get; }

        #region ExecuteReader:执行数据库命令，返回数据读取器 
        /// <summary>
        /// 执行一个数据库命令，返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库文本，带{d}参数模板</param>
        /// <param name="args">{d}对应的值</param>
        /// <returns></returns>
        IResultReader ExecuteReaderByArgs(string commandText, params object[] args);

        /// <summary>
        /// 执行一个数据库命令，返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令参数</param>
        /// <returns></returns>
        IResultReader ExecuteReader(string commandText, params IDataParameter[] commandParameters);

        /// <summary>
        /// 执行一个数据库命令，返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令参数</param>
        /// <returns></returns>
        Task<IResultReader> ExecuteReaderAsync(string commandText, params IDataParameter[] commandParameters);
        #endregion

        #region ExecuteScalar:执行数据库命令,返回结果的第一行第一列
        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令参数</param>
        /// <param name="commandParameters"></param>
        /// <returns>返回结果的第一行第一列</returns>
        ScalerResult ExecuteScalar(string commandText, params IDataParameter[] commandParameters);

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本，带占位符</param>
        /// <param name="args">带占位的参数</param>
        /// <returns></returns>
        ScalerResult ExecuteScalarByArgs(string commandText, params object[] args);

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        Task<ScalerResult> ExecuteScalarAsync(string commandText, params IDataParameter[] commandParameters);
        #endregion

        #region NonQueryResult:执行数据库命令,返回受影响的行数

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        NonQueryResult ExecuteNonQuery(string commandText, params IDataParameter[] commandParameters);

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="args">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        NonQueryResult ExecuteNonQueryByArgs(string commandText, params object[] args);

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        Task<NonQueryResult> ExecuteNonQueryAsync(string commandText, params IDataParameter[] commandParameters);
        #endregion

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// 创建参数化语句（可以是子句）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="refParameters"></param>
        /// <param name="args"></param>
        string CreateDbCommand(string text, ref List<IDataParameter> refParameters, params object[] args);

        /// <summary>
        /// 预提交语句并返回事务
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        IDbTransaction PreCommitTransaction(IList<DataCommand> commands);

        #region
        /// <summary>
        /// 执行数据库存储过程,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据读取器</returns>
        DataReader ExecuteProcReader(string commandText, params object[] ps);


        /// <summary>
        /// 执行数据库存储过程,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回结果的第一行第一列</returns>
        ScalerResult ExecuteProcScalar(string commandText, params object[] ps);

        /// <summary>
        /// 执行数据库存储过程,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据表</returns>
        DataTableResult ExecuteProcDataTable(string commandText, params object[] ps);

        /// <summary>
        /// 执行数据库存储过程,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据集</returns>
        DataSetResult ExecuteProcDataSet(string commandText, params object[] ps);

        /// <summary>
        /// 执行数据库存储过程,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回受影响的行数</returns>
        NonQueryResult ExecuteProcNonQuery(string commandText, params object[] ps);
        #endregion
    }
}
