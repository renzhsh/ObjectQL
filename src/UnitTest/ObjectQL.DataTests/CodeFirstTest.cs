using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectQL.DataExtTests.Models;
using System.Linq;

namespace ObjectQL.DataTests
{
    [TestClass]
    public class CodeFirstTest
    {
        [TestInitialize]
        public void Test_Initialize_Policy()
        {
            ObjectQLEngine.Startup();
        }

        [TestMethod]
        public void Test_Data_Access()
        {
            var gateway = new DataGateway();

            gateway.Delete<TestColumn>(item => item.FID == "123456");

            var Model = new TestColumn()
            {
                FID = "123456",
                FSByte = 24,
                FByte = 45,
                FInt = 1000,
                FFloat = 1.2345F,
                FDouble = 2.3456789D,
                FDecimal = 0.000000000012345M,
                FBoolean = true,
                FChar = '$',
                FChars = new char[] { 'A', 'B', 'C' },
                FDate = DateTime.Now,
                FBytes = new byte[] { 0, 1, 2, 3 },
                FEnum = TestEnum.Enum12
            };

            gateway.Insert(Model);

            var actual = gateway.Where<TestColumn>(item => item.FID == "123456").Select().FirstOrDefault();

            Assert.IsNotNull(actual);
            Assert.AreEqual(Model.FID, actual.FID, "FID");
            Assert.AreEqual(Model.FSByte, actual.FSByte, "FSByte");
            Assert.AreEqual(Model.FByte, actual.FByte, "FByte");
            Assert.AreEqual(Model.FInt, actual.FInt, "FInt");
            Assert.AreEqual(Model.FFloat, actual.FFloat, "FFloat");
            Assert.AreEqual(Model.FDouble, actual.FDouble, "FDouble");
            Assert.AreEqual(Model.FDecimal, actual.FDecimal, "FDecimal");
            Assert.AreEqual(Model.FBoolean, actual.FBoolean, "FBoolean");
            Assert.AreEqual(Model.FChar, actual.FChar, "FChar");
            Assert.AreEqual(Model.FDate.ToLongTimeString(), actual.FDate.ToLongTimeString(), "FDate");
            Assert.AreEqual(Model.FEnum, actual.FEnum, "FEnum");
        }
    }
}
