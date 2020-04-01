/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ValidateUtils
 * 命名空间：Jinhe
 * 文 件 名：ValidateUtils
 * 创建时间：2016/10/24 16:31:15
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jinhe
{
    /// <summary>
    /// 常用的验证类
    /// </summary>
    public static class ValidateUtils
    {
        #region 正则表达式
        #region 电话号码
        private const string regExpChinaMobile = @"^1(3[4-9]|5[012789]|8[78])\d{8}$"; //134.135.136.137.138.139.150.151.152.157.158.159.187.188 ,147(数据卡)
        private const string regExpChinaUnicom = @"^1(3[0-2]|5[56]|8[56])\d{8}$"; //130.131.132.155.156.185.186
        private const string regExpChinaTelcom = @"^1(8[09]|[35]3)\d{8}$"; //133.153.180.189 
        private const string regExpFixedPhone = @"^0(10|2[0-9]|[3-9][0-9]{2})-?\d{7,8}(-\d+)?$"; //固定电话
        private const string regExpMobile = @"^1((47)|(3[0-9])|(5[012356789])(8[056789])[0-9]8$"; //中国手机号码
        #endregion

        /// <summary>
        /// 电子邮箱
        /// </summary>
        private const string regExpEmail = @"^[_\.0-9a-zA-Z-]+@([0-9a-zA-Z][0-9a-zA-Z-]+\.)+[a-zA-Z]{2,3}$";

        /// <summary>
        /// 中文字符
        /// </summary>
        private const string regExpCn = @"^[\u4e00-\u9fa5]+$";

        /// <summary>
        /// 英文字符
        /// </summary>
        private const string regExpEn = @"^[a-zA-Z]+$";

        /// <summary>
        /// 英文数字下划线
        /// </summary>
        private const string regExpEnUnNumber = @"^[a-zA-Z0-9_]+$";

        /// <summary>
        /// 以英文或下划线开头的只包含英文数字下划线的字符串
        /// </summary>
        private const string regExpEnUn_EnUnNumber = @"^[a-zA-Z_]+[a-zA-Z0-9_]+$";

        /// <summary>
        /// IP地址
        /// </summary>
        private const string regExpIPAddress = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";//IP地址

        /// <summary>
        /// 日期
        /// </summary>
        private const string regExpDate = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";

        /// <summary>
        /// 时间
        /// </summary>
        private const string regExpTime = @"^(20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$";

        /// <summary>
        /// 日期时间
        /// </summary>
        private const string regExpDateTime = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$";

        /// <summary>
        /// 匹配HTML正则表达式
        /// </summary>
        private const string regExpHtml = "<(S*?)[^>]*>.*?|<.*? />";

        /// <summary>
        /// 字母开头，允许4-20字节，允许字母数字下划线
        /// </summary>
        private const string regExpId = "^[a-zA-Z][a-zA-Z0-9_]{3,19}$ ";

        #region 验证身份证号码
        /// <summary>
        /// 身份证号码
        /// </summary>
        private const string regExpIdCard = @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$";

        #endregion

        #region 验证QQ
        /// <summary>
        /// QQ号码
        /// </summary>
        private const string regExpQQ = @"^[1-9][0-9]{4,9}$";
        #endregion

        #region 判断一个字符串是否是用数字连接的

        /// <summary>
        /// 数字连接(连接符为',',有可能只是连接符号的组合,而没有数字)
        /// </summary>
        private static readonly string regExpNumberJoin = @"^(\d+|,)+$";

        /// <summary>
        /// 严格的数字连接(要求必须为如:x或者x,x,x,x的样式,连接符为',')
        /// </summary>
        private static readonly string regExpStrictNumberJoin = @"^\d+(,\d+)*$";

        /// <summary>
        /// 字符串连接(连接符为',',有可能只是连接符号的组合,而没有字符)
        /// </summary>
        private static readonly string regExpStringJoin = @"^(\w+|,)+$";

        /// <summary>
        /// 严格的字符串连接(要求必须为如:x或者x,x,x,x的样式,连接符为',')
        /// </summary>
        private static readonly string regExpStrictStringJoin = @"^\w+(,\w+)*$";

        /// <summary>
        /// GUID
        /// </summary>
        private static readonly string regExpGuid = @"^\w{8}-(\w{4}-){3}\w{12}$";

        #endregion
        #endregion

        #region 验证字符串是否满足指定的类型
        /// <summary>
        /// 验证字符串是否满足指定的类型
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <param name="type">验证类型</param>
        /// <param name="enableNullOrEmpty">要验证的内容是否允许为null或string.Empty</param>
        /// <returns>返回一个值，指示传入的参数是否是指定的类型</returns>
        public static bool CheckString(string input, ValidateType type, bool enableNullOrEmpty)
        {
            bool result = false;

            if (enableNullOrEmpty && string.IsNullOrEmpty(input))
            {
                result = true;
            }
            else
            {
                if (type == ValidateType.NullOrWhiteSpace)
                {
                    result = true;
                    if (input != null)
                    {
                        for (int i = 0; i < input.Length; i++)
                        {
                            if (!char.IsWhiteSpace(input[i]))
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    #region 主体验证部分
                    if (input == null || input.Length == 0)
                    {
                        return result;
                    }
                    switch (type)
                    {
                        case ValidateType.Int:
                            {
                                int temp;
                                result = int.TryParse(input, out temp);
                                break;
                            }
                        case ValidateType.GZInt:
                            {
                                int temp;
                                if (int.TryParse(input, out temp))
                                {
                                    result = temp > 0;
                                }
                                break;
                            }
                        case ValidateType.LZInt:
                            {
                                int temp;
                                if (int.TryParse(input, out temp))
                                {
                                    result = temp < 0;
                                }
                                break;
                            }
                        case ValidateType.Long:
                            {
                                int temp;
                                result = int.TryParse(input, out temp);
                                break;
                            }
                        case ValidateType.GZLong:
                            {
                                long temp;
                                if (long.TryParse(input, out temp))
                                {
                                    result = temp > 0;
                                }
                                break;
                            }
                        case ValidateType.LZLong:
                            {
                                long temp;
                                if (long.TryParse(input, out temp))
                                {
                                    result = temp < 0;
                                }
                                break;
                            }
                        case ValidateType.Float:
                            {
                                float temp;
                                result = float.TryParse(input, out temp);
                                break;
                            }
                        case ValidateType.GZFloat:
                            {
                                float temp;
                                if (float.TryParse(input, out temp))
                                {
                                    result = temp > 0;
                                }
                                break;
                            }
                        case ValidateType.LZFloat:
                            {
                                float temp;
                                if (float.TryParse(input, out temp))
                                {
                                    result = temp < 0;
                                }
                                break;
                            }
                        case ValidateType.Double:
                            {
                                double temp;
                                result = double.TryParse(input, out temp);
                                break;
                            }
                        case ValidateType.GZDouble:
                            {
                                double temp;
                                if (double.TryParse(input, out temp))
                                {
                                    result = temp > 0;
                                }
                                break;
                            }
                        case ValidateType.LZDouble:
                            {
                                double temp;
                                if (double.TryParse(input, out temp))
                                {
                                    result = temp < 0;
                                }
                                break;
                            }
                        default:
                            {
                                string pattern = Reflect.GetField(typeof(ValidateUtils), "regExp" + type.ToString(), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToString();
                                result = Regular.IsMatch(pattern, input);
                                break;
                            }
                    }
                    #endregion
                }
            }
            return result;
        }

        #endregion

        #region 验证字符串是否满足指定的正则表达式
        /// <summary>
        /// 验证字符串是否满足指定的类型(默认允许为空)
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <param name="type">验证类型</param>
        /// <returns>返回一个值，指示传入的参数是否是指定的类型</returns>
        public static bool CheckString(string input, ValidateType type)
        {
            return CheckString(input, type, true);
        }

        /// <summary>
        /// 验证字符串是否满足指定的类型
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="enableNullOrEmpty">要验证的内容是否允许为null或string.Empty</param>
        /// <returns>返回一个值，指示传入的参数是否满足正则表达式</returns>
        public static bool CheckString(string input, string regex, bool enableNullOrEmpty)
        {
            return CheckString(input, regex, RegexOptions.None, enableNullOrEmpty);
        }

        /// <summary>
        /// 验证字符串是否满足指定的类型
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="options">正则表达式选项</param>
        /// <param name="enableNullOrEmpty">要验证的内容是否允许为null或string.Empty</param>
        /// <returns>返回一个值，指示传入的参数是否满足正则表达式</returns>
        public static bool CheckString(string input, string regex, RegexOptions options, bool enableNullOrEmpty)
        {
            bool result = false;

            if (enableNullOrEmpty && string.IsNullOrEmpty(input))
            {
                result = true;
            }
            else
            {
                result = Regular.IsMatch(regex, input, options);
            }
            return result;
        }

        #endregion
    }

}
