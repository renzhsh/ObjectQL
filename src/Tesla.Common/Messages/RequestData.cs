using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinhe.Messages
{
    [Serializable]
    public class RequestData : RequestData<object>
    { }

    [Serializable]
    public class RequestData<TBody>
    {
        public static Func<string, string> GetKeyByApiID;

        public virtual RequestHeader Head { set; get; }

        public TBody Body { set; get; }

        public String ToJson()
        {
            return Serialize.JsonSerilize(this);
        }

        /// <summary>
        /// 创建DES加密的签名(HEX)
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="encryptFunc">默认空时采用DES加密</param>
        /// <returns></returns>
        public String CreateEncryptSign(string sKey, Func<byte[], byte[], byte[]> encryptFunc = null)
        {
            if (encryptFunc == null)
            {
                encryptFunc = Security.DesEncrypt;
            }
            var srcData = Encoding.UTF8.GetBytes(this.MD5Sign);
            var key = Security.ConvertHexToBytes(sKey);
            var encryptData = encryptFunc(srcData, key);
            return Security.ConvertBytesToHex(encryptData);
        }

        protected string MD5Sign
        {
            get
            { 
                String body = this.Head.AppID + Serialize.JsonSerilize(this.Body);
                String base64Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
                String md5 = Security.MD5Compute(base64Text).ToLower();
                return md5;
            }
        }

        /// <summary>
        /// 解密签名(默认DES)
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="decryptFunc"></param>
        /// <returns></returns>
        public String DecryptSign(string sKey, Func<byte[], string, string> decryptFunc = null)
        {
            if (decryptFunc == null)
            {
                decryptFunc = Security.DesDecrypt;
            }
            var signData = Security.ConvertHexToBytes(this.Head.Sign);
            var result = decryptFunc(signData, sKey);
            return result;
        }

        /// <summary>
        /// 验证Head的签名是否正确
        /// </summary>
        /// <param name="getKeyByApiIDFunc"></param>
        /// <param name="decryptFunc"></param>
        /// <returns></returns>
        public bool Validate(Func<string, string> getKeyByApiIDFunc, Func<byte[], string, string> decryptFunc = null)
        {
            var key = getKeyByApiIDFunc(this.Head.AppID);
            var decryptSign = this.DecryptSign(key, decryptFunc);
            if (this.MD5Sign == decryptSign)
            {
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class RequestHeader
    {
        public string AppID { set; get; }

        public string Sign { set; get; }

        public double Version { set; get; }

        public string AccessTicket { set; get; }
    }
}
