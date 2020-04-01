using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.Model
{
    public class Role
    {

        ///<summary> 
        /// 
        ///</summary> 
        public String RoleID { set; get; }


        ///<summary> 
        /// 父角色
        ///</summary> 
        public String ParentID { set; get; }

        ///<summary> 
        /// 0_系统角色
        ///</summary> 
        public RoleType Type { set; get; }

        ///<summary> 
        /// 角色描述(最大长度100)
        ///</summary> 
        public String RoleDesc { set; get; }

        ///<summary> 
        /// 
        ///</summary> 
        public decimal IsLeaf { set; get; }

        ///<summary> 
        /// 
        ///</summary> 
        public String Path { set; get; }

        ///<summary> 
        /// 角色名称
        ///</summary> 
        public String RoleName { set; get; }

        ///<summary> 
        /// 
        ///</summary> 
        public decimal Expanded { set; get; }

        ///<summary> 
        /// 排序
        ///</summary> 
        public decimal Sort { set; get; }

        ///<summary> 
        /// 域ID
        ///</summary> 
        public String AppID { set; get; }

        ///<summary> 
        /// 树深度
        ///</summary> 
        public decimal Lvl { set; get; }

        ///<summary> 
        /// 
        ///</summary> 
        public String CreateUserID { set; get; }

        ///<summary> 
        /// 状态，-11：无效数据
        ///</summary> 
        public decimal State { set; get; }

        ///<summary> 
        /// 
        ///</summary> 
        public String IconCls { set; get; }

        ///<summary> 
        ///是否允许删除
        ///</summary> 
        public decimal BanDel { set; get; }

        ///<summary> 
        /// 
        ///</summary> 
        public DateTime CreateDate { set; get; }

    }

    public enum RoleType
    {
        普通用户 = 0,
        普通管理员 = 1,
        应用管理员 = 2,
        超级管理员 = 99,
    }
}
