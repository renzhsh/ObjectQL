using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data.SqlServer;

namespace ObjectQL.Data
{
    public class SqlServerDriverProvider : IObjectQLDriverProvider
    {
        public SqlServerDriverProvider()
        {
            CommandBuildProvider = new SqlServerCommandBuildProvider();
        }

        protected ICommandBuildProvider CommandBuildProvider { get; }

        public ICommandBuildProvider GetCommandBuildProvider()
        {
            return CommandBuildProvider;
        }

        public IDataAccess GetDataAccess(ConnectionSettings setting)
        {
            var db = new SqlServerDataAccess(setting);

            db.CommandBuildProvider = CommandBuildProvider;

            return db;
        }

        public IMigrateProvider GetMigrateProvider(ConnectionSettings setting)
        {
            throw new NotImplementedException();
        }
    }
}
