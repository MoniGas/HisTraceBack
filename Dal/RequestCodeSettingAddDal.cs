/********************************************************************************

** 作者： 陈志钢

** 创始时间：2017-08-18

** 修改人：xxx

** 修改时间：xxxx-xx-xx  

** 描述：主要用于系统服务生成二维码TXT文件的数据库访问类
 * RequestCodeSettingAdd  state：1 待处理  2 处理成功 3 处理失败

*********************************************************************************/

using System.Data;
using System.Data.SqlClient;
namespace Dal
{
    public class RequestCodeSettingAddDal
    {
        private string ConnectString = System.Configuration.ConfigurationManager.AppSettings["WebConnect"];

        /// <summary>
        /// 获得带处理的业务
        /// </summary>
        /// <returns></returns>
        public DataView getRecord()
        {
            using (SqlConnection conn = new SqlConnection(ConnectString))
            {
                conn.Open();
                string sql = "select * from RequestCodeSettingAdd where state<2";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                command.Fill(ds, "ds");
                DataView dvRecord = ds.Tables[0].DefaultView;
                return dvRecord;
            }
        }

        /// <summary>
        /// 获得带处理的业务
        /// </summary>
        /// <param name="ID">业务ID</param>
        /// <returns></returns>
        public DataView getRecordByID(long ID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectString))
            {
                conn.Open();
                string sql = string.Format("select * from RequestCodeSettingAdd where ID={0}",ID);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                command.Fill(ds, "ds");
                DataView dvRecord = ds.Tables[0].DefaultView;
                return dvRecord;
            }
        }

        /// <summary>
        /// 获得生成码信息
        /// </summary>
        /// <param name="RequestID">生成码ID</param>
        /// <returns></returns>
        public DataView getRequestCode(long RequestID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectString))
            {
                conn.Open();
                string sql = string.Format(@"select  S.MaterialID,S.Count,S.EnterpriseId,S.beginCode,S.endCode,
                  C.RequestCode_ID,C.Route_DataBase_ID,C.type from  RequestCodeSetting S left join RequestCode  C on s.RequestID=C.RequestCode_ID
                  where S.ID={0}", RequestID);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                command.Fill(ds, "ds");
                DataView dvRecord = ds.Tables[0].DefaultView;
                return dvRecord;
            }
        }

        /// <summary>
        /// 获得二维码信息
        /// </summary>
        /// <param name="connectString">数据库连接串</param>
        /// <param name="tableName">表名</param>
        /// <param name="RequestID">生成码记录ID</param>
        /// <param name="beginNO">开始编码</param>
        /// <param name="endNO">结束编码</param>
        /// <returns></returns>
        public DataView getCodeLst(string connectString,string tableName,long RequestID,  long beginNO,long endNO)
        {
            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();
                string sqltmp = string.Format(@"select Enterprise_FWCode_ID from {0} where RequestCode_ID={2}  and  EWM like '%.{1}.%'",
                    tableName, beginNO.ToString().PadLeft(6,'0'), RequestID);
                SqlCommand _cmd = new SqlCommand();
                _cmd.Connection = conn;
                DataSet _ds = new DataSet();
                SqlDataAdapter _command = new SqlDataAdapter(sqltmp, conn);
                _command.Fill(_ds, "ds");
                DataView dvTmp = _ds.Tables[0].DefaultView;
                string beginID = dvTmp[0][0].ToString();
                sqltmp = string.Format(@"select Enterprise_FWCode_ID from {0} where RequestCode_ID={2}  and  EWM like '%.{1}.%'",
                    tableName, endNO.ToString().PadLeft(6, '0'), RequestID);
                 _cmd = new SqlCommand();
                _cmd.Connection = conn;
                 _ds = new DataSet();
                 _command = new SqlDataAdapter(sqltmp, conn);
                _command.Fill(_ds, "ds");
                 dvTmp = _ds.Tables[0].DefaultView;
                string endID = dvTmp[0][0].ToString();
                string sql = string.Format(@"select   EWM,FWCode,Type
                   from  {0} a where a.RequestCode_ID={1}   
                   and a.Enterprise_FWCode_ID between {2} and  {3}", tableName, RequestID, beginID, endID);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                command.Fill(ds, "ds");
                DataView dvRecord = ds.Tables[0].DefaultView;
                return dvRecord;
            }
        }

        /// <summary>
        /// 获得数据库路由信息
        /// </summary>
        /// <param name="Route_DataBase_ID">路由ID</param>
        /// <returns></returns>
        public DataView getDataRouteInfo(long Route_DataBase_ID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectString))
            {
                conn.Open();
                string sql = string.Format(@"select  * from  Route_DataBase where Route_DataBase_ID=
                  {0}", Route_DataBase_ID);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                command.Fill(ds, "ds");
                DataView dvRecord = ds.Tables[0].DefaultView;
                return dvRecord;
            }
        }

        /// <summary>
        /// 获得产品信息
        /// </summary>
        /// <param name="Material_ID">产品ID</param>
        /// <returns></returns>
        public DataView getMaterialInfo(long Material_ID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectString))
            {
                conn.Open();
                string sql = string.Format(@"select  * from  Material where Material_ID=
                  {0}", Material_ID);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                command.Fill(ds, "ds");
                DataView dvRecord = ds.Tables[0].DefaultView;
                return dvRecord;
            }
        }

        /// <summary>
        /// 修改记录状态
        /// </summary>
        /// <param name="ID">主键</param>
        /// <param name="path">物理路径</param>
        /// <param name="URL">链接</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public bool UpdateRecord(long ID, string path, string URL, int state)
        {
            using (SqlConnection conn = new SqlConnection(ConnectString))
            {
                conn.Open();
                string sql = string.Format(@"update RequestCodeSettingAdd set FileURL='{0}',
                WebURL='{1}',State={2} where ID={3}",path,URL,state,ID);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                return cmd.ExecuteNonQuery()> 0;
            }
        }
    }
}
