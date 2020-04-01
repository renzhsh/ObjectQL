/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataRelation
 * 命名空间：ObjectQL.Data
 * 文 件 名：DataRelation
 * 创建时间：2017/3/22 14:29:20
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ObjectQL.Mapping;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 连接类型
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// 等值连接
        /// </summary>
        Inner,
        /// <summary>
        /// 左连接
        /// </summary>
        Left,
        ///// <summary>
        ///// 右连接
        ///// </summary>
        //Right
    }
    ///// <summary>
    ///// 字段关联关系
    ///// </summary>
    //internal class FieldRelation
    //{
    //    private DbFieldInfo _first;
    //    private DbFieldInfo _second;
    //    private JoinType _joinType; 
    //    private string _relationTableKey = string.Empty;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="first"></param>
    //    /// <param name="second"></param>
    //    /// <param name="jonType"></param>
    //    public FieldRelation(DbFieldInfo first, DbFieldInfo second, JoinType jonType)
    //    {
    //        this._first = first;
    //        this._second = second;
    //        this._joinType = jonType;
    //    }

    //    /// <summary>
    //    /// JOIN类型
    //    /// </summary>
    //    public JoinType JoinType {
    //        get {
    //            return this._joinType;
    //        }
    //    }

    //    /// <summary>
    //    /// Second被关联的顺序号
    //    /// </summary>
    //    public int RelationSn
    //    {
    //        set; get;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string RelationTableKey
    //    {
    //        get
    //        {
    //            if (string.IsNullOrEmpty(_relationTableKey))
    //                _relationTableKey = $"{this._first.TableName}:{this._second.TableName}";
    //            return _relationTableKey;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string RelationFieldKey
    //    {
    //        get
    //        {
    //            if (string.IsNullOrEmpty(_relationTableKey))
    //                _relationTableKey = $"{this._first.TableName}:{this._first.ColumnName}:{this._second.TableName}:{this._second.ColumnName}";
    //            return _relationTableKey;
    //        }
    //    }

    //    /// <summary>
    //    /// 原始表名
    //    /// </summary>
    //    protected string JoinOriginTable
    //    {
    //        get
    //        {
    //            return this._second.TableName;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string JoinTable
    //    {
    //        get
    //        {
    //            if (this.RelationSn > 0)
    //            {
    //                return $"{this.JoinOriginTable}{this.RelationSn}";
    //            }
    //            return this.JoinOriginTable;
    //        }
    //    }

    //    internal string JoinOn
    //    {
    //        get
    //        {
    //            string result;
    //            if (this._joinType == JoinType.Inner) 
    //                result = $" {this._joinType} JOIN {this.JoinOriginTable} {this.JoinTable} ON {this.RelationClause}";
    //            else
    //                result = $" {this._joinType} JOIN {this.JoinOriginTable} {this.JoinTable} ON {this.RelationClause}";
    //            return result;
    //        }
    //    } 

    //    /// <summary>
    //    /// 关联子语句
    //    /// </summary>
    //    internal string ExistsClause
    //    {
    //        get
    //        {
    //            var result = $"SELECT {_second.ToString(this.JoinTable)} FROM  {this.JoinOriginTable} {this.JoinTable} WHERE {RelationClause}"; 
    //            return result;
    //        }
    //    }

    //    /// <summary>
    //    /// 字段关联子语句
    //    /// </summary>
    //    internal string RelationClause {
    //        get {
    //            return $"{_first.ToString()}={_second.ToString(this.JoinTable)}"; 
    //        }
    //    }
    //}
}
