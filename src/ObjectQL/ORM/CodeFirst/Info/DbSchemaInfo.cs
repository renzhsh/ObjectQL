using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;

namespace ObjectQL.CodeFirst
{
    public class DbSchemaInfo
    {
        public string Schema { get; set; }

        public ConnectionSettings ConnectionSetting { get; set; }

        /// <summary>
        /// 建库策略
        /// </summary>
        public string BuildingPolicy { get; set; }
    }
}
