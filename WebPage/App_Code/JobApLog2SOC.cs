//******************************************************************
//*  功能說明：上傳 Audit Log
//*  作    者：James
//*  創建日期：2019/07/4
//*  修改記錄：2020/10/07 Ares Luke 調整業務需求此功能移至卡流，且移除擴充原本沒有的屬性，並新增手動ReRun參數
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
// using CSIPACQ.EntityLayer;
using Framework.Common.IO;
using Framework.Common.Utility;
using Framework.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using BusinessRules;
using EntityLayer;
using Framework.Common.Logging;
using System.Linq;

public class JobApLog2SOC : IJob
{
    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strErrMsg = string.Empty;
    protected string strFunctionKey = "06";
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:
    /// 創建時間:
    /// 修改記錄:2020/11/25_Ares_Luke-新增排除資料機制(排除Customer_Id,Account_Nbr)與寄信內容
    /// </summary>
    /// <param name="context"></param>
    public void Execute(JobExecutionContext context)
    {
        strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
        JobHelper.strJobId = strJobId;

        try
        {
            string strMsgID = string.Empty;
            string strRETSTR = string.Empty;

            strRETSTR = "*********** " + strJobId + " START **************";
            JobHelper.SaveLog(strRETSTR);

            #region 取得 查詢條件和取值
            //若沒指定日期就是要查前一天,但放置的目錄還是當天
            string strRunDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
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
                            strRunDate = tempDt.ToString("yyyyMMdd");
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



            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId) == "" || JobHelper.SerchJobStatus(strJobId) == "0")
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


            int intDataCnt = 0;
            int intExcludeCnt = 0;

            DataTable dt = getData(strRunDate, true);
            if (dt != null)
            {
                //排除Customer_Id,Account_Nbr為空。
                intExcludeCnt = dt.Rows.Count;
            }
            dt = getData(strRunDate, false);
            intDataCnt = dt.Rows.Count;

            strErrMsg = SendTxt(dt, strRunDate);

            //記錄job結束時間
            EndTime = DateTime.Now;

            #region job結束日誌記錄            

            //*判斷job完成狀態
            string strJobStatus = "S";
            string strReturnMsg = string.Empty;
            strReturnMsg += string.Format("總計筆數:{0}筆,送出筆數:{1}筆,無需送出筆數:{2}筆", intDataCnt + intExcludeCnt , intDataCnt, intExcludeCnt);

            //執行結果 寫入 DB            
            if (!string.IsNullOrEmpty(strErrMsg))
            {
                strReturnMsg += ". 失敗訊息:" + strErrMsg;
                strJobStatus = "F";
                JobHelper.SaveLog(strErrMsg);
            }

            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");

            #endregion

            strRETSTR = "*********** " + strJobId + " End **************";
            JobHelper.SaveLog(strRETSTR);

