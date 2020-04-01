using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectQL.Data;
/*************************************************************************************
* CLR 版本：4.0.30319.42000
* 类 名 称：SequenceTests
* 命名空间：ObjectQL.DataExtTests.Data
* 文 件 名：SequenceTests
* 创建时间：2017/4/10 17:01:50
* 作    者：renzhsh
* 说    明：
* 修改时间：
* 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data.OracleClient;
using ObjectQL.Sequences;
using System.Diagnostics;
using System.Threading;
using ObjectQL.Linq;

namespace ObjectQL.DataExtTests
{
    [TestClass()]
    public class SequenceTests
    {
        private DataGateway gateway;
        public SequenceTests()
        {
            ObjectQLEngine.Startup();
            gateway = new DataGateway();
        }



        [TestMethod]
        public void Test_Snowflake()
        {
            StringBuilder sb = new StringBuilder();
            var flake = new Snowflake(12, 12);
            for (int i = 0; i < 100; i++)
            {
                if (i % 2 == 0) Thread.Sleep(1);
                var log = "开始执行 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff") + "    " + flake.NextId() + "\n";
                sb.AppendLine(log);
            }

            Debug.Write(sb.ToString());
        }

        [TestMethod]
        public void Test_SingleSnowflake()
        {
            StringBuilder sb = new StringBuilder();
            var flake = new SingleSnowflake();
            long last = -1;
            for (int i = 0; i < 100; i++)
            {
                long current = flake.NextId();
                var log = "开始执行 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "    " + current + "\n";
                sb.AppendLine(log);
                if (last == -1)
                {
                    last = current;
                }
                else
                {
                    Assert.IsTrue(current > last);
                    last = current;
                }
            }

            Debug.Write(sb.ToString());
        }

        [TestMethod]
        public void Test_Single_TimeZero()
        {
            SingleSnowflake previous = new SingleSnowflake(new DateTime(2018, 7, 1), IntervalUnit.MSecond);
            for (var i = 1; i < 48; i++)
            {
                var now = new DateTime(2018 - i, 7, 1);
                SingleSnowflake current = new SingleSnowflake(now, IntervalUnit.MSecond);

                long p1 = previous.NextId();
                long p2 = current.NextId();

                Debug.WriteLine($"{now.ToLongDateString()}: {p2} > {p1}");

                Assert.IsTrue(p2 > p1);

                previous = current;
            }
        }

        [TestMethod]
        public void Test_IncreaseSequence()
        {
            for (var i = 1; i < 20; i++)
            {
                var value = Sequence.Next(i);
                Debug.WriteLine($"{i}:{value}");
                Assert.AreEqual(value.ToString().Length, i, $"length={i}");
            }
        }

        [TestMethod]
        public void Test_Increase_Loop()
        {
            Increase seq = new Increase("test", 1, 3, 2, 31);
            for (var i = 1; i < 20; i++)
            {
                var value = seq.Next(4);
                Debug.WriteLine($"{i}:{value}");
            }
        }

        [TestMethod]
        public void Test_Custom_Format()
        {
            for (var i = 0; i < 10; i++)
            {
                var result = Sequence.Next("mas", 4, "MAS{1}{0}{2}", DateTime.Now.ToString("yyyyMMdd"), "abc");
                Debug.WriteLine(result);
            }
        }

        [TestMethod]
        public void Test_LuhmValue()
        {
            Debug.WriteLine($"=================NextMonthLuhmValue================");
            for (var i = 0; i < 10; i++)
            {
                Debug.WriteLine(Sequence.NextLuhmValue("default", 4, Sequence.MonthFormat));
            }
            Debug.WriteLine($"=================NextDayLuhmValue================");
            for (var i = 0; i < 10; i++)
            {
                Debug.WriteLine(Sequence.NextLuhmValue("default", 4, Sequence.DayFormat));
            }

            Debug.WriteLine($"=================NextHourLuhmValue================");
            for (var i = 0; i < 10; i++)
            {
                Debug.WriteLine(Sequence.NextLuhmValue("default", 4, Sequence.HourFormat));
            }
        }
    }
}