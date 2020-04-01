/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ConnOwnerShip
 * 命名空间：ObjectQL.Data
 * 文 件 名：ConnOwnerShip
 * 创建时间：2016/10/19 13:16:04
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

namespace ObjectQL.Data
{
    /// <summary>
    /// 指示要执行的数据库命令的数据库连接对象是来自于外部还是内部生成的
    /// </summary>
    public enum ConnOwnerShip
    {
        /// <summary>
        /// 当DataReader关闭时,自动关闭数据连接对象
        /// </summary>
        Internal,

        /// <summary>
        /// 当DataReader关闭时,不关闭数据连接对象
        /// </summary>
        External
    }
}
