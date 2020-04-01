using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.DataAnnotation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute() { }
        public TableAttribute(string TableName)
        {
            this.TableName = TableName;
        }

        public string TableName { get; set; }

        /// <summary>
        /// 方案或表空间
        /// </summary>
        public string Schema { get; set; }

        public string Prefix { get; set; } = "T_";
    }
}
