/* *
 * 作者：renzhsh
 * 
 * 
 * 
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace ObjectQL.Data
{
    /// <summary>
    /// 数据库命令参数集合
    /// </summary>
    public class DataParameterCollection : IDisposable, IEnumerable
    {
        private IDataParameterCollection _parameters = null;

        public DataParameterCollection(IDataParameterCollection collection)
        {
            this._parameters = collection;
        }

        /// <summary>
        /// 参数个数
        /// </summary>
        public int Count
        {
            get
            {
                return _parameters.Count;
            }
        }

        /// <summary>
        /// 获取指定名称的数据库命令参数
        /// </summary>
        /// <param name="name">索引指定参数名称</param>
        /// <returns>数据库命令参数</returns>
        public IDataParameter this[string name]
        {
            get
            {
                if (_parameters != null)
                {
                    var result = this._parameters[name];
                    return (IDataParameter)result;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取指定名称的数据库命令参数
        /// </summary>
        /// <param name="index">索引指定位置</param>
        /// <returns>数据库命令参数</returns>
        public IDataParameter this[int index]
        {
            get
            {
                if (_parameters != null)
                {
                    return (IDataParameter)this._parameters[index];
                }
                return null;
            }
        }

        /// <summary>
        /// 检查是否包含某个指定名称的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns></returns>
        public bool Contains(string parameterName)
        {
            if (_parameters == null)
                return false;
            return _parameters.Contains(parameterName);
        }

        /// <summary>
        /// 清除数据库参数
        /// </summary>
        public void Clear()
        {
            if (_parameters != null)
            {
                _parameters.Clear();
            }
        }

        #region IEnumerable成员
        //public IEnumerator<IDataParameter> GetEnumerator()
        //{
        //    return _parameters?.GetEnumerator() as IEnumerator<IDataParameter>;
        //} 
         

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            var result = _parameters?.GetEnumerator();
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters?.GetEnumerator();
        }
        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this._parameters != null)
            {
                this._parameters.Clear();
                this._parameters = null;
            }
        }


        #endregion
    }
}
