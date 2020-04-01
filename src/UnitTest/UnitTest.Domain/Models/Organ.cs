/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：Organ
 * 命名空间：ObjectQL.DataExtTests.Models
 * 文 件 名：Organ
 * 创建时间：2017/3/23 17:01:10
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

namespace ObjectQL.DataExtTests.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Organ
    {
        public string OrganName { set; get; }

        public string OrganId { set; get; }

        public string ParentId { set; get; }

        public Organ Parent { set; get; }

        public IEnumerable<Organ> Child { set; get; }

    }
}
