/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 接口名称：IEntityMap
 * 命名空间：ObjectQL.Data
 * 文 件 名：IEntityMap
 * 创建时间：2016/10/20 13:04:06
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Linq.Expressions;

namespace ObjectQL.Mapping
{
    /// <summary>
    /// 实体关系映射
    /// </summary>
    public interface IEntityMap
    {
        /// <summary>
        /// 实体的关系映射信息
        /// </summary>
        EntityInfo EntityInfo { get; }

        /// <summary>
        /// 映射配置
        /// </summary>
        void Mapping();

        /// <summary>
        /// 实体表达式的属性信息
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        EntityPropertyMap Property<T, TResult>(Expression<Func<T, TResult>> expression) where T : class;

        /// <summary>
        /// 实体映射的表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEntityMap ToTable<T>(string name) where T : class;
    }
}
