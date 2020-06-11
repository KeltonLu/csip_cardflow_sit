//******************************************************************
//*  功能說明：綜合資料處理業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/05/18
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using EntityLayer;


namespace BusinessRules
{
    public class BRM_AutoJob : BRBase<EntityM_AutoJob>
    { 
        #region SQL語句
        public const string SEL_FKEY_BY_FKEYandFNAME = @"SELECT RUN_SECONDS , RUN_MINUTES,RUN_HOURS,RUN_DAY_OF_MONTH,RUN_MONTH,RUN_DAY_OF_WEEK,EXEC_PROG, STATUS, RUN_USER_LDAPID,RUN_USER_LDAPPWD, RUN_USER_RACFID,RUN_USER_RACFPWD,MAIL_TO,DESCRIPTION,CHANGED_USER,CONVERT(varchar, CHANGED_TIME, 120 ) as  CHANGED_TIME  FROM  M_AUTOJOB WHERE   FUNCTION_KEY= @FUNCTION_KEY AND JOB_ID= @JOB_ID";
        public const string SEL_FKEY_BY_PROPERTY = @"SELECT COUNT(*)  FROM  M_AUTOJOB WHERE   ";

        public const string SEL_JOB = @"SELECT JOB_ID,RUN_SECONDS,RUN_MINUTES,RUN_HOURS,RUN_DAY_OF_MONTH,RUN_MONTH,RUN_DAY_OF_WEEK,EXEC_PROG, STATUS, RUN_USER_LDAPID,RUN_USER_LDAPPWD, RUN_USER_RACFID,RUN_USER_RACFPWD,MAIL_TO,DESCRIPTION,CHANGED_USER,CONVERT(varchar, CHANGED_TIME, 120 ) as  CHANGED_TIME  FROM  M_AUTOJOB WHERE FUNCTION_KEY= @FUNCTION_KEY ";

        #endregion

