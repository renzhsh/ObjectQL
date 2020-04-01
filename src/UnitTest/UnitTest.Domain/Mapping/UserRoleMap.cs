/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：UserRoleMap
 * 命名空间：ObjectQL.DataExtTests1.Mapping
 * 文 件 名：UserRoleMap
 * 创建时间：2017/3/21 15:16:56
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using ObjectQL.Data;
using ObjectQL.DataExtTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace ObjectQL.DataExtTests.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRoleMap : EntityMap<UserRole>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_USER_ROLE");
            this.Property(x => x.Id).HasColumnName("USER_ROLE_ID").IsPrimaryKey();
            this.Property(x => x.RoleId).HasColumnName("ROLE_ID");
            this.Property(x => x.UserId).HasColumnName("USER_ID");
        }

    }
}
