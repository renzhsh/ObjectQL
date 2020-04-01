using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace UserCenter.Mapping
{
    public class UserRoleMap : EntityMap<Model.UserRole>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_USER_ROLE");

            this.Property(x => x.UserRoleID).HasColumnName("ID").IsPrimaryKey();
            this.Property(x => x.CreateDate).HasColumnName("CREATE_DATE").Default("sysdate");
            this.Property(x => x.RoleID).HasColumnName("ROLE_ID");
            this.Property(x => x.UserID).HasColumnName("USER_ID");

        }
    }
}
