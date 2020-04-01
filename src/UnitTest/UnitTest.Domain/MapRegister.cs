/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：MapRegister
 * 命名空间：ObjectQL.DataExtTests1
 * 文 件 名：MapRegister
 * 创建时间：2017/3/21 16:13:23
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using ObjectQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace ObjectQL.DataExtTests
{
    /// <summary>
    /// 
    /// </summary>
    public class MapRegister : EntityMapRegister
    {
        public override void RegistMap(EntityMapContainer container)
        {
            base.RegistMap(container);

            this.AddMapping<Mapping.UserMap>();
            this.AddMapping<Mapping.RoleMap>();
            this.AddMapping<Mapping.UserRoleMap>();
            this.AddMapping<Mapping.UserOrganMap>();
            this.AddMapping<Mapping.OrganMap>();

            this.AddMapping<Mapping.LoginLogMap>();
            this.AddMapping<Mapping.GroupMap>();
            this.AddMapping<Mapping.UserGroupMap>();

            this.AddMapping<Mapping.EnforcerMap>();
            this.AddMapping<Mapping.AuthUserMap>();
        }
    }

    public class CodeFirstRegister : ObjectQL.CodeFirstRegister
    {
        public override void Configure()
        {
            //AddEntity<ObjectQL.DataExtTests.Models.Student>();
            AddEntity<ObjectQL.DataExtTests.Models.TestColumn>();
        }
    }
}
