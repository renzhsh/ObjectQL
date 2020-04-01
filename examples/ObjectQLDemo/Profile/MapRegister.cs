using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace UserCenter
{
    class MapRegister : EntityMapRegister
    {
        public override void RegistTo(IEntityMapContainer container)
        {
            base.RegistTo(container);

            this.AddMapping<Mapping.UserMap>();
            this.AddMapping<Mapping.RoleMap>();
            this.AddMapping<Mapping.UserRoleMap>();
            this.AddMapping<Mapping.RealUserMap>();
        }
    }
}
