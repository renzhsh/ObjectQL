using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.CodeFirst
{
    public class DbColumnObject
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// 数据字段名称
        /// </summary>
        public string ColumnName { set; get; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public int DataMaxLength { get; set; }

        /// <summary>
        /// 小数点位数
        /// </summary>
        public int DataScale { get; set; }

        /// <summary>
        /// 字符串 Unicode 编码
        /// </summary>
        public bool Unicode { get; set; } = true;

        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool AllowNull { get; set; }
    }

    public class DbColumnNameComparer<T> : IEqualityComparer<T> where T : DbColumnObject, new()
    {
        public bool Equals(T x, T y)
        {
            return x.ColumnName.Equals(y.ColumnName);
        }

        public int GetHashCode(T obj)
        {
            var target = (DbColumnObject)obj;
            return target.ColumnName.GetHashCode();
        }
    }

    public class DbColumnEqualityComparer<T> : IEqualityComparer<T> where T : DbColumnObject, new()
    {
        public bool Equals(T x, T y)
        {
            return x.ColumnName.Equals(y.ColumnName) && x.DataType.Equals(y.DataType) &&
                x.DataMaxLength == y.DataMaxLength && x.DataScale == y.DataScale &&
                x.AllowNull == y.AllowNull;
        }

        public int GetHashCode(T obj)
        {
            var target = (DbColumnObject)obj;
            return target.ColumnName.GetHashCode() & target.DataType.GetHashCode() &
                target.DataMaxLength.GetHashCode() & target.DataScale.GetHashCode() &
                target.AllowNull.GetHashCode();
        }
    }
}
