/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：CompositeSpecification
 * 命名空间：ObjectQL.Data.Specifications
 * 文 件 名：CompositeSpecification
 * 创建时间：2016/10/19 14:25:05
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

namespace ObjectQL.Specifications
{
    /// <summary>
    /// 复合规约
    /// </summary>
    public abstract class CompositeSpecification<T> : Specification<T>, ICompositeSpecification<T>
    {
        #region 私有字段
        /// <summary>
        /// 左边的语句
        /// </summary>
        protected readonly ISpecification<T> left;
        /// <summary>
        /// 右边的语句
        /// </summary>
        protected readonly ISpecification<T> right;
        #endregion

        /// <summary>
        /// 构造一个CompositeSpecification类型的实例.
        /// </summary>
        /// <param name="left">左边的规约</param>
        /// <param name="right">右边的规约</param>
        public CompositeSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.left = left;
            this.right = right;
        }

        #region ICompositeSpecification<T>的成员
        /// <summary>
        /// 获取左侧（第一个）的规约
        /// </summary>
        public ISpecification<T> Left
        {
            get { return this.left; }
        }
        /// <summary>
        /// 获取右侧（第二个）的规约
        /// </summary>
        public ISpecification<T> Right
        {
            get { return this.right; }
        }
        #endregion
    }
}
