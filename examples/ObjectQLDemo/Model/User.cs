using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.Model
{
    public class User
    {
        ///<summary> 
        /// 主键
        ///</summary> 
        public String Id { set; get; }

        ///<summary> 
        /// 用户名
        ///</summary> 
        public String UserName { set; get; }

        ///<summary> 
        /// 邮箱
        ///</summary> 
        public String Email { set; get; }

        ///<summary> 
        /// 姓名
        ///</summary> 
        public String Name { set; get; }

        ///<summary> 
        /// 性别
        ///</summary> 
        public decimal Sex { set; get; }

        ///<summary> 
        /// 手机号码
        ///</summary> 
        public String PhoneNumber { set; get; }

        ///<summary> 
        /// 密码
        ///</summary> 
        public String Password { set; get; }

        public DateTime CreateDate { get; set; }

        public UserRole UserRole { get; set; }

        public IEnumerable<UserRole> UserRoles { set; get; }

        public IEnumerable<Role> Roles { set; get; }

    }
}
