using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.DataAnnotation;

namespace ObjectQL.CodeFirst
{
    /// <summary>
    /// 数据库表
    /// </summary>
    public class DbTableObject
    {
        public DbTableObject(string name)
        {
            TableName = name;
        }

        public string TableName { get; }

        /// <summary>
        /// 数据库的解决方案/表空间
        /// </summary>
        public string Schema { get; set; }

        public ConnectionSettings ConnectionSettings { get; set; }

        public Dictionary<string, DbColumnObject> Columns { get; } = new Dictionary<string, DbColumnObject>();

        public List<DbConstraintObject> Constraints { get; } = new List<DbConstraintObject>();
    }

    public class DbTableNameComparer<T> : IEqualityComparer<T> where T : DbTableObject, new()
    {
        public bool Equals(T x, T y)
        {
            return x.TableName.Equals(y.TableName);
        }

        public int GetHashCode(T obj)
        {
            var target = (DbTableObject)obj;
            return target.GetHashCode();
        }
    }
}
