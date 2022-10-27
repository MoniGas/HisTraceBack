using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqModel;
using System.Security.Cryptography;
using System.IO;

namespace InterfaceWeb
{
    public class BaseDataDAL
    {
        static string interfaceUrl = System.Configuration.ConfigurationManager.AppSettings["interfaceUrl"].ToString().Trim();
        static string access_token = System.Configuration.ConfigurationManager.AppSettings["access_token"].ToString().Trim();
        static string access_token_code = System.Configuration.ConfigurationManager.AppSettings["access_token_code"].ToString().Trim();
        static string parseUrl = System.Configuration.ConfigurationManager.AppSettings["IpRedirect"].ToString().Trim();
        static WebClient _WebClient = null;

        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <returns></returns>
        public static AreaInfo GetAllAreas()
        {
            AreaInfo result = new AreaInfo();
            if (_WebClient == null)
            {
                _WebClient = new WebClient();
                _WebClient.Encoding = Encoding.UTF8;
            }
            try
            {
                string functionUrl = "/AllInterFace/GetAllAreaInfo";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                result = JsonDes.JsonDeserialize<AreaInfo>(strResult);
            }
            catch
            {
                result.ResultCode = -1;
                result.ResultMsg = "连接接口失败！";
            }
            return result;
        }
        /// <summary>
        /// 获取行业信息
        /// </summary>
        /// <returns></returns>
        public static TradeInfo GetAllTrade()
        {
            TradeInfo result = new TradeInfo();
            if (_WebClient == null)
            {
                _WebClient = new WebClient();
                _WebClient.Encoding = Encoding.UTF8;
            }
            try
            {
                //string url = _GetAllTradeInfo + _Token;
                //string strResult = _WebClient.OpenRead(url, _Token);
                //result = JsonDes.JsonDeserialize<TradeInfo>(strResult);
                string functionUrl = "/AllInterFace/GetAllTradeInfo";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                result = JsonDes.JsonDeserialize<TradeInfo>(strResult);
            }
            catch
            {
                result.ResultCode = -1;
                result.ResultMsg = "连接接口失败！";
            }
            return result;
        }
        /// <summary>
        /// 根据企业名称查询企业是否注册
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static Organunitinfo GetCompany(string companyName)
        {
            Organunitinfo obj = new Organunitinfo();
            if (!string.IsNullOrEmpty(companyName))
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                try
                {
                    string functionUrl = "/AllInterFace/GetCompanyInfo";
                    Dictionary<string, string> dataDic = new Dictionary<string, string>
                    {
                        {"access_token", access_token},
                        {"companyName", companyName},
                        {"searchType", "1"},
                        {"time", GetTimeStamp(DateTime.Now)}
                    };
                    dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                    string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                    obj = JsonDes.JsonDeserialize<Organunitinfo>(strResult);
                }
                catch (Exception)
                {
                    obj.ResultCode = -1;
                    obj.ResultMsg = "连接接口失败！";
                }
            }
            return obj;
        }
        /// <summary>
        /// 登录成功，返回企业信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static OrganUnit GetUnitInfo(string userName, string pwd)
        {
            OrganUnit unit = new OrganUnit();
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(pwd))
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient {Encoding = Encoding.UTF8};
                }
                try
                {
                    string functionUrl = "/AllInterFace/LoginVerify";
                    Dictionary<string, string> dataDic = new Dictionary<string, string>
                    {
                        {"access_token", access_token},
                        {"loginName", userName},
                        {"loginPswd", pwd},
                        {"time", GetTimeStamp(DateTime.Now)}
                    };
                    dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                    string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                    var obj = JsonDes.JsonDeserialize<MainCodeInfo>(strResult);
                    if (obj.ResultCode == 1)
                    {
                        unit = GetCompanyBaseInfo(obj.OrganUnit_Oid);
                    }
                    else
                    {
                        unit.ResultCode = -1;
                        unit.ResultMsg = "登录失败！";
                    }
                }
                catch (Exception)
                {
                    unit.ResultCode = -1;
                    unit.ResultMsg = "连接接口失败！";
                }
            }
            return unit;
        }

        /// <summary>
        /// 根据企业IDcode查询企业信息
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static OrganUnit GetCompanyBaseInfo(string companyIDcode)
        {
            OrganUnit obj = new OrganUnit();
            if (!string.IsNullOrEmpty(companyIDcode))
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient {Encoding = Encoding.UTF8};
                }
                try
                {
                    //string url = _GetCompanyBaseInfo + _Token;
                    //url += "&companyIDcode=" + companyIDcode;
                    //string strResult = _WebClient.OpenRead(url, _Token);
                    //obj = JsonDes.JsonDeserialize<OrganUnit>(strResult);
                    string functionUrl = "/AllInterFace/GetCompanyBaseInfo";
                    Dictionary<string, string> dataDic = new Dictionary<string, string>();
                    dataDic.Add("access_token", access_token);
                    dataDic.Add("companyIDcode", companyIDcode);
                    dataDic.Add("time", GetTimeStamp(DateTime.Now));
                    dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                    string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                    obj = JsonDes.JsonDeserialize<OrganUnit>(strResult);
                }
                catch (Exception ex)
                {
                    obj.ResultCode = -1;
                    obj.ResultMsg = "连接接口失败！";
                }
            }
            return obj;
        }
        /// <summary>
        /// 企业注册1001接口
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="email"></param>
        /// <param name="OrganizeName"></param>
        /// <param name="OrganizeNameEn"></param>
        /// <param name="unitType_ID"></param>
        /// <param name="Province"></param>
        /// <param name="selCity"></param>
        /// <param name="selArea"></param>
        /// <param name="OrganizeUser"></param>
        /// <param name="OrganizeUserEn"></param>
        /// <param name="OrganizeUserTel"></param>
        /// <returns></returns>
        public static CompanyIDcode RegCompanyInfo(string userName, string pwd, string email, string OrganizeName, string OrganizeNameEn, string unitType_Code,
            string Province, string selCity, string selArea, string OrganizeUser, string OrganizeUserEn, string OrganizeUserTel, string code)
        {
            CompanyIDcode idCode = new CompanyIDcode();
            idCode.result_code = -1;
            idCode.result_msg = "注册失败！";
            if (_WebClient == null)
            {
                _WebClient = new WebClient { Encoding = Encoding.UTF8 };
            }
            try
            {
                string smsVer = code;
                if (!string.IsNullOrEmpty(smsVer))
                {
                    string functionUrl = "/sp/idcode/medical/companyinfo/reg";
                    Dictionary<string, string> dataDic = new Dictionary<string, string>
                    {
                        {"access_token", access_token},
                        {"login_name", userName},
                        {"login_password", pwd},
                        {"email", email},
                        {"organunit_name", OrganizeName},
                        {"organunit_name_en", OrganizeNameEn},
                        {"unittype_code", unitType_Code},
                        {"province_id", Province},
                        {"city_id", selCity},
                        {"area_id", selArea},
                        {"linkman", OrganizeUser},
                        {"linkman_en", OrganizeUserEn},
                        {"linkphone", OrganizeUserTel},
                        {"sms_verify_code", smsVer},
                        {"time", GetTimeStamp(DateTime.Now)}
                    };
                    dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                    string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                    idCode = JsonDes.JsonDeserialize<CompanyIDcode>(strResult);
                }
            }
            catch (Exception ex)
            {
                idCode.result_code = -1;
                idCode.result_msg = "连接接口失败！";
            }
            return idCode;
        }
        /// <summary>
        /// 获取短信验证接口
        /// </summary>
        /// <param name="phoneCode"></param>
        /// <returns></returns>
        public static Result GetVerifyInfo(string phoneCode)
        {
            Result obj = new Result();
            if (!string.IsNullOrEmpty(phoneCode))
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                try
                {
                    string functionUrl = "/AllInterFace/SendVerifyInfo";
                    Dictionary<string, string> dataDic = new Dictionary<string, string>
                    {
                        {"access_token", access_token},
                        {"phoneCode", phoneCode},
                        {"time", GetTimeStamp(DateTime.Now)}
                    };
                    dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                    string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                    obj = JsonDes.JsonDeserialize<Result>(strResult);
                }
                catch (Exception)
                {
                    obj.ResultCode = -1;
                    obj.ResultMsg = "连接接口失败！";
                }
            }
            return obj;
        }
        /// <summary>
        /// 修改企业信息
        /// </summary>
        /// <param name="idcode"></param>
        /// <param name="trade"></param>
        /// <param name="address"></param>
        /// <param name="email"></param>
        /// <param name="personName"></param>
        /// <param name="telephone"></param>
        /// <returns></returns>
        public static Result ModifyCompanyInfo(string idcode, string trade, string address, string email, string personName, string telephone, string file)
        {
            Result obj = new Result
            {
                ResultCode = -1,
                ResultMsg = "连接接口失败！"
            };
            if (string.IsNullOrEmpty(idcode))
            {
                obj.ResultMsg = "请输入企业主码！";
                return obj;
            }
            if (string.IsNullOrEmpty(trade))
            {
                obj.ResultMsg = "请选择行业！";
                return obj;
            }
            if (string.IsNullOrEmpty(address))
            {
                obj.ResultMsg = "请输入企业地址！";
                return obj;
            }
            if (_WebClient == null)
            {
                _WebClient = new WebClient {Encoding = Encoding.UTF8};
            }
            try
            {
                string functionUrl = "/AllInterFace/ModifyCompanyInfo";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"companyIDcode", idcode},
                    {"trade_ID", trade},
                    {"organunitAddress", address},
                    {"organunitNameEn", ""},
                    {"organunitAddressEn", ""},
                    {"email", email},
                    {"linkMan", personName},
                    {"linkManEn", ""},
                    {"fax", telephone},
                    {"unitworkAddress", ""},
                    {"unitworkAddressEn", ""},
                    {"unitSizeType", ""},
                    {"registeredCapital", ""},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                obj = JsonDes.JsonDeserialize<Result>(strResult);
            }
            catch (Exception)
            {
                obj.ResultCode = -1;
                obj.ResultMsg = "连接接口失败！";
            }
            return obj;
        }

        #region 完善企业信息20191104医疗器械
        public static HisResult ModifyEnInfo(string idcode, string trade, string address, string email, string personName, 
            string telephone, string file,string  access,string accesCode)
        {
            HisResult obj = new HisResult
            {
                result_code = -1,
                result_msg = "连接接口失败！"
            };
            if (string.IsNullOrEmpty(idcode))
            {
                obj.result_msg = "请输入企业主码！";
                return obj;
            }
            if (string.IsNullOrEmpty(trade))
            {
                obj.result_msg = "请选择行业！";
                return obj;
            }
            if (string.IsNullOrEmpty(address))
            {
                obj.result_msg = "请输入企业地址！";
                return obj;
            }
            if (_WebClient == null)
            {
                _WebClient = new WebClient { Encoding = Encoding.UTF8 };
            }
            try
            {
                string functionUrl = "/sp/idcode/medical/companyinfo/modify";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"company_idcode", idcode},
                    {"email", email},
                    {"organunit_address", address},
                    {"organunit_name_en", ""},
                    {"organunit_address_en", ""},
                    {"linkman", personName},
                    {"linkman_en", ""},
                    {"fax", telephone},
                    {"unit_workaddress", ""},
                    {"unit_workaddress_en", ""},
                    {"unit_size_type", ""},
                    {"registered_capital", ""},
                    {"unittype_code", ""},
                    {"gotourl", ""},
                    {"linkphone", ""},
                    {"unit_logo", ""},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                obj = JsonDes.JsonDeserialize<HisResult>(strResult);
            }
            catch (Exception)
            {
                obj.result_code = -1;
                obj.result_msg = "连接接口失败！";
            }
            return obj;
        }
        #endregion

        /// <summary>
        /// 企业认证
        /// </summary>
        /// <param name="idcode">企业主码</param>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public static Result Verify(string idcode, string fileUrl, int zjType, string zjhm)
        {
            Result obj = new Result
            {
                ResultCode = -1,
                ResultMsg = "认证失败！"
            };
            if (string.IsNullOrEmpty(idcode))
            {
                obj.ResultMsg = "请输入企业主码！";
                return obj;
            }
            if (string.IsNullOrEmpty(fileUrl))
            {
                obj.ResultMsg = "请输入上传营业执照！";
                return obj;
            }
            if (zjType < 0)
            {
                obj.ResultMsg = "请选择证件类型！";
                return obj;
            }
            if (string.IsNullOrEmpty(zjhm))
            {
                obj.ResultMsg = "请输入相应的证件号码！";
                return obj;
            }
            if (_WebClient == null)
            {
                _WebClient = new WebClient {Encoding = Encoding.UTF8};
            }
            try
            {
                string functionUrl = "/AllInterFace/VerifyCompanyInfo";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"companyIDcode", idcode},
                    {"Organizationcode", zjhm},
                    {"codePayType", zjType.ToString()},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                //dataDic.Add("file1", fileUrl);
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                Dictionary<string, string> fileDic = new Dictionary<string, string> {{"file1", fileUrl}};
                string strResult = _WebClient.HttpPost(interfaceUrl + functionUrl, dataDic, fileDic);
                obj = JsonDes.JsonDeserialize<Result>(strResult);
            }
            catch (Exception)
            {
                obj.ResultCode = -1;
                obj.ResultMsg = "连接接口失败！";
            }
            return obj;
        }

        /// <summary>
        /// 获取企业状态
        /// </summary>
        /// <param name="companyIDcode">企业主码</param>
        /// <returns></returns>
        public static OrganUnitStatusInfo GetStatus(string companyIDcode)
        {
            OrganUnitStatusInfo obj = new OrganUnitStatusInfo
            {
                ResultCode = -1,
                ResultMsg = "获取认证信息失败！"
            };
            if (string.IsNullOrEmpty(companyIDcode))
            {
                obj.ResultMsg = "请输入企业主码！";
                return obj;
            }
            try
            {
                string functionUrl = "/AllInterFace/GetOrganUnitStatus";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"companyIDcode", companyIDcode},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                obj = JsonDes.JsonDeserialize<OrganUnitStatusInfo>(strResult);
            }
            catch (Exception)
            {
                obj.ResultCode = -1;
                obj.ResultMsg = "连接接口失败！";
            }
            return obj;
        }

        #region 注册备案品类接口
        /// <summary>
        /// 注册备案品类
        /// </summary>
        /// <param name="mainCode"></param>
        /// <param name="industryCategory_ID">品类ID</param>
        /// <param name="categoryCode"></param>
        /// <param name="materialName"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public static string Recode(string mainCode, string industryCategory_ID, string categoryCode, string materialName, string materialCode)
        {
            string strReturn = null;
            if (_WebClient == null)
            {
                _WebClient = new WebClient { Encoding = Encoding.UTF8 };
            }
            try
            {

                string functionUrl = "/AllInterFace/RegProductIDcodeInfo";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("companyIDcode", mainCode);
                dataDic.Add("codeUse_ID", "10");
                dataDic.Add("industryCategory_ID", industryCategory_ID);
                dataDic.Add("categoryCode", categoryCode);
                dataDic.Add("modelNumber", materialName);
                dataDic.Add("modelNumberCode", materialCode);
                dataDic.Add("codePayType", "5");
                dataDic.Add("goToUrl", parseUrl + "Wap_Index/Index");
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                strReturn = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
            }
            catch (Exception ex)
            {
            }
            return strReturn;
        }

        /// <summary>
        /// 注册医疗器械品类
        /// 平台没有生成码操作，弃用
        /// </summary>
        /// <param name="mainCode"></param>
        /// <param name="industryCategory_ID"></param>
        /// <param name="categoryCode"></param>
        /// <param name="materialName"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public static string ReCategory(string mainCode, string industryCategory_ID, string categoryCode, string materialName,
            long StartNum,  long EndNum,int BZSpecType,string shengchanPH,string qz,DateTime? RequestDate,
                             DateTime? YouXiaoDate,DateTime? ShiXiaoDate)
        {
            string strReturn = null;
            if (_WebClient == null)
            {
                _WebClient = new WebClient { Encoding = Encoding.UTF8 };
            }
            try
            {

                string functionUrl = "/sp/idcode/medical/upload/codeprefix";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("company_idcode", mainCode);
                dataDic.Add("industrycategory_id", industryCategory_ID);
                dataDic.Add("category_code", categoryCode);
                dataDic.Add("model_number", materialName);
                dataDic.Add("specification", BZSpecType.ToString());
                dataDic.Add("prefix_str", qz);//前缀
                dataDic.Add("start_num", StartNum.ToString());
                dataDic.Add("end_num", EndNum.ToString());
                dataDic.Add("start_date", RequestDate.Value.ToString("yyyy-MM-dd"));//生产日期
                dataDic.Add("batch_number", shengchanPH);//生产批号
                dataDic.Add("end_date", ShiXiaoDate.Value.ToString("yyyy-MM-dd"));//失效日期
                dataDic.Add("effective_date", YouXiaoDate.Value.ToString("yyyy-MM-dd"));//有效期
                dataDic.Add("gotourl", parseUrl + "Wap_Index/Index");
                dataDic.Add("sample_url", parseUrl + "Wap_Index/Index");
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                strReturn = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
            }
            catch (Exception ex)
            {
            }
            return strReturn;
        }
        #endregion

        #region 调用接口使用
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static string GetTimeStamp(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long ts = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
            return ts.ToString();
        }
        /// <summary>
        /// 获取hash
        /// </summary>
        /// <param name="functionUrl"></param>
        /// <param name="dataDic"></param>
        /// <returns></returns>
        private static string GetHash(string functionUrl, Dictionary<string, string> dataDic,string tokenCode)
        {
            //dataDic = dataDic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            dataDic = dataDic.OrderBy(x => x.Key, new ComparerString()).ToDictionary(x => x.Key, y => y.Value);
            string dataStr = "";
            foreach (var item in dataDic)
            {
                string s = item.Key + "=" + item.Value;
                dataStr += string.IsNullOrEmpty(dataStr) ? s : ("&" + s);
            }
            string url = functionUrl + "?" + dataStr;
            string hash = GetMD5String(url + tokenCode);
            return hash;
        }
        /// <summary>
        /// MD5　加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetMD5String(string str)
        {
            string encryptedstr = "";
            MD5 md5 = new MD5CryptoServiceProvider();
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                //数值英文字母
                encryptedstr = encryptedstr + s[i].ToString("X2");
            }
            return encryptedstr;
        }

        public class ComparerString : IComparer<String>
        {
            public int Compare(String x, String y)
            {
                return string.CompareOrdinal(x, y);
            }
        }
        #endregion

        #region 码标接口 2019-08-20 王坤

        /// <summary>
        /// 获取码标接口
        /// </summary>
        /// <param name="unitUserName">单位用户名（必填）</param>
        /// <param name="unitUserPwd">单位用户密码（必填）</param>
        /// <param name="mb">码标（不含MA.,若不填写，则返回全部）</param>
        /// <returns></returns>
        public static ResultInfo GetMbCode(string unitUserName, string unitUserPwd, string mb)
        {
            ResultInfo obj = new ResultInfo
            {
                result_code = -1,
                result_msg = "失败！"
            };
            if (_WebClient == null)
            {
                _WebClient = new WebClient { Encoding = Encoding.UTF8 };
            }
            try
            {
                string functionUrl = "/sp/ma/mb_unit_query";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"unit_user_name", unitUserName},
                    {"unit_user_pwd", unitUserPwd},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                if (!string.IsNullOrEmpty(mb))
                {
                    dataDic.Add("mb", mb);
                }
               
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));

                var strReturn = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                obj = JsonDes.JsonDeserialize<ResultInfo>(strReturn);
            }
            catch (Exception)
            {
                obj.result_code = -1;
                obj.result_msg = "连接接口失败！";
            }
            return obj;
        }
        #endregion


        #region 医疗器械接口
        /// <summary>
        /// 获取企业性质
        /// </summary>
        /// <returns></returns>
        public static InterFaceHisUnitType GetHisUnitType()
        {
            InterFaceHisUnitType result = new InterFaceHisUnitType();
            if (_WebClient == null)
            {
                _WebClient = new WebClient();
                _WebClient.Encoding = Encoding.UTF8;
            }
            try
            {
                string functionUrl = "/sp/idcode/medical/unittypes";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                result = JsonDes.JsonDeserialize<InterFaceHisUnitType>(strResult);
            }
            catch
            {
                result.result_code = -1;
                result.result_msg = "连接接口失败！";
            }
            return result;
        }

        /// <summary>
        /// 获取所有品类接口
        /// </summary>
        /// <returns></returns>
        public static string GetHisIndustryCategory()
        {
            //InterFaceHisIndustryCategory result = new InterFaceHisIndustryCategory();
            string result = "";
            if (_WebClient == null)
            {
                _WebClient = new WebClient();
                _WebClient.Encoding = Encoding.UTF8;
            }
            try
            {
                string functionUrl = "/sp/idcode/medical/industrycategory";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                //result = JsonDes.JsonDeserialize<InterFaceHisIndustryCategory>(strResult);
                result = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
            }
            catch
            {
                result= "连接接口失败！";
            }
            return result;
        }

        /// <summary>
        /// 获取批量注册品类码内容的下载地址
        /// 2019-11-01
        /// 刘晓杰
        /// </summary>
        /// <param name="company_idcode">单位主码(必填)</param>
        /// <param name="batch_no">批量生成申请批次（必填）</param>
        /// <param name="password">解压包解压密码（6~16位字符，非必填）</param>
        /// <returns></returns>
        public static InterFaceHisCodeFileUrlInfo GetUploadBatch(string company_idcode, string batch_no, string password)
        {
            InterFaceHisCodeFileUrlInfo obj = new InterFaceHisCodeFileUrlInfo();

            if (string.IsNullOrEmpty(company_idcode))
                return obj;

            if (_WebClient == null)
            {
                _WebClient = new WebClient { Encoding = Encoding.UTF8 };
            }
            try
            {
                string functionUrl = "/sp/idcode/medical/idcodeinfo/uploadbatch";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"company_idcode", company_idcode},
                    {"batch_no", batch_no},
                    {"password", password},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                dataDic.Add("hash", GetHash(functionUrl, dataDic,access_token_code));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                //返回数据格式：{"result_code":1,"result_msg":"成功","codefileurl_info":"http://apitest.idcode.org.cn/UploadCode/20191101100301881_new.zip"}
                obj = JsonDes.JsonDeserialize<InterFaceHisCodeFileUrlInfo>(strResult);
            }
            catch (Exception)
            {
                obj.result_code = -1;
                obj.result_msg = "连接接口失败！";
                obj.codefileurl_info = "";
            }
            return obj;
        }
        #endregion

        /// <summary>
        /// 根据用户名密码获取pi列表 1111接口
        /// </summary>
        /// <param name="mainCode">企业主码</param>
        /// <param name="diCode">DI码</param>
        /// <returns></returns>
        public static ListHisPI GetPIInfo(string enToken,string enTokenCode, string mainCode, string di, string start_date)
        {
            ListHisPI result = new ListHisPI();
            if (_WebClient == null)
            {
                _WebClient = new WebClient();
                _WebClient.Encoding = Encoding.UTF8;
            }
            try
            {
                if (enToken != null && enToken != "")
                {
                    access_token = enToken;
                }
                string functionUrl = "/sp/idcode/medical/pi/record";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("company_idcode", mainCode);
				//if (!string.IsNullOrEmpty(start_date))
				//{
				//    dataDic.Add("start_date", start_date);
				//}
                dataDic.Add("udi_di", di);
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,enTokenCode));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                result = JsonDes.JsonDeserialize<ListHisPI>(strResult);
            }
            catch
            {
                result.result_code = -1;
                result.result_msg = "连接接口失败！";
            }
            return result;
        }

        /// <summary>
        /// 根据批次编号查询码明细 1112接口
        /// </summary>
        /// <param name="mainCode"></param>
        /// <param name="batch_no"></param>
        /// <returns></returns>
        public static ListPICode GetPICode(string enToken,string enTokenCode,string mainCode, string batch_no)
        {
            ListPICode result = new ListPICode();
            if (_WebClient == null)
            {
                _WebClient = new WebClient();
                _WebClient.Encoding = Encoding.UTF8;
            }
            try
            {
                if (enToken != null && enToken != "")
                {
                    access_token = enToken;
                }
                string functionUrl = "/sp/idcode/medical/pi/list";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("company_idcode", mainCode);
                dataDic.Add("batch_no", batch_no);
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,enTokenCode));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                result = JsonDes.JsonDeserialize<ListPICode>(strResult);
            }
            catch
            {
                result.result_code = -1;
                result.result_msg = "连接接口失败！";
            }
            return result;
        }
        /// <summary>
        /// 获取DI信息
        /// </summary>
        /// <param name="mainCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ListHisDI GetDIList(string mainCode,string token,string tokenCode)
        {
            ListHisDI result = new ListHisDI();
            if (_WebClient == null)
            {
                _WebClient = new WebClient();
                _WebClient.Encoding = Encoding.UTF8;
            }
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    access_token = token;
                }
                if (!string.IsNullOrEmpty(tokenCode))
                {
                    access_token_code = tokenCode;
                }
                string functionUrl = "/sp/idcode/medical/di/list/ex";
                Dictionary<string, string> dataDic = new Dictionary<string, string>();
                dataDic.Add("access_token", access_token);
                dataDic.Add("company_idcode", mainCode);//UDI注册人代码(必填)
                dataDic.Add("model_number", "");//名称(非必填)
                dataDic.Add("category_code", "");//品类编码(非必填)
                dataDic.Add("specification", "");//包装规格(非必填)
                dataDic.Add("start_date", "");//开始时间(非必填)
                dataDic.Add("end_date", "");//结束时间(非必填)
                dataDic.Add("time", GetTimeStamp(DateTime.Now));
                dataDic.Add("hash", GetHash(functionUrl, dataDic,tokenCode));
                string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                result = JsonDes.JsonDeserialize<ListHisDI>(strResult);
            }
            catch (Exception ex)
            {
                result.result_code = -1;
                result.result_msg = "连接接口失败！";
            }
            return result;
        }
        /// <summary>
        /// 1101接口
        /// UDI-DI注册上传DI到发码机构
        /// 2021-4-21加 服务调用上传到发码机构
        /// </summary>
        /// <param name="company_idcode"></param>
        /// <param name="category_code"></param>
        /// <param name="model_number"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public static HisResult IDCodeMedicalReg(string company_idcode,
            string category_code, string model_number, string specification, string enToken,string enTokenCode)
        {
            HisResult info = new HisResult();
            try
            {
                if (enToken != null && enToken != "")
                {
                    access_token = enToken;
                }
                if (enTokenCode != null && enTokenCode != "")
                {
                    access_token_code = enTokenCode;
                }
                string functionUrl = "/sp/idcode/medical/idcodeinfo/reg";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", access_token},
                    {"company_idcode", company_idcode},
                    {"category_code", category_code},
                    {"model_number", model_number},
                    {"specification", specification},
                    //{"product_model", product_model},
                    //{"serial_number", code_list_str},
                    //{"start_date", start_date},
                    //{"batch_number", batch_number},
                    //{"d_batch_number", d_batch_number},
                    //{"end_date", end_date},
                    //{"effective_date", effective_date},
                    {"gotourl", "http://udi2.com/Wap_Index/Index"},
                    {"sample_url", "http://udi2.com/Wap_Index/Index"},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                dataDic.Add("hash", GetHash(functionUrl, dataDic, access_token_code));
                string rst = WebClient.sendPost(interfaceUrl + functionUrl, dataDic, "post");
                if (rst.Contains("DOCTYPE"))
                    return new HisResult();
                info = JsonDes.JsonDeserialize<HisResult>(rst);
                return info;
            }
            catch (Exception ex)
            {
                info.result_code = -3;
                info.result_msg = "连接接口失败：" + ex.Message;
                return info;
            }
        }

        //UDI-PI注册上传PI到发码机构20210422 1104接口 此接口不用 改为上传TXT文档上传
        public static IDCodeUploadCodeListMsg IDCodeMedicalUploadCodeList(string enToken,string enTokenCode,
           string company_idcode, string category_code, string model_number, string specification,
           string code_list_str, string start_date, string batch_number, string d_batch_number, 
            string end_date, string effective_date, string product_model)
        {
            IDCodeUploadCodeListMsg info = new IDCodeUploadCodeListMsg();
            try
            {
                string functionUrl = "/sp/idcode/medical/upload/codelist";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", enToken},
                    {"company_idcode", company_idcode},
                    {"category_code", category_code},
                    {"model_number", model_number},
                    {"specification", specification},
                    {"code_list_str", code_list_str},
                    {"start_date", start_date},
                    {"batch_number", batch_number},
                    {"d_batch_number", d_batch_number},
                    {"end_date", end_date},
                    {"effective_date", effective_date},
                    {"product_model", product_model},
                    {"gotourl", "http://udi2.com/Wap_Index/Index"},
                    {"sample_url", "http://udi2.com/Wap_Index/Index"},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                dataDic.Add("hash", GetHash(functionUrl, dataDic,enTokenCode));
                Dictionary<string, string> fileDic = new Dictionary<string, string>();//没有文件不用赋值
                //string rst = _WebClient.HttpPost(interfaceUrl + functionUrl, dataDic, fileDic);
                string rst = WebClient.sendPost(interfaceUrl + functionUrl, dataDic, "post");
                //WriteLog("time:" + dataDic["time"] + "\t hash:" + dataDic["hash"] + "\t result:" + rst + "\t interfaceUrl:" + functionUrl , "IDCodeLog");
                if (rst.Contains("DOCTYPE"))
                    return new IDCodeUploadCodeListMsg();
                if (rst == "未能解析此远程名称: 'api.idcode.org.cn'" || rst == "未能解析此远程名称: 'api.utcgl.com'")
                {
                    info.result_code = -3;
                    info.result_msg = rst;
                    return info;
                }
                else
                {
                    info = JsonDes.JsonDeserialize<IDCodeUploadCodeListMsg>(rst);
                }
                return info;
            }
            catch (Exception ex)
            {
                info.result_code = -3;
                info.result_msg = "" + ex.Message;
                return info;
            }
        }

        //UDI-PI注册上传PI到发码机构20210430 1103接口
        public static IDCodeUploadCodeListMsg IDCodeMedicalUploadCodeFile(string enToken, string enTokenCode,
           string company_idcode, string category_code, string model_number, string specification,
            string start_date, string batch_number, string d_batch_number,
            string end_date, string effective_date, string product_model, string fileUrl)
        {
            IDCodeUploadCodeListMsg info = new IDCodeUploadCodeListMsg();
            try
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient();
                    _WebClient.Encoding = Encoding.UTF8;
                }
                string functionUrl = "/sp/idcode/medical/upload/codefile";
                Dictionary<string, string> dataDic = new Dictionary<string, string>
                {
                    {"access_token", enToken},
                    {"company_idcode", company_idcode},
                    {"category_code", category_code},
                    {"model_number", model_number},
                    {"specification", specification},
                    //{"code_file", fileUrl},
                    {"start_date", start_date},
                    {"batch_number", batch_number},
                    {"d_batch_number", d_batch_number},
                    {"end_date", end_date},
                    {"effective_date", effective_date},
                    {"gotourl", "http://udi2.com/Wap_Index/Index"},
                    {"sample_url", "http://udi2.com/Wap_Index/Index"},
                    {"product_model", product_model},
                    {"time", GetTimeStamp(DateTime.Now)}
                };
                dataDic.Add("hash", GetHash(functionUrl, dataDic, enTokenCode));
                Dictionary<string, string> fileDic = new Dictionary<string, string> { { "code_file", fileUrl } };
                string rst = _WebClient.HttpPost(interfaceUrl + functionUrl, dataDic, fileDic);
                //string rst = WebClient.sendPost(interfaceUrl + functionUrl, dataDic, "post");
                //WriteLog("time:" + dataDic["time"] + "\t hash:" + dataDic["hash"] + "\t result:" + rst + "\t interfaceUrl:" + functionUrl , "IDCodeLog");
                if (rst.Contains("DOCTYPE"))
                    return new IDCodeUploadCodeListMsg();

                if (rst == "未能解析此远程名称: 'api.idcode.org.cn'" || rst == "未能解析此远程名称: 'api.utcgl.com'")
                {
                    info.result_code = -3;
                    info.result_msg = rst;
                    return info;
                }
                else
                {
                    info = JsonDes.JsonDeserialize<IDCodeUploadCodeListMsg>(rst);
                }
                return info;
            }
            catch (Exception ex)
            {
                info.result_code = -3;
                info.result_msg = "" + ex.Message;
                return info;
            }
        }


        public static HisOrganUnit GetCompanyMainCode(string loginname, string pwd)
        {
            HisOrganUnit obj = new HisOrganUnit();
            if (!string.IsNullOrEmpty(loginname) && !string.IsNullOrEmpty(pwd))
            {
                if (_WebClient == null)
                {
                    _WebClient = new WebClient { Encoding = Encoding.UTF8 };
                }
                try
                {
                    string functionUrl = "/sp/idcode/medical/loginverify";
                    Dictionary<string, string> dataDic = new Dictionary<string, string>();
                    dataDic.Add("access_token", access_token);
                    dataDic.Add("login_name", loginname);
                    dataDic.Add("login_pswd", pwd);
                    dataDic.Add("time", GetTimeStamp(DateTime.Now));
                    dataDic.Add("hash", GetHash(functionUrl, dataDic, access_token_code));
                    string strResult = _WebClient.HttpGet(interfaceUrl + functionUrl, dataDic);
                    WriteLog(strResult, "MainCode");
                    obj = JsonDes.JsonDeserialize<HisOrganUnit>(strResult);
                }
                catch (Exception ex)
                {
                    obj.result_code = -1;
                    obj.result_msg = "连接接口失败！";
                }
            }
            return obj;
        }

        public static void WriteLog(string msg, string directoryName)
        {
            string errlogpath = System.Configuration.ConfigurationManager.AppSettings["ErrLogPath"];
            errlogpath = errlogpath + "\\" + directoryName + "\\" + DateTime.Now.ToString("yyyyMM");
            if (!Directory.Exists(errlogpath))
            {
                Directory.CreateDirectory(errlogpath);
            }

            using (StreamWriter sw = new StreamWriter(errlogpath + "\\" + DateTime.Now.ToString("dd") + "errorLog.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "------" + msg);
            }
        }
    }
}