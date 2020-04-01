using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace UserCenter.Mapping
{
    public class RoleMap:EntityMap<Model.Role>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_ROLE");

            this.Property(x => x.RoleID).HasColumnName("ID").IsPrimaryKey();
            this.Property(x => x.CreateDate).HasColumnName("CREATE_DATE").Default("sysdate");
            this.Property(x => x.ParentID).HasColumnName("PARENT_ID");
            this.Property(x => x.AppID).HasColumnName("APP_ID");
            this.Property(x => x.Type).HasColumnName("TYPE");
            this.Property(x => x.RoleDesc).HasColumnName("ROLE_DESC");
            this.Property(x => x.IsLeaf).HasColumnName("IS_LEAF");
            this.Property(x => x.Path).HasColumnName("PATH");
            this.Property(x => x.RoleName).HasColumnName("ROLE_NAME");
            this.Property(x => x.Expanded).HasColumnName("EXPANDED");
            this.Property(x => x.Sort).HasColumnName("SORT");
            this.Property(x => x.Lvl).HasColumnName("LVL");
            this.Property(x => x.CreateUserID).HasColumnName("CREATE_USER_ID");
            this.Property(x => x.State).HasColumnName("STATE");
            this.Property(x => x.IconCls).HasColumnName("ICON_CLS");
            this.Property(x => x.BanDel).HasColumnName("BAN_DEL");
        }
    }
}
