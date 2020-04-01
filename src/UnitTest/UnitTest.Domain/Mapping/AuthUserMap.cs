/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：UserMap
 * 命名空间：ObjectQL.DataExtTests1.Mapping
 * 文 件 名：UserMap
 * 创建时间：2017/3/21 15:15:45
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
    public class AuthUserMap : EntityMap<AuthUser>
    {
        public override void Mapping()
        {
            this.ToTable("AUTH_USER");

            this.Property(x => x.UserID).HasColumnName("USER_ID").IsPrimaryKey();
            this.Property(x => x.UserName).HasColumnName("USER_NAME");
            this.Property(x => x.Password).HasColumnName("PASSWORD");
        }
    }
}
