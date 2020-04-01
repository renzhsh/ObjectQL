using ObjectQL.Data;
using ObjectQL.Mapping;
namespace ObjectQL.DataExtTests.Mapping
{
    /// <summary>
    /// 用户组
    ///</summary> 
    public class GroupMap : EntityMap<Group>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_GROUP");

            this.Property(x => x.GroupID).HasColumnName("GROUP_ID").IsPrimaryKey();
            this.Property(x => x.GroupName).HasColumnName("GROUP_NAME");
        }
    }
}
