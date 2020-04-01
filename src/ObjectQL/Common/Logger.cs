using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace ObjectQL
{
    public class Logger
    {
        static Logger()
        {
            LoadFileAppender();
        }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ObjectQL");

        /// <summary>
        /// 使用文本记录异常日志
        /// </summary>
        /// <Author>Ryanding</Author>
        /// <date>2011-05-01</date>
        public static void LoadFileAppender()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            currentPath = Path.Combine(currentPath, @"Log/ObjectQL");

            string txtLogPath = Path.Combine(currentPath, $"{DateTime.Now.ToString("yyyyMMdd")}.log");

            log4net.Repository.Hierarchy.Hierarchy hier =
                (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();

            if (hier != null)
            {
                FileAppender fileAppender = new FileAppender();
                fileAppender.Name = "LogFileAppender";
                fileAppender.File = txtLogPath;
                fileAppender.AppendToFile = true;

                PatternLayout patternLayout = new PatternLayout
                {
                    ConversionPattern =
                        "%date %-5level %message%newline"
                };
                patternLayout.ActivateOptions();
                fileAppender.Layout = patternLayout;

                //选择UTF8编码，确保中文不乱码。
                fileAppender.Encoding = Encoding.UTF8;

                fileAppender.ActivateOptions();
                BasicConfigurator.Configure(fileAppender);
            }
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            log.Warn(message);
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception"></param>
        public static void Warn(string message, Exception exception)
        {
            log.Warn(message, exception);
        }

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(string message)
        {
            log.Fatal(message);
        }

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Fatal(string message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            log.Error(message);
        }

        public static void Error(Exception exception)
        {
            log.Error("", exception);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(string message, Exception exception)
        {
            log.Error(message, exception);
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            log.Debug(message);
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Debug(string message, Exception exception)
        {
            log.Debug(message, exception);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            log.Info(message);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(string message, Exception exception)
        {
            log.Info(message, exception);
        }
    }
}
