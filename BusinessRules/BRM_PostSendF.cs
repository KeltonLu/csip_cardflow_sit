//******************************************************************
//*  功能說明：郵局交寄資訊匯入失敗檔業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/06/08
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
    public class BRM_PostSendF : BRBase<EntityM_PostSendF>
    {

        /// <summary>
        /// 功能說明:新增一筆郵局交寄資訊匯入失敗檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(EntityM_PostSendF PostSendF, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_PostSendF.AddNewEntity(PostSendF))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_001";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_002";
                        return false;
                    }

                }
            }
            catch (Exception exp)
            {
                BRM_PostSendF.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:刪除一筆郵局交寄資訊匯入失敗檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(EntityM_PostSendF PostSendF, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_PostSendF.DeleteEntityByCondition(PostSendF, strCondition))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_005";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_006";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_PostSendF.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆郵局交寄資訊匯入失敗檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(EntityM_PostSendF PostSendF, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_PostSendF.UpdateEntityByCondition(PostSendF, strCondition, FiledSpit))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_003";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_004";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_PostSendF.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢郵局交寄資訊匯入失敗檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPostSendF"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchPost(string strCondition, ref  DataTable dtPostSendF, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                string sql = @"select p.sno, p.Podate,p.Cardno,p.OutPutDate,
                                 case   p.EndCaseFlg
                                 when 'Y' then '結案' 
                                 else  '未結案' 
                                 end  EndCaseFlgs ,b.Action,b.Mailno,b.custname,b.add1,b.Maildate,b.id
                                 from  tbl_Post p left join tbl_Card_BaseInfo b on p.cardno=b.cardno";
              
                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }
                sql += " order by p.Podate desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_PostSendF.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtPostSendF = ds.Tables[0];
                    strMsgID = "06_06040100_007";
                    return true;
                }
                else
                {
                    strMsgID = "06_06040100_008";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_PostSendF.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }

        }
      
       
        /// <summary>
        /// 功能說明:查詢郵局交寄資訊匯入失敗檔by cardno
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPostSendF"></param>
        /// <returns></returns>
        public static bool SearchPostByNo(string strCondition, ref  DataTable dtPostSendF, ref string strMsgID)
        {
            try
            {
                string sql = @"select p.Podate,p.Cardno,p.OutPutDate,
                                 case   p.EndCaseFlg
                                 when 'Y' then '結案' 
                                 else  '未結案' 
                                 end  EndCaseFlgs ,p.Backdate,p.EndCaseFlg,p.Note,p.Stateflg,p.Sno,b.Action,b.Mailno,b.id
                                 from  tbl_Post p left join tbl_Card_BaseInfo b on p.cardno=b.cardno";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_PostSendF.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtPostSendF = ds.Tables[0];
                    strMsgID = "06_06040100_007";
                    return true;
                }
                else
                {
                    strMsgID = "06_06040100_008";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_PostSendF.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }
        /// <summary>
        /// 功能說明:判断是否有重復的屬性匯入日志
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">Post</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(EntityM_PostSendF PostSendF)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_PostSendF.M_M_date, Operator.Equal, DataTypeUtils.String, PostSendF.M_date);

            Sql.AddCondition(EntityM_PostSendF.M_maildate, Operator.Equal, DataTypeUtils.String, PostSendF.maildate);

            if (BRM_PostSendF.Search(Sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
