using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinhe.Messages
{
    public class ResponseData<T>
    {
        [DataMember]
        public int Code { set; get; }

        [DataMember]
        public string Message { set; get; }


        [DataMember]
        public bool Success
        {
            get
            {
                return this.Code == (int)RetCode.C0000;
            }
        }

        [DataMember]
        public T Data { set; get; }
    }

    public class ResponseData : ResponseData<object>
    {
    }
     
}
