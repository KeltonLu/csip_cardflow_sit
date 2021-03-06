//******************************************************************
//*  功能說明：自動化卡片註消   執行一次，抓取40天前FTP上OU13
//*  作    者：Talas
//*  創建日期：2019/12/30
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>

//*******************************************************************
using System;
using System.Data;
using Framework.Common.Logging;
using BusinessRules;
using System.IO;
using Framework.Common.Utility;

/// <summary>
/// AutoOUCancelOASA 的摘要描述
/// </summary>
public class OnceOUCancelOASA : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId;
    protected string strFunctionKey = "06";
   // private string strSessionId = "";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strFolderName;
    protected string strLocalPath;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;
    protected string strGetDate = "";
    /// <summary>
    /// 增加日期設定 下載檔案日期已此欄位為主，可於啟動JOB時額外設定
    /// </summary>
    public DateTime CurrentJobDate = DateTime.Now;
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:linda
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    {
        try
        {
            #region 获取jobID
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            //strJobId = "0109";

            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
            #endregion

            #region 記錄job啟動時間
            StartTime = DateTime.Now;
            #endregion

            #region 获取本地路徑
            strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId;
            strFolderName = strJobId + StartTime.ToString("yyyyMMddHHmmss");
            strLocalPath = strLocalPath + "\\" + strFolderName + "\\";
            #endregion
            #region 計數器歸零
            SCount = 0;
            FCount = 0;
            #endregion

            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId).Equals("") || JobHelper.SerchJobStatus(strJobId).Equals("0"))
            {
                JobHelper.SaveLog("JOB 工作狀態為：停止！", LogState.Info);
                return;
                //*job停止
            }
            #endregion

            #region 檢測JOB是否在執行中
            if (BRM_LBatchLog.JobStatusChk(strFunctionKey, strJobId, DateTime.Now))
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不在執行           
                return;
            }
            else
            {
                BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, "R", "開始執行");
            }
            #endregion

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000109, "IM");

            #region 登陸ftp下載注銷檔 
            ProssDownload();
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job結束日誌記錄

            //*判斷job完成狀態
            string strJobStatus = "S";          
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, "下載完成");
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            JobHelper.SaveLog("JOB結束！", LogState.Info);
            #endregion
        }
        catch (Exception ex)
        {
            JobHelper.SaveLog("發生異常, 異常原因: " + ex.ToString());

            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "CommonModel_發生錯誤_" + ex.ToString());
            BRM_LBatchLog.SaveLog(ex);
        }
        //Talas 增加清空ImportDate，不管原來有沒有設定
        finally
        {
            //如有設定參數則回復為空白(同一JOBID都清空)
            //BR_FileInfo.UpdateParameter(strJobId);

        }

    }


    /// <summary>
    /// 處理檔案下載
    /// </summary>
    private void ProssDownload()
    {
        dtFileInfo = new DataTable();


        if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
        {
            JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
            if (dtFileInfo.Rows.Count > 0)
            {
                foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                {
                    //只會有OU13
                  
                    //FTP 檔名
                    //string strFileInfo = rowFileInfo["FtpFileName"].ToString() + DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Substring(4, 4) + ".EXE";
                    //下載 40 天內所有的檔案
                    for (int i = 1; i < 40; i++)
                    {
                        string strGetDate = DateTime.Today.AddDays(-1 * i).ToString("yyyyMMdd").Substring(4, 4);
                        strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("OU13TmpFilePath");
                        if (!Directory.Exists(strLocalPath)){
                            Directory.CreateDirectory(strLocalPath);
                        }
                        string strFileInfo = rowFileInfo["FtpFileName"].ToString() + strGetDate + ".EXE";
                        strLocalPath = strLocalPath + "\\";
                        //FTP 路徑+檔名
                        string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;
                        string strFtpIp = rowFileInfo["FtpIP"].ToString();
                        string strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        string strFtpPwd = rowFileInfo["FtpPwd"].ToString();

                        FTPFactory objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                        //*檔案存在
                        if (objFtp.isInFolderList(strFtpFileInfo))
                        {
                            JobHelper.SaveLog("開始下載檔案！", LogState.Info);
                            //*下載檔案
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                            {
                                JobHelper.SaveLog("下載檔案成功！", LogState.Info);
                            }
                        }
                        //*檔案不存在
                        else
                        {
                            JobHelper.SaveLog("3. 註銷檔 : 【" + strFileInfo + "】 不存在！【FAIL】");
                        }
                    }
                }
            }
        }
        else
        {
            JobHelper.SaveLog("從DB抓取檔案資料失敗！");
        }
    }
    #endregion



}

