using System;
using System.Collections.Generic;
using System.Data;

namespace ObjectQL.Data
{
    /// <summary>
    /// 用于数据库命令的创建
    /// </summary>
    public interface ICommandBuildProvider
    {
        /// <summary>
        /// 查询所有用户表的语句
        /// </summary>
        string AllTableSql {
            get;
        }

        /// <summary>
        /// 参数变量名前缀（不同数据库可能不同）
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// 字符串连接操作运算符
        /// </summary>
        string ConnectFixSymbol { get; }
         

        /// <summary>
        /// 创建输入参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        IDataParameter MakeIn(String parameterName, Object parameterValue);

        /// <summary>
        /// 创建输入参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="size">参数值长度</param>
        /// <returns></returns>
        IDataParameter MakeIn(String parameterName, Object parameterValue, int size);

        /// <summary>
        /// 获取数据库参数集合
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        IEnumerable<IDataParameter> GetDbParameters(Dictionary<string, object> parameterValues);

        /// <summary>
        /// 获取数据库参数集合
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        IEnumerable<IDataParameter> GetDbParameters(IEnumerable<KeyValuePair<string, object>> parameterValues);

        /// <summary>
        /// 创建输出参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns></returns>
        IDataParameter MakeOut(String parameterName, Object parameterValue);

        /// <summary>
        /// 创建输出参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="size">参数值长度</param>
        /// <returns></returns>
        IDataParameter MakeOut(string parameterName, object parameterValue, int size);

        /// <summary>
        /// 创建分页语句
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="start">skip</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        string CreatePageSql(string commandText, int start, int limit);
    }
}
