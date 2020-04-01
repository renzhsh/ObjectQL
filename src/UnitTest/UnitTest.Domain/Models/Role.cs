using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectQL.DataExtTests.Models
{
    /// <summary>
    /// 
    ///</summary>
    public class Role
    {

        ///<summary> 
        /// 
        ///</summary> 
        public String RoleID { set; get; }

        ///<summary> 
        /// 
        ///</summary> 
        public DateTime CreateDate { set; get; }

        ///<summary> 
        /// 0_系统角色
        ///</summary> 
        public decimal Type { set; get; }

        ///<summary> 
        /// 角色描述(最大长度100)
        ///</summary> 
        public String RoleDesc { set; get; }

        ///<summary> 
        /// 状态，-11：无效数据
        ///</summary> 
        public decimal State { set; get; }

        ///<summary> 
        /// 角色名称
        ///</summary> 
        public String RoleName { set; get; }

    }
}