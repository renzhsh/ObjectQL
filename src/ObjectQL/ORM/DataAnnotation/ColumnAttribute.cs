using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.DataAnnotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute() { }

        public ColumnAttribute(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }

        public string ColumnName { get; set; }

        /// <summary>
        /// 前缀，默认'F_'
        /// </summary>
        public string Prefix { get; set; } = "F_";

        /// <summary>
        /// 允许为空，默认true
        /// </summary>
        public bool AllowNull { get; set; } = true;

        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// 小数点位数
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 字符串Unicode编码，默认true;
        /// true => nvarchar | false => varchar
        /// </summary>
        public bool Unicode { get; set; } = true;
    }
}
