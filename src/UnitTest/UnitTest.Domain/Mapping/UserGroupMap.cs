using ObjectQL.Data;
using ObjectQL.Mapping;
namespace ObjectQL.DataExtTests.Mapping
{
    /// <summary>
    /// 
    ///</summary> 
    public class UserGroupMap: EntityMap<UserGroup>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_USER_GROUP"); 

            this.Property(x => x.Id).HasColumnName("ID").IsPrimaryKey();
            this.Property(x => x.CreateDate).HasColumnName("CREATE_DATE").Default("sysdate");
            this.Property(x => x.UserID).HasColumnName("USER_ID");
            this.Property(x => x.GroupID).HasColumnName("GROUP_ID");
        }
    }
}
    