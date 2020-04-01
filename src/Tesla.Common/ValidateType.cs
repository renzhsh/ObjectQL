/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ValidateType
 * 命名空间：Jinhe
 * 文 件 名：ValidateType
 * 创建时间：2016/10/24 16:31:45
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/


namespace Jinhe
{
    /// <summary>
    /// 正则表达式验证的类型
    /// </summary>
    public enum ValidateType
    {

        /// <summary>
        /// 空字符串或全部为空格的字符串
        /// </summary>
        NullOrWhiteSpace,

        /// <summary>
        /// 整数
        /// </summary>
        Int,

        /// <summary>
        /// 大于0整数
        /// </summary>
        GZInt,

        /// <summary>
        /// 小于0整数
        /// </summary>
        LZInt,

        /// <summary>
        /// 长整数
        /// </summary>
        Long,

        /// <summary>
        /// 浮点型
        /// </summary>
        Float,

        /// <summary>
        /// 大于0浮点型
        /// </summary>
        GZFloat,

        /// <summary>
        /// 小于0浮点型
        /// </summary>
        LZFloat,

        /// <summary>
        /// 双精度数
        /// </summary>
        Double,

        /// <summary>
        /// 大于0双精度数
        /// </summary>
        GZDouble,

        /// <summary>
        /// 小于0双精度数
        /// </summary>
        LZDouble,

        /// <summary>
        /// 大于0长整数
        /// </summary>
        GZLong,

        /// <summary>
        /// 小于0长整数
        /// </summary>
        LZLong,

        /// <summary>
        /// 电子邮箱
        /// </summary>
        Email,

        /// <summary>
        /// HTML
        /// </summary>
        Html,

        /// <summary>
        /// 汉字
        /// </summary>
        Cn,

        /// <summary>
        /// 英文
        /// </summary>
        En,

        /// <summary>
        /// 英文下划线数字
        /// </summary>
        EnUnNumber,

        /// <summary>
        /// 英文下划线数字且以英文或下划线开头
        /// </summary>
        EnUn_EnUnNumber,

        /// <summary>
        /// 中国所有的手机号
        /// </summary>
        Mobile,

        /// <summary>
        /// 中国移动电话
        /// </summary>
        ChinaMobile,

        /// <summary>
        /// 中国联通电话
        /// </summary>
        ChinaUnicom,

        /// <summary>
        /// 中国电信电话
        /// </summary>
        ChinaTelcom,

        /// <summary>
        /// 固定电话
        /// </summary>
        FixedPhone,

        /// <summary>
        /// 字母开头，允许4-20字节，允许字母数字下划线
        /// </summary>
        Id,

        /// <summary>
        /// 18位身份证号码
        /// </summary>
        IdCard,

        /// <summary>
        /// QQ号码
        /// </summary>
        QQ,

        /// <summary>
        /// 验证IP地址
        /// </summary>
        IPAddress,

        /// <summary>
        /// 日期验证(yyyy-MM-dd)
        /// </summary>
        Date,

        /// <summary>
        /// 时间验证(HH:mm:ss)
        /// </summary>
        Time,

        /// <summary>
        /// 日期时间(yyyy-MM-dd HH:mm:dd) 
        /// </summary>
        DateTime,

        /// <summary>
        /// 验证GUID
        /// </summary>
        Guid,

        /// <summary>
        /// 数字连接串(连接符为',',有可能只是连接符号的组合,而没有数字)
        /// </summary>
        NumberJoin,

        /// <summary>
        /// 严格数字连接串(要求必须为如:x或者x,x,x,x的样式,连接符为',')
        /// </summary>
        StrictNumberJoin,

        /// <summary>
        /// 字符串连接串(连接符为',',有可能只是连接符号的组合,而没有字符)
        /// </summary>
        StringJoin,

        /// <summary>
        /// 严格字符串连接串(要求必须为如:x或者x,x,x,x的样式,连接符为',')
        /// </summary>
        StrictStringJoin

    }
}
