//******************************************************************
//*  功能說明：自動化認同代碼匯入
//*  作    者：Simba Liu
//*  創建日期：2010/06/07
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
using System.Text;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;


public class AutoImportAffinity : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strAmOrPm;
    protected string strLocalPath = string.Empty;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;

    protected string strFtpIp = string.Empty;
    protected string strFtpUserName = string.Empty;
    protected string strFtpPwd = string.Empty;

    protected FTPFactory objFtp;
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
            dtLocalFile.Columns.Add("ZipFileName");        //壓縮資料檔名
            dtLocalFile.Columns.Add("TxtFileName");        //解壓後資料檔名
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000116, "IM");

            #region 登陸ftp下載檔案

            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                if (dtFileInfo.Rows.Count > 0)
                {
                    //*創建子目錄，存放下載文件
                    string strFolderName = string.Empty;
                    JobHelper.CreateFolderName(strJobId, ref strFolderName);
                    String errMsg = "";
                    //*下載自動化認同代碼檔名
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();
                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                        //本地路徑
                        strLocalPath = ConfigurationManager.AppSettings["FileDownload"] + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //FTP 檔名
                        string strFileInfo = rowFileInfo["FtpFileName"].ToString() + ".TXT";
                        //FTP 路徑+檔名
                        string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;

                        //*檔案存在
                        if (objFtp.isInFolderList(strFtpFileInfo)) //*路徑待確認
                        {
                            //*下載檔案
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath;
                                row["FtpFilePath"] = strFtpFileInfo;
                                row["FolderName"] = strLocalPath;
                                row["ZipFileName"] = strFileInfo;
                                //row["TxtFileName"] = rowFileInfo["FtpFileName"].ToString();
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString());
                                dtLocalFile.Rows.Add(row);
                            }
                            else
                            {
                                errMsg += (errMsg == "" ? "" : "、") + rowFileInfo["FtpFileName"].ToString();
                                // ArrayList alInfo = new ArrayList();
                                // alInfo.Add(rowFileInfo["FtpFileName"].ToString());
                                //下載失敗發送Mail通知
                                // SendMail("1", alInfo, Resources.JobResource.Job0000043);
                            }
                        }
                        //*檔案不存在
                        else
                        {
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job011600, rowFileInfo["FtpFileName"].ToString()));
                        }
                    }
                    if (errMsg != "")
                    {
                        ArrayList alInfo = new ArrayList();
                        alInfo.Add(errMsg);
                        //下載失敗發送Mail通知
                        SendMail("1", alInfo, Resources.JobResource.Job0000043);
                    }
                }
            }

            #endregion

            #region 處理本地壓縮檔
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                //int ZipCount = 0;
                //bool blnResult = JobHelper.ZipExeFile(strLocalPath, strLocalPath + rowLocalFile["ZipFileName"].ToString(), rowLocalFile["ZipPwd"].ToString(), ref ZipCount);
                ////*解壓成功
                //if (blnResult)
                //{
                rowLocalFile["ZipStates"] = "S";
                //    rowLocalFile["TxtFileName"] = rowLocalFile["ZipFileName"].ToString().Replace(".ZIP", ".txt");
                //    JobHelper.SaveLog(DateTime.Now.ToString() + " 解壓縮檔案成功！");
                //}
                ////*解壓失敗
                //else
                //{
                //    ArrayList alInfo = new ArrayList();
                //    alInfo.Add(rowLocalFile["ZipFileName"]);
                //    //解壓失敗發送Mail通知
                //    SendMail("1", alInfo, Resources.JobResource.Job0000002);
                //    rowLocalFile["ZipStates"] = "F";
                //    JobHelper.SaveLog(DateTime.Now.ToString() + " 解壓縮檔案失敗！");
                //}
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
                        rowLocalFile["ImportStates"] = "F";
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
            DataRow[] Row = dtLocalFile.Select("FormatStates='S'");
            if (Row != null && Row.Length > 0)
            {
                //*讀取檔名正確資料
                for (int rowcount = 0; rowcount < Row.Length; rowcount++)
                {
                    string strFileName = Row[rowcount]["ZipFileName"].ToString();
                    string strPath = Row[rowcount]["LocalFilePath"].ToString() + strFileName;

                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        int No = 0;                                //*匯入之錯誤編號
                        ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                        DataTable dtDetail = null;                 //檢核結果列表
                        //*檢核成功
                        if (UploadCheck(strPath, strFileName, "", ref No, ref arrayErrorMsg, ref dtDetail))
                        {
                            Row[rowcount]["CheckStates"] = "S";
                            //*正式匯入
                            if (ImportToDB(dtDetail))
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
                            Row[rowcount]["ImportStates"] = "F";
                            //*send mail
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
                            ArrayList alInfo = new ArrayList();
                            alInfo.Add(strFileName);
                            alInfo.Add(sbErrorInfo);
                            //內容格式錯誤發送Mail
                            SendMail("3", alInfo, Resources.JobResource.Job0000036);
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
            DataRow[] RowD = dtLocalFile.Select("FormatStates='S' and ImportStates='S'");
            for (int m = 0; m < RowD.Length; m++)
            {
                objFtp.Delete(RowD[m]["FtpFilePath"].ToString());//*路徑未設置
            }
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job結束日誌記錄
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
            if (FCount > 0)
            {
                // strReturnMsg = MessageHelper.GetMessage("06_01160000_003");
                strReturnMsg = Resources.JobResource.Job011603;
                JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, "F", strReturnMsg);
            }
            else
            {
                //strReturnMsg = MessageHelper.GetMessage("06_01160000_002");
                strReturnMsg = Resources.JobResource.Job011602;
                JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, "S", strReturnMsg);
            }
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
        string strUserID = string.Empty;
        string strFunctionKey = "06";
        string strUploadID = "06011600";
        DateTime dtmThisDate = DateTime.Now;
        int intMax = int.MaxValue;
        string strMsgID = string.Empty;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        #endregion

        dtDetail = AutoImportFiles.UploadCheck("", strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, Resources.JobResource.Job0000116, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

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
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <returns></returns>
    public bool ImportToDB(DataTable dtDetail)
    {
        bool blnResult = true;
        try
        {
            foreach (DataRow row in dtDetail.Rows)
            {
                string strMsgID = string.Empty;
                Entity_AffName AffName = new Entity_AffName();
                AffName.AFFID = row["AFFID"].ToString();
                AffName.AFFName = row["AFFName"].ToString();
                AffName.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");
                AffName.ImportFileName = dtLocalFile.Rows[0]["ZipFileName"].ToString();

                SqlHelper sqlhelp = new SqlHelper();
                sqlhelp.AddCondition(Entity_AffName.M_AFFID, Operator.Like, DataTypeUtils.String, AffName.AFFID);

                if (BRM_AffName.IsRepeat(AffName))
                {
                    BRM_AffName.Update(AffName, sqlhelp.GetFilterCondition(), ref strMsgID, "AFFName", "ImportDate", "ImportFileName");
                }
                else
                {
                    BRM_AffName.Insert(AffName, ref strMsgID);
                }
            }
        }
        catch (Exception exp)
        {
            blnResult = false;
            JobHelper.SaveLog(exp.Message);
        }
        return blnResult;
    }
    #endregion

}
