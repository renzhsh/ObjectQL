/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：CommandBuildProvider
 * 命名空间：ObjectQL.Data.Impl
 * 文 件 名：CommandBuildProvider
 * 创建时间：2017/2/14 9:48:31
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ObjectQL.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CommandBuildProvider<TParameter, TDbType> : ICommandBuildProvider
        where TParameter: DbParameter,new() 
    {
        public abstract string AllTableSql
        {
            get;
        }

        public abstract string ConnectFixSymbol
        {
            get;
        }

        public abstract string ParameterPrefix
        {
            get;
        }

        public abstract string CreatePageSql(string commandText, int start, int limit);

        /// <summary>
        /// 获取数据库参数集合
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public IEnumerable<IDataParameter> GetDbParameters(Dictionary<string, object> parameterValues)
        {
            if (parameterValues == null)
            {
                yield break;
            }
            List<IDataParameter> collection = new List<IDataParameter>();
            foreach (var kvp in parameterValues)
            {
                var item = this.MakeIn(kvp.Key, kvp.Value);
                yield return item;
            }
        } 

        /// <summary>
        /// 获取数据库参数集合
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public IEnumerable<IDataParameter> GetDbParameters(IEnumerable<KeyValuePair<string, object>> parameterValues)
        {
            if (parameterValues == null)
            {
                yield break;
            }
            List<IDataParameter> collection = new List<IDataParameter>();
            foreach (var kvp in parameterValues)
            {
                var item = this.MakeIn(kvp.Key, kvp.Value);
                yield return item;
            }
        }

        /// <summary>
        /// 创建输入参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns></returns>
        public IDataParameter MakeIn(string parameterName, object parameterValue)
        {
            var type = GetDbType(parameterValue);
            var parameter = CreateParameter(parameterName, type);  
            parameter.Value = GetDbValue(parameterValue, type);
            return parameter;
        }

        /// <summary>
        /// 创建输入参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="size">参数值长度</param>
        /// <returns></returns>
        public IDataParameter MakeIn(string parameterName, object parameterValue, int size)
        {
            var type = GetDbType(parameterValue);
            var parameter = CreateParameter(parameterName, type, size);
            parameter.Value = GetDbValue(parameterValue, type);
            return parameter;
        }

        /// <summary>
        /// 创建输出参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns></returns>
        public IDataParameter MakeOut(string parameterName, object parameterValue)
        {
            var p =  CreateParameter(parameterName, GetDbType(parameterValue));
            p.Direction = ParameterDirection.Output;
            return p;
        }

        /// <summary>
        /// 创建输出参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="size">参数值长度</param>
        /// <returns></returns>
        public IDataParameter MakeOut(string parameterName, object parameterValue, int size)
        {
            var p = CreateParameter(parameterName, GetDbType(parameterValue), size);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        protected abstract TDbType GetDbType(object value);

        protected abstract Object GetDbValue(object value, TDbType type);

        protected abstract TParameter CreateParameter(string parameterName, TDbType oraType);

        protected abstract TParameter CreateParameter(string parameterName, TDbType oraType, int size);
    }
}
