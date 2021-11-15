//******************************************************************
//*  功能說明：自動化簡訊處理-簡訊檔匯出
//*  作    者：zhiyuan
//*  創建日期：2010/06/9
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Text;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using System.IO;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM;
using System.Text.RegularExpressions;
using Framework.Common.Logging;
using Framework.Common.Utility;

/// <summary>
/// AutoNewsletterInfoMsg 的摘要描述
/// </summary>
public class AutoNewsletterInfoMsg : Quartz.IJob
{
    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strPFileName = string.Empty;   //招領文件名
    protected string strBFileName = string.Empty;   //退件文件名
    protected string strPOverFileName = string.Empty;   //CTL空檔，在DAT完成後產生，目的在確認DAT已正確產出。
    protected string strBOverFileName = string.Empty;   //CTL空檔，在DAT完成後產生，目的在確認DAT已正確產出。
    protected string strAmOrPm = string.Empty;
    protected string strFolderName = string.Empty;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間;
    protected DateTime EndTime;
    protected StringBuilder sbFileInfo = new StringBuilder();
    
    private DateTime _jobDate = DateTime.Now;
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    {
        try
        {
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
            //strJobId = "0118";
            
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
            
            
            #region 記錄job啟動時間
            StartTime = DateTime.Now;
            #endregion

            #region 計數器歸零
            SCount = 0;
            FCount = 0;
            #endregion

            #region 匯出資料明細
            dtLocalFile = new DataTable();
            dtLocalFile.Columns.Add("LocalFilePath");      //本地全路徑
            dtLocalFile.Columns.Add("LocalOverFilePath");      //本地全路徑
            dtLocalFile.Columns.Add("FtpFilePath");        //Ftp全路徑
            dtLocalFile.Columns.Add("FolderName");         //目錄名稱
            dtLocalFile.Columns.Add("TxtFileName");        //資料檔名
            dtLocalFile.Columns.Add("TxtOverFileName");    //CTL空檔名，在DAT完成後產生，目的在確認DAT已正確產出
            dtLocalFile.Columns.Add("UploadStates");       //資料上載狀態
            dtLocalFile.Columns.Add("FtpPath");            //FTP路徑
            dtLocalFile.Columns.Add("FtpIP");              //FTP IP
            dtLocalFile.Columns.Add("FtpUserName");        //FTP用戶名
            dtLocalFile.Columns.Add("FtpPwd");             //FTP密碼
            #endregion

            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId) == "" || JobHelper.SerchJobStatus(strJobId) == "0")
            {
                JobHelper.SaveLog("JOB 工作狀態為：停止！", LogState.Info);
                return;
            }
            #endregion

