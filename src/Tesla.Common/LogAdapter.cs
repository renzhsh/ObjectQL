﻿/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：Log
 * 命名空间：Jinhe
 * 文 件 名：Log
 * 创建时间：2016/11/21 10:28:46
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using Jinhe.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Jinhe
{
    /// <summary>
    /// 
    /// </summary>
    /// <summary>
    /// 日志(默认日志级别为Error.依次为:Debug, Warn, Error, Fatal, Info),其中Info始终写日志
    /// </summary>
    public class LogAdapter:ILogger
    {
        #region 原LogAdapter
        private static string basepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
        private static LogLevel level = LogLevel.Error;
        private static string timeformat = "yyyy-MM-dd HH:mm:ss:fff  ";
        private string type;
        private static Dictionary<string, LogAdapter> dictlog = new Dictionary<string, LogAdapter>();
        private static LogAdapter app = null;
        private static LogAdapter db = null;
        private string logFilePath = null;

        #region 基本属性

        /// <summary>
        /// 获取或设置日志文件的根目录
        /// </summary>
        public static string BasePath
        {
            get
            {
                return basepath;
            }
            set
            {
                basepath = value;
            }
        }

        /// <summary>
        /// 获取或设置日志记录的级别(默认为Error.依次为:Debug, Warn, Error, Fatal, Info)
        /// </summary>
        public static LogLevel Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        /// <summary>
        /// 能否记录调试信息
        /// </summary>
        public static bool CanDebug
        {
            get
            {
                return (int)LogLevel.Debug >= (int)LogAdapter.level;
            }
        }

        /// <summary>
        /// 能否记录错误信息
        /// </summary>
        public static bool CanError
        {
            get
            {
                return (int)LogLevel.Error >= (int)LogAdapter.level;
            }
        }

        /// <summary>
        /// 能否记录致命错误
        /// </summary>
        public static bool CanFatal
        {
            get
            {
                return (int)LogLevel.Fatal >= (int)LogAdapter.level;
            }
        }

        /// <summary>
        /// 能否记录警告信息
        /// </summary>
        public static bool CanWarn
        {
            get
            {
                return (int)LogLevel.Warn >= (int)LogAdapter.level;
            }
        }

        #endregion

        /// <summary>
        /// 系统默认日志实例
        /// </summary>
        public static LogAdapter App
        {
            get
            {

                if (app == null)
                {
                    db = GetLogger("app");
                }
                return app;
            }
        }

        /// <summary>
        /// 系统数据访问层日志实例
        /// </summary>
        public static LogAdapter Db
        {
            get
            {
                if(db==null)
                {
                    db = GetLogger("db");
                }
                return db;
            }
        }

        /// <summary>
        /// 设置日志输出流,可以设置为一个绝对路径或相对路径
        /// </summary>
        public string LogFilePath
        {
            set
            {
                if (Path.IsPathRooted(value))
                {
                    this.logFilePath = value;
                }
                else
                {
                    this.logFilePath = Path.Combine(LogAdapter.basepath, value);
                }
            }
        }

        #region 构造函数


        /// <summary>
        /// 获取LogAdapter类的一个实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>LogAdapter类的一个实例</returns>
        public static LogAdapter GetLogger(Type type)
        {
            return new LogAdapter(type.FullName);
        }

        /// <summary>
        /// 获取LogAdapter类的一个实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>LogAdapter类的一个实例</returns>
        public static LogAdapter GetLogger(string type)
        {
            LogAdapter log = null;
            string lowertype = type.ToLower();
            if (dictlog.ContainsKey(lowertype))
            {
                log = dictlog[lowertype];
            }
            else
            {
                log = new LogAdapter(type);
                dictlog.Add(lowertype, log);
            }
            return log;
        }
        /// <summary>
        /// 使用指定的名称初始化一个日志适配器
        /// </summary>
        /// <param name="type">日志名称</param>
        private LogAdapter(string type)
        {
            this.type = type;
        }

        #endregion

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="type">日志内容的类型(会按照类型生成子文件夹)</param>
        /// <param name="msg">日志内容</param>
        /// <param name="compact">是否使用压缩的格式来记录日志信息</param>
        private void Write(LogLevel level, LogFileSpan sequence, string type, object msg, bool compact)
        {
            if (level == LogLevel.Info || (int)level >= (int)LogAdapter.level) //可以写日志
            {
                #region 使用系统定义的文件
                string path = this.logFilePath;
                if (string.IsNullOrEmpty(path))
                {
                    switch (sequence)
                    {
                        case LogFileSpan.None:
                            {
                                path = string.Format("{1}{0}{2}.txt", Path.DirectorySeparatorChar, level, type);
                                break;
                            }
                        case LogFileSpan.Year:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.Year);
                                break;
                            }
                        case LogFileSpan.Month:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.ToString("yyyyMM"));
                                break;
                            }
                        case LogFileSpan.Week:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}{4}周.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.Year, DateUtils.GetWeekOfYear(DateTime.Now));
                                break;
                            }
                        case LogFileSpan.Hour:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.ToString("yyyyMMddHH"));
                                break;
                            }
                        default:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.ToString("yyyyMMdd"));
                                break;
                            }
                    }
                    path = Path.Combine(LogAdapter.basepath, path);
                }
                try
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                    {
                        if (compact)
                        {
                            sw.Write(DateTime.Now.ToString(timeformat));
                        }
                        else
                        {
                            sw.Write("┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄  ");
                            sw.Write(DateTime.Now.ToString(timeformat));
                            sw.WriteLine("┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄");
                        }
                        sw.WriteLine(msg);
                        if (!compact)
                        {
                            sw.WriteLine();
                        }
                        sw.Close();
                    }
                }
                catch
                {
                }
                #endregion
            }
        }

        #region 写入信息

        /// <summary>
        /// 写入信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Info(object msg)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, msg, true);
        }

        /// <summary>
        /// 写入信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Info(string format, params object[] msgs)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="msg">日志内容</param>
        public void Info(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Info, sequence, this.type, msg, false);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Info(LogFileSpan sequence, string format, params object[] msgs)
        {
            Write(LogLevel.Info, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        public void Info(bool compact, object msg)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Info(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="msg">日志内容</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        public void Info(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Info, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Info(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Info, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region 写入调试信息

        /// <summary>
        /// 写入调试信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Debug(object msg)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, msg, true);
        }

        /// <summary>
        /// 写入调试信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Debug(string format, params object[] msgs)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }


        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="msg">日志内容</param>
        public void Debug(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Debug, sequence, this.type, msg, true);
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Debug(LogFileSpan sequence, string format, params object[] msgs)
        {
            Write(LogLevel.Debug, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Debug(bool compact, object msg)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, msg, compact);

        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Debug(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);

        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Debug(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Debug, sequence, this.type, msg, compact);

        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Debug(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Debug, sequence, this.type, string.Format(format, msgs), compact);

        }
        #endregion

        #region 写入警告信息

        /// <summary>
        /// 写入警告信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Warn(object msg)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, msg, true);

        }

        /// <summary>
        /// 写入警告信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Warn(string format, params object[] msgs)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="msg">日志内容</param>
        public void Warn(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Warn, sequence, this.type, msg, false);
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Warn(LogFileSpan sequence, string format, params object[] msgs)
        {
            Write(LogLevel.Warn, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Warn(bool compact, object msg)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Warn(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Warn(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Warn, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// 写入警告信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Warn(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Warn, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region 写入错误信息

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="msg">错误信息</param>
        /// <param name="ex">异常</param>
        public void Error(string msg, Exception ex)
        {
            Error(string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, msg, ex.Message, ex.StackTrace));
        }

        /// <summary>
        /// 写入错误信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Error(object msg)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, msg, true);

        }

        /// <summary>
        /// 写入错误信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Error(string format, params object[] msgs)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="msg">日志内容</param>
        public void Error(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Error, sequence, this.type, msg, false);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Error(LogFileSpan sequence, string format, params object[] msgs)
        {
            Write(LogLevel.Error, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Error(bool compact, object msg)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Error(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Error(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Error, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Error(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Error, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region 写入致命信息

        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="msg">错误信息</param>
        /// <param name="ex">异常</param>
        public void Fatal(string msg, Exception ex)
        {
            Fatal(string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, msg, ex.Message, ex.StackTrace));
        }

        /// <summary>
        /// 写入致命错误信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Fatal(object msg)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, msg, true);
        }

        /// <summary>
        /// 写入致命错误信息(默认生成日志文件的频率为一天,在default文件夹下)
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Fatal(string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="msg">日志内容</param>
        public void Fatal(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Fatal, sequence, this.type, msg, false);
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Fatal(LogFileSpan sequence, string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Fatal(bool compact, object msg)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Fatal(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="msg">日志内容</param>
        public void Fatal(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Fatal, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// 写入致命错误信息
        /// </summary>
        /// <param name="sequence">日志文件生成频率</param>
        /// <param name="compact">是否使用紧凑格式(每行记录一条信息)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="msgs">日志内容</param>
        public void Fatal(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, sequence, this.type, string.Format(format, msgs), compact);
        }

        public void Warn(object message, Exception exception)
        {
            this.Warn(message);
        }

        public void Fatal(object message, Exception exception)
        {
            this.Fatal(message);
        }

        public void Error(object message, Exception exception)
        {
            this.Error(message);
        }

        public void Debug(object message, Exception exception)
        {
            this.Debug(message);
        }

        public void Info(object message, Exception exception)
        {
            this.Info(message);
        }
        #endregion

        #endregion

    }

    /// <summary>
    /// 对应于config文件的节点，用来描述日志的基本信息
    /// </summary>
    public sealed class LogSection : ConfigurationSection
    {
        /// <summary>
        /// 日志根目录的路径
        /// </summary>
        [ConfigurationProperty("path", IsRequired = false, DefaultValue = "log")]
        public string Path
        {
            get
            {
                return (string)base["path"];
            }
        }

        /// <summary>
        /// 日志根目录的级别
        /// </summary>
        [ConfigurationProperty("level", IsRequired = true, DefaultValue = LogLevel.Error)]
        public LogLevel Level
        {
            get
            {
                return (LogLevel)base["level"];
            }
        }
    }

    /// <summary>
    /// 日志文件生成的频率
    /// </summary>
    public enum LogFileSpan
    {
        /// <summary>
        /// 没有间隔,日志信息写入到一个文件中
        /// </summary>
        None,

        /// <summary>
        /// 按年生成文件
        /// </summary>
        Year,

        /// <summary>
        /// 按月生成文件
        /// </summary>
        Month,

        /// <summary>
        /// 按周生成文件
        /// </summary>
        Week,

        /// <summary>
        /// 按日生成文件
        /// </summary>
        Day,

        /// <summary>
        /// 小时
        /// </summary>
        Hour
    }

    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {

        /// <summary>
        /// 调试信息
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 警告信息
        /// </summary>
        Warn = 2,

        /// <summary>
        /// 错误信息
        /// </summary>
        Error = 3,

        /// <summary>
        /// 致命错误
        /// </summary>
        Fatal = 4,

        /// <summary>
        /// 普通信息(在任何情况下都会记录)
        /// </summary>
        Info = 5

    }
}
