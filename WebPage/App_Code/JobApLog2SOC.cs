//******************************************************************
//*  功能說明：上傳 Audit Log
//*  作    者：James
//*  創建日期：2019/07/4
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
// using CSIPACQ.EntityLayer;
using Framework.Common.IO;
using Framework.Common.Utility;
using Framework.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using EntityLayer;

public class JobApLog2SOC : IJob
{
    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strErrMsg = string.Empty;
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:
    /// 創建時間:
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(JobExecutionContext context)
    {
        strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
        ExecuteManual(strJobId);
    }
    #endregion

    public bool ExecuteManual(string jobID)
    {
        bool result = true;
        try
        {
            string strMsgID = string.Empty;
            string strRETSTR = string.Empty;
            strJobId = jobID;

            strRETSTR = "*********** " + strJobId + " START **************";
            JobHelper.Write(strJobId, strRETSTR);

            #region 取得 查詢條件和取值
            string strRunDate = "";
            string strRunFlag = "";//  若 strRunFlag = Y => 代表只會依 strRunDate 撈資料
            //若 strRunFlag = N => 代表只會撈 uploadFlag = 0 和 strRunDate

            //取得 查詢日期和是否強制執行
            getFileInfoParameter(ref strRunDate, ref strRunFlag);

            //匯入日期            
            if (string.IsNullOrEmpty(strRunDate) == true)
            {
                //若沒指定日期就是要查前一天,但放置的目錄還是當天
                strRunDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            }
            #endregion

            int intDataCnt = 0;
            DataTable dt = getData(strRunDate, strRunFlag);
            if (dt != null)
            {
                intDataCnt = dt.Rows.Count;
            }
            strErrMsg = SendTxt(dt, strRunDate);

            //記錄job結束時間
            EndTime = DateTime.Now;

            #region job結束日誌記錄            

            //*判斷job完成狀態
            string strJobStatus = "S";
            string strReturnMsg = string.Empty;
            strReturnMsg += string.Format("資料筆數:{0}", intDataCnt);

            //執行結果 寫入 DB            
            if (!string.IsNullOrEmpty(strErrMsg))
            {
                strReturnMsg += ". 失敗訊息:" + strErrMsg;
                strJobStatus = "F";
                result = false;
                JobHelper.Write(strJobId, strErrMsg);
            }

            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);

            #endregion

            strRETSTR = "*********** " + strJobId + " End **************";
            JobHelper.Write(strJobId, strRETSTR);

            //一律更新上傳狀態, 若需重產, 就 ForceImp 給 "Y" 值
            updateUploadflag(strRunDate);
            strErrMsg = strReturnMsg;
        }
        catch (Exception ex)
        {
            result = false;

            JobHelper.Write(strJobId, ex.Message);
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, "F", "發生錯誤：" + ex.Message);

