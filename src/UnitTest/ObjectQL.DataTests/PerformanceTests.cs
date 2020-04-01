/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：PerformanceTests
 * 命名空间：ObjectQL.DataExtTests
 * 文 件 名：PerformanceTests
 * 创建时间：2017/4/1 8:31:07
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using ObjectQL.Data;
using ObjectQL.DataExtTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Linq;
using ObjectQL.Data.OracleClient;

namespace ObjectQL.PerformanceTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class PerformanceTests
    {
        private DataGateway gateway;
        private int length = 1000;

        public PerformanceTests()
        {
            ObjectQLEngine.Startup();
            gateway = new DataGateway();
        }

        [TestMethod]
        public void PT0_INSERT()
        {
            this.DeleteAll();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            AddTestData(length);
            this.gateway.Complete();
            watch.Stop();
            Console.WriteLine($"添加{length}条记录，用时{watch.ElapsedMilliseconds / 1000}秒，平均用时{watch.ElapsedMilliseconds / length}");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PT1_BATCH_INSERT()
        {
            this.DeleteAll();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            AddBatchData(length, 1000);
            watch.Stop();
            Console.WriteLine($"添加{length}条记录，用时{watch.ElapsedMilliseconds / 1000}秒，平均用时{watch.ElapsedMilliseconds / length}");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PT0_DELETE()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            DeleteTestData(length);
            watch.Stop();
            Console.WriteLine($"删除{length}条记录，用时{watch.ElapsedMilliseconds / 1000}秒，平均用时{watch.ElapsedMilliseconds / length}毫秒");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PT2_Parallel()
        {
            if (!gateway.Where<User>(item => item.Id == "1234567890123456").Exists())
            {
                gateway.Insert(new User
                {
                    Id = "1234567890123456"
                });
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            Parallel.For(1, length, task =>
            {
                UpdateCriteria<User> update = new UpdateCriteria<User>(){
                    { x=> x.Sex, Sex.女 }
                    };
                gateway.Update(update, item => item.Id == "1234567890123456");
                gateway.Complete();
                var entity = gateway.Where<User>(item => item.Id == "1234567890123456").Select().First();
            });

            watch.Stop();
            Console.WriteLine($"并行执行{length}条记录，用时{watch.ElapsedMilliseconds / 1000}秒，平均用时{watch.ElapsedMilliseconds / length}毫秒");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PT3_SELECT_PAGE()
        {
            this.DeleteAll();
            this.AddTestData(length);
            Stopwatch watch = new Stopwatch();
            var skip = this.length / 10 * 9;
            var count = 100;
            watch.Start();
            var query = gateway.Where<LoginLog>();
            query.Join<User>(x => x.UserID, y => y.Id)
                .Load(x => x.User)
                .Skip(skip)
                .Take(count);
            var result = query
                .Select()
                .ToList();
            watch.Stop();
            Console.WriteLine($"获取{count}条记录，用时{watch.ElapsedMilliseconds}毫秒");
            Console.WriteLine(query.BuilderContext.CommandText);
            Assert.IsTrue(result.Count() == count && result[0].User != null && !string.IsNullOrEmpty(result[0].User.Id));
            this.DeleteTestData(length);
        }

        [TestMethod]
        public void PT3_SELECT_PAGE_VS_SQL()
        {
            this.DeleteAll();
            this.AddTestData(this.length);
            Stopwatch watch = new Stopwatch();
            var skip = this.length / 10 * 9;
            var length = 10;
            watch.Start();
            string sql = String.Format("select * from (SELECT * FROM (SELECT ROWNUM AS RN, AA.* FROM (SELECT * FROM SYS_LOGIN_LOG  Inner JOIN SYS_USER SYS_USER ON SYS_LOGIN_LOG.USER_ID=SYS_USER.ID   ) AA  ) WHERE RN>{0}) where RN<={1}", skip, skip + length);
            OracleDataAccess db = new OracleDataAccess(new ConnectionSettings()
            {
                ConnectionName = "ObjectQLConnectString",
                ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl)));User Id=objectql;Password=objectql;",
                ProviderName = "ORACLE"
            });
            var result = new List<LoginLog>();
            using (var reader = db.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    result.Add(new LoginLog()
                    {
                        LogID = reader["LOG_ID"].ToString(),
                        UserID = reader["USER_ID"].ToString(),
                        Content = reader["CONTENT"].ToString(),
                    });
                }
                reader.Close();
            }
            watch.Stop();
            Console.WriteLine($"获取{length}条记录，用时{watch.ElapsedMilliseconds}毫秒");
            Assert.IsTrue(result.Count() == length);
            this.DeleteAll();
        }

        protected void AddTestData(int length)
        {
            for (var i = 0; i < length; i++)
            {
                this.gateway.Insert<LoginLog>(new LoginLog()
                {
                    LogID = $"UL{i}",
                    UserID = "1705020000049134",
                    CreateDate = DateTime.Now,
                    Content = $"UL{i}登录"
                });
            }
        }

        protected void AddBatchData(int length, int pageSize = 1000)
        {
            var page = Math.Floor((decimal)length / pageSize);
            for (var i = 0; i < page; i++)
            {
                var pageList = new List<LoginLog>();
                for (var j = 0; j < pageSize; j++)
                {
                    var index = i * pageSize + j;
                    if (index > length - 1)
                    {
                        break;
                    }
                    pageList.Add(new LoginLog()
                    {
                        LogID = $"UL{index}",
                        UserID = "1234567890123456",
                        CreateDate = DateTime.Now,
                        Content = $"UL{i}登录"
                    });
                }
                this.gateway.InsertBatch(pageList);
            }
        }

        protected void DeleteTestData(int length)
        {
            for (var i = 0; i < length; i++)
            {
                var id = $"UL{i}";
                this.gateway.Delete<LoginLog>(x => x.LogID == id);
            }
        }

        protected void DeleteAll()
        {
            this.gateway.Delete<LoginLog>(item => true);
        }
    }
}
