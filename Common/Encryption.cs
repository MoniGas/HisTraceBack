using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class Encryption
    {
        // 代表加密算法，切记不能有"Y"和"N"，因为这两个字符分别代表的是“图形防伪”和“数字防伪”
        private string[] array_List = { "A", "G", "R", "J", "L", "S" };

        public Dictionary<char, string> GetDictionary(string codeType)
        {
            Dictionary<char, string> objDictionary = new Dictionary<char, string>();

            switch (codeType)
            {
                case "A":
                    A(objDictionary);
                    return objDictionary;
                case "G":
                    G(objDictionary);
                    return objDictionary;
                case "R":
                    R(objDictionary);
                    return objDictionary;
                case "J":
                    J(objDictionary);
                    return objDictionary;
                case "L":
                    L(objDictionary);
                    return objDictionary;
                case "S":
                    S(objDictionary);
                    return objDictionary;
            }

            return null;
        }

        private void A(Dictionary<char, string> objDictionary)
        {
            objDictionary.Add('.', "M");
            objDictionary.Add('0', "K");
            objDictionary.Add('1', "$");
            objDictionary.Add('2', "A");
            objDictionary.Add('3', "F");
            objDictionary.Add('4', "=");
            objDictionary.Add('5', "L");
            objDictionary.Add('6', "v");
            objDictionary.Add('7', "T");
            objDictionary.Add('8', "c");
            objDictionary.Add('9', "Y");

            objDictionary.Add('M', ".");
            objDictionary.Add('K', "0");
            objDictionary.Add('$', "1");
            objDictionary.Add('A', "2");
            objDictionary.Add('F', "3");
            objDictionary.Add('=', "4");
            objDictionary.Add('L', "5");
            objDictionary.Add('v', "6");
            objDictionary.Add('T', "7");
            objDictionary.Add('c', "8");
            objDictionary.Add('Y', "9");
        }

        private void G(Dictionary<char, string> objDictionary)
        {
            objDictionary.Add('.', "E");
            objDictionary.Add('0', "U");
            objDictionary.Add('1', "P");
            objDictionary.Add('2', "G");
            objDictionary.Add('3', "L");
            objDictionary.Add('4', "W");
            objDictionary.Add('5', "=");
            objDictionary.Add('6', "$");
            objDictionary.Add('7', "Q");
            objDictionary.Add('8', "M");
            objDictionary.Add('9', "Z");

            objDictionary.Add('E', ".");
            objDictionary.Add('U', "0");
            objDictionary.Add('P', "1");
            objDictionary.Add('G', "2");
            objDictionary.Add('L', "3");
            objDictionary.Add('W', "4");
            objDictionary.Add('=', "5");
            objDictionary.Add('$', "6");
            objDictionary.Add('Q', "7");
            objDictionary.Add('M', "8");
            objDictionary.Add('Z', "9");
        }

        private void R(Dictionary<char, string> objDictionary)
        {
            objDictionary.Add('.', "H");
            objDictionary.Add('0', "Q");
            objDictionary.Add('1', "E");
            objDictionary.Add('2', "A");
            objDictionary.Add('3', "$");
            objDictionary.Add('4', "=");
            objDictionary.Add('5', "B");
            objDictionary.Add('6', "Z");
            objDictionary.Add('7', "T");
            objDictionary.Add('8', "S");
            objDictionary.Add('9', "F");

            objDictionary.Add('H', ".");
            objDictionary.Add('Q', "0");
            objDictionary.Add('E', "1");
            objDictionary.Add('A', "2");
            objDictionary.Add('$', "3");
            objDictionary.Add('=', "4");
            objDictionary.Add('B', "5");
            objDictionary.Add('Z', "6");
            objDictionary.Add('T', "7");
            objDictionary.Add('S', "8");
            objDictionary.Add('F', "9");
        }

        private void J(Dictionary<char, string> objDictionary)
        {
            objDictionary.Add('.', "=");
            objDictionary.Add('0', "W");
            objDictionary.Add('1', "R");
            objDictionary.Add('2', "O");
            objDictionary.Add('3', "B");
            objDictionary.Add('4', "Q");
            objDictionary.Add('5', "V");
            objDictionary.Add('6', "M");
            objDictionary.Add('7', "K");
            objDictionary.Add('8', "G");
            objDictionary.Add('9', "P");

            objDictionary.Add('=', ".");
            objDictionary.Add('W', "0");
            objDictionary.Add('R', "1");
            objDictionary.Add('O', "2");
            objDictionary.Add('B', "3");
            objDictionary.Add('Q', "4");
            objDictionary.Add('V', "5");
            objDictionary.Add('M', "6");
            objDictionary.Add('K', "7");
            objDictionary.Add('G', "8");
            objDictionary.Add('P', "9");
        }

        private void L(Dictionary<char, string> objDictionary)
        {
            objDictionary.Add('.', "=");
            objDictionary.Add('0', "U");
            objDictionary.Add('1', "P");
            objDictionary.Add('2', "$");
            objDictionary.Add('3', "W");
            objDictionary.Add('4', "A");
            objDictionary.Add('5', "D");
            objDictionary.Add('6', "J");
            objDictionary.Add('7', "X");
            objDictionary.Add('8', "M");
            objDictionary.Add('9', "C");

            objDictionary.Add('=', ".");
            objDictionary.Add('U', "0");
            objDictionary.Add('P', "1");
            objDictionary.Add('$', "2");
            objDictionary.Add('W', "3");
            objDictionary.Add('A', "4");
            objDictionary.Add('D', "5");
            objDictionary.Add('J', "6");
            objDictionary.Add('X', "7");
            objDictionary.Add('M', "8");
            objDictionary.Add('C', "9");
        }

        private void S(Dictionary<char, string> objDictionary)
        {
            objDictionary.Add('.', "I");
            objDictionary.Add('0', "E");
            objDictionary.Add('1', "Y");
            objDictionary.Add('2', "F");
            objDictionary.Add('3', "D");
            objDictionary.Add('4', "L");
            objDictionary.Add('5', "S");
            objDictionary.Add('6', "Z");
            objDictionary.Add('7', "$");
            objDictionary.Add('8', "V");
            objDictionary.Add('9', "N");

            objDictionary.Add('I', ".");
            objDictionary.Add('E', "0");
            objDictionary.Add('Y', "1");
            objDictionary.Add('F', "2");
            objDictionary.Add('D', "3");
            objDictionary.Add('L', "4");
            objDictionary.Add('S', "5");
            objDictionary.Add('Z', "6");
            objDictionary.Add('$', "7");
            objDictionary.Add('V', "8");
            objDictionary.Add('N', "9");
        }

        /// <summary>
        /// 明文流水号和加密流水号的转换
        /// </summary>
        /// <param name="serial">流水号</param>
        /// <param name="numR">加密数组标识，解密时该参数直接写0，加密时为随机数字0~5</param>
        /// <returns></returns>
        public string Algorithm(string serial,int numR)
        {
            string result = string.Empty;
            string type = serial.Substring(serial.Length - 1, 1);
            // 判断为加密操作
            if (VerifyDigital(type))
            {
                Dictionary<char, string> objDictionary = GetDictionary(array_List[numR]);

                char[] serialArray = serial.ToCharArray();

                foreach (char charTemp in serialArray)
                {
                    result += objDictionary[charTemp];
                }

                result += array_List[numR];
            }
            else // 判断为解密操作
            {
                string index = serial.Substring(serial.Length - 1, 1);
                Dictionary<char, string> objDictionary = GetDictionary(index);
                char[] serialArray = serial.Substring(0, serial.Length - 1).ToCharArray();
                foreach (char charTemp in serialArray)
                {
                    // 判断加密数组中存在该字符，否则直接返回该字符
                    if (objDictionary.ContainsKey(charTemp))
                    {
                        result += objDictionary[charTemp];
                    }
                    else 
                    {
                        result += charTemp.ToString();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 加密二维码转换成明文二维码
        /// </summary>
        /// <param name="ewm">完整的二维码</param>
        /// <returns>完整的明文二维码</returns>
        public string CodeDecrypt(string ewm)
        {
            if (string.IsNullOrEmpty(ewm))
            {
                return "";
            }
            try
            {
                string[] EwmArray = ewm.Split('.');

                if (EwmArray.Length == 1)
                {
                    return ewm;
                }
                // 判断是明文二维码
                if (VerifyDigital(EwmArray[EwmArray.Length - 1]))
                {
                    return ewm;
                }
                else // 判断二维码为加密二维码
                {
                    // 存储加密流水号
                    string strTemp = EwmArray[EwmArray.Length - 1];
                    // 转换成明文流水号
                    return ewm.Replace(strTemp, new Encryption().Algorithm(strTemp, 0));
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 验证数字方法
        /// </summary>
        /// <param name="Character">需要被验证的字符</param>
        /// <returns>返回验证结果</returns>
        public bool VerifyDigital(string Character)
        {
            try
            {
                Convert.ToInt32(Character);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region  刘晓杰于2019年11月4日从CFBack项目移入此

        /// <summary>
        /// DES加密/解密类。
        /// </summary>
        public class DesEncrypt
        {

            #region ========加密========

            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public static string Encrypt(string text)
            {
                return Encrypt(text, "SMBack");
            }
            /// <summary> 
            /// 加密数据 
            /// </summary> 
            /// <param name="text"></param> 
            /// <param name="sKey"></param> 
            /// <returns></returns> 
            public static string Encrypt(string text, string sKey)
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray;
                inputByteArray = Encoding.Default.GetBytes(text);
                des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }

            #endregion

            #region ========解密========

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="Text"></param>
            /// <returns></returns>
            public static string Decrypt(string Text)
            {
                return Decrypt(Text, "DTcms");
            }
            /// <summary> 
            /// 解密数据 
            /// </summary> 
            /// <param name="Text"></param> 
            /// <param name="sKey"></param> 
            /// <returns></returns> 
            public static string Decrypt(string Text, string sKey)
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int len;
                len = Text.Length / 2;
                byte[] inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.Default.GetString(ms.ToArray());
            }

            #endregion

        }

        #endregion
    }
}
