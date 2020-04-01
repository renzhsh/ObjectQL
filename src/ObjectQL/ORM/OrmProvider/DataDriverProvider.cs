/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataAccessTypeMap
 * 命名空间：ObjectQL.Data
 * 文 件 名：DataAccessTypeMap
 * 创建时间：2016/10/20 17:13:15
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ObjectQL.Data;
using Jinhe;

namespace ObjectQL.Mapping
{
    /// <summary>
    /// 数据库访问类实例的映射
    /// </summary>
    public class DataDriverProvider
    {
        protected readonly Dictionary<string, IObjectQLDriverProvider> driverProviderDict = new Dictionary<string, IObjectQLDriverProvider>();

        /// <summary>
        /// 是否初始化完成
        /// </summary>
        internal bool Initialized => driverProviderDict.Count > 0;

        /// <summary>
        /// 创建IDataAccess的实例, 注意：需提供构造函数DataAccess(ConnectionSettings settings)
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public IDataAccess GetDataAccess(ConnectionSettings settings)
        {
            if (!driverProviderDict.Keys.Contains(settings.ProviderName))
            {
                return null;
            }

            return driverProviderDict[settings.ProviderName].GetDataAccess(settings);
        }

        public IDataAccess GetDataAccess(string connectionName)
        {
            // 数据库链接配置
            var connectionStringSettings = Config.GetConnectionSettings(connectionName);
            var settings = new ConnectionSettings(connectionStringSettings);

            return GetDataAccess(settings);
        }

        /// <summary>
        /// 获取数据库连接实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDataAccess GetDataAccess<T>() where T : class, new()
        {
            var setting = OrmContext.OrmProvider.GetEntityInfo<T>().TableInfo.ConnectionSetting;
            return GetDataAccess(setting);
        }

        /// <summary>
        /// SQL命令支持类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICommandBuildProvider GetCommandBuildProvider<T>() where T : class, new()
        {
            var settings = OrmContext.OrmProvider.GetEntityInfo<T>().TableInfo.ConnectionSetting;
            return GetCommandBuildProvider(settings.ProviderName);
        }
        /// <summary>
        /// SQL数据支持类的实例
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public ICommandBuildProvider GetCommandBuildProvider(string providerName)
        {
            if (!driverProviderDict.Keys.Contains(providerName))
            {
                return null;
            }
            return driverProviderDict[providerName].GetCommandBuildProvider();

        }

        public ICommandBuildProvider GetCommandBuildProvider(ConnectionSettings settings)
        {
            return GetCommandBuildProvider(settings.ProviderName);
        }

        public IMigrateProvider GetMigrateProvider<T>() where T : class, new()
        {
            var setting = OrmContext.OrmProvider.GetEntityInfo<T>().TableInfo.ConnectionSetting;
            return GetMigrateProvider(setting);
        }

        public IMigrateProvider GetMigrateProvider(ConnectionSettings settings)
        {
            if (!driverProviderDict.Keys.Contains(settings.ProviderName))
            {
                return null;
            }
            return driverProviderDict[settings.ProviderName].GetMigrateProvider(settings);
        }

        private readonly object _look = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="provider"></param>
        public void Register(string providerName, IObjectQLDriverProvider provider)
        {
            lock (_look)
            {
                if (!driverProviderDict.Keys.Contains(providerName))
                {
                    driverProviderDict.Add(providerName, provider);
                }
            }
        }
    }

}
