/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ICompositeSpecification
 * 命名空间：ObjectQL.Data.Specifications
 * 文 件 名：ICompositeSpecification
 * 创建时间：2016/10/19 14:27:32
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

namespace ObjectQL.Specifications
{
    /// <summary>
    /// 复合规约接口
    /// </summary>
    public interface ICompositeSpecification<T>
    {
        /// <summary>
        /// 获取左侧（第一个）的规约
        /// </summary>
        ISpecification<T> Left { get; }

        /// <summary>
        /// 获取右侧（第二个）的规约
        /// </summary>
        ISpecification<T> Right { get; }
    }
}
