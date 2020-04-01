using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.CodeFirst
{
    public class DbColumnChange
    {
        public DbColumnChange(DbColumnObject column)
        {
            Column = column;
        }

        public DbColumnObject Column { get; }

        public bool TypeChange { get; set; }

        public bool ExpandSpace { get; set; }

        public bool AllowNullChange { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (TypeChange)
            {
                sb.Append($"类型修改=>{Column.DataType};");
            }
            if (ExpandSpace)
            {
                sb.Append($"范围修改=>DataMaxLength={Column.DataMaxLength},DataScale={Column.DataScale};");
            }
            if (AllowNullChange)
            {
                sb.Append($"非空约束修改=>AllowNull={Column.AllowNull};");
            }

            return sb.ToString();
        }
    }
}
