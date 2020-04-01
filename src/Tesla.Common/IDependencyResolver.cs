/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DependencyResolver
 * 命名空间：Jinhe
 * 文 件 名：DependencyResolver
 * 创建时间：2017/11/23 17:59:24
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;

namespace Jinhe
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object Resolve(Type serviceType);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object ResolveKeyed(object serviceKey, Type serviceType);
    }
}
