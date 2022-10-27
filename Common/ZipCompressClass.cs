using System;
using System.Text;
using Ionic.Zip;

namespace Common
{
    /// <summary>  
    /// 压缩和解压文件  
    /// </summary>  
    public class ZipCompressClass
    {
        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="FilePath">需要压缩文件夹路径</param>
        /// <param name="FileName">压缩后文件名</param>
        /// <param name="SavePath">压缩后存放路径</param>
        /// <param name="PassWord">压缩密码，null为无密码</param>
        /// <returns>异常消息，成功返回null</returns>
        public string SetZipFile(string FilePath, string SavePath, string PassWord)
        {
            try
            {
                ZipFile zipfile = new ZipFile(SavePath, Encoding.Default);
                if (!string.IsNullOrEmpty(PassWord))
                    zipfile.Password = PassWord;
                zipfile.AddDirectory(FilePath);
                zipfile.Save();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return null;
        }
    }  
}
