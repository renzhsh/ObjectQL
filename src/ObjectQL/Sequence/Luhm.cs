using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectQL.Sequences
{
    /// <summary>
    /// Luhm校验算法
    /// https://baike.baidu.com/item/Luhn%E7%AE%97%E6%B3%95/22799984?fr=aladdin
    /// </summary>
    public class Luhm
    {
        public static int Resolve(long gen)
        {
            int curval = 0;
            int total = 0;
            long bitNum = 1;
            String strTogen = gen.ToString();
            int lenght = strTogen.Length;
            for (int i = 0; i < lenght; i++)
            {
                curval = int.Parse(strTogen.Substring(i, 1));
                if (bitNum == 1)
                {
                    bitNum = 0;
                    curval = curval * 2;
                    if (curval > 9)
                    {
                        curval -= 9;
                    }
                }
                else
                {
                    bitNum = 1;
                }
                total += curval;
            }
            int mod = total % 10;
            if (mod == 0)
            {
                return 0;
            }
            else
            {
                return 10 - mod;
            }
        }
    }
}
