//******************************************************************
//*  功能說明：自動化簡訊處理-CSI檔匯出
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
using Framework.Data.OM;
using Framework.Common.Logging;
using Framework.Common.Utility;

/// <summary>
/// AutoNewsletterInfoCSI 的摘要描述
/// </summary>
public class AutoNewsletterInfoCSI : Quartz.IJob
{
    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strFolderName = string.Empty;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;
    
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
            //strJobId = "0119";
            
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
            dtLocalFile.Columns.Add("FtpFilePath");        //Ftp全路徑
            dtLocalFile.Columns.Add("FolderName");         //目錄名稱
            dtLocalFile.Columns.Add("TxtFileName");        //資料檔名
            dtLocalFile.Columns.Add("UploadStates");       //資料上載狀態
            dtLocalFile.Columns.Add("FtpPath");            //FTP路徑
            dtLocalFile.Columns.Add("FtpIP");              //FTP IP
            dtLocalFile.Columns.Add("FtpUserName");        //FTP用戶名
            dtLocalFile.Columns.Add("FtpPwd");             //FTP密碼
            #endregion

            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId).Equals("") || JobHelper.SerchJobStatus(strJobId).Equals("0"))
            {
                JobHelper.SaveLog("JOB 工作狀態為：停止！", LogState.Info);
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job1000115, "OU");

            //*無JOB交換當信息或查詢失敗
            if (!JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                return;
            }

            if (dtFileInfo.Rows.Count > 0)
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);

                strFolderName = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\" + strJobId + "\\" + strJobId + _jobDate.ToString("yyyyMMddHHmmss");
                foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                {
                    String txtNextDate = "";
                    txtNextDate = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", _jobDate.ToString("yyyyMMdd"), +1), "yyyyMMdd", null).ToString("yyyyMMdd");
                    //*重寄異動紀錄檔
                    if (rowFileInfo["FunctionFlg"].ToString().Equals("1"))
                    {
                        //  OutChgFile(rowFileInfo["FtpFileName"].ToString() + DateTime.Now.ToString("yyyyMMdd") + ".TXT", rowFileInfo);
                        //20120319, 20120416 modified by charlotte - start
                        OutChgFile(rowFileInfo["FtpFileName"].ToString() + txtNextDate + ".TXT", rowFileInfo);
                        //OutChgFile(rowFileInfo["FtpFileName"].ToString() + ".TXT", rowFileInfo);
                        //20120319, 20120416 modified by charlotte - end
                    }
                    //*退件未結案檔
                    if (rowFileInfo["FunctionFlg"].ToString().Equals("2"))
                    {
                        //     OutBackFile(rowFileInfo["FtpFileName"].ToString() + DateTime.Now.ToString("yyyyMMdd") + ".TXT", rowFileInfo);
                        //20120319 modified by charlotte - start
                        //OutBackFile(rowFileInfo["FtpFileName"].ToString() + txtNextDate + ".TXT", rowFileInfo);
                        OutBackFile(rowFileInfo["FtpFileName"].ToString() + ".TXT", rowFileInfo);
                        //20120319 modified by charlotte - end
                    }
                    //*簡訊檔
                    if (rowFileInfo["FunctionFlg"].ToString().Equals("3"))
                    {
                        //   OutSMSFile(rowFileInfo["FtpFileName"].ToString() + DateTime.Now.ToString("yyyyMMdd") + ".TXT", rowFileInfo);
                        OutSMSFile(rowFileInfo["FtpFileName"].ToString() + txtNextDate + ".TXT", rowFileInfo);
                    }
                }
            }

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
                //FileTools.DeleteFile(rows[k]["FileName"].ToString());
            }
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job執行結果寫入日誌
            WriteLogToDB();
            JobHelper.SaveLog("JOB結束！", LogState.Info);
            #endregion
        }
        catch (Exception ex)
        {
            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "CommonModel_發生錯誤_" + ex.ToString());
            BRM_LBatchLog.SaveLog(ex);
        }
    }
    #endregion

    #region 匯出重寄異動紀錄檔
    /// <summary>
    /// 功能說明:匯出重寄異動紀錄檔
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strFileName">產出檔案的名稱</param>
    private void OutChgFile(string strFileName, DataRow drLocalFile)
    {
        JobHelper.SaveLog("開始讀取要匯出的檔案(匯出重寄異動紀錄檔)資料！", LogState.Info);

        DataTable dtDetail = new DataTable();
        string strTmp = string.Empty;
        string strAddTmp = string.Empty;
        StringBuilder sbFileInfo = new StringBuilder();
        if (BRM_CardDataChange.GetChgData(ref dtDetail))
        {
            DataTable dtUserName = new DataTable();
            JobHelper.GetUserNameByUserId(dtDetail, "UpdUser", ref dtUserName);
            foreach (DataRow dr in dtDetail.Rows)
            {
                strAddTmp = string.Empty;
                //  strTmp = dr["ID"].ToString() + ","; //身分證字號
                strTmp = JobHelper.SetStrngValue2(dr["ID"].ToString(), 12) + ",";//身分證字號
                strTmp += dr["UpdDate"].ToString() + " " + dr["Endtime"].ToString() + ",";    //異動日期時間
                strAddTmp = Resources.JobResource.Job4010115 + dr["OldAdd1"].ToString(); //異動前地址一：
                strAddTmp += Resources.JobResource.Job4020115 + dr["NewAdd1"].ToString(); //，異動後地址一：
                strAddTmp += Resources.JobResource.Job4070115; //，
                strAddTmp += Resources.JobResource.Job4030115 + dr["OldAdd2"].ToString();  //異動前地址二：
                strAddTmp += Resources.JobResource.Job4040115 + dr["NewAdd2"].ToString();  //，異動後地址二：
                strAddTmp += Resources.JobResource.Job4070115; //，
                strAddTmp += Resources.JobResource.Job4050115 + dr["OldAdd3"].ToString();   //異動前地址三：
                strAddTmp += Resources.JobResource.Job4060115 + dr["NewAdd3"].ToString();  //，異動後地址三：

                strTmp += strAddTmp + ",";  //異動地址
                strTmp += dr["UpdUser"].ToString().Replace(" ", "") + ",";    //異動員工編號
                strTmp += GetUserName(dr["UpdUser"].ToString(), dtUserName).Replace(" ", "");    //異動員工姓名????
                strTmp += "\r\n";
                sbFileInfo.Append(strTmp);
            }
            if (sbFileInfo.Length > 2)
            {
                sbFileInfo.Append(sbFileInfo.ToString().Substring(0, sbFileInfo.ToString().Length - 2));
            }
            FileTools.EnsurePath(strFolderName);
            FileTools.CreateAppend(strFolderName + "\\" + strFileName, sbFileInfo.ToString());
            DataRow row = dtLocalFile.NewRow();//*記錄文件名稱以便刪除之用
            row["LocalFilePath"] = strFolderName + "\\" + strFileName;
            row["TxtFileName"] = strFileName;
            row["FtpPath"] = drLocalFile["FtpPath"].ToString();
            row["FtpIP"] = drLocalFile["FtpIP"].ToString();
            row["FtpUserName"] = drLocalFile["FtpUserName"].ToString();
            row["FtpPwd"] = drLocalFile["FtpPwd"].ToString();
            dtLocalFile.Rows.Add(row);
        }

        JobHelper.SaveLog("結束讀取要匯出的檔案(匯出重寄異動紀錄檔)資料！", LogState.Info);
    }
    #endregion

    /// <summary>
    /// 功能說明:取得用戶中文名稱
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/09
    /// 修改記錄:
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="dtUserList"></param>
    /// <returns></returns>
    private string GetUserName(string UserId, DataTable dtUserList)
    {
        foreach (DataRow dr in dtUserList.Rows)
        {
            if (UserId.Equals(dr["USER_ID"].ToString()))
            {
                return dr["USER_NAME"].ToString();
            }
        }
        return "";
    }

    /// <summary>
    /// 功能說明:退件未結案檔
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="drLocalFile"></param>
    private void OutBackFile(string strFileName, DataRow drLocalFile)
    {
        JobHelper.SaveLog("開始讀取要匯出的檔案(退件未結案檔)資料！", LogState.Info);

        DataTable dtDetail = new DataTable();
        string strTmp = string.Empty;
        StringBuilder sbFileInfo = new StringBuilder();
        if (BRM_CardBackInfo.GetBackData(ref dtDetail))
        {
            foreach (DataRow dr in dtDetail.Rows)
            {
                sbFileInfo.Append(JobHelper.SetStrngValue2(dr["ID"].ToString(), 12) + "\r\n");
            }
            if (sbFileInfo.Length > 2)
            {
                sbFileInfo.Append(sbFileInfo.ToString().Substring(0, sbFileInfo.ToString().Length - 2));
            }
            FileTools.EnsurePath(strFolderName);
            FileTools.CreateAppend(strFolderName + "\\" + strFileName, sbFileInfo.ToString());
            DataRow row = dtLocalFile.NewRow();//*記錄文件名稱以便刪除之用
            row["LocalFilePath"] = strFolderName + "\\" + strFileName;
            row["TxtFileName"] = strFileName;
            row["FtpPath"] = drLocalFile["FtpPath"].ToString();
            row["FtpIP"] = drLocalFile["FtpIP"].ToString();
            row["FtpUserName"] = drLocalFile["FtpUserName"].ToString();
            row["FtpPwd"] = drLocalFile["FtpPwd"].ToString();
            dtLocalFile.Rows.Add(row);
        }

        JobHelper.SaveLog("結束讀取要匯出的檔案(退件未結案檔)資料！", LogState.Info);
    }

    /// <summary>
    /// 功能說明:簡訊檔
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="drLocalFile"></param>
    private void OutSMSFile(string strFileName, DataRow drLocalFile)
    {
        JobHelper.SaveLog("開始讀取要匯出的檔案(簡訊檔)資料！", LogState.Info);

        DataTable dtDetail = new DataTable();
        string strTmp = string.Empty;
        StringBuilder sbFileInfo = new StringBuilder();
        if (BRM_SLetterInfoCallCust.GetSMSData(ref dtDetail))
        {
            foreach (DataRow dr in dtDetail.Rows)
            {
                //strTmp = dr["ID"].ToString() + ",";
                strTmp = JobHelper.SetStrngValue2(dr["ID"].ToString(), 12) + ",";
                // strTmp += dr["Mobil"].ToString() + ",";
                strTmp += JobHelper.SetStrngValue2(dr["Mobil"].ToString(), 10) + ",";
                strTmp += JobHelper.SetStrngValue(dr["CardNo"].ToString(), 19).Substring(JobHelper.SetStrngValue(dr["CardNo"].ToString(), 19).Length - 4) + ",";
                strTmp += dr["Source_Flg"].ToString().Equals("0") ? Resources.JobResource.Job0000044 : Resources.JobResource.Job0000045;
                strTmp += "\r\n";

                sbFileInfo.Append(strTmp);
            }
            if (sbFileInfo.Length > 2)
            {
                sbFileInfo.Append(sbFileInfo.ToString().Substring(0, sbFileInfo.ToString().Length - 2));
            }
            FileTools.EnsurePath(strFolderName);
            FileTools.CreateAppend(strFolderName + "\\" + strFileName, sbFileInfo.ToString());
            DataRow row = dtLocalFile.NewRow();//*記錄文件名稱以便刪除之用
            row["LocalFilePath"] = strFolderName + "\\" + strFileName;
            row["TxtFileName"] = strFileName;
            row["FtpPath"] = drLocalFile["FtpPath"].ToString();
            row["FtpIP"] = drLocalFile["FtpIP"].ToString();
            row["FtpUserName"] = drLocalFile["FtpUserName"].ToString();
            row["FtpPwd"] = drLocalFile["FtpPwd"].ToString();
            dtLocalFile.Rows.Add(row);
        }

        JobHelper.SaveLog("結束讀取要匯出的檔案(簡訊檔)資料！", LogState.Info);
    }

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
        JobHelper.SaveLog("開始WriteLogToDB！", LogState.Info);
        string strStatus = string.Empty;
        string strMessage = string.Empty;
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
        BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
        BRM_LBatchLog.insert(LBatchLog);
        JobHelper.SaveLog("結束WriteLogToDB！", LogState.Info);
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
            string strSubject = string.Format(dtCallMail.Rows[0]["MailTittle"].ToString(), Resources.JobResource.Job1000115, strFileName);
            string strBody = string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strFileName, Resources.JobResource.Job1000115, strCon);
            JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
        }
    }
    #endregion
}
