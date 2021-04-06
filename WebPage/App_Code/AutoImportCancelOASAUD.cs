//******************************************************************
//*  功能說明：自動化卡片管制解管
//*  作    者：linda
//*  創建日期：2010/07/12
//*  修改記錄：2021/04/06 新增.TXT處理 陳永銘
//*<author>            <time>            <TaskID>            <desc>
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

/// <summary>
/// AutoImportCancelOASAUD 的摘要描述
/// </summary>
public class AutoImportCancelOASAUD : Quartz.IJob
{

    #region job基本參數設置
    protected string strFunctionKey = "06";
    private string strSessionId = "";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected int MCount;
    protected int TOASACount1;
    protected int SOASACount1;
    protected int FOASACount1;
    protected int TOASACount2;
    protected int SOASACount2;
    protected int FOASACount2;
    protected int intExist;
    protected string strFolderName;
    protected string strLocalPath;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;
    private string strMail;
    protected string strJobId;

    private DateTime _jobDate = DateTime.Now;
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
        string strFileDate = "";
        string strFileName = "";
        string strPath = "";
        string strFunctionName = "";
        string strMsgID = "";
        string strFileInfo = "";
        string strImportDate = "";
        string strFtpFileInfo = "";
        string strFtpIp = "";
        string strFtpUserName = "";
        string strFtpPwd = "";

        try
        {
            #region 获取jobID

            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            strMail = jobDataMap.GetString("mail").Trim();
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;

            //strJobId = "0108";
            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);

            #region 判斷是否手動啟動排程
            if (context.JobDetail.JobDataMap["param"] != null)
            {
                if (!string.IsNullOrWhiteSpace(context.JobDetail.JobDataMap["param"].ToString()))
                {
                    string strParam = context.JobDetail.JobDataMap["param"].ToString();
                    string[] arrStrParam = strParam.Split(',');
                    if (arrStrParam.Length == 2)
                    {
                        DateTime tempDt;
                        if (!string.IsNullOrWhiteSpace(arrStrParam[0]) && DateTime.TryParse(arrStrParam[0], out tempDt))
                        {
                            _jobDate = DateTime.Parse(arrStrParam[0]);
                            JobHelper.SaveLog(strJobId + ",檢核參數成功,設定參數:" + strParam, LogState.Info);
                        }
                        else
                        {
                            JobHelper.SaveLog(strJobId + ",檢核參數異常,設定參數:" + strParam, LogState.Info);
                            return;
                        }
                    }
                    else
                    {
                        JobHelper.SaveLog(strJobId + ",檢核參數異常,設定參數:" + strParam, LogState.Info);
                        return;
                    }
                }
            }
            #endregion


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

