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
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;

namespace ObjectQL.Data.SqlServer
{
    /// <summary>
    /// 用于SqlServer数据库命令的创建
    /// </summary>
    public class SqlServerCommandBuildProvider : CommandBuildProvider<SqlParameter, SqlDbType>, ICommandBuildProvider
    {
        /// <summary>
        /// 查询所有数据库表
        /// </summary>
        public override string AllTableSql
        {
            get
            {
                return "SELECT Name FROM SysObjects Where XType='U' ORDER BY Name";
            }

        }
        /// <summary>
        /// 字符串连接操作符
        /// </summary>
        public override string ConnectFixSymbol
        {
            get
            {
                return "+";
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
            string sql = String.Format("SELECT TOP {0} *  FROM ( SELECT ROW_NUMBER() OVER (ORDER BY getdate()) AS RowNumber,* FROM ({1}) as _bb ) as _aa WHERE RowNumber > {2}", limit, commandText.Insert(7, " top 100 percent "), start);
            return sql;
        }

        /// <summary>
        /// 根据指定对象获取其数据库类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override SqlDbType GetDbType(object value)
        {
            if (value == null)
            {
                return SqlDbType.VarChar;
            }
            String type = value.GetType().ToString();
            if (value is Int16)
            {
                return SqlDbType.SmallInt;
            }
            else if (value is Int32)
            {
                return SqlDbType.Int;
            }
            else if (value is Int64)
            {
                return SqlDbType.BigInt;
            }
            else if (value is Decimal)
            {
                return SqlDbType.Decimal;
            }
            else if (value is Double)
            {
                return SqlDbType.Float;
            }
            else if (value is DateTime)
            {
                return SqlDbType.DateTime;
            }
            else if (value is float)
            {
                return SqlDbType.Real;
            }
            else if (value is byte[])
            {
                return SqlDbType.Binary;
            }
            else if (value is char[])
            {
                return SqlDbType.Char;
            }
            else if (value is TimeSpan)
            {
                // INTERVAL DAY TO SECOND
                return SqlDbType.Timestamp;
            }
            else
            {
                return SqlDbType.VarChar;
            }
        }

        protected override SqlParameter CreateParameter(string parameterName, SqlDbType type)
          => new SqlParameter(parameterName, type);


        protected override SqlParameter CreateParameter(string parameterName, SqlDbType type, int size)
         => new SqlParameter(parameterName, type, size);

        protected override Object GetDbValue(object value, SqlDbType type)
        {
            return value;
        }
    }
}