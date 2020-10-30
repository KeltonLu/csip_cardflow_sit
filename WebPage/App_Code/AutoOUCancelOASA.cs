//******************************************************************
//*  功能說明：自動化卡片註消
//*  作    者：linda
//*  創建日期：2010/07/06
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//  Nash (U)          20190503          就算OU檔案是空檔，也會寫入系統中讓User可以知道資料筆數為0
//  Nash (U)          20190503          新增LOG
//  Area Luke         20201023          新增(當日最後排程啟動時)寄信需求。
//*******************************************************************
using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Common.Utility;
using Framework.Data.OM;
using CSIPCommonModel.EntityLayer;
using System.Collections.Generic;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;

/// <summary>
/// AutoOUCancelOASA 的摘要描述
/// </summary>
public class AutoOUCancelOASA : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId;
    protected string strFunctionKey = "06";
    private string strSessionId = "";
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
    protected string strJobLogMsg = string.Empty;
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
            #endregion

            #region 記錄job啟動時間
            StartTime = DateTime.Now;
            #endregion

            #region 获取本地路徑
            strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId;
            strFolderName = strJobId + StartTime.ToString("yyyyMMddHHmmss");
            strLocalPath = strLocalPath + "\\" + strFolderName + "\\";
            #endregion
            strJobLogMsg = strJobLogMsg + "###################" + StartTime + " JOB : 【" + strJobId + "】啟動 ###################\n";
            //20190503 (U) by Nash 新增文字檔LOG
            JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】1. 啟動", LogState.Info);
            #region 計數器歸零
            SCount = 0;
            FCount = 0;
            #endregion

            #region 匯入資料明細
            dtLocalFile = new DataTable();
            dtLocalFile.Columns.Add("LocalFilePath");      //本地全路徑
            dtLocalFile.Columns.Add("FtpFilePath");        //Ftp全路徑
            dtLocalFile.Columns.Add("FolderName");         //目錄名稱
            dtLocalFile.Columns.Add("FileName");        //資料檔名
            dtLocalFile.Columns.Add("FileDate");         //檔案日期
            dtLocalFile.Columns.Add("ZipPwd");           //壓縮密碼
            dtLocalFile.Columns.Add("ZipStates");        //壓縮狀態
            dtLocalFile.Columns.Add("ZipFileName");      //壓縮檔檔名
            dtLocalFile.Columns.Add("CheckStates");        //數據格式驗證狀態
            dtLocalFile.Columns.Add("ImportStates");       //資料匯入狀態
            #endregion

            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId).Equals("") || JobHelper.SerchJobStatus(strJobId).Equals("0"))
            {
                JobHelper.SaveLog(DateTime.Now.ToString() + "JOB 工作狀態為：停止！", LogState.Info);
                return;
                //*job停止
            }
            #endregion
            
            #region 檢測JOB是否在執行中
            if (BRM_LBatchLog.JobStatusChk(strFunctionKey, strJobId, DateTime.Now))
            {
            
                JobHelper.SaveLog(DateTime.Now.ToString() + "JOB 工作狀態為：正在執行！", LogState.Info);
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
            #region 處理本地壓縮檔  
            ProcDeCompress();
            #endregion
            #region 開始資料匯入
            ProcImport();
            #endregion
            #region OASA注銷   找出今日註銷，發送電文
            ProcRegOASA(context);
            #endregion

            #region 當日最後一次排程後寄送OU MAIL
            JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 開始判斷是否為當日最後排程！", LogState.Info);

            DataTable dtProperty = new DataTable();

            bool isLastTime = false;
            if (BRM_PROPERTY_KEY.GetProperty("06", "0109Time", ref dtProperty))
            {
                DataRow[] drHh = dtProperty.Select("PROPERTY_CODE='" + "hh" + "'");
                DataRow[] drMm = dtProperty.Select("PROPERTY_CODE='" + "mm" + "'");
                DataRow[] drSs = dtProperty.Select("PROPERTY_CODE='" + "ss" + "'");
                if (drHh.Length > 0 && drMm.Length > 0 && drSs.Length > 0)
                {
                    DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                        int.Parse(drHh[0]["PROPERTY_NAME"].ToString()),
                        int.Parse(drMm[0]["PROPERTY_NAME"].ToString()),
                        int.Parse(drSs[0]["PROPERTY_NAME"].ToString()));

                    isLastTime = DateTime.Compare(StartTime, d) >= 0;
                }
            }

            if (isLastTime)
            {
                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 判斷結果: 是！", LogState.Info);
                //* 聲明SQL Command變量
                SqlCommand sqlCommand = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText =
                        "SELECT ROW_NUMBER() over (order by a.CancelOASADate) as no, b.CancelOASAFile, CASE WHEN LEN(CardNo) < 8 THEN '卡號長度異常' ELSE REPLACE(CARDNO, SUBSTRING(CARDNO, 5, LEN(CARDNO) - 8), SUBSTRING('XXXXXXXXXXXXXXXX', 1, LEN(CARDNO) - 8)) END AS CardNo, BlockCode, MemoLog , '注銷失敗' AS SFFLG " +
                        "FROM TBL_CANCELOASA A " +
                        "LEFT JOIN TBL_CANCELOASA_DETAIL B ON A.CANCELOASAFILE = B.CANCELOASAFILE " +
                        "WHERE B.SFFLG = '2' " +
                        "AND(A.CANCELOASAFILE LIKE 'OU15%' OR A.CANCELOASAFILE LIKE 'OU16%') " +
                        "AND B.CARDNO LIKE('0377%') " +
                        "AND A.CANCELOASADATE = CONVERT(VARCHAR, GETDATE(), 111)"
                };

                DataSet ds = BRM_CancelOASA.SearchOnDataSet(sqlCommand);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 查詢結果共" + dt.Rows.Count + "筆！", LogState.Info);
                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 開始寄信！", LogState.Info);
                        //發送Mail
                        SendMail("5", new ArrayList(), Resources.JobResource.Job0000036, dt);
                    }
                    else
                    {
                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 查詢無資料！", LogState.Info);
                        //發送Mail
                        SendMail("5", new ArrayList(), Resources.JobResource.Job0000036, null);
                    }
                }
                else
                {
                    JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 查詢無資料！", LogState.Info);
                    //發送Mail
                    SendMail("5", new ArrayList(), Resources.JobResource.Job0000036, null);
                }
            }
            else
            {
                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 判斷結果: 否！", LogState.Info);
            }

            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job結束日誌記錄
            //*判斷job完成狀態
            DataRow[] RowS = dtLocalFile.Select("ImportStates='S'");
            DataRow[] RowF = dtLocalFile.Select("ImportStates='F'");

            if (RowS != null && RowS.Length > 0)
            {
                SCount = RowS.Length;
            }
            if (RowF != null && RowF.Length > 0)
            {
                FCount = RowF.Length;
            }
            //*判斷job完成狀態
            string strJobStatus = JobHelper.GetJobStatus(SCount, FCount);
            string strReturnMsg = string.Empty;
            strReturnMsg += Resources.JobResource.Job0000024 + SCount;
            strReturnMsg += Resources.JobResource.Job0000025 + FCount + "!";
            if (RowF != null && RowF.Length > 0)
            {
                string strTemps = string.Empty;
                for (int k = 0; k < RowF.Length; k++)
                {
                    strTemps += RowF[k]["FileName"].ToString() + "  ";
                }
                strReturnMsg += Resources.JobResource.Job0000026 + strTemps;
            }
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】9. 執行結束！", LogState.Info);
            #endregion
        }
        catch (Exception ex)
        {
            strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB AutoImportFiles : 【" + strJobId + "】發生異常 , 異常原因 : " + ex.ToString() + " \n";
            JobHelper.SaveLog(DateTime.Now.ToString() + strJobLogMsg);
            //20190503 (U) by Nash 新增文字檔LOG
            JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】發生異常 , 異常原因 : " + ex.ToString() + "【FAIL】");

            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "CommonModel_發生錯誤_" + ex.ToString());
            BRM_LBatchLog.SaveLog(ex);
            //20190503 (U) by Nash 新增文字檔LOG
            JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】CommonModel_發生錯誤_" + ex.ToString() + "【FAIL】");
        }
        //Talas 增加清空ImportDate，不管原來有沒有設定
        finally
        {
            JobHelper.SaveLog(DateTime.Now.ToString() + strJobLogMsg);
            //如有設定參數則回復為空白(同一JOBID都清空)
            //BR_FileInfo.UpdateParameter(strJobId);

        }

    }
    /// <summary>
    /// 處理OASA註銷 (打電文)
    /// </summary>
    /// <param name="context"></param>
    private void ProcRegOASA(JobExecutionContext context)
    {
        DataTable dtFileInfoOASA = new DataTable();
        if (JobHelper.SearchFileInfo(ref dtFileInfoOASA, strJobId))
        {
            ///抓tb_FILEINFO 找出要下載的檔案名稱及設定
            if (dtFileInfoOASA.Rows.Count > 0)
            {
                foreach (DataRow rowFileInfoOASA in dtFileInfoOASA.Rows)
                {

                    string strFtpFileName = rowFileInfoOASA["FtpFileName"].ToString().Trim();
                    string strCancelTime = rowFileInfoOASA["CancelTime"].ToString().Trim();
                    //多增加2的屬性，抓取今日匯入的
                    strCancelTime = "2";
                    string strBlkCode = rowFileInfoOASA["BLKCode"].ToString().Trim();
                    string strMemo = rowFileInfoOASA["MEMO"].ToString().Trim();
                    string strReasonCode = rowFileInfoOASA["ReasonCode"].ToString().Trim();
                    string strActionCode = rowFileInfoOASA["ActionCode"].ToString().Trim();

                    DataTable dtOASASourceFileInfo = new DataTable();
                    //由 tbl_CancelOASA_Source 找出 下載檔名相符，更新日期為當日
                    if (BRM_CancelOASASource.SearchOASASourceFileInfo(strFtpFileName, strCancelTime, ref dtOASASourceFileInfo))
                    {
                        if (dtOASASourceFileInfo.Rows.Count > 0)
                        {
                            DataTable dtCardInfoOASA = new DataTable();
                            BRM_CancelOASASource.SearchOASACardInfo(strFtpFileName, strCancelTime, ref dtCardInfoOASA);

                            foreach (DataRow rowOASASourceFileInfo in dtOASASourceFileInfo.Rows)
                            {
                                int intSOASACount = 0;
                                int intFOASACount = 0;
                                string strImportFile = rowOASASourceFileInfo["ImportFile"].ToString().Trim();
                                string strImportDate = rowOASASourceFileInfo["ImportDate"].ToString().Trim();
                                string strOASAUserId = string.Empty;
                                //依檔名及匯入日期，將 tbl_CancelOASA_Source 狀態Stauts變更為1  位置怪怪的
                                BRM_CancelOASASource.UpdateStauts(strImportFile, strImportDate);

                                DataRow[] rowCardInfoOASA = dtCardInfoOASA.Select("ImportFile='" + strImportFile + "' and ImportDate='" + strImportDate + "'");
                                EntitySet<Entity_CancelOASA_Detail> SetCancelOASADetail = new EntitySet<Entity_CancelOASA_Detail>();

                                for (int i = 0; i < rowCardInfoOASA.Length; i++)
                                {
                                    //20190503 (U) by Nash 新增文字檔LOG及將卡號隱碼                                         
                                    string strCardNo = rowCardInfoOASA[i]["CardNo"].ToString().Trim();
                                    string strCardNohead = rowCardInfoOASA[i]["CardNo"].ToString().Trim().Substring(0, 6);
                                    string strCardNolast = rowCardInfoOASA[i]["CardNo"].ToString().Trim().Substring(12, 4);
                                    string strCardNolog = strCardNohead + "XXXXXX" + strCardNolast;  //將卡號隱碼
                                    string strExpDate = rowCardInfoOASA[i]["ExpDate"].ToString().Trim();
                                    string strMemoLog = string.Empty;
                                    string strBlockLog = string.Empty;
                                    string strSFFlg = string.Empty;
                                    string strSource = string.Empty;

                                    if (strImportFile.Substring(0, 4) == "ou15" || strImportFile.Substring(0, 4) == "ou16")
                                    {
                                        strSource = "K";
                                    }
                                    else
                                    {
                                        strSource = "Z";
                                    }


                                    if (this.HtgOASAAdd(strSource, strCardNo, strExpDate, strBlkCode, strMemo, strReasonCode, strActionCode, ref strMemoLog, ref strBlockLog, ref strOASAUserId, context))
                                    {
                                        intSOASACount++;
                                        strSFFlg = "1";
                                        //20190503 (U) by Nash 新增文字檔LOG及將卡號隱碼
                                        strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strImportFile + "】 卡號 : 【" + strCardNolog + "】 JCAX 發送成功,資訊如下 ~\n";
                                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】8. 註銷檔 : 【" + strImportFile + "】 卡號 : 【" + strCardNolog + "】 JCAX 發送成功,資訊如下", LogState.Info);
                                    }
                                    else
                                    {
                                        intFOASACount++;
                                        strSFFlg = "2";
                                        //20190503 (U) by Nash 新增文字檔LOG及將卡號隱碼
                                        strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strImportFile + "】 卡號 : 【" + strCardNolog + "】 JCAX 發送失敗,資訊如下 ~\n";
                                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】8. 註銷檔 : 【" + strImportFile + "】 卡號 : 【" + strCardNolog + "】 JCAX 發送失敗,資訊如下【FAIL】");
                                    }

                                    strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + "              CardNo : 【" + strCardNolog + "】 \n";
                                    strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + "              ExpDate : 【" + strExpDate + "】 \n";
                                    strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + "              BlkCode : 【" + strBlkCode + "】 \n";
                                    strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + "              Memo : 【" + strMemo + "】 \n";
                                    strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + "              ReasonCode : 【" + strReasonCode + "】 \n";
                                    strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + "              ActionCode : 【" + strActionCode + "】 \n";


                                    Entity_CancelOASA_Detail CancelOASADetail = new Entity_CancelOASA_Detail();
                                    CancelOASADetail.CancelOASAFile = strImportFile;
                                    CancelOASADetail.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                                    CancelOASADetail.CardNo = strCardNo;
                                    CancelOASADetail.BlockCode = strBlkCode;
                                    CancelOASADetail.ActionCode = strActionCode;
                                    CancelOASADetail.Memo = strMemo;
                                    CancelOASADetail.ReasonCode = strReasonCode;
                                    CancelOASADetail.MemoLog = strMemoLog;
                                    CancelOASADetail.BlockLog = strBlockLog;
                                    CancelOASADetail.SFFlg = strSFFlg;
                                    SetCancelOASADetail.Add(CancelOASADetail);
                                }


                                //  EntitySet<Entity_CancelOASA> SetCancelOASA = new EntitySet<Entity_CancelOASA>();
                                Entity_CancelOASA CancelOASA = new Entity_CancelOASA();
                                CancelOASA.CancelOASAFile = strImportFile;
                                CancelOASA.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                                CancelOASA.CancelOASAUser = strOASAUserId;
                                CancelOASA.TotalCount = intFOASACount + intSOASACount;
                                CancelOASA.SCount = intSOASACount;
                                CancelOASA.FCount = intFOASACount;
                                CancelOASA.CancelOASASource = "0";
                                //需要調整，寫入前先查詢是否有資料，有則更新，無則新增  由原有方法切離
                                //SetCancelOASA.Add(CancelOASA);
                                //if (BRM_CancelOASA.BatInsert(SetCancelOASA))
                                if (BR_CancelOASA.InsertCancelOASA(CancelOASA))
                                {
                                    string strMsgID = string.Empty;
                                    BRM_CancelOASADetail.BatInsert(SetCancelOASADetail, ref strMsgID);
                                }
                            }
                        }
                    }
                }
            }
        }
        MainFrameInfoOASA.ClearHtgSessionJob(ref strSessionId, strJobId);

        JobHelper.SaveLog(DateTime.Now.ToString() + strJobLogMsg, LogState.Info);
    }

    //處理本地資料匯入
    private void ProcImport()
    {
        strJobLogMsg = strJobLogMsg + "===============" + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】資料處理 (檢核/匯入)開始！===============\n";
        //20190503 (U) by Nash 新增文字檔LOG
        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】6. 資料處理 (檢核/匯入)開始！", LogState.Info);
        if (dtLocalFile.Rows.Count > 0)
        {
            //*讀取檔名正確資料
            foreach (DataRow Row in dtLocalFile.Rows)
            {
                string strFileName = Row["FileName"].ToString();
                string strPath = Row["LocalFilePath"].ToString();
                //  string strPath = strLocalPath + Row[rowcount]["FileName"].ToString().Trim();
                string strFunctionName = Resources.JobResource.Job0000109;
                //*file存在local
                if (File.Exists(strPath))
                {
                    string strMsgID = string.Empty;
                    ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                    DataTable dtDetail = null;                 //檢核結果列表

                    //*檢核成功
                    if (UploadCheck(strPath, strFunctionName, strFileName.Substring(0, 4), ref strMsgID, ref arrayErrorMsg, ref dtDetail))
                    {
                        Row["CheckStates"] = "S";
                        //*正式匯入
                        if (ImportData(dtDetail, strFileName))
                        {
                            Row["ImportStates"] = "S";
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101002, strFileName), LogState.Info);
                            strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strFileName + "】 檢核/匯入成功！\n";
                            //20190503 (U) by Nash 新增文字檔LOG
                            JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】7. 註銷檔 : 【" + strFileName + "】 檢核/匯入成功！", LogState.Info);
                        }
                        else
                        {
                            Row["ImportStates"] = "F";
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003, strFileName));
                            strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strFileName + "】 匯入失敗！\n";
                            //20190503 (U) by Nash 新增文字檔LOG
                            JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】7. 註銷檔 : 【" + strFileName + "】 匯入失敗！【FAIL】");
                        }
                    }
                    //*檢核失敗
                    else
                    {
                        Row["CheckStates"] = "F";
                        Row["ImportStates"] = "F";
                        //檔案格式檢核失敗，一樣寫入
                        Entity_CancelOASA CancelOASA = new Entity_CancelOASA();
                        CancelOASA.CancelOASAFile = strFileName;
                        CancelOASA.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                        CancelOASA.CancelOASAUser = "sys";
                        CancelOASA.TotalCount = -2;
                        CancelOASA.SCount = -2;
                        CancelOASA.FCount = -2;
                        CancelOASA.CancelOASASource = "0";
                        // SetEmptyCancelOASA.Add(CancelOASA);   
                        //  blnResult = BRM_CancelOASA.BatInsert(SetEmptyCancelOASA);
                        BR_CancelOASA.InsertCancelOASA(CancelOASA);


                        string strError = string.Empty;
                        if (arrayErrorMsg.Count > 0)
                        {
                            for (int intError = 0; intError < arrayErrorMsg.Count; intError++)
                            {
                                strError += arrayErrorMsg[intError] + " ";
                            }
                        }
                        else
                        {
                            strError = Resources.JobResource.ResourceManager.GetString(strMsgID);
                        }

                        strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strFileName + "】 檢核失敗！\n";
                        //20190503 (U) by Nash 新增文字檔LOG
                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】7. 註銷檔 : 【" + strFileName + "】 檢核失敗！【FAIL】");

                        ArrayList alInfo = new ArrayList();
                        alInfo.Add(strFileName);
                        alInfo.Add(strError);
                        //內容格式錯誤
                        SendMail("2", alInfo, Resources.JobResource.Job0000036);
                    }


                }
                //*file不存在local
                else
                {
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000035, strPath));
                }

            }
        }
    }

    /// <summary>
    /// 處理本地壓縮檔
    /// </summary>
    private void ProcDeCompress()
    {
        strJobLogMsg = strJobLogMsg + "===============" + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】開始處理壓縮檔！===============\n";
        //20190503 (U) by Nash 新增文字檔LOG
        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】4. 開始處理壓縮檔！", LogState.Info);
        String errMsg = "";

        foreach (DataRow rowLocalFile in dtLocalFile.Rows)
        {
            string strZipFileName = rowLocalFile["ZipFileName"].ToString().Trim();
            //if (strZipFileName.Substring(0, 4) != "OS56")
            //{
            bool blnResult = ExeFile(strLocalPath, strZipFileName, rowLocalFile["ZipPwd"].ToString());
            ////*解壓成功
            if (blnResult)
            {
                rowLocalFile["ZipStates"] = "S";
                rowLocalFile["FileName"] = strZipFileName.Replace(".EXE", "");
                rowLocalFile["LocalFilePath"] = strLocalPath + strZipFileName.Replace(".EXE", "");
                strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strZipFileName + "】 解壓縮檔案成功！\n";
                //20190503 (U) by Nash 新增文字檔LOG
                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】5. 註銷檔 : 【" + strZipFileName + "】 解壓縮檔案成功！", LogState.Info);
                //if (strZipFileName.Substring(0, 4).Equals("os66"))
                //{
                //    rowLocalFile["FileName"] = strZipFileName.Replace(".EXE", ".txt");
                //}
                //else
                //{
                //    rowLocalFile["FileName"] = strZipFileName.Replace(".EXE", ".txt");
                //}
            }
            //*解壓失敗
            else
            {
                errMsg += (errMsg == "" ? "" : "、") + strZipFileName;
                rowLocalFile["ZipStates"] = "F";
                // ArrayList alInfo = new ArrayList();
                // alInfo.Add(strZipFileName);
                strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strZipFileName + "】 解壓縮檔案失敗！\n";
                //20190503 (U) by Nash 新增文字檔LOG
                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】5. 註銷檔 : 【" + strZipFileName + "】 解壓縮檔案失敗！【FAIL】");
                //解壓失敗發送Mail通知
                // SendMail("1", alInfo, Resources.JobResource.Job0000002);

            }
            //}
            //else
            //{
            //    rowLocalFile["ZipStates"] = "S";
            //    rowLocalFile["FileName"] = strZipFileName;
            //}

        }

        if (errMsg != "")
        {
            ArrayList alInfo = new ArrayList();
            alInfo.Add(errMsg);
            //解壓失敗發送Mail通知
            SendMail("1", alInfo, Resources.JobResource.Job0000002);
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
            if (dtFileInfo.Rows.Count > 0)
            {
                string strMsg = string.Empty;
                strJobLogMsg = strJobLogMsg + "===============" + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】開始下載檔案！===============\n";
                //20190503 (U) by Nash 新增文字檔LOG
                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】2. 開始下載檔案！", LogState.Info);
                //取得當日以下載檔案，避免重複下載   --調整為全部已下載檔案
                Dictionary<string, string> SDC = GetFileDic();
                foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                {
                    //Talas 20191226 增加對於OU13下載處理的旗標
                    bool isOu13 = false;
                    string ou13RealDate = "";
                    //CurrentJobDate 欄位運用，則使用該日期作為執行基準日   不再已系統日為主
                    if (!string.IsNullOrEmpty(rowFileInfo["ImportDate"].ToString()))
                    {
                        string baseDate = rowFileInfo["ImportDate"].ToString();
                        //Talas 調整輸入為八碼日期，轉換為真正日期
                        if (baseDate.Length == 8)
                        {
                            baseDate = baseDate.Substring(0, 4) + "/" + baseDate.Substring(4, 2) + "/" + baseDate.Substring(6, 2);
                        }
                        DateTime.TryParse(baseDate, out CurrentJobDate);
                    }

                    //FTP 檔名
                    //string strFileInfo = rowFileInfo["FtpFileName"].ToString() + DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Substring(4, 4) + ".EXE";
                    //要依據不同的檔名抓不同的區間
                    //OU04前一天的檔名。例：今天5/3註銷日期為5/2的檔案。
                    //OU15與OU16抓前一工作天的檔名。例：今天5/9註銷日期為5/6的檔案。
                    //OU09與OU13抓31天前的檔名。例：今天5/3要註銷4/2的檔案。
                    //Talas 20191226  OU09與OU13調整為抓1天前的檔名。例：今天5/3要註銷5/2的檔案。原因為FTP
                    if (rowFileInfo["FtpFileName"].ToString().Equals("ou04") || rowFileInfo["FtpFileName"].ToString().Equals("ou19") || rowFileInfo["FtpFileName"].ToString().Equals("ou20"))
                    {
                        // strGetDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Substring(4, 4);
                        strGetDate = CurrentJobDate.AddDays(-1).ToString("yyyyMMdd").Substring(4, 4);

                        //strGetDate = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyyMMdd").Substring(4, 4);
                    }
                    else if (rowFileInfo["FtpFileName"].ToString().Equals("ou15") || rowFileInfo["FtpFileName"].ToString().Equals("ou16"))
                    {
                        //  strGetDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Substring(4, 4);
                        //strGetDate = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyyMMdd").Substring(4, 4);
                        strGetDate = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", CurrentJobDate.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyyMMdd").Substring(4, 4);

                    }
                    else
                    {
                        isOu13 = true;
                        //strGetDate = DateTime.Now.AddDays(-31).ToString("yyyyMMdd").Substring(4, 4);
                        //要下載的，改成D-1
                        strGetDate = CurrentJobDate.AddDays(-1).ToString("yyyyMMdd").Substring(4, 4);

                        //要匯入的，還是 D-31
                        ou13RealDate = CurrentJobDate.AddDays(-31).ToString("yyyyMMdd").Substring(4, 4);
                        // strGetDate = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -31), "yyyyMMdd", null).ToString("yyyyMMdd").Substring(4, 4);
                    }
                    //  string strFileInfo = rowFileInfo["FtpFileName"].ToString() + DateTime.Now.ToString("yyyyMMdd").Substring(4, 4) + ".EXE";
                    string strFinfo = rowFileInfo["FtpFileName"].ToString() + strGetDate;
                    string strFileInfo = strFinfo + ".EXE";
                    //若已下載，則略過
                    if (SDC.ContainsKey(strFileInfo))
                    {
                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】3. 註銷檔 : 【" + strFileInfo + "】 已存在，不重新下載！", LogState.Info);
                        continue;
                    }
                    //FTP 路徑+檔名
                    string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;
                    string strFtpIp = rowFileInfo["FtpIP"].ToString();
                    string strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                    string strFtpPwd = rowFileInfo["FtpPwd"].ToString();

                    FTPFactory objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                    //*檔案存在
                    if (objFtp.isInFolderList(strFtpFileInfo))
                    {
                        //先判斷是否OU13，因為要下載的路徑不一樣
                        if (isOu13)
                        {
                            // strLocalPath = UtilHelper.GetAppSettings("OU13TmpFilePath");
                            strLocalPath = strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("OU13TmpFilePath") + "\\";

                            if (!Directory.Exists(strLocalPath))
                            {
                                Directory.CreateDirectory(strLocalPath);
                            }
                        }
                        else
                        {
                            strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName + "\\";
                        }


                        //*下載檔案
                        if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                        {
                            //非IU13~寫入後續處理
                            if (!isOu13)
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath + strFileInfo; //本地路徑
                                row["FtpFilePath"] = strFtpFileInfo; //FTP路徑
                                row["FolderName"] = strLocalPath; //本地資料夾
                                row["FileName"] = strFileInfo; //注銷檔名稱
                                row["ZipFileName"] = strFileInfo; //注銷檔名稱
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //FTP壓縮檔密碼解密
                                dtLocalFile.Rows.Add(row);
                                strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strFileInfo + "】 下載成功！\n";
                                //20190503 (U) by Nash 新增文字檔LOG
                                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】3. 註銷檔 : 【" + strFileInfo + "】 下載成功！", LogState.Info);
                            }

                        }
                    }
                    //*檔案不存在
                    else
                    {
                        if (!isOu13)
                        {
                            //增加一個寫入NA，但OU13特殊，需另外處理
                            Entity_CancelOASA CancelOASA = new Entity_CancelOASA();
                            CancelOASA.CancelOASAFile = strFinfo;
                            CancelOASA.CancelOASADate = CurrentJobDate.ToString("yyyy/MM/dd");
                            CancelOASA.CancelOASAUser = "sys";
                            CancelOASA.TotalCount = -1;
                            CancelOASA.SCount = -1;
                            CancelOASA.FCount = -1;
                            CancelOASA.CancelOASASource = "0";
                            BR_CancelOASA.InsertCancelOASA(CancelOASA);
                        }
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0109001, rowFileInfo["FtpFileName"].ToString()));
                        strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strFileInfo + "】 不存在！\n";
                        //20190503 (U) by Nash 新增文字檔LOG
                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】3. 註銷檔 : 【" + strFileInfo + "】 不存在！【FAIL】");
                    }
                    //OU13的處理移出，獨立規則，找31天前的
                    if (isOu13)
                    {
                        //指定回真正作業目錄
                        strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //增加目錄檢核
                        if (!Directory.Exists(strLocalPath))
                        {
                            Directory.CreateDirectory(strLocalPath);
                        }

                        //到  strLocalPath 找 31天前的檔名
                        string strOUFileInfo = rowFileInfo["FtpFileName"].ToString() + ou13RealDate + ".EXE";
                        string WorkPath = strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("OU13TmpFilePath");    //暫存檔目錄
                        string LocalFile = strLocalPath + strOUFileInfo;       //應匯入檔案，在真正下載目錄
                        string importFile = WorkPath + "\\" + strOUFileInfo;   //31天前檔案，在暫存目錄
                        if (File.Exists(importFile))
                        {
                            //存在則視為已下載，不處理   不存在，才複製
                            if (!File.Exists(LocalFile))
                            {
                                File.Move(importFile, LocalFile);
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = LocalFile; //本地路徑
                                row["FtpFilePath"] = strFtpFileInfo; //FTP路徑
                                row["FolderName"] = strLocalPath; //本地資料夾    
                                row["FileName"] = strOUFileInfo; //注銷檔名稱
                                row["ZipFileName"] = strOUFileInfo; //注銷檔名稱
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //FTP壓縮檔密碼解密
                                dtLocalFile.Rows.Add(row);
                                strJobLogMsg = strJobLogMsg + DateTime.Now.ToString() + " JOB : 【" + strJobId + "】註銷檔 : 【" + strOUFileInfo + "】 搬移成功！\n";
                                //20190503 (U) by Nash 新增文字檔LOG
                                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】3. 註銷檔 : 【" + strOUFileInfo + "】 搬移成功！", LogState.Info);
                            }
                        }
                        else //檔案不存在，寫入NA
                        {
                            //增加一個寫入NA 
                            Entity_CancelOASA CancelOASA = new Entity_CancelOASA();
                            CancelOASA.CancelOASAFile = rowFileInfo["FtpFileName"].ToString() + ou13RealDate;
                            CancelOASA.CancelOASADate = CurrentJobDate.ToString("yyyy/MM/dd");
                            CancelOASA.CancelOASAUser = "sys";
                            CancelOASA.TotalCount = -1;
                            CancelOASA.SCount = -1;
                            CancelOASA.FCount = -1;
                            CancelOASA.CancelOASASource = "0";
                            BR_CancelOASA.InsertCancelOASA(CancelOASA);

                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 匯入資料檢核
    /// <summary>
    /// 功能說明:匯入資料檢核
    /// 作    者:Linda
    /// 創建時間:2010/06/07
    /// 修改記錄:
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="strTpye"></param>
    /// <returns></returns>
    public bool UploadCheck(string strPath, string strFunctionName, string strFileName, ref string strMsgID, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
    {
        bool blnResult = true;
        #region base parameter
        string strUserID = "";
        string strFunctionKey = "06";
        string strUploadID = "";
        DateTime dtmThisDate = DateTime.Now;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        int intMax = 15000;
        #endregion

        switch (strFileName)
        {
            case "ou04":
                strUploadID = "06010900";
                break;
            case "ou09":
                strUploadID = "06010901";
                break;
            case "ou19":
                strUploadID = "06010901";
                break;
            case "ou20":
                strUploadID = "06010901";
                break;
            case "ou13":
                strUploadID = "06010902";
                break;
            case "ou15":
                strUploadID = "06010903";
                break;
            case "ou16":
                strUploadID = "06010904";
                break;
        }

        dtDetail = BaseHelper.UploadCheck(strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, strFunctionName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

        //*檢核成功
        if (strMsgID == "" && arrayErrorMsg.Count == 0)
        {
            blnResult = true;

            if (strFileName.Equals("ou09") || strFileName.Equals("ou19") || strFileName.Equals("ou20"))
            {
                if (dtDetail.Rows.Count > 0)
                {
                    for (int i = 0; i < dtDetail.Rows.Count; i++)
                    {
                        if (dtDetail.Rows[i]["CardNo"].ToString().Equals("0000000000000000"))
                        {
                            dtDetail.Rows[i]["CardNo"] = dtDetail.Rows[i]["CardNo2"].ToString();
                        }
                    }
                }
            }

            if (dtDetail.Rows.Count > 0)
            {
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    if (!ValidateHelper.IsNum(dtDetail.Rows[i]["CardNo"].ToString()))
                    {
                        blnResult = false;
                        arrayErrorMsg.Add(string.Format(Resources.JobResource.Job0109002, i + 1));
                    }
                }
            }


        }
        //*檢核失敗
        else
        {
            blnResult = false;
        }
        return blnResult;
    }
    #endregion

    #region 資料匯入
    /// <summary>
    /// 功能說明:資料匯入
    /// 作    者:Linda
    /// 創建時間:2010/07/06
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strFileName"></param>
    /// <returns></returns>
    public bool ImportData(DataTable dtDetail, string strFileName)
    {
        bool blnResult = true;
        string sFileName = strFileName.Substring(0, 4);
        string sExpDate = "";
        //20190503 (U) by Nash 就算OU檔案是空檔，也會寫入系統中讓User可以知道資料筆數為0
        if (dtDetail.Rows.Count > 0)
        {
            EntitySet<Entity_CancelOASA_Source> SetCancelOASASource = new EntitySet<Entity_CancelOASA_Source>();
            for (int i = 0; i < dtDetail.Rows.Count; i++)
            {
                Entity_CancelOASA_Source CancelOASASource = new Entity_CancelOASA_Source();
                CancelOASASource.CardNo = dtDetail.Rows[i]["CardNo"].ToString().Trim();  //卡號
                CancelOASASource.ImportFile = strFileName.Trim();
                CancelOASASource.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");
                CancelOASASource.Stauts = "0";
                if (sFileName.Equals("ou09") || sFileName.Equals("ou19") || sFileName.Equals("ou20") || sFileName.Equals("ou13"))
                {
                    CancelOASASource.ExpDate = DateTime.Now.AddMonths(3).ToString("MMyy");
                }
                else
                {
                    sExpDate = dtDetail.Rows[i]["ExpDate"].ToString().Trim();//到期日
                    //防止噴例外
                    if (sExpDate.Length >= 4)
                    {
                        CancelOASASource.ExpDate = sExpDate.Substring(2, 2) + sExpDate.Substring(0, 2);
                    }
                }
                //Talas 修改先查詢是否當日已匯入，若是，則不重新匯入
                //if (BR_FileInfo.isExist(CancelOASASource.ImportFile, CancelOASASource.ImportDate, CancelOASASource.CardNo))
                //{
                //    JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】3. 註銷檔 : 【" + strFileName + "】 部分資料已存在，不重新匯入！ 資料行數 " + (i + 1).ToString());
                //}
                //else
                //{
                SetCancelOASASource.Add(CancelOASASource);
                //}
            }
            string strMsgID = string.Empty;
            blnResult = BRM_CancelOASASource.BatInsert(SetCancelOASASource, ref strMsgID);
        }
        else
        {
            //需要調整，寫入前先查詢是否有資料，有則更新，無則新增  由原有方法切離
            //EntitySet<Entity_CancelOASA> SetEmptyCancelOASA = new EntitySet<Entity_CancelOASA>();
            Entity_CancelOASA CancelOASA = new Entity_CancelOASA();
            CancelOASA.CancelOASAFile = strFileName;
            CancelOASA.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
            CancelOASA.CancelOASAUser = "sys";
            CancelOASA.TotalCount = 0;
            CancelOASA.SCount = 0;
            CancelOASA.FCount = 0;
            CancelOASA.CancelOASASource = "0";
            // SetEmptyCancelOASA.Add(CancelOASA);   
            //  blnResult = BRM_CancelOASA.BatInsert(SetEmptyCancelOASA);
            blnResult = BR_CancelOASA.InsertCancelOASA(CancelOASA);
        }
        return blnResult;
    }
    #endregion

    #region 新增主機OASA資料
    /// <summary>
    /// 功能說明:新增主機OASA資料
    /// 作    者:Linda
    /// 創建時間:2010/07/07
    /// 修改記錄:
    /// <param name="htReturn">主機傳回資料</param>
    /// <param name="dtblUpdateData">更改的主機欄位信息的DataTable</param>
    /// <param name="strDesp">異動BLK CODE欄位名稱</param>
    /// <returns>true成功，false失敗</returns>
    private bool HtgOASAAdd(string strSource, string strCardNo, string strExpDate, string strBlkCode, string strMemo, string strReasonCode, string strActionCode, ref string strErrorMsg, ref string strBLCLog, ref string strUserId, Quartz.JobExecutionContext context)
    {
        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料

        //string strPurgeDate = DateTime.Now.AddMonths(3).ToString("MMdd");
        //string strPurgeDate =

        htInput.Add("sessionId", strSessionId);

        htInput.Add("FUNCTION_CODE", "A");
        htInput.Add("SOURCE_CODE", strSource);//*交易來源別
        htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
        htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
        htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]

        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("OASA_BLOCK_CODE", strBlkCode);//*BLK CODE
        htInput.Add("OASA_MEMO", strMemo);//*MEMO
        htInput.Add("OASA_REASON_CODE", strReasonCode);//*REASON CODE
        htInput.Add("OASA_ACTION_CODE", strActionCode);//*ACTION CODE

        htInput.Add("OASA_PURGE_DATE", strExpDate);//*PURGE DATE

        //*提交OASA_P4_Submit主機資料

        Hashtable htResultA = MainFrameInfoOASA.GetMainFrameInfo(MainFrameInfoOASA.HtgType.P4_JCAX, htInput, false, "100", GetAgentInfo(context), strJobId);
        if (!htResultA.Contains("HtgMsg"))
        {
            strErrorMsg = "";//*主機返回成功訊息
            strBLCLog = "";
            strUserId = htResultA["USER_ID"].ToString().Trim();
            strSessionId = htResultA["sessionId"].ToString().Trim();
            return true;
        }
        else
        {
            strErrorMsg = htResultA["HtgMsg"].ToString().Trim();
            strSessionId = "";
            if (htResultA.Count > 2)
            {
                strBLCLog = htResultA["OASA_BLOCK_CODE"].ToString().Trim();
                strUserId = htResultA["USER_ID"].ToString().Trim();
                strSessionId = htResultA["sessionId"].ToString().Trim();
            }
            return false;
        }

    }
    #endregion

    /// <summary>
    /// 得到登陸主機信息
    /// </summary>
    /// <returns>EntityAGENT_INFO</returns>
    private EntityAGENT_INFO GetAgentInfo(Quartz.JobExecutionContext context)
    {
        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        if (jobDataMap != null && jobDataMap.Count > 0)
        {
            eAgentInfo.agent_id = jobDataMap.GetString("userId");
            eAgentInfo.agent_pwd = jobDataMap.GetString("passWord");
            eAgentInfo.agent_id_racf = jobDataMap.GetString("racfId");
            eAgentInfo.agent_id_racf_pwd = jobDataMap.GetString("racfPassWord");
        }
        return eAgentInfo;
    }



    #region 解壓ARJ壓縮
    /// <summary>
    /// 功能說明:解壓文件
    /// 作    者:Linda
    /// 創建時間:2010/09/13
    /// 修改記錄:
    /// </summary>
    /// <param name="destFolder">解壓文件夾路徑</param>
    /// <param name="srcZipFile">需要解壓的文件名</param>
    /// <param name="password">解壓密碼</param>
    public bool ExeFile(string destFolder, string srcZipFile, string password)
    {
        string strTXTFileName = string.Empty;
        string strExeFileName = srcZipFile.Substring(0, srcZipFile.Trim().Length - 4);

        strTXTFileName = srcZipFile.Replace(".EXE", "");


        System.Diagnostics.Process p = new System.Diagnostics.Process();
        //设定程序名
        p.StartInfo.FileName = "cmd.exe";
        //关闭Shell的使用
        p.StartInfo.UseShellExecute = false;
        //重定向标准输入
        p.StartInfo.RedirectStandardInput = true;
        //重定向标准输出
        p.StartInfo.RedirectStandardOutput = true;
        //设置不显示窗口
        p.StartInfo.CreateNoWindow = true;
        //执行VER命令
        p.Start();
        string strCommand1 = " " + destFolder + srcZipFile + " -g" + password + " -y " + destFolder;
        p.StandardInput.WriteLine(strCommand1);
        string strCommand2 = " ren " + destFolder + strExeFileName + " " + strTXTFileName;
        p.StandardInput.WriteLine(strCommand2);
        p.StandardInput.WriteLine("exit");
        p.WaitForExit(3000);
        p.Close();
        if (File.Exists(destFolder + strTXTFileName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion


    /// <summary>
    /// 功能說明:mail警訊通知
    /// 作    者:Linda
    /// 創建時間:2010/06/11
    /// 修改記錄:
    /// </summary>
    /// <param name="strCallType">Mail警訊種類</param>
    /// <param name="strCallType">Mail警訊內文</param>
    /// <param name="strCallType">錯誤狀況</param>
    /// <param name="lastTimeDt">最後一次的查詢結果</param>
    public void SendMail(string strCallType, ArrayList alMailInfo, string strErrorName,DataTable lastTimeDt = null)
    {
        try
        {

            DataTable dtCallMail = new DataTable();

            string strMsgID = string.Empty;

            SqlHelper sqlhelp = new SqlHelper();

            sqlhelp.AddCondition(EntityM_CallMail.M_JobID, Operator.Equal, DataTypeUtils.String, strJobId);

            sqlhelp.AddCondition(EntityM_CallMail.M_ConditionID, Operator.Equal, DataTypeUtils.String, strCallType);

            BRM_CallMail.SearchMailByNo(sqlhelp.GetFilterCondition(), ref dtCallMail, ref strMsgID);

            if (null != dtCallMail && dtCallMail.Rows.Count > 0)
            {

                string strDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                string strFrom = UtilHelper.GetAppSettings("MailSender");

                string[] strTo = new string[] { };

                string[] strCc = new string[] { };

                string strSubject = string.Empty;

                string strBody = string.Empty;


                switch (strCallType)
                {

                    case "1":

                        strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');

                        strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

                        //格式化Mail Tittle

                        strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                        strSubject = string.Format(strSubject, Resources.JobResource.Job0000109, alMailInfo[0]);

                        //格式化Mail Body

                        strBody = dtCallMail.Rows[0]["MailContext"].ToString();

                        strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000109, strErrorName);

                        JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);

                        break;

                    case "2":

                        strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');

                        strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

                        //格式化Mail Tittle

                        strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                        strSubject = string.Format(strSubject, Resources.JobResource.Job0000109, alMailInfo[0]);

                        //格式化Mail Body

                        strBody = dtCallMail.Rows[0]["MailContext"].ToString();

                        strBody = string.Format(strBody, strDateTime, alMailInfo[0], alMailInfo[1]);

                        JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);

                        break;
                    case "5":
                        strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                        strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

                        //格式化Mail Tittle
                        strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 ToUsers: " + dtCallMail.Rows[0]["ToUsers"].ToString() + "！", LogState.Info);
                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 CcUsers: " + dtCallMail.Rows[0]["CcUsers"].ToString() + "！", LogState.Info);

                        JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】 開始執行寄信..." + "！", LogState.Info);
                        if (lastTimeDt != null)
                        {
                            //格式化Mail Body
                            for (int i = 0; i < lastTimeDt.Rows.Count; i++)
                            {
                                if (i == 0)
                                {
                                    strBody += "<table style=\"border: 1px solid #000000; border-collapse: collapse;\"><tbody>";
                                    // strBody += string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), "NO", "檔案來源", "卡號", "BLOCK CODE", "備註(訊息說明)", "註銷狀態");
                                    strBody += string.Format(("<tr>" +
                                                              "<td style =\"width: 40px;  text-align: center;border: 1px solid #000000;\">{0}</td>" +
                                                              "<td style =\"width: 100px; text-align: center;border: 1px solid #000000;\">{1}</td>" +
                                                              "<td style =\"width: 170px; text-align: center;border: 1px solid #000000;\">{2}</td>" +
                                                              "<td style =\"width: 100px; text-align: center;border: 1px solid #000000;\">{3}</td>" +
                                                              "<td style =\"width: 400px; text-align: center;border: 1px solid #000000;\">{4}</td>" +
                                                              "<td style =\"width: 100px; text-align: center;border: 1px solid #000000;\">{5}</td>" +
                                                              "</tr>"), "NO", "檔案來源", "卡號", "BLOCK CODE", "備註(訊息說明)", "註銷狀態");
                                }

                                strBody += string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), 
                                        lastTimeDt.Rows[i]["no"],
                                        lastTimeDt.Rows[i]["CancelOASAFile"],
                                        lastTimeDt.Rows[i]["CardNo"], 
                                        lastTimeDt.Rows[i]["BlockCode"], 
                                        lastTimeDt.Rows[i]["MemoLog"], 
                                        lastTimeDt.Rows[i]["SFFLG"]);

                                if (i == lastTimeDt.Rows.Count - 1)
                                {
                                    strBody += "</table></tbody>";
                                }
                            }

                            //寄送
                            bool mailStatus = JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                            if (mailStatus)
                            {
                                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】寄信成功！", LogState.Info);
                            }
                            else
                            {
                                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】寄信失敗！", LogState.Info);
                            }
                        }
                        else
                        {
                            //寄送
                            bool mailStatus = JobHelper.SendMail(strTo, strCc, strFrom, strSubject, "今日查無資料。");
                            if (mailStatus)
                            {
                                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】寄信成功！", LogState.Info);
                            }
                            else
                            {
                                JobHelper.Write(strJobId, DateTime.Now.ToString() + " JOB : 【" + strJobId + "】寄信失敗！", LogState.Info);
                            }
                        }

                        break;

                    default:
                        //發送Mail
                        JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                        break;

                }

            }
        }
        catch (Exception ex)
        {
            BRM_LBatchLog.SaveLog(ex);
        }
    }
    /// <summary>
    /// 取得下載目錄內清單製成字典    JOBID改參數傳入，未來其他JOB有需要亦可以移植
    /// </summary>
    /// <param name="sPath"></param>
    /// <returns></returns>
    public Dictionary<string, string> GetFileDic()
    {
        string sPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId;
        if (!Directory.Exists(sPath))
        {
            return new Dictionary<string, string>();
        }
        Dictionary<string, string> rDC = new Dictionary<string, string>();
        //只抓啟動基準日是否已下載 改為全部
        // string searchPattenName = strJobId + CurrentJobDate.ToString("yyyyMMdd") + "*";
        string[] DCColl = Directory.GetDirectories(sPath, "*", SearchOption.AllDirectories);
        foreach (string sitem in DCColl)
        {
            string[] FileColl = Directory.GetFiles(sitem);
            foreach (string fitem in FileColl)
            {
                string cFileName = Path.GetFileName(fitem);
                if (!rDC.ContainsKey(cFileName)) //檔名
                {
                    rDC.Add(cFileName, fitem);
                }
            }
        }
        return rDC;
    }
    /// <summary>
    /// 預留方法，若以存在NA，本次NA，則不更新
    /// 若以存在NA，但本次非NA，則以實際數值更新
    /// </summary>
    /// <returns></returns>
    public bool SetFileDownInfo()
    {
        return false;
    }

}

