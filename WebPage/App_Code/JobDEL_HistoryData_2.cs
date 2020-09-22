//******************************************************************
//*  作    者：Area Luke
//*  功能說明：刪除一年前的紀錄(清除使用者紀錄, 清除JOB錯誤資訊,
//  　         操作紀錄, 使用者連線ID, 功能歷程記錄, 動作歷程記錄,
//             屬性歷程記錄, 角色權限歷程記錄, 角色歷程記錄, 工作日歷程記錄,
//             AP執行紀錄, 排程執行紀錄)
//*  創建日期：2020/08/21
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Framework.Data;
using Framework.Common.Utility;
using Framework.Common.Logging;
using CSIPCommonModel.BusinessRules;
using Quartz;

/// <summary>
/// jobBackup 的摘要描述
/// </summary>
public class JobDEL_HistoryData_2 : IJob
{
    public JobDEL_HistoryData_2()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    protected string StrJobId;
    protected string StrFunctionKey = UtilHelper.GetAppSettings("FunctionKey");
    protected JobHelper JobHelper = new JobHelper();
    protected DateTime StartTime;
    protected DateTime EndTime;
    protected string StrMailMsg = string.Empty;

    private readonly string _strFrom = UtilHelper.GetAppSettings("MailSender");


    #region IJob 程式入口

    /// <summary>
    /// Job 調用入口
    /// </summary>
    /// <param name="context"></param>
    public void Execute(JobExecutionContext context)
    {

        string mailTo = context.JobDetail.JobDataMap.GetString("mail");
        string strSubject = context.JobDetail.JobDataMap.GetString("title");


        try
        {
            #region 獲取JOBID

            StrJobId = context.JobDetail.JobDataMap["JOBID"].ToString().Trim();
            JobHelper.strJobId = StrJobId;

            #endregion

            #region 記錄job啟動時間

            StartTime = DateTime.Now;
            JobHelper.SaveLog(StrJobId + ", HOUSE_KEEPING 啟動！", LogState.Info);

            #endregion

            #region 判斷job工作狀態

            if (JobHelper.SerchJobStatus(StrJobId) == "" || JobHelper.SerchJobStatus(StrJobId) == "0")
            {
                JobHelper.SaveLog("JOB 工作狀態為：停止！", LogState.Info);
                return;
            }

            #endregion

            #region 檢測JOB是否在執行中

            if (BusinessRules.BRM_LBatchLog.JobStatusChk(StrFunctionKey, StrJobId, DateTime.Now))
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不在執行           
                return;
            }
            else
            {
                BusinessRules.BRM_LBatchLog.Insert(StrFunctionKey, StrJobId, StartTime, "R", "JOB 工作狀態為：開始執行！");
            }

            #endregion


            #region 001. 清除使用者紀錄

            //* 聲明SQL Command變量
            SqlCommand sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM LOGON_INFO WHERE DATEDIFF(YEAR, SYS_TIME, GETDATE()) >= 1"
            };

            int count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            string tempMsg = "01.清除使用者紀錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 002. 清除JOB錯誤資訊

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM TBL_JOBERRORINFO WHERE DATEDIFF(YEAR,IMPORTTIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_System", ref count);
            tempMsg = "02.清除JOB錯誤資訊,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 003. 操作紀錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM TBS_LOG WHERE DATEDIFF(YEAR,CREATE_DT, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_System", ref count);
            tempMsg = "03.清除操作紀錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 004. 使用者連線ID

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM SESSION_INFO WHERE DATEDIFF(YEAR,CHANGED_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "04.清除使用者連線ID,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 005. 功能歷程記錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM M_FUNCTION_HS WHERE DATEDIFF(YEAR,CHANGED_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "05.清除功能歷程記錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 006. 動作歷程記錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM M_ACTION_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "06.清除動作歷程記錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 007. 屬性歷程記錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM M_PROPERTY_KEY_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "07.清除屬性歷程記錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 008. 角色權限歷程記錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM R_ROLE_FUNCTION_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "08.清除角色權限歷程記錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 009. 角色歷程記錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM R_ROLE_FUNCTION_KEY_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "09.清除角色歷程記錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 010. 工作日歷程記錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM WORK_DATE_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "10.清除工作日歷程記錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 011. AP執行紀錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM L_AP_LOG WHERE DATEDIFF(YEAR,QUERY_DATETIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "11.清除AP執行紀錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 012. 排程執行紀錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM L_BATCH_LOG WHERE DATEDIFF(YEAR,START_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "12.清除排程執行紀錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 013. 排程執行紀錄

            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "DELETE FROM L_BATCH_LOG WHERE DATEDIFF(YEAR,START_TIME, GETDATE()) >= 1"
            };

            count = 0;
            Delete(sql, "Connection_CSIP", ref count);
            tempMsg = "13.清除排程執行紀錄,刪除筆數：" + count;
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br>";

            #endregion

            #region 記錄結束時間

            EndTime = DateTime.Now;

            #endregion

            #region 執行結果寫入日誌

            //*批次完成記錄LOG信息
            //*插入L_BATCH_LOG資料庫
            BRL_BATCH_LOG.Delete(StrFunctionKey, StrJobId, "R");
            BRL_BATCH_LOG.Insert(StrFunctionKey, StrJobId, StartTime, "S", StrMailMsg);

            #endregion

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                string[] strTo = new[] {mailTo.Trim()};
                string strBody = StrMailMsg;
                JobHelper.SendMail(_strFrom, strTo, strSubject + "執行成功！", strBody);
            }
            else
            {
                JobHelper.SaveLog("寄信失敗，無 Email通知人資料。", LogState.Info);
            }


            JobHelper.SaveLog("HOUSE_KEEPING 結束！", LogState.Info);
        }
        catch (Exception exp)
        {
            //*批次完成記錄LOG信息
            //*插入L_BATCH_LOG資料庫
            BRL_BATCH_LOG.Delete(StrFunctionKey, StrJobId, "R");
            BRL_BATCH_LOG.Insert(StrFunctionKey, StrJobId, StartTime, "F", exp.Message.ToString());
            Logging.Log(exp, StrJobId);


            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                string[] strTo = new[] { mailTo.Trim() };
                string strBody = StrMailMsg + ",Exception:"  + exp;
                JobHelper.SendMail(_strFrom, strTo, strSubject + "執行失敗！", strBody);
            }
            else
            {
                JobHelper.SaveLog("寄信失敗，無 Email通知人資料。", LogState.Info);
            }
        }
        finally
        {
            
        }
    }

    #endregion


    /// <summary>
    /// Delete by SQL
    /// </summary>
    /// <param name="command">DbCommand</param>
    /// <param name="strConnectionName">連接字串名</param>
    private bool Delete(DbCommand command, string strConnectionName, ref int count)
    {
        DataHelper dh = new DataHelper(strConnectionName);
        try
        {
            count = dh.ExecuteNonQuery(command);
            return true;
        }
        catch (Exception exp)
        {
            Logging.Log(exp, StrJobId);
            return false;
        }
    }

}