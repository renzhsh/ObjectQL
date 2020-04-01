using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.CodeFirst;
using ObjectQL.Data;

namespace ObjectQL.Data.OracleClient
{
    /// <summary>
    /// 使用的数据类型包括：
    /// 字符串：char/varchar/nvarchar/long
    /// 数字：binary_float/binary_double/number
    /// 二进制：raw/long raw
    /// 日期：date/timestamp
    /// </summary>
    public class OracleMigrateProvider : BaseMigrateProvider
    {
        public OracleMigrateProvider(ConnectionSettings connSetting) : base(connSetting) { }

        #region "Table"
        public override IEnumerable<DbTableObject> GetOwnerTables()
        {
            const string sql = "select table_name, column_name, data_type, data_length, data_precision, data_scale, nullable, data_default, char_length from user_tab_columns";

            IDictionary<string, DbTableObject> dict = new Dictionary<string, DbTableObject>();

            var dr = DataAccess.ExecuteReader(sql);

            while (dr.Read())
            {
                string tableName = (string)dr["TABLE_NAME"];
                DbTableObject tableInfo = null;
                if (dict.ContainsKey(tableName))
                    tableInfo = dict[tableName];
                else
                {
                    tableInfo = new DbTableObject(tableName)
                    {
                        ConnectionSettings = ConnectionSetting
                    };
                    dict.Add(tableName, tableInfo);
                }

                var column = BuildColumn(dr);
                tableInfo.Columns.Add(column.ColumnName, column);
            }

            Dictionary<string, List<string>> colDict = new Dictionary<string, List<string>>();

            var cstSql = "select col.constraint_name name,col.column_name,col.position ,cst.constraint_type type,cst.table_name " +
                "from USER_CONS_COLUMNS col,user_constraints cst " +
                $"where col.constraint_name=cst.constraint_name and cst.constraint_type!='C' " +
                "order by name ,position";

            dr = DataAccess.ExecuteReader(cstSql);

            while (dr.Read())
            {
                string tableName = (string)dr["TABLE_NAME"];
                if (!dict.ContainsKey(tableName)) continue;

                var tableInfo = dict[tableName];

                string name = (string)dr["NAME"];
                if (colDict.ContainsKey(name))
                {
                    colDict[name].Add((string)dr["COLUMN_NAME"]);
                }
                else
                {
                    DbConstraintObject cst = new DbConstraintObject
                    {
                        Name = (string)dr["NAME"]
                    };

                    switch ((string)dr["TYPE"])
                    {
                        case "P":
                            cst.Type = ConstraintType.PrimaryKey;
                            break;
                        case "U":
                            cst.Type = ConstraintType.Unique;
                            break;
                        case "F":
                            cst.Type = ConstraintType.ForeignKey;
                            break;
                    }

                    tableInfo.Constraints.Add(cst);

                    colDict.Add(cst.Name, new List<string> { (string)dr["COLUMN_NAME"] });
                }
            }

            dict.Values.Select(item => item.Constraints).ToList().ForEach(list =>
            {
                list.ForEach(item =>
                {
                    item.ColumnNames = colDict[item.Name].ToArray();
                });
            });


            return dict.Values;
        }

        public override DbTableObject GetTable(string name)
        {
            DbTableObject tableInfo = new DbTableObject(name)
            {
                ConnectionSettings = ConnectionSetting
            };

            string sql = $"select column_name,data_type, data_length,data_precision,data_scale,nullable,data_default,char_length from user_tab_columns where table_name='{name}'";

            var dr = DataAccess.ExecuteReader(sql);

            while (dr.Read())
            {
                DbColumnObject column = BuildColumn(dr);

                tableInfo.Columns.Add(column.ColumnName, column);
            }

            tableInfo.Constraints.AddRange(GetConstraints(name));

            return tableInfo;
        }

        protected DbColumnObject BuildColumn(IResultReader dr)
        {
            Func<object, int> ConvertInt = (obj) =>
            {
                return Convert.IsDBNull(obj) ? 0 : Convert.ToInt32(obj);
            };

            DbColumnObject column = new DbColumnObject
            {
                ColumnName = (string)dr["COLUMN_NAME"],
                DataType = (string)dr["DATA_TYPE"],
                AllowNull = ((string)dr["NULLABLE"]) == "Y" ? true : false
            };
            var stringTypes = new string[] { "CHAR", "VARCHAR2", "NVARCHAR2", "LONG" };
            if (stringTypes.Contains(column.DataType))
            {
                column.Unicode = (column.DataType == "NVARCHAR2");
                column.DataMaxLength = ConvertInt(dr["CHAR_LENGTH"]);
            }

            if (column.DataType == "NUMBER")
            {
                column.DataMaxLength = ConvertInt(dr["DATA_PRECISION"]);
                column.DataScale = ConvertInt(dr["DATA_SCALE"]);
            }

            if (column.DataType == "RAW")
            {
                column.DataMaxLength = ConvertInt(dr["DATA_PRECISION"]);
            }

            if (column.DataType.StartsWith("TIMESTAMP"))
            {
                column.DataScale = ConvertInt(dr["DATA_SCALE"]);
            };

            return column;
        }

