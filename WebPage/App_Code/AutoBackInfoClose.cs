//******************************************************************
//*  功能說明：自動化卡回註銷
//*  作    者：linda
//*  創建日期：2010/07/09
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
//20161108 (U) by Tank, 調整fun來源

using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using BusinessRules;
using EntityLayer;
using System.Collections;
using Framework.Data.OM.Collections;
using CSIPCommonModel.EntityLayer;
//20161108 (U) by Tank
using System.Data.SqlClient;

/// <summary>
/// AutoBackInfoClose 的摘要描述
/// </summary>
public class AutoBackInfoClose : Quartz.IJob
{
    #region job基本參數設置
    protected string strJobId;
    protected string strFunctionKey = "06";
    private string strSessionId = "";
    protected JobHelper JobHelper = new JobHelper();
    protected DataTable dtFileInfo;
    protected DataTable dtOASAInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;
    protected int SCount = 0;
    protected int FCount = 0;
    protected string strReturnMsg = string.Empty;


    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:Linda
    /// 創建時間:2010/06/05
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    {
        try
        {
            #region 获取jobID
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString().Trim();
            JobHelper.strJobId = strJobId;
            //strJobId = "0112";
            #endregion

            #region 記錄job啟動時間
            StartTime = DateTime.Now;

            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
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


            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000112, "IM");

            #region 結案操作
            DataTable dtMinusDate = new DataTable();
            if (GetMinusDate(ref dtMinusDate))
            {
                strReturnMsg = "";
                for (int intCardType = 1; intCardType <= dtMinusDate.Rows.Count; intCardType++)
                {
                    DataRow[] rowMinusDate = dtMinusDate.Select("PROPERTY_CODE='" + intCardType + "'");
                    string strMinusDate = rowMinusDate[0]["PROPERTY_NAME"].ToString().Trim();
                    if (intCardType == 1)//信用卡 先獲取注銷資料再結案
                    {
                        DataTable dtCardEndInfo = new DataTable();
                        //20161108 (U) by Tank, fun底層拉出
                        //if (BRM_CardBackInfo.GetCardEndInfo(intCardType, strMinusDate, ref dtCardEndInfo))
                        if (GetCardEndInfo_New(intCardType, strMinusDate, ref dtCardEndInfo))
                        {
                            if (dtCardEndInfo.Rows.Count > 0)
                            {
                                dtOASAInfo = dtCardEndInfo;//先獲取注銷資料
                            }
                        }
                        //20161108 (U) by Tank, fun底層拉出
                        //if (BRM_CardBackInfo.CardBackInfoEnd(intCardType, strMinusDate))//結案
                        if (CardBackInfoEnd_New(intCardType, strMinusDate))//結案
                        {
                            SCount++;
                            strReturnMsg += string.Format(Resources.JobResource.Job0112006, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                        }
                        else
                        {
                            FCount++;
                            strReturnMsg += string.Format(Resources.JobResource.Job0112001, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0112001, DateTime.Now.ToString("yyyy/MM/dd")));
                        }

                    }
                    else///2金融卡/3現金卡/4e-Cash 直接結案
                    {
                        //20161108 (U) by Tank, fun底層拉出
                        //if (BRM_CardBackInfo.CardBackInfoEnd(intCardType, strMinusDate))
                        if (CardBackInfoEnd_New(intCardType, strMinusDate))
                        {
                            SCount++;
                            switch (intCardType)
                            {
                                case 2:
                                    strReturnMsg += string.Format(Resources.JobResource.Job0112007, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                                    break;
                                case 3:
                                    strReturnMsg += string.Format(Resources.JobResource.Job0112008, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                                    break;
                                case 4:
                                    strReturnMsg += string.Format(Resources.JobResource.Job0112009, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                                    break;
                            }

                        }
                        else
                        {
                            FCount++;
                            switch (intCardType)
                            {
                                case 2:
                                    strReturnMsg += string.Format(Resources.JobResource.Job0112002, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0112002, DateTime.Now.ToString("yyyy/MM/dd")));
                                    break;
                                case 3:
                                    strReturnMsg += string.Format(Resources.JobResource.Job0112003, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0112003, DateTime.Now.ToString("yyyy/MM/dd")));
                                    break;
                                case 4:
                                    strReturnMsg += string.Format(Resources.JobResource.Job0112004, DateTime.Now.ToString("yyyy/MM/dd")) + " ";
                                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0112004, DateTime.Now.ToString("yyyy/MM/dd")));
                                    break;
                            }

                        }

                    }

                }
            }

