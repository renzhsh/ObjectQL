using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;

namespace ObjectQL.Mapping
{
    public class DbTableInfo
    {

        public DbTableInfo(string name)
        {
            TableName = name;
        }

        public string TableName { get; }

        public ConnectionSettings ConnectionSetting { get; set; }
    }
}
