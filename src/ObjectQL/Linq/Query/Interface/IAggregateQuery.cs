/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IAggregateQuery
 * 命名空间：ObjectQL
 * 文 件 名：IAggregateQuery
 * 创建时间：2018/2/11 8:54:46
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

namespace ObjectQL
{
    /// <summary>
    /// 聚合函数
    /// </summary>
    public interface IAggregateQuery
    {
        /// <summary>
        /// 统计记录数
        /// </summary>
        /// <returns></returns>
        int Count();

    }
}
