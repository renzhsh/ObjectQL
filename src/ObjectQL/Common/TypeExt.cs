/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：TypeExt
 * 命名空间：ObjectQL
 * 文 件 名：TypeExt
 * 创建时间：2017/4/13 8:45:31
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectQL
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIEnumerable(this Type type)
        {
            return (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        /// <summary>
        /// List泛型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsListGenericType(this Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(List<>);
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetIEnumerableImpl(this Type type)
        {
            if (IsIEnumerable(type))
                return type;
            //
            Type[] t = type.FindInterfaces((m, o) => IsIEnumerable(m), null);
            return t[0];
        }
    }
}