        public override void CreateTable(DbTableObject info)
        {
            string colSql = string.Join(",", info.Columns.Values.Select(item => CreateColumnSql(item)));

            string sql = $"create Table {info.TableName} ({colSql}) ";

            if (!string.IsNullOrEmpty(info.Schema))
            {
                sql += $"tablespace {info.Schema} pctfree 10 initrans 1 maxtrans 255 storage(initial 64K minextents 1 maxextents unlimited)";
            }

            DataAccess.ExecuteNonQuery(sql);
        }

        protected override string CreateColumnSql(DbColumnObject column)
        {
            StringBuilder sql = new StringBuilder($"{column.ColumnName} ");

            bool resolved = false;
            //字符串
            var stringTypes = new string[] { "CHAR", "VARCHAR2", "NVARCHAR2", "LONG" };
            if (stringTypes.Contains(column.DataType))
            {
                if (column.DataType == "LONG")
                {
                    sql.Append($"{column.DataType} ");
                }
                else
                {
                    sql.Append($"{column.DataType}({column.DataMaxLength}) ");
                }

                resolved = true;
            }

            // 数字
            string[] numberTypes = new string[] { "BINARY_FLOAT", "BINARY_DOUBLE", "NUMBER" };
            if (numberTypes.Contains(column.DataType))
            {
                if (column.DataType == "NUMBER")
                {
                    if (column.DataMaxLength > 0)
                    {
                        if (column.DataScale > 0)
                        {
                            sql.Append($"{column.DataType}({column.DataMaxLength},{column.DataScale}) ");
                        }
                        else
                        {
                            sql.Append($"{column.DataType}({column.DataMaxLength}) ");
                        }
                    }
                    else
                    {
                        sql.Append($"{column.DataType} ");
                    }
                }
                else
                {
                    sql.Append($"{column.DataType} ");
                }

                resolved = true;
            }

            // 二进制
            string[] binaryTypes = new string[] { "RAW", "LONG RAW" };
            if (binaryTypes.Contains(column.DataType))
            {
                if (column.DataType == "RAW")
                {
                    sql.Append($"{column.DataType}({column.DataMaxLength}) ");
                }
                else
                {
                    sql.Append($"{column.DataType} ");
                }

                resolved = true;
            }

            // 日期
            if (column.DataType == "DATE" || column.DataType.StartsWith("TIMESTAMP"))
            {
                sql.Append($"{column.DataType} ");

                resolved = true;
            }

            if (!resolved)
            {
                throw new Exception($"未处理的数据类型：{column.DataType}");
            }

            if (!column.AllowNull)
            {
                sql.Append("not null");
            }

            return sql.ToString();
        }

        public override void AlterColumn(string table, DbColumnChange change)
        {
            var column = change.Column;
            string sql = $"ALTER TABLE {table} modify ({CreateColumnSql(column)})";
            DataAccess.ExecuteNonQuery(sql);
        }

        #endregion

        #region "Constraint"

        public override IEnumerable<DbConstraintObject> GetConstraints(string table)
        {
            List<DbConstraintObject> result = new List<DbConstraintObject>();

            Dictionary<string, List<string>> colDict = new Dictionary<string, List<string>>();

            string sql = "select col.constraint_name name,col.column_name,col.position ,cst.constraint_type type,cst.table_name " +
                "from USER_CONS_COLUMNS col,user_constraints cst " +
                $"where col.constraint_name=cst.constraint_name and cst.constraint_type!='C' and cst.table_name='{table}' " +
                "order by name ,position";

            var dr = DataAccess.ExecuteReader(sql);

            while (dr.Read())
            {
                string name = (string)dr["NAME"];
                if (colDict.ContainsKey(name))
                {
                    colDict[name].Add((string)dr["COLUMN_NAME"]);
                }
                else
                {
                    DbConstraintObject cst = new DbConstraintObject
                    {
                        Name = (string)dr["NAME"]
                    };

                    switch (cst.Name)
                    {
                        case "P":
                            cst.Type = ConstraintType.PrimaryKey;
                            break;
                        case "U":
                            cst.Type = ConstraintType.Unique;
                            break;
                        case "F":
                            cst.Type = ConstraintType.ForeignKey;
                            break;
                    }

                    colDict.Add(cst.Name, new List<string> { (string)dr["COLUMN_NAME"] });
                }
            }

            result.ForEach(item =>
            {
                item.ColumnNames = colDict[item.Name].ToArray();
            });

            return result;
        }

