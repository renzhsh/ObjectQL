/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：RoleMap
 * 命名空间：ObjectQL.DataExtTests1.Mapping
 * 文 件 名：RoleMap
 * 创建时间：2017/3/21 15:16:24
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
    public class RoleMap : EntityMap<Role>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_ROLE");

            this.Property(x => x.RoleID).HasColumnName("ROLE_ID").IsPrimaryKey();
            //.AssociationToForeign<UserRole>(r=>r.RoleId);
            this.Property(x => x.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(x => x.Type).HasColumnName("TYPE");
            this.Property(x => x.RoleDesc).HasColumnName("ROLE_DESC");
            this.Property(x => x.State).HasColumnName("STATE");
            this.Property(x => x.RoleName).HasColumnName("ROLE_NAME");
        }
    }
}
