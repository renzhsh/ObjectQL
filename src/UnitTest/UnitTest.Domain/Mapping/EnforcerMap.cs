using ObjectQL.Data;
using ObjectQL.DataExtTests.Models;
using ObjectQL.Mapping;

namespace ObjectQL.DataExtTests.Mapping
{
    /// <summary>
    /// 
    ///</summary> 
    public class EnforcerMap : EntityMap<Enforcer>
    {
        public override void Mapping()
        {
            this.ToTable("LAW_ENFORCER");


            this.Property(x => x.UserID).HasColumnName("USER_ID").IsPrimaryKey();
            
        }
    }
}