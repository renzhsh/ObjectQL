/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataTableResult
 * 命名空间：ObjectQL.Data
 * 文 件 名：DataTableResult
 * 创建时间：2018/4/11 14:04:27
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using Jinhe;

namespace ObjectQL.Data
{
    #region DataTableResult
    /// <summary>
    /// 执行CommandAdapter对象填充的表结果
    /// </summary>
    public class DataTableResult : ResultBase, IListSource, IDisposable
    {
        private StringBuilder sbdesc = null;
        private DataTable value;
        /// <summary>
        /// 数据库命令执行后的返回值
        /// </summary>
        public DataTable Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 使用一个数据库命令的执行后的返回值和数据库命令初始化对象
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="command">命令</param>
        public DataTableResult(DataTable result, IDbCommand command)
            : base(command)
        {
            this.value = result;
            if (LogAdapter.CanDebug)
            {
                LogAdapter.Db.Debug(this.Description);
            }
        }

        public DataTableResult(DataTable result, string commandText, CommandType commandType, IDataParameterCollection parameters)
            : base(commandText, commandType, parameters)
        {
            value = result;
        }

        public DataTableResult(DataTable result, string commandText, CommandType commandType, DataParameterCollection parameters)
            : base(commandText, commandType, parameters)
        {
            this.value = result;
        }

        /// <summary>
        /// 结果描述
        /// </summary>
        public override string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库返回结果信息如下");
                    sbdesc.AppendLine("类型类型:" + CommandType);
                    sbdesc.AppendLine("命令文本:" + CommandText);
                    int max = 0;
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        sbdesc.AppendLine("参数列表:");
                        foreach (IDataParameter p in Parameters)
                        {
                            max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                        }
                        foreach (IDataParameter p in Parameters)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                        }
                    }
                    max = 0;
                    sbdesc.AppendLine("结果行数:" + value.Rows.Count.ToString());
                    sbdesc.AppendLine("各列类型:");
                    foreach (DataColumn dc in value.Columns)
                    {
                        max = dc.ColumnName.Length > max ? dc.ColumnName.Length : max;
                    }
                    foreach (DataColumn dc in value.Columns)
                    {
                        sbdesc.AppendLine(string.Format("{0} : {1}", dc.ColumnName.PadRight(max, ' '), dc.DataType.ToString().PadRight(11, ' ')));
                    }
                }
                return sbdesc.ToString();
            }
        }

        #region IDisposable Members

        /// <summary>
        /// 释放资源
        /// </summary>
        void IDisposable.Dispose()
        {
            this.value.Dispose();
            base.Dispose();
        }

        #endregion

        #region IListSource Members

        /// <summary>
        /// 集合是 IList 对象集合
        /// </summary>
        public bool ContainsListCollection
        {
            get { return (this.value as IListSource).ContainsListCollection; }
        }

        /// <summary>
        /// 返回一个数据绑定控件可用的数据源
        /// </summary>
        /// <returns>数据源</returns>
        public IList GetList()
        {
            return (this.value as IListSource).GetList();
        }


        #endregion
    }
    #endregion

    
}
