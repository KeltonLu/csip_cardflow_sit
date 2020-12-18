//******************************************************************
//*  作    者：Ares Luke
//*  功能說明：刪除一年前的紀錄(清除使用者紀錄, 清除JOB錯誤資訊,
//  　         操作紀錄, 使用者連線ID, 功能歷程記錄, 動作歷程記錄,
//             屬性歷程記錄, 角色權限歷程記錄, 角色歷程記錄, 工作日歷程記錄,
//             AP執行紀錄, 排程執行紀錄)
//*  創建日期：2020/08/21
//*  修改記錄：Ares Luke 2020/11/03 新增批次刪除 避免大資料造成TimeOut。
//*  修改記錄：Ares Luke 2020/12/01 新增計時器LOG。
//*  修改紀錄：2020/12/17_Ares_Stanley-修改LOG紀錄文字避免欄位不足
//*  修改記錄：Ares Luke 2020/12/17 調整多收信人。

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
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

    private readonly int BatchNum = 1000;

    private readonly string _strFrom = UtilHelper.GetAppSettings("MailSender");


    #region IJob 程式入口

    /// <summary>
    /// Job 調用入口
    /// </summary>
    /// <param name="context"></param>
    public void Execute(JobExecutionContext context)
    {
        var mailTo = context.JobDetail.JobDataMap.GetString("mail");
        var strSubject = context.JobDetail.JobDataMap.GetString("title");


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

            // 預計刪除總筆數,批次次數
            int totalNum = 0;
            decimal runNum = 0;
            // 實際刪除筆數計算
            int totalDeleteCount = 0;
            // 資料庫
            string dbName = "";
            SqlCommand sql = new SqlCommand();

            string tempMsg = "";

            #region 紀錄預計刪除日期Log
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT CONVERT(VARCHAR(10), DATEADD(YEAR,-1,GETDATE()), 111)"
            };
            var dh = new DataHelper("Connection_CSIP");
            DataSet ds = dh.ExecuteDataSet(sql);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                Logging.Log("【預計刪除 " + ds.Tables[0].Rows[0][0].ToString() + "前資料】", StrJobId, LogState.Info);
                Logging.Log("============================================================", StrJobId, LogState.Info);
            }
            #endregion 

            // 計時器
            Stopwatch sw;

            ConsumeTimeStart(sw = new Stopwatch());
            #region 001. 清除使用者紀錄

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM LOGON_INFO WHERE DATEDIFF(YEAR, SYS_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM LOGON_INFO WHERE DATEDIFF(YEAR, SYS_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };

                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "1.使用者紀錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";
            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 002. 清除JOB錯誤資訊

            dbName = "Connection_System";
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM TBL_JOBERRORINFO WHERE DATEDIFF(YEAR,IMPORTTIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM TBL_JOBERRORINFO WHERE DATEDIFF(YEAR,IMPORTTIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };

                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "2.JOB錯誤資訊,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 003. 操作紀錄

            dbName = "Connection_System";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM TBS_LOG WHERE DATEDIFF(YEAR,CREATE_DT, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM TBS_LOG WHERE DATEDIFF(YEAR,CREATE_DT, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "3.操作紀錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 004. 使用者連線ID

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM SESSION_INFO WHERE DATEDIFF(YEAR,CHANGED_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM SESSION_INFO WHERE DATEDIFF(YEAR,CHANGED_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "4.使用者連線ID,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 005. 功能歷程記錄

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM M_FUNCTION_HS WHERE DATEDIFF(YEAR,CHANGED_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM M_FUNCTION_HS WHERE DATEDIFF(YEAR,CHANGED_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "5.功能歷程記錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 006. 動作歷程記錄

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM M_ACTION_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM M_ACTION_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "6.動作歷程記錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 007. 屬性歷程記錄

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM M_PROPERTY_KEY_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM M_PROPERTY_KEY_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "7.屬性歷程記錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 008. 角色權限歷程記錄

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM R_ROLE_FUNCTION_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM R_ROLE_FUNCTION_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "8.角色權限歷程記錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 009. 角色歷程記錄


            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM R_ROLE_FUNCTION_KEY_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM R_ROLE_FUNCTION_KEY_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "9.角色歷程記錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 010. 工作日歷程記錄


            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM WORK_DATE_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM WORK_DATE_HS WHERE DATEDIFF(YEAR,ACTION_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "10.工作日歷程記錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 011. AP執行紀錄

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM L_AP_LOG WHERE DATEDIFF(YEAR,QUERY_DATETIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM L_AP_LOG WHERE DATEDIFF(YEAR,QUERY_DATETIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "11.AP執行紀錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg + "<br/>";

            #endregion
            ConsumeTimeEnd(sw);

            ConsumeTimeStart(sw = new Stopwatch());
            #region 012. 排程執行紀錄

            dbName = "Connection_CSIP";
            //* 聲明SQL Command變量
            sql = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) FROM L_BATCH_LOG WHERE DATEDIFF(YEAR,START_TIME, GETDATE()) >= 1"
            };
            
            TotalNum(sql, dbName, ref totalNum, ref runNum);
            
            //實際刪除筆數計算
            totalDeleteCount = 0;
            for (int i = 0; i < runNum; i++)
            {
                //* 聲明SQL Command變量
                sql = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "WITH DEL AS(SELECT TOP " + BatchNum +
                                  " * FROM L_BATCH_LOG WHERE DATEDIFF(YEAR,START_TIME, GETDATE()) >= 1) DELETE DEL WHERE 1=1"
                };
            
                int count = 0;
                Delete(sql, dbName, ref count);
                totalDeleteCount += count;
            }

            tempMsg = "12.排程執行紀錄,刪除：" + totalDeleteCount + "筆";
            Logging.Log(tempMsg, StrJobId, LogState.Info);
            StrMailMsg += tempMsg ;

            #endregion
            ConsumeTimeEnd(sw);

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
                var strTo = mailTo.Trim().Split(';');
                var strBody = StrMailMsg;
                if (JobHelper.SendMail(_strFrom, strTo, strSubject + "執行成功！", strBody))
                {
                    JobHelper.SaveLog("寄信成功。", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog("寄信失敗。", LogState.Info);
                }
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
                var strTo = new[] {mailTo.Trim()};
                var strBody = StrMailMsg + ",Exception:" + exp;
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
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:Delete by SQL
    /// 作    者:Ares Luke
    /// 創建時間:2020/08/21
    /// 修改紀錄:2020/11/18_Ares_Luke-新增TimeOut時間
    /// 2020/12/01_Ares_Luke-新增執行SQL的LOG
    /// </summary>
    /// <param name="command">DbCommand</param>
    /// <param name="strConnectionName">連接字串名</param>
    /// <param name="count">筆數</param>
    private bool Delete(DbCommand command, string strConnectionName, ref int count)
    {
        var dh = new DataHelper(strConnectionName);
        try
        {
            Logging.Log("執行SQL:" + command.CommandText, StrJobId, LogState.Info);
            command.CommandTimeout = int.Parse(UtilHelper.GetAppSettings("SqlCmdTimeoutMax"));
            count = dh.ExecuteNonQuery(command);
            return true;
        }
        catch (Exception exp)
        {
            Logging.Log(exp, StrJobId);
            return false;
        }
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:統計總筆數與批次執行次數
    /// 作    者:Ares Luke
    /// 創建時間:2020/08/21
    /// 修改紀錄:2020/11/18_Ares_Luke-新增TimeOut時間
    /// </summary>
    /// <param name="command">DbCommand</param>
    /// <param name="strConnectionName">連接字串名</param>
    /// <param name="totalNum">總筆數</param>
    /// <param name="runNum">執行次數</param>
    private void TotalNum(DbCommand command, string strConnectionName, ref int totalNum, ref decimal runNum)
    {
        var dh = new DataHelper(strConnectionName);
        try
        {
            command.CommandTimeout = int.Parse(UtilHelper.GetAppSettings("SqlCmdTimeoutMax"));
            totalNum = 0;
            runNum = 0;
            totalNum = (Int32) dh.ExecuteScalar(command);
            runNum = (decimal) Math.Ceiling((double) totalNum / BatchNum);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, StrJobId);
        }
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:開始計時器並記錄LOG
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/01
    /// 修改紀錄:
    /// </summary>
    /// <param name="sw"></param>
    private void ConsumeTimeStart(Stopwatch sw)
    {
        Logging.Log("開始時間:" + DateTime.Now.ToString(), StrJobId, LogState.Info);
        sw.Start();
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:結束計時器並記錄LOG
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/01
    /// 修改紀錄:
    /// </summary>
    /// <param name="sw"></param>
    private void ConsumeTimeEnd(Stopwatch sw)
    {
        Logging.Log("結束時間:" + DateTime.Now.ToString(), StrJobId, LogState.Info);
        Logging.Log("總花費時間:" + sw.ElapsedMilliseconds + "毫秒", StrJobId, LogState.Info);
        Logging.Log("============================================================", StrJobId, LogState.Info);
        sw.Stop();
    }
}