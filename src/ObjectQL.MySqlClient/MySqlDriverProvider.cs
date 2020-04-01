using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.Data.MySqlClient;

namespace ObjectQL.Data
{
    public class MySqlDriverProvider : IObjectQLDriverProvider
    {
        public MySqlDriverProvider()
        {
            CommandBuildProvider = new MySqlCommandBuildProvider();
        }

        protected ICommandBuildProvider CommandBuildProvider { get; }

        public ICommandBuildProvider GetCommandBuildProvider()
        {
            return CommandBuildProvider;
        }

        public IDataAccess GetDataAccess(ConnectionSettings setting)
        {
            var db = new MySqlDataAccess(setting);

            db.CommandBuildProvider = CommandBuildProvider;

            return db;
        }

        public IMigrateProvider GetMigrateProvider(ConnectionSettings setting)
        {
            throw new NotImplementedException();
        }
    }
}
