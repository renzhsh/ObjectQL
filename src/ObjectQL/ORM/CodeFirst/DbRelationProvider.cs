using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.DataAnnotation;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ObjectQL.CodeFirst
{
    public class DbRelationProvider
    {
        private static IDictionary<string, DbTableObject> tableDict = new
            Dictionary<string, DbTableObject>();

        private static IDictionary<string, DbSchemaInfo> schemaDict = new
            Dictionary<string, DbSchemaInfo>();

        public void AddMapContainer(MapContainer container)
        {
            if (!schemaDict.ContainsKey(container.ConnectionKey))
            {
                var connectionStringSettings = Jinhe.Config.GetConnectionSettings(container.ConnectionKey);
                var settings = new ConnectionSettings(connectionStringSettings);
                schemaDict.Add(container.ConnectionKey, new DbSchemaInfo
                {
                    Schema = container.Schema,
                    BuildingPolicy = container.BuildingPolicy,
                    ConnectionSetting = settings
                });
            }
        }

        public DbTableObject Register<T>(ConnectionSettings connSetting) where T : class, new()
        {
            Type entityType = typeof(T);

            DbTableObject info = null;

            TableAttribute attr = entityType.GetCustomAttribute<TableAttribute>();

            if (attr == null)
            {
                attr = new TableAttribute();
            }

            string tableName = !string.IsNullOrEmpty(attr.TableName) ? attr.TableName : GetDbName(entityType.Name, attr.Prefix);

            info = new DbTableObject(tableName)
            {
                ConnectionSettings = connSetting,
                Schema = attr.Schema
            };

            PropertyInfo[] propertyInfos = entityType.GetProperties();

            foreach (var prop in propertyInfos)
            {
                //  不支持泛型
                if (prop.PropertyType.IsGenericType)
                {
                    // Nullable<>除外
                    if (prop.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                    {
                        continue;
                    }
                }

                NotMappedAttribute notMapped = prop.GetCustomAttribute<NotMappedAttribute>();
                if (notMapped != null) continue;

                DbColumnObject colInfo = null;
                ColumnAttribute colAttr = prop.GetCustomAttribute<ColumnAttribute>();

                if (colAttr == null)
                {
                    colAttr = new ColumnAttribute();
                }

                string colName = !string.IsNullOrEmpty(colAttr.ColumnName) ? colAttr.ColumnName : GetDbName(prop.Name, colAttr.Prefix);
                colInfo = new DbColumnObject
                {
                    AllowNull = colAttr.AllowNull,
                    ColumnName = colName,
                    DataMaxLength = colAttr.MaxLength,
                    DataScale = colAttr.Scale,
                    Unicode = colAttr.Unicode,
                    PropertyType = prop.PropertyType
                };

                info.Columns.Add(prop.Name, colInfo);
            }

            // 确定字段的数据库类型
            var migrateProvider = OrmContext.DriverProviders.GetMigrateProvider(connSetting);
            info.Columns.Values.ToList()
                .ForEach(item =>
                {
                    migrateProvider.ConvertDbType(ref item);
                });

            RegisterConstraints(entityType, info);

            tableDict.Add(entityType.Name, info);

            return info;
        }

        private void RegisterConstraints(Type entityType, DbTableObject info)
        {
            PropertyInfo[] propertyInfos = entityType.GetProperties();

            string[] suggestPrimaryKeys = new string[] { "ID", $"{entityType.Name.ToUpper()}ID" };

            List<DbConstraintObject> GuessConstraint = new List<DbConstraintObject>();

            foreach (var prop in propertyInfos)
            {
                if (!info.Columns.ContainsKey(prop.Name))
                {
                    continue;
                }

                var colInfo = info.Columns.Where(kvp => kvp.Key == prop.Name).First().Value;

                //默认的主键规则
                if (suggestPrimaryKeys.Contains(prop.Name.ToUpper()))
                {
                    GuessConstraint.Add(new DbConstraintObject(
                        new GuessPrimaryKey
                        {
                            Property = prop.Name
                        })
                    {
                        ColumnNames = new string[] { colInfo.ColumnName }
                    });
                }

                IEnumerable<ConstraintAttribute> colCstAttrs = prop.GetCustomAttributes<ConstraintAttribute>();

                if (colCstAttrs != null && colCstAttrs.Count() > 0)
                {
                    if (colCstAttrs.Where(item => item is PrimaryKeyAttribute).Count() > 0)
                    {
                        colInfo.IsPrimaryKey = true;
                    }

                    colCstAttrs.ToList().ForEach(item =>
                    {
                        item.Property = prop.Name;
                    });

                    info.Constraints.AddRange(colCstAttrs.Select(item => new DbConstraintObject(item)
                    {
                        Type = item is PrimaryKeyAttribute ? ConstraintType.PrimaryKey : ConstraintType.Unique,
                        ColumnNames = new string[] { colInfo.ColumnName }
                    }));
                }
            }

            IEnumerable<ConstraintAttribute> tbCstAttrs = entityType.GetCustomAttributes<ConstraintAttribute>();
            if (tbCstAttrs != null && tbCstAttrs.Count() > 0)
            {
                tbCstAttrs.ToList().ForEach(con =>
                {
                    string[] section = con.Property.Split(',');
                    if (section.Length == 0)
                    {
                        throw new Exception($"实体{entityType.FullName}设置约束{con.Name}({con.GetType().Name})失败，没有指定约束属性。");
                    }

                    string[] exceSection = section.Except(propertyInfos.Select(p => p.Name)).ToArray();
                    if (exceSection.Length > 0)
                    {
                        throw new Exception($"实体{entityType.FullName}设置约束{con.Name}({con.GetType().Name})失败，不存在的约束属性{string.Join(",", exceSection)}。");
                    }
                });
                //info.Constraints.AddRange(tbCstAttrs.Select(item => new DbConstraintObject(item)));
                info.Constraints.AddRange(tbCstAttrs.Select(item =>
                {
                    string[] section = item.Property.Split(',');
                    return new DbConstraintObject(item)
                    {
                        Type = item is PrimaryKeyAttribute ? ConstraintType.PrimaryKey : ConstraintType.Unique,
                        ColumnNames = info.Columns.Where(kvp => section.Contains(kvp.Key))
                        .OrderBy(kvp => kvp.Value.ColumnName)
                        .Select(kvp => kvp.Value.ColumnName)
                        .ToArray()
                    };
                }));
            }

            //未显式设置主键
            if (info.Constraints.Where(item => item.Attribute is PrimaryKeyAttribute).Count() == 0)
            {
                var guessKey = GuessConstraint.FirstOrDefault();
                if (guessKey == null)
                {
                    throw new Exception($"实体{entityType.FullName}没有设置主键");
                }
                else
                {
                    var col = info.Columns[guessKey.Attribute.Property];

                    var item = new PrimaryKeyAttribute
                    {
                        Property = guessKey.Attribute.Property
                    };
                    info.Constraints.Add(new DbConstraintObject(item)
                    {
                        Type = ConstraintType.PrimaryKey,
                        ColumnNames = new string[] { col.ColumnName }
                    });

                    col.IsPrimaryKey = true;
                }
            }
            // 设置多个主键
            else if (info.Constraints.Where(item => item.Attribute is PrimaryKeyAttribute).Count() > 1)
            {
                throw new Exception($"实体{entityType.FullName}设置了多个主键");
            }

            info.Constraints.ForEach(item =>
            {
                string prefix = item.Type == ConstraintType.PrimaryKey ? "pk" : "uq";
                var section = item.Attribute.Property.Split(',');
                string cols = string.Join("_", section.Select(name => name.Length > 3 ? name.Substring(0, 3) : name));

                item.Name = $"{prefix}_{info.TableName}_{cols}".ToUpper();
            });
        }

        private string GetDbName(string name, string prefix = "")
        {
            List<string> nameSection = new List<string>();

            var _prefix = prefix.Trim().Trim('_');
            if (!string.IsNullOrEmpty(_prefix))
            {
                nameSection.Add(_prefix);
            }

            Regex reg = new Regex("[a-zA-Z][a-z0-9]*");
            MatchCollection mc = reg.Matches(name);

            for (var i = 0; i < mc.Count; i++)
            {
                nameSection.Add(mc[i].Value);
            }

            return string.Join("_", nameSection).ToUpper();

        }

        public IEnumerable<DbTableObject> TableInfos
        {
            get
            {
                return tableDict.Values;
            }
        }

        public IEnumerable<DbSchemaInfo> Schemas
        {
            get
            {
                return schemaDict.Values;
            }
        }
    }
}
