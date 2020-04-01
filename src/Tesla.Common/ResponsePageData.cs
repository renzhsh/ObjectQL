using Jinhe.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Jinhe
{
    public class ResponsePageData : ResponseData
    { 
        [DataMember]
        public int recordsTotal
        {
            set; get;
        }

        [DataMember]
        public int recordsFiltered
        {
            get
            {
                return recordsTotal;
            }
        }
    }
}