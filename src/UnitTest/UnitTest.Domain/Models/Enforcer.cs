using System;
using System.Collections.Generic;

namespace ObjectQL.DataExtTests.Models
{
    /// <summary>
    /// 
    ///</summary>
    public class Enforcer
    {
        ///<summary> 
        /// 执法人员UserID(作为Auth_User的拓展信息)
        ///</summary> 
        public String UserID { set; get; }

        /// <summary>
        /// 用户
        /// </summary>
        public AuthUser Users { get; set; }
    }
}