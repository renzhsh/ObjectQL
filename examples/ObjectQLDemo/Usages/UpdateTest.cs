using System;
using UserCenter.Model;
using System.Collections.Generic;
using System.Linq;
using ObjectQL;
using ObjectQL.Linq;

namespace UserCenter.Usages
{
    public class UpdateTest
    {
        public UpdateTest()
        {
            gateway = new DataGateway();
        }

        public DataGateway gateway { get; private set; }

        public void Update()
        {
            //UpdateCriteria<RealUser> criteria = new UpdateCriteria<RealUser>()
            //{
            //    {x=>x.RealName,"realName" },
            //    {x=>x.IDCardNum,"109458325" }
            //};

            UpdateCriteria<RealUser> criteria = new UpdateCriteria<RealUser>();

            criteria.Add(x => x.RealName, "realName");
            criteria.Add(x => x.IDCardNum, "109458325");

            gateway.Update(criteria, item => item.RealUserID == "1702270000003820");
        }
    }
}
