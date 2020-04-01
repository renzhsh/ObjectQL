/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：DataCommand
 * 命名空间：ObjectQL.Data.Impl
 * 文 件 名：DataCommand
 * 创建时间：2017/5/2 22:29:39
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ObjectQL.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DataCommand
    {
        public string CommandText { set; get; }

        public IDataParameter[] Parameters { set; get; }

        public CommandType CommandType { set; get; }
    }
}
