/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：EntityMapRegister
 * 命名空间：ObjectQL.Data
 * 文 件 名：EntityMapRegister
 * 创建时间：2016/10/20 21:01:25
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/


namespace ObjectQL.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EntityMapRegister : IObjectRegister
    {
        private EntityMapContainer _container = null;

        /// <summary>
        /// 新增映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void AddMapping<T>() where T : IEntityMap, new()
        {
            _container.AddMapping<T>();
        }

        /// <summary>
        /// 注册实体容器
        /// </summary>
        /// <param name="container"></param>
        public virtual void RegistMap(EntityMapContainer container)
        {
            _container = container;
        }
    }
}
