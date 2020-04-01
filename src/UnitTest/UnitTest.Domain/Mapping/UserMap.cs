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
    public class UserMap : EntityMap<User>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_USER");

            this.Property(x => x.Id).HasColumnName("ID").IsPrimaryKey();
            //.AssociationToForeign<UserRole>(r=>r.UserId);
            this.Property(x => x.PhoneNumberConfirmed).HasColumnName("PHONE_NUMBER_CONFIRMED");
            this.Property(x => x.SecurityStamp).HasColumnName("SECURITY_STAMP");
            this.Property(x => x.Type).HasColumnName("TYPE");
            this.Property(x => x.LockoutEnabled).HasColumnName("LOCKOUT_ENABLED");
            this.Property(x => x.UserName).HasColumnName("USER_NAME");
            this.Property(x => x.Email).HasColumnName("EMAIL");
            this.Property(x => x.Name).HasColumnName("NAME");
            this.Property(x => x.TwoFactorEnabled).HasColumnName("TWO_FACTOR_ENABLED");
            this.Property(x => x.Sex).HasColumnName("SEX");
            this.Property(x => x.PhoneNumber).HasColumnName("PHONE_NUMBER");
            this.Property(x => x.AccessFailedCount).HasColumnName("ACCESS_FAILED_COUNT");
            this.Property(x => x.EmailConfirmed).HasColumnName("EMAIL_CONFIRMED");
            this.Property(x => x.LockoutEndDateUtc).HasColumnName("LOCKOUT_END_DATE_UTC");
            this.Property(x => x.Password).HasColumnName("PASSWORD");
            this.Property(x => x.Status).HasColumnName("STATUS");
            this.Property(x => x.CreateTime).HasColumnName("CREATE_DATE");

            //this.Property(x => x.UserRoles).AssociationTo<User, UserRole>(x => x.Id, y => y.UserId);
            //this.Property(x => x.UserOrgans).AssociationTo<User, UserOrgan>(x => x.Id, y => y.UserId);
        }
    }
}
