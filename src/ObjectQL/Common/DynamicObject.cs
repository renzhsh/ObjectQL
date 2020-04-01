/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DynamicObject
 * 命名空间：ObjectQL
 * 文 件 名：DynamicObject
 * 创建时间：2017/11/28 13:48:31
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpandoObjectExt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expando"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static object GetUpperAttribute(this ExpandoObject expando, string attributeName)
        {
            return expando.GetAttribute(attributeName?.ToUpper());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expando"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static object GetAttribute(this ExpandoObject expando, string attributeName)
        {
            if (expando == null)
                return null;
            return string.IsNullOrEmpty(attributeName) ? null : (expando as IDictionary<string, object>)[attributeName];
        }

        /// <summary>
        /// 返回属性名称
        /// </summary>
        public static IEnumerable<string> GetAttributes(this ExpandoObject expando)
        {
            return (expando as IDictionary<string, object>)?.Keys;
        }
    }
}
