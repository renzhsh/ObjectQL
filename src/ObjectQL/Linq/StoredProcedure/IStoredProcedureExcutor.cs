using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using System.Data;

namespace ObjectQL.Linq.StoredProcedure
{
    /// <summary>
    /// 执行存储过程
    /// </summary>
    public interface IStoredProcedureExcutor
    {
        /// <summary>
        /// 执行数据库存储过程,返回结果的第一行第一列
        /// </summary>
        /// <returns></returns>
        DataReader GetDataReader();

        /// <summary>
        /// 执行数据库存储过程,返回一个数据集
        /// </summary>
        /// <returns></returns>
        DataSetResult GetDataSetResult();

        /// <summary>
        /// 执行数据库存储过程,返回一个数据表
        /// </summary>
        /// <returns></returns>
        DataTableResult GetDataTableResult();

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <returns></returns>
        NonQueryResult GetNonQueryResult();

        /// <summary>
        /// 执行数据库存储过程,返回结果的第一行第一列
        /// </summary>
        /// <returns></returns>
        ScalerResult GetScalerResult();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDataParameter> OutOrReturnParameters();
    }
}