            updateUploadflag(strRunDate);
            strErrMsg = strReturnMsg;
        }
        catch (Exception ex)
        {
            JobHelper.SaveLog(ex.Message);
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, "F", "發生錯誤：" + ex.Message);

            //*JOB失敗發送MAIL 
            strErrMsg = ex.Message;
        }


        sendMailmsg(strErrMsg);

    }
    #endregion

    private DataTable getData(string strRunDate,Boolean ExcludeStatue)
    {
        SqlCommand sqlcmd = new SqlCommand();
        string sql = "Select System_Code,Login_Account_Nbr,convert(char(23),Query_Datetime,121)Query_Datetime,AP_Txn_Code,Server_Name,User_Terminal,AP_Account_Nbr,Txn_Type_Code," +
            "Statement_Text,Object_Name,Txn_Status_Code,Customer_Id,Account_Nbr,Branch_Nbr,Role_Id,Import_Source,As_Of_Date " +
            "From L_AP_LOG (nolock) where As_Of_Date=@As_Of_Date  and IsUpload='0'";
        if (ExcludeStatue) {
            sql += " and ISNULL(Customer_Id,'') = '' and ISNULL(Account_Nbr,'') = '' ";
        }
        sqlcmd.Parameters.Add(new SqlParameter("@As_Of_Date", strRunDate));

        sql += " order by Query_Datetime";
        DataHelper dh = new DataHelper("Connection_CSIP");

        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandTimeout = int.Parse(UtilHelper.GetAppSettings("SqlCmdTimeoutMax"));

        sqlcmd.CommandText = sql;

        DataSet ds = new DataSet();
        ds = dh.ExecuteDataSet(sqlcmd);

        if (ds == null)
        {
            return null;
        }
        return ds.Tables[0];
    }

    private void updateUploadflag(string strRunDate)
    {
        SqlCommand sqlcmd = new SqlCommand();
        string sql = "update L_AP_LOG set IsUpload=1 where As_Of_Date=@As_Of_Date";

        sqlcmd.Parameters.Add(new SqlParameter("@As_Of_Date", strRunDate));

        DataHelper dh = new DataHelper("Connection_CSIP");

        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandTimeout = int.Parse(UtilHelper.GetAppSettings("SqlCmdTimeoutMax"));
        sqlcmd.CommandText = sql;

        dh.ExecuteNonQuery(sqlcmd);

    }

    /// <summary>
    /// 讀取FTP設定
    /// </summary>
    /// <param name="Jobid"></param>
    /// <returns></returns>
    private Entity_FileInfo GetFtpSetting()
    {
        Entity_FileInfo result = new Entity_FileInfo();
        DataTable dt = new DataTable();
        Com_FileInfo.selectFileInfo(ref dt, strJobId);
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        result.FtpFileName = dt.Rows[0]["FtpFileName"].ToString();
        result.FtpUserName = dt.Rows[0]["FtpUserName"].ToString();
        result.FtpPwd = dt.Rows[0]["FtpPwd"].ToString();
        result.FtpIP = dt.Rows[0]["FtpIP"].ToString();
        result.FtpPath = dt.Rows[0]["FtpPath"].ToString();
        result.ZipPwd = dt.Rows[0]["ZipPwd"].ToString();
        
        return result;
    }

    /// <summary>
    /// 取得通知郵件外部參數資料
    /// </summary>
    /// <param name="sType"></param>
    /// <returns></returns>
    private bool GetMailInfo(ref DataTable dtMailInfo)
    {
        DataSet ds = new DataSet();
        try
        {
            DataHelper dh_CSIP = new DataHelper("Connection_CSIP");
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = int.Parse(UtilHelper.GetAppSettings("SqlCmdTimeoutMax"));
            sqlcmd.CommandText = @"select Property_Code,Property_Name from M_PROPERTY_CODE where function_key='06' and property_key='JobApLog2SOC' and off_flag='1' order by SEQUENCE ";
            ds = dh_CSIP.ExecuteDataSet(sqlcmd);

            if (ds != null)
            {
                dtMailInfo = ds.Tables[0];
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            JobHelper.SaveLog(ex.ToString());
            return false;
        }
    }

    /// <summary>
    /// 產檔,上傳FTP
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="RunDate"></param>
    /// <returns></returns>
    private string SendTxt(DataTable dt, string RunDate)
    {
        string result = string.Empty;
        StringBuilder sbHead = new StringBuilder();
        StringBuilder sbDetail = new StringBuilder();
        List<string> tmpStrs = new List<string>();
        string[] colNames = "System_Code,Login_Account_Nbr,Query_Datetime,AP_Txn_Code,Server_Name,User_Terminal,AP_Account_Nbr,Txn_Type_Code,Statement_Text,Object_Name,Txn_Status_Code,Customer_Id,Account_Nbr,Branch_Nbr,Role_Id,Import_Source,As_Of_Date".Split(',');
        int rsCount = 0;
        int rowflag = 0;
        string tmpStr = "";
        if (dt != null)
        {
            rsCount = dt.Rows.Count;

            //迴圈組明細檔內容, 最後一行則不加入換行符號
            foreach (DataRow item in dt.Rows)
            {
                tmpStrs.Clear();
                foreach (string _name in colNames)
                {
                    tmpStr = "";
                    //若對應欄位不存在時, 至少為空白,而不至於此行完全沒值
                    try
                    {
                        tmpStr = item[_name].ToString();
                    }
                    catch (Exception)
                    {
                    }
                    tmpStrs.Add(tmpStr);
                }
                rowflag++;

                tmpStr = string.Join(",", tmpStrs.ToArray());
                if (rowflag < rsCount)
                {
                    tmpStr += "\n";
                }

                sbDetail.Append(tmpStr);
            }
        }
        //取得 FTP 相關設定值
        Entity_FileInfo _ftpInfo = GetFtpSetting();
        if (_ftpInfo == null)
        {
            JobHelper.SaveLog(strJobId + "讀取FTP設定失敗。");
            result = "讀取FTP設定失敗！";
            return result;
        }
        string fileNameTitle = string.Format("{0}{1}", _ftpInfo.FtpFileName, RunDate); //"CSIP_"+yyyyMMdd
        string fileNameH = string.Format("{0}.H", fileNameTitle);
        string fileNameD = string.Format("{0}.D", fileNameTitle);
        //要複製的新檔名
        string _currentTime = DateTime.Now.ToString("HHmmss");
        string fileNameD_new = string.Format("{0}{1}.D", fileNameTitle, _currentTime);
        string fileNameH_new = string.Format("{0}{1}.H", fileNameTitle, _currentTime);

        //*本地存放目錄(格式為 FileUpload\JobApLog2SOC\JobIDyyyyMMdd):日期一律為產檔的當天
        string _currentDate = DateTime.Now.ToString("yyyyMMdd");
        string tmpFname = AppDomain.CurrentDomain.BaseDirectory + @"FileUpload\{0}\{1}";
        string folderName = string.Format(tmpFname, strJobId, _currentDate);
        string fileFullNameD = Path.Combine(folderName, fileNameD);
        string fileFullNameD_new = Path.Combine(folderName, fileNameD_new);
        string fileFullNameH = Path.Combine(folderName, fileNameH);
        string fileFullNameH_new = Path.Combine(folderName, fileNameH_new);

        JobHelper.CreateLocalFolder(folderName);
        //表頭內容 e.x: CSIP_20190708,        17,20190708
        sbHead.AppendFormat("{0},{1},{2}", fileNameTitle, rsCount.ToString().PadLeft(10, ' '), RunDate);

        //檔案:刪除, 建立 ,複製
        try
        {
            JobHelper.SaveLog("刪除舊檔D:" + fileFullNameD, LogState.Info);
            FileTools.DeleteFile(fileFullNameD);
            JobHelper.SaveLog("刪除舊檔H:" + fileFullNameH, LogState.Info);
            FileTools.DeleteFile(fileFullNameH);

            JobHelper.SaveLog("建立新檔D:" + fileFullNameD, LogState.Info);
            FileTools.Create(fileFullNameD, sbDetail.ToString());
            JobHelper.SaveLog("建立新檔H:" + fileFullNameH, LogState.Info);
            FileTools.Create(fileFullNameH, sbHead.ToString());

            JobHelper.SaveLog("複製為新檔D:" + fileFullNameD, LogState.Info);
            FileTools.CopyFile(fileFullNameD, fileFullNameD_new);
            JobHelper.SaveLog("複製為新檔H:" + fileFullNameH, LogState.Info);
            FileTools.CopyFile(fileFullNameH, fileFullNameH_new);
        }
        catch (Exception ex)
        {
            JobHelper.SaveLog(string.Format("建檔失敗!({0})", ex.Message), LogState.Info);
            result = string.Format("建檔失敗!({0})", ex.Message);
            return result;
        }

        //-----------------------------------------        
        //上傳檔案        
        try
        {
            FTPFactory objFtp_UpLoad = new FTPFactory(_ftpInfo.FtpIP, _ftpInfo.FtpPath, _ftpInfo.FtpUserName, _ftpInfo.FtpPwd, "21", @"C:\CS09", "Y");
            bool ftpUploadFlagD = objFtp_UpLoad.Upload(_ftpInfo.FtpPath, fileNameD, fileFullNameD_new);
            bool ftpUploadFlagH = objFtp_UpLoad.Upload(_ftpInfo.FtpPath, fileNameH, fileFullNameH_new);

            //判斷上傳是否有異常
            if (ftpUploadFlagD == false || ftpUploadFlagH == false)
            {
                List<string> ftpMsgs = new List<string>();
                if (ftpUploadFlagD == false)
                {
                    ftpMsgs.Add(fileNameD);
                }
                if (ftpUploadFlagH == false)
                {
                    ftpMsgs.Add(fileNameH);
                }

                result += string.Format("FTP上傳失敗！:({0})", string.Join(",", ftpMsgs.ToArray()));
                JobHelper.SaveLog(string.Format("FTP上傳失敗！:({0})", string.Join(",", ftpMsgs.ToArray())), LogState.Info);
            }
        }
        catch (Exception ex)
        {
            result = ex.Message;
            JobHelper.SaveLog(ex.Message);
        }

        return result;

    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:寄信的方法
    /// 作    者:
    /// 創建時間:
    /// 修改紀錄:2020/11/17_Ares_Luke-調整信件Title(新增HostName、IP)
    /// </summary>
    /// <param name="logMsg"></param>
    private bool sendMailmsg(string logMsg)
    {
        bool result = false;
        DataTable mDt = new DataTable();
        if (GetMailInfo(ref mDt))
        {
            if (mDt.Rows.Count == 0)
            {
                JobHelper.SaveLog("未設定通知信箱資料。");
                return false;
            }
            List<string> MailUsers = new List<string>();
            for (int i = 0; i < mDt.Rows.Count; i++)
            {
                MailUsers.Add(mDt.Rows[i][1].ToString());
            }

            string name = System.Net.Dns.GetHostName();
            var host = System.Net.Dns.GetHostEntry(name);
            System.Net.IPAddress ip = host.AddressList.Where(n => n.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First();
            String strSubject = "(" + ip + "_" + name + ")  SOC AP Log執行結果";

            JobHelper.SendMail("", MailUsers.ToArray(), strSubject , logMsg);
        }
        return result;
    }



    
}