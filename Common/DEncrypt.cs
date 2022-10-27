using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using System.Net;
using System.Configuration;

namespace Common
{
	public class DEncrypt
	{
		//密钥
		private readonly byte[] key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["key"]);
		//向量
		private readonly byte[] iv = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["iv"]);

		#region MD5加密字符串，默认32位
		/// <summary>
		/// MD5加密字符串，默认32位
		/// </summary>
		/// <param name="str">待加密字符串</param>
		/// <returns></returns>
		public static string GetMD5String(string str)
		{
			string encryptedstr = "";
			MD5 md5 = new MD5CryptoServiceProvider();
			// 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
			byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
			// 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
			for (int i = 0; i < s.Length; i++)
			{
				//数值英文字母
				encryptedstr = encryptedstr + s[i].ToString("X2");
			}
			return encryptedstr;
		}
		#endregion

		#region DES加密
		/// <summary>
		/// DES加密
		/// </summary>
		/// <param name="value">被加密的明文</param>
		/// <returns>Base64编码密文</returns>
		public string Encrypt(string value)
		{
			try
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
			catch (Exception ex)
			{
				return "";
			}
		}
		#endregion

		#region  DES解密
		/// <summary>
		/// DES解密
		/// </summary>
		/// <param name="value">被解密的密文</param>
		/// <returns>明文</returns>
		public string Decrypt(string value)
		{
			try
			{
				DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
				byte[] buffer = Convert.FromBase64String(value);
				MemoryStream ms = new MemoryStream(buffer);
				CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(key, iv), CryptoStreamMode.Read);
				StreamReader sr = new StreamReader(cs);
				return sr.ReadToEnd();
			}
			catch (Exception ex)
			{
				return "";
			}
		}
		#endregion

		#region 获取时间戳
		/// <summary>
		/// 获取时间戳
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static string GetTimeStamp(DateTime time)
		{
			DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
			long ts = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
			return ts.ToString();
		}
		#endregion

		#region 时间戳转日期
		/// <summary>
		/// 时间戳转日期
		/// </summary>
		/// <param name="timeStamp"></param>
		/// <returns></returns>
		public DateTime ConvertToTime(string timeStamp)
		{
			DateTime time = DateTime.Now;
			DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			long lTime = long.Parse(timeStamp + "0000");
			TimeSpan toNow = new TimeSpan(lTime);
			time = dtStart.Add(toNow);
			return time;
		}
		#endregion

		#region 获取hash
		/// <summary>
		/// 获取hash
		/// </summary>
		/// <param name="functionUrl"></param>
		/// <param name="dataDic"></param>
		/// <returns></returns>
		public static string GetHash(string functionUrl, Dictionary<string, string> dataDic, string access_token_code)
		{
			//dataDic = dataDic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
			dataDic = dataDic.OrderBy(x => x.Key, new ComparerString()).ToDictionary(x => x.Key, y => y.Value);
			string dataStr = "";
			foreach (var item in dataDic)
			{
				string s = item.Key + "=" + item.Value;
				dataStr += string.IsNullOrEmpty(dataStr) ? s : ("&" + s);
			}
			string url = functionUrl + "?" + dataStr;
			string hash = GetMD5String(url + access_token_code);
			return hash;
		}

		public class ComparerString : IComparer<String>
		{
			public int Compare(String x, String y)
			{
				return string.CompareOrdinal(x, y);
			}
		}
		#endregion

		#region 获取网络时间
		//获取网络时间
		public string GetNetDateTime()
		{
			WebRequest request = null;
			WebResponse response = null;
			WebHeaderCollection headerCollection = null;
			string datetime = string.Empty;
			try
			{
				request = WebRequest.Create("https://www.baidu.com");
				request.Timeout = 3000;
				request.Credentials = CredentialCache.DefaultCredentials;
				response = (WebResponse)request.GetResponse();
				headerCollection = response.Headers;
				foreach (var h in headerCollection.AllKeys)
				{ if (h == "Date") { datetime = headerCollection[h]; } }
				return datetime;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				if (request != null)
				{ request.Abort(); }
				if (response != null)
				{ response.Close(); }
				if (headerCollection != null)
				{ headerCollection.Clear(); }
			}
		}
		#endregion

	}
}