            //*JOB失敗發送MAIL 
            strErrMsg = ex.Message;
            //sendMailmsg(strErrMsg);
        }
        finally
        {
            //清空參數值
            Com_FileInfo.UpdateParameter(strJobId);
        }

        sendMailmsg(strErrMsg);

        return result;
    }

    private DataTable getData(string strRunDate, string strRunFlag)
    {
        SqlCommand sqlcmd = new SqlCommand();
        string sql = "Select System_Code,Login_Account_Nbr,convert(char(23),Query_Datetime,121)Query_Datetime,AP_Txn_Code,Server_Name,User_Terminal,AP_Account_Nbr,Txn_Type_Code," +
            "Statement_Text,Object_Name,Txn_Status_Code,Customer_Id,Account_Nbr,Branch_Nbr,Role_Id,Import_Source,As_Of_Date " +
            "From L_AP_LOG (nolock) where As_Of_Date=@As_Of_Date";

        sqlcmd.Parameters.Add(new SqlParameter("@As_Of_Date", strRunDate));
        //只要不是強制執行, 那就只需撈 IsUpload= 0 的資料
        if (strRunFlag != "Y" && strRunFlag != "1")
        {
            sql += " and IsUpload=@IsUpload";
            sqlcmd.Parameters.Add(new SqlParameter("@IsUpload", "0"));
        }

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
    /// 查看tbl_FileInfo 
    /// </summary>
    /// <param name="strRunDate"></param>
    /// <param name="strRunFlag">是否要強制匯入，(ForceImp=Y) =>要</param>
    private void getFileInfoParameter(ref string strRunDate, ref string strRunFlag)
    {
        DataHelper dh = new DataHelper("Connection_System");
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandTimeout = int.Parse(UtilHelper.GetAppSettings("SqlCmdTimeoutMax"));
        sqlcmd.CommandText = "Select isnull(Parameter,'') Parameter,isnull(ForceImp,'') ForceImp From tbl_FileInfo where Job_ID='" + strJobId + "'";

        DataSet ds = new DataSet();
        ds = dh.ExecuteDataSet(sqlcmd);
        try
        {
            if (ds == null)
            {
            }
            else
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dt = ds.Tables[0].Rows[0];
                    strRunDate = dt.ItemArray[0].ToString().Trim();
                    strRunFlag = dt.ItemArray[1].ToString().Trim();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 讀取FTP設定
    /// </summary>
    /// <param name="Jobid"></param>
    /// <returns></returns>
    private ftpInfo GetFtpSetting()
    {
        ftpInfo result = new ftpInfo();
        DataTable dt = new DataTable();
        Com_FileInfo.selectFileInfo(ref dt, strJobId);
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        result.FtpFileName = dt.Rows[0]["FtpFileName"].ToString();
        result.FtpUserName = dt.Rows[0]["FtpUserName"].ToString();
        result.FtpPwd = RedirectHelper.GetDecryptString(dt.Rows[0]["FtpPwd"].ToString());
        result.FtpIP = dt.Rows[0]["FtpIP"].ToString();
        result.FtpPath = dt.Rows[0]["FtpPath"].ToString();
        result.ZipPwd = dt.Rows[0]["ZipPwd"].ToString();
        result.Parameter = dt.Rows[0]["Parameter"].ToString();
        result.ForceImp = dt.Rows[0]["ForceImp"].ToString();

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
            JobHelper.Write(strJobId, ex.ToString());
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
        ftpInfo _ftpInfo = GetFtpSetting();
        if (_ftpInfo == null)
        {
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
            FileTools.DeleteFile(fileFullNameD);
            FileTools.DeleteFile(fileFullNameH);
            FileTools.Create(fileFullNameD, sbDetail.ToString());
            FileTools.Create(fileFullNameH, sbHead.ToString());

            FileTools.CopyFile(fileFullNameD, fileFullNameD_new);
            FileTools.CopyFile(fileFullNameH, fileFullNameH_new);
        }
        catch (Exception ex)
        {
            result = string.Format("建檔失敗!({0})", ex.Message);
            return result;
        }

        //-----------------------------------------        
        //上傳檔案        
        try
        {
            FTPFactory objFtp_UpLoad = new FTPFactory(_ftpInfo.FtpIP, _ftpInfo.FtpPath, _ftpInfo.FtpUserName, _ftpInfo.FtpPwd, "21", folderName, "Y");
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
            }
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;

    }

    /// <summary>
    /// 寄信的方法
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
                JobHelper.Write(strJobId, "未設定通知信箱資料");
                return false;
            }
            List<string> MailUsers = new List<string>();
            for (int i = 0; i < mDt.Rows.Count; i++)
            {
                MailUsers.Add(mDt.Rows[i][1].ToString());
            }

            JobHelper.SendMail("", MailUsers.ToArray(), "SOC AP Log執行結果", logMsg);
        }

        return result;
    }

    /// <summary>
    /// 擴充原本沒有的屬性
    /// </summary>
    private class ftpInfo : Entity_FileInfo
    {
        private string _ForceImp;
        private string _Parameter;


        public string Parameter
        {
            get
            {
                return this._Parameter;
            }
            set
            {
                this._Parameter = value;
            }
        }



        public string ForceImp
        {
            get
            {
                return this._ForceImp;
            }
            set
            {
                this._ForceImp = value;
            }
        }
    }

}