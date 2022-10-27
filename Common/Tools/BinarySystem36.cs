using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Tools
{
    public class BinarySystem36
    {
        //36进制数据字典
        public Dictionary<int, string> dNo = new Dictionary<int, string>();
        //翻转后的36进制数据字典
        private Dictionary<string, int> dNoReverse = new Dictionary<string, int>();
        public BinarySystem36()
        {
            dNo.Add(0, "0");
            dNo.Add(1, "1");
            dNo.Add(2, "2");
            dNo.Add(3, "3");
            dNo.Add(4, "4");
            dNo.Add(5, "5");
            dNo.Add(6, "6");
            dNo.Add(7, "7");
            dNo.Add(8, "8");
            dNo.Add(9, "9");
            dNo.Add(10, "A");
            dNo.Add(11, "B");
            dNo.Add(12, "C");
            dNo.Add(13, "D");
            dNo.Add(14, "E");
            dNo.Add(15, "F");
            dNo.Add(16, "G");
            dNo.Add(17, "H");
            dNo.Add(18, "I");
            dNo.Add(19, "J");
            dNo.Add(20, "K");
            dNo.Add(21, "L");
            dNo.Add(22, "M");
            dNo.Add(23, "N");
            dNo.Add(24, "O");
            dNo.Add(25, "P");
            dNo.Add(26, "Q");
            dNo.Add(27, "R");
            dNo.Add(28, "S");
            dNo.Add(29, "T");
            dNo.Add(30, "U");
            dNo.Add(31, "V");
            dNo.Add(32, "W");
            dNo.Add(33, "X");
            dNo.Add(34, "Y");
            dNo.Add(35, "Z");
            foreach (var d in dNo)
            {
                dNoReverse.Add(d.Value, d.Key);
            }
        }
        /// <summary>
        /// 36进制生成方法
        /// </summary>
        /// <param name="noValue">十进制数</param>
        /// <param name="iLength">转换长度</param>
        /// <returns></returns>
        public string gen36No(int noValue, int iLength)
        {
            BinarySystem36 binary = new BinarySystem36();
            int iDiv = 0;
            int iMod = noValue;
            int i = 1;
            if (noValue > Math.Pow(36, iLength) - 1)
            {
                return "超出范围";
            }
            else
            {
                List<string> sLst = new List<string>();
                while (iMod != 0)
                {
                    int tmp = int.Parse(Math.Pow(36, iLength - i).ToString());
                    iDiv = iMod / tmp;
                    iMod = iMod % tmp;
                    sLst.Add(binary.dNo[iDiv]);
                    i++;
                }
                int iLstLength = sLst.Count();
                for (int k = iLstLength + 1; k <= iLength; k++)
                {
                    sLst.Add("0");
                }
                string[] tmpArr = sLst.ToArray();
                return string.Join("", tmpArr);
            }
        }

        /// <summary>
        /// 36进制转成10进制
        /// </summary>
        /// <param name="value">62进制数值</param>
        /// <returns>10进制结果</returns>
        public int Convert36ToNo(string value)
        {
            BinarySystem36 binary = new BinarySystem36();
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {

                result += int.Parse(Math.Pow(36, i).ToString()) * binary.dNoReverse[value.Substring(value.Length - (i + 1), 1)];
            }
            return result;
        }
    }
}
