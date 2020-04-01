/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：ISerialize
 * 命名空间：Jinhe
 * 文 件 名：ISerialize
 * 创建时间：2016/11/21 9:31:00
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

namespace Jinhe
{
    /// <summary>
    /// 序列化接口
    /// </summary>
    public interface ISerialize
    {
        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <returns></returns>
        string JsonSerilize<T>(T obj);

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化的对象的类型</typeparam>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <returns>反序列化后的对象</returns>
        T JsonDeserilize<T>(string value);
    }
}
