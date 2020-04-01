/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：RetCode
 * 命名空间：Jinhe.Common
 * 文 件 名：RetCode
 * 创建时间：2016/10/24 14:02:32
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

namespace Jinhe
{
    public enum RetCode
    {
        /// <summary>
        /// 没有找到指定记录
        /// </summary>
        NotFoundRecord = 1,

        /// <summary>
        /// 操作成功
        /// </summary>
        C0000 = 0,

        /// <summary>
        /// 
        /// </summary>
        C1000 = 1000,

        /// <summary>
        /// 错误的签名
        /// </summary>
        C1001 = 1001,

        /// <summary>
        /// 错误的参数
        /// </summary>
        C1002 = 1002,


        #region 未确定的异常
        /// <summary>
        /// 未确定的异常（未做异常处理且非自定义的异常）
        /// </summary>
        C1111 = 1111,
        /// <summary>
        /// 未确定的异常（未做异常处理但是自定义的异常）
        /// </summary>
        C1011 = 1011,



        #endregion

        #region 验证类错误



        /// <summary>
        /// 用户名错误
        /// </summary>
        C2001 = 2001,

        /// <summary>
        /// 密码错误
        /// </summary>
        C2002 = 2002,

        /// <summary>
        /// 登录验证码错误
        /// </summary>
        C2003 = 2003,

        /// <summary>
        /// 用户名重复
        /// </summary>
        C2004 = 2004,
        /// <summary>
        /// EMAIL重复
        /// </summary>
        C2005 = 2005,
        /// <summary>
        /// 电话号码重复
        /// </summary>
        C2006 = 2006,

        /// <summary>
        /// 用户未实名
        /// </summary>
        C2009 = 2009,
        #endregion

        #region 数据库操作异常

        /// <summary>
        /// 未获得或未初始化数据库链接
        /// </summary>
        DB9000 = 9000,

        /// <summary>
        /// 数据库操作异常
        /// </summary>
        DB9001 = 9001,

        /// <summary>
        /// ORM的指定字段没有映射数据库字段
        /// </summary>
        DB9002 = 9002,

        /// <summary>
        /// ORM没有映射数据映射关系
        /// </summary>
        DB9003 = 9003,

        /// <summary>
        /// ORM数据映射关系中不包含任何字段关系
        /// </summary>
        DB9004 = 9004,

        /// <summary>
        /// 违反数据唯一性约束
        /// </summary>
        DB9010 = 9010,

        /// <summary>
        /// 违反数据完整性约束
        /// </summary>
        DB9011 = 9011,
        #endregion
    }
}