            #region 匯入資料明細
            dtLocalFile = new DataTable();
            dtLocalFile.Columns.Add("LocalFilePath");    //本地全路徑
            dtLocalFile.Columns.Add("FtpFilePath");      //Ftp全路徑
            dtLocalFile.Columns.Add("FolderName");       //目錄名稱
            dtLocalFile.Columns.Add("ZipFileName");      //壓縮檔檔名
            dtLocalFile.Columns.Add("FileName");         //檔名
            dtLocalFile.Columns.Add("FileDate");         //檔案日期
            dtLocalFile.Columns.Add("ZipPwd");           //壓縮密碼
            dtLocalFile.Columns.Add("ZipStates");        //壓縮狀態
            dtLocalFile.Columns.Add("CheckStates");      //數據格式驗證狀態
            dtLocalFile.Columns.Add("ImportStates");     //資料匯入狀態

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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000108, "IM");

            #region 登陸ftp下載注銷檔

            dtFileInfo = new DataTable();
            intExist = 0;
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                if (dtFileInfo.Rows.Count > 0)
                {
                    string strMsg = string.Empty;

                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        strFileInfo = string.Empty;
                        strImportDate = rowFileInfo["ImportDate"].ToString().Trim();
                        strFileDate = string.Empty;
                        //FTP 檔名
                        if (!strImportDate.Equals(string.Empty))
                        {
                            strFileDate = strImportDate;

                        }
                        else
                        {
                            if (rowFileInfo["FtpFileName"].ToString().Trim().Substring(0, 4).Equals("os66"))
                            {
                                strFileDate = _jobDate.ToString("yyyy/MM/dd");
                            }
                            else
                            {
                                strFileDate = _jobDate.AddDays(-1).ToString("yyyy/MM/dd");
                            }
                        }
                        switch (rowFileInfo["FtpFileName"].ToString().Trim().Substring(0, 4))
                        {
                            case "OS56":
                                //2021/04/06 新增.TXT處理 陳永銘
                                strFileInfo = rowFileInfo["FtpFileName"].ToString() + strFileDate.Replace("/", "").Substring(4, 4);
                                break;
                            case "os55":
                                //2021/04/06 新增.TXT處理 陳永銘
                                strFileInfo = rowFileInfo["FtpFileName"].ToString() + strFileDate.Replace("/", "").Substring(4, 4);
                                break;
                            case "os66":
                                //2021/04/06 新增.TXT處理 陳永銘
                                strFileInfo = rowFileInfo["FtpFileName"].ToString() + strFileDate.Replace("/", "").Substring(4, 4);
                                break;
                        }

                        //FTP 路徑+檔名
                        strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;
                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();

                        FTPFactory objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                        //*檔案存在
                        if (objFtp.isInFolderList(strFtpFileInfo))
                        {
                            //*下載檔案
                            JobHelper.SaveLog("開始下載檔案！", LogState.Info);
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath + strFileInfo; //本地路徑
                                row["FtpFilePath"] = strFtpFileInfo; //FTP路徑
                                row["FolderName"] = strLocalPath; //本地資料夾
                                row["ZipFileName"] = strFileInfo; //注銷檔名稱
                                row["FileDate"] = strFileDate; //檔案日期
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //FTP壓縮檔密碼解密
                                //row["ZipPwd"] = rowFileInfo["ZipPwd"].ToString(); //FTP壓縮檔密碼解密
                                dtLocalFile.Rows.Add(row);
                                intExist = intExist + 1;
                                JobHelper.SaveLog("下載檔案成功！", LogState.Info);
                            }
                        }
                        //*檔案不存在
                        else
                        {
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0108001, rowFileInfo["FtpFileName"].ToString()));
                        }
                    }
                    if (intExist == 0)
                    {
                        BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
                        BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "FTP上本次沒有需要注銷的檔案");
                        return;
                    }
                }
            }
            else
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
            }
            #endregion

            #region 處理本地壓縮檔
            String errMsg = "";
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                string strZipFileName = rowLocalFile["ZipFileName"].ToString().Trim();
                if (strZipFileName.Substring(0, 4) != "OS56")
                {
                    //2021/04/06 新增.TXT處理 陳永銘
                    if (rowLocalFile["ZipPwd"].ToString() == "")
                    {
                        continue;
                    }

                    bool blnResult = ExeFile(strLocalPath, strZipFileName, rowLocalFile["ZipPwd"].ToString());
                    ////*解壓成功
                    if (blnResult)
                    {
                        JobHelper.SaveLog("解壓縮檔案成功！", LogState.Info);
                        rowLocalFile["ZipStates"] = "S";
                        if (strZipFileName.Substring(0, 4).Equals("os66"))
                        {
                            rowLocalFile["FileName"] = strZipFileName.Replace(".EXE", ".txt");
                        }
                        else
                        {
                            rowLocalFile["FileName"] = strZipFileName.Replace(".EXE", ".txt");
                        }
                        JobHelper.SaveLog("解壓縮檔案成功！", LogState.Info);
                    }
                    //*解壓失敗
                    else
                    {
                        errMsg += (errMsg == "" ? "" : "、") + strZipFileName;
                        rowLocalFile["ZipStates"] = "F";
                        // ArrayList alInfo = new ArrayList();
                        // alInfo.Add(strZipFileName);
                        //解壓失敗發送Mail通知
                        // SendMail("1", alInfo, Resources.JobResource.Job0000002);
                        JobHelper.SaveLog("解壓縮檔案失敗！");
                    }
                }
                else
                {
                    rowLocalFile["ZipStates"] = "S";
                    rowLocalFile["FileName"] = strZipFileName;
                    JobHelper.SaveLog("檔案無須解壓縮！");
                }

            }
            if (errMsg != "")
            {
                ArrayList alInfo = new ArrayList();
                alInfo.Add(errMsg);
                //解壓失敗發送Mail通知
                SendMail("1", alInfo, Resources.JobResource.Job0000002);
            }
            #endregion

            #region 開始資料匯入
            DataRow[] Row = dtLocalFile.Select("ZipStates='S'");
            JobHelper.SaveLog("開始資料匯入部分！", LogState.Info);
            if (Row != null && Row.Length > 0)
            {
                //*讀取檔名正確資料
                JobHelper.SaveLog("開始讀取要匯入的檔案資料！", LogState.Info);
                for (int rowcount = 0; rowcount < Row.Length; rowcount++)
                {
                    strFileName = Row[rowcount]["FileName"].ToString().Trim();
                    strFileDate = Row[rowcount]["FileDate"].ToString().Trim();
                    strPath = strLocalPath + Row[rowcount]["FileName"].ToString().Trim();
                    strFunctionName = Resources.JobResource.Job0000108;
                    DataTable dtOASAUDInfo = new DataTable();
                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        JobHelper.SaveLog("本地檔案存在！", LogState.Info);
                        strMsgID = string.Empty;
                        ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                        DataTable dtDetail = null;                 //檢核結果列表

                        JobHelper.SaveLog("開始檢核檔案：" + strFileName, LogState.Info);
                        //*檢核成功
                        if (UploadCheck(strPath, strFunctionName, strFileName.Substring(0, 4), ref strMsgID, ref arrayErrorMsg, ref dtDetail))
                        {
                            JobHelper.SaveLog("檢核檔案成功！", LogState.Info);
                            Row[rowcount]["CheckStates"] = "S";

                            //判斷該檔案是否增經匯入過
                            if (BRM_CancelOASAUd.SearchOASAUDCardInfo(strFileName, strFileDate, ref dtOASAUDInfo))
                            {
                                if (dtOASAUDInfo.Rows.Count > 0) //已匯入過
                                {
                                    //判斷是否已上主機 , 若是 (success_flag<>0) , 則後續不再執行發送電文動作
                                    if (BRM_CancelOASAUd.IS_OASAUDCardInfo_UpToMF(strFileName, strFileDate))
                                    {
                                        //該批檔案已上主機
                                        Row[rowcount]["ImportStates"] = "M";
                                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0108002, strFileName), LogState.Info);
                                    }
                                    else
                                    {
                                        Row[rowcount]["ImportStates"] = "S";
                                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0108002, strFileName), LogState.Info);
                                    }
                                }
                                else
                                {
                                    //*正式匯入
                                    if (ImportData(dtDetail, strFileName, strFileDate))
                                    {
                                        Row[rowcount]["ImportStates"] = "S";
                                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101002, strFileName), LogState.Info);
                                    }
                                    else
                                    {
                                        Row[rowcount]["ImportStates"] = "F";
                                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003, strFileName));
                                    }
                                }
                            }

                        }
                        //*檢核失敗
                        else
                        {
                            JobHelper.SaveLog("檢核檔案失敗！");
                            Row[rowcount]["CheckStates"] = "F";
                            Row[rowcount]["ImportStates"] = "F";

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
            #endregion

            #region OASA注銷
            DataRow[] rowOASA = dtLocalFile.Select("ImportStates='S'");
            string strOASAFileName = "";
            string strOASAFileDate = "";
            string strErrorMsg = "";
            string strUserId = "";
            string strPurgeDate = "";
            string strSFFlg = "";
            DataTable dtOASAInfo = new DataTable();
            TOASACount1 = 0;
            SOASACount1 = 0;
            FOASACount1 = 0;
            TOASACount2 = 0;
            SOASACount2 = 0;
            FOASACount2 = 0;

            //*上傳P4_JCAX修改主機資料
            for (int intOASA = 0; intOASA < rowOASA.Length; intOASA++)
            {
                strOASAFileName = rowOASA[intOASA]["FileName"].ToString().Trim();
                strOASAFileDate = rowOASA[intOASA]["FileDate"].ToString().Trim();

                if (BRM_CancelOASAUd.SearchOASAUDCardInfo(strOASAFileName, strOASAFileDate, ref dtOASAInfo))
                {
                    if (dtOASAInfo.Rows.Count > 0)
                    {

                        foreach (DataRow rowOASAInfo in dtOASAInfo.Rows)
                        {
                            if (rowOASAInfo["NBLKCode"].ToString().Trim() == "X")
                            {
                                if (this.HtgOASADelete(rowOASAInfo["CardNo"].ToString().Trim(), "", rowOASAInfo["MEMO"].ToString().Trim(), rowOASAInfo["Reason_Code"].ToString().Trim(), rowOASAInfo["Action_code"].ToString().Trim(), ref strErrorMsg, ref strUserId, ref strPurgeDate, context))
                                {
                                    strSFFlg = "1";

                                    if (strOASAFileName.Substring(0, 4) == "os66")
                                    {
                                        TOASACount2 = TOASACount2 + 1;
                                        SOASACount2 = SOASACount2 + 1;
                                    }
                                    else
                                    {
                                        TOASACount1 = TOASACount1 + 1;
                                        SOASACount1 = SOASACount1 + 1;
                                    }
                                }
                                else
                                {
                                    strSFFlg = "2";
                                    if (strOASAFileName.Substring(0, 4) == "os66")
                                    {
                                        TOASACount2 = TOASACount2 + 1;
                                        FOASACount2 = FOASACount2 + 1;
                                    }
                                    else
                                    {
                                        TOASACount1 = TOASACount1 + 1;
                                        FOASACount1 = FOASACount1 + 1;
                                    }
                                }
                            }
                            else
                            {
                                if (strOASAFileName.Substring(0, 4) == "os66")//監控補掛只做「Add」  
                                {
                                    if (this.HtgOASAAdd(rowOASAInfo["CardNo"].ToString().Trim(), rowOASAInfo["NBLKCode"].ToString().Trim(), rowOASAInfo["MEMO"].ToString().Trim(), rowOASAInfo["Reason_Code"].ToString().Trim(), rowOASAInfo["Action_code"].ToString().Trim(), ref strErrorMsg, ref strUserId, ref strPurgeDate, context))
                                    {
                                        strSFFlg = "1";
                                        TOASACount2 = TOASACount2 + 1;
                                        SOASACount2 = SOASACount2 + 1;
                                    }
                                    else
                                    {
                                        strSFFlg = "2";
                                        TOASACount2 = TOASACount2 + 1;
                                        FOASACount2 = FOASACount2 + 1;
                                    }
                                }
                                else//管制解管是先「Change」再「Add」，都失敗才算失敗
                                {

                                    if (this.HtgOASAChange(rowOASAInfo["CardNo"].ToString().Trim(), rowOASAInfo["NBLKCode"].ToString().Trim(), rowOASAInfo["MEMO"].ToString().Trim(), rowOASAInfo["Reason_Code"].ToString().Trim(), rowOASAInfo["Action_code"].ToString().Trim(), ref strErrorMsg, ref strUserId, ref strPurgeDate, context))
                                    {
                                        strSFFlg = "1";
                                        TOASACount1 = TOASACount1 + 1;
                                        SOASACount1 = SOASACount1 + 1;
                                    }
                                    else
                                    {
                                        if (this.HtgOASAAdd(rowOASAInfo["CardNo"].ToString().Trim(), rowOASAInfo["NBLKCode"].ToString().Trim(), rowOASAInfo["MEMO"].ToString().Trim(), rowOASAInfo["Reason_Code"].ToString().Trim(), rowOASAInfo["Action_code"].ToString().Trim(), ref strErrorMsg, ref strUserId, ref strPurgeDate, context))
                                        {
                                            strSFFlg = "1";
                                            TOASACount1 = TOASACount1 + 1;
                                            SOASACount1 = SOASACount1 + 1;
                                        }
                                        else
                                        {
                                            strSFFlg = "2";
                                            TOASACount1 = TOASACount1 + 1;
                                            FOASACount1 = FOASACount1 + 1;
                                        }
                                    }
                                }
                            }

                            Entity_CancelOASAUd CancelOASAUd = new Entity_CancelOASAUd();

                            CancelOASAUd.CardNo = rowOASAInfo["CardNo"].ToString().Trim();
                            CancelOASAUd.FileName_Real = strOASAFileName;
                            CancelOASAUd.File_Date = strOASAFileDate;
                            CancelOASAUd.Success_Flag = strSFFlg;
                            CancelOASAUd.Fail_Reason = strErrorMsg;
                            CancelOASAUd.UpdUser = strUserId;
                            CancelOASAUd.PurgeDate = strPurgeDate;
                            CancelOASAUd.Sys_Date = DateTime.Now.ToString("yyyy/MM/dd");

                            SqlHelper sqlhelp = new SqlHelper();
                            sqlhelp.AddCondition(Entity_CancelOASAUd.M_FileName_Real, Operator.Equal, DataTypeUtils.String, CancelOASAUd.FileName_Real);
                            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.Equal, DataTypeUtils.String, CancelOASAUd.File_Date);
                            sqlhelp.AddCondition(Entity_CancelOASAUd.M_CardNo, Operator.Equal, DataTypeUtils.String, CancelOASAUd.CardNo);
                            if (!BRM_CancelOASAUd.Update(CancelOASAUd, sqlhelp.GetFilterCondition(), "Success_Flag", "Fail_Reason", "UpdUser", "PurgeDate", "Sys_Date"))
                            {
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0108003, strOASAFileName, strOASAFileDate, rowOASAInfo["CardNo"].ToString().Trim()));
                            }
                        }
                    }
                }
            }
            MainFrameInfoOASA.ClearHtgSessionJob(ref strSessionId, strJobId);

            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job結束日誌記錄
            //*判斷job完成狀態
            DataRow[] RowS = dtLocalFile.Select("ImportStates='S'");
            DataRow[] RowF = dtLocalFile.Select("ImportStates='F'");
            DataRow[] RowM = dtLocalFile.Select("ImportStates='M'");

            string strJobStatus = "";

            if (RowS != null && RowS.Length > 0)
            {
                SCount = RowS.Length;
            }
            if (RowF != null && RowF.Length > 0)
            {
                FCount = RowF.Length;
            }
            if (RowM != null && RowM.Length > 0)
            {
                MCount = RowM.Length;
            }
            //*判斷job完成狀態
            //string strJobStatus = JobHelper.GetJobStatus(SCount, FCount);
            if (FCount <= 0)
            {
                strJobStatus = "S";
            }
            else
            {
                strJobStatus = "F";
            }
            string strReturnMsg = string.Empty;
            strReturnMsg += Resources.JobResource.Job0000024 + SCount + "　";
            strReturnMsg += Resources.JobResource.Job0000025 + FCount + "　";
            strReturnMsg += Resources.JobResource.Job0000026_1 + MCount + "　";
            if (RowF != null && RowF.Length > 0)
            {
                string strTemps = string.Empty;
                for (int k = 0; k < RowF.Length; k++)
                {
                    strTemps += RowF[k]["FileName"].ToString() + "、";
                }
                strReturnMsg += Resources.JobResource.Job0000026 + strTemps + "　";
            }
            if (RowM != null && RowM.Length > 0)
            {
                string strTemps = string.Empty;
                for (int j = 0; j < RowM.Length; j++)
                {
                    strTemps += RowM[j]["FileName"].ToString() + "、";
                }
                strReturnMsg += Resources.JobResource.Job0000026_2 + strTemps + "　";
            }

            strReturnMsg += "管制解管來源筆數: " + TOASACount1 + "　";
            strReturnMsg += "TOTAL成功:" + SOASACount1 + "　";
            strReturnMsg += "TOTAL失敗:" + FOASACount1 + "　";
            strReturnMsg += "監控補掛來源筆數: " + TOASACount2 + "　";
            strReturnMsg += "TOTAL成功:" + SOASACount2 + "　";
            strReturnMsg += "TOTAL失敗:" + FOASACount2 + "　";

            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
            #endregion


            #region 發送卡流系統-管制解管及監控補掛通知
            ArrayList alInfo8 = new ArrayList();
            alInfo8.Add(TOASACount1);
            alInfo8.Add(SOASACount1);
            alInfo8.Add(FOASACount1);
            alInfo8.Add(TOASACount2);
            alInfo8.Add(SOASACount2);
            alInfo8.Add(FOASACount2);

            if (SCount > 0 || FCount > 0)  //若匯入的總檔案皆屬於 M (今日已執行完畢)，　則不需發送 mail
            {
                SendMail("8", alInfo8, Resources.JobResource.Job0000002);
            }
            #endregion
            JobHelper.SaveLog("JOB結束！", LogState.Info);
        }
        catch (Exception ex)
        {
            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "AutoImportCancelOASAUD_發生錯誤_" + ex.ToString());
            BRM_LBatchLog.SaveLog(ex);
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
            case "os55":
                strUploadID = "06010800";
                break;
            case "OS56":
                strUploadID = "06010801";
                break;
            case "os66":
                strUploadID = "06010802";
                break;
        }

        dtDetail = BaseHelper.UploadCheck(strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, Resources.JobResource.Job0000108, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

        //*檢核成功
        if (strMsgID == "" && arrayErrorMsg.Count == 0)
        {
            blnResult = true;
            if (dtDetail.Rows.Count > 0)
            {
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    if (!ValidateHelper.IsNum(dtDetail.Rows[i]["CardNum"].ToString()))
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
    /// 創建時間:2010/07/13
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strFileName"></param>
    /// <returns></returns>
    public bool ImportData(DataTable dtDetail, string strFileName, string strFileDate)
    {
        bool blnResult = true;
        string strPermitDate = "";
        string strSendDate = "";


        EntitySet<Entity_CancelOASAUd> SetCancelOASAUd = new EntitySet<Entity_CancelOASAUd>();

        for (int i = 0; i < dtDetail.Rows.Count; i++)
        {
            strPermitDate = "";
            strSendDate = "";

            Entity_CancelOASAUd CancelOASAUd = new Entity_CancelOASAUd();

            if (!strFileName.Substring(0, 4).Equals("os66"))
            {
                CancelOASAUd.FileName = dtDetail.Rows[i]["FileName"].ToString().Trim();
                CancelOASAUd.FailFlag = dtDetail.Rows[i]["FailFlag"].ToString().Trim();

                //文件來源類型 0：管制解管主机OS55  1：管制解管GUIOS56 2：管制解管GUIOS56CTCP 3：監控補掛OS66
                if (strFileName.Substring(0, 4).Equals("os55"))
                {
                    CancelOASAUd.FileCode = "0";
                }
                else
                {
                    if (strFileName.Substring(0, 8).Trim().Equals("OS56CTCP"))
                    {
                        CancelOASAUd.FileCode = "2";
                    }
                    else
                    {
                        CancelOASAUd.FileCode = "1";
                    }
                }

            }
            else
            {
                CancelOASAUd.FileCode = "3";
            }

            CancelOASAUd.CardNo = dtDetail.Rows[i]["CardNum"].ToString().Trim();
            CancelOASAUd.CardType = dtDetail.Rows[i]["CardType"].ToString().Trim();
            CancelOASAUd.PurgeDate = dtDetail.Rows[i]["Purge_Date"].ToString().Trim();
            CancelOASAUd.SourceTypeCode = dtDetail.Rows[i]["SourceTypeCode"].ToString().Trim();
            CancelOASAUd.OBLKCode = dtDetail.Rows[i]["OBLKCode"].ToString().Trim();
            CancelOASAUd.NBLKCode = dtDetail.Rows[i]["NBLKCode"].ToString().Trim();
            CancelOASAUd.MEMO = dtDetail.Rows[i]["MEMO"].ToString().Trim();
            CancelOASAUd.Reason_Code = dtDetail.Rows[i]["Reason_Code"].ToString().Trim();
            CancelOASAUd.Action_Code = dtDetail.Rows[i]["Action_code"].ToString().Trim();
            CancelOASAUd.Cwb_Regions = dtDetail.Rows[i]["CWB_Regions"].ToString().Trim();

            strPermitDate = dtDetail.Rows[i]["PermitDate"].ToString().Trim();
            strPermitDate = strPermitDate.Substring(0, 4) + "/" + strPermitDate.Substring(4, 2) + "/" + strPermitDate.Substring(6, 2);
            CancelOASAUd.PermitDate = strPermitDate;

            strSendDate = dtDetail.Rows[i]["SendDate"].ToString().Trim();
            strSendDate = strSendDate.Substring(0, 4) + "/" + strSendDate.Substring(4, 2) + "/" + strSendDate.Substring(6, 2);
            CancelOASAUd.SendDate = strSendDate;

            CancelOASAUd.CardType2 = dtDetail.Rows[i]["CardType2"].ToString().Trim();
            CancelOASAUd.FileName_Real = strFileName;
            CancelOASAUd.File_Date = strFileDate;

            CancelOASAUd.Success_Flag = "0";

            SetCancelOASAUd.Add(CancelOASAUd);
        }
        string strMsgID = string.Empty;
        blnResult = BRM_CancelOASAUd.BatInsert(SetCancelOASAUd, ref strMsgID);
        return blnResult;
    }
    #endregion

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

        if (srcZipFile.Substring(0, 4).Equals("os66"))
        {
            strTXTFileName = srcZipFile.Replace("EXE", "txt");
        }
        else
        {
            strTXTFileName = srcZipFile.Replace("EXE", "txt");
        }

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

    #region 新增主機OASA資料
    /// <summary>
    /// 功能說明:新增主機OASA資料
    /// 作    者:Linda
    /// 創建時間:2010/07/07
    private bool HtgOASAAdd(string strCardNo, string strBlkCode, string strMemo, string strReasonCode, string strActionCode, ref string strErrorMsg, ref string strUserId, ref string strPurgeDate, Quartz.JobExecutionContext context)
    {
        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料

        string strPurgeDateReq = _jobDate.AddMonths(3).ToString("MMdd");

        htInput.Add("sessionId", strSessionId);

        htInput.Add("FUNCTION_CODE", "A");
        htInput.Add("SOURCE_CODE", "Z");//*交易來源別
        htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
        htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
        htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]

        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("OASA_BLOCK_CODE", strBlkCode);//*BLK CODE
        htInput.Add("OASA_MEMO", strMemo);//*MEMO
        htInput.Add("OASA_REASON_CODE", strReasonCode);//*REASON CODE
        htInput.Add("OASA_ACTION_CODE", strActionCode);//*ACTION CODE

        htInput.Add("OASA_PURGE_DATE", strPurgeDateReq);//*PURGE DATE

        //*提交OASA_P4_Submit主機資料

        Hashtable htResultA = MainFrameInfoOASA.GetMainFrameInfo(MainFrameInfoOASA.HtgType.P4_JCAX, htInput, false, "100", GetAgentInfo(context), strJobId);
        if (!htResultA.Contains("HtgMsg"))
        {
            strErrorMsg = "";//*主機返回成功訊息
            strUserId = htResultA["USER_ID"].ToString().Trim();
            strPurgeDate = htResultA["OASA_PURGE_DATE"].ToString().Trim();
            strSessionId = htResultA["sessionId"].ToString().Trim();
            return true;
        }
        else
        {
            strErrorMsg = htResultA["HtgMsg"].ToString().Trim();
            strSessionId = "";
            if (htResultA.Count > 2)
            {
                strUserId = htResultA["USER_ID"].ToString().Trim();
                strPurgeDate = htResultA["OASA_PURGE_DATE"].ToString().Trim();
                strSessionId = htResultA["sessionId"].ToString().Trim();
            }
            return false;
        }

    }
    #endregion

    #region 修改主機OASA資料
    /// <summary>
    /// 功能說明: 修改主機OASA資料
    /// 作    者:Linda
    /// 創建時間:2010/09/01
    private bool HtgOASAChange(string strCardNo, string strBlkCode, string strMemo, string strReasonCode, string strActionCode, ref string strErrorMsg, ref string strUserId, ref string strPurgeDate, Quartz.JobExecutionContext context)
    {
        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料

        string strPurgeDateReq = _jobDate.AddMonths(3).ToString("MMdd");

        htInput.Add("sessionId", strSessionId);

        htInput.Add("FUNCTION_CODE", "C");
        htInput.Add("SOURCE_CODE", "Z");//*交易來源別
        htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
        htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
        htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]

        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("OASA_BLOCK_CODE", strBlkCode);//*BLK CODE
        htInput.Add("OASA_MEMO", strMemo);//*MEMO
        htInput.Add("OASA_REASON_CODE", strReasonCode);//*REASON CODE
        htInput.Add("OASA_ACTION_CODE", strActionCode);//*ACTION CODE

        htInput.Add("OASA_PURGE_DATE", strPurgeDateReq);//*PURGE DATE

        //*提交OASA_P4_Submit主機資料

        Hashtable htResultA = MainFrameInfoOASA.GetMainFrameInfo(MainFrameInfoOASA.HtgType.P4_JCAX, htInput, false, "100", GetAgentInfo(context), strJobId);
        if (!htResultA.Contains("HtgMsg"))
        {
            strErrorMsg = "";//*主機返回成功訊息
            strUserId = htResultA["USER_ID"].ToString().Trim();
            strPurgeDate = htResultA["OASA_PURGE_DATE"].ToString().Trim();
            strSessionId = htResultA["sessionId"].ToString().Trim();
            return true;
        }
        else
        {
            strErrorMsg = htResultA["HtgMsg"].ToString().Trim();
            strSessionId = "";
            if (htResultA.Count > 2)
            {
                strUserId = htResultA["USER_ID"].ToString().Trim();
                strPurgeDate = htResultA["OASA_PURGE_DATE"].ToString().Trim();
                strSessionId = htResultA["sessionId"].ToString().Trim();
            }
            return false;
        }

    }
    #endregion

    #region 刪除主機OASA資料
    /// <summary>
    /// 功能說明: 刪除主機OASA資料
    /// 作    者:Linda
    /// 創建時間:2010/09/17
    private bool HtgOASADelete(string strCardNo, string strBlkCode, string strMemo, string strReasonCode, string strActionCode, ref string strErrorMsg, ref string strUserId, ref string strPurgeDate, Quartz.JobExecutionContext context)
    {
        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料

        string strPurgeDateReq = _jobDate.AddMonths(3).ToString("MMdd");

        htInput.Add("sessionId", strSessionId);

        htInput.Add("FUNCTION_CODE", "D");
        htInput.Add("SOURCE_CODE", "Z");//*交易來源別
        htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
        htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
        htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]

        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("OASA_BLOCK_CODE", strBlkCode);//*BLK CODE
        htInput.Add("OASA_MEMO", strMemo);//*MEMO
        htInput.Add("OASA_REASON_CODE", strReasonCode);//*REASON CODE
        htInput.Add("OASA_ACTION_CODE", strActionCode);//*ACTION CODE

        htInput.Add("OASA_PURGE_DATE", strPurgeDateReq);//*PURGE DATE

        //*提交OASA_P4_Submit主機資料

        Hashtable htResultA = MainFrameInfoOASA.GetMainFrameInfo(MainFrameInfoOASA.HtgType.P4_JCAX, htInput, false, "100", GetAgentInfo(context), strJobId);
        if (!htResultA.Contains("HtgMsg"))
        {
            strErrorMsg = "";//*主機返回成功訊息
            strUserId = htResultA["USER_ID"].ToString().Trim();
            strPurgeDate = htResultA["OASA_PURGE_DATE"].ToString().Trim();
            strSessionId = htResultA["sessionId"].ToString().Trim();
            return true;
        }
        else
        {
            strErrorMsg = htResultA["HtgMsg"].ToString().Trim();
            strSessionId = "";
            if (htResultA.Count > 2)
            {
                strUserId = htResultA["USER_ID"].ToString().Trim();
                strPurgeDate = htResultA["OASA_PURGE_DATE"].ToString().Trim();
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
    /// <summary>

    /// 功能說明:mail警訊通知

    /// 作    者:Linda

    /// 創建時間:2010/06/11

    /// 修改記錄:

    /// </summary>

    /// <param name="strCallType">Mail警訊種類</param>

    /// <param name="strCallType">Mail警訊內文</param>

    /// <param name="strCallType">錯誤狀況</param>

    public void SendMail(string strCallType, ArrayList alMailInfo, string strErrorName)
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

            string strSubject = string.Empty;

            string strBody = string.Empty;

            //string[] strTo = new string[] { };
            string[] strTo = strMail.Split(';');

            string[] strCc = new string[] { };


            //strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
            strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

            switch (strCallType)
            {

                case "1":
                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000108, alMailInfo[0]);

                    //格式化Mail Body

                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();

                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000108, strErrorName);

                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);

                    break;

                case "2":

                    //格式化Mail Tittle

                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000108, alMailInfo[0]);

                    //格式化Mail Body

                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();

                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], alMailInfo[1]);

                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);

                    break;

                case "8":

                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                    //格式化Mail Body

                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();

                    strBody = string.Format(strBody, alMailInfo[0], alMailInfo[1], alMailInfo[2], alMailInfo[3], alMailInfo[4], alMailInfo[5]);

                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);

                    break;

                default:

                    //發送Mail

                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);

                    break;

            }

        }

    }
}
