/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：UpdateContent
 * 命名空间：ObjectQL.Data
 * 文 件 名：UpdateContent
 * 创建时间：2017/12/7 11:58:09
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectQL.Linq
{ 
    /// <summary>
    /// 更新的内容
    /// </summary>
    public class UpdateObject<T>
        where T : class, new()
    {
        public UpdateObject()
        {
        }


        public UpdateObject(Expression<Func<T, bool>> where)
        {
            Where = where;
        }

        /// <summary>
        /// 更新的列和值
        /// </summary>
        protected UpdateCriteria<T> UpdateCriteria { set; get; }

        /// <summary>
        /// 更新的Where表达式
        /// </summary>
        public Expression<Func<T, bool>> Where { set; get; }

        public UpdateObject<T> Update(Expression<Func<T, object>> exp, object value)
        {
            if (UpdateCriteria == null)
            {
                UpdateCriteria = new UpdateCriteria<T>();
            }
            UpdateCriteria.Add(exp, value);
            return this;
        }

        public void Update(IDictionary<Expression<Func<T, object>>, object> set)
        {
            if (set == null)
                return;
            foreach (var item in set)
            {
                Update(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal UpdateCriteria<T> GetUpdateCriteria()
        { 
            return UpdateCriteria;
        }
    }
}
