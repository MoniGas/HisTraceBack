/********************************************************************************
** 作者： 赵慧敏v2.5版本修改
** 创始时间：2017-2-09
** 联系方式 :15031109901
** 描述：拍码追溯页面
** 版本：v2.5
** 版权：研一 农业项目组  
*********************************************************************************/
using System.Collections.Generic;
using Common.Argument;
using LinqModel;
using Dal;

namespace BLL
{
    /// <summary>
    /// 拍码追溯页业务层
    /// </summary>
    public class ScanCodeBLL
    {
        ScanCodeDAL _ScanCodeDAL = new ScanCodeDAL();
        #region 根据批次获取二维码信息
        public Enterprise_FWCode_00 GetCodePreview(string bId)
        {
            Enterprise_FWCode_00 result = null;
            long id = 0;
            if (long.TryParse(bId, out id))
            {
                result = new Dal.ScanCodeDAL().GetCodeByBatch(id);
            }
            else
            {
                result = new Dal.ScanCodeDAL().GetCodeByEWM(bId);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 更新码的拍码及验证次数
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <param name="scanCount">查看次数是否加1</param>
        /// <param name="fwCount">验证次数是否加1</param>
        /// <returns>返参结果实例</returns>
        public RetResult UpdateCount(string ewm, bool scanCount, bool fwCount, int? count)
        {
            RetResult ObjRetResult = _ScanCodeDAL.UpdateCount(ewm, scanCount, fwCount, count);
            return ObjRetResult;
        }

        /// <summary>
        /// 根据二维码获取二维码信息
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public CodeInfo GetCode(string ewm, int type)
        {
            CodeInfo result = _ScanCodeDAL.GetCode(ewm, type);
            return result;
        }

        /// <summary>
        /// 根据二维码获取销售数据
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public View_Enterprise_FWCode_00 GetSaleCodeInfo(string ewm)
        {
            return _ScanCodeDAL.GetSaleCodeInfo(ewm);
        }

        /// <summary>
        /// 预览码
        /// </summary>
        /// <param name="ewm"></param>
        /// <returns></returns>
        public Enterprise_FWCode_00 GetViewCode(string ewm)
        {
            Enterprise_FWCode_00 result = _ScanCodeDAL.GetViewCode(ewm);
            return result;
        }
        #region page1
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="ewm"></param>
        public void FWValidate(string ewm)
        {
            new Dal.ScanCodeDAL().FWValidate(ewm);
        }

        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="mId">产品编号</param>
        /// <returns></returns>
        public Material GetMaterial(long mId)
        {
            Material result = _ScanCodeDAL.GetMaterial(mId);
            return result;
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <param name="ewm">二维码</param>
        /// <returns></returns>
        public ScanMaterial GetMaterialNew(long materialID, string code)
        {
            ScanMaterial result = _ScanCodeDAL.GetMaterialNew(materialID, code);
            return result;
        }
        /// <summary>
        /// 获取产品型号
        /// </summary>
        /// <param name="MaterialSpecId">型号编号</param>
        /// <returns>型号列表</returns>
        public View_MaterialSpecForMarket GetMaterialModel(long MaterialSpecId)
        {
            View_MaterialSpecForMarket result = _ScanCodeDAL.GetMaterialModel(MaterialSpecId);
            return result;
        }

        /// <summary>
        /// 查找班组信息
        /// </summary>
        /// <param name="setID">设置编号</param>
        /// <returns></returns>
        public ScanSubstation GetSubstation(long setID)
        {
            return _ScanCodeDAL.GetSubstation(setID);
        }

        /// <summary>
        /// 获取品牌
        /// </summary>
        /// <param name="bId">品牌编号</param>
        /// <returns></returns>
        public Brand GetBrand(long bId)
        {
            Brand result = _ScanCodeDAL.GetBrand(bId);
            return result;
        }

        /// <summary>
        /// 产品描述
        /// </summary>
        /// <param name="code">设置编号</param>
        /// <returns></returns>
        public ScanMaterialMemo GetMaterialMemo(long materialID)
        {
            return _ScanCodeDAL.GetMaterialMemo(materialID);
        }

        /// <summary>
        /// 获取区域品牌
        /// </summary>
        /// <param name="mId">编号</param>
        /// <returns></returns>
        public Brand GetAreaBrand(long mId)
        {
            Brand result = _ScanCodeDAL.GetAreaBrand(mId);
            return result;
        }
        #endregion

        #region page2查询信息
        /// <summary>
        /// 根据批次获取检验检测报告
        /// </summary>
        /// <param name="bId">企业编号</param>
        /// <param name="beId">批次编号</param>
        /// <returns></returns>
        public List<Batch_JianYanJianYi> GetJianYanJianCe(long bId, long? beId, int type, long setId)
        {
            List<Batch_JianYanJianYi> result = _ScanCodeDAL.GetJianYanJianCe(bId, beId, type, setId);
            return result;
        }

        /// <summary>
        /// 根据批次获取巡检信息
        /// </summary>
        /// <param name="bId">企业编号</param>
        /// <param name="beId">批次编号</param>
        /// <returns></returns>
        public List<Batch_XunJian> GetXunJian(long bId, long? beId, int type, long setId)
        {
            return _ScanCodeDAL.GetXunJian(bId, beId, type, setId);
        }

        /// <summary>
        /// 根据批次获取生产信息
        /// </summary>
        /// <param name="bId">企业编号</param>
        /// <param name="beId">批次编号</param>
        /// <returns></returns>
        public List<View_ZuoYeAndZuoYeType> GetProduce(long bId, long? beId, int type, int zuoyeType, long setId)
        {
            return _ScanCodeDAL.GetProduce(bId, beId, type, zuoyeType, setId);
        }
        #endregion

        #region page2sub
        /// <summary>
        /// 获取检验检测报告
        /// </summary>
        /// <param name="jId">比那好</param>
        /// <returns></returns>
        public ScanInfo GetJianYanJianCe(long jId)
        {
            return _ScanCodeDAL.GetJianYanJianCe(jId);
        }

        /// <summary>
        /// 根据设置编号查找检测报告
        /// </summary>
        /// <param name="codeSetId"></param>
        /// <returns></returns>
        public ScanInfo GetJianYanJian(long codeSetId)
        {
            return _ScanCodeDAL.GetJianYanJian(codeSetId);
        }

        /// <summary>
        /// 获取巡检信息
        /// </summary>
        /// <param name="xId">编号</param>
        /// <returns></returns>
        public ScanInfo GetXunJian(long xId)
        {
            return _ScanCodeDAL.GetXunJian(xId);
        }

        /// <summary>
        /// 获取生产/加工信息
        /// </summary>
        /// <param name="pId">编号</param>
        /// <returns></returns>
        public ScanInfo GetProduce(long pId)
        {
            return _ScanCodeDAL.GetProduce(pId);
        }

        /// <summary>
        /// 获取原料信息
        /// </summary>
        /// <param name="sId">设置编号</param>
        /// <returns></returns>
        public ScanInfo GetYuanliaoSub(long sId)
        {
            return _ScanCodeDAL.GetYuanliaoSub(sId);
        }
        #endregion

        #region page2按照设置查询信息
        public List<View_RequestOrigin> GetYuanliao(long? setId)
        {
            List<View_RequestOrigin> result = _ScanCodeDAL.GetYuanliao(setId);
            return result;
        }
        #endregion

        #region page3
        #region 企业信息
        public Enterprise_Info GetEnterprise(long eId)
        {
            return new Dal.ScanCodeDAL().GetEnterprise(eId);
        }
        #endregion

        #region 经销商信息
        public Dealer GetDealer(long dId)
        {
            return new Dal.ScanCodeDAL().GetDealer(dId);
        }
        #endregion

        #region 投诉
        public RetResult AddComplaint(Complaint model)
        {
            return new Dal.ScanCodeDAL().AddComplaint(model);
        }
        #endregion
        #endregion

        /// <summary>
        /// 仓储信息
        /// </summary>
        /// <returns></returns>
        public ScanWareHouseInfo GetWareHouse(long setID)
        {
            return _ScanCodeDAL.GetWareHouse(setID);
        }

        /// <summary>
        /// 物流信息
        /// </summary>
        /// <returns></returns>
        public ScanLogistics GetLogistics(long setID)
        {
            return _ScanCodeDAL.GetLogistics(setID);
        }

        /// <summary>
        /// 企业信息
        /// </summary>
        /// <returns></returns>
        public ScanEnterprise GetEnterpriseInfo(long enterpriseID)
        {
            return _ScanCodeDAL.GetEnterpriseMemo(enterpriseID);
        }

        /// <summary>
        /// 商城信息
        /// </summary>
        /// <returns></returns>
        public ShopInfo GetShop(long enterpriseID)
        {
            return _ScanCodeDAL.GetShop(enterpriseID);
        }
        public EnterpriseShopLink ShopEn(long enterpriseID)
        {
            return _ScanCodeDAL.ShopEn(enterpriseID);
        }
        /// <summary>
        /// 获取班组详情
        /// </summary>
        /// <param name="batchZyId"></param>
        /// <returns></returns>
        public View_BatchZuoye GetBatchZuoye(long batchZyId)
        {
            return _ScanCodeDAL.GetBatchZuoye(batchZyId);
        }

        /// <summary>
        ///  获取产品评价
        /// </summary>
        /// <param name="marterialId">产品id</param>
        /// <returns></returns>
        public List<NewComplaint> GetEvaluation(long marterialId, long enterpriseId)
        {
            return _ScanCodeDAL.GetEvaluation(marterialId, enterpriseId);
        }

        /// <summary>
        /// 获取产品实施视频
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public MaterialShopLink GetMaterialShopLike(long materialId)
        {
            return _ScanCodeDAL.GetMaterialShopLike(materialId);
        }

        /// <summary>
        /// 判断是否有视频
        /// </summary>
        /// <param name="materialId">产品编号</param>
        /// <returns></returns>
        public bool MaterialVideo(long materialId)
        {
            return _ScanCodeDAL.MaterialVideo(materialId);
        }

        public ScanInfo GetMaterialVideo(long materialId)
        {
            return _ScanCodeDAL.GetMaterialVideo(materialId);
        }

        /// <summary>
        /// 红包零钱记录
        /// </summary>
        /// <returns></returns>
        public List<View_RedSendChange> GetSendChange(long userId)
        {
            return new ScanCodeDAL().GetSendChange(userId);
        }

        /// <summary>
        /// 红包零钱记录
        /// </summary>
        /// <returns></returns>
        public List<View_SendedChange> SendedChange(long userId)
        {
            return new ScanCodeDAL().SendedChange(userId);
        }

        /// <summary>
        /// 获取模板4图片信息
        /// </summary>
        /// <param name="eid">企业ID</param>
        /// <param name="rid">生成码settingID</param>
        /// <returns></returns>
        public RequestCodeSettingMuBan GetMuBanModel(long eid, long rid)
        {
            return new ScanCodeDAL().GetMuBanModel(eid, rid);
        }
    }
}
