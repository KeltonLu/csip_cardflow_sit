//******************************************************************
//*  功能說明：自動化簡訊處理-通知主機檔匯出
//*  作    者：zhiyuan
//*  創建日期：2010/06/8
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
//20161108 (U) by Tank, 調整SQL及判斷VD卡方式

using System;
using System.Data;
using System.Configuration;
using System.Text;
using Framework.Common.Logging;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using Framework.Data.OM.Collections;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM;
//20161108 (U) by Tank
using Framework.Data;
using System.Data.SqlClient;

/// <summary>
/// AutoNewsletterInfoMF 的摘要描述
/// </summary>
public class AutoNewsletterInfoMF : Quartz.IJob
//public class AutoNewsletterInfoMF
{
    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string strCreditFileName = string.Empty;   //信用卡文件名
    protected string strVDFileName = string.Empty;   //VD/金融卡文件名
    protected char cOutType = ' ';   //A:招領及退件都執行  B:執行退件  C:執行招領
    protected string strFolderName = string.Empty;
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間;
    protected DateTime EndTime;
    StringBuilder sbFileInfo = new StringBuilder();
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
    //public void Execute()
    {
        try
        {

            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            //strJobId = "0115_1";

            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);

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

            #region 檢測JOB今日是否為工作日，工作日才要執行
            if (!BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
            {
                JobHelper.SaveLog(DateTime.Now.ToString("yyyyMMdd") + "今日非工作日！", LogState.Info);
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


            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job5000115, "OU");

            #region 查詢需匯出的資料
            //*無JOB交換當信息或查詢失敗
            if (!JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
                return;
            }

            if (dtFileInfo.Rows.Count > 0)
            {
                strFolderName = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["UpLoadFilePath"] + "\\" + strJobId + "\\" + strJobId + StartTime.ToString("yyyyMMddHHmmss");
                foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                {
                    sbFileInfo.Length = 0;
                    //*招領及退件都執行
                    if (rowFileInfo["PExpFlg"].ToString().Equals("1") && rowFileInfo["BExpFlg"].ToString().Equals("1"))
                    {
                        cOutType = 'A';
                    }
                    else
                    {
                        //*執行招領
                        if (rowFileInfo["PExpFlg"].ToString().Equals("1"))
                        {
                            cOutType = 'C';
                        }

                        //執行退件
                        if (rowFileInfo["BExpFlg"].ToString().Equals("1"))
                        {
                            cOutType = 'B';
                        }
                    }
                    GetFileName(cOutType);
                    OutFile(cOutType, rowFileInfo["FunctionFlg"].ToString(), rowFileInfo);
                }
            }
            #endregion

            #region 登陸ftp上載文件
            String errMsg = "";
            for (int j = 0; j < dtLocalFile.Rows.Count; j++)
            {
                string strFtpUploadPath = dtLocalFile.Rows[j]["FtpPath"].ToString() + "//";
                string strFtpIp = dtLocalFile.Rows[j]["FtpIP"].ToString();
                string strFtpUserName = dtLocalFile.Rows[j]["FtpUserName"].ToString();
                string strFtpPwd = dtLocalFile.Rows[j]["FtpPwd"].ToString();

                FTPFactory objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                if (objFtp.Upload(strFtpUploadPath, dtLocalFile.Rows[j]["TxtFileName"].ToString(), dtLocalFile.Rows[j]["LocalFilePath"].ToString()))
                {
                    //*更新上載狀態為S
                    dtLocalFile.Rows[j]["UploadStates"] = "S";
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000033, dtLocalFile.Rows[j]["LocalFilePath"].ToString()), LogState.Info);
                }
                else
                {
                    errMsg += (errMsg == "" ? "" : "、") + dtLocalFile.Rows[j]["TxtFileName"].ToString();
                    //*更新上載狀態為F
                    dtLocalFile.Rows[j]["UploadStates"] = "F";
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000034, dtLocalFile.Rows[j]["LocalFilePath"].ToString()));
                    //*發送登陸FTP失敗郵件
                    // SendMail(dtLocalFile.Rows[j]["TxtFileName"].ToString(), Resources.JobResource.Job0000008);
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

    /// <summary>
    /// 功能說明:產生資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
    /// 修改記錄:
    /// </summary>
    /// <param name="OutType"></param>
    /// <param name="strFunFlg"></param>
    private void OutFile(char OutType, string strFunFlg, DataRow drLocalFile)
    {
        switch (OutType)
        {
            case 'A':
                OutPFile(strFunFlg);
                OutBFile(strFunFlg);
                //*創建文件
                CreateFile(strFunFlg, drLocalFile);
                break;
            case 'B':
                OutBFile(strFunFlg);
                //*創建文件
                CreateFile(strFunFlg, drLocalFile);
                break;
            case 'C':
                OutPFile(strFunFlg);
                //*創建文件
                CreateFile(strFunFlg, drLocalFile);
                break;
        }
    }

    /// <summary>
    /// 功能說明:創建匯出檔案
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="strFunFlg"></param>
    /// <param name="drLocalFile"></param>
    private void CreateFile(string strFunFlg, DataRow drLocalFile)
    {
        string strFileName = string.Empty;
        if (strFunFlg.Equals("1"))
        {
            strFileName = strCreditFileName;
        }
        if (strFunFlg.Equals("2"))
        {
            strFileName = strVDFileName;
        }
        FileTools.EnsurePath(strFolderName);
        //if (string.IsNullOrEmpty(sbFileInfo.ToString()))
        //{
        //    sbFileInfo.Append(" ");
        //}
        //StreamWriter sw = new StreamWriter(strFolderName + "\\" + strFileName, true, Encoding.Default);
        //sw.WriteLine(sbFileInfo.ToString());
        //sw.Close();
        //sw.Dispose();

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

    /// <summary>
    /// 功能說明:創建招領資料
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="strFunFlg"></param>
    private void OutPFile(string strFunFlg)
    {
        DataTable dtAllData = new DataTable();
        DataRow[] drRows = null;
        string strTmp = string.Empty;
        if (BRM_TCardBaseInfo.GetPOutData(ref dtAllData))
        {
            if (strFunFlg.Equals("1"))
            {

                //*信用卡
                //201304 悠遊Debit 需求修改
                //20161108 (U) by Tank, 調整為取SQL
                //drRows = dtAllData.Select("cardtype not in ('000','013','370','035','571','018','019','039','040','038','037')");
                drRows = dtAllData.Select("cardtype not in (" + GetCardTypeString("1") + ")");
                foreach (DataRow dr in drRows)
                {
                    strTmp = JobHelper.SetStrngValue(dr["ID"].ToString(), 16);
                    strTmp += JobHelper.SetStrngValue(dr["CardNo"].ToString(), 16);
                    strTmp += "\r\n";
                    sbFileInfo.Append(strTmp);
                }
                //*Type='0'為郵寄
                InsertToCallCust('0', drRows, strCreditFileName);
            }

            if (strFunFlg.Equals("2"))
            {

                //*VD/金融卡
                // 簡訊要排除晶片晶融卡 000
                //drRows = dtAllData.Select("cardtype in ('000','013','370','035','571')");
                //20161108 (U) by Tank, 調整為取SQL
                //drRows = dtAllData.Select("cardtype in ('013','370','035','571','039','040')");
                drRows = dtAllData.Select("cardtype in (" + GetCardTypeString("2") + ")");   //2:取VD卡
                strTmp = string.Empty;
                foreach (DataRow dr in drRows)
                {
                    strTmp = JobHelper.SetStrngValue(dr["ID"].ToString(), 16);
                    strTmp += JobHelper.SetStrngValue(dr["CardNo"].ToString(), 16);
                    strTmp += "\r\n";
                    sbFileInfo.Append(strTmp);
                }
                //*將產出資料匯入到簡訊處理信息檔
                //*Type='0'為郵寄
                InsertToCallCust('0', drRows, strVDFileName);
            }
        }
    }

    /// <summary>
    /// 功能說明:創建退件資料
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="strFunFlg"></param>
    private void OutBFile(string strFunFlg)
    {
        DataTable dtAllData = new DataTable();
        DataRow[] drRows = null;
        string strTmp = string.Empty;
        if (BRM_CardBackInfo.GetBOutData(ref dtAllData))
        {
            if (strFunFlg.Equals("1"))
            {
                //*信用卡
                //max add 悠遊Debit 需求修改
                //20161108 (U) by Tank, 調整為取SQL
                //drRows = dtAllData.Select("cardtype not in ('000','013','370','035','571','018','019','039','040','038','037')");
                drRows = dtAllData.Select("cardtype not in (" + GetCardTypeString("1") + ")");
                foreach (DataRow dr in drRows)
                {
                    strTmp = JobHelper.SetStrngValue(dr["ID"].ToString(), 16);
                    strTmp += JobHelper.SetStrngValue(dr["CardNo"].ToString(), 16);
                    strTmp += "\r\n";
                    sbFileInfo.Append(strTmp);
                }
                //*將產出資料匯入到簡訊處理信息檔
                //*Type='1'為退件
                InsertToCallCust('1', drRows, strCreditFileName);
            }

            if (strFunFlg.Equals("2"))
            {
                //*VD/金融卡
                //max add 悠遊Debit 需求修改
                //20161108 (U) by Tank, 調整為取SQL
                //drRows = dtAllData.Select("cardtype in ('018','013','370','035','571','039','040','037')");
                drRows = dtAllData.Select("cardtype in (" + GetCardTypeString("2") + ")"); //2取VD卡 3取現金卡
                strTmp = string.Empty;
                foreach (DataRow dr in drRows)
                {
                    //二CALL 的時候要排除VD (370 013 )
                    //201304 悠遊Debit 需求修改
                    //20161108 (U) by Tank, 調整判斷VD卡方式
                    //if (dr["cardtype"].ToString().Equals("370") || dr["cardtype"].ToString().Equals("013") || dr["cardtype"].ToString().Equals("571") || dr["cardtype"].ToString().Equals("035") || dr["cardtype"].ToString().Equals("039") || dr["cardtype"].ToString().Equals("040") || dr["cardtype"].ToString().Equals("038") || dr["cardtype"].ToString().Equals("037"))
                    if (funCheckVDCard(dr["cardtype"].ToString()))
                    {
                        if (dr["Exp_Count"].ToString().Equals("") || dr["Exp_Count"].ToString().Equals("0"))
                        {
                            strTmp = JobHelper.SetStrngValue(dr["ID"].ToString(), 16);
                            strTmp += JobHelper.SetStrngValue(dr["CardNo"].ToString(), 16);
                            strTmp += "\r\n";
                            sbFileInfo.Append(strTmp);
                        }
                    }
                    else
                    {
                        strTmp = JobHelper.SetStrngValue(dr["ID"].ToString(), 16);
                        strTmp += JobHelper.SetStrngValue(dr["CardNo"].ToString(), 16);
                        strTmp += "\r\n";
                        sbFileInfo.Append(strTmp);
                    }

                }
                //*將產出資料匯入到簡訊處理信息檔
                //*Type='1'為退件
                InsertToCallCust('1', drRows, strVDFileName);
            }
        }
    }

    /// <summary>
    /// 功能說明:寫入到簡訊檔
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="dtRows"></param>
    /// <param name="strExpFile"></param>
    /// <returns></returns>
    private bool InsertToCallCust(char Type, DataRow[] dtRows, string strExpFile)
    {
        EntitySet<Entity_SLetterInfoCallCust> SetCallCust = new EntitySet<Entity_SLetterInfoCallCust>();
        foreach (DataRow dr in dtRows)
        {
            Entity_SLetterInfoCallCust CallCust = new Entity_SLetterInfoCallCust();
            CallCust.ID = dr["ID"].ToString();  //ID
            CallCust.CardNo = dr["CardNo"].ToString();  //卡號
            CallCust.Source_Flg = Type.ToString();   //來源標識
            CallCust.Exp_File = strExpFile; //匯出通知主機檔名
            CallCust.Exp_Date = DateTime.Now.ToString("yyyy/MM/dd");    //匯出通知主機日期
            switch (Type)
            {
                case '0':
                    CallCust.Maildate = dr["Maildate"].ToString();  //郵寄日期
                    CallCust.Mailno = dr["Mailno"].ToString();    //郵寄號碼
                    break;
                case '1':
                    CallCust.Serial_no = dr["serial_no"].ToString(); //序號
                    break;
            }
            if (!BRM_SLetterInfoCallCust.GetInfoCallCust(dr["ID"].ToString(), dr["CardNo"].ToString(), DateTime.Now.ToString("yyyy/MM/dd")))
            {
                SetCallCust.Add(CallCust);
            }
        }

        if (BRM_SLetterInfoCallCust.BatInsertFor0115_1(SetCallCust))
        {
            //UpdateExpCount(Type, dtRows);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 回寫產出次數
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="drRows"></param>
    private void UpdateExpCount(char cType, DataRow[] drRows)
    {
        string strErrMsg = string.Empty;
        if (cType == '0')
        {
            foreach (DataRow dr in drRows)
            {
                EntityM_PostSend PostSend = new EntityM_PostSend();
                PostSend.maildate = dr["maildate"].ToString();
                PostSend.mailno = dr["mailno"].ToString();
                PostSend.M_date = dr["M_date"].ToString();
                PostSend.Exp_Count = ExpCount(dr["Exp_Count"].ToString().Trim()).ToString();
                PostSend.Exp_Date = DateTime.Now.ToString("yyyy/MM/dd");
                SqlHelper sqlHelp = new SqlHelper();
                sqlHelp.AddCondition(EntityM_PostSend.M_maildate, Operator.Equal, DataTypeUtils.String, PostSend.maildate);
                sqlHelp.AddCondition(EntityM_PostSend.M_mailno, Operator.Equal, DataTypeUtils.String, PostSend.mailno);
                sqlHelp.AddCondition(EntityM_PostSend.M_M_date, Operator.Equal, DataTypeUtils.String, PostSend.M_date);

                BRM_PostSend.Update(PostSend, sqlHelp.GetFilterCondition(), ref strErrMsg, "Exp_Count", "Exp_Date");
            }
        }
        if (cType == '1')
        {
            foreach (DataRow dr in drRows)
            {
                Entity_CardBackInfo CardBack = new Entity_CardBackInfo();
                CardBack.serial_no = dr["serial_no"].ToString();
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

    /// <summary>
    /// 功能說明:根據執行的類型返回要產生的文件名稱
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/08
    /// 修改記錄:
    /// </summary>
    /// <param name="OutType"></param>
    private void GetFileName(char OutType)
    {
        string strDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
        switch (OutType)
        {
            case 'A':
                strCreditFileName = "P" + strDate + "-B" + strDate + "P4.txt";
                strVDFileName = "P" + strDate + "-B" + strDate + "P4D.txt";
                break;
            case 'B':
                strCreditFileName = "B" + strDate + "P4.txt";
                strVDFileName = "B" + strDate + "P4D.txt";
                break;
            case 'C':
                strCreditFileName = "P" + strDate + "P4.txt";
                strVDFileName = "P" + strDate + "P4D.txt";
                break;
        }
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
        string strStatus = string.Empty;
        string strMessage = string.Empty;
        DataRow[] rowStatus = dtLocalFile.Select("UploadStates='F'");
        //*匯出成功
        if (rowStatus != null && rowStatus.Length > 0)
        {
            strStatus = "F";
            strMessage = Resources.JobResource.Job010201;
        }
        //*匯出失敗
        else
        {
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
            string strSubject = string.Format(dtCallMail.Rows[0]["MailTittle"].ToString(), Resources.JobResource.Job5000115, strFileName);
            string strBody = string.Format(dtCallMail.Rows[0]["MailContext"].ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strFileName, Resources.JobResource.Job5000115, strCon);
            JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
        }
    }
    #endregion

    //20161108 (U) by Tank, 判斷是否為VD卡
    protected bool funCheckVDCard(string strCardType)
    {
        DataHelper dh = new DataHelper();
        SqlCommand sqlcmd = new SqlCommand();
        DataSet ds = new DataSet();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 240;
            sqlcmd.CommandText = @"select CardTypeName from tbl_CardType where BankCardFlag='Y' and BankCardType ='2' and CardType=@CardType ";

            SqlParameter ParCardType = new SqlParameter("@CardType", strCardType);
            sqlcmd.Parameters.Add(ParCardType);
            ds = dh.ExecuteDataSet(sqlcmd);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }

    //20161108 (U) by Tank, 取CardType組成字串
    protected string GetCardTypeString(string strFlag)
    {
        DataHelper dh = new DataHelper();
        SqlCommand sqlcmd = new SqlCommand();
        DataSet ds = new DataSet();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 240;

            string sql = string.Empty;
            switch (strFlag)
            {
                case "1":   //取銀行卡
                    sql = @"select CardType from tbl_CardType where BankCardFlag='Y'";
                    break;

                case "2":   //取 2:VD卡 跟 3:現金卡
                    sql = @"select CardType from tbl_CardType where BankCardFlag='Y' and BankCardType in ('2','3')";
                    break;
            }

            sqlcmd.CommandText = sql;
            ds = dh.ExecuteDataSet(sqlcmd);

            string strCardTypeString = string.Empty;
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow rows in ds.Tables[0].Rows)
                {
                    strCardTypeString += ",'" + rows[0].ToString() + "'";
                }

                return strCardTypeString.Substring(1);
            }
            else
            {
                return "";
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }
}
