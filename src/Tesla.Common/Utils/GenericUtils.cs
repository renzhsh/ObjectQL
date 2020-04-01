/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：GenericUtils
 * 命名空间：Jinhe.Utils
 * 文 件 名：GenericUtils
 * 创建时间：2017/2/10 15:07:54
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinhe.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class GenericUtils
    {
        /// <summary>
        /// 将IEnumerable转成树
        /// </summary>
        /// <typeparam name="T">列表集合的类型</typeparam>
        /// <typeparam name="K">ParentId的类型</typeparam>
        /// <param name="collection">列表集合</param>
        /// <param name="idSelector">节点标示 x=> x.Id</param>
        /// <param name="parentIdSelector">父节点标示 x=> x.ParentId</param>
        /// <param name="rootId">根节点的值</param>
        /// <returns>树</returns>
        public static IEnumerable<TreeItem<T>> GenerateTree<T, K>(
          this IEnumerable<T> collection,
          Func<T, K> idSelector,
          Func<T, K> parentIdSelector,
          K rootId = default(K))
        {
            foreach (var c in collection.Where(x => parentIdSelector(x).Equals(rootId)))
            {
                yield return new TreeItem<T>
                {
                    Item = c,
                    Children = collection.GenerateTree(idSelector, parentIdSelector, idSelector(c))
                };
            }
        }
    }
}
