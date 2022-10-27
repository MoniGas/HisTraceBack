using System;
using System.Collections.Generic;
using System.Web.Services;

namespace iagric_plant
{
    /// <summary>
    /// WinCEInterface 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class WinCEInterface : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [WebMethod]
        public LinqModel.View_WinCe_EnterpriseInfoUser userLogin(string userName)
        {
            BLL.Enterprise_UserBLL bll = new BLL.Enterprise_UserBLL();
            LinqModel.View_WinCe_EnterpriseInfoUser user = bll.GetEntityByLoginName(userName);
            return user;
        }
        /// <summary>
        /// 获取企业产品列表
        /// </summary>
        /// <param name="enterpriseID"></param>
        /// <returns></returns>
        [WebMethod]
        public List<LinqModel.View_WinCe_MaterialInfo> getMaterialLst(long enterpriseID)
        {
            BLL.MaterialBLL bll = new BLL.MaterialBLL();
            return bll.getMaterialLst(enterpriseID);
        }

        /// <summary>
        /// 获取数据库服务器时间，连接失败返回空
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string getServerTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 添加绑定记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [WebMethod]
        public bool addBindRecord(List<LinqModel.BindCodeRecords> record)
        {
            BLL.MaterialBLL bll = new BLL.MaterialBLL();
            foreach (LinqModel.BindCodeRecords r in record)
            {
                r.BindDate = DateTime.Now;
            }
            return bll.addBindRecord(record);
        }

        /// <summary>
        /// 添加出库
        /// </summary>
        /// <param name="model"></param>
        /// <param name="outID"></param>
        /// <returns></returns>
        [WebMethod]
        public bool addOutStorage(LinqModel.OutStorageTable model, out long outID)
        {
            BLL.OutStorageBLL bll = new BLL.OutStorageBLL();
            model.OutStorageDate = DateTime.Now;
            return bll.AddOutStorage(model, out outID);
        }

        /// <summary>
        /// 添加出库明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [WebMethod]
        public bool addOutStorageDetail(List<LinqModel.OutStorageDetail> model)
        {
            BLL.OutStorageBLL bll = new BLL.OutStorageBLL();
            return bll.AddOutStorageDetail(model);
        }

        /// <summary>
        /// 获取经销商信息
        /// </summary>
        /// <param name="enterialID"></param>
        /// <returns></returns>
        [WebMethod]
        public List<LinqModel.Dealer> getDealer(long enterialID)
        {
            BLL.DealerBLL bll = new BLL.DealerBLL();
            return bll.getLst(enterialID);
        }
    }
}
