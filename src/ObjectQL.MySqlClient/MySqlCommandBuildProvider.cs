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

using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;

namespace ObjectQL.Data.MySqlClient
{
    /// <summary>
    /// 用于MySql数据库命令的创建
    /// </summary>
    public class MySqlCommandBuildProvider : CommandBuildProvider<MySqlParameter, MySqlDbType>, ICommandBuildProvider
    {
        /// <summary>
        /// 查询所有数据库表
        /// </summary>
        public override string AllTableSql
        {
            get
            {
                return "select table_name from information_schema.tables"; // + "where table_schema='test'";
            }

        }
        /// <summary>
        /// 字符串连接操作符
        /// </summary>
        public override string ConnectFixSymbol
        {
            get
            {
                return "||";
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
            string sql = String.Format("SELECT * FROM ({0})as total  LIMIT {1},{2}", commandText, start, limit);
            return sql;
        }

        protected override Object GetDbValue(object value, MySqlDbType type)
        {
            if (type == MySqlDbType.Int16)
            {
                value = Convert.ToInt16(value);
            }
            return value;
        }


        /// <summary>
        /// 根据指定对象获取其数据库类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override MySqlDbType GetDbType(object value)
        {
            if (value == null)
            {
                return MySqlDbType.VarChar;
            }
            String type = value.GetType().ToString();
            if (value is Int16)
            {
                return MySqlDbType.Int16;
            }
            else if (value is Int32)
            {
                return MySqlDbType.Int32;
            }
            else if (value is Int64)
            {
                return MySqlDbType.Int64;
            }
            else if (value is Decimal)
            {
                return MySqlDbType.Decimal;
            }
            else if (value is Double)
            {
                return MySqlDbType.Double;
            }
            else if (value is DateTime)
            {
                return MySqlDbType.DateTime;
            }
            else if (value is float)
            {
                return MySqlDbType.Float;
            }
            else if (value is byte[])
            {
                return MySqlDbType.Blob;
            }
            else if (value is char[])
            {
                return MySqlDbType.String;
            }
            else if (value is TimeSpan)
            {
                // INTERVAL DAY TO SECOND
                return MySqlDbType.Timestamp;
            }
            else
            {
                return MySqlDbType.VarChar;
            }
        }


        protected override MySqlParameter CreateParameter(string parameterName, MySqlDbType type)
            => new MySqlParameter(parameterName, type);


        protected override MySqlParameter CreateParameter(string parameterName, MySqlDbType type, int size)
         => new MySqlParameter(parameterName, type, size);
    }
}
