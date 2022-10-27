using System.Windows.Forms;

namespace GenCodeServiceAllInOne
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           // GetMaincode.DOGetMainCode();
           //GetDIInfo.Start();
           //GetPICode.GetCodeInfo();
            //GetUDIData.DOGetUDIData();
            UpdateDIData.DOGetUDIData();
            //GetPIInfo.Start();
            // OperateActive.ConcealRed();
            //UploadBatch.Start();
            // GenFileNew.Start();
            //ActiveCode.doActiveCodeNew();
            // ScanEwm.DoScanEwm();
            //UpPIInfo.UpPIDataInfo();
            //UpDIInfo.DOUpDIInfo();//上传DI到发码机构
            //UpdateSellInfo.Start();//为APP出库的二维码赋经销商信息和销售日期
            //同步国药监DI数据
           // GetUDIData.Start();
            MessageBox.Show(this, "");
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            GetUDIData.DOGetUDIData();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            UpdateDIData.DoUpdateDIData();
        }
    }
}