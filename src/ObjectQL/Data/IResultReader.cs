using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectQL.Data
{
    /// <summary>
    /// 数据读取器
    /// </summary>
    public interface IResultReader : IDisposable
    {
        /// <summary>
        /// 指示数据读取器的游标向前移动指定长度
        /// </summary>
        /// <param name="skip">游标移动长度</param>
        void Skip(int skip);

        /// <summary>
        /// 执行记取命令
        /// </summary>
        /// <returns>返回一个值,指示DataReader是否成功读取了一条数据</returns>
        bool Read();

        /// <summary>
        /// 关闭读取器
        /// </summary>
        void Close();

        /// <summary>
        /// 行索引值(默认为-1,当读取了第一行行,变为0)
        /// </summary>
        int CurrentRowIndex { get; }

        /// <summary>
        /// 获取具有指定名称的列。
        /// </summary>
        /// <param name="name">要查找的列的名称。</param>
        /// <returns>名称指定为 Object 的列。</returns>
        object this[string name] { get; }

        /// <summary>
        /// 获取位于指定索引处的列。
        /// </summary>
        /// <param name="index">要获取的列的从零开始的索引</param>
        /// <returns>作为 Object 位于指定索引处的列</returns>
        object this[int index] { get; }

        /// <summary>
        /// 检查数据读取器是否包含某个指定的字段
        /// </summary>
        /// <param name="filedName">要检查的字段的名称</param>
        /// <returns>返回一个值,指示是否包含这个字段</returns>
        bool HasField(string filedName);

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> Fields { get; }

        /// <summary>
        /// 字段数
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetName(int index);
    }
}
