using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.CodeFirst.Policy
{
    public interface IMigratePolicy
    {
        void Execute();
    }
}
