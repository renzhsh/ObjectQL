/*************************************************************************************
 * CLR 版本：4.0.30319.42000
 * 类 名 称：NonQueryResult
 * 命名空间：ObjectQL.Data
 * 文 件 名：NonQueryResult
 * 创建时间：2016/10/19 10:44:11
 * 作    者：renzhsh
 * 说    明：
 * 修改时间：
 * 修 改 人：
*************************************************************************************/

using System.Data;
using System.Text;

namespace ObjectQL.Data
{
    public class NonQueryResult : ResultBase
    {
        private int value = 0;
        /// <summary>
        /// 数据库命令执行后的返回值
        /// </summary>
        public int Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 使用一个数据库命令的执行后的返回值和数据库命令初始化对象
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="command">命令</param>
        public NonQueryResult(int result, IDbCommand command)
            : base(command)
        {
            this.value = result;
        }

        private StringBuilder sbdesc;
        /// <summary>
        /// 结果描述
        /// </summary>
        public override string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库返回结果信息如下");
                    sbdesc.AppendLine("类型类型:" + CommandType);
                    sbdesc.AppendLine("命令文本:" + CommandText);
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        sbdesc.AppendLine("参数列表:");
                        int max = 0;
                        foreach (IDataParameter p in Parameters)
                        {
                            max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                        }
                        foreach (IDataParameter p in Parameters)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                        }
                    }
                    sbdesc.AppendLine("影响行数:" + value);
                }
                return sbdesc.ToString();
            }
        }
    }
}