            #region 檢測JOB今日是否為工作日，工作日才要執行
            if (!BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
            {
                JobHelper.SaveLog("JOB 非工作日：停止！", LogState.Info);
                // 返回不在執行           
                return;
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


            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job3000115, "OU");

            #region 查詢需匯出的資料
            //*無JOB交換當信息或查詢失敗
            if (!JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("無JOB交換當信息或查詢失敗！", LogState.Info);
                return;
            }

            if (dtFileInfo.Rows.Count > 0)
            {
                strFolderName = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\" + strJobId + "\\" + strJobId + _jobDate.ToString("yyyyMMddHHmmss");

                foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                {
                    //GetFileName(rowFileInfo["FtpFileName"].ToString());
                    strPFileName = "MF1.DAT";
                    strPOverFileName = "MF1.CTL";
                    strBFileName = "MR1.DAT";
                    strBOverFileName = "MR1.CTL";
                    //*執行招領
                    if (rowFileInfo["PExpFlg"].ToString().Equals("1"))
                    {
                        OutPFile("P", rowFileInfo);
                    }

                    //執行退件
                    if (rowFileInfo["BExpFlg"].ToString().Equals("1"))
                    {
                        OutPFile("B", rowFileInfo);
                    }
                }
            }
            #endregion

            #region 登陸ftp上載文件
            String errMsg = "";
            for (int j = 0; j < dtLocalFile.Rows.Count; j++)
            {
                string strFtpUploadPath = dtLocalFile.Rows[j]["FtpPath"].ToString();
                string strFtpIp = dtLocalFile.Rows[j]["FtpIP"].ToString();
                string strFtpUserName = dtLocalFile.Rows[j]["FtpUserName"].ToString();
                string strFtpPwd = dtLocalFile.Rows[j]["FtpPwd"].ToString();

                FTPFactory objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                if (objFtp.Upload(strFtpUploadPath, dtLocalFile.Rows[j]["TxtFileName"].ToString(), dtLocalFile.Rows[j]["LocalFilePath"].ToString()))
                {
                    objFtp.Upload(strFtpUploadPath, dtLocalFile.Rows[j]["TxtOverFileName"].ToString(), dtLocalFile.Rows[j]["LocalOverFilePath"].ToString());
                    //*更新上載狀態為S
                    dtLocalFile.Rows[j]["UploadStates"] = "S";
                    JobHelper.SaveLog("上傳檔案成功！", LogState.Info);
                }
                else
                {
                    errMsg += (errMsg == "" ? "" : "、") + dtLocalFile.Rows[j]["TxtFileName"].ToString();
                    //*更新上載狀態為F
                    dtLocalFile.Rows[j]["UploadStates"] = "F";
                    //*發送登陸FTP失敗郵件
                    // SendMail(dtLocalFile.Rows[j]["TxtFileName"].ToString(), Resources.JobResource.Job0000008);
                    JobHelper.SaveLog("上傳檔案失敗！", LogState.Info);
                }
            }
            if (errMsg != "")
            {
                SendMail(errMsg, Resources.JobResource.Job0000008);
            }
            #endregion

            #region 刪除本地上載成功文件
            DataRow[] rows = dtLocalFile.Select("UploadStates='S'");
            for (int k = 0; k < rows.Length; k++)
            {
                //FileTools.DeleteFile(rows[k]["LocalFilePath"].ToString());
            }
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job執行結果寫入日誌
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            WriteLogToDB();
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

    #region 將資料匯出為TXT檔
    /// <summary>
    /// 功能說明:將資料匯出為TXT檔
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="strType">P:招領 B:退件</param>
    /// <param name="strFunFlg">1:信用卡 2:VD/金融卡</param>
    //private void OutPFile(string strType, string strFunFlg, DataRow drLocalFile)
    //{
    //    DataTable dtAllData = new DataTable();
    //    DataRow[] drRows = null;
    //    StringBuilder sbFileInfo = new StringBuilder();
    //    string strTmp = string.Empty;
    //    string strFileName = string.Empty;
    //    if (strType.Equals("B"))
    //    {
    //        switch (strFunFlg)
    //        {
    //            case "1":
    //                strTmp = "A3";
    //                break;
    //            case "2":
    //                strTmp = "A9";
    //                break;
    //            case "3":
    //                strTmp = "A10";
    //                break;
    //            default:
    //                return;
    //        }
    //        strFileName = strTmp + strPFileName;
    //    }
    //    if (strType.Equals("P"))
    //    {
    //        switch (strFunFlg)
    //        {
    //            case "1":
    //                strTmp = "PO1";
    //                break;
    //            case "2":
    //                strTmp = "PO2";
    //                break;
    //            case "3":
    //                strTmp = "PO3";
    //                break;
    //            default:
    //                return;
    //        }
    //        strFileName = strTmp + strBFileName;
    //    }
    //    if (BRM_SLetterInfoCallCust.GetOutData(strType, ref dtAllData))
    //    {
    //        if (strFunFlg.Equals("1"))
    //        {
    //            //*信用卡
    //            drRows = dtAllData.Select("cardtype not in ('589651','447757','515352','589651','888888')");
    //        }
    //        else if (strFunFlg.Equals("2"))
    //        {
    //            //*VD/金融卡
    //            drRows = dtAllData.Select("cardtype = '447757','515352'");
    //        }
    //        else if (strFunFlg.Equals("3"))
    //        {
    //            //*現金卡
    //            drRows = dtAllData.Select("cardtype = '589651'");
    //        }

    //        foreach (DataRow dr in drRows)
    //        {
    //            strTmp = dr["ID"].ToString() + ",";     //身份證號碼
    //            strTmp += dr["Mobil"].ToString() + ","; //行動電話
    //            strTmp += dr["Name"].ToString().Replace(" ", "") + ","; //姓名
    //            if (strType.ToUpper().Equals("P"))
    //            {
    //                strTmp += dr["Name"].ToString().Replace(" ", "") + ","; //姓名
    //                strTmp += dr["Mailno"].ToString() + ",";  //掛號號碼
    //                strTmp += dr["Post_Name"].ToString().Replace(" ", "");   //招領郵局
    //            }
    //            else
    //            {
    //                strTmp += dr["Name"].ToString().Replace(" ", ""); //姓名
    //            }
    //            strTmp += "\r\n";
    //            sbFileInfo.Append(strTmp);
    //        }
    //        //*更新簡訊處理信息檔
    //        if (BRM_SLetterInfoCallCust.UpdCallCustFor0115_3(drRows, strFileName))
    //        {
    //        }
    //        FileTools.EnsurePath(strFolderName);
    //        FileTools.CreateAppend(strFolderName + "\\" + strFileName, sbFileInfo.ToString());
    //        DataRow row = dtLocalFile.NewRow();//*記錄文件名稱以便刪除之用
    //        row["LocalFilePath"] = strFolderName + "\\" + strFileName;
    //        row["TxtFileName"] = strFileName;
    //        row["FtpPath"] = drLocalFile["FtpPath"].ToString();
    //        row["FtpIP"] = drLocalFile["FtpIP"].ToString();
    //        row["FtpUserName"] = drLocalFile["FtpUserName"].ToString();
    //        row["FtpPwd"] = drLocalFile["FtpPwd"].ToString();
    //        dtLocalFile.Rows.Add(row);
    //    }
    //}
    //#endregion

    private void OutPFile(string strType, DataRow drLocalFile)
    {

        DataTable dtAllData = new DataTable();
        DataRow[] drRows = null;
        StringBuilder sbFileInfo = new StringBuilder();
        string strTmp = string.Empty;
        string strFileName = string.Empty;
        string strOverFileName = string.Empty;
        if (strType.Equals("B"))
        {
            JobHelper.SaveLog("開始讀取要匯出的檔案(執行退件)資料！", LogState.Info);
            strFileName = strBFileName;
            strOverFileName = strBOverFileName;
        }
        if (strType.Equals("P"))
        {
            JobHelper.SaveLog("開始讀取要匯出的檔案(執行招領)資料！", LogState.Info);
            strFileName = strPFileName;
            strOverFileName = strPOverFileName;
        }
        if (BRM_SLetterInfoCallCust.GetOutData(strType, ref dtAllData))
        {
            drRows = dtAllData.Select("cardtype <>'888888'");

            foreach (DataRow dr in drRows)
            {
                string strCardType = dr["CardType"].ToString();
                strTmp = JobHelper.SetStrngValue(dr["ID"].ToString(), 16);     //身份證號碼
                strTmp += JobHelper.SetStrngValue(dr["Mobil"].ToString(), 10); //行動電話
                //strTmp += JobHelper.SetStrngValue(dr["Name"].ToString().Replace(" ", ""), 10); //姓名
                string strName = JobHelper.SetStrngValue(dr["Name"].ToString().Replace(" ", ""), 10); //姓名

                //中文姓名加隱碼
                string pattern = @"^[\u4E00-\u9fa5]+$";
                int ibyte_len;
                if (dr["Name"].ToString().Trim() == "")
                {
                    ibyte_len = 0;
                }
                else
                {
                    ibyte_len = System.Text.Encoding.Default.GetByteCount(dr["Name"].ToString().Substring(0, 1));
                }

                if (ibyte_len > 1)  //判斷是否為中文或全形符號 
                {
                    if (Regex.IsMatch(dr["Name"].ToString().Substring(0, 1), pattern))    //判斷是否為中文
                    {
                        strTmp += strName.Substring(0, 1) + "○" + strName.Substring(2);
                    }
                    else
                    {
                        strTmp += JobHelper.SetStrngValue(strName, 10);
                    }
                }
                else
                {
                    strTmp += JobHelper.SetStrngValue(strName, 10);
                }
                switch (strCardType)
                {
                    case "447757":
                        strTmp += JobHelper.SetStrngValue("晶片金融卡", 10);
                        break;
                    case "515352":
                        strTmp += JobHelper.SetStrngValue("晶片金融卡", 10);
                        break;
                    case "553002":
                        strTmp += JobHelper.SetStrngValue("晶片金融卡", 10);
                        break;
                    case "589651":
                        strTmp += JobHelper.SetStrngValue("現金卡", 10);
                        break;
                    default:
                        strTmp += JobHelper.SetStrngValue("信用卡", 10);
                        break;
                }
                if (strType.ToUpper().Equals("P"))
                {
                    strTmp += JobHelper.SetStrngValue(dr["Mailno"].ToString(), 6).Substring(0, 6);  //掛號號碼
                    strTmp += JobHelper.SetStrngValue(dr["Post_Name"].ToString().Replace(" ", ""), 20);   //招領郵局
                    switch (strCardType)
                    {
                        case "447757":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        case "515352":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        case "553002":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        case "589651":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        default:
                            strTmp += JobHelper.SetStrngValue("帳單地址", 8);
                            break;
                    }
                }
                else
                {
                    switch (strCardType)
                    {
                        case "447757":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        case "515352":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        case "553002":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        case "589651":
                            strTmp += JobHelper.SetStrngValue("通訊地址", 8);
                            break;
                        default:
                            strTmp += JobHelper.SetStrngValue("帳單地址", 8);
                            break;
                    }
                    strTmp += JobHelper.SetStrngValue("", 26);
                }

                sbFileInfo.Append(strTmp);
            }

            //*更新簡訊處理信息檔
            if (BRM_SLetterInfoCallCust.UpdCallCustFor0115_3(drRows, strFileName))
            {
                UpdateExpCount(strType, drRows);
            }
            FileTools.EnsurePath(strFolderName);
            CreateAppend2(strFolderName + "\\" + strFileName, sbFileInfo.ToString(), Encoding.GetEncoding("big5"));
            CreateAppend2(strFolderName + "\\" + strOverFileName, "", Encoding.GetEncoding("big5"));
            DataRow row = dtLocalFile.NewRow();//*記錄文件名稱以便刪除之用
            row["LocalFilePath"] = strFolderName + "\\" + strFileName;
            row["LocalOverFilePath"] = strFolderName + "\\" + strOverFileName;
            row["TxtFileName"] = strFileName;
            row["TxtOverFileName"] = strOverFileName;
            row["FtpPath"] = drLocalFile["FtpPath"].ToString();
            row["FtpIP"] = drLocalFile["FtpIP"].ToString();
            row["FtpUserName"] = drLocalFile["FtpUserName"].ToString();
            row["FtpPwd"] = drLocalFile["FtpPwd"].ToString();
            dtLocalFile.Rows.Add(row);
        }

        JobHelper.SaveLog("結束讀取要匯出的檔案(執行招領)資料！", LogState.Info);
    }
    #endregion

    /// <summary>
    /// 新建或打開現有文件，並添加內容
    /// </summary>
    /// <param name="path">文件路徑</param>
    /// <param name="content">文件內容</param>
    /// <param name="encoding">內容編碼</param>
    public static void CreateAppend2(string path, string content, Encoding encoding)
    {
        //FileTools.EnsurePath(Path.GetFullPath(path));
        FileStream stream1 = new FileStream(path, FileMode.Append, FileAccess.Write);
        StreamWriter writer1 = new StreamWriter(stream1, encoding);
        writer1.Write(content);
        writer1.Close();
        stream1.Close();
    }


    //#region 根據執行的類型返回要產生的文件名稱
    ///// <summary>
    ///// 功能說明:根據執行的類型返回要產生的文件名稱
    ///// 作    者:zhiyuan
    ///// 創建時間:2010/06/08
    ///// 修改記錄:
    ///// </summary>
    ///// <param name="OutType"></param>
    //private void GetFileName(string strFileName)
    //{
    //    string strDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
    //    strBFileName = strDate + strFileName + ".txt";
    //    strPFileName = strDate + strFileName + "_Po.txt";
    //}
    //#endregion

    #region 記錄JOB成功或失敗資料至質料庫表
    /// <summary>
    /// 功能說明:記錄JOB成功和失敗資料至質料庫表
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strJobId"></param>
    /// <returns></returns>
    public void WriteLogToDB()
    {
        string strStatus = string.Empty;
        string strMessage = string.Empty;
        try
        {
            DataRow[] rowStatus = dtLocalFile.Select("UploadStates='F'");
            //*匯出失敗
            if (rowStatus != null && rowStatus.Length > 0)
            {
                JobHelper.SaveLog("WriteLogToDB:匯出失敗！", LogState.Info);
                strStatus = "F";
                strMessage = Resources.JobResource.Job010201;
            }
            //*匯出成功
            else
            {
                JobHelper.SaveLog("WriteLogToDB:匯出成功！", LogState.Info);
                strStatus = "S";
                strMessage = Resources.JobResource.Job010200;
            }
            EntityM_LBatchLog LBatchLog = new EntityM_LBatchLog();
            LBatchLog.FUNCTION_KEY = "06";
            LBatchLog.JOB_ID = strJobId;
            LBatchLog.START_TIME = StartTime.ToShortTimeString();
            LBatchLog.END_TIME = EndTime.ToShortTimeString();
            LBatchLog.STATUS = strStatus;
            LBatchLog.RETURN_MESSAGE = strMessage;
            BRM_LBatchLog.insert(LBatchLog);
        }
        catch (Exception exp) {
            JobHelper.SaveLog(exp.Message);
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
            string strSubject = string.Format(dtCallMail.Rows[0]["MailTittle"].ToString(), Resources.JobResource.Job3000115, strFileName);
            string strBody = string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strFileName, Resources.JobResource.Job3000115, strCon);
            JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
        }
    }
    #endregion

