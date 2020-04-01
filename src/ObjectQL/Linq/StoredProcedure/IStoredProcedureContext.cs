using System.Collections.Generic;
using System.Data;
using ObjectQL.Data;

namespace ObjectQL.Linq.StoredProcedure
{
    public interface IStoredProcedureContext
    {
        IStoredProcedureExcutor SetConnection(string connectionName);
    }
}