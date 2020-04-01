using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.Mapping;

namespace ObjectQL
{
    public abstract class CodeFirstRegister : IObjectRegister
    {
        public ConnectionSettings ConnSetting { get; internal set; }

        public abstract void Configure();

        protected void AddEntity<T>() where T : class, new()
        {
            CodeFirst.DbTableObject tableInfo = OrmContext.RelationProvider.Register<T>(this.ConnSetting);

            EntityInfo entityInfo = new EntityInfo(typeof(T), tableInfo.TableName);

            entityInfo.TableInfo.ConnectionSetting = this.ConnSetting;

            foreach (var propName in tableInfo.Columns.Keys)
            {
                var colInfo = tableInfo.Columns[propName];
                EntityPropertyInfo propInfo = entityInfo.GetPropertyInfo(propName);
                propInfo.HasColumn = true;
                propInfo.IsPrimary = colInfo.IsPrimaryKey;
                propInfo.DbFieldInfo.ColumnName = colInfo.ColumnName;
            }

            OrmContext.OrmProvider.AddEntityInfo(entityInfo);
        }
    }
}
