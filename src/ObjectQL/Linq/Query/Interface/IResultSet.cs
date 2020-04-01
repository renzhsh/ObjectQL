/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IResultSet
 * 命名空间：ObjectQL.Data
 * 文 件 名：IResultSet
 * 创建时间：2018/3/28 15:05:38
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResultSet<T> : IEnumerable<T>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Total { get; }

    }
}
