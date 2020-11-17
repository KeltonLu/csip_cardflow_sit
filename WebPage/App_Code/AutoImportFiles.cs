//******************************************************************
//*  功能說明：0101自動化大宗檔匯入
//*  作    者：HAO CHEN
//*  創建日期：2010/06/03
//*  修改記錄：2013/07/08  莊詔翔
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using Framework.Common.Logging;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Common.Utility;
using System.Text;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM;

/// <summary>
/// AutoImportFiles 的摘要描述
/// </summary>
//public class AutoImportFiles 
public class AutoImportFiles : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected int FFileCount;
    protected string strLocalPath = string.Empty;

    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間
    protected DateTime EndTime;
    protected DataRow[] drError;
    protected DataRow[] other;
    protected StringBuilder sbErrorFiles = new StringBuilder();
    protected string strTemps = string.Empty;
    protected string strPath = string.Empty;

    protected string strFtpIp = string.Empty;
    protected string strFtpUserName = string.Empty;
    protected string strFtpPwd = string.Empty;
    protected DateTime UploadTime;
    protected string strCardType = string.Empty;

    protected FTPFactory objFtp;

    protected DateTime jobDate = DateTime.Now;


    protected Boolean isReRun = false;
    protected DateTime inDate = new DateTime();
    protected DateTime tranDate = new DateTime();
    protected DateTime fileDate = new DateTime();
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/14
    /// 修改記錄:2020/11/09_Ares_Stanley-調整Log內容
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    //public void Execute()
    {
        try
        {
            string strTmp = string.Empty;
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            //strJobId = "0101";
            string strTemp = Resources.JobResource.Job0101001;
            Boolean haveFiles = false;

            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);

            #region 判斷是否手動啟動排程
            if (context.JobDetail.JobDataMap["param"] != null)
            {
                if (!string.IsNullOrWhiteSpace(context.JobDetail.JobDataMap["param"].ToString()))
                {
                    string strParam = context.JobDetail.JobDataMap["param"].ToString();
                    string[] arrStrParam = strParam.Split(',');

                    Boolean reRunErr = false;
                    if (arrStrParam.Length == 4)
                    {
                        DateTime tempDt;
                        if (!string.IsNullOrWhiteSpace(arrStrParam[0]) && DateTime.TryParse(arrStrParam[0], out tempDt))
                        {
                            //檔案日
                            fileDate = DateTime.Parse(arrStrParam[0]);
                            JobHelper.SaveLog(strJobId + ",檢核參數成功,設定檔案日參數:" + arrStrParam[0], LogState.Info);
                        }
                        else
                            reRunErr = true;

                        if (!string.IsNullOrWhiteSpace(arrStrParam[1]) && DateTime.TryParse(arrStrParam[1], out tempDt))
                        {
                            //轉檔日
                            inDate = DateTime.Parse(arrStrParam[1]);
                            JobHelper.SaveLog(strJobId + ",檢核參數成功,設定轉檔日參數:" + arrStrParam[1], LogState.Info);
                        }
                        else
                            reRunErr = true;

                        if (!string.IsNullOrWhiteSpace(arrStrParam[2]) && DateTime.TryParse(arrStrParam[2], out tempDt))
                        {
                            //製卡日
                            tranDate = DateTime.Parse(arrStrParam[2]);
                            JobHelper.SaveLog(strJobId + ",檢核參數成功,設定製卡日參數:" + arrStrParam[2], LogState.Info);
                        }
                        else
                            reRunErr = true;
                    }
                    else
                    {
                        reRunErr = true;
                    }

                    if (reRunErr)
                    {
                        JobHelper.SaveLog(strJobId + ",檢核參數異常,設定參數:" + strParam, LogState.Info);
                        BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "ReRun參數異常");
                        return;
                    }
                    else
                    {
                        isReRun = true;
                    }
                }
            }
            #endregion

            #region 計數器歸零
            SCount = 0;
            FCount = 0;
            #endregion

            #region 匯入資料明細
            dtLocalFile = new DataTable();
            dtLocalFile.Columns.Add("LocalFilePath");      //本地全路徑
            dtLocalFile.Columns.Add("FtpFilePath");        //Ftp全路徑
            dtLocalFile.Columns.Add("FolderName");         //目錄名稱
            dtLocalFile.Columns.Add("ZipFileName");        //資料檔名
            dtLocalFile.Columns.Add("TxtFileName");        //資料檔名
            dtLocalFile.Columns.Add("CardType");           //卡片種類
            dtLocalFile.Columns.Add("MerchCode");          //製卡廠Code
            dtLocalFile.Columns.Add("MerchName");          //製卡廠名稱
            dtLocalFile.Columns.Add("Trandate");           //轉檔日設定
            dtLocalFile.Columns.Add("ZipStates");          //解壓狀態
            dtLocalFile.Columns.Add("ZipPwd");             //解壓縮密碼
            dtLocalFile.Columns.Add("FormatStates");       //格式判斷狀態
            dtLocalFile.Columns.Add("CheckStates");        //數據格式驗證狀態
            dtLocalFile.Columns.Add("ImportStates");       //資料匯入狀態
            #endregion

            #region 錯誤資料
            DataTable dtErr = new DataTable();
            dtErr.Columns.Add("FilePath");      //路徑
            dtErr.Columns.Add("Context");       //錯誤內容
            #endregion

            #region 記錄下載的壓縮檔
            ArrayList Array = new ArrayList();
            ArrayList ArrayPwd = new ArrayList();
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
            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000101, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty; //*本地存放目錄(格式為yyyyMMddHHmmss+JobID)
            JobHelper.CreateFolderName(strJobId, ref strFolderName);

            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                if (dtFileInfo.Rows.Count > 0)
                {
                    //*處理大總檔檔名
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        //本地路徑
                        strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //FTP 檔名
                        string strFileInfo = jobDate.ToString("yyyyMMdd") + rowFileInfo["FtpFileName"].ToString() + ".ZIP";
                        if (isReRun)
                        {
                            strFileInfo = fileDate.ToString("yyyyMMdd") + rowFileInfo["FtpFileName"].ToString() + ".ZIP";
                        }
                        //FTP 路徑+檔名
                        string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;

                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();

                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                        //*檔案存在
                        if (objFtp.isInFolderList(strFtpFileInfo))
                        {
                            JobHelper.SaveLog("開始下載 "+ strFileInfo+ " ！", LogState.Info);
                            //*下載檔案
                            haveFiles = true;
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath; //本地路徑
                                row["FtpFilePath"] = strFtpFileInfo; //FTP路徑
                                row["FolderName"] = strLocalPath; //本地資料夾
                                row["ZipFileName"] = strFileInfo; //FTP壓縮檔名稱
                                row["CardType"] = rowFileInfo["CardType"].ToString(); //卡片種類
                                row["MerchCode"] = rowFileInfo["MerchCode"].ToString(); //製卡廠代碼
                                row["MerchName"] = rowFileInfo["MerchName"].ToString(); //製卡廠名稱
                                row["Trandate"] = GetTrandate(rowFileInfo["AMPMFlg"].ToString()); //轉檔日設定
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //FTP壓縮檔密碼解密
                                dtLocalFile.Rows.Add(row);
                                JobHelper.SaveLog("下載檔案成功！", LogState.Info);
                            }
                        }
                        //*檔案不存在
                        else
                        {
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101001, rowFileInfo["FtpFileName"].ToString()), LogState.Warn); //單獨變更大宗檔不存在的Log層級
                        }
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
                int ZipCount = 0;
                string strFileInfo = rowLocalFile["ZipFileName"].ToString();
                JobHelper.SaveLog("嘗試解壓縮檔案：" + strFileInfo, LogState.Info);
                bool blnResult = JobHelper.ZipExeFile(strLocalPath, strLocalPath + rowLocalFile["ZipFileName"].ToString(), rowLocalFile["ZipPwd"].ToString(), ref ZipCount);
                
                //*解壓成功
                if (blnResult)
                {
                    rowLocalFile["ZipStates"] = "S";
                    rowLocalFile["TxtFileName"] = rowLocalFile["ZipFileName"].ToString().Replace(".ZIP", ".TXT");
                }
                //*解壓失敗
                else
                {
                    errMsg += (errMsg == "" ? "" : "、") + rowLocalFile["ZipFileName"];
                    // ArrayList alInfo = new ArrayList();
                    // alInfo.Add(rowLocalFile["ZipFileName"]);
                    //解壓失敗發送Mail通知
                    // SendMail("1", alInfo, Resources.JobResource.Job0000002);
                    rowLocalFile["ZipStates"] = "F";
                    FFileCount++;
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

            #region txt格式判斷
            //*讀取folder中的txt檔案并判斷格式
            //修改時間:2020/11/05_Ares_Stanley-新增檔名判斷，避免錯誤的FormatStates回寫到正確的資料內; 2020/11/17_Ares_Stanley-修正TXT檢測錯誤，修改沒有TXT檔的LOG
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                string[] FileArray = FileTools.GetFileList(rowLocalFile["FolderName"].ToString());
                int iCount = 0;
                bool txtExists = false;
                foreach (string strTmps in FileArray)
                {
                    if(strTmps.ToString() == rowLocalFile["FolderName"].ToString() + rowLocalFile["TxtFileName"].ToString())
                    {
                        //*txt檔名格式正確
                        if (JobHelper.ValidateTxt(FileArray[iCount]))
                        {
                            rowLocalFile["FormatStates"] = "S";
                            txtExists = true;
                        }
                        //*txt檔名格式錯誤
                        else
                        {
                            rowLocalFile["FormatStates"] = "F";
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101001, rowLocalFile["ZipFileName"].ToString()));
                            continue;
                        }
                    }
                    iCount++;
                }
                //if (FileArray.Length < 0)
                //{
                //    JobHelper.SaveLog(Resources.JobResource.Job0000012);
                //}
                if (txtExists == false)
                {
                    JobHelper.SaveLog(rowLocalFile["ZipFileName"] + " 沒有TXT檔", LogState.Info);
                }
            }
            #endregion

            #region 開始資料匯入
            DataRow[] Row = dtLocalFile.Select("ZipStates='S' and FormatStates='S'");
            JobHelper.SaveLog("開始資料匯入部分！", LogState.Info);
            if (Row != null && Row.Length > 0)
            {
                //*讀取檔名正確資料
                JobHelper.SaveLog("開始讀取要匯入的檔案資料！", LogState.Info);
                for (int rowcount = 0; rowcount < Row.Length; rowcount++)
                {
                    string strFileName = Row[rowcount]["TxtFileName"].ToString();
                    strPath = Row[rowcount]["LocalFilePath"].ToString() + strFileName;
                    strCardType = Row[rowcount]["CardType"].ToString();      //*獲取卡片種類
                    //string strMerchName = Row[rowcount]["MerchName"].ToString();    //*獲取製卡廠名稱

                    string strFunctionName = Resources.JobResource.Job0000101;      //Job功能名稱

                    Entity_CardBaseInfo eCardBaseInfo = new Entity_CardBaseInfo();
                    eCardBaseInfo.trandate = Row[rowcount]["Trandate"].ToString();  //*獲取轉檔日
                    eCardBaseInfo.Merch_Code = Row[rowcount]["MerchCode"].ToString(); //*獲取製卡廠代碼
                    eCardBaseInfo.card_file = strFileName;                          //*匯入檔名
                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        JobHelper.SaveLog("本地 " + strFileName + " 存在！", LogState.Info);
                        int No = 0;                                //*匯入之錯誤編號
                        ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                        DataTable dtDetail = null;                 //檢核結果列表
                        //*檢核資料
                        JobHelper.SaveLog("開始檢核檔案：" + strFileName, LogState.Info);
                        if (UploadCheck(strPath, strFunctionName, strCardType, ref No, ref arrayErrorMsg, ref dtDetail))
                        {
                            JobHelper.SaveLog("檢核 " + strFileName + " 成功！", LogState.Info);
                        }
                        else
                        {
                            JobHelper.SaveLog("檢核" + strFileName + " 失敗！");
                        }

                        drError = dtDetail.Select("action is null");
                        other = dtDetail.Select("action is not null"); // other.Length = current SCount
                        FCount = drError.Length;
                        if (null != drError && drError.Length > 0)
                        {
                            //匯入檢核錯誤的資料至錯誤資料檔
                            JobHelper.SaveLog("開始匯入檢核錯誤的資料至錯誤資料檔！", LogState.Info);
                            for (int iError = 0; iError < drError.Length; iError++)
                            {
                                string strMsgID = string.Empty;
                                Entity_JobErrorInfo JobErrInfo = new Entity_JobErrorInfo();
                                DataRow drErr = dtErr.NewRow();
                                drErr["FilePath"] = Row[rowcount]["LocalFilePath"].ToString().Substring(0, Row[rowcount]["LocalFilePath"].ToString().Length - 1) + "F\\" + strFileName;
                                drErr["Context"] = drError[iError]["Context"].ToString();       //檢核錯誤的資料
                                dtErr.Rows.Add(drErr);
                                JobErrInfo.JobID = strJobId;                                    //JObid
                                JobErrInfo.ImportFileName = strFileName;                        //匯入檔名
                                JobErrInfo.ErrorContext = drError[iError]["Context"].ToString();//檢核錯誤的資料
                                JobErrInfo.ImportTime = DateTime.Now.ToString("yyyy/MM/dd");    //匯入時間
                                JobErrInfo.LocalFilePath = Row[rowcount]["LocalFilePath"].ToString().Substring(0, Row[rowcount]["LocalFilePath"].ToString().Length - 1) + "F\\" + strFileName;                             //檔案路徑
                                JobErrInfo.LoadFlag = "0";                                      //
                                JobErrInfo.Reason = Resources.JobResource.Job0000014;           //檢核失敗的原因
                                BRM_JobErrorInfo.Insert(JobErrInfo, ref strMsgID);
                                dtDetail.Rows.Remove(drError[iError]);
                                if (iError+1 == drError.Length)
                                {
                                    JobHelper.SaveLog(string.Format("檔案 {0} 共有 {1} 筆匯入失敗", strFileName, drError.Length));
                                }
                            }
                            // 每次檢核完後呼叫 SetJobError 把筆數回寫DB
                            int testScount = 0;
                            int testTotal = 0;
                            testScount = other.Length;
                            testTotal = testScount + FCount;
                            SetJobError(testScount, FCount, strCardType);

                            if (sbErrorFiles.Length <= 0)
                            {
                                FFileCount++;
                                sbErrorFiles.Append(Row[rowcount]["ZipFileName"].ToString());
                            }
                            else
                            {
                                FFileCount++;
                                sbErrorFiles.Append(";");
                                sbErrorFiles.Append(Row[rowcount]["ZipFileName"].ToString());
                            }

                            strTemps += Row[rowcount]["TxtFileName"].ToString() + "  ";
                            JobHelper.SaveLog("檢核錯誤的錯誤資料匯入完成！", LogState.Info);
                        }

                        if (null != dtDetail && dtDetail.Rows.Count > 0)
                        {
                            //*匯入檢核正確的資料至卡片基本檔
                            JobHelper.SaveLog("開始匯入檢核正確的資料至卡片基本資料檔！", LogState.Info);
                            if (ImportToDB(dtDetail, strFileName, eCardBaseInfo, strCardType))
                            {
                                Row[rowcount]["ImportStates"] = "S";
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101002, strFileName), LogState.Info);
                            }
                            else
                            {
                                Row[rowcount]["ImportStates"] = "F";
                                //JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003, strFileName));
                                strTemps += string.Format(Resources.JobResource.Job0101003, strFileName) + " ";
                                FFileCount++;
                            }
                            JobHelper.SaveLog("檢核正確的正確資料匯入完成！", LogState.Info);
                        }
                    }
                    //*file不存在local
                    else
                    {
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101004, strPath));
                        FFileCount++;
                    }
                }
                JobHelper.SaveLog("結束讀取要匯入的檔案資料！", LogState.Info);
            }
            #endregion

            #region 檢測是否有資料匯入
            if (haveFiles == false)
            {
                JobHelper.SaveLog("沒有資料可以匯入", LogState.Info);
            }
            #endregion

            #region 成功匯入則刪除ftp上的資料
            DataRow[] RowD = dtLocalFile.Select("ZipStates='S' and FormatStates='S' and ImportStates='S'");

            for (int m = 0; m < RowD.Length; m++)
            {
                objFtp.Delete(RowD[m]["FtpFilePath"].ToString());
                string deleteFileName = RowD[m]["ZipFileName"].ToString();
                JobHelper.SaveLog("刪除FTP上的 " + deleteFileName + " 成功！", LogState.Info);
            }
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job結束日誌記錄
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            DataRow[] RowS = dtLocalFile.Select("ZipStates='S' and FormatStates='S' and ImportStates='S'");
            if (RowS != null && RowS.Length > 0)
            {
                SCount = RowS.Length;
                SCount = dtLocalFile.Rows.Count - FFileCount;
            }

            //*判斷job完成狀態
            
            string strJobStatus = JobHelper.GetJobStatus(SCount, FCount);
            string strReturnMsg = string.Empty;
            strReturnMsg += Resources.JobResource.Job0000024 + SCount;
            strReturnMsg += Resources.JobResource.Job0000025 + FFileCount + "!";
            if (!string.IsNullOrEmpty(strTemps))
            {
                strReturnMsg += Resources.JobResource.Job0000026 + strTemps;
                ArrayList alInfo = new ArrayList();
                alInfo.Add(sbErrorFiles);
                alInfo.Add(SCount);
                alInfo.Add(FCount);
                //內容格式錯誤
                SendMail("2", alInfo, Resources.JobResource.Job0000036);
                JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, "F", strReturnMsg);
                //SetJobError(SCount, FCount, strCardType);
            }
            else
            {
                JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
            }

            //刪除
            //foreach (DataRow dr in RowS)
            //{
            //    if (File.Exists(dr["LocalFilePath"].ToString() + dr["TxtFileName"].ToString()))
            //    {
            //        File.Delete(dr["LocalFilePath"].ToString() + dr["TxtFileName"].ToString());
            //    }
            //}

            //寫錯誤資料
            foreach (DataRow dr in dtErr.Rows)
            {
                if (!Directory.Exists(dr["FilePath"].ToString().Substring(0, dr["FilePath"].ToString().LastIndexOf("\\") + 1)))
                {
                    Directory.CreateDirectory(dr["FilePath"].ToString().Substring(0, dr["FilePath"].ToString().LastIndexOf("\\") + 1));
                }
                if (!File.Exists(dr["FilePath"].ToString()))
                {
                    FileStream fs = new FileStream(dr["FilePath"].ToString(), FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.WriteLine(dr["Context"].ToString());
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    FileStream fs = new FileStream(dr["FilePath"].ToString(), FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.WriteLine(dr["Context"].ToString());
                    sw.Close();
                    fs.Close();
                }
            }
            #endregion

            #region 更新異動檔中無法對應到卡片基本檔的資料為退單處理
            DataTable dtCardDataChange = new DataTable();
            //如果假日~就不要檢查異動資料
            if (BRWORK_DATE.IS_WORKDAY("06", jobDate.ToString("yyyyMMdd")))
            {
                if (BRM_CardDataChange.SearchForCard(ref dtCardDataChange))
                {
                    BRM_CardDataChange.SetCardOutputFlg(dtCardDataChange);
                }
            }
            #endregion
            JobHelper.SaveLog("JOB結束！", LogState.Info);
        }
        catch (Exception ex)
        {
            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "CommonModel_發生錯誤_" + ex.ToString());
            BRM_LBatchLog.SaveLog(ex);
        }

    }
    #endregion

    #region 處理轉檔日
    /// <summary>
    /// 功能說明:根據設定的轉檔日屬性獲取轉檔日期
    /// 作    者:HaoChen
    /// 創建時間:2010/06/07
    /// 修改記錄:
    /// </summary>
    /// <param name="strMessage"></param>
    public string GetTrandate(string strTrandate)
    {
        switch (strTrandate)
        {
            case "A":
                return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", jobDate.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
            case "P":
                if (BRWORK_DATE.IS_WORKDAY("06", jobDate.ToString("yyyyMMdd")))
                {
                    return jobDate.ToString("yyyy/MM/dd");
                }
                else
                {
                    return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", jobDate.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
                }
            default:
                if (BRWORK_DATE.IS_WORKDAY("06", jobDate.ToString("yyyyMMdd")))
                {
                    return jobDate.ToString("yyyy/MM/dd");
                }
                else
                {
                    return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", jobDate.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
                }
        }

    }
    #endregion

    #region mail警訊通知
    /// <summary>
    /// 功能說明:mail警訊通知
    /// 作    者:HAO CHEN
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
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000101, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000101, strErrorName);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
                case "2":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000101, alMailInfo[0]);

                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], alMailInfo[1], alMailInfo[2]);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
            }
        }
    }
    #endregion

    #region 匯入資料檢核
    /// <summary>
    /// 功能說明:匯入資料檢核
    /// 作    者:HAO CHEN
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="strTpye"></param>
    /// <returns></returns>
    public bool UploadCheck(string strPath, string strFileName, string strTpye, ref int No, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
    {
        bool blnResult = true;
        #region base parameter
        string strUserID = "sys";
        string strFunctionKey = "06";
        string strUploadID = string.Empty;
        DateTime dtmThisDate = DateTime.Now;
        UploadTime = DateTime.Parse(dtmThisDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        int intMax = int.MaxValue;
        string strMsgID = string.Empty;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        #endregion

        switch (strTpye)
        {
            case "1": //*信用卡大宗掛號檔解析
                strUploadID = "06010100";
                strFileName = Resources.JobResource.Job000010101;      //Job功能名稱
                dtDetail = UploadCheck(strTpye, strUserID, strFunctionKey, strUploadID,
                               UploadTime, strFileName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);
                break;
            case "2"://*金融卡大宗掛號檔解析
                strUploadID = "06010101";
                strFileName = Resources.JobResource.Job000010102;      //Job功能名稱
                dtDetail = UploadCheck(strTpye, strUserID, strFunctionKey, strUploadID,
                               UploadTime, strFileName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);
                break;
        }

        //*檢核成功
        if (string.IsNullOrEmpty(strMsgID))
        {
            blnResult = true;
        }
        //*檢核失敗
        else
        {
            blnResult = false;
        }
        return blnResult;
    }

    private void SetJobError(int iSCount, int iFCount, string strTpye)
    {
        string strErrMsg = "";

        CSIPCommonModel.EntityLayer.EntityL_UPLOAD EntityUpload = new CSIPCommonModel.EntityLayer.EntityL_UPLOAD();
        EntityUpload.FUNCTION_KEY = "06";
        if (strTpye.Equals("1"))
        {
            EntityUpload.UPLOAD_ID = "06010100";
        }
        else
        {
            EntityUpload.UPLOAD_ID = "06010101";
        }
        EntityUpload.UPLOAD_DATE = UploadTime;
        if (!CSIPCommonModel.BusinessRules.BRL_UPLOAD.IsRepeat(EntityUpload, ref strErrMsg))
        {
            EntityUpload.UPLOAD_STATUS = "N";
            EntityUpload.UPLOAD_TOTAL_COUNT = iSCount + iFCount;
            EntityUpload.UPLOAD_SUC_COUNT = iSCount;
            EntityUpload.UPLOAD_FAIL_COUNT = iFCount;
            EntityUpload.CHANGED_TIME = DateTime.Parse(DateTime.Now.ToString());
            EntityUpload.CHANGED_USER = "sys";
            CSIPCommonModel.BusinessRules.BRL_UPLOAD.Update(EntityUpload, ref strErrMsg);
        }
    }

    /// <summary>
    /// 匯入檢核
    /// </summary>
    /// <param name="strUserID"> 用戶ID</param>
    /// <param name="strFunctionKey">系統權限</param>
    /// <param name="strUploadID"> 匯入作業編號</param>
    /// <param name="dtmThisDate"> 匯入作業時間</param>
    /// <param name="strUploadName"> 匯入作業名稱</param>
    /// <param name="strFilePath">上傳文件地址</param>
    /// <param name="intMax">最大筆數</param>
    /// <param name="arrListMsg">檢核回傳信息</param>
    /// <param name="strMsgID">錯誤信息ID</param>
    /// <param name="dtblBegin">頭筆數數據</param>
    /// <param name="dtblEnd">尾筆數數據</param>
    /// <returns>DataTable</returns>
    public static DataTable UploadCheck(string strCardType, string strUserID, string strFunctionKey, string strUploadID, DateTime dtmThisDate, string strUploadName, string strFilePath, int intMax, ArrayList arrListMsg, ref string strMsgID, DataTable dtblBegin, DataTable dtblEnd)
    {
        EntityL_UPLOAD eLUpload = new EntityL_UPLOAD();

        //* 匯入日志欄位賦值
        eLUpload.CHANGED_USER = strUserID;
        eLUpload.FUNCTION_KEY = strFunctionKey;
        eLUpload.UPLOAD_ID = strUploadID;
        eLUpload.UPLOAD_NAME = strUploadName;
        eLUpload.UPLOAD_DATE = dtmThisDate;
        eLUpload.UPLOAD_STATUS = "Y";
        eLUpload.FILE_NAME = string.Empty;

        EntityL_UPLOAD_DETAIL eLUploadDetail = new EntityL_UPLOAD_DETAIL();

        //* 匯入失敗日志欄位賦值
        eLUploadDetail.FUNCTION_KEY = strFunctionKey;
        eLUploadDetail.UPLOAD_ID = strUploadID;
        eLUploadDetail.UPLOAD_DATE = dtmThisDate;
        eLUploadDetail.FAIL_REC_NO = string.Empty;

        DataTable dtblUpload = new DataTable();

        #region  檔案名稱檢核

        //if (Regex.Match(strFilePath, "[\u4E00-\u9FA5]+").Length > 0)
        //{
        //    strMsgID = "Job0000011";

        //    BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

        //    return dtblUpload;
        //}
        #endregion

        #region  檔案類型檢核

        System.IO.FileInfo file = new System.IO.FileInfo(strFilePath);

        eLUpload.FILE_NAME = file.Name;

        DataTable dtblUploadCheck = null;

        //* 判斷檔案是否存在
        if (!file.Exists)
        {
            strMsgID = Resources.JobResource.Job0000012;
            eLUpload.UPLOAD_STATUS = "N";
            BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }
        else
        {
            try
            {
                dtblUploadCheck = BRM_UPLOAD_CHECK.Search(strFunctionKey, strUploadID);
            }
            catch
            {
                strMsgID = Resources.JobResource.Job0000013;
                eLUpload.UPLOAD_STATUS = "N";
                BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }

            //* 判斷該匯入檢核有無類型判斷數據
            if (dtblUploadCheck.Rows.Count > 0)
            {
                //* 判斷檔案類型
                if (file.Extension.ToUpper() != dtblUploadCheck.Rows[0]["EXTEND_NAME"].ToString())
                {
                    strMsgID = Resources.JobResource.Job0000014;
                    eLUpload.UPLOAD_STATUS = "N";
                    BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

                    return dtblUpload;
                }
            }
            else
            {
                strMsgID = Resources.JobResource.Job0000015;
                eLUpload.UPLOAD_STATUS = "N";
                BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }
        }
        #endregion

        int intBeginCount = int.Parse(dtblUploadCheck.Rows[0]["BEGIN_COUNT"].ToString());
        int intEndCount = int.Parse(dtblUploadCheck.Rows[0]["END_COUNT"].ToString());

        int intBeginColumn = int.Parse(dtblUploadCheck.Rows[0]["BEGIN_COLUMN"].ToString());
        int intEndColumn = int.Parse(dtblUploadCheck.Rows[0]["END_COLUMN"].ToString());

        #region  資料庫欄位類型定義檢核

        DataTable dtblUploadType = null;

        try
        {
            dtblUploadType = BRM_UPLOAD_TYPE.Search(strFunctionKey, strUploadID);
        }
        catch
        {
            strMsgID = Resources.JobResource.Job0000013;
            eLUpload.UPLOAD_STATUS = "N";
            BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }


        if (dtblUploadType.Rows.Count > 0)
        {
            //* 生成輸出表的欄位
            for (int i = 0; i < dtblUploadType.Rows.Count; i++)
            {
                DataColumn dcolUpload = new DataColumn(dtblUploadType.Rows[i]["FIELD_NAME"].ToString());

                dtblUpload.Columns.Add(dcolUpload);
            }
            DataColumn dcTep = new DataColumn("Context");
            dtblUpload.Columns.Add(dcTep);
        }
        else
        {
            strMsgID = Resources.JobResource.Job0000015;
            eLUpload.UPLOAD_STATUS = "N";
            BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }
        #endregion

        int intTemp = 0;
        int intOut = 0;
        int intFieldLength = 0;
        int intDecimalDigits = 0;
        int intUploadTotalCount = 0;
        decimal decOut = 0;
        string strTemp = string.Empty;
        string strUpload = string.Empty;
        string strField = string.Empty;


        #region  檔案筆數檢核
        StreamReader objStreamReader = null;
        //* 讀取文件,記錄行數
        try
        {
            objStreamReader = file.OpenText();

            while (objStreamReader.Peek() != -1)
            {
                objStreamReader.ReadLine();
                intUploadTotalCount++;
            }

            eLUpload.UPLOAD_TOTAL_COUNT = intUploadTotalCount - intBeginCount - intEndCount;
        }
        catch
        {
            strMsgID = Resources.JobResource.Job0000016;
            eLUpload.UPLOAD_STATUS = "N";
            BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }
        finally
        {
            objStreamReader.Close();
            file = null;
        }

        //* 資料行數大于15000,提示錯誤
        if (intUploadTotalCount - intBeginCount - intEndCount > intMax)
        {
            strMsgID = Resources.JobResource.Job0000017;
            eLUpload.UPLOAD_STATUS = "N";
            BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }

        #endregion

        #region  檔案欄位檢核

        try
        {
            string strMessage = string.Empty;

            objStreamReader = new StreamReader(strFilePath, System.Text.Encoding.Default);

            string strString = string.Empty;

            string strSplit = dtblUploadCheck.Rows[0]["LIST_SEPARATOR"].ToString();

            #region 有分隔符
            if (strSplit != "")
            {
                //* 頭筆數數據
                for (int i = 0; i < intBeginColumn; i++)
                {
                    dtblBegin.Columns.Add("begin" + i.ToString());
                }

                //* 尾筆數數據
                for (int i = 0; i < intEndColumn; i++)
                {
                    dtblEnd.Columns.Add("end" + i.ToString());
                }

                while (objStreamReader.Peek() != -1)
                {
                    intTemp++;

                    strString = objStreamReader.ReadLine();

                    string[] strUploads = strString.Split(strSplit.ToCharArray());
                    if (strUploads.Length <= 20)
                    {
                        strMsgID = Resources.JobResource.Job0000036;
                        eLUpload.UPLOAD_STATUS = "N";
                        DataRow drowUpload = dtblUpload.NewRow();
                        drowUpload["Context"] = strString;
                        dtblUpload.Rows.Add(drowUpload);
                        //金融卡格式不正確則返回 并記錄日志
                        BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);
                        Logging.Log(strUploadName + ":" + Resources.JobResource.Job0000036, LogState.Error, LogLayer.BusinessRule);
                        return dtblUpload;
                    }


                    if (intTemp > intBeginCount && intTemp <= intUploadTotalCount - intEndCount)
                    {
                        DataRow drowUpload = dtblUpload.NewRow();

                        //* 資料庫中欄位檢核個數與文件中的個數不等
                        if (dtblUploadType.Rows.Count > strUploads.Length || dtblUploadType.Rows.Count < strUploads.Length)
                        {
                            drowUpload["Context"] = strString;
                            dtblUpload.Rows.Add(drowUpload);

                            arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000018);
                            strMsgID = Resources.JobResource.Job0000036;
                            //* 資料庫中欄位檢核個數與文件中的個數不等,記錄進檢核日志
                            BaseHelper.LogUpload(eLUploadDetail, intTemp, strMsgID);
                        }
                        else
                        {
                            for (int i = 0; i < dtblUploadType.Rows.Count; i++)
                            {
                                strUpload = strUploads[i].Trim();
                                intFieldLength = int.Parse(dtblUploadType.Rows[i]["FIELD_LENGTH"].ToString());

                                intDecimalDigits = int.Parse(dtblUploadType.Rows[i]["DECIMAL_DIGITS"].ToString());

                                #region
                                switch (dtblUploadType.Rows[i]["FIELD_TYPE"].ToString().ToUpper())
                                {
                                    //* 字符類型
                                    case "STRING":
                                        if (BaseHelper.GetByteLength(strUpload) > intFieldLength)
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000019, ref arrListMsg);
                                            
                                            //* 欄位長度錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            drowUpload[0] = null;
                                        }
                                        break;

                                    //* 整數類型
                                    case "INT":
                                        if (!int.TryParse(strUpload == "" ? "0" : strUpload, out intOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            drowUpload[0] = null;
                                        }
                                        else
                                        {
                                            if (strUpload.Length > intFieldLength)
                                            {
                                                BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000019, ref arrListMsg);

                                                //* 欄位長度錯誤,記錄進檢核日志
                                                BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                drowUpload[0] = null;
                                            }
                                        }
                                        break;

                                    //* 時間日期類型
                                    case "DATETIME":
                                        strField = strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "");
                                        if (!int.TryParse(strField == "" ? "0" : strField, out intOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            drowUpload[0] = null;
                                        }
                                        break;

                                    //* 數字類型
                                    case "DECIMAL":
                                        if (!decimal.TryParse(strUpload == "" ? "0" : strUpload, out decOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            drowUpload[0] = null;
                                        }
                                        else
                                        {
                                            if (strUpload.Split('.').Length > 1)
                                            {
                                                strField = strUpload.Split('.')[0];
                                                if (strField.Length > intFieldLength - intDecimalDigits - 1)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    drowUpload[0] = null;
                                                }
                                                else
                                                {
                                                    strField = strUpload.Split('.')[1];

                                                    if (strField.Length > intDecimalDigits)
                                                    {
                                                        BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                        drowUpload[0] = null;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strUpload.Length > intFieldLength - intDecimalDigits - 1)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    drowUpload[0] = null;
                                                }
                                            }
                                        }

                                        break;


                                    //* 百分比類型
                                    case "PERCENT":
                                        strField = strUpload.Replace("%", "");

                                        if (!decimal.TryParse(strField == "" ? "0" : strField, out decOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            drowUpload[0] = null;
                                        }
                                        else
                                        {
                                            if (strField.Split('.').Length > 1)
                                            {
                                                strTemp = strField.Split('.')[0];
                                                if (strTemp.Length > intFieldLength - intDecimalDigits - 2)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    drowUpload[0] = null;
                                                }
                                                else
                                                {
                                                    strTemp = strField.Split('.')[1];

                                                    if (strTemp.Length > intDecimalDigits)
                                                    {
                                                        BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                        drowUpload[0] = null;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strField.Length > intFieldLength - intDecimalDigits - 2)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    drowUpload[0] = null;
                                                }
                                            }
                                        }

                                        break;

                                }
                                #endregion
                                drowUpload[i] = strUpload;

                            }
                            drowUpload["Context"] = strString;
                            dtblUpload.Rows.Add(drowUpload);
                        }
                    }
                    else if (intTemp <= intBeginCount)
                    {
                        DataRow drowBegin = dtblBegin.NewRow();


                        for (int i = 0; i < dtblBegin.Columns.Count; i++)
                        {
                            if (strUploads[i] != null)
                                drowBegin[i] = strUploads[i];
                        }

                        dtblBegin.Rows.Add(drowBegin);
                    }
                    else
                    {
                        DataRow drowEnd = dtblEnd.NewRow();

                        for (int i = 0; i < dtblEnd.Columns.Count; i++)
                        {
                            if (strUploads[i] != null)
                                drowEnd[i] = strUploads[i];
                        }

                        dtblEnd.Rows.Add(drowEnd);
                    }
                }
            }
            #endregion
            #region 無分隔符
            else
            {
                //* 頭筆數數據
                dtblBegin.Columns.Add("begin");

                //* 尾筆數數據
                dtblEnd.Columns.Add("end");

                int intRowTotal = 0;
                //* 每行允許的總長度
                for (int i = 0; i < dtblUploadType.Rows.Count; i++)
                {
                    intRowTotal = intRowTotal + Convert.ToInt32(dtblUploadType.Rows[i]["FIELD_LENGTH"].ToString());
                }

                while (objStreamReader.Peek() != -1)
                {
                    intTemp++;
                    strString = objStreamReader.ReadLine();
                    string[] strUploads = strString.Split("|".ToCharArray());
                    if (strUploads.Length > 1)
                    {
                        strMsgID = Resources.JobResource.Job0000036;

                        DataRow drowUpload = dtblUpload.NewRow();
                        drowUpload["Context"] = strString;
                        dtblUpload.Rows.Add(drowUpload);
                        //信用卡 格式不正確則返回 并記錄日志
                        BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);
                        Logging.Log(strUploadName + ":" + Resources.JobResource.Job0000036, LogState.Error, LogLayer.BusinessRule);
                        return dtblUpload;
                    }

                    if (intTemp > intBeginCount && intTemp <= intUploadTotalCount - intEndCount)
                    {

                        DataRow drowUpload = dtblUpload.NewRow();

                        if (BaseHelper.GetByteLength(strString) < intRowTotal || BaseHelper.GetByteLength(strString) > intRowTotal)
                        {
                            drowUpload["Context"] = strString;
                            dtblUpload.Rows.Add(drowUpload);
                            arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000023);
                            strMsgID = Resources.JobResource.Job0000036;
                            //* 欄位長度錯誤,記錄進檢核日志
                            BaseHelper.LogUpload(eLUploadDetail, intTemp, strMsgID);
                        }
                        else
                        {
                            int intNextBegin = 0;
                            for (int i = 0; i < dtblUploadType.Rows.Count; i++)
                            {
                                intFieldLength = int.Parse(dtblUploadType.Rows[i]["FIELD_LENGTH"].ToString());
                                intDecimalDigits = int.Parse(dtblUploadType.Rows[i]["DECIMAL_DIGITS"].ToString());

                                //*截取需要檢核的欄位
                                //int iStart = 1;
                                //int iEnd = intFieldLength;

                                strUpload = BaseHelper.GetSubstringByByte(strString, intNextBegin, intFieldLength, out intNextBegin).Trim();
                                #region 檢核類型
                                switch (dtblUploadType.Rows[i]["FIELD_TYPE"].ToString().ToUpper())
                                {
                                    //* 整數類型
                                    case "INT":

                                        if (!int.TryParse(strUpload == "" ? "0" : strUpload, out intOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000020");
                                            drowUpload[0] = null;
                                        }
                                        break;

                                    //* 時間日期類型
                                    case "DATETIME":
                                        if (!int.TryParse(strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "") == "" ? "0" : strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", ""), out intOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000020");
                                            drowUpload[0] = null;
                                        }
                                        break;

                                    //* 數字類型
                                    case "DECIMAL":

                                        if (!decimal.TryParse(strUpload == "" ? "0" : strField, out decOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000020");
                                            drowUpload[0] = null;
                                        }
                                        else
                                        {
                                            if (strUpload.Split('.').Length > 1)
                                            {
                                                strField = strUpload.Split('.')[0];
                                                if (strField.Length > intFieldLength - intDecimalDigits - 1)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000021");
                                                    drowUpload[0] = null;
                                                }
                                                else
                                                {
                                                    strField = strUpload.Split('.')[1];

                                                    if (strField.Length > intDecimalDigits)
                                                    {
                                                        BaseHelper.AddErrorMsg(intTemp, i, "Job0000022", ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000022");
                                                        drowUpload[0] = null;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strUpload.Length > intFieldLength - intDecimalDigits - 1)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000021");
                                                    drowUpload[0] = null;
                                                }
                                            }
                                        }

                                        break;

                                    //* 百分比類型
                                    case "PERCENT":
                                        strField = strUpload.Replace("%", "");

                                        if (!decimal.TryParse(strField == "" ? "0" : strField, out decOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000020");
                                            drowUpload[0] = null;
                                        }
                                        else
                                        {
                                            if (strField.Split('.').Length > 1)
                                            {
                                                strTemp = strField.Split('.')[0];
                                                if (strTemp.Length > intFieldLength - intDecimalDigits - 2)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000021");
                                                    drowUpload[0] = null;
                                                }
                                                else
                                                {
                                                    strTemp = strField.Split('.')[1];

                                                    if (strTemp.Length > intDecimalDigits)
                                                    {
                                                        BaseHelper.AddErrorMsg(intTemp, i, "Job0000022", ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000022");
                                                        drowUpload[0] = null;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strField.Length > intFieldLength - intDecimalDigits - 2)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, "Job0000021");
                                                    drowUpload[0] = null;
                                                }
                                            }
                                        }

                                        break;

                                }
                                #endregion
                                drowUpload[i] = strUpload;
                            }
                            drowUpload["Context"] = strString;
                            dtblUpload.Rows.Add(drowUpload);
                        }
                    }
                    else if (intTemp <= intBeginCount)
                    {
                        DataRow drowBegin = dtblBegin.NewRow();

                        drowBegin[0] = strString;

                        dtblBegin.Rows.Add(drowBegin);

                    }
                    else
                    {
                        DataRow drowEnd = dtblEnd.NewRow();

                        drowEnd[0] = strString;

                        dtblEnd.Rows.Add(drowEnd);
                    }
                }
            }


            BRL_UPLOAD.Add(eLUpload, ref strMessage);
            #endregion
        }
        catch
        {
            strMsgID = "Job0000016";
            eLUpload.UPLOAD_STATUS = "N";
            BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }
        finally
        {
            objStreamReader.Close();
        }

        #endregion
        
        return dtblUpload;
    }
    #endregion

    #region 匯入資料至DB
    public bool ImportToDB(DataTable dtDetail, string strFileName, Entity_CardBaseInfo TCardBaseInfo, string strCardType)
    {
        if (isReRun)
        {
            return ImportToDB(dtDetail, strFileName, TCardBaseInfo, strCardType, false, inDate.ToString("yyyy/MM/dd"), tranDate.ToString("yyyy/MM/dd"));
        }
        else {
            return ImportToDB(dtDetail, strFileName, TCardBaseInfo, strCardType, false, "", "");
        }
    }
    /// <summary>
    /// 功能說明:匯入資料至DB
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄: 2020/11/12_Ares_Stanley-修改匯入失敗錯誤
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strFileName"></param>
    /// <param name="strAMPMFlg"></param>
    /// <returns></returns>
    public bool ImportToDB(DataTable dtDetail, string strFileName, Entity_CardBaseInfo TCardBaseInfo, string strCardType, bool bForWeb, string strIndate, string strTranDate)
    {
        bool blnResult = false;
        DateTime dt = new DateTime();
        //*製卡日設定
        if (BRWORK_DATE.IS_WORKDAY("06", jobDate.ToString("yyyyMMdd")))
        {
            dt = jobDate;
        }
        else
        {
            dt = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", jobDate.ToString("yyyyMMdd"), 1), "yyyyMMdd", null);
        }

        EntitySet<EntityLayer.Entity_CardBaseInfo> SetCardBaseInfo = new EntitySet<EntityLayer.Entity_CardBaseInfo>();
        switch (strCardType)
        {
            case "1"://信用卡
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    //*明細欄位設定
                    TCardBaseInfo.indate1 = dt.ToString("yyyy/MM/dd");                      //製卡日
                    TCardBaseInfo.action = dtDetail.Rows[i]["action"].ToString();           //卡別
                    if (null != dtDetail.Rows[i]["kind"] && !string.IsNullOrEmpty(dtDetail.Rows[i]["kind"].ToString()))
                    {
                        TCardBaseInfo.kind = dtDetail.Rows[i]["kind"].ToString();           //取卡方式
                    }
                    else
                    {
                        TCardBaseInfo.kind = "0";                                           //取卡方式
                    }
                    TCardBaseInfo.cardtype = dtDetail.Rows[i]["cardtype"].ToString();       //卡片種類
                    TCardBaseInfo.photo = dtDetail.Rows[i]["photo"].ToString();             //相片卡別
                    TCardBaseInfo.affinity = dtDetail.Rows[i]["AffinityCode"].ToString();   //認同代碼
                    TCardBaseInfo.id = dtDetail.Rows[i]["IdNo"].ToString();                 //身分證字號
                    TCardBaseInfo.cardno = dtDetail.Rows[i]["cardno1"].ToString().Replace("-", "");          //卡號1
                    TCardBaseInfo.cardno2 = dtDetail.Rows[i]["cardno2"].ToString().Replace("-", "");         //卡號2
                    TCardBaseInfo.zip = dtDetail.Rows[i]["ZIP-C"].ToString();               //郵遞區號
                    TCardBaseInfo.add1 = dtDetail.Rows[i]["City"].ToString();               //Add1
                    TCardBaseInfo.add2 = dtDetail.Rows[i]["Add1"].ToString();               //Add2
                    TCardBaseInfo.add3 = dtDetail.Rows[i]["Add2"].ToString();               //Add3
                    TCardBaseInfo.mailno = "";                                              //無，待大宗回饋更新
                    TCardBaseInfo.n_card = JobHelper.GetCntcard(dtDetail.Rows[i]["AffinityCode"].ToString());    //卡數
                    TCardBaseInfo.maildate = "";                                            //無，待大宗回饋更新
                    TCardBaseInfo.expdate = dtDetail.Rows[i]["Card1EXPDATE"].ToString();    //有效期1
                    TCardBaseInfo.expdate2 = dtDetail.Rows[i]["Card2EXPDATE"].ToString();   //有效期2
                    TCardBaseInfo.seq = dtDetail.Rows[i]["SEQ"].ToString();                 //序號
                    TCardBaseInfo.custname = dtDetail.Rows[i]["CustName"].ToString();       //歸戶姓名
                    TCardBaseInfo.name1 = dtDetail.Rows[i]["Card1Name"].ToString();         //客戶姓名1
                    TCardBaseInfo.name2 = dtDetail.Rows[i]["Card2Name"].ToString();         //客戶姓名2
                    //TCardBaseInfo.trandate = dtDetail.Rows[i]["AffinityCode"].ToString(); //已設定
                    //TCardBaseInfo.card_file = dtDetail.Rows[i]["AffinityCode"].ToString();//已設定
                    TCardBaseInfo.disney_code = "";                                         //製卡檔無該欄位 --> 空白不填。
                    TCardBaseInfo.branch_id = dtDetail.Rows[i]["branch_id"].ToString();     //分行代碼
                    TCardBaseInfo.monlimit = dtDetail.Rows[i]["Month_Limit"].ToString();    //信用額度
                    TCardBaseInfo.is_LackCard = "1";                                        //*缺卡狀態(不缺卡)
                    //SetCardBaseInfo.Add(TCardBaseInfo);
                    TCardBaseInfo.OriginalDBflg = "0";
                    TCardBaseInfo.OutStore_Status = "0";
                    TCardBaseInfo.IntoStore_Status = "0";
                    TCardBaseInfo.Urgency_Flg = "0";
                    if (bForWeb || isReRun)
                    {
                        TCardBaseInfo.indate1 = strIndate;
                        TCardBaseInfo.trandate = strTranDate;
                    }
                    string strMsgID = string.Empty;
                    //if (!BRM_TCardBaseInfo.IsRepeatByAll(TCardBaseInfo))
                    //{
                    //    blnResult = BRM_TCardBaseInfo.Insert(TCardBaseInfo, ref strMsgID);
                    //}
                    //else
                    //{
                    //    string strMsgIDs = string.Empty;
                    //    Entity_JobErrorInfo JobErrInfo = new Entity_JobErrorInfo();
                    //    JobErrInfo.JobID = strJobId;                                    //JObid
                    //    JobErrInfo.ImportFileName = strFileName;                        //匯入檔名
                    //    JobErrInfo.ErrorContext = dtDetail.Rows[i]["Context"].ToString();//檢核錯誤的資料
                    //    JobErrInfo.ImportTime = DateTime.Now.ToString("yyyy/MM/dd");    //匯入時間
                    //    JobErrInfo.LocalFilePath = strPath;                             //檔案路徑
                    //    JobErrInfo.LoadFlag = "0";                                      //
                    //    JobErrInfo.Reason = Resources.JobResource.Job0000014;           //檢核失敗的原因
                    //    BRM_JobErrorInfo.Insert(JobErrInfo, ref strMsgIDs);
                    //    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003, strFileName));
                    //}

                    blnResult = BRM_TCardBaseInfo.Insert(TCardBaseInfo, ref strMsgID);

                    if (!blnResult)
                    {
                        string strMsgIDs = string.Empty;
                        Entity_JobErrorInfo JobErrInfo = new Entity_JobErrorInfo();
                        JobErrInfo.JobID = strJobId;                                    //JObid
                        JobErrInfo.ImportFileName = strFileName;                        //匯入檔名
                        JobErrInfo.ErrorContext = dtDetail.Rows[i]["Context"].ToString();//檢核錯誤的資料
                        JobErrInfo.ImportTime = DateTime.Now.ToString("yyyy/MM/dd");    //匯入時間
                        JobErrInfo.LocalFilePath = strPath;                             //檔案路徑
                        JobErrInfo.LoadFlag = "0";                                      //
                        JobErrInfo.Reason = Resources.JobResource.Job0000014;           //檢核失敗的原因
                        BRM_JobErrorInfo.Insert(JobErrInfo, ref strMsgIDs);
                        #region 重複卡號隱碼
                        string star = "";
                        string cardNoHead ="";
                        string cardNoFoot = "";
                        string cardNoLog = "";
                        if (TCardBaseInfo.cardno.Length > 10)
                        {
                            for (int starN = 0; starN< TCardBaseInfo.cardno.Length - 10; starN++)
                            {
                                star += "*";
                            }
                            cardNoHead = TCardBaseInfo.cardno.Substring(0, 6);
                            cardNoFoot = TCardBaseInfo.cardno.Substring(TCardBaseInfo.cardno.Length - 4);
                            cardNoLog = cardNoHead +  star + cardNoFoot; 
                        }
                        #endregion
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003 + " " + "錯誤卡號：" + cardNoLog + " " + "錯誤原因：" + JobErrInfo.Reason, strFileName));
                    }

                    BRM_CardDataChange.UpdateIndate(TCardBaseInfo);
                }
                break;

            case "2"://金融卡
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    //*明細欄位設定
                    TCardBaseInfo.indate1 = dt.ToString("yyyy/MM/dd");                              //製卡日
                    TCardBaseInfo.action = dtDetail.Rows[i]["ACTION"].ToString().Trim();            //卡別

                    //取卡方式
                    string strTakeCard = string.Empty;
                    if (null != dtDetail.Rows[i]["TAKE-CARD-FLAG"] && !string.IsNullOrEmpty(dtDetail.Rows[i]["TAKE-CARD-FLAG"].ToString()))
                    {
                        strTakeCard = dtDetail.Rows[i]["TAKE-CARD-FLAG"].ToString().Trim();
                    }
                    switch (strTakeCard)
                    {
                        case "1":
                            TCardBaseInfo.kind = "21";
                            break;
                        case "2":
                            TCardBaseInfo.kind = "22";
                            break;
                        case "3":
                            TCardBaseInfo.kind = "23";
                            break;
                        case "4":
                            TCardBaseInfo.kind = "24";
                            break;
                        case "5":
                            TCardBaseInfo.kind = "25";
                            break;
                        default:
                            TCardBaseInfo.kind = "0";
                            break;
                    }

                    TCardBaseInfo.cardtype = dtDetail.Rows[i]["CARD-TYPE"].ToString().Trim();       //卡片種類
                    TCardBaseInfo.photo = dtDetail.Rows[i]["PHOTO-TYPE"].ToString().Trim();         //相片卡別
                    TCardBaseInfo.affinity = dtDetail.Rows[i]["AFFINITY-CODE"].ToString().Trim();   //認同代碼
                    TCardBaseInfo.id = dtDetail.Rows[i]["IDNO"].ToString().Trim();                  //身分證字號
                    TCardBaseInfo.cardno = dtDetail.Rows[i]["CARD-NO-1"].ToString().Trim().Replace("-", "");         //卡號1
                    TCardBaseInfo.cardno2 = dtDetail.Rows[i]["CARD-NO-2"].ToString().Trim().Replace("-", "");        //卡號2
                    TCardBaseInfo.zip = dtDetail.Rows[i]["ZIP-C"].ToString().Trim();                //郵遞區號

                    string strAddgroup = JobHelper.SetStrngValue(dtDetail.Rows[i]["CITY"].ToString(), 120);
                    int iNextbegin = 0;
                    ArrayList alAdd = new ArrayList();
                    for (int iObj = 0; iObj < 3; iObj++)
                    {
                        alAdd.Add(BaseHelper.GetSubstringByByte(strAddgroup, iNextbegin, 40, out iNextbegin));
                    }
                    TCardBaseInfo.add1 = alAdd[0].ToString().Trim();                                //Add1
                    TCardBaseInfo.add2 = alAdd[1].ToString().Trim();                                //Add2
                    TCardBaseInfo.add3 = alAdd[2].ToString().Trim();                                //Add3

                    TCardBaseInfo.mailno = "";                                                      //無，待大宗回饋更新
                    TCardBaseInfo.n_card = JobHelper.GetCntcard(dtDetail.Rows[i]["AFFINITY-CODE"].ToString());    //卡數
                    TCardBaseInfo.maildate = "";                                                    //無，待大宗回饋更新
                    TCardBaseInfo.expdate = dtDetail.Rows[i]["CARD-1-EXP-DATE"].ToString().Trim();  //有效期1
                    TCardBaseInfo.expdate2 = dtDetail.Rows[i]["CARD-2-EXP-DATE"].ToString().Trim(); //有效期2
                    TCardBaseInfo.seq = dtDetail.Rows[i]["SEQ"].ToString().Trim();                  //序號
                    TCardBaseInfo.custname = dtDetail.Rows[i]["CUST-NAME"].ToString().Trim();       //歸戶姓名
                    TCardBaseInfo.name1 = dtDetail.Rows[i]["CARD-1-NAME"].ToString().Trim();        //客戶姓名1
                    TCardBaseInfo.name2 = dtDetail.Rows[i]["CARD-2-NAME"].ToString().Trim();        //客戶姓名2
                    //TCardBaseInfo.trandate = dtDetail.Rows[i]["AffinityCode"].ToString();         //已設定
                    //TCardBaseInfo.card_file = dtDetail.Rows[i]["AffinityCode"].ToString();        //已設定
                    TCardBaseInfo.disney_code = "";                                                 //製卡檔無該欄位 --> 空白不填。
                    TCardBaseInfo.branch_id = dtDetail.Rows[i]["BRANCH-IN"].ToString().Trim();      //分行代碼
                    TCardBaseInfo.monlimit = dtDetail.Rows[i]["MONTH-LIMIT"].ToString().Trim();     //信用額度
                    TCardBaseInfo.is_LackCard = "1";                                                //*缺卡狀態(不缺卡)
                    //SetCardBaseInfo.Add(TCardBaseInfo);
                    TCardBaseInfo.OriginalDBflg = "0";
                    TCardBaseInfo.OutStore_Status = "0";
                    TCardBaseInfo.IntoStore_Status = "0";
                    TCardBaseInfo.Urgency_Flg = "0";
                    if (bForWeb || isReRun)
                    {
                        TCardBaseInfo.indate1 = strIndate;
                        TCardBaseInfo.trandate = strTranDate;
                    }
                    string strMsgID = string.Empty;
                    if (!BRM_TCardBaseInfo.IsRepeatByAll(TCardBaseInfo)) //* 判斷是否有重復資料
                    {
                        blnResult = BRM_TCardBaseInfo.Insert(TCardBaseInfo, ref strMsgID);
                    }
                    else
                    {
                        string strMsgIDs = string.Empty;
                        Entity_JobErrorInfo JobErrInfo = new Entity_JobErrorInfo();
                        JobErrInfo.JobID = strJobId;                                    //JObid
                        JobErrInfo.ImportFileName = strFileName;                        //匯入檔名
                        JobErrInfo.ErrorContext = dtDetail.Rows[i]["Context"].ToString();//檢核錯誤的資料
                        JobErrInfo.ImportTime = DateTime.Now.ToString("yyyy/MM/dd");    //匯入時間
                        JobErrInfo.LocalFilePath = strPath;                             //檔案路徑
                        JobErrInfo.LoadFlag = "0";                                      //
                        JobErrInfo.Reason = Resources.JobResource.Job0000014;           //檢核失敗的原因
                        BRM_JobErrorInfo.Insert(JobErrInfo, ref strMsgIDs);
                        #region 重複卡號隱碼
                        string star = "";
                        string cardNoHead = "";
                        string cardNoFoot = "";
                        string cardNoLog = "";
                        if (TCardBaseInfo.cardno.Length > 10)
                        {
                            for (int starN = 0; starN < TCardBaseInfo.cardno.Length - 10; starN++)
                            {
                                star += "*";
                            }
                            cardNoHead = TCardBaseInfo.cardno.Substring(0, 6);
                            cardNoFoot = TCardBaseInfo.cardno.Substring(TCardBaseInfo.cardno.Length - 4);
                            cardNoLog = cardNoHead + star + cardNoFoot;
                        }
                        #endregion
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003 + " " + "錯誤卡號：" + cardNoLog + " " + "錯誤原因：" + JobErrInfo.Reason, strFileName));
                        JobHelper.SaveLog("上述檔案有重複的資料！", LogState.Info);
                    }
                }
                BRM_CardDataChange.UpdateIndate(TCardBaseInfo);
                break;
        }
        return blnResult;
    }
    #endregion

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明: 解決手動上傳匯入，重複資料Insert BRM_JobErrorInfo無JobID與路徑問題
    /// 作    者: Ares Luke
    /// 創建時間: 2020/11/12
    /// 修改記錄:
    /// </summary>
    /// <param name="webFilePath"></param>
    public void BForWebErrInfo(string webFilePath)
    {
        this.strJobId = "0101";
        this.strPath = webFilePath;
    }
}
