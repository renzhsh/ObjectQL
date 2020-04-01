using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.CodeFirst.Policy;

namespace ObjectQL.CodeFirst
{
    public class MigrateEngine
    {
        internal static void Mirgate()
        {
            Parallel.ForEach(OrmContext.RelationProvider.Schemas, item =>
            {
                IMigratePolicy policy = null;
                switch (item.BuildingPolicy.ToUpper())
                {
                    case "INITIALIZE":
                        policy = new InitializeMigratePolicy(item);
                        break;
                    case "MODIFY":
                    default:
                        policy = new ModifyMigratePolicy(item);
                        break;
                        //case "UPGRADE":
                        //    policy = new UpgradeMigratePolicy(item);
                        //    break;
                }
                policy.Execute();
            });
        }
    }
}
