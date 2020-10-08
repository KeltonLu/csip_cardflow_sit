//******************************************************************
//*  功能說明：0102自動化大宗回饋檔匯入
//*  作    者：HAO CHEN
//*  創建日期：2010/06/10
//*  修改記錄：
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
using Framework.Common.Utility;
using System.Text;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM;

/// <summary>
/// AutoImportFilesReturn 的摘要描述
/// </summary>
public class AutoImportFilesReturn : Quartz.IJob
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
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間
    protected DateTime EndTime;
    protected static int iOperatcion = 0;
    string strFtpIp = string.Empty;
    string strFtpUserName = string.Empty;
    string strFtpPwd = string.Empty;
    FTPFactory objFtp;
    
    protected DateTime JobDate = DateTime.Now;
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/14
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    {
        try
        {
            string strTmp = string.Empty;
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
            string strTemp = Resources.JobResource.Job0101001;

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
                            JobDate = DateTime.Parse(arrStrParam[0]);
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000102, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty; //*本地存放目錄(格式為yyyyMMddHHmmss+JobID)
            JobHelper.CreateFolderName(strJobId, ref strFolderName);
            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                if (dtFileInfo.Rows.Count > 0)
                {

                    //*大宗回饋檔下載
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();
                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                        string[] strFiles = objFtp.GetFileList(rowFileInfo["FtpPath"].ToString());
                        ArrayList alImportFiles = new ArrayList();

                        foreach (string strFile in strFiles)
                        {
                            if (string.IsNullOrEmpty(strFile))//遇到空的檔名則繼續下一筆
                            {
                                continue;
                            }
                            string strTmpFile1 = JobDate.ToString("yyyyMMdd");
                            string strTmpFile2 = rowFileInfo["FtpFileName"].ToString().Trim();
                            if (strFile.Substring(0, 1).Equals(strTmpFile2))
                            {
                                if (strFile.Substring(1, 8).Equals(strTmpFile1))
                                {
                                    alImportFiles.Add(strFile);
                                }
                            }
                        }

                        //本地路徑
                        strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName + "\\";
                        string strFileName = string.Empty;
                        for (int iFileCount = 0; iFileCount < alImportFiles.Count; iFileCount++)
                        {
                            //FTP FILE NAME
                            strFileName = alImportFiles[0].ToString();
                            //FtpPath + FtpFileName
                            string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileName;

                            JobHelper.SaveLog("開始下載檔案！", LogState.Info);
                            //*下載檔案
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileName))
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath;                    //本地路徑
                                row["FtpFilePath"] = strFtpFileInfo;                    //FTP路徑
                                row["FolderName"] = strLocalPath;                       //本地資料夾
                                row["ZipFileName"] = strFileName;                       //FTP壓縮檔名稱
                                row["CardType"] = rowFileInfo["CardType"].ToString();   //卡片種類
                                row["MerchCode"] = rowFileInfo["MerchCode"].ToString(); //製卡廠代碼
                                row["MerchName"] = rowFileInfo["MerchName"].ToString(); //製卡廠名稱
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //FTP壓縮檔密碼解密
                                dtLocalFile.Rows.Add(row);
                                JobHelper.SaveLog("下載檔案成功！", LogState.Info);
                            }
                            else
                            {
                                //send mail 無法下載
                            }
                        }

                        if (alImportFiles.Count <= 0)
                        {
                            if (iOperatcion >= 2)
                            {
                                //send mail 檔案不存在(18:00及20:00都抓不到檔案)
                                ArrayList alInfo = new ArrayList();
                                alInfo.Add(strFileName);
                                SendMail("4", alInfo, Resources.JobResource.Job0000041);
                                iOperatcion = 0;
                            }
                            else
                            {
                                iOperatcion += 1;
                            }
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0102001, strFileName));
                        }
                    }
                }
            }
            #endregion

            #region 處理本地壓縮檔
            String errMsg = "";
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                int ZipCount = 0;
                bool blnResult = JobHelper.ZipExeFile(strLocalPath, strLocalPath + rowLocalFile["ZipFileName"].ToString(), rowLocalFile["ZipPwd"].ToString(), ref ZipCount);
                ////*解壓成功
                if (blnResult)
                {
                    rowLocalFile["ZipStates"] = "S";
                    rowLocalFile["TxtFileName"] = rowLocalFile["ZipFileName"].ToString().ToUpper().Replace(".ZIP", ".TXT");
                    JobHelper.SaveLog("解壓縮檔案成功！", LogState.Info);
                }
                //*解壓失敗
                else
                {
                    errMsg += (errMsg == "" ? "" : "、") + rowLocalFile["ZipFileName"];
                    rowLocalFile["ZipStates"] = "F";
                    JobHelper.SaveLog("解壓縮檔案失敗！");
                    // ArrayList alInfo = new ArrayList();
                    // alInfo.Add(rowLocalFile["ZipFileName"]);
                    //解壓失敗發送Mail通知
                    // SendMail("1", alInfo, Resources.JobResource.Job0000002);

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
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                string[] FileArray = FileTools.GetFileList(rowLocalFile["FolderName"].ToString());
                int iCount = 0;
                foreach (string strTmps in FileArray)
                {
                    //*txt檔名格式正確
                    if (JobHelper.ValidateTxt(FileArray[iCount]))
                    {
                        rowLocalFile["FormatStates"] = "S";
                    }
                    //*txt檔名格式錯誤
                    else
                    {
                        rowLocalFile["FormatStates"] = "F";
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101001, rowLocalFile["FtpFileName"].ToString()));
                        continue;
                    }
                    iCount++;
                }
                if (FileArray.Length < 0)
                {
                    JobHelper.SaveLog(Resources.JobResource.Job0000012);
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
                    string strFunctionName = Resources.JobResource.Job0000102;      //Job功能名稱

                    Entity_CardBaseInfo eCardBaseInfo = new Entity_CardBaseInfo();
                    eCardBaseInfo.Merch_Code = Row[rowcount]["MerchCode"].ToString(); //*獲取製卡廠代碼
                    eCardBaseInfo.card_file = strFileName;                          //*卡片名稱
                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        JobHelper.SaveLog("本地檔案存在！", LogState.Info);
                        int No = 0;                                //*匯入之錯誤編號

                        DataTable dtDetail = null;                 //檢核結果列表
                        //*檢核資料
                        JobHelper.SaveLog("開始檢核檔案：" + strFileName, LogState.Info);;
                        if (UploadCheck(strPath, strFunctionName, ref No, ref arrayErrorMsg, ref dtDetail))
                        {
                            JobHelper.SaveLog("檢核檔案成功！", LogState.Info);
                            //CheckMailDate(dtDetail);
                            
                            //*匯入檢核正確的回饋檔資料至卡片基本檔
                            if (BatUpdateFor0102(dtDetail))
                            {
                                Row[rowcount]["ImportStates"] = "S";
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101002, strFileName), LogState.Info);
                            }
                            else
                            {
                                //匯入失敗
                                Row[rowcount]["ImportStates"] = "F";
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003, strFileName));
                            }
                            JobHelper.SaveLog("正確資料匯入完成！", LogState.Info);
                        }
                        else
                        {
                            JobHelper.SaveLog("檢核檔案失敗！");
                            //檢核失敗
                            Row[rowcount]["ImportStates"] = "F";
                        }

                    }
                    //*file不存在local
                    else
                    {
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0101004, strPath));
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

            StringBuilder sbErrorInfo = new StringBuilder();
            for (int iError = 0; iError < arrayErrorMsg.Count; iError++)
            {
                if (null != sbErrorInfo && sbErrorInfo.Length > 0)
                {
                    sbErrorInfo.Append(";");
                    sbErrorInfo.Append(arrayErrorMsg[iError].ToString());
                }
                else
                {
                    sbErrorInfo.Append(arrayErrorMsg[iError].ToString());
                }
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

                ArrayList alInfo = new ArrayList();
                alInfo.Add(strTemps);
                alInfo.Add(sbErrorInfo);
                //內容格式錯誤
                SendMail("3", alInfo, Resources.JobResource.Job0000036);
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

    /// <summary>
    /// 檢核郵寄天數，不符合條件則發送郵件
    /// </summary>
    /// <param name="dtResult"></param>
    public void CheckMailDate(DataTable dtResult)
    {
        DataTable dtDetail = BRM_TCardBaseInfo.GetKindTableFor0102(dtResult, Resources.JobResource.Job010202);
        int iThreeMailDay = int.Parse(UtilHelper.GetAppSettings("ThreeMailDay"));
        int iUsualMailDay = int.Parse(UtilHelper.GetAppSettings("UsualMailDay"));
        String errMsg = "";

        if (dtDetail.Rows.Count > 0)
        {
            for (int i = 0; i < dtDetail.Rows.Count; i++)
            {
                if (dtDetail.Rows[i]["KIND"].ToString().Trim().Equals("5"))
                {
                    if (int.Parse(CSIPCommonModel.BusinessRules.BRWORK_DATE.QueryByDate("06", dtDetail.Rows[i]["INDATE1"].ToString().Replace(".", "/"), dtDetail.Rows[i]["MailDate"].ToString().Replace(".", "/"))) > iThreeMailDay)
                    {
                        errMsg += (errMsg == "" ? "" : "、") + dtDetail.Rows[i]["CARDNO"].ToString();
                        // ArrayList MailInfo = new ArrayList();
                        // MailInfo.Add(dtDetail.Rows[i]["CARDNO"].ToString());
                        // SendMail("7", MailInfo, "");
                    }
                }
                else
                {
                    if (int.Parse(CSIPCommonModel.BusinessRules.BRWORK_DATE.QueryByDate("06", dtDetail.Rows[i]["INDATE1"].ToString().Replace(".", "/"), dtDetail.Rows[i]["MailDate"].ToString().Replace(".", "/"))) > iUsualMailDay)
                    {
                        errMsg += (errMsg == "" ? "" : "、") + dtDetail.Rows[i]["CARDNO"].ToString();
                        // ArrayList MailInfo = new ArrayList();
                        // MailInfo.Add(dtDetail.Rows[i]["CARDNO"].ToString());
                        // SendMail("7", MailInfo, "");
                    }
                }
            }
        }

        if (errMsg != "")
        {
            ArrayList MailInfo = new ArrayList();
            MailInfo.Add(errMsg);
            SendMail("7", MailInfo, "");
        }
    }
    /// <summary>
    /// 資料匯入並檢核郵寄天數，不符合條件則發送郵件
    /// </summary>
    /// <param name="dtResult"></param>
    public bool BatUpdateFor0102(DataTable dtResult)
    {
        try
        {
            bool blnResult = true;
            DataTable dtDetail = BRM_TCardBaseInfo.GetKindTableFor0102(dtResult, Resources.JobResource.Job010202);
            int iThreeMailDay = int.Parse(UtilHelper.GetAppSettings("ThreeMailDay"));
            int iUsualMailDay = int.Parse(UtilHelper.GetAppSettings("UsualMailDay"));
            String errMsg = "";

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {

                //SqlHelper sqlhelp = new SqlHelper();
                Entity_CardBaseInfo TCardBaseInfo = new Entity_CardBaseInfo();

                TCardBaseInfo.id = dtResult.Rows[i]["IDNO"].ToString().Trim();
                TCardBaseInfo.cardno = dtResult.Rows[i]["CARDNO"].ToString().Trim().Replace("-", "");
                //因為發現廠商回饋檔中的製卡日欄位可能與卡片製卡日不同，請將對應卡片條件改為卡號、身分證號、card type、card_file BUG202
                //TCardBaseInfo.indate1 = dtTCardBaseInfo.Rows[i]["INDATE1"].ToString().Trim().Replace(".", "/");
                TCardBaseInfo.cardtype = dtResult.Rows[i]["CARDTYPE"].ToString().Trim();
                TCardBaseInfo.card_file = dtResult.Rows[i]["cardfile"].ToString().Trim();

                TCardBaseInfo.mailno = dtResult.Rows[i]["MailNo"].ToString();
                TCardBaseInfo.maildate = dtResult.Rows[i]["MailDate"].ToString().Replace(".", "/");
                TCardBaseInfo.indate1 = dtResult.Rows[i]["indate1"].ToString().Replace(".", "/");

                //sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
                //sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
                ////因為發現廠商回饋檔中的製卡日欄位可能與卡片製卡日不同，請將對應卡片條件改為卡號、身分證號、card type、card_file BUG202
                ////sqlhelp.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.indate1);
                //sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardtype, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardtype);
                //sqlhelp.AddCondition(Entity_CardBaseInfo.M_card_file, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.card_file);

                //if (!BRM_TCardBaseInfo.Update(TCardBaseInfo, sqlhelp.GetFilterCondition(), "mailno", "maildate"))//*更新條件設置
                //{
                //    blnResult = false;
                //    break;
                //}

                if (!BRM_TCardBaseInfo.BatUpdateFor0102(TCardBaseInfo))//*更新條件設置
                {
                    blnResult = false;
                    break;
                }

                if (dtDetail.Rows[i]["KIND"].ToString().Trim().Equals("5"))
                {
                    if (int.Parse(CSIPCommonModel.BusinessRules.BRWORK_DATE.QueryByDate("06", dtDetail.Rows[i]["INDATE1"].ToString().Replace(".", "/"), dtDetail.Rows[i]["MailDate"].ToString().Replace(".", "/"))) > iThreeMailDay)
                    {
                        errMsg += (errMsg == "" ? "" : "、") + dtDetail.Rows[i]["CARDNO"].ToString();
                        // ArrayList MailInfo = new ArrayList();
                        // MailInfo.Add(dtDetail.Rows[i]["CARDNO"].ToString());
                        // SendMail("7", MailInfo, "");
                    }
                }
                else
                {
                    if (int.Parse(CSIPCommonModel.BusinessRules.BRWORK_DATE.QueryByDate("06", dtDetail.Rows[i]["INDATE1"].ToString().Replace(".", "/"), dtDetail.Rows[i]["MailDate"].ToString().Replace(".", "/"))) > iUsualMailDay)
                    {
                        errMsg += (errMsg == "" ? "" : "、") + dtDetail.Rows[i]["CARDNO"].ToString();
                        // ArrayList MailInfo = new ArrayList();
                        // MailInfo.Add(dtDetail.Rows[i]["CARDNO"].ToString());
                        // SendMail("7", MailInfo, "");
                    }
                }
            }
            if (errMsg != "")
            {
                ArrayList MailInfo = new ArrayList();
                MailInfo.Add(errMsg);
                SendMail("7", MailInfo, "");
            }
            if (blnResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception exp)
        {
            BRM_TCardBaseInfo.SaveLog(exp.Message);
            return false;
        }

    }

    /// <summary>
    /// 日期1與日期2相差的天數
    /// </summary>
    /// <param name="strDate1"></param>
    /// <param name="strDate2"></param>
    /// <returns></returns>
    public int GetDateDay(string strDate1, string strDate2)
    {
        DateTime dtDate1 = new DateTime();
        DateTime dtDate2 = new DateTime();
        int day = 0;
        try
        {
            string[] strTmp = strDate1.Split('/');
            dtDate1 = new DateTime(int.Parse(strTmp[0]), int.Parse(strTmp[1]), int.Parse(strTmp[2]));

            strTmp = strDate2.Split('/');
            dtDate2 = new DateTime(int.Parse(strTmp[0]), int.Parse(strTmp[1]), int.Parse(strTmp[2]));

            TimeSpan ts = dtDate1 - dtDate2;
            day = ts.Days;
        }
        catch
        {
            return 0;
        }
        return day;
    }

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
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000102, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000102, strErrorName);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
                case "3":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');
                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000102, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], alMailInfo[1]);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
                case "4":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');
                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000102, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], strErrorName);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
                case "7":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');
                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0]);
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
    public bool UploadCheck(string strPath, string strFileName, ref int No, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
    {
        bool blnResult = true;
        #region base parameter
        string strUserID = "sys";
        string strFunctionKey = "06";
        string strUploadID = "06010200";
        DateTime dtmThisDate = DateTime.Now;
        int intMax = int.MaxValue;
        string strMsgID = string.Empty;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        #endregion

        //*卡大宗回饋檔解析
        dtDetail = UploadCheck(strUserID, strFunctionKey, strUploadID,
                   dtmThisDate, strFileName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

        //if (string.IsNullOrEmpty(strMsgID))
        //{
        //    blnResult = true;
        //}
        //*檢核成功
        if (arrayErrorMsg.Count <= 0)
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
    public static DataTable UploadCheck(string strUserID, string strFunctionKey, string strUploadID, DateTime dtmThisDate, string strUploadName, string strFilePath, int intMax, ArrayList arrListMsg, ref string strMsgID, DataTable dtblBegin, DataTable dtblEnd)
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

        //#region  檔案名稱檢核

        //if (Regex.Match(strFilePath, "[\u4E00-\u9FA5]+").Length > 0)
        //{
        //    strMsgID = "Job0000011";

        //    BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

        //    return dtblUpload;
        //}
        //#endregion

        #region  檔案類型檢核

        System.IO.FileInfo file = new System.IO.FileInfo(strFilePath);

        eLUpload.FILE_NAME = file.Name;

        DataTable dtblUploadCheck = null;

        //* 判斷檔案是否存在
        if (!file.Exists)
        {
            strMsgID = "Job0000012";
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
                strMsgID = "Job0000013";
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
                    strMsgID = "Job0000014";
                    eLUpload.UPLOAD_STATUS = "N";
                    BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

                    return dtblUpload;
                }
            }
            else
            {
                strMsgID = "Job0000015";
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
            strMsgID = "Job0000013";
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
            strMsgID = "Job0000015";
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
            strMsgID = "Job0000016";
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
            strMsgID = "Job0000017";
            eLUpload.UPLOAD_STATUS = "N";
            BaseHelper.LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }

        #endregion

        #region  檔案欄位檢核

        try
        {
            string strMessage = string.Empty;
            BRL_UPLOAD.Add(eLUpload, ref strMessage);

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

                    if (intTemp > intBeginCount && intTemp <= intUploadTotalCount - intEndCount)
                    {
                        DataRow drowUpload = dtblUpload.NewRow();

                        //* 資料庫中欄位檢核個數與文件中的個數不等
                        if (dtblUploadType.Rows.Count > strUploads.Length || dtblUploadType.Rows.Count < strUploads.Length)
                        {
                            drowUpload["Context"] = strString;
                            dtblUpload.Rows.Add(drowUpload);

                            arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000018);

                            //* 資料庫中欄位檢核個數與文件中的個數不等,記錄進檢核日志
                            BaseHelper.LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000018);
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
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000019", ref arrListMsg);

                                            //* 欄位長度錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                        }
                                        break;

                                    //* 整數類型
                                    case "INT":
                                        if (!int.TryParse(strUpload == "" ? "0" : strUpload, out intOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                        }
                                        else
                                        {
                                            if (strUpload.Length > intFieldLength)
                                            {
                                                BaseHelper.AddErrorMsg(intTemp, i, "Job0000019", ref arrListMsg);

                                                //* 欄位長度錯誤,記錄進檢核日志
                                                BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            }
                                        }
                                        break;

                                    //* 時間日期類型
                                    case "DATETIME":
                                        strField = strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "");
                                        if (!int.TryParse(strField == "" ? "0" : strField, out intOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                        }
                                        break;

                                    //* 數字類型
                                    case "DECIMAL":
                                        if (!decimal.TryParse(strUpload == "" ? "0" : strUpload, out decOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                }
                                                else
                                                {
                                                    strField = strUpload.Split('.')[1];

                                                    if (strField.Length > intDecimalDigits)
                                                    {

                                                        BaseHelper.AddErrorMsg(intTemp, i, "Job0000022", ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strUpload.Length > intFieldLength - intDecimalDigits - 1)
                                                {

                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                }
                                                else
                                                {
                                                    strTemp = strField.Split('.')[1];

                                                    if (strTemp.Length > intDecimalDigits)
                                                    {
                                                        BaseHelper.AddErrorMsg(intTemp, i, "Job0000022", ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strField.Length > intFieldLength - intDecimalDigits - 2)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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

                    if (intTemp > intBeginCount && intTemp <= intUploadTotalCount - intEndCount)
                    {

                        DataRow drowUpload = dtblUpload.NewRow();

                        if (BaseHelper.GetByteLength(strString) < intRowTotal || BaseHelper.GetByteLength(strString) > intRowTotal)
                        {
                            drowUpload["Context"] = strString;
                            dtblUpload.Rows.Add(drowUpload);
                            arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000023);

                            //* 欄位長度錯誤,記錄進檢核日志
                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                        }
                                        break;

                                    //* 時間日期類型
                                    case "DATETIME":
                                        if (!int.TryParse(strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "") == "" ? "0" : strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", ""), out intOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                        }
                                        break;

                                    //* 數字類型
                                    case "DECIMAL":

                                        if (!decimal.TryParse(strUpload == "" ? "0" : strField, out decOut))
                                        {
                                            BaseHelper.AddErrorMsg(intTemp, i, "Job0000020", ref arrListMsg);

                                            //* 欄位類型錯誤,記錄進檢核日志
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                }
                                                else
                                                {
                                                    strField = strUpload.Split('.')[1];

                                                    if (strField.Length > intDecimalDigits)
                                                    {
                                                        BaseHelper.AddErrorMsg(intTemp, i, "Job0000022", ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strUpload.Length > intFieldLength - intDecimalDigits - 1)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
                                            BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                }
                                                else
                                                {
                                                    strTemp = strField.Split('.')[1];

                                                    if (strTemp.Length > intDecimalDigits)
                                                    {
                                                        BaseHelper.AddErrorMsg(intTemp, i, "Job0000022", ref arrListMsg);
                                                        //* 欄位小數位數錯誤,記錄進檢核日志
                                                        BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (strField.Length > intFieldLength - intDecimalDigits - 2)
                                                {
                                                    BaseHelper.AddErrorMsg(intTemp, i, "Job0000021", ref arrListMsg);
                                                    //* 欄位整數位數錯誤,記錄進檢核日志
                                                    BaseHelper.LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
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
}
