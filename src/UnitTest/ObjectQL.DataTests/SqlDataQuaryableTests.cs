/*************************************************************************************
* CLR 版本：4.0.30319.42000
* 类 名 称：SqlDataQuaryableTests
* 命名空间：ObjectQL.DataExtTests1.Data
* 文 件 名：SqlDataQuaryableTests
* 创建时间：2017/3/21 15:06:08
* 作    者：renzhsh
* 说    明：
* 修改时间：
* 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectQL.DataExtTests.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectQL.Data;
using ObjectQL.Linq;
using ObjectQL.Data.OracleClient;

namespace ObjectQL.DataExtTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass()]
    public class SqlDataQuaryableTests
    {
        private DataGateway gateway;

        public SqlDataQuaryableTests()
        {
            ObjectQLEngine.Startup();
            gateway = new DataGateway();
        }

        [TestMethod()]
        public void T0()
        {
            var result = gateway.Where<User>().Select();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void T1()
        {
            gateway.Where<User>(x => x.Id == "12345678").Exists();
        }


        [TestMethod()]
        public void T1_Exists()
        {
            bool ret = true;
            IDataQueryable<User> query = null;
            ret = gateway.Where<User>().Exists();
            Assert.IsTrue(ret, "step 1");
            ret = gateway.Where<User>(x => 1 == 1).Exists();
            Assert.IsTrue(ret, "step 2");
            ret = !gateway.Where<User>(x => 1 != 1).Exists();
            Assert.IsTrue(ret, "step 3");
            gateway.Delete<User>(x => x.Id == "12345678");
            gateway.Insert(new User()
            {
                Id = "12345678",
                UserName = "test123",
                Password = "testPassword"
            });
            ret = gateway.Where<User>(x => x.Id == "12345678" && x.SecurityStamp == null)
                .Exists();
            Assert.IsTrue(ret, "step 4");
            string checkNull = null;
            query = gateway.Where<User>(x => x.SecurityStamp == checkNull
                           );
            ret = query.Exists();
            Assert.IsTrue(ret, "step 5");

            query = gateway.Where<User>(x => x.Id == "12345678");
            ret = query.Exists();
            Assert.IsTrue(ret, "step 6");

            query = gateway.Where<User>(x => x.Id == "12345678" && x.UserName.StartsWith("test12"));
            ret = query.Exists();
            Assert.IsTrue(ret, "step 7");

            query = gateway.Where<User>(x => x.Id == "12345678" || x.UserName.Contains("test12"));
            ret = query.Exists();
            Assert.IsTrue(ret, "step 8");

            query = gateway.Where<User>(x => x.Id == "12345678" || x.UserName.EndsWith("123"));
            ret = query.Exists();
            Assert.IsTrue(ret, "step 9");

            query = gateway.Where<User>(x => x.UserName.EndsWith("123"));
            ret = query.Exists();
            Assert.IsTrue(ret, "step 10");

            query = gateway.Where<User>(x => x.UserName.StartsWith("test12"));
            ret = query.Exists();
            Assert.IsTrue(ret, "step 11");

            query = gateway.Where<User>(x => x.UserName.Contains("test12"));
            ret = query.Exists();
            Assert.IsTrue(ret, "step 12");

            string[] userNames = new string[] { "test123" };
            query = gateway.Where<User>(x => userNames.Contains(x.UserName));

            var dateTimes = new List<DateTime>() { DateTime.Now }.ToArray();
            query = gateway.Where<User>(x => dateTimes.Contains(x.CreateTime));
            ret = !query.Exists();
            Assert.IsTrue(ret, "step 13");
        }

        [TestMethod()]
        public void T2_APPEND_OR_WHERE()
        {
            bool ret = true;
            gateway.Insert(new LoginLog()
            {
                LogID = $"1",
                UserID = "1234567890123456",
                CreateDate = DateTime.Now,
                Content = $"U登录"
            });

            var query = gateway.Where<LoginLog>()
                         .AppendAndWhere(item => item.UserID == "1234567890123456");

            query = query.AppendOrWhere(item => item.LogID == "1");

            Console.WriteLine(query.BuilderContext.CommandText);
            ret = query.Select(i => i.UserID).Any();

            query.AppendAndWhere(item => item.UserID == "1234567890123456");
            Console.WriteLine(query.BuilderContext.CommandText);
            ret = query.Select(i => i.UserID).Any();

            // query.AppendOrWhere(item => item.Token == "1234567890123456" && item.UserID == "1234567890123456"); 
            Console.WriteLine(query.BuilderContext.CommandText);
            ret = query.Select().Any();

            gateway.Delete<LoginLog>(x => x.UserID == "1234567890123456");
            Assert.IsTrue(ret);
        }

        [TestMethod()]
        public void T2_Load()
        {
            #region "Insert Test Data"
            if (!gateway.Where<Organ>(item => item.OrganId == "11111").Exists())
            {
                gateway.Insert<Organ>(new Organ
                {
                    OrganId = "11111",
                    OrganName = "SD"
                });
            }

            if (!gateway.Where<Organ>(item => item.OrganId == "22222").Exists())
            {
                gateway.Insert<Organ>(new Organ
                {
                    OrganId = "22222",
                    ParentId = "11111",
                    OrganName = "JH"
                });
            }

            #endregion

            // bug 

            // var organQuery = gateway.Where<Organ>()
            //.Join<Organ>(x => x.ParentId, y => y.OrganId, JoinType.Left)
            //.Load(x => x.Parent)
            //.Join<Organ>(x => x.OrganId, y => y.ParentId, JoinType.Left)
            //.Load(x => x.Child);

            // var organResult = organQuery.Select();
            // Assert.IsTrue(organResult.Where(item => item.Parent != null).Any(), "step 1");
            // Assert.IsTrue(organResult.Where(item => item.Child != null).Any(), "step 2");

            gateway.Delete<Organ>(o => o.OrganId == "11111");
            gateway.Delete<Organ>(o => o.OrganId == "22222");
        }

        /// <summary>
        /// 多表关联测试
        /// </summary>
        [TestMethod()]
        public void T2_AppendRelationTest()
        {
            this.AddTestData();

            #region 测试代码
            var query = gateway.Where<User>()
                               //.AppendAndWhere(item => item.Id == "U1")
                               //.AppendAndWhere(item => item.UserName == "U1_NAME")
                               ;
            query.OrderBy(i => i.CreateTime).Skip(0).Take(1);
            var result = query.Select().ToList();
            Assert.IsTrue(result.Count() == 1);
            var total = query.Count();
            Trace.WriteLine(query.BuilderContext.CountCommandText);
            Assert.IsTrue(total > result.Count());
            query.Join<UserRole>(x => x.Id, y => y.UserId)
                //.Join<UserRole>(x => x.Id, y => y.UserId)
                .JoinNext<Role>(x => x.RoleId, y => y.RoleID)
                .Load(x => x.Roles)
                .Join<UserOrgan>(x => x.Id, y => y.UserId, JoinType.Left)
                ////.Join<UserRole>((x, y) => x.Id == y.UserId) 
                .JoinNext<Organ>(x => x.OrganId, y => y.OrganId)

                //.JoinOnAndWhere(item => item.OrganName.Contains("R1_"))
                .Join<UserGroup>(x => x.Id, y => y.UserID, JoinType.Left)
                .JoinNext<Group>(x => x.GroupID, y => y.GroupID)
                .OrderBy(x => x.Email)
                ;


            gateway.Where<UserRole>().OrderBy(x => x.Id).Desc()
                                           .Skip(1).Take(2).Select().FirstOrDefault();

            result = query.Select().ToList();
            //query.SelectEntity<Organ>(item => new Organ()
            //{
            //    OrganId = item.Email
            //});
            Console.WriteLine(query.BuilderContext.CommandText);
            //Assert.IsNull(result[0].Email);
            Assert.IsNotNull(result[0].Id);

            var s = query.SelectToModel<dynamic>(item => new { id = item.Id, name = string.Join(",", item.Roles.Select(x => x.RoleID)) })
                .ToList();
            query.AppendAndWhere(item => item.Id == "U1")
                 .AppendAndWhere(item => item.UserName == "U1_NAME");
            //var result1 = gateway.Where<User>(t => t.Id == "U1")
            //     .Join<UserRole>(x => x.Id, y => y.UserId)
            //     .JoinNext<Role>(x => x.RoleId, y => y.RoleID)
            //     .AppendAndWhere(x => x.RoleID == "111")
            //     .Load(t => t.Roles)
            //     .Select();

            query.Select();


            //var exists = query1.Exists();



            //if (organResult.Where(item => !string.IsNullOrEmpty(item.ParentId)).Any())
            //{
            //    Assert.IsTrue(organResult.First().ParentId == organResult.First().Parent.OrganId);
            //}
            //Assert.IsTrue(organResult.Any());
            #endregion

            //this.DeleteTestData();

            // 断言

            //ret = ret && total > 0 && exists; 
        }

        /// <summary>
        /// 测试新增、删除和查询、修改
        /// </summary>
        [TestMethod()]
        public void T3_InsertAndSelectAndDelete()
        {
            gateway.Delete<User>(x => x.Id == "U1");
            gateway.Insert(new User()
            {
                Id = "U1",
                UserName = "U1_USERNAME",
                Password = "U1_PASSWORD"
            });
            var model = gateway.Where<User>(y => y.Id == "U1").Select().FirstOrDefault();
            var ret = model != null && model.Id == "U1";
            model = gateway.Where<User>(y => y.Id == "U1").Select(x => x.Id).FirstOrDefault();
            ret = ret && model != null && model.Id == "U1" && model.Name == null;
            gateway.Delete<User>(x => x.Id == "U1");
            var delRet = gateway.Where<User>(y => y.Id == "U1").Select()?.FirstOrDefault();
            ret = ret && delRet == null;
            Assert.IsTrue(ret);
        }

        [TestMethod()]
        public void T4_ExistsOther()
        {
            this.AddTestData();
            bool ret = true;
            var query = this.gateway.Where<User>()
                 .Exists<UserRole>(x => x.Id, y => y.UserId, y => y.RoleId == "R1");
            Trace.WriteLine(query.BuilderContext.CommandText);

            var result = query.Select();
            ret = result.FirstOrDefault() != null;
            Assert.IsTrue(ret);

            //Trace.WriteLine(query.BuilderContext.ExistsCommandText);
            ret = query.Exists();
            Assert.IsTrue(ret);
            query = this.gateway.Where<User>()
                 .NotExists<UserRole>(x => x.Id, y => y.UserId, y => y.RoleId == "R1");

            ret = query.Exists();
            Assert.IsTrue(ret);
            this.DeleteTestData();
        }

        [TestMethod()]
        public void T5_OrderBy()
        {
            // this.AddTestData();
            var query = this.gateway.Where<User>()
                             .Exists<UserRole>(x => x.Id, y => y.UserId, y => y.RoleId == "R1")
                             .Join<UserRole>(x => x.Id, y => y.UserId, JoinType.Left)
                             .Load(x => x.UserRoles)
                             .OrderBy(item => new { item.Id, item.CreateTime })
                           .Desc();

            var result = query.Select();
            //Console.WriteLine(sql);
            //Console.WriteLine(query.BuilderContext.CommandText);
            //var result = query.Select();
            //query = this.gateway.Where<User>()
            //               .Exists<UserRole>(x => x.Id, y => y.UserId, y => y.RoleId == "R1")
            //               .OrderBy(item => new { item.Id, item.CreateTime });
            //Console.WriteLine(query.BuilderContext.CommandText);
            //query.Desc();
            //Console.WriteLine(query.BuilderContext.CommandText);
            //query.Skip(0).Take(1);
            //Console.WriteLine(query.BuilderContext.CommandText);

            //result = query.Select();
            //this.DeleteTestData();
            //var ret = result?.FirstOrDefault() != null;
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void T6_SkipAndTake()
        {
            //this.AddTestData();
            var query = this.gateway.Where<User>();
            //query.AppendAndWhere(x => x.Password == null);
            //query.AppendAndWhere(x => x.Id.Contains("1"));
            var s = new { Sex = Sex.男 };
            // 测试枚举
            query.AppendAndWhere(x => x.Sex == s.Sex);
            query.OrderBy(item => item.Id).Skip(0).Take(10);
            //query.OrderBy(x =>new { x.Email , x.Id});
            Trace.WriteLine(query.BuilderContext.CommandText);
            var result = query.Select();
            result = this.gateway.Where<User>()
                .Exists<UserRole>(x => x.Id, y => y.UserId)
                .OrderBy(item => item.Id).Skip(0).Take(10).Select();

            int total = 0;
            var myRev = this.gateway.Where<User>()
               .Join<UserRole>(x => x.Id, y => y.UserId)
               .Skip(0)
               .Take(10)
               .OrderBy(x => new { x.Email, x.Id });

            result = myRev.Select(item => new { item.Id });


            this.DeleteTestData();
            var ret = result != null && result.FirstOrDefault() != null;
            Assert.IsTrue(ret);
        }

        [TestMethod()]
        public void T7_QueryBySql()
        {
            var query = this.gateway.Query("select * from sys_user");
            var result = query.Get<User>();
            var list = query.Skip(0).Take(2).Get<User>();
            Assert.IsTrue(list.Count() == 2);
            var ret = result != null && result.FirstOrDefault() != null && !string.IsNullOrEmpty(result.First().Id);
            Assert.IsTrue(ret);
        }

        [TestMethod()]
        public void T8_TEST_CMD()
        {
            var query = this.gateway.Query("select * from (SELECT * FROM (SELECT ROWNUM AS RN, AA.* FROM (SELECT SYS_USER.ID as SYS_USERID,SYS_USER.NAME as SYS_USERNAME,SYS_USER.EMAIL as SYS_USEREMAIL,SYS_USER.PASSWORD as SYS_USERPASSWORD,SYS_USER.STATUS as SYS_USERSTATUS,SYS_USER.SEX as SYS_USERSEX,SYS_USER.USER_NAME as SYS_USERUSER_NAME,SYS_USER.TYPE as SYS_USERTYPE,SYS_USER.TWO_FACTOR_ENABLED as SYS_USERTWO_FACTOR_ENABLED,SYS_USER.LOCKOUT_ENABLED as SYS_USERLOCKOUT_ENABLED,SYS_USER.SECURITY_STAMP as SYS_USERSECURITY_STAMP,SYS_USER.CREATE_DATE as SYS_USERCREATE_DATE,SYS_USER.PHONE_NUMBER as SYS_USERPHONE_NUMBER,SYS_USER.ACCESS_FAILED_COUNT as SYS_USERACCESS_FAILED_COUNT,SYS_USER.EMAIL_CONFIRMED as SYS_USEREMAIL_CONFIRMED,SYS_USER.PHONE_NUMBER_CONFIRMED as SYS_USERPHONE_NUMBER_CONFIRMED,SYS_USER.LOCKOUT_END_DATE_UTC as SYS_USERLOCKOUT_END_DATE_UTC FROM SYS_USER   WHERE   ((SYS_USER.NAME LIKE '%'||{0}||'%') OR (SYS_USER.USER_NAME LIKE '%'||{1}||'%')) AND  EXISTS (SELECT SYS_USER_ROLE.USER_ID FROM  SYS_USER_ROLE SYS_USER_ROLE WHERE SYS_USER.ID=SYS_USER_ROLE.USER_ID   AND ((SYS_USER_ROLE.ROLE_ID IN  ( {2} ) ))) ) AA  ) WHERE RN>0) where RN<=10", "1", "1", "1111111111111111");
            query.Get<User>();
            // Assert.IsTrue(query.Count() > 0);
        }

        [TestMethod()]
        public void T9_TEST_SELECT_LIST_CONTAINS()
        {
            gateway.Insert(new Role
            {
                RoleID = "1111111111111111"
            });

            var userRoleIds1 = new string[] { "1111111111111111" };
            var query = this.gateway.Where<Role>(u => userRoleIds1.Contains(u.RoleID));
            Console.WriteLine(query.BuilderContext.CountCommandText);
            var num = query.Count();
            Console.WriteLine(num);
            Assert.IsTrue(num > 0, "step 1");
            List<string> userRoleIds = new List<string>() {
                "1111111111111111"
            };
            query = this.gateway.Where<Role>(u => userRoleIds.Contains(u.RoleID));
            Console.WriteLine(query.BuilderContext.CountCommandText);
            Assert.IsTrue(query.Count() > 0, "step 2");

            gateway.Delete<Role>(r => r.RoleID == "1111111111111111");
        }

        [TestMethod()]
        public void T9_TEST_UPDATE_ENUM()
        {
            if (!gateway.Where<User>(item => item.Id == "1234567890123456").Exists())
            {
                gateway.Insert(new User()
                {
                    Id = "1234567890123456",
                    Sex = Sex.男
                });
            }

            UpdateCriteria<User> update = new UpdateCriteria<User>(){
                { x=> x.Sex, Sex.女 }
            };
            this.gateway.Update(update, item => item.Id == "1234567890123456");
            this.gateway.Complete();
            var entity = this.gateway.Where<User>(item => item.Id == "1234567890123456").Select().First();
            Assert.AreEqual(entity.Sex, Sex.女);

            gateway.Delete<User>(item => item.Id == "1234567890123456");
        }

        [TestMethod()]
        public void T9_TEST_UPDATE_Parallel()
        {
            if (!gateway.Where<User>(item => item.Id == "1234567890123456").Exists())
            {
                gateway.Insert(new User()
                {
                    Id = "1234567890123456",
                    Sex = Sex.男
                });
            }


            Parallel.For(1, 10, task =>
            {
                UpdateCriteria<User> update = new UpdateCriteria<User>(){
                    { x=> x.Sex, Sex.女 }
                };
                this.gateway.Update(update, item => item.Id == "1234567890123456");
                this.gateway.Complete();
                var entity = this.gateway.Where<User>(item => item.Id == "1234567890123456").Select().First();
                Assert.AreEqual(entity.Sex, Sex.女);
            });

            Parallel.For(1, 10, task =>
            {
                UpdateCriteria<User> update = new UpdateCriteria<User>(){
                    { x=> x.Sex, Sex.男 }
                };
                this.gateway.Update(update, item => item.Id == "1234567890123456");
                this.gateway.Complete();
                var entity = this.gateway.Where<User>(item => item.Id == "1234567890123456").Select().First();
                Assert.AreEqual(entity.Sex, Sex.男);
            });

            gateway.Delete<User>(item => item.Id == "1234567890123456");
        }

        [TestMethod()]
        public void T10_DELETE()
        {
            bool ret = true;
            this.gateway.Insert(new LoginLog()
            {
                LogID = $"1",
                UserID = "1234567890123456",
                CreateDate = DateTime.Now,
                Content = $"U登录"
            });

            this.gateway.Insert(new LoginLog()
            {
                LogID = $"2",
                UserID = "1234567890123456",
                CreateDate = DateTime.Now,
                Content = $"U登录"
            });

            this.gateway.Insert(new LoginLog()
            {
                LogID = $"3",
                UserID = "1234567890123456",
                CreateDate = DateTime.Now,
                Content = $"U登录"
            });

            gateway.Delete<LoginLog>(x => x.LogID == "1");
            ret = !gateway.Where<LoginLog>(x => x.LogID == "1").Exists();
            Assert.IsTrue(ret, "step 1");

            ret = gateway.Where<LoginLog>(x => x.LogID == "2").Exists();
            Assert.IsTrue(ret, "step 2");

            string[] delArray = null;
            gateway.Delete<LoginLog>(x => delArray.Contains(x.LogID));
            delArray = new string[2];
            gateway.Delete<LoginLog>(x => delArray.Contains(x.LogID));
            ret = gateway.Where<LoginLog>(x => x.LogID == "2").Exists();
            Assert.IsTrue(ret, "step 3");

            delArray = new string[] { "3" };
            ret = gateway.Where<LoginLog>(x => delArray.Contains(x.LogID) && x.UserID == "1234567890123456").Exists();
            Assert.IsTrue(ret, "step 4");

            gateway.Delete<LoginLog>(x => delArray.Contains(x.LogID));
            ret = !gateway.Where<LoginLog>(x => delArray.Contains(x.LogID)).Exists();
            Assert.IsTrue(ret, "step 5");

            // 测试没有通过
            //var delArray2 = new DateTime[] { DateTime.Now };
            //gateway.Delete<LoginLog>(x => (x.LogID == "") && delArray2.Contains(x.CreateDate));
            //ret = !gateway.Where<LoginLog>(x => delArray2.Contains(x.CreateDate)).Exists();
            //Assert.IsTrue(ret, "step 6");

            var deletes = new string[] { "1", "2", "3" };
            gateway.Delete<LoginLog>(item => deletes.Contains(item.LogID));
        }

        [TestMethod()]
        public void FindInfos()
        {
            string[] userIDs = { "user1" };
            var query = gateway.Where<Enforcer>(x => userIDs.Contains(x.UserID))
                .Join<AuthUser>(x => x.UserID, y => y.UserID, JoinType.Left)
                .Load(x => x.Users);
            var result = query.Select().ToList();
        }

        #region 处理测试数据
        protected void AddTestData()
        {
            #region 删除测试数据
            gateway.Delete<UserRole>(x => x.Id == "UR11");
            gateway.Delete<UserRole>(x => x.Id == "UR12");
            gateway.Delete<UserRole>(x => x.Id == "UR21");
            gateway.Delete<UserRole>(x => x.Id == "UR22");
            gateway.Delete<User>(x => x.Id == "U1");
            gateway.Delete<User>(x => x.Id == "U2");
            gateway.Delete<Role>(x => x.RoleID == "R1");
            gateway.Delete<Role>(x => x.RoleID == "R2");
            #endregion

            #region 新增测试数据
            gateway.Insert(new Role()
            {
                RoleID = "R1",
                RoleName = "R1_NAME"
            });

            gateway.Insert(new Role()
            {
                RoleID = "R2",
                RoleName = "R2_NAME"
            });

            gateway.Insert(new User()
            {
                Id = "U1",
                UserName = "U1_NAME",
                Password = "U1_PASSWORD"
            });

            gateway.Insert(new User()
            {
                Id = "U2",
                UserName = "U2_NAME",
                Password = "U2_PASSWORD"
            });

            gateway.Insert(new UserRole()
            {
                Id = "UR11",
                RoleId = "R1",
                UserId = "U1"
            });

            gateway.Insert(new UserRole()
            {
                Id = "UR12",
                RoleId = "R2",
                UserId = "U1"
            });

            gateway.Insert(new UserRole()
            {
                Id = "UR21",
                RoleId = "R1",
                UserId = "U2"
            });

            gateway.Insert(new UserRole()
            {
                Id = "UR22",
                RoleId = "R2",
                UserId = "U2"
            });
            #endregion
        }

        protected void DeleteTestData()
        {
            #region 删除测试数据
            gateway.Delete<UserRole>(x => x.Id == "UR11");
            gateway.Delete<UserRole>(x => x.Id == "UR12");
            gateway.Delete<UserRole>(x => x.Id == "UR21");
            gateway.Delete<UserRole>(x => x.Id == "UR22");
            gateway.Delete<User>(x => x.Id == "U1");
            gateway.Delete<User>(x => x.Id == "U2");
            gateway.Delete<Role>(x => x.RoleID == "R1");
            gateway.Delete<Role>(x => x.RoleID == "R2");
            #endregion
        }
        #endregion
    }


}