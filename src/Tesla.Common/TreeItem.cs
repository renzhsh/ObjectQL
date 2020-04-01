/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：TreeItem
 * 命名空间：Jinhe
 * 文 件 名：TreeItem
 * 创建时间：2017/2/10 15:09:11
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
    public class TreeItem<T>
    {
        public T Item { get; set; }
        public IEnumerable<TreeItem<T>> Children { get; set; }
    }
}
