using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.Data.AccessClient;

namespace ObjectQL.Data
{
    public class AccessDriverProvider : IObjectQLDriverProvider
    {
        public AccessDriverProvider()
        {
            CommandBuildProvider = new AccessCommandBuildProvider();
        }

        protected ICommandBuildProvider CommandBuildProvider { get; }

        public ICommandBuildProvider GetCommandBuildProvider()
        {
            return CommandBuildProvider;
        }

        public IDataAccess GetDataAccess(ConnectionSettings setting)
        {
            var db = new AccessDataAccess(setting);

            db.CommandBuildProvider = CommandBuildProvider;

            return db;
        }

        public IMigrateProvider GetMigrateProvider(ConnectionSettings setting)
        {
            throw new NotImplementedException();
        }
    }
}
