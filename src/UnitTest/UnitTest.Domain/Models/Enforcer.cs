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
        /// ִ����ԱUserID(��ΪAuth_User����չ��Ϣ)
        ///</summary> 
        public String UserID { set; get; }

        /// <summary>
        /// �û�
        /// </summary>
        public AuthUser Users { get; set; }
    }
}