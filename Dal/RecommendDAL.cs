/********************************************************************************
** 作者： 赵慧敏
** 创始时间：2017-3-19
** 联系方式 :13313318725
** 描述：推荐产品
** 版本：v1.0
** 版权：研一 农业项目组  
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using Common.Log;
using Common.Argument;

namespace Dal
{
    /// <summary>
    /// 推荐产品
    /// </summary>
    public class RecommendDAL : DALBase
    {
        /// <summary>
        /// 查询企业推荐产品列表
        /// </summary>
        /// <param name="enterpriseId">企业编码</param>
        /// <param name="name">名称</param>
        /// <param name="totalCount">数量</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public List<View_Recommend> GetList(long enterpriseId, string name, out long totalCount, int pageIndex)
        {
            List<View_Recommend> result = null;
            totalCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = dataContext.View_Recommend.Where(m => m.EnterpriseID == enterpriseId && m.Type == 1);
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.RecommendName.Contains(name.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RecommendID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    for (int i = 0; i < result.Count; i++)
                    {
                        try
                        {
                            Enterprise_FWCode_00 codeModel = RecommendCode(result[i].SettingID.Value);
                            result[i].Code = codeModel.EWM;
                        }
                        catch { continue; }
                    }
                    ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "RecommendDAL.GetList():View_Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }

        #region 获取二维码
        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="sId">码批次编号</param>
        /// <returns></returns>
        private Enterprise_FWCode_00 RecommendCode(long sId)
        {
            Enterprise_FWCode_00 result = null;
            RequestCodeSetting settingCode = new RequestCodeSetting();
            RequestCode rCode = new RequestCode();
            using (DataClassesDataContext dataContextWeb = GetDataContext())
            {
                settingCode = dataContextWeb.RequestCodeSetting.FirstOrDefault(m => m.ID == sId);
                long rId = settingCode.RequestID;
                rCode = dataContextWeb.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                
                long sbeginCode = settingCode.beginCode.Value;
                long sendCode = settingCode.endCode.Value;
                long rbeginCode = rCode.StartNum == null ? 0 : rCode.StartNum.Value;
                long rendCode = rCode.EndNum == null ? 0 : rCode.EndNum.Value;
                long beginCode = rbeginCode + sbeginCode - 1;
                long endCode = rbeginCode + sendCode - 1;
                long createCount = rCode.TotalNum.GetValueOrDefault(0);
                long saleCount = rCode.saleCount.GetValueOrDefault(0);
                string tablename = "";
                using (DataClassesDataContext dataContext = GetCodeNewDataContext(rId, out tablename))
                {
                    if (dataContext != null)
                    {
                        dataContext.CommandTimeout = 600;
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("select top 1 * from " + tablename + " where 1=1");
                        strSql.Append(" and RequestCode_ID=" + rId + "and Enterprise_FWCode_ID>=" + beginCode + "and Enterprise_FWCode_ID<=" + endCode);
                        strSql.Append(" order by UseTime desc");
                        result = dataContext.ExecuteQuery<Enterprise_FWCode_00>(strSql.ToString()).FirstOrDefault();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取码
        /// </summary>
        /// <param name="rId">申请码编号</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        private DataClassesDataContext GetCodeNewDataContext(long rId, out string tablename)
        {
            string datasource = "";
            string database = "";
            string username = "";
            string pass = "";
            tablename = "";
            DataClassesDataContext result = null;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    RequestCode code = dataContext.RequestCode.FirstOrDefault(m => m.RequestCode_ID == rId);
                    long table_id = code.Route_DataBase_ID == null ? 0 : code.Route_DataBase_ID.Value;
                    Route_DataBase table = dataContext.Route_DataBase.FirstOrDefault(m => m.Route_DataBase_ID == table_id);
                    if (table == null)
                    {
                        return null;
                    }
                    datasource = table.DataSource;
                    database = table.DataBaseName;
                    username = table.UID;
                    pass = table.PWD;
                    tablename = table.TableName;

                }
                result = GetDataContext(datasource, database, username, pass);
            }
            catch { throw; }
            return result;
        }
        #endregion

        /// <summary>
        /// 企业新增推荐产品
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        public RetResult Add(Recommend model)
        {
            string msg = "推荐产品失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    if (new EnterpriseSwitchDAL().GetIsOn(model.EnterpriseID, (int)Common.EnumFile.EnterpriseSwitch.Recommend) == null)
                    {
                        msg = "请先开通推荐产品服务！";
                    }
                    else
                    {
                        if (dataContext.Recommend.FirstOrDefault(m => m.MaterialID == model.MaterialID) == null)
                        {
                            try
                            {
                                model.CodeIndex = dataContext.RequestCodeSetting.FirstOrDefault(m => m.ID == model.SettingID).beginCode;
                            }
                            catch { }
                            dataContext.Recommend.InsertOnSubmit(model);
                            dataContext.SubmitChanges();
                            msg = "推荐产品成功！";
                            error = CmdResultError.NONE;
                        }
                        else
                        {
                            msg = "已经推荐过该产品！";
                        }
                    }
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 取消推荐
        /// </summary>
        /// <param name="id">企业编号</param>
        /// <returns></returns>
        public RetResult Del(long id)
        {
            string msg = "取消推荐失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    dataContext.Recommend.DeleteOnSubmit(
                        dataContext.Recommend.FirstOrDefault(m => m.RecommendID == id)
                    );
                    dataContext.SubmitChanges();
                    msg = "取消推荐成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 审核推荐产品
        /// </summary>
        /// <param name="id">企业编号</param>
        /// <param name="type"审核结果></param>
        /// <returns></returns>
        public RetResult Verify(long id, int type)
        {
            string msg = "审核推荐产品失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    Recommend recommend = dataContext.Recommend.FirstOrDefault(m => m.RecommendID == id);
                    if (recommend != null)
                    {
                        recommend.Verify = type;
                        dataContext.SubmitChanges();
                        msg = "审核推荐产品成功！";
                        error = CmdResultError.NONE;
                    }
                    else
                    {
                        msg = "没有找到要审核的推荐产品！";
                    }
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 查询推荐企业列表
        /// </summary>
        /// <param name="adminId">管理编号</param>
        /// <param name="name">企业名称</param>
        /// <param name="totalCount">数量</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="type">审核类型</param>
        /// <returns></returns>
        public List<View_Recommend> GetAdminList(long adminId, string name, out long totalCount, int pageIndex, int type)
        {
            List<View_Recommend> result = null;
            totalCount = 0;
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = from m in dataContext.View_Recommend where m.Type == type select m;
                    if (adminId != 16)
                    {
                        data = data.Where(m => m.PRRU_PlatForm_ID == adminId);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        data = data.Where(m => m.EnterpriseName.Contains(name.Trim()));
                    }
                    totalCount = data.Count();
                    result = data.OrderByDescending(m => m.RecommendID).Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();
                    ClearLinqModel(result);
                }
            }
            catch (Exception ex)
            {
                string errData = "RecommendDAL.GetList():View_Recommend表";
                WriteLog.WriteErrorLog(errData + ":" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 查询要推荐企业的列表
        /// </summary>
        /// <param name="platId">监管部门编号</param>
        /// <returns></returns>
        public List<Enterprise_Info> GetEnterpriseList(long platId)
        {
            List<Enterprise_Info> result = new List<Enterprise_Info>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    var data = from m in dataContext.View_Recommend where m.Type == 1 select m;
                    if (platId != 16)
                    {
                        data = data.Where(m => m.PRRU_PlatForm_ID == platId);
                    }
                    foreach (var item in data)
                    {
                        if (result.FirstOrDefault(m => m.Enterprise_Info_ID == item.EnterpriseID) == null
                            && dataContext.Recommend.FirstOrDefault(m => m.EnterpriseID == item.EnterpriseID && m.Type == 2) == null)
                        {
                            result.Add(dataContext.Enterprise_Info.FirstOrDefault(m => m.Enterprise_Info_ID == item.EnterpriseID));
                        }
                    }
                    ClearLinqModel(result);
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 推荐企业
        /// </summary>
        /// <param name="arrayId">企业编号</param>
        /// <returns></returns>
        public RetResult AdminAdd(string arrayId, long platForm_ID)
        {
            string msg = "推荐企业失败！";
            CmdResultError error = CmdResultError.EXCEPTION;
            try
            {
                using (var dataContext = GetDataContext())
                {
                    List<Recommend> addList = new List<Recommend>();
                    int number = dataContext.Recommend.Where(p => p.PlatForm_ID == platForm_ID).ToList().Count();
                    if (number >= 50)
                    {
                        msg = "您目前已经推荐了【" + number + "】企业，最多推荐50家企业！";
                        Ret.SetArgument(error, msg, msg);
                        return Ret;
                    }
                    foreach (var item in arrayId.Split(','))
                    {
                        Recommend temp = new Recommend();
                        temp.AddTime = DateTime.Now;
                        temp.EnterpriseID = Convert.ToInt64(item);
                        temp.Type = 2;
                        temp.Verify = 1;
                        temp.PlatForm_ID = platForm_ID;
                        addList.Add(temp);
                    }
                    dataContext.Recommend.InsertAllOnSubmit(addList);
                    dataContext.SubmitChanges();
                    msg = "推荐企业成功！";
                    error = CmdResultError.NONE;
                }
            }
            catch { }
            Ret.SetArgument(error, msg, msg);
            return Ret;
        }

        /// <summary>
        /// 拍码页码查询推荐产品
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public List<MyRecommend> GetScanRecommend(long enterpriseId, long materialId)
        {
            List<MyRecommend> result = new List<MyRecommend>();
            List<MyRecommend> material = new List<MyRecommend>();
            List<MyRecommend> enterprise = new List<MyRecommend>();
            try
            {
                using (DataClassesDataContext dataContext = GetDataContext())
                {
                    long addressID = (long)dataContext.Enterprise_Info.FirstOrDefault(n => n.Enterprise_Info_ID == enterpriseId).Dictionary_AddressSheng_ID;
                    long typeID = (long)dataContext.Material.FirstOrDefault(l => l.Material_ID == materialId).Dictionary_MaterialType_ID;
                    List<View_Recommend> liMaterial = dataContext.View_Recommend.Where(m => m.Dictionary_AddressSheng_ID != addressID
                        && m.Dictionary_MaterialType_ID != typeID && m.Type == 1).OrderBy(m => Guid.NewGuid()).ToList();
                    int number = liMaterial.Count() > 3 ? 3 : liMaterial.Count();
                    List<int> list = GenerateRandom(liMaterial.Count(), number);
                    foreach (var sub in list)
                    {
                        MyRecommend model = new MyRecommend();
                        Enterprise_FWCode_00 fwCode = RecommendCode(liMaterial[sub].SettingID.Value);
                        if (fwCode == null)
                        {
                            continue;
                        }
                        model.code = fwCode.EWM;
                        model.name = liMaterial[sub].MaterialName;
                        model.type = "1";
                        material.Add(model);
                    }
                    var liEnterprise = dataContext.View_Recommend.Where(m => m.Type == 2 && m.EnterpriseID != enterpriseId).GroupBy(m => m.EnterpriseID).ToList();
                    number = liEnterprise.Count() > 3 ? 3 : liEnterprise.Count();
                    list = GenerateRandom(liEnterprise.Count(), number);
                    foreach (var obj in list)
                    {
                        foreach (var sub in liEnterprise[obj])
                        {
                            View_Recommend temp = dataContext.View_Recommend.
                                OrderBy(m => Guid.NewGuid()).FirstOrDefault(m => m.EnterpriseID == sub.EnterpriseID
                                && m.Type == 1 && m.Dictionary_MaterialType_ID != typeID);
                            if (temp == null)
                            {
                                continue;
                            }
                            MyRecommend model = new MyRecommend();
                            Enterprise_FWCode_00 fwCode = RecommendCode(temp.SettingID.Value);
                            if (fwCode == null)
                            {
                                continue;
                            }
                            model.code = fwCode.EWM;
                            model.name = temp.MaterialName;
                            model.type = "2";
                            enterprise.Add(model);
                        }
                    }
                    //数量不够3个的时候凑够3个
                    if (material.Count != enterprise.Count)
                    {
                        if (material.Count > 0 && material.Count < 3)
                        {
                            for (int i = 0; i < material.Count; i++)
                            {
                                if (i == material.Count - 1)
                                {
                                    i = 0;
                                }
                                if (material.Count < 3)
                                {
                                    material.Add(material[i]);
                                }
                                if (material.Count == 3)
                                {
                                    break;
                                }
                            }
                        }
                        if (enterprise.Count > 0 && enterprise.Count < 3)
                        {
                            for (int i = 0; i < enterprise.Count; i++)
                            {
                                if (i == enterprise.Count - 1)
                                {
                                    i = 0;
                                }
                                if (enterprise.Count < 3)
                                {
                                    enterprise.Add(enterprise[i]);
                                }
                                if (enterprise.Count == 3)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    foreach (var sub in material)
                    {
                        result.Add(sub);
                    }
                    foreach (var sub in enterprise)
                    {
                        result.Add(sub);
                    }
                }
            }
            catch 
            {
            }
            return result;
        }

        /// <summary>
        /// 随机生成数组列表
        /// </summary>
        /// <param name="iMax">最大数</param>
        /// <param name="iNum">选择数量</param>
        /// <returns></returns>
        List<int> GenerateRandom(int iMax, int iNum)
        {
            List<int> lstRet = new List<int>();
            if (iMax <= 3)
            {
                for (int i = 0; i < iMax; i++)
                {
                    lstRet.Add(i);
                }
                return lstRet;
            }
            Random random = new Random();
            while(lstRet.Count<iNum)
            {
                int nValue = random.Next(0, iMax);
                if (!lstRet.Contains(nValue))
                    lstRet.Add(nValue);
            }
            return lstRet;
        }
    }
}
