/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：OracleDataAccess
 * 命名空间：ObjectQL.Data.Impl
 * 文 件 名：OracleDataAccess
 * 创建时间：2016/10/19 15:41:48
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ObjectQL.Data.OracleClient
{
    /// <summary>
    /// 
    /// </summary>
    public static class OracleConnectionExt
    {
        public static IDbCommand CreateCommand(this IDbConnection connect)
        {
            var cmd = connect.CreateCommand();
            if (cmd is OracleCommand)
            {
                (cmd as OracleCommand).BindByName = true;
            }
            return cmd;
        }
    }
    /// <summary>
    /// 用于Oracle数据库访问
    /// </summary>
    public class OracleDataAccess
        : DataAccess<OracleConnection, OracleDataAdapter, OracleTransaction, OracleParameter>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public OracleDataAccess(ConnectionSettings setting) : base(setting)
        {

        }

        private ICommandBuildProvider _commandBuildProvider = new OracleCommandBuildProvider();
        /// <summary>
        /// 创建数据库命名的Provider
        /// </summary>
        public override ICommandBuildProvider CommandBuildProvider
        {
            get
            {
                return _commandBuildProvider;
            }
            set
            {
                _commandBuildProvider = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        protected override IDbCommand CreateCommand(OracleConnection connection)
        {
            var cmd = connection.CreateCommand();
            // 解决ORACLE ODP.NET的参数化顺序与SQL中的顺序不一直到导致不能正确加载数据的问题
            cmd.BindByName = true;
            return cmd;
        }

        /// <summary>
        /// 获取存储过程的参数
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>存储过程的参数</returns>
        protected override IDbDataParameter[] GetProcParameters(string procName)
        {
            IDbDataParameter[] pvs = GrabParameters(procName);
            if (pvs == null)
            {
                using (OracleCommand cmd = new OracleCommand(procName, CreateConnection() as OracleConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    OracleCommandBuilder.DeriveParameters(cmd);
                    cmd.Connection.Dispose();
                    //cmd.Parameters.RemoveAt(0);
                    pvs = new OracleParameter[cmd.Parameters.Count];
                    cmd.Parameters.CopyTo(pvs, 0);
                    SaveParameters(procName, pvs);
                    pvs = GrabParameters(procName);
                }
            }
            return pvs;
        }
    }
}
