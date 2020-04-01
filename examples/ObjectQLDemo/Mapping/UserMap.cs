using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace UserCenter.Mapping
{
    public class UserMap:EntityMap<Model.User>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_USER");

            this.Property(x => x.Id).HasColumnName("ID").IsPrimaryKey();
            this.Property(x => x.CreateDate).HasColumnName("CREATE_DATE").Default("sysdate");
            //this.Property(x => x.PhoneNumberConfirmed).HasColumnName("PHONE_NUMBER_CONFIRMED");
            //this.Property(x => x.SecurityStamp).HasColumnName("SECURITY_STAMP");
            //this.Property(x => x.Type).HasColumnName("TYPE");
            //this.Property(x => x.LockoutEnabled).HasColumnName("LOCKOUT_ENABLED");
            this.Property(x => x.UserName).HasColumnName("USER_NAME");
            this.Property(x => x.Email).HasColumnName("EMAIL");
            this.Property(x => x.Name).HasColumnName("NAME");
            //this.Property(x => x.TwoFactorEnabled).HasColumnName("TWO_FACTOR_ENABLED");
            this.Property(x => x.Sex).HasColumnName("SEX");
            this.Property(x => x.PhoneNumber).HasColumnName("PHONE_NUMBER");
            //this.Property(x => x.AccessFailedCount).HasColumnName("ACCESS_FAILED_COUNT");
            //this.Property(x => x.EmailConfirmed).HasColumnName("EMAIL_CONFIRMED");
            //this.Property(x => x.LockoutEndDateUtc).HasColumnName("LOCKOUT_END_DATE_UTC");
            this.Property(x => x.Password).HasColumnName("PASSWORD");
            //this.Property(x => x.Status).HasColumnName("STATUS");

        }
    }
}
