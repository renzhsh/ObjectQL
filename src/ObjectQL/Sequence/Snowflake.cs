using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Sequences
{
    /// <summary>
    /// 高性能、分布式、整体有序的ID生成器(19位)，每毫秒(millisecond)内可产生4095个序列号
    /// https://segmentfault.com/a/1190000011282426#articleHeader5
    /// </summary>
    public class Snowflake
    {
        /*      1       41,时间戳      10,机器码        12,序列号
         *      0 | 0000 ... 0000 | 0000 .... 0000 | 0000 ... 0000
         *      
         *  1.最高位不用
         *  2.机器码，可以部署2^10=1024个节点。包括5位datacenterId和5位workerId
         *  3.序列号，用来记录同毫秒内产生的不同id。12位（bit）可以表示的最大正整数是2^12−1=4095，
         *   表示同一机器同一时间截（毫秒)内产生的4095个ID序号
         */

        /// <summary>
        /// 序列号字节数,12
        /// </summary>
        private const long sequenceBits = 12L;

        /// <summary>
        /// 机器Id字节数，5
        /// </summary>
        private const long machineIdBits = 5L;

        /// <summary>
        /// 数据Id字节数，5
        /// </summary>
        private const long datacenterIdBits = 5L;

        /// <summary>
        /// 机器Id数据左移位数,12
        /// </summary>
        private const long machineIdShift = sequenceBits;

        /// <summary>
        /// 数据Id数据左移位数,12+5 = 17
        /// </summary>
        private const long datacenterIdShift = sequenceBits + machineIdBits;

        /// <summary>
        /// 时间戳左移动位数,12+5+5 = 22
        /// </summary>
        private const long timestampShift = sequenceBits + machineIdBits + datacenterIdBits;

        private const long maxMachineId = -1L ^ (-1L << (int)machineIdBits); //最大机器ID，31
        private const long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);//最大数据ID，31

        //4095,一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private const long sequenceMask = -1L ^ (-1L << (int)sequenceBits);

        //起始时间戳，用于用当前时间戳减去这个时间戳，算出偏移量
        private readonly DateTime TimeZero = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private long machineId;//机器ID
        private long datacenterId = 0L;//数据ID
        private long sequence = 0L;//计数从零开始

        private long lastTimestamp = -1L;//最后时间戳

        private object syncRoot = new object();//加锁对象

        private static Snowflake snowflake;

        public static Snowflake Current
        {
            get
            {
                if (snowflake == null)
                {
                    snowflake = new Snowflake();
                }

                return snowflake;
            }
        }

        public Snowflake()
        {
            Snowflakes(0L, -1);
        }

        public Snowflake(long machineId)
        {
            Snowflakes(machineId, -1);
        }

        public Snowflake(long machineId, long datacenterId)
        {
            Snowflakes(machineId, datacenterId);
        }

        private void Snowflakes(long machineId, long datacenterId)
        {
            if (machineId >= 0)
            {
                if (machineId > maxMachineId)
                {
                    throw new Exception("机器码ID非法");
                }
                this.machineId = machineId;
            }
            if (datacenterId >= 0)
            {
                if (datacenterId > maxDatacenterId)
                {
                    throw new Exception("数据中心ID非法");
                }
                this.datacenterId = datacenterId;
            }
        }

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns>毫秒</returns>
        private long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - TimeZero).TotalMilliseconds;
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            if (timestamp <= lastTimestamp)
            {
                timestamp = GetTimestamp();
            }
            return timestamp;
        }

        /// <summary>
        /// 获取长整形的ID(19位)
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (syncRoot)
            {
                long timestamp = GetTimestamp();
                if (lastTimestamp == timestamp)
                { //同一微秒中生成ID
                    sequence = (sequence + 1) & sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (sequence == 0)
                    {
                        //一微秒内产生的ID计数已达上限，等待下一微妙
                        timestamp = GetNextTimestamp(lastTimestamp);
                    }
                }
                else
                {
                    //不同微秒生成ID
                    sequence = 0L;
                }
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }
                lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                long Id = (timestamp << (int)timestampShift)
                    | (datacenterId << (int)datacenterIdShift)
                    | (machineId << (int)machineIdShift)
                    | sequence;
                return Id;
            }
        }
    }

    /// <summary>
    /// 简化版Snowflake(9-16位)，每时间间隔(IntervalUnit)内可产生4095个序列号
    /// </summary>
    public class SingleSnowflake
    {
        /*      1     19,时间戳      12,序列号
         *      0 | 0000 ... 0000 | 0000 ... 0000
         *      
         *  1.最高位不用
         *  2.序列号，用来记录同间隔内产生的不同id。12位（bit）可以表示的最大正整数是2^12−1=4095，
         *   表示同一机器同一时间截（毫秒)内产生的4095个ID序号
         */



        /// <summary>
        /// 默认起始时间点2010-1-1，时间间隔为Hour
        /// </summary>
        public SingleSnowflake() { }

        /// <summary>
        /// 默认起始时间点2010-1-1
        /// </summary>
        /// <param name="unit"></param>
        public SingleSnowflake(IntervalUnit unit)
        {
            IntervalUnit = unit;
        }

        /// <summary>
        /// 默认时间间隔为Hour
        /// </summary>
        /// <param name="timeZero">起始时间点</param>
        public SingleSnowflake(DateTime timeZero, IntervalUnit unit = IntervalUnit.Hour)
        {
            if (new DateTime(1970, 1, 1) > timeZero)
            {
                throw new ArgumentOutOfRangeException("timeZero", "请指定1970-1-1之后的时间");
            }
            TimeZero = timeZero.ToUniversalTime();
            IntervalUnit = unit;
        }

        /// <summary>
        /// 序列号字节数,12
        /// </summary>
        private const long sequenceBits = 12L;

        /// <summary>
        /// 时间戳左移动位数,12
        /// </summary>
        private const long timestampShift = sequenceBits;

        //4095,一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private const long sequenceMask = -1L ^ (-1L << (int)sequenceBits);

        private long sequence = 0L;//计数从零开始

        private long lastTimestamp = -1L;//最后时间戳

        private object syncRoot = new object();//加锁对象

        /// <summary>
        /// 起始时间点
        /// </summary>
        public DateTime TimeZero { get; } = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 时间间隔
        /// </summary>
        public IntervalUnit IntervalUnit { get; } = IntervalUnit.Hour;

        #region "static"
        #region "static private"
        private static SingleSnowflake _msFlake;
        private static SingleSnowflake _sFlake;
        private static SingleSnowflake _mFlake;
        private static SingleSnowflake _hFlake;
        private static SingleSnowflake _dFlake;
        #endregion

        /// <summary>
        /// 毫秒级别的FlakeId
        /// </summary>
        /// <returns></returns>
        public static long NextMilliSecondFlakeId()
        {
            if (_msFlake == null)
            {
                _msFlake = new SingleSnowflake(IntervalUnit.MSecond);
            }
            return _msFlake.NextId();
        }

        /// <summary>
        /// Second级别的Flake
        /// </summary>

        public static long NextSecondFlakeId()
        {
            if (_sFlake == null)
            {
                _sFlake = new SingleSnowflake(IntervalUnit.Second);
            }
            return _sFlake.NextId();
        }

        /// <summary>
        /// Minute级别的Flake
        /// </summary>
        public static long NextMinuteFlakeId()
        {
            if (_mFlake == null)
            {
                _mFlake = new SingleSnowflake(IntervalUnit.Minute);
            }
            return _mFlake.NextId();

        }

        public static long NextHourFlakeId()
        {
            if (_hFlake == null) _hFlake = new SingleSnowflake(IntervalUnit.Hour);
            return _hFlake.NextId();
        }

        public static long NextDayFlakeId()
        {
            if (_dFlake == null) _dFlake = new SingleSnowflake(IntervalUnit.Day);
            return _dFlake.NextId();
        }
        #endregion

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns></returns>
        private long GetTimestamp()
        {
            var span = DateTime.UtcNow - TimeZero;
            long result = 0L;
            switch (IntervalUnit)
            {
                case IntervalUnit.MSecond:
                    result = (long)span.TotalMilliseconds;
                    break;
                case IntervalUnit.Second:
                    result = (long)span.TotalSeconds;
                    break;
                case IntervalUnit.Hour:
                    result = (long)span.TotalHours;
                    break;
                case IntervalUnit.Day:
                    result = (long)span.TotalDays;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            if (timestamp <= lastTimestamp)
            {
                timestamp = GetTimestamp();
            }
            return timestamp;
        }

        /// <summary>
        /// 获取长整形的ID(9-16位)
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (syncRoot)
            {
                long timestamp = GetTimestamp();
                if (lastTimestamp == timestamp)
                { //同一微秒中生成ID
                    sequence = (sequence + 1) & sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (sequence == 0)
                    {
                        //一微秒内产生的ID计数已达上限，等待下一微妙
                        timestamp = GetNextTimestamp(lastTimestamp);
                    }
                }
                else
                {
                    //不同微秒生成ID
                    sequence = 0;
                }
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }
                lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳

                return (timestamp << (int)timestampShift) | sequence;
            }
        }
    }

    /// <summary>
    /// 间隔单位
    /// </summary>
    public enum IntervalUnit
    {
        /// <summary>
        /// 毫秒级
        /// </summary>
        MSecond,
        /// <summary>
        /// 秒级
        /// </summary>
        Second,
        /// <summary>
        /// 分钟
        /// </summary>
        Minute,
        /// <summary>
        /// 小时
        /// </summary>
        Hour,
        /// <summary>
        /// 天
        /// </summary>
        Day
    }
}
