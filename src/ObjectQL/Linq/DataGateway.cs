/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataGateway
 * 命名空间：ObjectQL.Data
 * 文 件 名：DataGateway
 * 创建时间：2016/10/20 13:42:33
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using ObjectQL.Data;

namespace ObjectQL
{
    /// <summary>
    /// 数据访问网关
    /// </summary>
    public class DataGateway
    {
        public DataGateway()
        {
            if (!ObjectQLEngine.Initialized)
            {
                throw new InvalidOperationException("ObjectQL 未能正常启动，请检查启动日志。");
            }
        }

        /// <summary>
        /// 获取BaseConnectString的数据库连接实例(IDataAccess)
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public IDataAccess GetBaseDataAccess()
        {
            var result = OrmContext.DriverProviders.GetDataAccess(Consts.BaseConnectStringName);
            if (result == null)
                throw new Exception("配置中没有ConnectionName 为 BaseConnectString的数据库连接");
            return result;
        }
    }
}
