using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;
using ObjectQL.CodeFirst;

namespace ObjectQL
{
    public class OrmContext
    {
        public static DataDriverProvider DriverProviders { get; } = new DataDriverProvider();

        public static DataOrmProvider OrmProvider { get; } = new DataOrmProvider();

        public static DbRelationProvider RelationProvider { get; } = new DbRelationProvider();
    }
}
