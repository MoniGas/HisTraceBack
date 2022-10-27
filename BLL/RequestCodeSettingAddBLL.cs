/********************************************************************************

** 作者： 陈志钢

** 创始时间：2017-08-18

** 修改人：xxx

** 修改时间：xxxx-xx-xx  

** 描述：主要用于系统服务生成二维码TXT文件的业务处理类类

*********************************************************************************/

using System.Data;
namespace BLL
{
    public class RequestCodeSettingAddBLL
    {
        private Dal.RequestCodeSettingAddDal dal = new Dal.RequestCodeSettingAddDal();
        
        /// <summary>
        /// 获得带处理的业务ID
        /// </summary>
        /// <param name="state">状态字段 0 新增 1调整</param>
        /// <returns></returns>
        public DataView getRecord()
        {
            return dal.getRecord();
        }

        /// <summary>
        /// 获得带处理的业务
        /// </summary>
        /// <param name="ID">业务ID</param>
        /// <returns></returns>
        public DataView getRecordByID(long ID)
        {
            return dal.getRecordByID(ID);
        }
        /// <summary>
        /// 获得生成码信息
        /// </summary>
        /// <param name="RequestID">生成码ID</param>
        /// <returns></returns>
        public DataView getRequestCode(long RequestID)
        {
            return dal.getRequestCode(RequestID);
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
        public DataView getCodeLst(string connectString, string tableName, long RequestID, long beginNO, long endNO)
        {
            return dal.getCodeLst(connectString, tableName, RequestID, beginNO, endNO);
        }
        
        /// <summary>
        /// 获得数据库路由信息
        /// </summary>
        /// <param name="Route_DataBase_ID">路由ID</param>
        /// <returns></returns>
        public DataView getDataRouteInfo(long Route_DataBase_ID)
        {
            return dal.getDataRouteInfo(Route_DataBase_ID);
        }

        /// <summary>
        /// 获得产品信息
        /// </summary>
        /// <param name="Material_ID">产品ID</param>
        /// <returns></returns>
        public DataView getMaterialInfo(long Material_ID)
        {
            return dal.getMaterialInfo(Material_ID);
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
            return dal.UpdateRecord( ID,  path,  URL,  state);
        }
    }
}
