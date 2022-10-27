/*****************************************************************
代码功能：62进制和10进制转换操作类
开发日期：2016年09月20日
作    者：赵慧敏
联系方式：13313318725
版权所有：河北广联信息技术有限公司研发一部    
******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Common.Tools
{
    /// <summary>
    /// 10进制于62进制转换
    /// </summary>
    public class BinarySystem62
    {
        //62进制数据字典
        private Dictionary<int, string> dNo = new Dictionary<int, string>();
        //翻转后的62进制数据字典
        private Dictionary<string, int> dNoReverse = new Dictionary<string, int>();
        public BinarySystem62()
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
            dNo.Add(10, "a");
            dNo.Add(11, "b");
            dNo.Add(12, "c");
            dNo.Add(13, "d");
            dNo.Add(14, "e");
            dNo.Add(15, "f");
            dNo.Add(16, "g");
            dNo.Add(17, "h");
            dNo.Add(18, "i");
            dNo.Add(19, "j");
            dNo.Add(20, "k");
            dNo.Add(21, "l");
            dNo.Add(22, "m");
            dNo.Add(23, "n");
            dNo.Add(24, "o");
            dNo.Add(25, "p");
            dNo.Add(26, "q");
            dNo.Add(27, "r");
            dNo.Add(28, "s");
            dNo.Add(29, "t");
            dNo.Add(30, "u");
            dNo.Add(31, "v");
            dNo.Add(32, "w");
            dNo.Add(33, "x");
            dNo.Add(34, "y");
            dNo.Add(35, "z");
            dNo.Add(36, "A");
            dNo.Add(37, "B");
            dNo.Add(38, "C");
            dNo.Add(39, "D");
            dNo.Add(40, "E");
            dNo.Add(41, "F");
            dNo.Add(42, "G");
            dNo.Add(43, "H");
            dNo.Add(44, "I");
            dNo.Add(45, "J");
            dNo.Add(46, "K");
            dNo.Add(47, "L");
            dNo.Add(48, "M");
            dNo.Add(49, "N");
            dNo.Add(50, "O");
            dNo.Add(51, "P");
            dNo.Add(52, "Q");
            dNo.Add(53, "R");
            dNo.Add(54, "S");
            dNo.Add(55, "T");
            dNo.Add(56, "U");
            dNo.Add(57, "V");
            dNo.Add(58, "W");
            dNo.Add(59, "X");
            dNo.Add(60, "Y");
            dNo.Add(61, "Z");
            foreach (var d in dNo)
            {
                dNoReverse.Add(d.Value, d.Key);
            }
        }

        /// <summary>
        /// 10进制转换成62进制
        /// </summary>
        /// <param name="noValue">10进制数值</param>
        /// <param name="iLength">62进制长度</param>
        /// <returns>返回结果</returns>
        public string gen62No(int noValue)
        {
            int iLength = Convert.ToInt32(ConfigurationManager.AppSettings["LSHLong"]);
            int iDiv = 0;
            int iMod = noValue;
            int i = 1;
            if (noValue > Math.Pow(62, iLength) - 1)
            {
                return "超出范围";
            }
            else
            {
                List<string> sLst = new List<string>();
                while (iMod != 0)
                {
                    int tmp = int.Parse(Math.Pow(62, iLength - i).ToString());
                    iDiv = iMod / tmp;
                    iMod = iMod % tmp;
                    sLst.Add(dNo[iDiv]);
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

        public string gen62No(int noValue, int iLength)
        {
            //int iLength = Convert.ToInt32(ConfigurationManager.AppSettings["LSHLong"]);
            int iDiv = 0;
            int iMod = noValue;
            int i = 1;
            if (noValue > Math.Pow(62, iLength) - 1)
            {
                return "超出范围";
            }
            else
            {
                List<string> sLst = new List<string>();
                while (iMod != 0)
                {
                    int tmp = int.Parse(Math.Pow(62, iLength - i).ToString());
                    iDiv = iMod / tmp;
                    iMod = iMod % tmp;
                    sLst.Add(dNo[iDiv]);
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
        /// 62进制转成10进制
        /// </summary>
        /// <param name="value">62进制数值</param>
        /// <returns>10进制结果</returns>
        public int Convert62ToNo(string value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {

                result += int.Parse(Math.Pow(62, i).ToString()) * dNoReverse[value.Substring(value.Length - (i + 1), 1)];
            }
            return result;
        }
    }
}
