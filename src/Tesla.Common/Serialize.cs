/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：Serialize
 * 命名空间：Jinhe
 * 文 件 名：Serialize
 * 创建时间：2016/11/21 9:40:06
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinhe
{
    /// <summary>
    /// 
    /// </summary>
    public class Serialize
    {
        #region JSON序列化与反序列化

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param> 
        public static string JsonSerilize(object o)
        {   
            String result = Newtonsoft.Json.JsonConvert.SerializeObject(o);           
            return result;
        } 

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="converters"></param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, converters);
        }

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="formatting">格式化样式,缩进或不缩进</param>
        /// <param name="settings"></param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o, Newtonsoft.Json.Formatting formatting, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, formatting, settings);
        }

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="formatting">格式化样式,缩进或不缩进</param>
        /// <param name="converters"></param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o, Newtonsoft.Json.Formatting formatting, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, formatting, converters);
        }
        
        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化的对象的类型</typeparam>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="settings"></param>
        /// <returns>反序列化后的对象</returns>
        public static T JsonDeserilize<T>(string value, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, settings);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化的对象的类型</typeparam>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="converters"></param>
        /// <returns>反序列化后的对象</returns>
        public static T JsonDeserilize<T>(string value, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, converters);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="type">要反序列化的对象的类型</param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="type">要反序列化的对象的类型</param>
        /// <param name="settings"></param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value, Type type, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, settings);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="type">要反序列化的对象的类型</param>
        /// <param name="converters"></param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value, Type type, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, converters);
        }

     
        #endregion
    }
}
