/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：OrganMap
 * 命名空间：ObjectQL.DataExtTests.Mapping
 * 文 件 名：OrganMap
 * 创建时间：2017/3/23 17:00:24
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using ObjectQL.Data;
using ObjectQL.DataExtTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Mapping;

namespace ObjectQL.DataExtTests.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganMap : EntityMap<Organ>
    {
        public override void Mapping()
        {
            this.ToTable("SYS_ORGAN");

            this.Property(x => x.OrganId).HasColumnName("ORG_ID").IsPrimaryKey();
            this.Property(x => x.OrganName).HasColumnName("ORG_NAME");
            this.Property(x => x.ParentId).HasColumnName("PARENT_ID"); 
        }
    }
}
