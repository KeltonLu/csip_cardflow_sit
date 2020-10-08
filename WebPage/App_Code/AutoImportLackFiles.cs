//******************************************************************
//*  功能說明：自動化缺卡大宗檔匯入
//*  作    者：Simba Liu
//*  創建日期：2010/05/14
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
using Framework.Data.OM.Collections;
using Framework.Common.Utility;
using System.Text;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM;

public class AutoImportLackFiles : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId = string.Empty;//* "0103";
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strAmOrPm;
    protected string strLocalPath = string.Empty;//*ConfigurationManager.AppSettings["DownloadFilePath"] + "0103";
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;
    string strFileName = string.Empty;

    string strFtpIp = string.Empty;
    string strFtpUserName = string.Empty;
    string strFtpPwd = string.Empty;
    string strMechCode = string.Empty;
    FTPFactory objFtp;
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
            #region 記錄job啟動時間
            StartTime = DateTime.Now;
            #endregion
            #region load jobid and LocalPath
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
            //strJobId = "0103";
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000103, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty;//*本地存放目錄(格式為JOBID+yyyyMMddHHmmss)

            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                if (dtFileInfo.Rows.Count > 0)
                {
                    //*創建子目錄名稱，存放下載文件
                    string strMsg = string.Empty;
                    JobHelper.CreateFolderName(strJobId, ref strFolderName);
                    //*處理大總檔檔名
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();
                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                        //本地路徑
                        strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //FTP 檔名
                        ArrayList arrFileList = new ArrayList();
                        if (rowFileInfo["FtpFileName"].ToString().Length >= 2)
                        {
                            if (rowFileInfo["FtpFileName"].ToString().Substring(rowFileInfo["FtpFileName"].ToString().Length - 2, 2).ToLower().Equals("de"))
                            {
                                arrFileList = GetDownList(objFtp.GetFileList(rowFileInfo["FtpPath"].ToString() + "//"), rowFileInfo["FtpFileName"].ToString());
                            }
                            else
                            {
                                arrFileList.Add(DateTime.Now.ToString("yyyyMMdd") + rowFileInfo["FtpFileName"].ToString() + ".ZIP");
                            }
                        }
                        else
                        {
                            arrFileList.Add(DateTime.Now.ToString("yyyyMMdd") + rowFileInfo["FtpFileName"].ToString() + ".ZIP");
                        }
                        //FTP 路徑+檔名
                        //string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;
                        //FTP 路徑
                        string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//";



                        //*檔案存在

                        foreach (string strFile in arrFileList)
                        {
                            if (objFtp.isInFolderList(strFtpFileInfo + strFile))
                            {
                                DataRow[] dr = dtLocalFile.Select("ZipFileName='" + strFile + "'");
                                if (dr.Length > 0)
                                {
                                    continue;
                                }
                                JobHelper.SaveLog("開始下載檔案！", LogState.Info);
                                //*下載檔案
                                if (objFtp.Download(strFtpFileInfo + strFile, strLocalPath, strFile))
                                {
                                    //*記錄下載的檔案信息
                                    DataRow row = dtLocalFile.NewRow();
                                    row["LocalFilePath"] = strLocalPath; //本地路徑
                                    row["FtpFilePath"] = strFtpFileInfo + strFile;  //FTP 路徑+檔名   //FTP路徑  <--原本的
                                    row["FolderName"] = strLocalPath; //本地資料夾
                                    row["ZipFileName"] = strFile; //FTP壓縮檔名稱
                                    row["CardType"] = rowFileInfo["CardType"].ToString(); //卡片種類
                                    row["MerchCode"] = rowFileInfo["MerchCode"].ToString(); //製卡廠代碼
                                    row["MerchName"] = rowFileInfo["MerchName"].ToString(); //製卡廠名稱
                                    row["Trandate"] = rowFileInfo["AMPMFlg"].ToString(); //轉檔日設定
                                    row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //FTP壓縮檔密碼解密
                                    dtLocalFile.Rows.Add(row);
                                    JobHelper.SaveLog("下載檔案成功！", LogState.Info);
                                }
                            }
                            //*檔案不存在
                            else
                            {
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job010300, strFile));
                            }
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
                bool blnResult = JobHelper.ZipExeFile(strLocalPath, strLocalPath + rowLocalFile["ZipFileName"].ToString(), rowLocalFile["ZipPwd"].ToString(), ref ZipCount);
                ////*解壓成功
                if (blnResult)
                {
                    rowLocalFile["ZipStates"] = "S";
                    rowLocalFile["TxtFileName"] = rowLocalFile["ZipFileName"].ToString().Replace(".ZIP", ".txt");
                    JobHelper.SaveLog("解壓縮檔案成功！", LogState.Info);
                }
                //*解壓失敗
                else
                {
                    errMsg += (errMsg == "" ? "" : "、") + rowLocalFile["ZipFileName"];
                    rowLocalFile["ZipStates"] = "F";
                    // ArrayList alInfo = new ArrayList();
                    // alInfo.Add(rowLocalFile["ZipFileName"]);
                    //解壓失敗發送Mail通知
                    // SendMail("1", alInfo, Resources.JobResource.Job0000002);
                    JobHelper.SaveLog("解壓縮檔案失敗！");
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
                    strFileName = Row[rowcount]["TxtFileName"].ToString();
                    string strPath = Row[rowcount]["LocalFilePath"].ToString() + strFileName;
                    string strCardType = Row[rowcount]["CardType"].ToString();   //*獲取檔案類型
                    string strFunctionName = Resources.JobResource.Job0000103;      //Job功能名稱
                                                                                    //*file存在local
                    if (File.Exists(strPath))
                    {

                        int No = 0;                                //*匯入之錯誤編號
                        ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                        DataTable dtDetail = null;                 //檢核結果列表
                                                                   //*檢核成功
                        if (UploadCheck(strPath, strFunctionName, strCardType, ref No, ref arrayErrorMsg, ref dtDetail))
                        {
                            JobHelper.SaveLog("檢核檔案成功！", LogState.Info);
                            Row[rowcount]["CheckStates"] = "S";

                            Entity_CardBaseInfo eCardBaseInfo = new Entity_CardBaseInfo();
                            eCardBaseInfo.trandate = GetTrandate(Row[rowcount]["Trandate"].ToString());  //*獲取轉檔日
                            eCardBaseInfo.Merch_Code = Row[rowcount]["MerchCode"].ToString(); //*獲取製卡廠代碼
                            eCardBaseInfo.card_file = strFileName;                          //*匯入檔名

                            JobHelper.SaveLog("開始匯入資料！", LogState.Info);
                            //*正式匯入
                            if (ImportToDB(dtDetail, strFileName, eCardBaseInfo, strCardType))
                            {
                                Row[rowcount]["ImportStates"] = "S";
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job010301, strFileName), LogState.Info);
                            }
                            else
                            {
                                Row[rowcount]["ImportStates"] = "F";
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job010302, strFileName));
                            }
                        }
                        //*檢核失敗
                        else
                        {
                            JobHelper.SaveLog("檢核檔案失敗！");
                            StringBuilder sbErrorFiles = new StringBuilder();
                            for (int i = 0; i < arrayErrorMsg.Count; i++)
                            {
                                if (sbErrorFiles.Length <= 0)
                                {
                                    sbErrorFiles.Append(arrayErrorMsg[i].ToString());
                                }
                                else
                                {
                                    sbErrorFiles.Append(";");
                                    sbErrorFiles.Append(arrayErrorMsg[i].ToString());
                                }
                            }
                            if (null != sbErrorFiles && sbErrorFiles.Length > 0)
                            {

                                Row[rowcount]["CheckStates"] = "F";
                                //*send mail
                                ArrayList al = new ArrayList();
                                al.Add(Row[rowcount]["ZipFileName"]);
                                al.Add(sbErrorFiles.ToString());
                                SendMail("3", al, Resources.JobResource.Job0000036);
                            }
                        }
                    }
                    //*file不存在local
                    else
                    {
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job010303, strPath));
                    }
                }
                JobHelper.SaveLog("結束讀取要匯入的檔案資料！", LogState.Info);
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
            //strReturnMsg += MessageHelper.GetMessage("06_01010000_004") + SCount;
            //strReturnMsg += MessageHelper.GetMessage("06_01010000_005") + FCount + "!";
            strReturnMsg += Resources.JobResource.Job010304 + SCount;
            strReturnMsg += Resources.JobResource.Job010305 + FCount + "!";
            if (RowF != null && RowF.Length > 0)
            {
                string strTemp = string.Empty;
                for (int k = 0; k < RowF.Length; k++)
                {
                    strTemp += RowF[k]["TxtFileName"].ToString() + "  ";
                }
                //strReturnMsg += MessageHelper.GetMessage("06_01010000_006") + strTemp;
                strReturnMsg += Resources.JobResource.Job010306 + strTemp;
            }
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
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
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000103, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000103, strErrorName);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
                case "3":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000103, alMailInfo[0]);

                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], alMailInfo[1]);
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
    /// 作    者:Simba Liu
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
        int intMax = int.MaxValue;
        string strMsgID = string.Empty;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();

        #endregion

        //*信用卡大宗掛號檔解析
        if (strTpye.Equals("1"))
        {
            strUploadID = "06010300";
            strFileName = Resources.JobResource.Job000010301;      //Job功能名稱
            dtDetail = AutoImportFiles.UploadCheck(strTpye, strUserID, strFunctionKey, strUploadID,
                                   dtmThisDate, strFileName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

        }
        //*金融卡大宗掛號檔解析
        else
        {
            strUploadID = "06010301";
            strFileName = Resources.JobResource.Job000010302;      //Job功能名稱
            dtDetail = AutoImportFiles.UploadCheck(strTpye, strUserID, strFunctionKey, strUploadID,
                                   dtmThisDate, strFileName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);
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
    #endregion

    #region 匯入資料至DB
    public bool ImportToDB(DataTable dtDetail, string strFileName, Entity_CardBaseInfo TCardBaseInfo, string strCardType)
    {
        return ImportToDB(dtDetail, strFileName, TCardBaseInfo, strCardType, false, "", "");
    }
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
    public bool ImportToDB(DataTable dtDetail, string strFileName, Entity_CardBaseInfo TCardBaseInfo, string strCardType, bool bForWeb, string strInDate, string strTranDate)
    {
        bool blnResult = true;
        DateTime dt = new DateTime();
        //*製卡日設定
        if (BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
        {
            dt = DateTime.Now;
        }
        else
        {
            dt = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), 1), "yyyyMMdd", null);
        }

        string strTmp = strFileName.Substring(strFileName.Length - 6, 2);
        switch (strTmp)
        {
            case "DO"://*新增缺卡大宗檔
                EntitySet<EntityLayer.Entity_CardBaseInfo> SetCardBaseInfo = new EntitySet<EntityLayer.Entity_CardBaseInfo>();
                switch (strCardType)
                {
                    case "1"://信用卡
                        for (int i = 0; i < dtDetail.Rows.Count; i++)
                        {
                            Entity_CardBaseInfo tmpCardBaseInfo = new Entity_CardBaseInfo();

                            tmpCardBaseInfo.trandate = TCardBaseInfo.trandate;                      //*獲取轉檔日
                            tmpCardBaseInfo.Merch_Code = TCardBaseInfo.Merch_Code;                  //*獲取製卡廠代碼
                            tmpCardBaseInfo.card_file = TCardBaseInfo.card_file;                    //*匯入檔名
                            //*明細欄位設定
                            tmpCardBaseInfo.indate1 = dt.ToString("yyyy/MM/dd");                      //*製卡日
                            tmpCardBaseInfo.action = dtDetail.Rows[i]["action"].ToString();           //*卡別

                            if (null != dtDetail.Rows[i]["kind"] && !string.IsNullOrEmpty(dtDetail.Rows[i]["kind"].ToString()))
                            {
                                tmpCardBaseInfo.kind = dtDetail.Rows[i]["kind"].ToString();           //取卡方式
                            }
                            else
                            {
                                tmpCardBaseInfo.kind = "0";                                           //取卡方式
                            }
                            tmpCardBaseInfo.cardtype = dtDetail.Rows[i]["cardtype"].ToString();       //*卡片種類
                            tmpCardBaseInfo.photo = dtDetail.Rows[i]["photo"].ToString();             //*相片卡別
                            tmpCardBaseInfo.affinity = dtDetail.Rows[i]["AffinityCode"].ToString();   //*認同代碼
                            tmpCardBaseInfo.id = dtDetail.Rows[i]["IdNo"].ToString();                 //*身分證字號
                            tmpCardBaseInfo.cardno = dtDetail.Rows[i]["cardno1"].ToString().Replace("-", "");   //*卡號1
                            tmpCardBaseInfo.cardno2 = dtDetail.Rows[i]["cardno2"].ToString().Replace("-", ""); //*卡號2
                            tmpCardBaseInfo.zip = dtDetail.Rows[i]["ZIP-C"].ToString();               //*郵遞區號
                            tmpCardBaseInfo.add1 = dtDetail.Rows[i]["City"].ToString();               //*Add1
                            tmpCardBaseInfo.add2 = dtDetail.Rows[i]["Add1"].ToString();               //*Add2
                            tmpCardBaseInfo.add3 = dtDetail.Rows[i]["Add2"].ToString();               //*Add3
                            tmpCardBaseInfo.mailno = "";                                              //*掛號號碼
                            tmpCardBaseInfo.n_card = JobHelper.GetCntcard(dtDetail.Rows[i]["AffinityCode"].ToString());    //卡數
                            tmpCardBaseInfo.maildate = "";                                            //*郵遞日期
                            tmpCardBaseInfo.expdate = dtDetail.Rows[i]["Card1EXPDATE"].ToString();    //*有效期1
                            tmpCardBaseInfo.expdate2 = dtDetail.Rows[i]["Card2EXPDATE"].ToString();   //*有效期2
                            tmpCardBaseInfo.seq = dtDetail.Rows[i]["SEQ"].ToString();                 //*序號
                            tmpCardBaseInfo.custname = dtDetail.Rows[i]["CustName"].ToString();       //*歸戶姓名
                            tmpCardBaseInfo.name1 = dtDetail.Rows[i]["Card1Name"].ToString();         //*客戶姓名1
                            tmpCardBaseInfo.name2 = dtDetail.Rows[i]["Card2Name"].ToString();         //*客戶姓名2
                            tmpCardBaseInfo.branch_id = dtDetail.Rows[i]["branch_id"].ToString();     //*分行代碼
                            tmpCardBaseInfo.monlimit = dtDetail.Rows[i]["Month_Limit"].ToString();    //*信用額度
                            tmpCardBaseInfo.is_LackCard = "0";                                        //*缺卡狀態(缺卡)
                            if (bForWeb)
                            {
                                tmpCardBaseInfo.indate1 = strInDate;
                                tmpCardBaseInfo.trandate = strTranDate;
                            }

                            SetCardBaseInfo.Add(tmpCardBaseInfo);
                        }
                        break;
                    case "2"://金融卡
                        for (int i = 0; i < dtDetail.Rows.Count; i++)
                        {
                            //*明細欄位設定
                            TCardBaseInfo.indate1 = dt.ToString("yyyy/MM/dd");                      //*製卡日
                            TCardBaseInfo.action = dtDetail.Rows[i]["ACTION"].ToString();           //*卡別
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
                            TCardBaseInfo.cardtype = dtDetail.Rows[i]["CARD-TYPE"].ToString();      //*卡片種類
                            TCardBaseInfo.photo = dtDetail.Rows[i]["PHOTO-TYPE"].ToString();        //*相片卡別
                            TCardBaseInfo.affinity = dtDetail.Rows[i]["AFFINITY-CODE"].ToString();  //*認同代碼
                            TCardBaseInfo.id = dtDetail.Rows[i]["IDNO"].ToString();                 //*身分證字號
                            TCardBaseInfo.cardno = dtDetail.Rows[i]["CARD-NO-1"].ToString().Replace("-", "");        //*卡號1
                            TCardBaseInfo.cardno2 = dtDetail.Rows[i]["CARD-NO-2"].ToString().Replace("-", "");       //*卡號2
                            TCardBaseInfo.zip = dtDetail.Rows[i]["ZIP-C"].ToString();               //*郵遞區號
                            TCardBaseInfo.add1 = dtDetail.Rows[i]["CITY"].ToString();               //*Add1
                            TCardBaseInfo.add2 = dtDetail.Rows[i]["Add1"].ToString();               //*Add2
                            TCardBaseInfo.add3 = dtDetail.Rows[i]["Add2"].ToString();               //*Add3
                            TCardBaseInfo.mailno = "";                                              //*掛號號碼
                            TCardBaseInfo.n_card = JobHelper.GetCntcard(dtDetail.Rows[i]["AffinityCode"].ToString());    //卡數
                            TCardBaseInfo.maildate = "";                                            //*郵遞日期
                            TCardBaseInfo.expdate = dtDetail.Rows[i]["CARD-1-EXP-DATE"].ToString(); //*有效期1
                            TCardBaseInfo.expdate2 = dtDetail.Rows[i]["CARD-2-EXP-DATE"].ToString();//*有效期2
                            TCardBaseInfo.seq = dtDetail.Rows[i]["SEQ"].ToString();                 //*序號
                            TCardBaseInfo.custname = dtDetail.Rows[i]["CUST-NAME"].ToString();      //*歸戶姓名
                            TCardBaseInfo.name1 = dtDetail.Rows[i]["CARD-1-NAME"].ToString();       //*客戶姓名1
                            TCardBaseInfo.name2 = dtDetail.Rows[i]["CARD-2-NAME"].ToString();       //*客戶姓名2
                            TCardBaseInfo.branch_id = dtDetail.Rows[i]["BRANCH-IN"].ToString();     //*分行代碼
                            TCardBaseInfo.monlimit = dtDetail.Rows[i]["MONTH-LIMIT"].ToString();    //*信用額度
                            TCardBaseInfo.is_LackCard = "0";                                        //*缺卡狀態(缺卡)
                            if (bForWeb)
                            {
                                TCardBaseInfo.indate1 = strInDate;
                                TCardBaseInfo.trandate = strTranDate;
                            }

                            SetCardBaseInfo.Add(TCardBaseInfo);
                        }
                        break;
                }
                string strMsgID = string.Empty;


                blnResult = BRM_TCardBaseInfo.Insert(SetCardBaseInfo, ref strMsgID);
                break;

            case "DE"://*取消缺卡大宗檔
                //要更新異動檔的資料
                string strcard_file = TCardBaseInfo.card_file;                    //*匯入檔名
                strcard_file = strcard_file.Substring(0, 8) + strcard_file.Substring(14, strcard_file.Length - 20) + "-1.TXT";
                blnResult = BRM_TCardBaseInfo.BatUpdateFor0103(dtDetail, strCardType, strcard_file);
                break;
        }
        return blnResult;
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
                return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
            case "P":
                if (BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
                {
                    return DateTime.Now.ToString("yyyyMMdd");
                }
                else
                {
                    return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString();
                }
            default:
                if (BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
                {
                    return DateTime.Now.ToString("yyyyMMdd");
                }
                else
                {
                    return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString();
                }
        }

    }
    #endregion

    #region 篩選要下載的檔案
    private ArrayList GetDownList(string[] strFileList, string strFtpFileName)
    {
        ArrayList arrFileList = new ArrayList();
        for (int i = 0; i < strFileList.Length; i++)
        {
            //如果文件名的長度與不符，則下一筆
            //多增加時分秒的長度近來判斷(+6)
            if (strFileList[i].Length != strFtpFileName.Length + 4 + 8 + 6)
            {
                continue;
            }
            if (strFileList[i].ToLower().LastIndexOf(".zip") < 0)
            {
                continue;
            }
            string strTmp = strFileList[i].ToLower().Remove(strFileList[i].ToLower().LastIndexOf(".zip"));
            //if (strTmp.Length < strFtpFileName.Length)
            //{
            //    continue;
            //}
            if (strTmp.ToLower().Substring(strTmp.Length - strFtpFileName.Length, strFtpFileName.Length).Equals(strFtpFileName.ToLower()))
            {
                arrFileList.Add(strFileList[i]);
            }
        }
        return arrFileList;
    }
    #endregion

}
