using System;

namespace Common.Tools
{
    public class VideoCutPic
    {
        /// <param name="VideoName">视频文件pic/guiyu.mov</param>
        /// <param name="WidthAndHeight">图片的尺寸如:240*180</param>
        /// <param name="CutTimeFrame">开始截取的时间如:"1"</param>
        #region 从视频画面中截取一帧画面为图片
        public string GetPicFromVideo(string VideoName, string WidthAndHeight, string CutTimeFrame)
        {
            string ffmpeg = "D:/ffmpeg.exe";
            string PicName = "D:/jietu.jpg";    //Server.MapPath(Guid.NewGuid().ToString().Replace("-", "") + ".jpg");
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.Arguments = " -i " + VideoName + " -y -f image2 -ss " + CutTimeFrame + " -t 0.001 -s " + WidthAndHeight + " " + PicName;  //設定程式執行參數
            try
            {
                System.Diagnostics.Process.Start(startInfo);
                return PicName;
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
        #endregion
    }
}
