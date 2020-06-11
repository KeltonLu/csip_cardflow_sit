//******************************************************************
//*  功能說明：自動化簡訊處理-主機回覆檔匯入
//*  作    者：zhiyuan
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
using System.Text;
using Quartz;
using Quartz.Impl;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using BusinessRulesNew;

/// <summary>
/// AutoNewsletterInfoMFBack 的摘要描述
/// </summary>
public class AutoNewsletterInfoMFBack : Quartz.IJob
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

    protected FTPFactory objFtp;
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
            //strJobId = "0115_2";
            #region 記錄job啟動時間
            StartTime = DateTime.Now;
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
            dtLocalFile.Columns.Add("FtpFileInfo");        //Ftp全路徑+路徑
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job2000115, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty;//*本地存放目錄(格式為yyyyMMdd+am/pm)
            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                if (dtFileInfo.Rows.Count > 0)
                {
                    //*創建子目錄名稱，存放下載文件
                    string strMsg = string.Empty;
                    JobHelper.CreateFolderName(strJobId, ref strFolderName);
                    String errMsg = "";
                    //*處理大總檔檔名
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        //本地路徑
                        strLocalPath = ConfigurationManager.AppSettings["FileDownload"] + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //FTP 檔名
                        //為了排除工作日的問題，抓取前一個工作日
                        // string  strImportDate = "";
                        // strImportDate = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd");

                        //string strFileInfo = rowFileInfo["FtpFileName"].ToString() + DateTime.Now.AddDays(-1).ToString("MMdd") + ".TXT";
                        string strFileInfo = rowFileInfo["FtpFileName"].ToString() + DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("MMdd") + ".TXT";
                        //FTP 路徑+檔名
                        string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;
                        string strFtpIp = rowFileInfo["FtpIP"].ToString();
                        string strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        string strFtpPwd = rowFileInfo["FtpPwd"].ToString();
                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                        //*檔案存在
                        if (objFtp.isInFolderList(strFtpFileInfo))
                        {
                            //*下載檔案
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath; //本地路徑
                                row["FtpFilePath"] = rowFileInfo["FtpPath"].ToString(); //FTP路徑
                                row["FtpFileInfo"] = strFtpFileInfo; //FTP路徑+檔名
                                row["FolderName"] = strJobId + DateTime.Now.ToString("yyyyMMddhhmmss"); //本地資料夾
                                row["ZipFileName"] = strFileInfo; //FTP壓縮檔名稱
                                row["ZipPwd"] = rowFileInfo["ZipPwd"].ToString(); //FTP壓縮檔密碼
                                row["CardType"] = rowFileInfo["CardType"].ToString(); //卡片種類
                                row["MerchCode"] = rowFileInfo["MerchCode"].ToString(); //製卡廠代碼
                                row["MerchName"] = rowFileInfo["MerchName"].ToString(); //製卡廠名稱
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //解密
                                //row["Trandate"] = GetTrandate(rowFileInfo["AMPMFlg"].ToString()); //轉檔日設定
                                //加密 RedirectHelper.GetEncryptParam(rowFileInfo["ZipPwd"].ToString());
                                dtLocalFile.Rows.Add(row);
                            }
                        }
                        //*檔案不存在
                        else
                        {
                            errMsg += (errMsg == "" ? "" : "、") + rowFileInfo["FtpFileName"].ToString();
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job2010115, rowFileInfo["FtpFileName"].ToString()));
                            // SendMail(rowFileInfo["FtpFileName"].ToString(), Resources.JobResource.Job0000008);
                        }
                    }
                    if (errMsg != "")
                    {
                        SendMail(errMsg, Resources.JobResource.Job0000008);
                    }
                }
            }
            #endregion

            #region 處理本地壓縮檔
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                //int ZipCount = 0;
                //*******update arj ***************//
                string strZipFileName = rowLocalFile["ZipFileName"].ToString().Trim();
                //    bool blnResult = ExeFile(strLocalPath, strZipFileName, rowLocalFile["ZipPwd"].ToString());
                //    ////*解壓成功
                //    if (blnResult)
                //    {
                rowLocalFile["ZipStates"] = "S";
                rowLocalFile["FormatStates"] = "S";
                rowLocalFile["TxtFileName"] = strZipFileName.Replace(".TXT", ".txt");
                //    }
                //    //*解壓失敗
                //    else
                //    {
                //        rowLocalFile["ZipStates"] = "F";
                //        rowLocalFile["FormatStates"] = "F";
                //        SendMail(rowLocalFile["ZipFileName"].ToString(), Resources.JobResource.Job0000002);
                //    }
                //*******update arj ***************//
                //bool blnResult = JobHelper.ZipExeFile(strLocalPath, strLocalPath + rowLocalFile["ZipFileName"].ToString(), rowLocalFile["ZipPwd"].ToString(), ref ZipCount);
                //////*解壓成功
                //if (blnResult)
                //{
                //    rowLocalFile["ZipStates"] = "S";
                //    rowLocalFile["FormatStates"] = "S";
                //    rowLocalFile["TxtFileName"] = rowLocalFile["ZipFileName"].ToString().Replace(".zip", ".txt");
                //}
                ////*解壓失敗
                //else
                //{
                //    rowLocalFile["ZipStates"] = "F";
                //    rowLocalFile["FormatStates"] = "F";
                //    SendMail(rowLocalFile["ZipFileName"].ToString(), Resources.JobResource.Job0000002);
                //}
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
                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        int No = 0;                                //*匯入之錯誤編號
                        ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
                        DataTable dtDetail = null;                 //檢核結果列表
                        string strMailErr = string.Empty;           //錯誤消息

                        //*檢核成功
                        if (UploadCheck(strPath, strFileName, ref No, ref strMailErr, ref arrayErrorMsg, ref dtDetail))
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
                objFtp.Delete(RowD[m]["FtpFileInfo"].ToString());//*路徑未設置
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
                strReturnMsg += Resources.JobResource.Job0000026;
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
            string strFrom = ConfigurationManager.AppSettings["MailSender"];
            string[] strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
            string[] strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');
            string strSubject = string.Format(dtCallMail.Rows[0]["MailTittle"].ToString(), Resources.JobResource.Job2000115, strFileName);
            string strBody = string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strFileName, Resources.JobResource.Job2000115, strCon);
            JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
        }
    }
    #endregion

    //#region 匯入資料檢核
    ///// <summary>
    ///// 功能說明:匯入資料檢核
    ///// 作    者:Simba Liu
    ///// 創建時間:2010/05/04
    ///// 修改記錄:
    ///// </summary>
    ///// <param name="strPath"></param>
    ///// <returns></returns>
    public bool UploadCheck(string strPath, string strFileName, ref int No, ref string strMsgID, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
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

        //strUploadID = "060115_200";
        strUploadID = "0115_2";
        dtDetail = BaseHelper.UploadCheck(strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, Resources.JobResource.Job2000115, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);


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
    //#endregion

    #region 匯入資料至DB
    /// <summary>
    /// 功能說明:匯入資料至DB
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strFileName"></param>
    /// <returns></returns>
    public bool ImportToDB(DataTable dtDetail, string strFileName)
    {
        bool blnResult = false;
        string Trandate = string.Empty;
        string strSql = string.Empty;

        if (BusinessRulesNew.BRM_SLetterInfoCallCust.BatUpdateFor0115_2(dtDetail, strFileName))
        {
            blnResult = true;
        }

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

        strTXTFileName = srcZipFile.ToUpper().Replace("EXE", "txt");

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
