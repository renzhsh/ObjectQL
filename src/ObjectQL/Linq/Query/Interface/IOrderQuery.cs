/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IOrderQuery
 * 命名空间：ObjectQL.Data
 * 文 件 名：IOrderQuery
 * 创建时间：2018/2/5 13:08:36
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
    /// <typeparam name="E"></typeparam>
    public interface IOrderQuery<E>: IAggregateQuery, IQueryExecutor<E>
          where E : class, new()
    {
        /// <summary>
        /// 按倒序
        /// </summary>
        /// <returns></returns>
        IOrderQuery<E> Desc();

        /// <summary>
        /// 指定需跳过的记录数
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        IOrderQuery<E> Skip(int skip);

        /// <summary>
        /// 指定获取记录数
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        IQueryExecutor<E> Take(int limit);
    }
}
