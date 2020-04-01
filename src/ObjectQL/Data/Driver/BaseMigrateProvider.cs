using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.CodeFirst;

namespace ObjectQL.Data
{
    public abstract class BaseMigrateProvider : IMigrateProvider
    {
        public BaseMigrateProvider(ConnectionSettings connSetting)
        {
            ConnectionSetting = connSetting;
        }

        public ConnectionSettings ConnectionSetting { get; }

        public IDataAccess DataAccess
        {
            get
            {
                return OrmContext.DriverProviders.GetDataAccess(ConnectionSetting);
            }

        }

        public abstract IEnumerable<DbTableObject> GetOwnerTables();

        public abstract DbTableObject GetTable(string name);

        public abstract void CreateTable(DbTableObject info);

        public virtual void RenameTable(string oldTable, string newTable)
        {
            string sql = $"alter table {oldTable} rename to {newTable}";

            DataAccess.ExecuteNonQuery(sql);
        }

        public virtual void DropTable(string name)
        {
            string sql = $"drop table {name}";
            DataAccess.ExecuteNonQuery(sql);
        }

        public virtual void AddColumn(string table, DbColumnObject column)
        {
            string sql = $"ALTER TABLE {table} ADD {CreateColumnSql(column)}";
            DataAccess.ExecuteNonQuery(sql);
        }

        public virtual void AlterColumn(string table, DbColumnChange change)
        {
            var column = change.Column;
            string sql = $"ALTER TABLE {table} ALTER COLUMN {CreateColumnSql(column)}";
            DataAccess.ExecuteNonQuery(sql);
        }

        public virtual void DropColumn(string table, string name)
        {
            string sql = $"alter table {table} drop column {name}";
            DataAccess.ExecuteNonQuery(sql);
        }

        public abstract IEnumerable<DbConstraintObject> GetConstraints(string table);

        public virtual void CreateConstraint(string table, DbConstraintObject constraint)
        {
            string sql = $"alter table {table} add constraint {constraint.Name} "
                + (constraint.Type == ConstraintType.PrimaryKey ? "primary key " : "unique ")
                + $"({string.Join(",", constraint.ColumnNames)})";

            DataAccess.ExecuteNonQuery(sql);
        }

        public virtual void DropConstraint(string table, string name)
        {
            string sql = $"alter table {table} drop constraint {name}";

            DataAccess.ExecuteNonQuery(sql);
        }

        public abstract void ConvertDbType(ref DbColumnObject column);

        protected abstract string CreateColumnSql(DbColumnObject column);
    }
}
