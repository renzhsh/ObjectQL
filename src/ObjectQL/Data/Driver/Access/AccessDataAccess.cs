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
using System.Data;
using System.Data.OleDb;

namespace ObjectQL.Data.AccessClient
{
    /// <summary>
    /// 用于Access数据库访问
    /// </summary>
    public class AccessDataAccess
        : DataAccess<OleDbConnection, OleDbDataAdapter, OleDbTransaction, OleDbParameter>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public AccessDataAccess(ConnectionSettings setting) : base(setting)
        {
        }

        private ICommandBuildProvider _commandBuildProvider = new AccessCommandBuildProvider();
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
        /// 获取存储过程的参数
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>存储过程的参数</returns>
        protected override IDbDataParameter[] GetProcParameters(string procName)
        {
            IDbDataParameter[] pvs = GrabParameters(procName);
            if (pvs == null)
            {
                using (OleDbCommand cmd = new OleDbCommand(procName, CreateConnection() as OleDbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    OleDbCommandBuilder.DeriveParameters(cmd);
                    cmd.Connection.Dispose();
                    cmd.Parameters.RemoveAt(0);
                    pvs = new OleDbParameter[cmd.Parameters.Count];
                    cmd.Parameters.CopyTo(pvs, 0);
                    SaveParameters(procName, pvs);
                    pvs = GrabParameters(procName);
                }
            }
            return pvs;
        }
    }
}
