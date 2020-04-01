using System.Collections.Generic;
using System.Data;

namespace ObjectQL.Data
{
    /// <summary>
    /// 数据库操作命令
    /// </summary>
    public interface IDataCommandBuilder
    {
        /// <summary>
        /// 命令参数集合
        /// </summary>
        IEnumerable<IDataParameter> Parameters { get; }

        /// <summary>
        /// 数据库文本命令文本
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// 数据库命令
        /// </summary>
        IDbCommand DbCommand { get; }

        /// <summary>
        /// 附加新的数据库条件和命令参数
        /// </summary>
        /// <param name="appendCommandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IDbCommand Append(string appendCommandText, params object[] args);
    }
}
