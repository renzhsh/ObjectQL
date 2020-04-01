using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.CodeFirst
{
    /// <summary>
    /// DbTableObject比对器
    /// </summary>
    public class DiffProvider
    {
        public DbChange Resolve(DbTableObject dbTable, DbTableObject previous)
        {
            var result = new DbChange(dbTable);
            if (previous == null)
            {
                result.NewTable = true;
                return result;
            }

            var colEqualComparer = new DbColumnEqualityComparer<DbColumnObject>();
            var colNameComparer = new DbColumnNameComparer<DbColumnObject>();

            result.ColumnNews = dbTable.Columns.Values.Except(previous.Columns.Values, colNameComparer);
            result.ColumnDeletes = previous.Columns.Values.Except(dbTable.Columns.Values, colNameComparer);
            result.ColumnChanges = dbTable.Columns.Values.Intersect(previous.Columns.Values, colNameComparer)
                .Except(previous.Columns.Values.Intersect(dbTable.Columns.Values, colNameComparer), colEqualComparer)
                .Select(current =>
                {
                    var change = new DbColumnChange(current);
                    var origin = previous.Columns.Values.Where(c => c.ColumnName == current.ColumnName).FirstOrDefault();

                    if (current.IsPrimaryKey)
                    {
                        current.AllowNull = false;
                    }

                    change.TypeChange = !current.DataType.Equals(origin.DataType);
                    change.ExpandSpace = current.DataMaxLength != origin.DataMaxLength || current.DataScale != origin.DataScale;
                    change.AllowNullChange = current.AllowNull != origin.AllowNull;

                    return change;
                });


            var conEqualComparer = new DbConstraintEqualComparer<DbConstraintObject>();
            var conNameComparer = new DbConstraintNameComparer<DbConstraintObject>();

            result.ConstraintNews = dbTable.Constraints.Except(previous.Constraints, conNameComparer);
            result.ConstraintDeletes = previous.Constraints.Except(dbTable.Constraints, conNameComparer);
            result.ConstraintChanges = dbTable.Constraints.Intersect(previous.Constraints, conNameComparer)
                .Except(previous.Constraints.Intersect(dbTable.Constraints, conNameComparer), conEqualComparer);

            return result;
        }
    }
}
