using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Data
{
    public interface IObjectQLDriverProvider
    {
        IDataAccess GetDataAccess(ConnectionSettings setting);

        ICommandBuildProvider GetCommandBuildProvider();

        IMigrateProvider GetMigrateProvider(ConnectionSettings setting);
    }
}
