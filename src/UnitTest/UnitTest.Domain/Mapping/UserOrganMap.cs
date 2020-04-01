/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：UserOrganMap
 * 命名空间：ObjectQL.DataExtTests1.Mapping
 * 文 件 名：UserOrganMap
 * 创建时间：2017/3/21 15:54:19
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
    public class UserOrganMap : EntityMap<UserOrgan>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_USER_ORGAN");

            this.Property(x => x.Id).HasColumnName("ID");
            this.Property(x => x.UserId).HasColumnName("USER_ID");
            this.Property(x => x.OrganId).HasColumnName("ORGAN_ID");
        }
    }
}
