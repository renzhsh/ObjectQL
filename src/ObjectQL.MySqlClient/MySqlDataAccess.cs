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
using MySql.Data.MySqlClient;
using System.Data;

namespace ObjectQL.Data.MySqlClient
{
    /// <summary>
    /// 用于MySql数据库访问
    /// </summary>
    public class MySqlDataAccess
        : DataAccess<MySqlConnection, MySqlDataAdapter, MySqlTransaction, MySqlParameter>
    {
        public MySqlDataAccess(ConnectionSettings setting) : base(setting)
        {
        }

        private ICommandBuildProvider _commandBuildProvider = new MySqlCommandBuildProvider();
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
                using (MySqlCommand cmd = new MySqlCommand(procName, CreateConnection() as MySqlConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    MySqlCommandBuilder.DeriveParameters(cmd);
                    cmd.Connection.Dispose();
                    cmd.Parameters.RemoveAt(0);
                    pvs = new MySqlParameter[cmd.Parameters.Count];
                    cmd.Parameters.CopyTo(pvs, 0);
                    SaveParameters(procName, pvs);
                    pvs = GrabParameters(procName);
                }
            }
            return pvs;
        }
    }
}
