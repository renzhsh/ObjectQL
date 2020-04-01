/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：ILog
 * 命名空间：Jinhe
 * 文 件 名：ILog
 * 创建时间：2016/11/2 15:38:22
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/


using System;

namespace Jinhe
{
    /// <summary>
    /// 日志(默认日志级别为Error.依次为:Debug, Warn, Error, Fatal, Info),其中Info始终写日志
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="message">日志内容</param>
        void Warn(object message);

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception"></param>
        void Warn(object message, Exception exception);

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="message"></param>
        void Fatal(object message);

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(object message, Exception exception);

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="message"></param>
        void Error(object message);
        
        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(object message, Exception exception);

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="message"></param>
        void Debug(object message);

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Debug(object message, Exception exception);

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="message"></param>
        void Info(object message);

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Info(object message, Exception exception);
    }
}
