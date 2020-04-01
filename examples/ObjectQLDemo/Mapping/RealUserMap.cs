using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace UserCenter.Mapping
{
    public class RealUserMap : EntityMap<Model.RealUser>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_REAL_USER");

            this.Property(x => x.UserID).HasColumnName("USER_ID").IsPrimaryKey();
            this.Property(x => x.RealUserID).HasColumnName("REAL_USER_ID");
            this.Property(x => x.CreateDate).HasColumnName("CREATE_DATE").Default("sysdate");
            this.Property(x => x.State).HasColumnName("STATE");
            this.Property(x => x.IDCardNum).HasColumnName("ID_CARD_NUM");
            this.Property(x => x.RealName).HasColumnName("REAL_NAME");
        }
    }
}
