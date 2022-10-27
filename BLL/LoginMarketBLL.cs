/********************************************************************************
** 作者：张翠霞
** 开发时间：2017-6-8
** 联系方式:13313318725
** 代码功能：用于系统登录
** 版本：v1.0
** 版权：追溯
*********************************************************************************/

using System.Collections.Generic;
using LinqModel;
using Dal;
using Common.Argument;

namespace BLL
{
    public class LoginMarketBLL
    {
        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="uName">用户名</param>
        /// <param name="uPwd">密码</param>
        /// <returns></returns>
        public RetResult EnterpriseLogin(string uName, string uPwd)
        {
            LoginMarketDAL dal = new LoginMarketDAL();
            RetResult result = new RetResult();
            View_EnterpriseInfoUser model = new View_EnterpriseInfoUser();
            result = dal.EnterpriseLogin(uName, uPwd);
            return result;
        }

        /// <summary>
        /// 获取最新进行中的活动
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <returns></returns>
        public YX_ActivitySub GetAcSub(long eid)
        {
            LoginMarketDAL dal = new LoginMarketDAL();
            YX_ActivitySub model = dal.GetAcSub(eid);
            return model;
        }

        /// <summary>
        /// 获取今日扫码用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_Redactivity_ScanRecord> GetScanRecord()
        {
            LoginMarketDAL dal = new LoginMarketDAL();
            List<YX_Redactivity_ScanRecord> model = dal.GetScanRecord(SessCokie.Get.EnterpriseID);
            return model;
        }

        /// <summary>
        /// 获取昨日扫码用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_Redactivity_ScanRecord> GetScanRecordZ()
        {
            LoginMarketDAL dal = new LoginMarketDAL();
            List<YX_Redactivity_ScanRecord> model = dal.GetScanRecordZ(SessCokie.Get.EnterpriseID);
            return model;
        }

        /// <summary>
        /// 获取今日领取红包的用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_RedGetRecord> RedGetRecord()
        {
            LoginMarketDAL dal = new LoginMarketDAL();
            List<YX_RedGetRecord> model = dal.RedGetRecord(SessCokie.Get.EnterpriseID);
            return model;
        }

        /// <summary>
        /// 获取昨日领取红包的用户数量
        /// </summary>
        /// <returns></returns>
        public List<YX_RedGetRecord> RedGetRecordZ()
        {
            LoginMarketDAL dal = new LoginMarketDAL();
            List<YX_RedGetRecord> model = dal.RedGetRecordZ(SessCokie.Get.EnterpriseID);
            return model;
        }

        /// <summary>
        /// 监管登录
        /// </summary>
        /// <param name="uName">用户名</param>
        /// <param name="uPwd">密码</param>
        /// <returns></returns>
        public RetResult AdminLogin(string uName, string uPwd)
        {
            LoginMarketDAL dal = new LoginMarketDAL();
            RetResult result = new RetResult();
            View_EnterpriseInfoUser model = new View_EnterpriseInfoUser();
            result = dal.AdminLogin(uName, uPwd);
            return result;
        }
    }
}
