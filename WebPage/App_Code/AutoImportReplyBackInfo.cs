//******************************************************************
//*  功能說明：自動化退件處理回饋檔匯入
//*  作    者：linda
//*  創建日期：2010/06/07
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using Framework.Common.Logging;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Common.Utility;
using Framework.Data.OM;

/// <summary>
/// AutoImportReplyBackInfo 的摘要描述
/// </summary>
public class AutoImportReplyBackInfo : Quartz.IJob
{
    #region job基本參數設置
    protected string strJobId;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string sFileName;
    protected string strLocalPath;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;

    protected FTPFactory objFtp;

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
            //strJobId = "0114";
            #endregion

            #region 获取本地路徑
            strLocalPath = UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId;
            #endregion

            #region 記錄job啟動時間
            StartTime = DateTime.Now;
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000114, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty;

            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                if (dtFileInfo.Rows.Count > 0)
                {
                    string strMsg = string.Empty;
                    //*創建子目錄，存放下載文件                    
                    strFolderName = strJobId + StartTime.ToString("yyyyMMddHHmmss");
                    //*處理退件處理回饋檔檔名
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        string strFtpIp = rowFileInfo["FtpIP"].ToString();

                        string strFtpUserName = rowFileInfo["FtpUserName"].ToString();

                        string strFtpPwd = rowFileInfo["FtpPwd"].ToString();

                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                        string[] arrFileList = objFtp.GetFileList(rowFileInfo["FtpPath"].ToString());

                        if (null != arrFileList)
                        {
                            for (int i = 0; i < arrFileList.Length - 1; i++)
                            {
                                if (!string.IsNullOrEmpty(arrFileList[i].Trim()) && arrFileList[i].Trim().Substring(0, 5).Equals("REPLY") && arrFileList[i].Trim().Length >= 23)
                                {
                                    if (arrFileList[i].Trim().Substring(13, 10).Equals("退件聯絡報表.ZIP"))
                                    {
                                        if (objFtp.Download(rowFileInfo["FtpPath"].ToString() + "//" + arrFileList[i].Trim(), strLocalPath + "\\" + strFolderName + "\\", arrFileList[i].Trim()))
                                        {
                                            //*記錄下載的檔案信息
                                            DataRow row = dtLocalFile.NewRow();
                                            row["LocalFilePath"] = strLocalPath + "\\" + strFolderName + "\\" + arrFileList[i].Trim();
                                            row["FtpFilePath"] = rowFileInfo["FtpPath"].ToString() + "//" + arrFileList[i].Trim();
                                            row["FolderName"] = strLocalPath + "\\" + strFolderName + "\\";
                                            row["ZipFileName"] = arrFileList[i].Trim();
                                            row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //FTP壓縮檔密碼解密
                                            dtLocalFile.Rows.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0114000, StartTime.ToString("yyyyMMdd")));
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
                bool blnResult = JobHelper.ZipExeFile(strLocalPath + "\\" + strFolderName, rowLocalFile["LocalFilePath"].ToString(), rowLocalFile["ZipPwd"].ToString(), ref ZipCount);
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

            #region 開始資料匯入
            DataRow[] Row = dtLocalFile.Select("ZipStates='S'");
            if (Row != null && Row.Length > 0)
            {
                //*讀取檔名正確資料
                for (int rowcount = 0; rowcount < Row.Length; rowcount++)
                {
                    string strFileName = Row[rowcount]["TxtFileName"].ToString();
                    string strPath = Row[rowcount]["FolderName"].ToString() + strFileName;
                    string strFunctionName = Resources.JobResource.Job0000114;
                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        string strMsgID = string.Empty;
                        ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                        DataTable dtDetail = null;                 //檢核結果列表

                        //*檢核成功
                        if (UploadCheck(strPath, strFunctionName, ref strMsgID, ref arrayErrorMsg, ref dtDetail))
                        {
                            Row[rowcount]["CheckStates"] = "S";
                            //*正式匯入
                            if (BRM_CardBackInfo.BackInfoUpdateFor0114(dtDetail))
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
                        //*檢核失敗
                        else
                        {
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

            #region 成功匯入則刪除ftp上的資料
            DataRow[] RowD = dtLocalFile.Select("ZipStates='S' and ImportStates='S'");
            for (int m = 0; m < RowD.Length; m++)
            {
                objFtp.Delete(RowD[m]["FtpFilePath"].ToString());//*路徑未設置
            }
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job結束日誌記錄
            //*判斷job完成狀態
            DataRow[] RowS = dtLocalFile.Select("ZipStates='S' and ImportStates='S'");
            DataRow[] RowF = dtLocalFile.Select("ZipStates='S' and ImportStates='F'");

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
        }
        catch (Exception ex)
        {
            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "CommonModel_發生錯誤_" + ex.ToString());
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
    public bool UploadCheck(string strPath, string strFileName, ref string strMsgID, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
    {
        bool blnResult = true;
        #region base parameter
        string strUserID = "";
        string strFunctionKey = "06";
        string strUploadID = "06011400";
        DateTime dtmThisDate = DateTime.Now;
        int intMax = 15000;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        #endregion

        dtDetail = BaseHelper.UploadCheck(strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, strFileName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

        //*檢核成功
        if (strMsgID == "" && arrayErrorMsg.Count == 0)
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

                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000114, alMailInfo[0]);

                    //格式化Mail Body

                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();

                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000114, strErrorName);

                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);

                    break;

                case "2":

                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');

                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

                    //格式化Mail Tittle

                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000114, alMailInfo[0]);

                    //格式化Mail Body

                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();

                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], alMailInfo[1]);

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
