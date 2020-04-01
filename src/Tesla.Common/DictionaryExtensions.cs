/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DictionaryExtensions
 * 命名空间：Jinhe
 * 文 件 名：DictionaryExtensions
 * 创建时间：2017/5/9 15:44:10
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
    public static class DictionaryExtensions
    {
        public static TAnonymous ToAnonymousType<T1, TAnonymous>(this IDictionary<String, Object> dictionary, Func<T1, TAnonymous> getAnonymousType)
        {
            var parameters = getAnonymousType.Method.GetParameters();
            return getAnonymousType(
                (T1)dictionary[parameters[0].Name]);
        }

        public static TAnonymous ToAnonymousType<T1, T2, TAnonymous>(this IDictionary<String, Object> dictionary, Func<T1, T2, TAnonymous> getAnonymousType)
        {
            var parameters = getAnonymousType.Method.GetParameters();
            return getAnonymousType(
                (T1)dictionary[parameters[0].Name],
                (T2)dictionary[parameters[1].Name]);
        }

        public static TAnonymous ToAnonymousType<T1, T2, T3, TAnonymous>(this IDictionary<String, Object> dictionary, Func<T1, T2, T3, TAnonymous> getAnonymousType)
        {
            var parameters = getAnonymousType.Method.GetParameters();
            return getAnonymousType(
                (T1)dictionary[parameters[0].Name],
                (T2)dictionary[parameters[1].Name],
                (T3)dictionary[parameters[2].Name]);
        }

        public static TAnonymous ToAnonymousType<T1, T2, T3, T4, TAnonymous>(this IDictionary<String, Object> dictionary, Func<T1, T2, T3, T4, TAnonymous> getAnonymousType)
        {
            var parameters = getAnonymousType.Method.GetParameters();
            return getAnonymousType(
                (T1)dictionary[parameters[0].Name],
                (T2)dictionary[parameters[1].Name],
                (T3)dictionary[parameters[2].Name],
                (T4)dictionary[parameters[3].Name]);
        }
    }
}
