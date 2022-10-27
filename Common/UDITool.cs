using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
	public class UDITool
	{
		private Dictionary<String, int> strDic = new Dictionary<string, int>();
        private List<string> strLst = new List<string>();
		#region 生成检验码 勿动
		/// <summary>
		/// 生成检验码
		/// </summary>
		/// <param name="Code"></param>
		/// <returns></returns>
        public string GenVerifyCode(String Code)
        {
            string res = "";
            try
            {
                List<string> lst = new List<string>();
                /// 原始字符串转成List,并通过字典翻译
                for (int i = 0; i < Code.Length; i++)
                {
                    lst.Add(strDic[Code.Substring(i, 1)].ToString());
                }
                ///转换后的字符串
                String strNO = String.Join("", lst.ToArray());
                ///字符串翻转数组
                String[] arrNO = new String[strNO.Length];
                int k = 0;
                for (int i = strNO.Length - 1; i >= 0; i--)
                {
                    arrNO[k] = strNO.Substring(i, 1);
                    k++;
                }
                List<int> result = new List<int>();
                for (int i = 0; i < arrNO.Length; i++)
                {
                    if ((i + 1) % 2 == 0)
                    {
                        arrNO[i] = OpStrNO(arrNO[i]);
                    }
                    result.Add(int.Parse(arrNO[i]));
                }
                int sumNO = result.Sum();
                int remainNO = 10 - sumNO % 10;
                if (remainNO == 10)
                {
                    res = "0";
                }
                else
                {
                    res = remainNO.ToString();
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
		#endregion

		#region 字符转换勿动
		public UDITool()
		{
			strDic.Add("A", 10);
			strDic.Add("B", 11);
			strDic.Add("C", 12);
			strDic.Add("D", 13);
			strDic.Add("E", 14);
			strDic.Add("F", 15);
			strDic.Add("G", 16);
			strDic.Add("H", 17);
			strDic.Add("I", 18);
			strDic.Add("J", 19);
			strDic.Add("K", 20);
			strDic.Add("L", 21);
			strDic.Add("M", 22);
			strDic.Add("N", 23);
			strDic.Add("O", 24);
			strDic.Add("P", 25);
			strDic.Add("Q", 26);
			strDic.Add("R", 27);
			strDic.Add("S", 28);
			strDic.Add("T", 29);
			strDic.Add("U", 30);
			strDic.Add("V", 31);
			strDic.Add("W", 32);
			strDic.Add("X", 33);
			strDic.Add("Y", 34);
			strDic.Add("Z", 35);
			strDic.Add("a", 36);
			strDic.Add("b", 37);
			strDic.Add("c", 38);
			strDic.Add("d", 39);
			strDic.Add("e", 40);
			strDic.Add("f", 41);
			strDic.Add("g", 42);
			strDic.Add("h", 45);
			strDic.Add("i", 46);
			strDic.Add("j", 47);
			strDic.Add("k", 48);
			strDic.Add("l", 49);
			strDic.Add("m", 50);
			strDic.Add("n", 51);
			strDic.Add("o", 52);
			strDic.Add("p", 53);
			strDic.Add("q", 54);
			strDic.Add("r", 55);
			strDic.Add("s", 56);
			strDic.Add("t", 57);
			strDic.Add("u", 58);
			strDic.Add("v", 59);
			strDic.Add("w", 60);
			strDic.Add("x", 61);
			strDic.Add("y", 62);
			strDic.Add("z", 63);
			strDic.Add("0", 0);
			strDic.Add("1", 1);
			strDic.Add("2", 2);
			strDic.Add("3", 3);
			strDic.Add("4", 4);
			strDic.Add("5", 5);
			strDic.Add("6", 6);
			strDic.Add("7", 7);
			strDic.Add("8", 8);
			strDic.Add("9", 9);
			strDic.Add("-", 64);
			strDic.Add(".", 65);
			strDic.Add("@", 66);
			strDic.Add("$", 67);
			strDic.Add(",", 68);
			strDic.Add("*", 69);
			strDic.Add("+", 70);
			strDic.Add("%", 71);
			strDic.Add("/", 72);
			strDic.Add("#", 73);
			strDic.Add("!", 74);
			strDic.Add("^", 75);
			strDic.Add("~", 76);


            #region 特殊字符
            strLst.Add("~");
            strLst.Add("`");
            strLst.Add("!");
            strLst.Add("@");
            strLst.Add("#");
            strLst.Add("$");
            strLst.Add("%");
            strLst.Add("^");
            strLst.Add("&");
            strLst.Add("*");
            strLst.Add("(");
            strLst.Add(")");
            strLst.Add("+");
            strLst.Add("=");
            strLst.Add("{");
            strLst.Add("}");
            strLst.Add(";");
            strLst.Add(":");
            strLst.Add("'");
            strLst.Add("\"");
            strLst.Add("<");
            strLst.Add(",");
            strLst.Add(".");
            strLst.Add("、");
            strLst.Add("|");
            strLst.Add(">");
            strLst.Add("/");
            strLst.Add("?");
            strLst.Add("（");
            strLst.Add("）");
            strLst.Add("【");
            strLst.Add("】");
            strLst.Add("；");
            strLst.Add("：");
            strLst.Add("‘");
            strLst.Add("“");
            strLst.Add("、");
            strLst.Add("|");
            strLst.Add("《");
            strLst.Add("》");
            strLst.Add("？");
            strLst.Add("，");
            strLst.Add("。"); 
            #endregion
		}
		#endregion

		#region 处理数字 大于10 返回个位十位和 小于0 直接返回
		/// <summary>
		/// 处理数字 大于10 返回个位十位和 小于0 直接返回
		/// </summary>
		/// <param name="NO"></param>
		/// <returns></returns>
		private string OpStrNO(string NO)
		{
			int iNO = int.Parse(NO) * 2;
			if (iNO > 9)
			{
				return ((iNO / 10) + (iNO % 10)).ToString();
			}
			else
			{
				return iNO.ToString();
			}
		}
		#endregion

        #region 品类编码检验特殊字符
        public int GenVerifyPlbm(string Code)
        {
            int count = 0;
            try
            {
                for (int i = 0; i < Code.Length; i++)
                {
                    count = strLst.Select(m => new { value = m }).Where(m => m.value == Code.Substring(i, 1)).ToList().Count;
                    if (count > 0)
                    {
                        return count;
                    }
                }

            }
            catch (Exception ex)
            {
                count = 1;
                return count;
            }
            return count;
        } 
        #endregion


        /// <summary>
        /// 生成单个UDI码
        /// 2021-9-22打码客户端生成码移植代码；服务与打码客户端代码需同步
        /// 否则生成的二维码可能不同
        /// MA.156.M0.100350.01004YA10017.SLMSAGAAAC.M210628.LAY21062803.E220628.V220627.C3
        /// </summary>
        /// <param name="fixCode">企业DI</param>
        /// <param name="sign">序列号文件读取内容S</param>
        /// <param name="startdate">生产日期M</param>
        /// <param name="ShengChanPH">生产批次号L</param>
        /// <param name="dbatchnumber">灭菌批次D</param>
        /// <param name="ShiXiaoDate">失效日期E</param>
        /// <param name="YouXiaoDate">有效期V</param>
        public string GenUDI(String fixCode, string sign, string startdate, string ShiXiaoDate, string YouXiaoDate, string ShengChanPH, string dbatchnumber)
        {
            List<int> str = new List<int>();
            List<String> lst = new List<string>();
            lst.Add(fixCode);
            lst.Add("S" + sign);//必须有
            if (!string.IsNullOrEmpty(startdate))
                lst.Add("M" + startdate);
            if (!string.IsNullOrEmpty(ShengChanPH))
                lst.Add("L" + ShengChanPH);
            if (!string.IsNullOrEmpty(dbatchnumber))
                lst.Add("D" + dbatchnumber);
            if (!string.IsNullOrEmpty(ShiXiaoDate))
                lst.Add("E" + ShiXiaoDate);
            if (!string.IsNullOrEmpty(YouXiaoDate))
                lst.Add("V" + YouXiaoDate);
            string tmpBackCode = "." + String.Join(".", lst.Skip(1).ToArray());
            lst.Add("C" + GenVerifyCode(tmpBackCode));
            return string.Join(".", lst.ToArray());
        }

        /// <summary>
        /// 生成单个DI
        /// 2022-10-14从客户端移植生成代码，用于私有部署生成DI接口
        /// </summary>
        /// <param name="maincode">主码</param>
        /// <param name="packlevel">包装级别</param>
        /// <param name="categorycode">品类编码</param>
        /// <returns></returns>
        public string GenDI(string maincode, string packlevel, string categorycode)
        {
            string DI = maincode + "." + packlevel + categorycode;
            //1.检验品类编码特殊字符
            int count = GenVerifyPlbm(categorycode);
            if (count > 0)
            {
                DI = "error";
            }
            else
            { 
                //获取校验码
                string verifycode = GenVerifyCode(DI);
                if (verifycode.Length > 1)
                {
                    DI = "error";
                }
                else
                {
                    DI = DI + verifycode.ToString();
                }
            }
            return DI;
        }

    }
}
