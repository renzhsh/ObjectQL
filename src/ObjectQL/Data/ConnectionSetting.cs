using System;
using System.Configuration;

namespace ObjectQL.Data
{
    /// <summary>
    /// 连接配置 
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public ConnectionSettings() { }

        /// <summary>
        /// 数据库连接配置
        /// </summary>
        /// <param name="settings"></param>
        public ConnectionSettings(ConnectionStringSettings settings)
        {
            this.ConnectionName = settings.Name;
            this.ConnectionString = settings.ConnectionString;
            this.ProviderName = settings.ProviderName;
        }

        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectionName { get; set; }


        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }


        /// <summary>
        /// 数据类型
        /// </summary>
        public String ProviderName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var target = (ConnectionSettings)obj;
            return ConnectionName.Equals(target.ConnectionName);
        }

        public override int GetHashCode()
        {
            return ConnectionName.GetHashCode();
        }

        public static bool operator ==(ConnectionSettings x, ConnectionSettings y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ConnectionSettings x, ConnectionSettings y)
        {
            return !x.Equals(y);
        }

    }
}
