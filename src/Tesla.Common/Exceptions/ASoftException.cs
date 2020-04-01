using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinhe.Exceptions
{
    /// <summary>
    /// 异常
    /// </summary>
    public class ASoftException : System.Exception
    {
        public ASoftException() { }

        public ASoftException(string message) : this(RetCode.C1011, message)
        {
            
        }

        public ASoftException(RetCode code,string message) : base(message)
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
