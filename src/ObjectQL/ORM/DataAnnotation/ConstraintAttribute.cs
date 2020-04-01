using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.DataAnnotation
{
    public abstract class ConstraintAttribute : Attribute
    {
        /// <summary>
        /// 约束名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 约束属性，多个属性以英文逗号(,)分隔
        /// </summary>
        public string Property { get; set; }
    }

    /// <summary>
    /// 主键约束
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : ConstraintAttribute { }

    /// <summary>
    /// 猜测的主键
    /// </summary>
    internal class GuessPrimaryKey : ConstraintAttribute { }

    /// <summary>
    /// 唯一性约束
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class UniqueAttribute : ConstraintAttribute { }

}
