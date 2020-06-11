//******************************************************************
//*  功能說明：操作Log記錄
//*  作    者：HAO CHEN
//*  創建日期：2010/07/15
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;

namespace BusinessRules
{
    public class BRM_Log : BRBase<Entity_Log>
    {
        /// <summary>
        /// Insert Data
        /// </summary>
        /// <param name="strUser">操作用戶</param>
        /// <param name="strJOBID">操作時間</param>
        /// <param name="strOperationName">作業名稱</param>
        /// <param name="strType">作業類型(A:新增,U:修改,D:刪除,IM:檔案匯入,OU:檔案匯出)</param>
        /// <param name="strRMsg">提示訊息</param>
        /// <returns>是否成功</returns>
        public static bool Insert(string strUser, string strDate, string strOperationName, string strType)
        {
            Entity_Log eLog = new Entity_Log();

            eLog.create_user = strUser;
            eLog.create_dt = strDate;
            eLog.Operation_Name = strOperationName;
            eLog.type_flg = strType;

            try
            {
                return BRM_Log.AddNewEntity(eLog);
            }
            catch (Exception ex)
            {
                BRM_Log.SaveLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢Log記錄
        /// 作    者:HAO CHEN
        /// 創建時間:2010/07/15
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPost"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchLog(string strCondition, ref  DataTable dtPost, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                string sql = @"select L.create_user, L.create_dt,L.Operation_Name,
                                 case   L.type_flg
                                 when 'A' then '新增' 
                                 when 'U' then '修改' 
                                 when 'D' then '刪除' 
                                 when 'IM' then '檔案匯入' 
                                 when 'OU' then '檔案匯出'
                                 end  type_flg 
                                 from  tbs_Log L";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }
                sql += " order by L.create_dt desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (null != ds && ds.Tables[0].Rows.Count > 0)
                {
                    dtPost = ds.Tables[0];
                    strMsgID = "06_06040000_003";
                    return true;
                }
                else
                {
                    strMsgID = "06_06040000_004";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040000_004";
                return false;
            }

        }
    }
}
