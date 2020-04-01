using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.Data.OracleClient;

namespace ObjectQL.Data
{
    public class OracleDirverProvider : IObjectQLDriverProvider
    {
        public OracleDirverProvider()
        {
            CommandBuildProvider = new OracleCommandBuildProvider();
        }

        protected ICommandBuildProvider CommandBuildProvider { get; }

        protected IMigrateProvider MigrateProvider { get; private set; }

        public ICommandBuildProvider GetCommandBuildProvider()
        {
            return CommandBuildProvider;
        }

        public IDataAccess GetDataAccess(ConnectionSettings setting)
        {
            var db = new OracleDataAccess(setting);

            db.CommandBuildProvider = CommandBuildProvider;

            return db;
        }

        public IMigrateProvider GetMigrateProvider(ConnectionSettings setting)
        {
            if (MigrateProvider == null)
            {
                MigrateProvider = new OracleMigrateProvider(setting);
            }

            return MigrateProvider;
        }
    }
}