        #endregion

        public override void ConvertDbType(ref DbColumnObject column)
        {
            var PropertyType = column.PropertyType.FullName;
            // Nullable<>
            if (column.PropertyType.IsGenericType && column.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var type = column.PropertyType.GetGenericArguments().FirstOrDefault();
                PropertyType = type.FullName;
            }
            switch (PropertyType)
            {
                // 整型
                // sbyte -2^7 - 2^7-1
                case "System.SByte":
                // byte 0-2^8-1
                case "System.Byte":
                    column.DataType = "NUMBER";
                    if (column.DataMaxLength == 0)
                    {
                        column.DataMaxLength = 3;
                    }
                    break;
                // short -2^16 - 2^16-1
                case "System.Int16":
                // ushort 0-2^16-1
                case "System.UInt16":
                    column.DataType = "NUMBER";
                    if (column.DataMaxLength == 0)
                    {
                        column.DataMaxLength = 5;
                    }
                    break;
                // int -2^31 - 2^31-1
                case "System.Int32":
                // uint 0-2^32-1
                case "System.UInt32":
                    column.DataType = "NUMBER";
                    if (column.DataMaxLength == 0)
                    {
                        column.DataMaxLength = 10;
                    }
                    break;
                // long -2^63 - 2^63-1
                case "System.Int64":
                // ulong 0-2^64-1
                case "System.UInt64":
                    column.DataType = "NUMBER";
                    break;
                // 浮点类型
                case "System.Single": //float
                    column.DataType = "BINARY_FLOAT";
                    break;
                case "System.Double":
                    column.DataType = "BINARY_DOUBLE";
                    break;
                case "System.Decimal":
                    column.DataType = "NUMBER";
                    break;
                // 布尔类型
                case "System.Boolean":
                    column.DataType = "NUMBER";
                    column.DataMaxLength = 1;
                    break;
                // 字符类型
                case "System.Char":
                    column.DataType = "CHAR";
                    column.DataMaxLength = 1;
                    break;
                case "System.Char[]":
                    column.DataType = "CHAR";
                    if (column.DataMaxLength > 2000)
                    {
                        column.DataMaxLength = 2000;
                    }
                    if (column.DataMaxLength == 0)
                    {
                        column.DataMaxLength = 40;
                    }
                    break;
                case "System.String":
                    if (column.DataMaxLength == 0)
                    {
                        column.DataType = (column.Unicode ? "NVARCHAR2" : "VARCHAR2");
                        column.DataMaxLength = 40;
                    }
                    else
                    {
                        if (column.Unicode)
                        {
                            if (column.DataMaxLength > 2000)
                            {
                                column.DataType = "LONG";
                            }
                            else
                            {
                                column.DataType = "NVARCHAR2";
                            }
                        }
                        else
                        {
                            if (column.DataMaxLength > 4000)
                            {
                                column.DataType = "LONG";
                            }
                            else
                            {
                                column.DataType = "VARCHAR2";
                            }
                        }
                    }
                    break;
                // 日期类型
                case "System.DateTime":
                    column.DataType = "DATE";
                    break;
                //case "System.TimeSpan":
                //    if (column.DataMaxLength == 0)
                //    {
                //        column.DataMaxLength = 6;
                //    }
                //    if (column.DataMaxLength > 9)
                //    {
                //        column.DataMaxLength = 9;
                //    }
                //    column.DataType = $"TIMESTAMP({column.DataMaxLength})";
                //    break;
                // 二进制
                case "System.Byte[]":
                    if (column.DataMaxLength == 0)
                    {
                        column.DataMaxLength = 2000;
                        column.DataType = "RAW";
                    }
                    if (column.DataMaxLength > 2000)
                    {
                        column.DataType = "LONG RAW";
                    }
                    break;
                default:
                    if (column.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        column.DataType = "NUMBER";
                        column.DataMaxLength = 3;
                    }
                    else
                    {
                        throw new Exception($"不支持的数据类型{column.PropertyType.FullName}");
                    }
                    break;
            }
        }
    }
}
