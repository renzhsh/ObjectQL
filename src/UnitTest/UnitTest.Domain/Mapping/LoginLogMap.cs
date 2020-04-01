using ObjectQL.Data;
using ObjectQL.Mapping;
namespace ObjectQL.DataExtTests.Mapping
{
    /// <summary>
    /// 用户登录日志
    ///</summary> 
    public class LoginLogMap: EntityMap<ObjectQL.DataExtTests.Models.LoginLog>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_LOGIN_LOG"); 
            
            this.Property(x => x.LogID).HasColumnName("LOG_ID");
            this.Property(x => x.LogID).IsPrimaryKey();
            this.Property(x => x.Content).HasColumnName("CONTENT");
            this.Property(x => x.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(x => x.UserID).HasColumnName("USER_ID");
        }
    }
}
    