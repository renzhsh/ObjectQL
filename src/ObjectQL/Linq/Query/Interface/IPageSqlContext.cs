/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IPageSqlContext
 * 命名空间：ObjectQL.Data
 * 文 件 名：IPageSqlContext
 * 创建时间：2018/3/28 14:50:33
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
    /// 分页上下文
    /// </summary>
    public interface IPageSqlContext : ISqlContext
    {
        /// <summary>
        /// 获取记录条数
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        IPageSqlContext Take(int take);
    }
}
