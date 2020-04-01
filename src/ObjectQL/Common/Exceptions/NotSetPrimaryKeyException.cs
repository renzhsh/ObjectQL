/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：NotSetPrimaryKeyException
 * 命名空间：ObjectQL.Exceptions
 * 文 件 名：NotSetPrimaryKeyException
 * 创建时间：2018/1/22 15:55:13
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jinhe;

namespace ObjectQL.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NotSetPrimaryKeyException : ASoftException
    {
        public NotSetPrimaryKeyException(string message) : base(message)
        { }
    }
}
