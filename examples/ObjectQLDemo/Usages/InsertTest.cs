using System;
using UserCenter.Model;
using System.Collections.Generic;
using System.Linq;
using ObjectQL;

//添加程序集引用System.Transactions.dll
using System.Transactions;

namespace UserCenter.Usages
{
    public class InsertTest
    {
        public InsertTest()
        {
            gateway = new DataGateway();
        }

        public DataGateway gateway { get; }

        public void Insert()
        {
            RealUser user = new RealUser
            {
                RealUserID = gateway.GetSequence().NextLuhmValue,
                UserID = gateway.GetSequence("realUser").NextLuhmValue,
                RealName = "adsfe",
                CreateDate = DateTime.Now
            };

            gateway.Insert(user);

        }

        public void InsertBatch()
        {
            List<RealUser> list = new List<RealUser>();
            for (int i = 0; i < 3; i++)
            {
                RealUser user = new RealUser
                {
                    RealUserID = gateway.GetSequence().NextLuhmValue,
                    UserID = gateway.GetSequence("realUser").NextLuhmValue,
                    RealName = "adsfe",
                    CreateDate = DateTime.Now
                };
                list.Add(user);
            }
            gateway.InsertBatch(list);
        }


        /// <summary>
        /// 启用事务
        /// </summary>
        public void EnableTransaction()
        {
            List<RealUser> list = new List<RealUser>();
            for (int i = 0; i < 3; i++)
            {
                RealUser user = new RealUser
                {
                    RealUserID = gateway.GetSequence().NextLuhmValue,
                    UserID = gateway.GetSequence("realUser").NextLuhmValue,
                    RealName = "adsfe",
                    CreateDate = DateTime.Now
                };
                list.Add(user);
            }
            using (TransactionScope scope = new TransactionScope())
            {
                gateway.InsertBatch(list);

                scope.Complete();
            }
        }

        /// <summary>
        /// 事务异常
        /// </summary>
        public void Transaction_Exception()
        {
            List<RealUser> list = new List<RealUser>();

            RealUser user = new RealUser
            {
                RealUserID = gateway.GetSequence().NextLuhmValue,
                UserID = gateway.GetSequence("realUser").NextLuhmValue,
                RealName = "adsfe",
                CreateDate = DateTime.Now
            };
            list.Add(user);

            user = new RealUser
            {
                RealUserID = gateway.GetSequence().NextLuhmValue,
                UserID = gateway.GetSequence("realUser").NextLuhmValue,
                RealName = "adsfe",

                //IDCardNum最长20位，这个字段将抛出异常
                IDCardNum = "1111111111111111111111",
                CreateDate = DateTime.Now
            };

            list.Add(user);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    gateway.InsertBatch(list);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                }
            }

        }
    }
}
