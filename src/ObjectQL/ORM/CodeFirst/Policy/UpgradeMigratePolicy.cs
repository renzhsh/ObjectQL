using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.CodeFirst.Policy
{
    /// <summary>
    /// 模型升级策略
    /// </summary>
    public class UpgradeMigratePolicy : BaseMigratePolicy
    {
        public UpgradeMigratePolicy(DbSchemaInfo schema) : base(schema) { }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
