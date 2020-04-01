using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.Model
{
    /// <summary>
    /// 实名信息
    ///</summary>
    public class RealUser
    {

        ///<summary> 
        /// 用户编号
        ///</summary> 
        public String UserID { set; get; }

        ///<summary> 
        /// REAL_USER_ID
        ///</summary> 
        public String RealUserID { set; get; }

        ///<summary> 
        /// 状态
        ///</summary> 
        public decimal State { set; get; }

        ///<summary> 
        /// 身份证号码
        ///</summary> 
        public String IDCardNum { set; get; }

        ///<summary> 
        /// 姓名
        ///</summary> 
        public String RealName { set; get; }


        ///<summary> 
        /// 创建时间
        ///</summary> 
        public DateTime CreateDate { set; get; }

    }
}
