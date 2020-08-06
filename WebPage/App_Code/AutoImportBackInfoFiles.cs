//******************************************************************
//*  功能說明：自動化郵局退件通知
//*  作    者：zhiyuan
//*  創建日期：2010/06/3
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
//20171117 (U) by Tank

using System;
using System.Data;
using Framework.Common.Logging;
using Framework.Common.Utility;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using System.Data.SqlClient;

public class AutoImportBackInfoFiles : Quartz.IJob
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
    protected DateTime UploadTime;

    string strFtpIp = string.Empty;
    string strFtpUserName = string.Empty;
    string strFtpPwd = string.Empty;
    FTPFactory objFtp;
    //20171117 (U) by Tank
    string strErrMsg = string.Empty;
    
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
            //strJobId = "0110";
            #region 記錄job啟動時間
            StartTime = DateTime.Now;

            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
            #endregion
            
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000110, "IM");

            #region 登陸ftp下載壓縮檔
            string strFolderName = string.Empty;//*本地存放目錄(格式為yyyyMMdd+am/pm)
            dtFileInfo = new DataTable();
            String errMsg = "";
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
                        //本地路徑
                        strLocalPath = UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName + "\\";
                        //FTP 檔名
                        string strFileInfo = _jobDate.ToString("yyyyMMdd") + rowFileInfo["FtpFileName"].ToString() + ".ZIP";
                        //FTP 路徑+檔名
                        string strFtpFileInfo = rowFileInfo["FtpPath"].ToString() + "//" + strFileInfo;

                        strFtpIp = rowFileInfo["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo["FtpPwd"].ToString();
                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                        //*檔案存在
                        if (objFtp.isInFolderList(strFtpFileInfo))
                        {
                            JobHelper.SaveLog("開始下載檔案！", LogState.Info);
                            //*下載檔案
                            if (objFtp.Download(strFtpFileInfo, strLocalPath, strFileInfo))
                            {
                                //*記錄下載的檔案信息
                                DataRow row = dtLocalFile.NewRow();
                                row["LocalFilePath"] = strLocalPath; //本地路徑
                                row["FtpFilePath"] = strFtpFileInfo; //FTP 路徑+檔名
                                row["FolderName"] = strLocalPath; //本地資料夾
                                row["ZipFileName"] = strFileInfo; //FTP壓縮檔名稱
                                row["ZipPwd"] = rowFileInfo["ZipPwd"].ToString(); //FTP壓縮檔密碼
                                row["CardType"] = rowFileInfo["CardType"].ToString(); //卡片種類
                                row["MerchCode"] = rowFileInfo["MerchCode"].ToString(); //製卡廠代碼
                                row["MerchName"] = rowFileInfo["MerchName"].ToString(); //製卡廠名稱
                                row["ZipPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["ZipPwd"].ToString()); //解密
                                //row["Trandate"] = GetTrandate(rowFileInfo["AMPMFlg"].ToString()); //轉檔日設定
                                //加密 RedirectHelper.GetEncryptParam(rowFileInfo["ZipPwd"].ToString());
                                dtLocalFile.Rows.Add(row);
                                JobHelper.SaveLog("下載檔案成功！", LogState.Info);
                            }
                        }
                        //*檔案不存在
                        else
                        {
                            errMsg += (errMsg == "" ? "" : "、") + rowFileInfo["FtpFileName"].ToString();
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0110000, rowFileInfo["FtpFileName"].ToString()));
                            // SendMail(rowFileInfo["FtpFileName"].ToString(), Resources.JobResource.Job0000008);
                        }

                    }

                    if (errMsg != "")
                    {
                        SendMail(errMsg, Resources.JobResource.Job0000008);
                    }
                }
            }
            else
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
            }
            #endregion

            #region 處理本地壓縮檔
            errMsg = "";
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
                    errMsg += (errMsg == "" ? "" : "、") + rowLocalFile["ZipFileName"].ToString();
                    rowLocalFile["ZipStates"] = "F";
                    // SendMail(rowLocalFile["ZipFileName"].ToString(), Resources.JobResource.Job0000002);
                    JobHelper.SaveLog("解壓縮檔案失敗！");
                }
            }
            if (errMsg != "")
            {
                SendMail(errMsg, Resources.JobResource.Job0000002);
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
                JobHelper.SaveLog("開始讀取要匯入的檔案資料！", LogState.Info);
                //*讀取檔名正確資料
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
                        string strMailErr = string.Empty;          //錯誤消息
                        string strFunctionName = Resources.JobResource.Job0000110;      //Job功能名稱
                        //*檢核成功
                        if (UploadCheck(strPath, strFunctionName, strCardType, ref No, ref strMailErr, ref arrayErrorMsg, ref dtDetail))
                        {
                            JobHelper.SaveLog("檢核檔案成功！", LogState.Info);
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
                            JobHelper.SaveLog("檢核檔案失敗！");
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
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");

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
                JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
                SetJobError(SCount, FCount);
            }
            else
            {
                JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
            }
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
            string strSubject = string.Format(dtCallMail.Rows[0]["MailTittle"].ToString(), Resources.JobResource.Job0000110, strFileName);
            string strBody = string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strFileName, Resources.JobResource.Job0000110, strCon);

            //20171117 (U) by Tank
            if (strErrMsg.Length > 0)
            {
                strBody = strBody + "<br>錯誤資訊如下:<br>" + strErrMsg;
            }

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
        UploadTime = DateTime.Parse(dtmThisDate.ToString());
        int intMax = int.MaxValue;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        #endregion

        strUploadID = "06011000";
        dtDetail = BaseHelper.UploadCheck(strUserID, strFunctionKey, strUploadID,
                       UploadTime, Resources.JobResource.Job0000110, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);


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
        //bool blnResult = false;
        string Trandate = string.Empty;
        string strSql = string.Empty;
        string strMsg = string.Empty;

        //20171117 (U) by Tank
        //if(!CheckData(dtDetail))
        //{
        //    JobHelper.SaveLog(Resources.JobResource.Job0110004);
        //    return false;
        //}

        //20171117 (U) by Tank
        //EntitySet<EntityLayer.Entity_CardBackInfo> SetCardChange = BRM_TCardBaseInfo.GetBackInfoFor0110(dtDetail,strFileName);
        EntitySet<EntityLayer.Entity_CardBackInfo> SetCardChange = GetBackInfoFor0110(dtDetail, strFileName);
        if (null == SetCardChange || SetCardChange.Count <= 0)
        {
            JobHelper.SaveLog(Resources.JobResource.Job0110005);
            SendMail(strFileName, Resources.JobResource.Job0110005);
            return false;
        }

        if (BRM_CardBackInfo.BatInsertFor0110(SetCardChange, ref strMsg))
        {
            if (strErrMsg.Length > 0)
            {
                SendMail(strFileName, Resources.JobResource.Job0110005);
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;

    }

    /// <summary>
    /// 功能說明: 按數據集查詢基本資料檔，查詢結果填充到EntitySet中并返回
    /// 作    者: 林鴻揚
    /// 創建時間: 2017/11/20
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strImportFileName"></param>
    /// <returns></returns>
    public EntitySet<Entity_CardBackInfo> GetBackInfoFor0110(DataTable dtDetail, string strImportFileName)
    {
        SqlConnection sqlConn = null;
        SqlCommand sqlCmd = null;
        SqlDataAdapter sdaBascInfo = new SqlDataAdapter();
        SqlParameter parmCardNo = new SqlParameter();
        SqlParameter parmAction = new SqlParameter();
        DataTable dtTmp = new DataTable();
        bool isError;

        EntitySet<Entity_CardBackInfo> SetCardChange = new EntitySet<Entity_CardBackInfo>();

        try
        {
            string strSql = "SELECT top 1 id,custname,trandate,cardtype,add1,add2,add3 FROM tbl_Card_BaseInfo WHERE cardno=@CardNo and action=@Action ORDER BY trandate DESC ";
            sqlConn = new SqlConnection(UtilHelper.GetConnectionStrings("Connection_System"));
            sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = strSql;
            parmCardNo.ParameterName = "@CardNo";
            parmAction.ParameterName = "@Action";

            sqlConn.Open();
            for (int i = 0; i < dtDetail.Rows.Count; i++)
            {
                sqlCmd.Parameters.Clear();
                dtTmp.Rows.Clear();
                parmCardNo.Value = dtDetail.Rows[i]["CardNo"].ToString();
                parmAction.Value = dtDetail.Rows[i]["Action"].ToString();
                sqlCmd.Parameters.Add(parmCardNo);
                sqlCmd.Parameters.Add(parmAction);
                sdaBascInfo = new SqlDataAdapter(sqlCmd);
                sdaBascInfo.Fill(dtTmp);

                string strCardNo = dtDetail.Rows[i]["CardNo"].ToString();
                string strCardNo_FN = strCardNo.Substring(0, 6) + "******" + strCardNo.Substring(strCardNo.Length - 4, 4);

                if (dtTmp.Rows.Count > 0)
                {

                    isError = false;

                    //流水號是否重複
                    if (BRM_CardBackInfo.IsRepeatBySno(dtDetail.Rows[i]["Serial_no"].ToString()))
                    {
                        strErrMsg = strErrMsg + "‧退件新增檔案卡號" + strCardNo_FN + "的流水號重複<br>";
                        isError = true;
                    }

                    //退件類別不存在-系統參數表中不存在
                    //*PROPERTY_KEY=6(Kind)
                    if (!DistinctCol2(dtDetail.Rows[i]["Kind"].ToString(), "Kind", "6"))
                    {
                        strErrMsg = strErrMsg + "‧退件新增檔案卡號" + strCardNo_FN + "的退件類別不存在<br>";
                        isError = true;
                    }

                    //卡別不存在—系統參數表不存在
                    //*PROPERTY_KEY=1(CardType)
                    if (!DistinctCol2(dtDetail.Rows[i]["Action"].ToString(), "Action", "1"))
                    {
                        strErrMsg = strErrMsg + "‧退件新增檔案卡號" + strCardNo_FN + "的卡別不存在<br>";
                        isError = true;
                    }

                    //退件原因不存在—系統參數表不存在
                    //*PROPERTY_KEY=5(Reason)
                    if (!DistinctCol2(dtDetail.Rows[i]["Reason"].ToString(), "Reason", "5"))
                    {
                        strErrMsg = strErrMsg + "‧退件新增檔案卡號" + strCardNo_FN + "的退件原因不存在<br>";
                        isError = true;
                    }

                    if (isError == false)
                    {
                        Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();
                        CardBackInfo.serial_no = dtDetail.Rows[i]["Serial_no"].ToString();    //流水號
                        CardBackInfo.Id = dtTmp.Rows[0]["id"].ToString();   //身份證字號
                        CardBackInfo.CustName = dtTmp.Rows[0]["custname"].ToString();   //姓名
                        CardBackInfo.Kind = dtDetail.Rows[i]["Kind"].ToString();    //退件類別
                        CardBackInfo.CardNo = dtDetail.Rows[i]["CardNo"].ToString();    //卡號
                        CardBackInfo.Action = dtDetail.Rows[i]["Action"].ToString();    //action
                        CardBackInfo.Trandate = dtTmp.Rows[0]["trandate"].ToString();   //轉當日
                        CardBackInfo.cardtype = dtTmp.Rows[0]["cardtype"].ToString();   //卡種
                        CardBackInfo.Backdate = DateHelper.InsertTimeSpan(DateHelper.ConvertToAD(dtDetail.Rows[i]["Serial_no"].ToString().Substring(1, 7)));  //退件日期
                        CardBackInfo.Reason = dtDetail.Rows[i]["Reason"].ToString();    //退件原因
                        CardBackInfo.Madd1 = dtTmp.Rows[0]["add1"].ToString();    //郵寄地址1
                        CardBackInfo.Madd2 = dtTmp.Rows[0]["add2"].ToString();    //郵寄地址2
                        CardBackInfo.Madd3 = dtTmp.Rows[0]["add3"].ToString();    //郵寄地址3
                        CardBackInfo.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");
                        CardBackInfo.ImportFileName = strImportFileName;
                        CardBackInfo.CardBackStatus = "0";
                        CardBackInfo.OriginalDBflg = "0";
                        CardBackInfo.Exp_Count = "0";
                        CardBackInfo.Enditem = "";
                        if (!BRM_CardBackInfo.IsRepeatBySno(CardBackInfo.serial_no))
                        {
                            SetCardChange.Add(CardBackInfo);
                        }
                    }

                }
                else
                {
                    strErrMsg = strErrMsg + "‧退件新增檔案卡號" + strCardNo_FN + "在卡片基本資料表中不存在<br>";
                }
            }
        }
        catch (System.Exception ex)
        {
            BRM_TCardBaseInfo.SaveLog(ex.Message);
        }
        finally
        {
            if (sdaBascInfo != null)
            {
                sdaBascInfo.Dispose();
            }
            if (sqlCmd != null)
            {
                sqlCmd.Dispose();
            }
            if (sqlConn != null)
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        return SetCardChange;
    }

    /// 功能說明: 檢核系統參數表，查看要匯入資料是否存在
    /// 作    者: 林鴻揚
    /// 創建時間: 2017/11/20
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strColName"></param>
    /// <param name="strPropertyKey"></param>
    /// <returns></returns>
    public bool DistinctCol2(string strData, string strColName, string strPropertyKey)
    {
        DataTable dtTmp = new DataTable();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetProperty(strFunctionKey, strPropertyKey, ref dtTmp))
        {
            DataRow[] drRows = dtTmp.Select("PROPERTY_CODE='" + strData + "'");
            if (drRows.Length == 0)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    /// <summary>
    /// 功能說明:檢核資料是否正確
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <returns></returns>
    private bool CheckData(DataTable dtDetail)
    {
        string strTmpSerialNo = string.Empty;
        DataRow[] drRows = dtDetail.Select("", "SERIAL_NO");
        ArrayList arrKind = new ArrayList();    //退件類別
        ArrayList arrCardType = new ArrayList();    //卡別
        ArrayList arrReason = new ArrayList();  //退件原因

        //*0:流水號重複-退件通知檔中重復

        for (int i = 0; i < drRows.Length; i++)
        {
            strTmpSerialNo = drRows[i]["SERIAL_NO"].ToString();
            for (int j = i + 1; j < drRows.Length; j++)
            {
                if (strTmpSerialNo.Equals(drRows[j]["SERIAL_NO"].ToString()))
                {
                    //JobHelper.SaveLog();
                    return false;
                }

            }
        }

        //*1:流水號已使用過-卡片退件資料表中存在
        if (!BRM_CardBackInfo.BatIsRepeatByColName(dtDetail, "SERIAL_NO"))
        {
            return false;
        }

        //*3:卡號不存在—卡片基本資料表中不存在
        if (!BRM_TCardBaseInfo.BatIsRepeatByColName(dtDetail, "CardNo", "Action"))
        {
            return false;
        }

        //*4:退件類別不存在-系統參數表中不存在
        //*PROPERTY_KEY=6(Kind)
        if (!DistinctCol(dtDetail, "Kind", "6"))
        {
            return false;
        }

        //*5:卡別不存在—系統參數表不存在
        //*PROPERTY_KEY=1(CardType)
        if (!DistinctCol(dtDetail, "Action", "1"))
        {
            return false;
        }

        //*6:退件原因不存在—系統參數表不存在
        //*PROPERTY_KEY=5(Reason)
        if (!DistinctCol(dtDetail, "Reason", "5"))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 功能說明:檢核系統參數表，查看要匯入資料是否存在
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strColName"></param>
    /// <param name="strPropertyKey"></param>
    /// <returns></returns>
    private bool DistinctCol(DataTable dtDetail, string strColName, string strPropertyKey)
    {
        ArrayList arrTmp = new ArrayList();
        DataTable dtTmp = new DataTable();
        DataRow[] drRows = dtDetail.Select("", strColName);
        string strTmp = drRows[0][strColName].ToString();
        arrTmp.Add(strTmp);
        for (int i = 1; i < drRows.Length; i++)
        {
            if (strTmp.Equals(drRows[i][strColName].ToString()))
            {
                continue;
            }
            else
            {
                strTmp = drRows[i][strColName].ToString();
                arrTmp.Add(strTmp);
            }
        }

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetProperty(strFunctionKey, strPropertyKey, ref dtTmp))
        {
            for (int i = 0; i < arrTmp.Count; i++)
            {
                drRows = dtTmp.Select("PROPERTY_CODE='" + arrTmp[i].ToString() + "'");
                if (drRows.Length > 0)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void SetJobError(int iSCount, int iFCount)
    {
        string strErrMsg = "";

        CSIPCommonModel.EntityLayer.EntityL_UPLOAD EntityUpload = new CSIPCommonModel.EntityLayer.EntityL_UPLOAD();
        EntityUpload.FUNCTION_KEY = "06";
        EntityUpload.UPLOAD_ID = "06011000";
        EntityUpload.UPLOAD_DATE = UploadTime;
        if (CSIPCommonModel.BusinessRules.BRL_UPLOAD.IsRepeat(EntityUpload, ref strErrMsg))
        {
            EntityUpload.UPLOAD_STATUS = "N";
            EntityUpload.UPLOAD_TOTAL_COUNT = iSCount + iFCount;
            EntityUpload.UPLOAD_SUC_COUNT = iSCount;
            EntityUpload.UPLOAD_FAIL_COUNT = iFCount;
            EntityUpload.CHANGED_USER = "sys";
            EntityUpload.CHANGED_TIME = DateTime.Parse(DateTime.Now.ToString());
            CSIPCommonModel.BusinessRules.BRL_UPLOAD.Update(EntityUpload, ref strErrMsg);
        }
    }
}