        /// <summary>
        /// 功能說明:取得jobid資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/18
        /// 修改記錄:
        /// </summary>
        /// <param name="eAutoJob">AUTOJOB</param>
        /// <returns>DataTable</returns>
        public static DataTable SearchJobTime(string strFunctionKey, string strJobid, int intPageIndex, int intPageSize, ref int intTotolCount, ref string strMsgID)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_FKEY_BY_FKEYandFNAME;
            sqlcmd.CommandType = CommandType.Text;
            SqlParameter parmKey = new SqlParameter("@" + EntityM_AutoJob.M_FUNCTION_KEY, strFunctionKey);
            sqlcmd.Parameters.Add(parmKey);
            SqlParameter parmId = new SqlParameter("@" + EntityM_AutoJob.M_JOB_ID, strJobid);
            sqlcmd.Parameters.Add(parmId);
            DataSet dstProperty = null;
            DataTable dtblProperty = null;
            dstProperty = BRM_AutoJob.SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount, "Connection_CSIP");
            if (dstProperty == null)
            {
                strMsgID = "00_00000000_000";
                return null;
            }
            else
            {
                dtblProperty = dstProperty.Tables[0];
            }
            return dtblProperty;
        }



        /// <summary>
        /// 功能說明:刪除jobid資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/18
        /// 修改記錄:
        /// </summary>
        /// <param name="eAutoJob"></param>
        /// <param name="strMsgID">ID</param>
        /// <returns>DataTable</returns>
        public static bool Delete(EntityM_AutoJob eAutoJob, ref string strMsgID)
        {

            SqlHelper Sql = new SqlHelper();
            Sql.AddCondition(EntityM_AutoJob.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, eAutoJob.FUNCTION_KEY);
            Sql.AddCondition(EntityM_AutoJob.M_JOB_ID, Operator.Equal, DataTypeUtils.String, eAutoJob.JOB_ID);
            try
            {
                BRM_AutoJob.DeleteEntityByCondition(eAutoJob, Sql.GetFilterCondition(), "Connection_CSIP");
                strMsgID = "00_01050000_001";
                return true;
            }
            catch
            {
                strMsgID = "00_01050000_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:判斷是否有重複的jobid
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/18
        /// 修改記錄:
        /// </summary>
        /// <param name="eAutojob">JOBID</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(EntityM_AutoJob eAutojob)
        {
            SqlHelper sql = new SqlHelper();
            sql.AddCondition(EntityM_AutoJob.M_JOB_ID, Operator.Equal, DataTypeUtils.String, eAutojob.JOB_ID);
            sql.AddCondition(EntityM_AutoJob.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, eAutojob.FUNCTION_KEY);
            if (BRM_AutoJob.Search(sql.GetFilterCondition(), "Connection_CSIP").Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 功能說明:添加jobid
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/18
        /// 修改記錄:
        /// </summary>
        /// <param name="eAutojob">JOBID</param>
        /// <param name="strMsgID">ID</param>
        /// <returns></returns>
        public static bool Add(EntityM_AutoJob eAutojob, ref string strMsgID)
        {
            if (!IsRepeat(eAutojob))
            {
                if (BRM_AutoJob.AddNewEntity(eAutojob))
                {
                    strMsgID = "00_01050000_014";
                    return true;
                }
                else
                {
                    strMsgID = "00_01050000_015";
                    return false;
                }
            }
            strMsgID = "00_01050000_016";
            return false;
        }

        /// <summary>
        /// 功能說明:修改jobid
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/18
        /// 修改記錄:
        /// </summary>
        /// <param name="eAuotojob">JOBID信息</param>
        /// <param name="strMsgID">消息ID</param>
        /// <returns></returns>
        public static bool Update(EntityM_AutoJob eAutojob, ref string strMsgID)
        {     
                SqlHelper Sql = new SqlHelper();
                Sql.AddCondition(EntityM_AutoJob.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, eAutojob.FUNCTION_KEY);
                Sql.AddCondition(EntityM_AutoJob.M_JOB_ID, Operator.Equal, DataTypeUtils.String, eAutojob.JOB_ID);
                if (BRM_AutoJob.UpdateEntity(eAutojob, Sql.GetFilterCondition(),"Connection_CSIP"))
                {
                    strMsgID = "00_01050000_018";
                    return true;
                }
                else
                {
                    strMsgID = "00_01050000_019";
                    return false;
                }          
        }


        /// <summary>
        /// 功能說明:取得job資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/18
        /// 修改記錄:
        /// </summary>
        /// <param name="eAutoJob">AUTOJOB</param>
        /// <returns>DataTable</returns>
        public static DataTable SearchJobData(string strFunctionKey, ref string strMsgID)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_JOB;
            sqlcmd.CommandType = CommandType.Text;
            SqlParameter parmKey = new SqlParameter("@" + EntityM_AutoJob.M_FUNCTION_KEY, strFunctionKey);
            sqlcmd.Parameters.Add(parmKey);
            DataTable dtblProperty = null;
            try
            {
                dtblProperty = BRM_AutoJob.SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
            }
            catch (Exception exp)
            {
                strMsgID = "00_00000000_000";
                throw exp;
            }
            return dtblProperty;
        }

        /// <summary>
        /// 功能說明:查詢ALL job
        /// 作    者:HAO CHEN
        /// 創建時間:2010/07/21
        /// 修改記錄:
        /// </summary>
        /// <param name="dtAutoJob"></param>
        /// <param name="strJobId"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static DataTable SearchAllJob(ref string strMsgID)
        {
            try
            {
                string sql = "select JOB_ID,JOB_ID + ' ' + DESCRIPTION as DESCRIPTION from M_AUTOJOB where FUNCTION_KEY='06' order by DESCRIPTION";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;

                DataTable dtTmp = new DataTable();
                DataSet ds = BRM_AutoJob.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (null != ds && ds.Tables[0].Rows.Count > 0)
                {
                    strMsgID = string.Empty;
                    return ds.Tables[0];
                }
                else
                {
                    strMsgID = "00_00000000_030";
                    return dtTmp;
                }
            }
            catch (Exception exp)
            {
                BRM_AutoJob.SaveLog(exp.Message);
                strMsgID = "00_00000000_030";
                DataTable dtTmps = new DataTable();
                return dtTmps;
            }
        }

        /// <summary>
        /// 功能說明:查詢job工作狀態
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/18
        /// 修改記錄:
        /// </summary>
        /// <param name="dtAutoJob"></param>
        /// <param name="strJobId"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchJobStatus(ref string strJobStatus, string strJobId, ref string strMsgID)
        { 
            try 
            {
                string sql = "select * from M_AUTOJOB where JOB_ID=@JOB_ID and FUNCTION_KEY='06'";
                
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                SqlParameter parmKey = new SqlParameter("@" + EntityM_AutoJob.M_JOB_ID, strJobId);
                sqlcmd.Parameters.Add(parmKey);
                DataSet ds = BRM_AutoJob.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (ds != null)
                {
                    DataTable dtAutoJob = ds.Tables[0];
                    if (dtAutoJob != null && dtAutoJob.Rows.Count > 0)
                    {
                        strJobStatus = dtAutoJob.Rows[0]["STATUS"].ToString();
                        strMsgID = "02_00000000_007";
                        return true;
                    }
                    else
                    {
                        return false;
                    }            
                }
                else
                {
                    strMsgID = "02_00000000_007";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_AutoJob.SaveLog(exp.Message);
                strMsgID = "02_00000000_008";
                return false;
            }
        }
    }
}
