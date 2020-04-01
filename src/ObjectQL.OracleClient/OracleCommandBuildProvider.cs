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
using Oracle.ManagedDataAccess.Client;
using OracleType = Oracle.ManagedDataAccess.Client.OracleDbType;

namespace ObjectQL.Data.OracleClient
{
    /// <summary>
    /// 用于Oracle数据库命令的创建
    /// </summary>
    public class OracleCommandBuildProvider : CommandBuildProvider<OracleParameter, OracleType>, ICommandBuildProvider
    {
        /// <summary>
        /// 查询所有数据库表
        /// </summary>
        public override string AllTableSql
        {
            get
            {
                return "select   TABLE_NAME  AS NAME  from   user_tables  ";
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
            }
        }
        /// <summary>
        /// 变量名前缀
        /// </summary>
        public override string ParameterPrefix
        {
            get
            {
                return ":";
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
            int end = start + limit - 1;
            string sql = String.Format("SELECT * FROM (SELECT ROWNUM AS RN, AA.* FROM ({0}) AA  ) WHERE RN BETWEEN {1} AND {2}", commandText, start, end);
            return sql;
        }

        #region 公共方法 

        #endregion

        /// <summary>
        /// 根据数据库类型转换数据类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override Object GetDbValue(object value, OracleType type)
        {
            if (type == OracleType.Int16)
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
        protected override OracleType GetDbType(object value)
        {
            if (value == null)
            {
                return OracleType.Varchar2;
            }
            String type = value.GetType().ToString();
            if (value is Int16)
            {
                return OracleType.Int16;
            }
            else if (value is Int32)
            {
                return OracleType.Int32;
            }
            else if (value is Int64)
            {
                return OracleType.Int64;
            }
            else if (value is Boolean)
            {
                return OracleType.Int16;
            }
            else if (value is Decimal)
            {
                return OracleType.Decimal;
            }
            else if (value is Double)
            {
                return OracleType.Double;
            }
            else if (value is DateTime)
            {
                return OracleType.Date;
            }
            else if (value is float)
            {
                return OracleType.Single;
            }
            else if (value is byte[])
            {
                return OracleType.Blob;
            }
            else if (value is char[])
            {
                return OracleType.Clob;
            }
            else if (value is TimeSpan)
            {
                // INTERVAL DAY TO SECOND
                return OracleType.IntervalDS;
            }
            else
            {
                return OracleType.Varchar2;
            }
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="oraType"></param>
        /// <returns></returns>
        protected override OracleParameter CreateParameter(string parameterName, OracleDbType oraType)
            => new OracleParameter(parameterName, oraType);

        /// <summary>
        /// 创建参数 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="oraType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected override OracleParameter CreateParameter(string parameterName, OracleDbType oraType, int size)
         => new OracleParameter(parameterName, oraType, size);
    }
}
