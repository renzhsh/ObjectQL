using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.CodeFirst
{
    public class DbChange
    {
        public DbChange(DbTableObject table)
        {
            Table = table;
        }

        public DbTableObject Table { get; }

        /// <summary>
        /// 新建表
        /// </summary>
        public bool NewTable { get; internal set; }

        /// <summary>
        /// 新建字段
        /// </summary>
        public bool NewColumn
        {
            get
            {
                return ColumnNews != null && ColumnNews.Count() > 0;
            }
        }

        /// <summary>
        /// 新建的字段
        /// </summary>
        public IEnumerable<DbColumnObject> ColumnNews { get; set; }

        /// <summary>
        /// 字段修改
        /// </summary>
        public bool ChangeColumn
        {
            get
            {
                return ColumnChanges != null && ColumnChanges.Count() > 0;
            }
        }

        /// <summary>
        /// 修改的字段
        /// </summary>
        public IEnumerable<DbColumnChange> ColumnChanges { get; set; }

        /// <summary>
        /// 字段删除
        /// </summary>
        public bool DeleteColumn
        {
            get
            {
                return ColumnDeletes != null && ColumnDeletes.Count() > 0;
            }
        }

        /// <summary>
        /// 删除的字段
        /// </summary>
        public IEnumerable<DbColumnObject> ColumnDeletes { get; set; }

        /// <summary>
        /// 新建约束
        /// </summary>
        public bool NewConstraint
        {
            get
            {
                return ConstraintNews != null && ConstraintNews.Count() > 0;
            }
        }

        /// <summary>
        /// 新建的约束
        /// </summary>
        public IEnumerable<DbConstraintObject> ConstraintNews { get; set; }

        /// <summary>
        /// 约束修改
        /// </summary>
        public bool ChangeConstraint
        {
            get
            {
                return ConstraintChanges != null && ConstraintChanges.Count() > 0;
            }
        }

        /// <summary>
        /// 修改的约束
        /// </summary>
        public IEnumerable<DbConstraintObject> ConstraintChanges { get; set; }

        /// <summary>
        /// 约束删除
        /// </summary>
        public bool DeleteConstraint
        {
            get
            {
                return ConstraintDeletes != null && ConstraintDeletes.Count() > 0;
            }
        }

        /// <summary>
        /// 删除的约束
        /// </summary>
        public IEnumerable<DbConstraintObject> ConstraintDeletes { get; set; }

        /// <summary>
        /// 是否变更
        /// </summary>
        public bool Changed
        {
            get
            {
                return NewTable || NewColumn || ChangeColumn || DeleteColumn ||
                    NewConstraint || ChangeConstraint || DeleteConstraint;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"======={Table.TableName}  Detail=======");
            if (NewTable)
            {
                sb.AppendLine("新建表");
            }
            if (NewColumn)
            {
                sb.AppendLine($"新建字段：{string.Join(",", ColumnNews.Select(item => item.ColumnName))}");
            }
            if (ChangeColumn)
            {
                sb.AppendLine($"变更字段：");
                ColumnChanges.ToList().ForEach(col =>
                {
                    sb.AppendLine(col.ToString());
                });
            }
            if (DeleteColumn)
            {
                sb.AppendLine($"删除字段：{string.Join(",", ColumnDeletes.Select(item => item.ColumnName))}");
            }
            if (NewConstraint)
            {
                sb.AppendLine($"新建约束：{string.Join(",", ConstraintNews.Select(item => item.Name))}");
            }
            if (ChangeConstraint)
            {
                sb.AppendLine($"变更约束：");
                ConstraintChanges.ToList().ForEach(cons =>
                {
                    sb.AppendLine($"{cons.Name}=>{string.Join(",", cons.ColumnNames)}");
                });
            }
            if (DeleteConstraint)
            {
                sb.AppendLine($"删除约束：{string.Join(",", ConstraintDeletes.Select(item => item.Name))}");
            }

            return sb.ToString();
        }
    }
}
