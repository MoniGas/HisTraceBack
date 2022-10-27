using System.ServiceProcess;
using System.Windows.Forms;

namespace GenCodeServiceAllInOne
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };
            ServiceBase.Run(servicesToRun);
            //Application.Run(new Form1());
        }
    }
}
