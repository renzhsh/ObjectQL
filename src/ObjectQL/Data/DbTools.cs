/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DbTools
 * 命名空间：ObjectQL.Data
 * 文 件 名：DbTools
 * 创建时间：2016/10/21 16:04:37
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Data;
using Jinhe;

namespace ObjectQL.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DbTools
    {
        /// <summary>
        /// 为Command命令添加相关的信息
        /// </summary>
        /// <param name="command">原始的Command命令</param>
        /// <param name="conn">数据库连接</param>
        /// <param name="tran">事务</param>
        /// <param name="ct">Command命令的类型</param>
        /// <param name="txt">Command命令的文本</param>
        /// <param name="cps">Command命令的参数</param>
        public static void PrepareCommand(IDbCommand command, IDbConnection conn, IDbTransaction tran, CommandType ct, string txt, IDataParameter[] cps)
        {
            command.Connection = conn;
            command.CommandText = txt;
            command.CommandType = ct;
            if (tran != null && tran.Connection == null)
            {
                throw new ArgumentException("事务已经回滚或提交.", "transaction");
            }
            command.Transaction = tran;
            if (cps != null && cps.Length > 0)
            {
                for (int i = 0, j = cps.Length; i < j; i++)
                {
                    var parameter = cps[i];
                    if (parameter != null
                        && (parameter.Direction == ParameterDirection.Input || parameter.Direction == ParameterDirection.InputOutput)
                        && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cps[i] = null;
                    command.Parameters.Add(parameter);
                }
            }
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
        }


        /// <summary>
        /// 记录数据访问异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="cmd">执行的数据库命令</param>
        public static void WriteDbException(Exception ex, IDbCommand cmd)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (cmd != null)
            {
                sb.AppendLine(string.Format("数据命令类型:{0}", cmd.CommandType));
                sb.AppendLine(string.Format("数据命令文本:{0}", cmd.CommandText));

                if (cmd.Parameters.Count > 0)
                {
                    sb.AppendLine("数据命令参数:");
                    int max = 0;
                    foreach (IDataParameter p in cmd.Parameters)
                    {
                        max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                    }
                    foreach (IDataParameter p in cmd.Parameters)
                    {
                        sb.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                    }
                }
            }
            LogAdapter.Db.Error(LogFileSpan.Day, string.Format("错误位置:{1}.{2}{0}错误描述:{0}{3}{4}{5}",
                Environment.NewLine,
                ex.TargetSite?.ReflectedType?.FullName,
                ex.TargetSite?.Name,
                ex.Message,
                ex.Message.EndsWith(Environment.NewLine) ? string.Empty : Environment.NewLine,
                sb));
        }
    }
}
