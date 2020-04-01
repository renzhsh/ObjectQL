using System;

namespace ObjectQL.DataExtTests.Models
{
    /// <summary>
    /// 用户
    ///</summary>
    public class AuthUser
    {
        ///<summary> 
        /// 主键
        ///</summary> 
        public String UserID { set; get; }

        ///<summary> 
        /// 账号
        ///</summary> 
        public String UserName { set; get; }

        ///<summary> 
        /// 密码
        ///</summary> 
        public String Password { set; get; }
    }

}
