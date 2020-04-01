/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：EntityMapContainer
 * 命名空间：ObjectQL.Data
 * 文 件 名：EntityMapContainer
 * 创建时间：2016/10/20 15:17:53
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Data;
using ObjectQL.CodeFirst;

namespace ObjectQL.Mapping
{
    /// <summary>
    /// 实体ORM映射配置容器
    /// </summary>
    public class EntityMapContainer
    {
        /// <summary>
        /// 实体容器
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="provider"></param>
        public EntityMapContainer(ConnectionSettings setting)
        {
            connSetting = setting;
        }

        private ConnectionSettings connSetting { get; }

        /// <summary>
        /// 数据库的解决方案/表空间
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 新增新的映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddMapping<T>() where T : IEntityMap, new()
        {
            var map = new T();
            map.Mapping();
            OrmContext.OrmProvider.AddEntityInfo(map.EntityInfo);
            map.EntityInfo.TableInfo.ConnectionSetting = this.connSetting;
        }

        public void AddEntity<T>() where T : class, new()
        {
            
        }
    }
}
