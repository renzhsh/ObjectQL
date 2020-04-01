/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：User
 * 命名空间：ObjectQL.Domain.Auth
 * 文 件 名：User
 * 创建时间：2016/12/7 11:28:01
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using ObjectQL.Data;
using System;
using System.Collections.Generic;

namespace ObjectQL.DataExtTests.Models
{
    public enum Sex {
        男 = 0,
        女 = 1
    }
    /// <summary>
    /// 
    /// </summary>
    public class User
    {
        public string Id { set; get; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { set; get; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public int Type { set; get; }

        /// <summary>
        /// 访问失败次数
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// 邮箱已验证
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// 允许锁定
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// 手机号是否已完成验证
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        ///// <summary>
        ///// 用户角色
        ///// </summary>
        //public virtual ICollection<TRole> Roles { get; }

        /// <summary>
        /// 秘钥
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        public DateTime CreateTime { set; get; }

        public Byte[] Test { set; get; }

        /// <summary>
        /// 允许双重认证
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }

        public IEnumerable<Role> Roles { set; get; }

        public IEnumerable<UserRole> UserRoles { set; get; }

        public IEnumerable<UserOrgan> UserOrgans { set; get; }

  
    }
}