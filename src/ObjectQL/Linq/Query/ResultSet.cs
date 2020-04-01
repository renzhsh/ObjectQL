/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ResultSet
 * 命名空间：ObjectQL.Data
 * 文 件 名：ResultSet
 * 创建时间：2018/3/28 15:06:28
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;

namespace ObjectQL.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class ResultSet<T> : IResultSet<T>
    {
        private IDataAccess _db;
        private IEnumerable<T> _data;
        private Func<IDataAccess, int> _countFunc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="data"></param>
        public ResultSet(IDataAccess db, IEnumerable<T> data)
        {
            _db = db;
            _data = data;
        }

        internal void SetTotal(Func<IDataAccess, int> func) {
            _countFunc = func;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data?.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Total
        {
            get
            {
                if (_countFunc!=null)
                {
                    return _countFunc(this._db);
                }
                return 0;
            }
        }

        public IEnumerable<T> Data
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
