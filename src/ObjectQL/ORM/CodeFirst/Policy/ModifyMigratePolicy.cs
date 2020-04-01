using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.CodeFirst.Policy
{
    /// <summary>
    /// 模型修正策略
    /// </summary>
    public class ModifyMigratePolicy : BaseMigratePolicy
    {
        public ModifyMigratePolicy(DbSchemaInfo schema) : base(schema) { }

        private IEnumerable<DbChange> NewTables { get; set; }

        private IEnumerable<DbChange> ChangeTables { get; set; }

        public override void Execute()
        {
            var changes = GetTableChanges();
            NewTables = changes.Where(change => change.NewTable);
            ChangeTables = changes.Where(change => !change.NewTable && change.Changed);

            Logger.Info($"==============Modify Migrate Policy==============");

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
                    Logger.Error($"创建表 {change.Table.TableName} 时发生异常：{ex.Message}");
                }
            });

            Parallel.ForEach(ChangeTables, change =>
            {
                try
                {
                    Logger.Info($"修改表 {change.Table.TableName}");

                    Logger.Info(change.ToString());

                    if (change.DeleteConstraint)
                    {
                        change.ConstraintDeletes.ToList().ForEach(con =>
                        {
                            try
                            {
                                MigrateProvider.DropConstraint(change.Table.TableName, con.Name);
                                Logger.Info($"删除约束 {con.Name}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"删除约束 {con.Name} 时发生异常：{ex.Message}");
                            }

                        });
                    }
                    if (change.ChangeConstraint)
                    {
                        change.ConstraintChanges.ToList().ForEach(con =>
                        {
                            try
                            {
                                MigrateProvider.DropConstraint(change.Table.TableName, con.Name);
                                Logger.Info($"删除约束 {con.Name}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"删除约束 {con.Name} 时发生异常：{ex.Message}");
                            }

                        });
                    }
                    if (change.NewColumn)
                    {
                        change.ColumnNews.ToList().ForEach(col =>
                        {
                            try
                            {
                                MigrateProvider.AddColumn(change.Table.TableName, col);
                                Logger.Info($"新建字段 {col.ColumnName}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"新建字段 {col.ColumnName} 时发生异常：{ex.Message}");
                            }
                        });

                    }
                    if (change.ChangeColumn)
                    {
                        change.ColumnChanges.ToList().ForEach(col =>
                        {
                            try
                            {
                                MigrateProvider.AlterColumn(change.Table.TableName, col);
                                Logger.Info($"修改字段 {col.Column.ColumnName}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"修改字段 {col.Column.ColumnName} 时发生异常：{ex.Message}");
                            }
                        });
                    }
                    if (change.DeleteColumn)
                    {
                        change.ColumnDeletes.ToList().ForEach(col =>
                        {
                            try
                            {
                                MigrateProvider.DropColumn(change.Table.TableName, col.ColumnName);
                                Logger.Info($"删除字段 {col.ColumnName}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"删除字段 {col.ColumnName} 时发生异常：{ex.Message}");
                            }
                        });
                    }
                    if (change.ChangeConstraint)
                    {
                        change.ConstraintChanges.ToList().ForEach(con =>
                        {
                            try
                            {
                                MigrateProvider.CreateConstraint(change.Table.TableName, con);
                                Logger.Info($"创建约束 {con.Name}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"创建约束 {con.Name} 时发生异常：{ex.Message}");
                            }

                        });
                    }
                    if (change.NewConstraint)
                    {
                        change.ConstraintNews.ToList().ForEach(con =>
                        {
                            try
                            {
                                MigrateProvider.CreateConstraint(change.Table.TableName, con);
                                Logger.Info($"创建约束 {con.Name}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"创建约束 {con.Name} 时发生异常：{ex.Message}");
                            }
                        });
                    }

                    Logger.Info($"修改表 {change.Table.TableName} 完成");
                }
                catch (Exception ex)
                {
                    Logger.Info($"修改表 {change.Table.TableName} 时发生异常：{ex.Message}");
                }
            });
            Logger.Info($"==============Modify Migrate End=================");
        }
    }
}