    #region 回寫產出次數
    /// <summary>
    /// 回寫產出次數
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="drRows"></param>
    private void UpdateExpCount(string strType, DataRow[] drRows)
    {
        string strErrMsg = string.Empty;
        if (strType == "P")
        {
            foreach (DataRow dr in drRows)
            {
                EntityM_PostSend PostSend = new EntityM_PostSend();
                PostSend.maildate = dr["maildate"].ToString();
                PostSend.mailno = dr["mailno"].ToString();
                PostSend.Exp_Count = ExpCount(dr["Exp_Count"].ToString().Trim()).ToString();
                PostSend.Exp_Date = DateTime.Now.ToString("yyyy/MM/dd");
                SqlHelper sqlHelp = new SqlHelper();
                sqlHelp.AddCondition(EntityM_PostSend.M_maildate, Operator.Equal, DataTypeUtils.String, PostSend.maildate);
                sqlHelp.AddCondition(EntityM_PostSend.M_mailno, Operator.Equal, DataTypeUtils.String, PostSend.mailno);

                BRM_PostSend.Update(PostSend, sqlHelp.GetFilterCondition(), ref strErrMsg, "Exp_Count", "Exp_Date");
            }
        }
        if (strType == "B")
        {
            foreach (DataRow dr in drRows)
            {
                Entity_CardBackInfo CardBack = new Entity_CardBackInfo();
                CardBack.serial_no = dr["Serial_no"].ToString();
                CardBack.Exp_Count = ExpCount(dr["Exp_Count"].ToString().Trim()).ToString();
                CardBack.Exp_Date = DateTime.Now.ToString("yyyy/MM/dd");

                SqlHelper sqlHelp = new SqlHelper();
                sqlHelp.AddCondition(Entity_CardBackInfo.M_serial_no, Operator.Equal, DataTypeUtils.String, CardBack.serial_no);

                BRM_CardBackInfo.Update(CardBack, sqlHelp.GetFilterCondition(), "Exp_Count", "Exp_Date");
            }
        }
    }

    private int ExpCount(string strCount)
    {
        try
        {
            int iCount = int.Parse(strCount);
            return iCount + 1;
        }
        catch
        {
            return 1;
        }
    }
    #endregion
}
