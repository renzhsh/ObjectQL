/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：ASoftException
 * 命名空间：Jinhe.Common
 * 文 件 名：ASoftException
 * 创建时间：2016/10/24 14:02:02
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;

namespace Jinhe
{
    public class ASoftException : System.Exception
    {
        public ASoftException() { }

        public ASoftException(string message) : this(RetCode.C1011, message)
        {
        }

        public ASoftException(RetCode code, string message) : base(message)
        {
            this.Code = (int)code;
        }

        public ASoftException(int code, string message) : base(message)
        {
            this.Code = code;
        }

        public ASoftException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.Code = (int)RetCode.C1011;
        }

        public ASoftException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }


        public ASoftException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public int Code { protected set; get; }
    }

}