            #endregion

            #region OASA注銷

            DataTable dtFileInfoOASA = new DataTable();

            if (JobHelper.SearchFileInfo(ref dtFileInfoOASA, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                if (dtFileInfoOASA.Rows.Count > 0)
                {
                    string strBlkCode = dtFileInfoOASA.Rows[0]["BLKCode"].ToString().Trim();
                    string strMemo = dtFileInfoOASA.Rows[0]["MEMO"].ToString().Trim();
                    string strReasonCode = dtFileInfoOASA.Rows[0]["ReasonCode"].ToString().Trim();
                    string strActionCode = dtFileInfoOASA.Rows[0]["ActionCode"].ToString().Trim();

                    int intSOASACount = 0;
                    int intFOASACount = 0;
                    string strCancelOASAFile = string.Empty;
                    string strOASAUserId = string.Empty;

                    if (GetOASAFileName(ref strCancelOASAFile))
                    {
                        EntitySet<Entity_CancelOASA_Detail> SetCancelOASADetail = new EntitySet<Entity_CancelOASA_Detail>();

                        if (dtOASAInfo != null)
                        {

                            foreach (DataRow rowOASAInfo in dtOASAInfo.Rows)
                            {
                                string strCardNo = rowOASAInfo["CardNo"].ToString().Trim();
                                string strMemoLog = string.Empty;
                                string strBlockLog = string.Empty;
                                string strSFFlg = string.Empty;
                                if (this.HtgOASAAdd(strCardNo, strBlkCode, strMemo, strReasonCode, strActionCode, ref strMemoLog, ref strBlockLog, ref strOASAUserId, context))
                                {
                                    intSOASACount++;
                                    strSFFlg = "1";
                                }
                                else
                                {
                                    intFOASACount++;
                                    strSFFlg = "2";
                                }

                                Entity_CancelOASA_Detail CancelOASADetail = new Entity_CancelOASA_Detail();
                                CancelOASADetail.CancelOASAFile = strCancelOASAFile;
                                CancelOASADetail.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                                CancelOASADetail.CardNo = strCardNo;
                                CancelOASADetail.BlockCode = strBlkCode;
                                CancelOASADetail.ActionCode = strActionCode;
                                CancelOASADetail.Memo = strMemo;
                                CancelOASADetail.ReasonCode = strReasonCode;
                                CancelOASADetail.MemoLog = strMemoLog;
                                CancelOASADetail.BlockLog = strBlockLog;
                                CancelOASADetail.SFFlg = strSFFlg;
                                SetCancelOASADetail.Add(CancelOASADetail);
                            }

                            EntitySet<Entity_CancelOASA> SetCancelOASA = new EntitySet<Entity_CancelOASA>();
                            Entity_CancelOASA CancelOASA = new Entity_CancelOASA();
                            string strOASAFile = string.Empty;
                            CancelOASA.CancelOASAFile = strCancelOASAFile;
                            CancelOASA.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                            CancelOASA.CancelOASAUser = strOASAUserId;
                            CancelOASA.TotalCount = intFOASACount + intSOASACount;
                            CancelOASA.SCount = intSOASACount;
                            CancelOASA.FCount = intFOASACount;
                            CancelOASA.CancelOASASource = "2";
                            SetCancelOASA.Add(CancelOASA);
                            if (BRM_CancelOASA.BatInsert(SetCancelOASA))
                            {
                                string strMsgID = string.Empty;
                                BRM_CancelOASADetail.BatInsert(SetCancelOASADetail, ref strMsgID);
                            }

                        }
                    }

                }
            }
            else
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
            }
            MainFrameInfoOASA.ClearHtgSessionJob(ref strSessionId);
            //}
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
        //*判斷job完成狀態
        string strJobStatus = JobHelper.GetJobStatus(SCount, FCount);
        JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);

    }
    #endregion

    #region 獲取日期條件
    /// <summary>
    /// 功能說明:GetMinusDate獲取日期條件
    /// 作    者:linda
    /// 創建時間:2010/07/08
    /// 修改記錄:
    /// </summary>
    /// <param name="dtMinusDate"></param>
    public bool GetMinusDate(ref DataTable dtMinusDate)
    {
        bool bolGetMinusDate = false;
        DataTable dt = new DataTable();
        string strMsgID = string.Empty;

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "22", ref dt))
        {
            if (dt.Rows.Count > 0)
            {
                dtMinusDate = dt;
                bolGetMinusDate = true;
            }
        }
        return bolGetMinusDate;
    }
    #endregion
    #region 獲取注銷檔名
    /// <summary>
    /// 功能說明:GetOASAFileName獲取注銷檔名
    /// 作    者:linda
    /// 創建時間:2010/07/08
    /// 修改記錄:
    /// </summary>
    /// <param name="dtMinusDate"></param>
    public bool GetOASAFileName(ref string strOASAFileName)
    {
        DataTable dtOASAFileName = new DataTable();
        if (BRM_CancelOASAFileName.SearchOASAFileName(ref dtOASAFileName))
        {
            if (dtOASAFileName.Rows.Count > 0)
            {
                strOASAFileName = Convert.ToString(Convert.ToDouble(dtOASAFileName.Rows[0]["CancelOASAFile"].ToString().Trim()) + 1);
            }
            else
            {
                strOASAFileName = DateTime.Now.ToString("yyyyMMdd") + "001";
            }
            if (!BRM_CancelOASAFileName.InsertOASAFileName(strOASAFileName))
            {
                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0112005, strOASAFileName));
                return false;
            }
            else
            {
                return true;
            }

        }
        else
        {
            return false;
        }

    }
    #endregion

    #region 新增主機OASA資料
    /// <summary>
    /// 功能說明:新增主機OASA資料
    /// 作    者:Linda
    /// 創建時間:2010/07/07
    /// 修改記錄:
    /// <param name="htReturn">主機傳回資料</param>
    /// <param name="dtblUpdateData">更改的主機欄位信息的DataTable</param>
    /// <param name="strDesp">異動BLK CODE欄位名稱</param>
    /// <returns>true成功，false失敗</returns>
    private bool HtgOASAAdd(string strCardNo, string strBlkCode, string strMemo, string strReasonCode, string strActionCode, ref string strErrorMsg, ref string strBLCLog, ref string strUserId, Quartz.JobExecutionContext context)
    {
        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料

        string strPurgeDateReq = DateTime.Now.AddMonths(3).ToString("MMdd");

        htInput.Add("sessionId", strSessionId);

        htInput.Add("FUNCTION_CODE", "A");
        htInput.Add("SOURCE_CODE", "Z");//*交易來源別
        htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
        htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
        htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]

        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("OASA_BLOCK_CODE", strBlkCode);//*BLK CODE
        htInput.Add("OASA_MEMO", strMemo);//*MEMO
        htInput.Add("OASA_REASON_CODE", strReasonCode);//*REASON CODE
        htInput.Add("OASA_ACTION_CODE", strActionCode);//*ACTION CODE

        htInput.Add("OASA_PURGE_DATE", strPurgeDateReq);//*PURGE DATE

        //*提交OASA_P4_Submit主機資料

        Hashtable htResultA = MainFrameInfoOASA.GetMainFrameInfo(MainFrameInfoOASA.HtgType.P4_JCAX, htInput, false, "100", GetAgentInfo(context));
        if (!htResultA.Contains("HtgMsg"))
        {
            strErrorMsg = "";//*主機返回成功訊息
            strBLCLog = "";
            strUserId = htResultA["USER_ID"].ToString().Trim();
            strSessionId = htResultA["sessionId"].ToString().Trim();
            return true;
        }
        else
        {
            strErrorMsg = htResultA["HtgMsg"].ToString().Trim();
            strSessionId = "";
            if (htResultA.Count > 2)
            {
                strBLCLog = htResultA["OASA_BLOCK_CODE"].ToString().Trim();
                strUserId = htResultA["USER_ID"].ToString().Trim();
                strSessionId = htResultA["sessionId"].ToString().Trim();
            }
            return false;
        }

    }
    #endregion

    #region 得到登陸主機信息
    /// <summary>
    /// 得到登陸主機信息
    /// </summary>
    /// <returns>EntityAGENT_INFO</returns>
    private EntityAGENT_INFO GetAgentInfo(Quartz.JobExecutionContext context)
    {
        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        if (jobDataMap != null && jobDataMap.Count > 0)
        {
            eAgentInfo.agent_id = jobDataMap.GetString("userId");
            eAgentInfo.agent_pwd = jobDataMap.GetString("passWord");
            eAgentInfo.agent_id_racf = jobDataMap.GetString("racfId");
            eAgentInfo.agent_id_racf_pwd = jobDataMap.GetString("racfPassWord");
        }
        return eAgentInfo;
    }
    #endregion

    //20161108 (U) by Tank 從底層copy出來
    /// <summary>
    /// 功能說明:查詢注銷結案資料
    /// 作    者:linda
    /// 創建時間:2010/07/08
    /// 修改記錄:
    /// </summary>
    /// <param name="dtLastCloseDate"></param>
    /// <returns></returns>
    public static bool GetCardEndInfo_New(int intCardType, string strMinusDate, ref DataTable dtCardEndInfo)
    {
        try
        {

            int newStrMinusDate = int.Parse(strMinusDate) * -1;
            string sql = @"Select * from dbo.tbl_Card_BackInfo";
            sql += " where CardBackStatus='0'";
            sql += " and Backdate<=convert(nvarchar(10), dateadd(mm,@strMinusDate,getdate()),111)";
            switch (intCardType)
            {
                case 1: //信用卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype not in ('000','013','370','018','019','039','040','038','037','041')";
                    sql += " and cardtype not in (select CardType from tbl_CardType where BankCardFlag='Y')";
                    break;
                case 2://金融卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype ='000'";
                    sql += " and cardtype in (select CardType from tbl_CardType where BankCardFlag='Y' and BankCardType='1')";
                    break;
                case 3://現金卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype ='018'";
                    sql += " and cardtype in (select CardType from tbl_CardType where BankCardFlag='Y' and BankCardType='3')";
                    break;
                case 4://e-Cash卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype ='019'";
                    sql += " and cardtype in (select CardType from tbl_CardType where BankCardFlag='Y' and BankCardType='4')";
                    break;
            }

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@strMinusDate", newStrMinusDate));
            DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd);
            if (ds != null)
            {
                dtCardEndInfo = ds.Tables[0];
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception exp)
        {
            BRM_CardBackInfo.SaveLog(exp.Message);
            return false;
        }
    }

    //20161108 (U) by Tank 從底層copy出來
    /// <summary>
    /// 功能說明:注銷結案
    /// 作    者:Linda
    /// 創建時間:2010/07/08
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDataChange"></param>
    /// <param name="dtUnableCard"></param>
    /// <param name="dtChangeCard"></param>
    public static bool CardBackInfoEnd_New(int intCardType, string strMinusDate)
    {
        try
        {
            int newStrMinusDate = int.Parse(strMinusDate) * -1;

            string sql = "update dbo.tbl_Card_BackInfo";
            sql += " set Enditem='6',EndFunction='1',Enduid='sys',Enddate=convert(nvarchar(10),getdate(),111),CardBackStatus='2',Closedate=convert(nvarchar(10),getdate(),111)";
            sql += " where CardBackStatus='0'";
            sql += " and Backdate<=convert(nvarchar(10), dateadd(mm,@strMinusDate,getdate()),111)";
            switch (intCardType)
            {
                case 1: //信用卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype not in ('000','013','370','018','019','039','040','038','037','041')";
                    sql += " and cardtype not in (select CardType from tbl_CardType where BankCardFlag='Y')";
                    break;
                case 2://金融卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype ='000'";
                    sql += " and cardtype in (select CardType from tbl_CardType where BankCardFlag='Y' and BankCardType='1')";
                    break;
                case 3://現金卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype ='018'";
                    sql += " and cardtype in (select CardType from tbl_CardType where BankCardFlag='Y' and BankCardType='3')";
                    break;
                case 4://e-Cash卡
                    //20161108 (U) by Tank, 調整為取SQL
                    //sql += " and cardtype ='019'";
                    sql += " and cardtype in (select CardType from tbl_CardType where BankCardFlag='Y' and BankCardType='4')";
                    break;
            }

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@strMinusDate", newStrMinusDate));
            if (BRM_CardBackInfo.Update(sqlcmd))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception exp)
        {
            BRM_CardBackInfo.SaveLog(exp.Message);
            return false;
        }
    }
}
