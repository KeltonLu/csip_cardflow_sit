//******************************************************************
//*  功能說明：自動化郵寄資訊檔匯入
//*  作    者：Simba Liu
//*  創建日期：2010/05/14
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


public class AutoBatchRegisterInfo : Quartz.IJob
{

    #region job基本參數設置
    protected string strJobId = string.Empty;//*"0111"
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strAmOrPm;
    protected string strLocalPath = string.Empty;//*ConfigurationManager.AppSettings["DownloadFilePath"] + "0111";
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
            #region 記錄job啟動時間
            StartTime = DateTime.Now;
            #endregion

            #region load jobid and LocalPath
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            strLocalPath = ConfigurationManager.AppSettings["DownloadFilePath"] + strJobId;
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000111, "IM");

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
                    //*處理大總檔檔名
                    foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                    {
                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();
                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                        string[] arrFileList = objFtp.GetFileList(rowFileInfo["FtpPath"].ToString());

                        if (null != arrFileList && arrFileList.Length > 0)
                        {
                            //本地路徑
                            strLocalPath = ConfigurationManager.AppSettings["FileDownload"] + "\\" + strJobId + "\\" + strFolderName + "\\";

                            foreach (string strFile in arrFileList)
                            {
                                if (!string.IsNullOrEmpty(strFile) && strFile.ToString().Trim().Length == 38)
                                {
                                    string[] strFlg = strFile.Split('-');
                                    string strDay = DateTime.Now.ToString("yyMMdd");
                                    if (null != strFlg && strFlg.Length > 0)
                                    {
                                        if (strFlg[3].Equals(strDay))
                                        {
                                            //FTP 路徑+檔名
                                            string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFile;

                                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFile))
                                            {
                                                //*記錄下載的檔案信息
                                                DataRow row = dtLocalFile.NewRow();
                                                row["LocalFilePath"] = strLocalPath;
                                                row["FtpFilePath"] = strFtpFileInfo;
                                                row["FolderName"] = strLocalPath;
                                                row["ZipFileName"] = strFile;
                                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString());
                                                dtLocalFile.Rows.Add(row);
                                            }

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
                bool blnResult = JobHelper.ZipExeFile(strLocalPath, strLocalPath + rowLocalFile["ZipFileName"].ToString(), rowLocalFile["ZipPwd"].ToString(), ref ZipCount);
                ////*解壓成功
                if (blnResult)
                {
                    rowLocalFile["ZipStates"] = "S";
                    rowLocalFile["FormatStates"] = "S";
                    rowLocalFile["TxtFileName"] = rowLocalFile["ZipFileName"].ToString().Replace(".ZIP", ".TXT");
                }
                //*解壓失敗
                else
                {
                    errMsg += (errMsg == "" ? "" : "、") + rowLocalFile["ZipFileName"].ToString();
                    rowLocalFile["ZipStates"] = "F";
                    rowLocalFile["FormatStates"] = "F";
                    // SendMail(rowLocalFile["ZipFileName"].ToString());
                }
            }
            if (errMsg != "")
            {
                SendMail(errMsg);
            }
            #endregion

            #region txt格式判斷
            //*讀取folder中的txt檔案并判斷格式
            foreach (DataRow rowLocalFile in dtLocalFile.Rows)
            {
                string[] FileArray = FileTools.GetFileList(rowLocalFile["LocalFilePath"].ToString());
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
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job011100, rowLocalFile["FtpFileName"].ToString()));
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

                    //*file存在local
                    if (File.Exists(strPath))
                    {
                        //*正式匯入
                        if (CheckAndImport(strPath, strFileName))
                        {
                            string strMsgID = string.Empty;
                            Entity_CPSFileName CPSFileName = new Entity_CPSFileName();
                            CPSFileName.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");
                            CPSFileName.ImportFileName = strFileName.Replace(".txt", "").Replace(".TXT", "");
                            if (!BRM_CPSFileName.IsRepeat(CPSFileName))
                            {
                                BRM_CPSFileName.Insert(CPSFileName, ref strMsgID);
                            }

                            Row[rowcount]["ImportStates"] = "S";
                            //JobHelper.SaveLog(string.Format(MessageHelper.GetMessage("06_01110000_002"), strFileName));
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job011102, strFileName), LogState.Info);
                        }
                        else
                        {
                            Row[rowcount]["ImportStates"] = "F";
                            //JobHelper.SaveLog(string.Format(MessageHelper.GetMessage("06_01110000_003"), strFileName));
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job011103, strFileName));
                        }
                    }
                    //*file不存在local
                    else
                    {
                        Row[rowcount]["ImportStates"] = "F";
                        //JobHelper.SaveLog(string.Format(MessageHelper.GetMessage("06_01010000_003"), strPath));
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job010303, strPath));
                    }

                }
            }
            #endregion

            #region 成功匯入則刪除ftp上的資料
            DataRow[] RowD = dtLocalFile.Select("ZipStates='S' and FormatStates='S' and ImportStates='S'");
            for (int m = 0; m < RowD.Length; m++)
            {
                objFtp.Delete(RowD[m]["FtpFilePath"].ToString());
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

    #region 解壓檔失敗mail通知
    /// <summary>
    /// 功能說明:解壓檔失敗mail通知
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strMessage"></param>
    public void SendMail(string strMessage)
    {
        DataTable dtCallMail = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityM_CallMail.M_JobID, Operator.Equal, DataTypeUtils.String, strJobId);
        sqlhelp.AddCondition(EntityM_CallMail.M_ConditionID, Operator.Equal, DataTypeUtils.String, "5");
        if (BRM_CallMail.SearchMailByNo(sqlhelp.GetFilterCondition(), ref dtCallMail, ref strMsgID))
        {
            string strFrom = ConfigurationManager.AppSettings["MailSender"];
            string[] strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
            string strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
            string strBody = dtCallMail.Rows[0]["MailContext"].ToString();
            JobHelper.SendMail(strFrom, strTo, strSubject, strBody);
        }
    }
    #endregion

    #region 壓縮包檔案格式錯誤mail通知
    /// <summary>
    /// 功能說明:壓縮包檔案格式錯誤
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strMessage"></param>
    public void SendMail(string strZipName, string strFileName)
    {
        DataTable dtCallMail = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityM_CallMail.M_JobID, Operator.Equal, DataTypeUtils.String, strJobId);
        sqlhelp.AddCondition(EntityM_CallMail.M_ConditionID, Operator.Equal, DataTypeUtils.String, "5");
        if (BRM_CallMail.SearchMailByNo(sqlhelp.GetFilterCondition(), ref dtCallMail, ref strMsgID))
        {
            string strFrom = ConfigurationManager.AppSettings["MailSender"];
            string[] strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
            string strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
            string strBody = dtCallMail.Rows[0]["MailContext"].ToString();
            JobHelper.SendMail(strFrom, strTo, strSubject, strBody);
        }
    }
    #endregion

    #region 匯入db錯誤mail通知
    /// <summary>
    /// 功能說明:匯入db錯誤mail通知
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strMessage"></param>
    public void SendMail(string strFileName, string strImportTime, int No)
    {
        DataTable dtCallMail = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityM_CallMail.M_JobID, Operator.Equal, DataTypeUtils.String, strJobId);
        sqlhelp.AddCondition(EntityM_CallMail.M_ConditionID, Operator.Equal, DataTypeUtils.String, "5");
        if (BRM_CallMail.SearchMailByNo(sqlhelp.GetFilterCondition(), ref dtCallMail, ref strMsgID))
        {
            string strFrom = ConfigurationManager.AppSettings["MailSender"];
            string[] strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
            string strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
            string strBody = dtCallMail.Rows[0]["MailContext"].ToString();
            JobHelper.SendMail(strFrom, strTo, strSubject, strBody);
        }
    }
    #endregion

    #region 檢查并匯入資料至DB
    /// <summary>
    /// 功能說明:檢查并匯入資料至DB
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strPath"></param>
    /// <returns></returns>
    public bool CheckAndImport(string strPath, string strImpFile)
    {
        bool blnResult = true;
        try
        {
            string[] strContent = JobHelper.Read(strPath);
            foreach (string strItem in strContent)
            {
                string Send_status_Code = "";
                switch (strItem.Substring(0, 3))
                {
                    //*投遞不成功
                    //case "316":
                    //    Import(strItem, "316", strImpFile);
                    //    break;
                    case "223":   //*投遞成功 
                        Send_status_Code = strItem.Substring(46, 2);
                        if (Send_status_Code == "I1" || Send_status_Code == "I2" || Send_status_Code == "I4")  //I1 & I2 & I4(投遞成功)
                        {
                            Send_status_Code = "";
                            Import(strItem, "Other", strImpFile);
                        }
                        break;
                    case "240":   //*招領中
                        // case "247":   //*註銷投遞成功記錄
                        // case "258":   //*退件        
                        Send_status_Code = strItem.Substring(46, 2);
                        if (Send_status_Code == "G2")  //G2(招領中)
                        {
                            Send_status_Code = "";
                            Import(strItem, "Other", strImpFile);
                        }
                        break;
                }
            }
        }
        catch (Exception exp)
        {
            JobHelper.SaveLog(exp.Message);
            blnResult = false;
        }
        return blnResult;
    }
    #endregion

    #region 匯入資料至DB
    /// <summary>
    /// 功能說明:匯入資料至DB
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strItem"></param>
    /// <param name="strSendStatus"></param>
    /// <param name="strImpFile"></param>
    public void Import(string strItem, string strSendStatus, string strImpFile)
    {
        string strMsgID = string.Empty;

        //*郵局交寄資訊檔
        EntityM_PostSend PostSend = new EntityM_PostSend();
        string strMailDate = DateTime.ParseExact(strItem.Substring(6, 8), "yyyyMMdd", null).ToString("yyyy/MM/dd");
        PostSend.maildate = strMailDate;                             //*交寄日期
        string strMailno = strItem.Substring(26, 20);
        PostSend.mailno = strMailno;                                 //*郵寄號碼(取消取前六碼)
        PostSend.Info1 = strItem.Substring(0, 3);                    //*資訊代碼1
        PostSend.Info2 = strItem.Substring(3, 3);                    //*資訊代碼2
        PostSend.Send_status_Code = strItem.Substring(46, 2);        //*郵件狀態代碼
        PostSend.Send_status_Name = strItem.Substring(48, 15);       //*郵件狀態中文
        string strMdate = DateTime.ParseExact(strItem.Substring(143, 14), "yyyyMMddHHmmss", null).ToString("yyyy/MM/dd HH:mm:ss");
        PostSend.M_date = strMdate;                                  //*處理日期時間
        PostSend.Post_Code = strItem.Substring(63, 6);               //*郵局局號
        PostSend.Post_Name = strItem.Substring(69, 10);              //*郵局局名
        PostSend.Post_TEL = strItem.Substring(79, 18);               //*郵局電話
        PostSend.Post_ADDR = strItem.Substring(108, 35);              //*郵局地址
        if (strSendStatus.Equals("316"))
        {
            PostSend.M_Code = strItem.Substring(157, 1);             //*處理方式代碼
            PostSend.M_Name = strItem.Substring(158, 10);            //*處理方式中文
            PostSend.Non_Send_Code = strItem.Substring(168, 2);      //*未妥投原因代碼
            PostSend.Non_Send_Name = strItem.Substring(170, 10);     //*未妥投原因中文
        }
        PostSend.Imp_date = DateTime.Now.ToString("yyyy/MM/dd");     //*匯入日期
        PostSend.Imp_Time = DateTime.Now.ToString("HH:mm:ss");       //*匯入時間
        PostSend.Imp_file = strImpFile;                              //*匯入檔名
        PostSend.Imp_code = "00";                                    //*匯入人員代號
        PostSend.Imp_name = "sys";                                  //*匯入人員姓名
        PostSend.Exp_Count = "0";

        //*郵局交寄資訊匯入失敗檔
        EntityM_PostSendF PostSendF = new EntityM_PostSendF();
        string strMailDateF = DateTime.ParseExact(strItem.Substring(6, 8), "yyyyMMdd", null).ToString("yyyy/MM/dd");
        PostSendF.maildate = strMailDateF;                            //*交寄日期
        string strMailnoF = strItem.Substring(26, 20);
        PostSendF.mailno = strMailnoF;                                //*郵寄號碼(取消取前六碼)
        PostSendF.Info1 = strItem.Substring(0, 3);                    //*資訊代碼1
        PostSendF.Info2 = strItem.Substring(3, 3);                    //*資訊代碼2
        PostSendF.Send_status_Code = strItem.Substring(46, 2);        //*郵件狀態代碼
        PostSendF.Send_status_Name = strItem.Substring(48, 15);       //*郵件狀態中文
        string strMdateF = DateTime.ParseExact(strItem.Substring(143, 14), "yyyyMMddHHmmss", null).ToString("yyyy/MM/dd HH:mm:ss");
        PostSendF.M_date = strMdateF;                                 //*處理日期時間
        PostSendF.Post_Code = strItem.Substring(63, 6);               //*郵局局號
        PostSendF.Post_Name = strItem.Substring(69, 10);              //*郵局局名
        PostSendF.Post_TEL = strItem.Substring(79, 18);               //*郵局電話
        PostSendF.Post_ADDR = strItem.Substring(108, 35);             //*郵局地址
        if (strSendStatus.Equals("316"))
        {
            PostSendF.M_Code = strItem.Substring(157, 1);             //*處理方式代碼
            PostSendF.M_Name = strItem.Substring(158, 10);            //*處理方式中文
            PostSendF.Non_Send_Code = strItem.Substring(168, 2);      //*未妥投原因代碼
            PostSendF.Non_Send_Name = strItem.Substring(170, 10);     //*未妥投原因中文
        }
        PostSendF.Imp_date = DateTime.Now.ToString("yyyy/MM/dd");     //*匯入日期
        PostSendF.Imp_Time = DateTime.Now.ToString("HH:mm:ss");       //*匯入時間
        PostSendF.Imp_file = strImpFile;                              //*匯入檔名
        PostSendF.Imp_code = "00";                                    //*匯入人員代號
        PostSendF.Imp_name = "系統";                                  //*匯入人員姓名

        if (string.IsNullOrEmpty(strMailDate) || string.IsNullOrEmpty(strMailno) || string.IsNullOrEmpty(strMdate))
        {
            return;
        }

        //*根據郵寄日與掛號號碼前6碼判断是否有重復的卡片資料
        Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
        CardBaseInfo.maildate = strMailDate;
        CardBaseInfo.mailno = strMailno.Trim();
        SqlHelper SqlCard = new SqlHelper();
        SqlCard.AddCondition(Entity_CardBaseInfo.M_maildate, Operator.Equal, DataTypeUtils.String, CardBaseInfo.maildate);
        SqlCard.AddCondition(Entity_CardBaseInfo.M_mailno, Operator.Equal, DataTypeUtils.String, CardBaseInfo.mailno);
        String strFilterCardBaseInfo = " maildate = '" + strMailDate + "' and  mailno = '" + strMailno.Trim() + "'";
        bool blnB = BRM_TCardBaseInfo.IsRepeatFor0111_1(strFilterCardBaseInfo);

        //*卡片基本資料表無對應資料
        if (!blnB)
        {
            PostSendF.F_Type = "0";
            BRM_PostSendF.Insert(PostSendF, ref strMsgID);
            return;
        }

        //*根據交寄日期郵件號碼處理日期時間判断是否有重復的郵局交寄資訊檔
        EntityM_PostSend PostSendS = new EntityM_PostSend();
        PostSendS.maildate = strMailDate;
        PostSendS.mailno = strMailno.Trim();
        PostSendS.M_date = strMdate;
        String strFilterPostSend = " maildate = '" + strMailDate + "' and  mailno = '" + strMailno.Trim() + "'";
        bool blnP = BRM_PostSend.IsRepeatFor0111_1(strFilterPostSend);

        //*重復郵寄資訊
        if (blnP)
        {
            PostSendF.F_Type = "1";
            BRM_PostSendF.Insert(PostSendF, ref strMsgID);
            return;
        }

        //*卡片基本資料表有對應資料，PostSend無對應資料
        if (blnB && !blnP)
        {
            BRM_PostSend.Insert(PostSend, ref strMsgID);
        }
    }
    #endregion
}
