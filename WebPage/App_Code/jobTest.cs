using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Quartz;
using Framework.Common.Logging;
using CSIPCommonModel.BusinessRules;

/// <summary>
/// jobTest 的摘要描述
/// </summary>
public class jobTest : IJob
{
    public jobTest()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    
    #region IJob 成員
    public void Execute(JobExecutionContext context)
    {
        string strFuncKey = "06";
        string strJobID = "";
        string strJobTitle;
        string strLdapID;
        string strLdapPWD;
        string strRacfID;
        string strRacfPwd;
        string strMailList;
        DateTime dtStart = DateTime.Now;

        try
        {
            strJobID = context.JobDetail.JobDataMap.GetString("jobid").Trim();
            strJobTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
            strLdapID = context.JobDetail.JobDataMap.GetString("userId").Trim();
            strLdapPWD = context.JobDetail.JobDataMap.GetString("passWord").Trim();
            strRacfID = context.JobDetail.JobDataMap.GetString("racfId").Trim();
            strRacfPwd = context.JobDetail.JobDataMap.GetString("racfPassWord").Trim();
            strMailList = context.JobDetail.JobDataMap.GetString("mail").Trim();
            string strMsgID = "";
            //*查詢資料檔L_BATCH_LOG，查看是否上次作業還未停止
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFuncKey, strJobID, "R", ref strMsgID);
            if (dtInfo == null)
            {
                return;
            }
            if (dtInfo.Rows.Count > 0)
            {
                return;
            }
            //*開始批次作業
            if (!BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobID, dtStart, "R", ""))
            {
                return;
            }


            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            string strMsg = strJobID + "執行于:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString() + "  ;下次執行于:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            Logging.Log(strMsg, strJobID, LogLayer.DB);

            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "S", "");

        }
        catch (Exception ex)
        {
            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", ex.Message);
            Logging.Log(ex, strJobID, LogLayer.DB);
        }
    }
    #endregion
}
