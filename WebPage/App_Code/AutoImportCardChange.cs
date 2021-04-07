//******************************************************************
//*  功能說明：自動化換卡異動檔匯入
//*  作    者：zhiyuan
//*  創建日期：2010/05/20
//*  修改記錄：2021/04/06 增加.TXT檔案處理
//*  修改記錄：2021/04/07 新增 LOG紀錄 陳永銘
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using Framework.Common.Logging;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Common.Utility;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM;

public class AutoImportCardChange : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strAmOrPm = string.Empty;
    protected string strLocalPath = string.Empty;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間;
    protected DateTime EndTime;

    string strFtpIp = string.Empty;
    string strFtpUserName = string.Empty;
    string strFtpPwd = string.Empty;
    FTPFactory objFtp;

    private DateTime _jobDate = DateTime.Now;
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:zhiyuan
    /// 創建時間:2010/05/21
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    {
        try
        {
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            //strJobId = "0107";
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



            #region 記錄job啟動時間的分段
            string strAmOrPm = string.Empty;
            JobHelper.IsAmOrPm(StartTime, ref strAmOrPm);
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000107, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty;//*本地存放目錄(格式為yyyyMMdd+am/pm)
            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                if (dtFileInfo.Rows.Count > 0)
                {
                    //2021/04/07 新增 LOG紀錄 陳永銘
                    JobHelper.SaveLog("資料筆數:" + dtFileInfo.Rows.Count.ToString(), LogState.Info);

                    //*創建子目錄名稱，存放下載文件
                    string strMsg = string.Empty;
                    JobHelper.CreateFolderName(strJobId, ref strFolderName);
                    String errMsg = "";
                    //*處理大總檔檔名
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        //2021/04/07 新增 LOG紀錄 陳永銘
                        JobHelper.SaveLog("FTP主機:" + strFtpIp, LogState.Info);

                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        //2021/04/07 新增 LOG紀錄 陳永銘
                        JobHelper.SaveLog("FTP使用者名稱:" + strFtpUserName, LogState.Info);

                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();
                        //2021/04/07 新增 LOG紀錄 陳永銘
                        JobHelper.SaveLog("FTP密碼:" + strFtpUserName, LogState.Info);

                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                        //獲取FTP路徑下的所有檔案
                        //2021/04/07 新增 LOG紀錄 陳永銘
                        JobHelper.SaveLog("FTP路徑:" + rowFileInfo["FtpPath"].ToString(), LogState.Info);
                        string[] strFiles = objFtp.GetFileList(rowFileInfo["FtpPath"].ToString() + "//");

                        //本地路徑
                        strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //2021/04/07 新增 LOG紀錄 陳永銘
                        JobHelper.SaveLog("本地路徑:" + strLocalPath, LogState.Info);

                        foreach (string strFile in strFiles)
                        {
                            //2021/04/07 新增 LOG紀錄 陳永銘
                            JobHelper.SaveLog("檔案名稱:" + strFile, LogState.Info);

                            //抓取為rr01開頭的檔案
                            //增加抓取前一工作日的檔案
                            //if (!string.IsNullOrEmpty(strFile) && strFile.Substring(0, 4).Equals("rr01"))
                            string filename = "No";
                            if (strFile.Length > 8)
                            {
                                if (strFile.Substring(0, 8).Equals("rr01" + DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", _jobDate.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("MMdd")))
                                {
                                    filename = "ok";
                                }
                            }
                            //    if (!string.IsNullOrEmpty(strFile) && strFile.Substring(0, 8).Equals("rr01" + DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("MMdd")))

                            if (!string.IsNullOrEmpty(strFile) && filename.Equals("ok"))
                            {
                                //FTP 路徑+檔名
                                string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFile;

                                JobHelper.SaveLog("開始下載檔案！", LogState.Info);
                                //*下載檔案         
                                if (objFtp.Download(strFtpFileInfo, strLocalPath, strFile))
                                {
                                    //*記錄下載的檔案信息
                                    DataRow row = dtLocalFile.NewRow();
                                    row["LocalFilePath"] = strLocalPath; //本地路徑
                                    row["FtpFilePath"] = strFtpFileInfo; //FTP路徑
                                    row["FolderName"] = strLocalPath; //本地資料夾
                                    //row["TxtFileName"] = strFile; //檔案名稱
                                    row["TxtFileName"] = ""; //檔案名稱
                                    row["ZipFileName"] = strFile; //FTP壓縮檔名稱
                                    //row["ZipPwd"] = ""; //FTP壓縮檔密碼
                                    row["CardType"] = rowFileInfo["CardType"].ToString(); //卡片種類
                                    row["MerchCode"] = rowFileInfo["MerchCode"].ToString(); //製卡廠代碼
                                    row["MerchName"] = rowFileInfo["MerchName"].ToString(); //製卡廠名稱
                                    row["Trandate"] = GetTrandate(rowFileInfo["AMPMFlg"].ToString()); //轉檔日設定
                                    row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //解密
                                    //加密 RedirectHelper.GetEncryptParam(rowFileInfo["ZipPwd"].ToString());
                                    dtLocalFile.Rows.Add(row);
                                    JobHelper.SaveLog("下載檔案成功！", LogState.Info);
                                }
                                //*檔案不存在
                                else
                                {
                                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0107001, rowFileInfo["FtpFileName"].ToString()));
                                    errMsg += (errMsg == "" ? "" : "、") + rowFileInfo["FtpFileName"].ToString();
                                    // SendMail(rowFileInfo["FtpFileName"].ToString(), Resources.JobResource.Job0000008);
                                }
                            }
                        }
                    }

                    if (errMsg != "")
                    {
                        SendMail(errMsg, Resources.JobResource.Job0000008);
                    }
                }
            }
            #endregion

            //#region 處理本地檔案
            //foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            //{
            //    bool blnResult = true;
            //    //*成功
            //    if (blnResult)
            //    {
            //        rowLocalFile["ZipStates"] = "S";
            //        rowLocalFile["FormatStates"] = "S";
            //    }
            //}
            //#endregion

            #region 處理本地壓縮檔
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                //2021/04/06 增加.TXT檔案處理
                if (rowLocalFile["ZipPwd"].ToString().Trim() == "")
                {
                    //2021/04/07 新增 LOG紀錄 陳永銘
                    JobHelper.SaveLog("無解壓縮密碼", LogState.Info);
                    continue;
                }

                string strZipFileName = rowLocalFile["ZipFileName"].ToString().Trim();

                bool blnResult = ExeFile(strLocalPath, strZipFileName, rowLocalFile["ZipPwd"].ToString());
                ////*解壓成功
                if (blnResult)
                {
                    rowLocalFile["ZipStates"] = "S";
                    rowLocalFile["FormatStates"] = "S";

                    //2021/04/06 增加.TXT檔案處理
                    rowLocalFile["TxtFileName"] = strZipFileName.Replace(".EXE", ".txt");
                    JobHelper.SaveLog("解壓縮檔案成功！", LogState.Info);
                }
                //*解壓失敗
                else
                {
                    rowLocalFile["ZipStates"] = "F";
                    JobHelper.SaveLog("解壓縮檔案失敗！", LogState.Info);
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
                    string strPath = Row[rowcount]["LocalFilePath"].ToString() + strFileName;
                    string strCardType = Row[rowcount]["CardType"].ToString();      //*獲取卡片種類
                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        JobHelper.SaveLog("本地檔案存在！", LogState.Info);
                        int No = 0;                                //*匯入之錯誤編號
                        ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                        DataTable dtDetail = null;                 //檢核結果列表
                        string strMailErr = string.Empty;           //錯誤消息
                        //*檢核成功
                        if (UploadCheck(strPath, strFileName, strCardType, ref No, ref strMailErr, ref arrayErrorMsg, ref dtDetail))
                        {
                            if (dtDetail != null)
                            {
                                if (dtDetail.Rows.Count > 0)
                                {
                                    Row[rowcount]["CheckStates"] = "S";
                                    //*正式匯入
                                    if (ImportToDB(dtDetail, strFileName))
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
                                else
                                {
                                    Row[rowcount]["ImportStates"] = "S";
                                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0107003, strFileName), LogState.Info);
                                }
                            }
                            else
                            {
                                Row[rowcount]["ImportStates"] = "S";
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0107003, strFileName), LogState.Info);
                            }
                        }
                        //*檢核失敗
                        else
                        {
                            if (arrayErrorMsg.Count > 0)
                            {
                                for (int i = 0; i < arrayErrorMsg.Count; i++)
                                {
                                    if (i.Equals(arrayErrorMsg.Count - 1))
                                    {
                                        strMailErr = arrayErrorMsg[i].ToString();
                                    }
                                    else
                                    {
                                        strMailErr = arrayErrorMsg[i].ToString() + ",";
                                    }
                                }
                            }
                            else
                            {
                                strMailErr = Resources.JobResource.ResourceManager.GetObject("strMailErr").ToString();
                            }
                            Row[rowcount]["CheckStates"] = "F";
                            //*send mail
                            SendMail(strFileName, strMailErr);
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

            #region 成功匯入則刪除ftp上的資料
            DataRow[] RowD = dtLocalFile.Select("ZipStates='S' and FormatStates='S' and ImportStates='S'");
            for (int m = 0; m < RowD.Length; m++)
            {
                objFtp.Delete(RowD[m]["FtpFilePath"].ToString());//*路徑未設置
                JobHelper.SaveLog("刪除FTP上的檔案成功！", LogState.Info);
            }
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job結束日誌記錄
            DataRow[] RowS = dtLocalFile.Select("ZipStates='S' and FormatStates='S' and ImportStates='S'");
            DataRow[] RowF = dtLocalFile.Select("ZipStates='S' and FormatStates='S' and ImportStates='F'");
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
                    strTemps += RowF[k]["TxtFileName"].ToString() + "  ";
                }
                strReturnMsg += Resources.JobResource.Job0000026 + strTemps;
            }
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
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
                return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", _jobDate.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
            case "P":
                if (BRWORK_DATE.IS_WORKDAY("06", _jobDate.ToString("yyyyMMdd")))
                {
                    return _jobDate.ToString("yyyyMMdd");
                }
                else
                {
                    return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", _jobDate.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
                }
            default:
                if (BRWORK_DATE.IS_WORKDAY("06", _jobDate.ToString("yyyyMMdd")))
                {
                    return _jobDate.ToString("yyyyMMdd");
                }
                else
                {
                    return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", _jobDate.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
                }
        }

    }
    #endregion

    #region mail通知
    /// <summary>
    /// 功能說明:mail通知
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/11
    /// 修改記錄:
    /// </summary>
    /// <param name="strMessage"></param>
    public void SendMail(string strFileName, string strCon)
    {
        DataTable dtCallMail = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityM_CallMail.M_JobID, Operator.Equal, DataTypeUtils.String, strJobId);
        sqlhelp.AddCondition(EntityM_CallMail.M_ConditionID, Operator.Equal, DataTypeUtils.String, "1");
        if (BRM_CallMail.SearchMailByNo(sqlhelp.GetFilterCondition(), ref dtCallMail, ref strMsgID))
        {
            string strFrom = UtilHelper.GetAppSettings("MailSender");
            string[] strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
            string[] strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');
            string strSubject = string.Format(dtCallMail.Rows[0]["MailTittle"].ToString(), Resources.JobResource.Job0000107, strFileName);
            string strBody = string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strFileName, Resources.JobResource.Job0000107, strCon);
            JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
        }
    }
    #endregion

    #region 匯入資料檢核
    ///// <summary>
    ///// 功能說明:匯入資料檢核
    ///// 作    者:Simba Liu
    ///// 創建時間:2010/05/04
    ///// 修改記錄:
    ///// </summary>
    ///// <param name="strPath"></param>
    ///// <param name="strTpye"></param>
    ///// <returns></returns>
    public bool UploadCheck(string strPath, string strFileName, string strTpye, ref int No, ref string strMsgID, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
    {
        bool blnResult = true;
        #region base parameter
        string strUserID = "sys";
        string strFunctionKey = "06";
        string strUploadID = string.Empty;
        DateTime dtmThisDate = DateTime.Now;
        int intMax = int.MaxValue;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        #endregion

        strUploadID = "06010700";
        dtDetail = BaseHelper.UploadCheck(strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, Resources.JobResource.Job0000107, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);


        //*檢核成功
        if (strMsgID == "" && arrayErrorMsg.Count == 0)
        {
            blnResult = true;
        }
        //*檢核失敗
        else
        {
            if (arrayErrorMsg.Count > 0)
            {
                //No = int.Parse(arrayErrorMsg[0].ToString());
            }
            blnResult = false;
        }
        return blnResult;
    }
    #endregion

    #region 匯入資料至DB
    /// <summary>
    /// 功能說明:匯入資料至DB
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strFileName"></param>
    /// <param name="strAMPMFlg"></param>
    /// <returns></returns>
    //public bool ImportToDB(DataTable dtDetail, string strFileName, string strTranDate)
    //{
    //    bool blnResult = false;
    //    string Trandate = DateTime.Now.Year + "/" + strFileName.Substring(4, 2) + "/" + strFileName.Substring(6, 2);
    //    Trandate = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", Trandate.Replace("/", ""), -1);
    //    Trandate = Trandate.Substring(0, 4) + "/" + Trandate.Substring(4, 2) + "/" + Trandate.Substring(6, 2);
    //    string strTranDateTmp = string.Empty;
    //    string strMsg = string.Empty;
    //    string strSql = string.Empty;
    //    StringBuilder SqlCondition = new StringBuilder();
    //    DataTable dtBaseInfo = null;

    //    DataRow[] drSort = dtDetail.Select("", "CardNo");

    //    string strCardNo = string.Empty;
    //    if (drSort.Length > 0)
    //    {
    //        strCardNo = drSort[0]["CardNo"].ToString();
    //        for (int i = 1; i < drSort.Length; i++)
    //        {
    //            if (strCardNo.Equals(drSort[i]["CardNo"].ToString()))
    //            {
    //                dtDetail.Rows.Remove(drSort[i]);
    //            }
    //            else
    //            {
    //                strCardNo = drSort[i]["CardNo"].ToString();
    //            }
    //        }
    //    }

    //    //*查詢BaseInfo檔的條件
    //    for (int i = 0; i < dtDetail.Rows.Count; i++)
    //    {
    //        SqlCondition = SqlCondition.Append("'" + dtDetail.Rows[i]["CardNo"].ToString().Replace("-", "") + "',");
    //    }
    //    if (SqlCondition.Length > 0)
    //    {
    //        strSql = SqlCondition.ToString().Substring(0, SqlCondition.Length - 1);
    //    }

    //    BRM_TCardBaseInfo.selectByCardChangeInfo(ref dtBaseInfo, strSql);
    //    if (null != dtBaseInfo && dtBaseInfo.Rows.Count > 0)
    //    {
    //        EntitySet<EntityLayer.Entity_CardChange> SetCardChange = new EntitySet<EntityLayer.Entity_CardChange>();
    //        for (int i = 0; i < dtDetail.Rows.Count; i++)
    //        {
    //            DataRow[] Row = dtBaseInfo.Select("CardNo='" + dtDetail.Rows[i]["CardNo"].ToString().Replace("-", "") + "'");
    //            if (Row.Length > 0)
    //            {
    //                EntityLayer.Entity_CardChange TCardChange = new EntityLayer.Entity_CardChange();
    //                //TCardChange.Sno = "";       //*系統產生
    //                //TCardChange.Trandate = strTranDate;   //*轉檔日
    //                TCardChange.Trandate = Trandate;
    //                TCardChange.Id = Row[0]["id"].ToString();        //*卡片基本資料檔身份證字號
    //                TCardChange.CustName = Row[0]["custname"].ToString();  //卡片基本資料檔客戶姓名
    //                TCardChange.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");    //*系統當前時間
    //                TCardChange.ImportFileName = strFileName;    //*換卡異動檔檔名
    //                TCardChange.OutputFlg = "N";
    //                TCardChange.FilePath = strLocalPath + "\\" + strFileName;
    //                TCardChange.OutputDate = "";
    //                TCardChange.OutputFileName = "";

    //                TCardChange.CardNo = dtDetail.Rows[i]["CardNo"].ToString().Replace("-", "");    //*換卡異動檔CardNo
    //                TCardChange.BlockCode = dtDetail.Rows[i]["BlockCode"].ToString();  //*換卡異動檔BlockCode

    //                SetCardChange.Add(TCardChange);
    //            }
    //            else
    //            {
    //                JobHelper.SaveLog(String.Format(Resources.JobResource.Job0107002, dtDetail.Rows[i]["CardNo"]));
    //            }
    //        }
    //        blnResult = BRM_CardChange.BatInsertFor0107(SetCardChange, ref strMsg);
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //    return blnResult;
    //}

    public bool ImportToDB(DataTable dtDetail, string strFileName)
    {
        bool blnResult = false;
        string strTranDate = string.Empty;
        string strCardNo = string.Empty;
        string strBlockCode = string.Empty;
        string strCardNoCompare = string.Empty;
        string strMsg = string.Empty;
        DataTable dtBaseInfo = null;
        int intNoCard = 0;

        strTranDate = _jobDate.Year + "/" + strFileName.Substring(4, 2) + "/" + strFileName.Substring(6, 2);
        strTranDate = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", strTranDate.Replace("/", ""), -1);
        strTranDate = strTranDate.Substring(0, 4) + "/" + strTranDate.Substring(4, 2) + "/" + strTranDate.Substring(6, 2);

        DataRow[] drSort = dtDetail.Select("", "CardNo");

        if (drSort.Length > 0)
        {
            for (int i = 0; i < drSort.Length; i++)
            {
                strCardNoCompare = drSort[i]["CardNo"].ToString();

                for (int j = i + 1; j < drSort.Length; j++)
                {
                    if (strCardNoCompare.Equals(drSort[j]["CardNo"].ToString()))
                    {
                        dtDetail.Rows.Remove(drSort[j]);
                    }
                }
            }
        }

        EntitySet<EntityLayer.Entity_CardChange> SetCardChange = new EntitySet<EntityLayer.Entity_CardChange>();
        for (int i = 0; i < dtDetail.Rows.Count; i++)
        {
            strBlockCode = dtDetail.Rows[i]["BlockCode"].ToString().Trim();
            strCardNo = dtDetail.Rows[i]["CardNo"].ToString().Replace("-", "");
            if (strBlockCode != "")
            {
                if (BRM_TCardBaseInfo.SelectByCardChangeInfo(ref dtBaseInfo, strCardNo, strTranDate))
                {
                    EntityLayer.Entity_CardChange TCardChange = new EntityLayer.Entity_CardChange();
                    TCardChange.Trandate = strTranDate;
                    TCardChange.Id = dtBaseInfo.Rows[0]["id"].ToString();        //*卡片基本資料檔身份證字號
                    TCardChange.CustName = dtBaseInfo.Rows[0]["custname"].ToString();  //卡片基本資料檔客戶姓名
                    TCardChange.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");    //*系統當前時間
                    TCardChange.ImportFileName = strFileName;    //*換卡異動檔檔名
                    TCardChange.OutputFlg = "N";
                    TCardChange.FilePath = strLocalPath + "\\" + strFileName;
                    TCardChange.OutputDate = "";
                    TCardChange.OutputFileName = "";

                    TCardChange.CardNo = strCardNo;    //*換卡異動檔CardNo
                    TCardChange.BlockCode = strBlockCode;  //*換卡異動檔BlockCode

                    SetCardChange.Add(TCardChange);
                }
                else
                {
                    JobHelper.SaveLog(String.Format(Resources.JobResource.Job0107002, dtDetail.Rows[i]["CardNo"]));
                    intNoCard = intNoCard + 1;
                }
            }
            else
            {
                JobHelper.SaveLog(String.Format(Resources.JobResource.Job0107004, dtDetail.Rows[i]["CardNo"]));
            }
        }
        if (intNoCard != 0)
        {
            return false;
        }
        blnResult = BRM_CardChange.BatInsertFor0107(SetCardChange, ref strMsg);

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

        strTXTFileName = srcZipFile.Replace("EXE", "txt");

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

}
