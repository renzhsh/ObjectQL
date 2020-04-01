/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：GlobalConfig
 * 命名空间：Jinhe
 * 文 件 名：GlobalConfig
 * 创建时间：2017/11/23 18:17:19
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinhe
{
    /// <summary>
    /// 
    /// </summary>
    public class GlobalConfig
    {
        /// <summary>
        /// DI实现
        /// </summary>
        public static IDependencyResolver DependencyResolver { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public static Func<string> GetCurrentUserId { set; get; }
    }
}
