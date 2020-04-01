using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Linq.StoredProcedure;

namespace ObjectQL
{
    public static class StoredProcedureExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ps"></param>
        public static IStoredProcedureExcutor DefaultStoredProcedure(this DataGateway gateway, string name, params object[] ps)
        {
            var excutor = new StoredProcedureContext(name, ps).SetConnection(Consts.BaseConnectStringName);
            return excutor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static IStoredProcedureContext StoredProcedure(this DataGateway gateway, string name, params object[] ps)
        {
            var context = new StoredProcedureContext(name, ps);
            return context;
        }

    }
}
