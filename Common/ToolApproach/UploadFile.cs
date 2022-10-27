using System;
using System.IO;

namespace Common.ToolApproach
{
    public class UploadFile
    {
        /// <summary>  
        /// 将传进来的文件转换成字符串  
        /// </summary>  
        /// <param name="FilePath">待处理的文件路径(本地或服务器)</param>  
        /// <returns></returns>  
        public static string FileToBinary(string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read); 
            int fileLength = Convert.ToInt32(fs.Length); 
            byte[] fileByteArray = new byte[fileLength];
            BinaryReader br = new BinaryReader(fs);
            for (int i = 0; i < fileLength; i++)
            {
                br.Read(fileByteArray, 0, fileLength); 
            }

            string strData = Convert.ToBase64String(fileByteArray); 
            return strData;
        }

        /// <summary>  
        /// 装传进来的字符串保存为文件  
        /// </summary>  
        /// <param name="path">需要保存的位置路径</param>  
        /// <param name="binary">需要转换的字符串</param>  
        public static bool BinaryToFile(string path, string binary)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(Convert.FromBase64String(binary));
                bw.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件本地路径</param>
        /// <param name="savePath">服务器保存路径</param>
        /// <param name="newFileName">生成的新文件名</param>
        /// <returns></returns>
        public static bool UpFile(string filePath, string savePath, out string newFileName)
        {
            newFileName = string.Empty;
            try
            {
                string fileExt = Path.GetExtension(filePath).ToString().ToLower();//获取文件扩展名
                newFileName = DateTime.Now.ToString("yyyyMMddhhMMssfff") + fileExt;
                string fileBinary = FileToBinary(filePath);//得到文件字符串
                if (BinaryToFile(savePath + newFileName, fileBinary))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
