/**********************
 企业授权加密算法
2020.2.3
 * 赵慧敏
***********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Common.Tools
{
    public class SecurityDES
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">被加密的明文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>Base64编码密文</returns>
        public static string Encrypt(string value, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cs);
            sw.Write(value);
            sw.Flush();
            cs.FlushFinalBlock();
            ms.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">被解密的密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>明文</returns>
        public static string Decrypt(string value, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            byte[] buffer = Convert.FromBase64String(value);
            MemoryStream ms = new MemoryStream(buffer);
            CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(key, iv), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
