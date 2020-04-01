using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectQL.Data;
/*************************************************************************************
* CLR 版本：4.0.30319.42000
* 类 名 称：DataGatewayTests
* 命名空间：ObjectQL.DataExtTests.Data
* 文 件 名：DataGatewayTests
* 创建时间：2017/9/28 17:31:20
* 作    者：renzhsh
* 说    明：
* 修改时间：
* 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.DataExtTests.Models;
using ObjectQL.Linq;
using ObjectQL.Data.OracleClient;

namespace ObjectQL.DataExtTests
{
    [TestClass()]
    public class DataGatewayTests
    {

        private DataGateway gateway;

        public DataGatewayTests()
        {
            ObjectQLEngine.Startup();
            gateway = new DataGateway();
        }

        [TestMethod()]
        public void QueryDynamicFromTest()
        {
            var result = gateway.Query("select ID from SYS_USER").Get();
            Console.WriteLine((result.First() as ExpandoObject)?.GetAttribute("ID"));
            Console.WriteLine(result.First().ID);
            Assert.IsNotNull(result.First().ID);
            Assert.IsTrue(result.Any());
        }

        [TestMethod()]
        public void UpdateBatchTest()
        {
            var updateContent = new UpdateObject<User>();
            updateContent.Update(x => x.Sex, Sex.男);
            updateContent.Where = item => item.Id == "1705020000049134";
            gateway.UpdateBatch(new List<UpdateObject<User>>
            {
                updateContent
            });
            var entity = this.gateway.Where<User>(item => item.Id == "1705020000049134").Select().First();
            Assert.AreEqual(entity.Sex, Sex.男);
        }

        [TestMethod()]
        public void GetScalarTest()
        {
            var count = gateway.Query("select count(1) from SYS_USER ").GetScalar<int>();
            Console.WriteLine(count);
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void StoredProcedureTest()
        {
            //var ps = gateway.DefaultStoredProcedure("test",new object[] { "test", "张三" }).OutOrReturnParameters();
            //foreach(var item in ps)
            //{
            //    Console.WriteLine(item.Value);
            //}
            Assert.IsTrue(true);
        }
    }
}