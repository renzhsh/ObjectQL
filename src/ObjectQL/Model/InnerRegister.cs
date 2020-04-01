using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Model
{
    internal class InnerRegister : CodeFirstRegister
    {
        public override void Configure()
        {
            AddEntity<Sequences.SeqGen>();
        }
    }
}
