using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectQL.Model;
using ObjectQL.DataAnnotation;
using ObjectQL.Linq;

namespace ObjectQL.Sequences
{
    /// <summary>
    /// 递增序列
    /// </summary>
    public class Increase
    {
        public Increase()
        {
            InitSequence("default", 1, 1, 1, long.MaxValue);
        }

        public Increase(string name)
        {
            InitSequence(name, 1, 1, 1, long.MaxValue);
        }

        public Increase(string name, long firstValue, int step = 1, long min = 1, long max = long.MaxValue)
        {
            InitSequence(name, firstValue, step, min, max);
        }

        private void InitSequence(string name, long firstValue, int step, long min, long max)
        {
            gateway = new DataGateway();
            SeqGen = gateway.Where<SeqGen>(item => item.SeqName == name).Select().FirstOrDefault();
            if (SeqGen == null)
            {
                SeqGen = new SeqGen
                {
                    SeqName = name,
                    SeqValue = firstValue,
                    SeqStep = step,
                    SeqMin = min,
                    SeqMax = max
                };

                gateway.Insert(SeqGen);
            }
        }

        private SeqGen SeqGen { get; set; }

        private DataGateway gateway = null;

        private object asynObject = new object();

        public long CurrentValue
        {
            get
            {
                return SeqGen.SeqValue;
            }
        }

        public long Next(int length = 0)
        {
            if (length > 19)
            {
                throw new ArgumentOutOfRangeException("length","64位序列值的最大长度为19");
            }

            lock (asynObject)
            {
                SeqGen.SeqValue += SeqGen.SeqStep;
                if (SeqGen.SeqValue > SeqGen.SeqMax)
                {
                    SeqGen.SeqValue = SeqGen.SeqMin;
                    SeqGen.SeqLoop++;
                }

                gateway.Update(UpdateCriteria<SeqGen>
                    .Builder
                    .Set(item => item.SeqValue, SeqGen.SeqValue)
                    .Set(item => item.SeqLoop, SeqGen.SeqLoop),
                    item => item.SeqName == SeqGen.SeqName);
            }

            long result = SeqGen.SeqValue;
            if (length > 0)
            {
                int seqLength = result.ToString().Length;
                if (seqLength > length)
                {
                    result = result % (long)Math.Pow(10, length);
                }
                else if (seqLength < length)
                {
                    result = (long)Math.Pow(10, length - 1) + result;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 序列生成器
    /// </summary>
    [Table(Prefix = "oql")]
    internal class SeqGen
    {
        /// <summary>
        /// 序列名
        /// </summary>
        [Column(MaxLength = 50)]
        [PrimaryKey]
        public string SeqName { get; set; }

        /// <summary>
        /// 序列当前值
        /// </summary>
        public long SeqValue { get; set; } = 1;

        /// <summary>
        /// 序列步长
        /// </summary>
        public int SeqStep { get; set; } = 1;

        /// <summary>
        /// 序列最小值
        /// </summary>
        public long SeqMin { get; set; } = 1;

        /// <summary>
        /// 序列最大值
        /// </summary>
        public long SeqMax { get; set; } = 1000000000L;

        /// <summary>
        /// 序列循环次数
        /// </summary>
        public int SeqLoop { get; set; }
    }
}
