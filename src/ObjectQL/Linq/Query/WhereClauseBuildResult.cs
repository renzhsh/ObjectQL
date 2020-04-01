/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：WhereClauseBuildResult
 * 命名空间：ObjectQL.Data
 * 文 件 名：WhereClauseBuildResult
 * 创建时间：2016/10/19 16:06:28
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class WhereClauseBuildResult
    {
        /// <summary>
        /// 
        /// </summary>
        public WhereClauseBuildResult() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause">where子句的文本内容</param>
        /// <param name="parameterValues">where子句中的参数</param>
        public WhereClauseBuildResult(string whereClause, Dictionary<string, object> parameterValues)
        {
            WhereClause = whereClause;
            ParameterValues = parameterValues;
        }

        /// <summary>
        /// 
        /// </summary>
        public string WhereClause { get; set; }

        /// <summary>
        /// 参数的名值对
        /// </summary>
        public Dictionary<string, object> ParameterValues { get; set; }

        private List<KeyValuePair<string, object>> _parameterValueList;

        /// <summary>
        /// 
        /// </summary>
        public List<KeyValuePair<string, object>> ParameterValueList
        {
            get
            {
                if (ParameterValues != null)
                    _parameterValueList = ParameterValues.ToList();
                return _parameterValueList;
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(WhereClause);
            sb.Append(Environment.NewLine);
            this.ParameterValueList?.ForEach(kvp =>
            {
                sb.Append(string.Format("{0} = [{1}] (Type: {2})", kvp.Key, kvp.Value.ToString(), kvp.Value.GetType().FullName));
                sb.Append(Environment.NewLine);
            });
            return sb.ToString();
        }
    }
}
