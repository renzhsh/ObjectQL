/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：OracleAccessX86Test
 * 命名空间：Jinhe.DataExtTests
 * 文 件 名：OracleAccessX86Test
 * 创建时间：2017/6/20 15:34:10
 * 作    者：hzk
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using Jinhe.Data;
using Jinhe.DataExtTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinhe.DataExtTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass()]
    public class OracleAccessX86Test
    {
        private readonly IDataGateway _gateway;
        public OracleAccessX86Test()
        {
            _gateway = DataGateway.Instance;
            _gateway.RegisterDataAccessType<Data.OracleClient.OracleDataAccess>().Provider("Oracle",
                new Data.OracleClient.OracleCommandBuildProvider());
            _gateway.EntityMapContainerInit();
        }

        [TestMethod()]
        public void OracleAccessX86Test_Insert()
        {
            //using (var scope = new System.Transactions.TransactionScope())
            //{
                var model = new BEventinfo
                {
                    Eventid = ObjectId.GenerateNewStringId(),
                    Publicreporter = "C",
                    Callbacknumber = "111",
                    Eventdesc = "微信反映/111",
                    Address = "安徽省马鞍山市雨山区安民街道太白大道2008-2号楼马鞍山市人民政府",
                    Basicgrid = "341204001",
                    Classcode = "020600",
                    Subclasscode = "020603",
                    Managelevel = "020000",
                    Callback = "1",
                    OpenID = "o8GMgw6guzByB_9Y_VPgpcpcwBLI"
                };

                _gateway.Insert(model);
            //    scope.Complete();
            //}
        }

        [TestMethod()]
        public void OracleAccessX86Test_Where()
        {
            var queryShr = _gateway.Where<UUsers>(item => item.Mapid == "341204001," && item.Deptid == 411)
                .Select();
            Assert.IsTrue(true);
        }
    }
}
