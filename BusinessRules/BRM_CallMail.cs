//******************************************************************
//*  功能說明：Mail警訊
//*  作    者：Simba Liu
//*  創建日期：2010/06/11
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
    public class BRM_CallMail : BRBase<EntityM_CallMail>
    {

        /// <summary>
        /// 功能說明:新增一筆Mail警訊
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/11
        /// 修改記錄:
        /// </summary>
        /// <param name="CallMail"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(EntityM_CallMail CallMail, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CallMail.AddNewEntity(CallMail))
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
                BRM_CallMail.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:刪除一筆Mail警訊
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/11
        /// 修改記錄:
        /// </summary>
        /// <param name="CallMail"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(EntityM_CallMail CallMail, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CallMail.DeleteEntityByCondition(CallMail, strCondition))
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
                BRM_CallMail.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆Mail警訊
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/11
        /// 修改記錄:
        /// </summary>
        /// <param name="CallMail"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(EntityM_CallMail CallMail, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CallMail.UpdateEntityByCondition(CallMail, strCondition, FiledSpit))
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
                BRM_CallMail.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢Mail警訊
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/11
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCallMail"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchPost(string strCondition, ref  DataTable dtCallMail, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("Select m.MailID,m.JobID,m.JobName,m.ConditionID,m.ConditionName,");
                sbSql.Append("m.MailTittle,m.MailContext,m.ToUsers,m.CcUsers,m.ToUserName,m.CcUserName ");
                sbSql.Append("From  tbl_CallMail m ");
              
                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append("WHERE ");
                    sbSql.Append(strCondition);
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CallMail.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCallMail = ds.Tables[0];
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
                BRM_CallMail.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }

        }

        /// <summary>
        /// 功能說明:查詢Mail警訊
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/11
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCallMail"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchMail(string strCondition, ref  DataTable dtCallMail, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("Select m.MailID,m.JobID,m.JobName,m.ConditionID,m.ConditionName,");
                sbSql.Append("m.MailTittle,m.MailContext,m.ToUsers,m.CcUsers,m.ToUserName,m.CcUserName ");
                sbSql.Append("From  tbl_CallMail m ");

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append("WHERE ");
                    sbSql.Append(strCondition);
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CallMail.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCallMail = ds.Tables[0];
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
                BRM_CallMail.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }

        }
      
       
        /// <summary>
        /// 功能說明:查詢Mail警訊by Condition
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/11
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCallMail"></param>
        /// <returns></returns>
        public static bool SearchMailByNo(string strCondition, ref  DataTable dtCallMail, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("Select m.MailID,m.JobID,m.JobName,m.ConditionID,m.ConditionName,");
                sbSql.Append("m.MailTittle,m.MailContext,m.ToUsers,m.CcUsers ");
                sbSql.Append("From  tbl_CallMail m ");

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append("WHERE ");
                    sbSql.Append(strCondition);
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CallMail.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCallMail = ds.Tables[0];
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
                BRM_CallMail.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }
 

        /// <summary>
        /// 功能說明:判断是否有重復的屬性匯入日志
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/11
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">CallMail</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(EntityM_CallMail CallMail)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_CallMail.M_JobID, Operator.Equal, DataTypeUtils.String, CallMail.JobID);

            if (BRM_CallMail.Search(Sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
