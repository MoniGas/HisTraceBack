using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel.InterfaceModels;
using Dal;

namespace BLL
{
    public class CheckBLL
    {
        CheckDAL dal = new CheckDAL();
        /// <summary>
        /// 扫码查看稽查信息
        /// </summary>
        /// <param name="scanCode"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public InterfaceResult getCodeInfo(string scanCode, string accessToken)
        {
            InterfaceResult result = dal.getCodeInfo(scanCode, accessToken);
            return result;
        }

        /// <summary>
        /// 提交稽查结果
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public InterfaceResult MarketCheck(AddCheckRequest request, string accessToken)
        {
            InterfaceResult result = dal.MarketCheck(request, accessToken);
            return result;
        }

        /// <summary>
        /// 获取稽查记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public InterfaceResult GetCheckList(CheckRecordRequest request, string accessToken)
        {
            InterfaceResult result = dal.GetCheckList(request, accessToken);
            return result;
        }
    }
}
