/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：OracleCommandProvider
 * 命名空间：ObjectQL.Data.Impl
 * 文 件 名：OracleCommandProvider
 * 创建时间：2016/10/19 15:46:48
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
    /// 用于Access数据库命令的创建
    /// </summary>
    public class AccessCommandBuildProvider : CommandBuildProvider<OleDbParameter, OleDbType>, ICommandBuildProvider
    {
        /// <summary>
        /// 查询所有数据库表
        /// </summary>
        public override string AllTableSql
        {
            get
            {
                return "select name from MSysObjects where type=1 and flags=0";
            }

        }
        /// <summary>
        /// 字符串连接操作符
        /// </summary>
        public override string ConnectFixSymbol
        {
            get
            {
                return "&";
                //return "concat()";
            }
        }
        /// <summary>
        /// 变量名前缀
        /// </summary>
        public override string ParameterPrefix
        {
            get
            {
                return "@";
            }
        }
        /// <summary>
        /// 分页语句
        /// </summary>
        /// <param name="commandText">条件范围</param>
        /// <param name="start">开始页</param>
        /// <param name="limit">页面容量</param>
        /// <returns></returns>
        public override string CreatePageSql(string commandText, int start, int limit)
        {
            string[] s1 = commandText.Split(' ');
            string[] s2 = s1[1].Split(',');
            string[] s3 = s2[0].Split('.');
            string field = s3[1];
            string table = s1[3];
            string sql = null;
            int end = start + limit;
            if (start == 0)
                sql = String.Format("select top {0} * from ({1}) _aa", end, commandText);
            else
                sql = String.Format("select a.* from ( select top {0} * from ({1}) _aa) a left join ( select top {2} * from ({3}) _bb ) b on a.{4} = b.{5} where iif(b.{6}, '0', '1') = '1'", end, commandText, start, commandText, field, field, field);
            return sql;
        }

        /// <summary>
        /// 根据指定对象获取其数据库类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override OleDbType GetDbType(object value)
        {
            if (value == null)
            {
                return OleDbType.VarChar;
            }
            String type = value.GetType().ToString();
            if (value is Int16)
            {
                return OleDbType.SmallInt;
            }
            else if (value is Int32)
            {
                return OleDbType.Integer;
            }
            else if (value is Int64)
            {
                return OleDbType.BigInt;
            }
            else if (value is Decimal)
            {
                return OleDbType.Decimal;
            }
            else if (value is Double)
            {
                return OleDbType.Double;
            }
            else if (value is DateTime)
            {
                return OleDbType.DBTimeStamp;
            }
            else if (value is float)
            {
                return OleDbType.Single;
            }
            else if (value is byte[])
            {
                return OleDbType.Binary;
            }
            else if (value is char[])
            {
                return OleDbType.Char;
            }
            else if (value is TimeSpan)
            {
                // INTERVAL DAY TO SECOND
                return OleDbType.DBTimeStamp;
            }
            else
            {
                return OleDbType.VarChar;
            }
        }


        protected override OleDbParameter CreateParameter(string parameterName, OleDbType type)
            => new OleDbParameter(parameterName, type);


        protected override OleDbParameter CreateParameter(string parameterName, OleDbType type, int size)
         => new OleDbParameter(parameterName, type, size);

        protected override Object GetDbValue(object value, OleDbType type)
        {
            return value;
        }
    }
}
