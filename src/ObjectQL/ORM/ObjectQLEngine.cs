using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jinhe;
using ObjectQL.Data;
using ObjectQL.Mapping;

namespace ObjectQL
{
    public class ObjectQLEngine
    {
        private ObjectQLEngine() { }
        public static void Startup()
        {
            if (Initialized)
            {
                Logger.Info("ObjectEngine has started.");
                return;
            }

            Logger.Info("ObjectEngine starting...");

            var engine = new ObjectQLEngine();

            if (ObjectQLSection.Instance.drivers == null || ObjectQLSection.Instance.drivers.Count == 0)
            {
                Logger.Fatal("未提供ObjectQL的驱动程序(IObjectQLDriverProvider)");
                return;
            }

            engine.RegisterDriverProvider(ObjectQLSection.Instance.drivers);

            if (!OrmContext.DriverProviders.Initialized)
            {
                Logger.Fatal("不存在有效的驱动程序(IObjectQLDriverProvider)");
                return;
            }

            engine.InitOrmRegister();

            Initialized = true;

            Logger.Info("ObjectEngine started.");

            CodeFirst.MigrateEngine.Mirgate();
            Logger.Info("MigrateEngine started.");
        }

        public static bool Initialized { get; private set; }

        private void RegisterDriverProvider(DriverAccessCollection drivers)
        {
            for (var i = 0; i < drivers.Count; i++)
            {
                var item = drivers[i];

                var type = Type.GetType(item.Provider);
                try
                {
                    if (type == null)
                    {
                        throw new Exception($"未能加载类型{item.Provider}");
                    }
                    else
                    {
                        var provider = Activator.CreateInstance(type) as IObjectQLDriverProvider;
                        OrmContext.DriverProviders.Register(item.Name, provider);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"注册驱动程序{type?.FullName}时出现异常, {ex.Message}");
                }
            }

        }

        /// <summary>
        /// 对象关系映射的配置初始化
        /// </summary>
        /// <returns></returns>
        private void InitOrmRegister()
        {
            OrmContext.OrmProvider.Clear();
            // 配置的容器计数
            var containerCount = ObjectQLSection.Instance.containers.Count;
            for (var index = 0; index < containerCount; index++)
            {
                // 容器配置
                var containerConfig = ObjectQLSection.Instance.containers[index];

                var connectionStringSettings = Config.GetConnectionSettings(containerConfig.ConnectionKey);
                var settings = new ConnectionSettings(connectionStringSettings);

                var mapSettingsCount = containerConfig.Count;
                for (var i = 0; i < mapSettingsCount; i++)
                {
                    // 获取Register的类
                    var type = Type.GetType(containerConfig[i].Name);
                    if (type == null)
                    {
                        throw new Exception($"未能加载类型{containerConfig[i].Name}");
                    }
                    else
                    {
                        if (type.IsSubclassOf(typeof(EntityMapRegister)))
                        {
                            ResolveMappingRegister(settings, type);
                        }
                        if (type.IsSubclassOf(typeof(CodeFirstRegister)))
                        {
                            ResolveCodeFirstRegister(containerConfig, settings, type);
                        }
                    }
                }
            }

            InitInnerModel();
        }

        private void ResolveMappingRegister(ConnectionSettings connSetting, Type registerType)
        {
            EntityMapContainer container = new EntityMapContainer(connSetting);

            var register = Activator.CreateInstance(registerType) as EntityMapRegister;
            register?.RegistMap(container);
        }

        private void ResolveCodeFirstRegister(MapContainer config, ConnectionSettings connSetting, Type registerType)
        {

            OrmContext.RelationProvider.AddMapContainer(config);
            CodeFirstRegister register = Activator.CreateInstance(registerType) as CodeFirstRegister;
            if (register != null)
            {
                register.ConnSetting = connSetting;
                register?.Configure();
            }
        }

        private void InitInnerModel()
        {
            var connectionStringSettings = Config.GetConnectionSettings(Consts.BaseConnectStringName);
            var settings = new ConnectionSettings(connectionStringSettings);

            var register = new Model.InnerRegister
            {
                ConnSetting = settings
            };

            register.Configure();
        }
    }
}
