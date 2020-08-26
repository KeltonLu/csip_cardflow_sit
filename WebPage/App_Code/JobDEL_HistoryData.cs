//******************************************************************
//*  作    者：Wallace Liu
//*  功能說明：刪除一年前的卡片基本資料檔資料(以轉檔日挑選資料)
//*  創建日期：2015/02/06
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using System.Data.SqlClient;
using Framework.Data;
using Framework.Common.Utility;
using Framework.Common.Logging;
using CSIPCommonModel.BusinessRules;
using Quartz;

/// <summary>
/// jobBackup 的摘要描述
/// </summary>
public class JobDEL_HistoryData : IJob
{
    public JobDEL_HistoryData()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    string strFuncKey = UtilHelper.GetAppSettings("FunctionKey");
    string strSucc = "";
    DateTime dTimeStart;
    private string strJobID;
    private string strJobMsg;
    private string strDelCount="";

    #region IJob 成員
    /// <summary>
    /// Job 調用入口
    /// </summary>
    /// <param name="context"></param>
    public void Execute(JobExecutionContext context)
    {
        // 獲取JOB開始時間
        dTimeStart = DateTime.Now;

        strJobMsg = "";
        //string parameter = "";

        try
        {
//            strJobID = context.JobDetail.JobDataMap.GetString("jobid").Trim();
            //Wallace "JOBID"為何要大寫怪!!!
            strJobID = context.JobDetail.JobDataMap.GetString("JOBID").Trim();
            string strMsgID = "";
            //*查詢資料檔L_BATCH_LOG，查看是否上次作業還未停止
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFuncKey, strJobID, "R", ref strMsgID);
            if (dtInfo == null || dtInfo.Rows.Count > 0)
            {
                Logging.Log("JOB 工作狀態為：正在執行！", strJobID, LogState.Info);
                return;
            }
            //*開始批次作業
            if (!InsertNewBatch())
            {
                return;
            }

            //這支程式不使用JOB設定資訊
            //*查詢JOB設定資訊，抓取執行參數，做交換檔設定時參數預設帶空白
            //DataSet dtFileInfo = BRM_FileInfo.GetFTPinfo(strJobID);
            //if (dtFileInfo == null)
            //    parameter = "";
            //else
            //    parameter = dtFileInfo.Tables[0].Rows[0]["Parameter"] != null ? dtFileInfo.Tables[0].Rows[0]["Parameter"].ToString() : "";

            //*開始執行作業
            DelHistoryData(ref strDelCount);

            
            //*批次完成記錄LOG信息
            string strMsg = strJobID + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            Logging.Log(strMsg, strJobID, LogLayer.DB);

            strSucc = "S";
            //strJobMsg = strDelCount;
            strJobMsg += Resources.JobResource.JobDEL_HistoryData001 + strDelCount;

            UpdateBatchLog(strJobMsg, dTimeStart);
            Logging.Log("JOB結束！", strJobID, LogState.Info);
        }   
        catch (Exception exp)
        {
            //*批次完成記錄LOG信息
            strSucc = "F";
            UpdateBatchLog(strJobMsg + exp.Message.ToString(), dTimeStart);
            Logging.Log(exp, strJobID);

            //*JOB失敗發送MAIL
            //    string strMail = context.JobDetail.JobDataMap.GetString("mail").Trim();
            //    string strTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
            //    if (!string.IsNullOrEmpty(strMail))
            //        BRM_FileInfo.SendFailMail(strMail, strTitle, exp.Message, dTimeStart);
        }
        finally
        {
            //如有設定參數則回復為空白
            //if (!string.IsNullOrEmpty(parameter))
            //    BRM_FileInfo.UpdateParameter(strJobID);
        }
    }
    #endregion

    /// <summary>
    /// 刪除資料
    /// </summary>
    /// <param name="Del_Count">回傳刪除的筆數</param>
    private void DelHistoryData(ref string Del_Count)
    {
        try
        {
            DataHelper dh = new DataHelper();
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.CommandTimeout = int.Parse(UtilHelper.GetAppSettings("SqlCmdTimeoutMax"));
            sqlcmd.CommandText = "SP_DEL_HistoryData";

            SqlParameter Count = sqlcmd.Parameters.Add("@delCount", SqlDbType.VarChar, 10);
            Count.Direction = ParameterDirection.Output;

            dh.ExecuteNonQuery(sqlcmd);
            Del_Count = Count.Value.ToString();
            Logging.Log("預計刪除筆數：" + Del_Count, strJobID, LogState.Info);
        }
        catch (System.Exception ex)
        {
            Logging.Log(ex, strJobID);
            throw ex;
        }
    }

    /// <summary>
    /// 更新執行狀態
    /// </summary>
    /// <param name="strError">JOB失敗信息</param>
    /// <param name="dateStart">JOB開始時間</param>
    private void UpdateBatchLog(string strError, DateTime dateStart)
    {
        //*插入L_BATCH_LOG資料庫
        BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
        BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dateStart, strSucc, strError);
    }

    /// <summary>
    /// 開始此次作業向Job_Status中插入一筆新的資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool InsertNewBatch()
    {
        return BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobID, dTimeStart, "R", "");
    }
}
