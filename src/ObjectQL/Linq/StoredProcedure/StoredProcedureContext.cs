/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ProcedureContext
 * 命名空间：ObjectQL.Data
 * 文 件 名：ProcedureContext
 * 创建时间：2018/4/11 14:20:58
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ObjectQL.Data;
using ObjectQL.Mapping;

namespace ObjectQL.Linq.StoredProcedure
{
    /// <summary>
    /// 
    /// </summary>
    public class StoredProcedureContext : IStoredProcedureContext, IStoredProcedureExcutor
    {
        private readonly string _procedure;
        private readonly object[] _ps;

        private string _connectionName;

        /// <summary>
        /// 存储过程的执行上下文
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="ps"></param>
        public StoredProcedureContext(string procedure, params object[] ps)
        {
            _procedure = procedure;
            _ps = ps;
        }


        /// <summary>
        /// 执行数据库存储过程,返回一个数据读取器
        /// </summary>
        /// <returns></returns>
        public DataReader GetDataReader() => GetDb().ExecuteProcReader(_procedure, _ps);

        /// <summary>
        /// 执行数据库存储过程,返回结果的第一行第一列
        /// </summary>
        /// <returns></returns>
        public ScalerResult GetScalerResult() => GetDb().ExecuteProcScalar(_procedure, _ps);

        /// <summary>
        /// 执行数据库存储过程,返回一个数据表
        /// </summary>
        /// <returns></returns>
        public DataTableResult GetDataTableResult() => GetDb().ExecuteProcDataTable(_procedure, _ps);

        /// <summary>
        /// 执行数据库存储过程,返回一个数据集
        /// </summary>
        /// <returns></returns>
        public DataSetResult GetDataSetResult() => GetDb().ExecuteProcDataSet(_procedure, _ps);

        /// <summary>
        /// 执行数据库存储过程,返回受影响的行数
        /// </summary>
        /// <returns></returns>
        public NonQueryResult GetNonQueryResult() => GetDb().ExecuteProcNonQuery(_procedure, _ps);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IDataAccess GetDb()
        {
            return OrmContext.DriverProviders.GetDataAccess(_connectionName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public IStoredProcedureExcutor SetConnection(string connectionName)
        {
            _connectionName = connectionName;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDataParameter> OutOrReturnParameters()
        {
            var ps = GetScalerResult().Parameters;
            if (ps == null)
                yield break;
            foreach (var x in ps)
            {
                var item = x as IDataParameter;
                if (item.Direction == ParameterDirection.Input)
                    continue;
                yield return item;
            }
        }
    }
}
