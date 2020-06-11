//******************************************************************
//*  功能說明：自動化無法製卡檔匯入
//*  作    者：Simba Liu
//*  創建日期：2010/05/26
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
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
using Quartz.Impl;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Common.Utility;
using System.Text;


public class AutoImportNoCardFiles : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId = string.Empty;//* "0106";
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
    protected int SCount;
    protected int FCount;
    protected string strAmOrPm;
    protected string strLocalPath = string.Empty;//*ConfigurationManager.AppSettings["DownloadFilePath"] + "0106";
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;

    string strFtpIp = string.Empty;
    string strFtpUserName = string.Empty;
    string strFtpPwd = string.Empty;
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
            string strTemp = Resources.JobResource.Job0000106;
            #region 記錄job啟動時間
            StartTime = DateTime.Now;
            #endregion

            #region load jobid and LocalPath
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
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
            dtLocalFile.Columns.Add("ZipStates");          //解壓狀態
            dtLocalFile.Columns.Add("ZipPwd");             //解壓縮密碼
            dtLocalFile.Columns.Add("FormatStates");       //格式判斷狀態
            dtLocalFile.Columns.Add("CheckStates");        //數據格式驗證狀態
            dtLocalFile.Columns.Add("ImportStates");       //資料匯入狀態
            #endregion

            #region 記錄下載的壓縮檔
            ArrayList Array = new ArrayList();
            #endregion

            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId).Equals("") || JobHelper.SerchJobStatus(strJobId).Equals("0"))
            {
                return;
                //*job停止
            }
            #endregion

            #region 檢測JOB是否在執行中
            if (BRM_LBatchLog.JobStatusChk(strFunctionKey, strJobId, DateTime.Now))
            {
                // 返回不在執行           
                return;
            }
            else
            {
                BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, "R", "開始執行");
            }
            #endregion

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000106, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty;
            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                if (dtFileInfo.Rows.Count > 0)
                {
                    //*創建子目錄，存放下載文件
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
                        strLocalPath = ConfigurationManager.AppSettings["FileDownload"] + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //FTP 檔名
                        string strFileInfo = rowFileInfo["FtpFileName"].ToString() + DateTime.Now.ToString("MMdd") + ".ZIP";
                        //FTP 路徑+檔名
                        string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;

                        //*檔案存在
                        if (objFtp.isInFolderList(strFtpFileInfo))
                        {
                            //*下載檔案
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath; //本地路徑
                                row["FtpFilePath"] = strFtpFileInfo; //FTP路徑
                                row["FolderName"] = strLocalPath; //本地資料夾
                                row["ZipFileName"] = strFileInfo; //FTP壓縮檔名稱
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); ;
                                dtLocalFile.Rows.Add(row);
                            }
                        }
                        //*檔案不存在
                        else
                        {
                            // JobHelper.SaveLog(string.Format(MessageHelper.GetMessage("06_01010000_000"), rowFileInfo["FileName"].ToString()));
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job011100, rowFileInfo["FtpFileName"].ToString()));
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
                    rowLocalFile["TxtFileName"] = rowLocalFile["ZipFileName"].ToString().Replace(".ZIP", ".txt");
                }
                //*解壓失敗
                else
                {
                    errMsg += (errMsg == "" ? "" : "、") + rowLocalFile["ZipFileName"];
                    rowLocalFile["ZipStates"] = "F";
                    // ArrayList alInfo = new ArrayList();
                    // alInfo.Add(rowLocalFile["ZipFileName"]);
                    //解壓失敗發送Mail通知
                    // SendMail("1", alInfo, Resources.JobResource.Job0000106);

                }
            }
            if (errMsg != "")
            {
                ArrayList alInfo = new ArrayList();
                alInfo.Add(errMsg);
                //解壓失敗發送Mail通知
                SendMail("1", alInfo, Resources.JobResource.Job0000106);
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
            if (Row != null && Row.Length > 0)
            {
                //*讀取檔名正確資料
                for (int rowcount = 0; rowcount < Row.Length; rowcount++)
                {
                    string strFileName = Row[rowcount]["TxtFileName"].ToString();
                    string strPath = Row[rowcount]["LocalFilePath"].ToString() + strFileName;
                    string strFunctionName = Resources.JobResource.Job0000106;      //Job功能名稱
                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        int No = 0;                                //*匯入之錯誤編號
                        DataTable dtDetail = null;                 //檢核結果列表
                        //*檢核成功
                        if (UploadCheck(strPath, strFunctionName, ref No, ref arrayErrorMsg, ref dtDetail))
                        {
                            Row[rowcount]["CheckStates"] = "S";
                            //*正式匯入
                            if (ImportToDB(dtDetail, strFileName))
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
                            Row[rowcount]["CheckStates"] = "F";
                        }
                    }
                    //*file不存在local
                    else
                    {
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job010303, strPath));
                    }

                }
            }
            #endregion

            #region 成功匯入則刪除ftp上的資料
            DataRow[] RowD = dtLocalFile.Select("ZipStates='S' and FormatStates='S' and ImportStates='S'");
            for (int m = 0; m < RowD.Length; m++)
            {
                objFtp.Delete(RowD[m]["FtpFilePath"].ToString());//*路徑未設置
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
            //strReturnMsg += MessageHelper.GetMessage("06_01010000_004") + SCount;
            //strReturnMsg += MessageHelper.GetMessage("06_01010000_005") + FCount + "!";
            strReturnMsg += Resources.JobResource.Job010304 + SCount;
            strReturnMsg += Resources.JobResource.Job010305 + FCount + "!";
            if (RowF != null && RowF.Length > 0)
            {
                string strTemps = string.Empty;
                for (int k = 0; k < RowF.Length; k++)
                {
                    strTemps += RowF[k]["TxtFileName"].ToString() + "  ";
                }
                strReturnMsg += Resources.JobResource.Job010306 + strTemp;

                ArrayList alInfo = new ArrayList();
                alInfo.Add(strTemps);
                alInfo.Add(sbErrorInfo);
                //內容格式錯誤
                SendMail("3", alInfo, Resources.JobResource.Job0000036);
            }
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
            #endregion
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
            string strFrom = ConfigurationManager.AppSettings["MailSender"];
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
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000106, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000106, strErrorName);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
                case "3":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');
                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000106, alMailInfo[0]);
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
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000106, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], strErrorName);
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
    /// 創建時間:2010/05/26
    /// 修改記錄:
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="strTpye"></param>
    /// <returns></returns>
    public bool UploadCheck(string strPath, string strFileName, ref int No, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
    {
        bool blnResult = true;
        #region base parameter
        string strUserID = string.Empty;
        string strFunctionKey = "06";
        string strUploadID = "06010600";
        DateTime dtmThisDate = DateTime.Now;
        int intMax = 15000;
        string strMsgID = string.Empty;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        #endregion
        dtDetail = AutoImportFiles.UploadCheck("", strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, strFileName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

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
    /// <summary>
    /// 功能說明:匯入資料至DB
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/26
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strFileName"></param>
    /// <returns></returns>
    public bool ImportToDB(DataTable dtDetail, string strFileName)
    {
        //*資料寫入無法製卡檔
        EntitySet<EntityLayer.Entity_UnableCard> SetUnableCard = new EntitySet<EntityLayer.Entity_UnableCard>();

        //*自動化無法製卡檔匯入時判斷是否存在卡片基本信息表
        for (int j = 0; j < dtDetail.Rows.Count; j++)
        {
            Entity_CardBaseInfo TCardBaseInfo = new Entity_CardBaseInfo();
            DataRow row = dtDetail.Rows[j];
            TCardBaseInfo.action = row["Action"].ToString();
            TCardBaseInfo.cardno = row["CardNo"].ToString().Replace("-", "");
            TCardBaseInfo.indate1 = row["indate1"].ToString().Replace(".", "/");

            //判斷是否有重復資料
            bool isRepeat = BRM_UnableCard.IsRepeat(TCardBaseInfo);
            if (!isRepeat)
            {
                EntityLayer.Entity_UnableCard UnableCard = new EntityLayer.Entity_UnableCard();
                //*匯入明細欄位設定
                UnableCard.indate1 = row["indate1"].ToString().Replace(".", "/");           //*無法製卡日
                UnableCard.id = row["id"].ToString();                                      //*身份證字號
                UnableCard.CustName = row["CustName"].ToString();                          //*姓名
                UnableCard.CardNo = row["CardNo"].ToString().Replace("-", "");              //*卡號
                UnableCard.blockcode = row["blockcode"].ToString();                        //*無法製卡原因
                UnableCard.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");       //*當前系統日期
                UnableCard.ImportFileName = strFileName;                           //*當前無法製卡檔檔名
                UnableCard.OutputFlg = "N";                                        //*轉出狀態
                UnableCard.OutputDate = "";                                        //*轉出時間
                UnableCard.Action = row["Action"].ToString();                      //*卡別
                SetUnableCard.Add(UnableCard);
            }
            else
            {
                JobHelper.SaveLog(strFileName + "： " + Resources.JobResource.Job0000046);
            }
        }
        string strMsgID = string.Empty;
        return BRM_UnableCard.BatInsertFor0106(SetUnableCard, ref strMsgID);
    }
    #endregion

}
