
///生成码服务  通用生成码服务
///2018-04-19
///陈志钢

using System.ServiceProcess;

namespace GenCodeServiceAllInOne
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //同步国药监DI数据
            GetUDIData.Start();//新增
            //UpdateDIData.Start();//更新
            //GenCode.start();
            GetDIInfo.Start();
            //上传PI信息至发码机构 
            UpPIInfo.Start();
            //获取发码机构的PI信息
            GetPIInfo.Start();
            //生成码
            GetPICode.Start();
            UploadBatch.Start();
            //生成码包
            GenFileNew.Start();
            //ActiveCode.start();
            //扫码大数据分析功能
            ScanEwm.Start();
            //藏红包/藏优惠券
            OperateActive.Start();
            //上传DI信息至发码机构
            UpDIInfo.Start();
            //更新APP出库信息的经销商信息
            UpdateSellInfo.Start();
            //更新企业主码和token
            GetMaincode.start();
           
        }

        protected override void OnStop()
        {
        }
    }
}
