/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：PageData
 * 命名空间：Jinhe
 * 文 件 名：PageData
 * 创建时间：2017/2/10 10:23:40
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinhe
{
    /// <summary>
    /// 分页数据
    /// </summary>
    [DataContract]
    [KnownType("GetknownTypes")]
    public class PageData<T> 
    {
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetknownTypes()
        {
           yield return typeof(PageData<T>);
        }

        private IEnumerable<T> _listData;
        /// <summary>
        /// 当前页数据集
        /// </summary> 
        [DataMember]
        public IEnumerable<T> Rows
        {
            set
            {
                _listData = value;
            }
            get
            {
                return _listData;
            }
        }

        /// <summary>
        /// 当前页数据集
        /// </summary>
        [Obsolete("Items属性已经过时，请使用Rows属性")]
        public IEnumerable<T> Items
        {
            set
            {
                _listData = value;
            }
            get
            {
                return _listData;
            }
        }

        /// <summary>
        /// 当且页数据集的开始索引
        /// </summary> 
        public int Start { set; get; }

        /// <summary>
        /// 当且页的长度
        /// </summary> 
        public int Length { set; get; }

        private int _total = 0;

        /// <summary>
        /// 总记录数
        /// </summary> 
        [DataMember]
        public int Total
        {
            set
            {
                if (this.Rows != null && value < Rows.Count())
                {
                    throw new ASoftException("Total小于Rows的长度，未正确设置分页数据的Total");
                }
                _total = value;
            }
            get
            {
                if (this.Rows != null && _total < Rows.Count())
                {
                    throw new ASoftException("Total小于Rows的长度，未正确设置分页数据的Total");
                }
                return _total;
            }
        }
    }
}
