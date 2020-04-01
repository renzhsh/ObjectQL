/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：StringUtils
 * 命名空间：Jinhe.Common
 * 文 件 名：StringUtils
 * 创建时间：2016/10/24 16:29:27
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinhe.Utils
{
    /// <summary>
    /// 字符串处理常用类
    /// </summary>
    public static class StringUtils
    {
        #region 对象拆分
        /// <summary>
        /// 把一个用逗号分隔的数字串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<int> SplitIntNumber(string input)
        {
            List<int> result = new List<int>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.NumberJoin))
            {
                foreach (string tmp in input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(int.Parse(tmp));
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的数字串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<long> SplitLongNumber(string input)
        {
            List<long> result = new List<long>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.NumberJoin))
            {
                foreach (string tmp in input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(long.Parse(tmp));
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的数字串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<int> SplitStrictNumber(string input)
        {
            List<int> result = new List<int>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.StrictNumberJoin))
            {
                foreach (string tmp in input.Split(','))
                {
                    result.Add(int.Parse(tmp));
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个严格用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的字符串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<string> SplitString(string input)
        {
            List<string> result = new List<string>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.StringJoin))
            {
                foreach (string tmp in input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(tmp);
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的字符串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<string> SplitStrictString(string input)
        {
            List<string> result = new List<string>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.StrictStringJoin))
            {
                foreach (string tmp in input.Split(','))
                {
                    result.Add(tmp);
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个严格用逗号分隔的字符串");
            }
            return result;
        }
        #endregion

    }
}
