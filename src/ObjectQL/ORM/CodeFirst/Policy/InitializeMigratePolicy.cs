using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;

namespace ObjectQL.CodeFirst.Policy
{
    /// <summary>
    /// 模型初始化策略
    /// </summary>
    public class InitializeMigratePolicy : BaseMigratePolicy
    {
        public InitializeMigratePolicy(DbSchemaInfo schema) : base(schema) { }

        private IEnumerable<DbChange> NewTables { get; set; }

        private IEnumerable<DbChange> ChangeTables { get; set; }

        public override void Execute()
        {
            var changes = GetTableChanges();
            NewTables = changes.Where(change => change.NewTable);
            ChangeTables = changes.Where(change => !change.NewTable && change.Changed);

            Logger.Info($"==============Initialize Migrate Policy==============");
            Logger.Info($"NewTables:{NewTables.Count()}, ChangeTables:{ChangeTables.Count()}");

            Parallel.ForEach(NewTables, change =>
            {
                try
                {
                    MigrateProvider.CreateTable(change.Table);
                    change.Table.Constraints.ForEach(con =>
                    {
                        MigrateProvider.CreateConstraint(change.Table.TableName, con);
                    });

                    Logger.Info($"创建表 {change.Table.TableName} 完成");
                }
                catch (Exception ex)
                {
                    Logger.Info($"创建表 {change.Table.TableName} 时发生异常：{ex.Message}");
                }
            });

            Parallel.ForEach(ChangeTables, change =>
            {
                try
                {
                    MigrateProvider.DropTable(change.Table.TableName);
                    MigrateProvider.CreateTable(change.Table);
                    change.Table.Constraints.ForEach(con =>
                    {
                        MigrateProvider.CreateConstraint(change.Table.TableName, con);
                    });

                    Logger.Info($"重置表 {change.Table.TableName} 完成");
                }
                catch(Exception ex)
                {
                    Logger.Info($"重置表 {change.Table.TableName} 时发生异常：{ex.Message}");
                }
            });
            Logger.Info($"==============Initialize Migrate End=================");
        }
    }
}
