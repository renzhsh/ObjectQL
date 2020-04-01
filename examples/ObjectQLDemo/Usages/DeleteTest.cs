using System;
using UserCenter.Model;
using System.Collections.Generic;
using System.Linq;
using ObjectQL;

namespace UserCenter.Usages
{
    public class DeleteTest
    {
        public void Delete()
        {
            var gateway = new DataGateway();
            gateway.Delete<RealUser>(item => item.RealUserID == "1702270000003804");
        }
    }
}
