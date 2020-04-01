using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.Sequences;

namespace ObjectQL
{
    /// <summary>
    /// 序列
    /// </summary>
    public static class Sequence
    {
        #region "递增序列"
        private static readonly Dictionary<string, Increase> dict = new Dictionary<string, Increase>();

        /// <summary>
        /// 递增序列
        /// </summary>
        /// <returns></returns>
        public static long Next()
        {
            return Next("default");
        }

        /// <summary>
        /// 递增序列
        /// </summary>
        /// <param name="name">业务名称</param>
        /// <returns></returns>
        public static long Next(string name)
        {
            return Next(name, 0);
        }

        /// <summary>
        /// 递增序列
        /// </summary>
        /// <param name="length">序列长度</param>
        /// <returns></returns>
        public static long Next(int length)
        {
            return Next("default", length);
        }

        /// <summary>
        /// 递增序列
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long Next(string name, int length)
        {
            var key = name.ToLower();
            Increase target = null;

            if (dict.ContainsKey(key))
            {
                target = new Increase(key);
            }
            else
            {
                lock (nextObj)
                {
                    if (!dict.ContainsKey(key))
                    {
                        target = new Increase(key);
                        dict.Add(key, target);
                    }
                }
            }

            return target.Next(length);
        }
        private static readonly object nextObj = new object();

        public static string Next(string name, int length, string format = "{0}", params object[] args)
        {
            var seq = Next(name, length);

            List<object> _args = new List<object>();
            _args.Add(seq);
            _args.AddRange(args);

            return string.Format(format, _args.ToArray());
        }

        public static long CurrentValue(string name = "default")
        {
            var key = name.ToLower();
            if (dict.ContainsKey(key))
            {
                var target = dict[key];

                return target.CurrentValue;
            }

            return 0L;
        }

        #endregion

        #region "Flake序列"

        /// <summary>
        /// 不依赖数据库、整体保持有序的序列
        /// </summary>
        /// <param name="unit">时间间隔，在同一间隔内可产生4095个不重复的序列</param>
        /// <returns></returns>
        public static long NextFlakeValue(IntervalUnit unit = IntervalUnit.Minute)
        {
            long result = 0L;
            switch (unit)
            {
                case IntervalUnit.MSecond:
                    result = SingleSnowflake.NextMilliSecondFlakeId();
                    break;
                case IntervalUnit.Second:
                    result = SingleSnowflake.NextSecondFlakeId();
                    break;
                case IntervalUnit.Minute:
                    result = SingleSnowflake.NextMinuteFlakeId();
                    break;
                case IntervalUnit.Hour:
                    result = SingleSnowflake.NextHourFlakeId();
                    break;
                case IntervalUnit.Day:
                    result = SingleSnowflake.NextDayFlakeId();
                    break;
            }

            return result;
        }
        #endregion

        public const string DayFormat = "yyyyMMdd";
        public const string ShortDayFormat = "yyMMdd";
        public const string MonthFormat = "yyyyMM";
        public const string HourFormat = "yyyyMMddHH";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">业务名称</param>
        /// <param name="length">序列长度</param>
        /// <param name="dateFormat">时间格式</param>
        /// <returns></returns>
        public static long NextLuhmValue(string name = "default", int length = 4, string dateFormat = "yyyyMMdd")
        {
            long seq, time, result;
            seq = Next(name, length);
            try
            {
                time = long.Parse(DateTime.Now.ToString(dateFormat));
            }
            catch (Exception ex)
            {
                throw new Exception($"无效的DateTime格式：{dateFormat}", ex);
            }

            result = time * (long)Math.Pow(10, length) + seq;

            result = result * 10 + Luhm.Resolve(result);

            return result;
        }
    }
}
