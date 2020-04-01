using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectQL.DataExtTests.Models
{
    /// <summary>
    /// 用户登录日志
    ///</summary>
    public class LoginLog
    {
        
        ///<summary> 
        /// 
        ///</summary> 
        public String LogID { set; get; }
     
        ///<summary> 
        /// 
        ///</summary> 
        public String Content { set; get; }
     
        ///<summary> 
        /// 
        ///</summary> 
        public DateTime CreateDate { set; get; }
     
        ///<summary> 
        /// 
        ///</summary> 
        public String UserID { set; get; }

        public User User { set; get; }
     
    }
}
    