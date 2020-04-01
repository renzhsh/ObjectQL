using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Data
{
    public class ScalerResult : ResultBase, IDisposable
    {
        private object value;
        /// <summary>
        /// 数据库命令执行后的返回值
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
        }
        public ScalerResult(object result, IDbCommand command)
         : base(command)
        {
            this.value = result;
        }

        /// <summary>
        /// 检测返回的值是否为null或者DBNull.Value(如果数据库没有返回行数据,则为null,如果数据库返回了行数据,但为NULL,则为DBNull.Value
        /// </summary>
        public bool IsNullOrDbNull
        {
            get
            {
                return value == null || Convert.IsDBNull(value);
            }
        }

        /// <summary>
        /// 如果数据库没有返回行数据,则返回true,否则返回false
        /// </summary>
        public bool IsNull
        {
            get
            {
                return value == null;
            }
        }

        /// <summary>
        /// 如果数据返回了数据,但第一行第一列为NULL,则返回true,否则返回false
        /// </summary>
        public bool IsDBNull
        {
            get
            {
                return Convert.IsDBNull(value);
            }
        }

        /// <summary>
        /// 返回Int32类型的值
        /// </summary>
        public int IntValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToInt32(this.value);
            }
        }

        /// <summary>
        /// 返回Int64类型的值
        /// </summary>
        public long LongValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToInt64(this.value);
            }
        }

        /// <summary>
        /// 返回浮点类型的值
        /// </summary>
        public float FloatValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToSingle(this.value);
            }
        }

        /// <summary>
        /// 返回双精度类型的值
        /// </summary>
        public double DoubleValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToDouble(this.value);
            }
        }

        public Decimal DecimalValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToDecimal(this.value);
            }
        }


        /// <summary>
        /// 返回布尔类型的值(只检查是否为true,t,1)
        /// </summary>
        public bool BoolValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                string val = this.StringValue.ToLower();
                return val == "true" || val == "1" || val == "t";
            }
        }

        /// <summary>
        /// 返回时间类型的值
        /// </summary>
        public DateTime DateTimeValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToDateTime(this.value);
            }
        }

        /// <summary>
        /// 返回String类型的值
        /// </summary>
        public string StringValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToString(this.value);
            }
        }
         

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>结果转换后的字符串</returns>
        public override string ToString()
        {
            return this.value.ToString();
        }

        #region IDisposable Members

        /// <summary>
        /// 释放资源
        /// </summary>
        void IDisposable.Dispose()
        {
            this.value = null;
            base.Dispose();
        }

        #endregion

        private StringBuilder sbdesc = null;
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
                    sbdesc.AppendLine("查询结果:" + value);
                }
                return sbdesc.ToString();
            }
        }
    }
}
