/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：SqlCollectors
 * 命名空间：ObjectQL.Data
 * 文 件 名：SqlCollectors
 * 创建时间：2017/5/2 22:18:08
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ObjectQL.Data;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlCollectors
    {
        private readonly static object _myLock = new object();
        private Dictionary<IDataAccess, IList<DataCommand>> dataAccessCommands = new Dictionary<IDataAccess, IList<DataCommand>>();

        /// <summary>
        /// 事务中增加操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="command"></param>
        public void AddExecuteNonQuery(IDataAccess db, DataCommand command)
        {
            lock (_myLock)
            {
                if (!dataAccessCommands.Keys.Contains(db))
                {
                    dataAccessCommands[db] = new List<DataCommand>();
                } 
                dataAccessCommands[db].Add(command);
            }
        }

        /// <summary>
        /// 完成事务
        /// </summary>
        public void Complete()
        {
            // 安全第一
            lock (_myLock)
            { 
                var list = new List<IDbTransaction>();
                try
                {
                    foreach (var item in dataAccessCommands)
                    {
                        if (item.Value != null && item.Value.Any())
                        {
                            var trans = item.Key.PreCommitTransaction(item.Value);
                            list.Add(trans);
                        }
                    }
                    Commit(list);
                }
                catch (Exception ex)
                {
                    Rollback(list);
                    throw ex;
                }
                finally
                {
                    Dispose(list);
                }
                list = null;
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="transList"></param>
        protected void Dispose(List<IDbTransaction> transList)
        {
            dataAccessCommands.Clear();
            if (transList == null || !transList.Any())
                return;
            for (int i = 0, j = transList.Count(); i < j; i++)
            {
                if (transList[i].Connection == null)
                    continue;
                transList[i].Connection.Close();
                transList[i].Connection.Dispose();
                transList[i].Dispose();
                transList[i] = null;
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="transList"></param>
        protected void Rollback(List<IDbTransaction> transList)
        {
            if (transList == null || !transList.Any())
                return;
            for (int i = 0, j = transList.Count(); i < j; i++)
            {
                var conn = transList[i].Connection;
                transList[i].Rollback();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// 完成并提交
        /// </summary>
        /// <param name="transList"></param>
        protected void Commit(List<IDbTransaction> transList)
        {
            if (transList == null || !transList.Any())
                return;
            for (int i = 0, j = transList.Count(); i < j; i++)
            {
                var conn = transList[i].Connection;
                transList[i].Commit();
                if(conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}
