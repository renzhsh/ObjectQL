using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.CodeFirst;

namespace ObjectQL.Data
{
    public interface IMigrateProvider
    {
        IEnumerable<DbTableObject> GetOwnerTables();

        DbTableObject GetTable(string name);

        void CreateTable(DbTableObject info);

        void RenameTable(string oldTable, string newTable);

        void DropTable(string name);

        void AddColumn(string table, DbColumnObject column);

        void AlterColumn(string table, DbColumnChange change);

        void DropColumn(string table, string name);

        IEnumerable<DbConstraintObject> GetConstraints(string table);

        void CreateConstraint(string table, DbConstraintObject constraint);

        void DropConstraint(string table, string name);

        void ConvertDbType(ref DbColumnObject column);
    }
}
