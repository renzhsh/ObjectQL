using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;

namespace ObjectQL.CodeFirst.Policy
{
    public abstract class BaseMigratePolicy : IMigratePolicy
    {
        public BaseMigratePolicy(DbSchemaInfo schema)
        {
            SchemaInfo = schema;
            ConnectionSetting = schema.ConnectionSetting;
            MigrateProvider = OrmContext.DriverProviders.GetMigrateProvider(ConnectionSetting);

            TableInfos = OrmContext.RelationProvider.TableInfos
                .Where(info => info.ConnectionSettings == ConnectionSetting)
                .ToList();

            TableInfos.ForEach(table =>
            {
                if (!string.IsNullOrEmpty(SchemaInfo.Schema))
                {
                    if (string.IsNullOrEmpty(table.Schema))
                    {
                        table.Schema = SchemaInfo.Schema;
                    }
                }
            });
        }

        public DbSchemaInfo SchemaInfo { get; }

        public ConnectionSettings ConnectionSetting { get; }

        protected IMigrateProvider MigrateProvider { get; }

        protected List<DbTableObject> TableInfos { get; }

        public abstract void Execute();

        private List<DbTableObject> GetPreviousTables()
        {
            return MigrateProvider.GetOwnerTables().ToList();
        }

        protected IEnumerable<DbChange> GetTableChanges()
        {
            var previous = GetPreviousTables();

            var result = new List<DbChange>();

            var diff = new DiffProvider();

            TableInfos.ForEach(table =>
            {
                result.Add(diff.Resolve(table, previous.Where(t => t.TableName == table.TableName).FirstOrDefault()));
            });

            return result;
        }

    }
}
