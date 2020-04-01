using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.DataAnnotation;

namespace ObjectQL.CodeFirst
{
    public class DbConstraintObject
    {
        public DbConstraintObject() { }
        public DbConstraintObject(ConstraintAttribute attribute)
        {
            Attribute = attribute;
        }

        public ConstraintAttribute Attribute { get; set; }

        public string Name { get; set; }

        public string[] ColumnNames { get; set; }

        /// <summary>
        /// 约束类型
        /// </summary>
        public ConstraintType Type { get; set; }
    }

    public enum ConstraintType
    {
        None,
        PrimaryKey,
        Unique,
        ForeignKey
    }

    public class DbConstraintNameComparer<T> : IEqualityComparer<T> where T : DbConstraintObject, new()
    {
        public bool Equals(T x, T y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(T obj)
        {
            var target = (DbConstraintObject)obj;
            return target.Name.GetHashCode();
        }
    }

    public class DbConstraintEqualComparer<T> : IEqualityComparer<T> where T : DbConstraintObject, new()
    {
        public bool Equals(T x, T y)
        {
            return x.Name.Equals(y.Name) && string.Join(",", x.ColumnNames).Equals(string.Join(",", y.ColumnNames));
        }

        public int GetHashCode(T obj)
        {
            var target = (DbConstraintObject)obj;
            var result = target.Name.GetHashCode();

            target.ColumnNames?.ToList().ForEach(item =>
            {
                result &= item.GetHashCode();
            });
            return result;
        }
    }
}
