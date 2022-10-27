using System;
using System.Web;

namespace Common.Argument
{
    /// <summary>
    /// 登录成功初始化信息数据对象
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 平台角色
        /// </summary>
        public int Role_ID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public long EnterpriseID { get; set; }

        /// <summary>
        /// 平台权限
        /// </summary>
        public string ModualArray { get; set; }

        /// <summary>
        /// 用户所在角色权限
        /// </summary>
        public string RoleModualArray { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        ///用户登录ID
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 企业标识码
        /// </summary>
        public string EnterpriseBSCode { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 企业主码
        /// </summary>
        public string MainCode { get; set; }

        /// <summary>
        /// 设置码编号
        /// </summary>
        public long SetingID { get; set; }

        /// <summary>
        /// 用户类型：1登录用户2追溯平台调用用户
        /// </summary>
        public int LoginType { get; set; }
    }

    /// <summary>
    /// 当前登录用户
    /// </summary>
    public class CurrentUser
    {
        public static UserInfo User
        {
            get { return HttpContext.Current.Session["User"] as UserInfo; }
            set
            {
                if (value == null)
                {
                    HttpContext.Current.Session.Remove("User");
                }
                else
                {
                    HttpContext.Current.Session["User"] = value;
                }
            }
        }
    }

    #region
    /// <summary>
    /// 监管部门登录实体
    /// </summary>
    public class AdminUserInfo
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPwd { get; set; }
    }

    /// <summary>
    ///  监管部门登录
    /// </summary>
    public class AdminCurrentUser
    {
        public static AdminUserInfo AdminUser
        {
            get { return HttpContext.Current.Session["AdminUser"] as AdminUserInfo; }
            set
            {
                if (value == null)
                {
                    HttpContext.Current.Session.Remove("AdminUser");
                }
                else
                {
                    HttpContext.Current.Session["AdminUser"] = value;
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// 登录成功初始化信息数据对象
    /// </summary>
    /// <summary>
    /// 登录成功初始化信息数据对象
    /// </summary>
    [Serializable]
    public class LoginInfo
    {
        /// <summary>
        /// 平台角色
        /// </summary>
        public int PRRU_PlatFormLevel_ID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public long EnterpriseID { get; set; }

        public long UpEnterpriseID { get; set; }

        /// <summary>
        /// 平台权限
        /// </summary>
        public string Modual_ID_Array { get; set; }

        /// <summary>
        /// 用户所在角色权限
        /// </summary>
        public string RoleModual_ID_Array { get; set; }

        /// <summary>
        /// 用户类型1默认2注册
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 用户角色ID
        /// </summary>
        public int UserRoleID { get; set; }

        /// <summary>
        /// 登录用户ID
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 上级单位ID
        /// </summary>
        public long Parent_ID { get; set; }

        /// <summary>
        /// 企业码审批方式
        /// </summary>
        public int ApprovalCodeType { get; set; }

        /// <summary>
        /// 企业主码
        /// </summary>
        public string MainCode { get; set; }

        /// <summary>
        /// 所操作模ID（该模块及其子模块）
        /// </summary>
        public int PRRU_Modual_ID { get; set; }

        /// <summary>
        /// 当前登录企业名称
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 用户角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 企业行业
        /// </summary>
        public string HangYeIDArr { get; set; }

        /// <summary>
        /// 登录企业类型1：ioid，2：IDcode
        /// </summary>
        public int EnterpriseType { get; set; }
        /// <summary>
        /// 是否为新用户，新用户弹引导页
        /// </summary>
        public bool NewUser { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 省编码
        /// </summary>
        public long shengId { get; set; }
        /// <summary>
        /// 市编码
        /// </summary>
        public long shiId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Verify { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int CodeType { get; set; }
    }
}
