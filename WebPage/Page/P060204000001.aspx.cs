//******************************************************************
//*  功能說明：人工注銷UI層

//*  作    者：Linda
//*  創建日期：2010/07/15
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Common.Utility;
using Framework.Data.OM.Collections;
using CSIPCommonModel.EntityLayer;

public partial class Page_P060204000001 : PageBase
{
    #region 事件
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            initTxt();
            BindBlockCode();
            BindMemo();
            rad020401.Checked=true;
            txtCardNoList.Enabled = true;
            fulFilePath.Enabled = false;
        }

    }

    protected void btnDoOASA_Click(object sender, EventArgs e)
    {
        string strCardNoList = "";
        string[] strCardNo = new string[] { };
        string strNumMessage="";
        string strCardTypeList="";
        string strCardTypeMessage="";
        string strOASACardNoList ="";
        string[] strOASACardNo = new string[] { };
        string strMessage = "";

        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020400_000");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");
        if (this.rad020401.Checked)
        {
            if (this.txtCardNoList.Text.Trim().Equals(string.Empty))
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06020400_012");
                return;
            }
            strCardNoList = this.txtCardNoList.Text.Trim().Replace("\r\n", ";");
            strCardNo = strCardNoList.Split(';');
            for (int i = 0; i < strCardNo.Length; i++)
            {
                if (strCardNo[i].ToString().Trim().Length == 16 && ValidateHelper.IsNum(strCardNo[i].ToString().Trim()))
                {
                    if (CheckCardType(strCardNo[i].ToString().Trim()))
                    {
                        strOASACardNoList += strCardNo[i].ToString().Trim() + " ";
                    }
                    else
                    {
                        strCardTypeList += strCardNo[i].ToString().Trim() + " ";
                    }
                }
                else
                {
                    strNumMessage += strCardNo[i].ToString().Trim() + " ";
                }
                
            }

            if (strNumMessage.Equals(string.Empty))
            {
                if (!strCardTypeList.Equals(string.Empty))
                {
                    strCardTypeMessage = string.Format(MessageHelper.GetMessage("06_06020400_000"), strCardTypeList);
                    jsBuilder.RegScript(this.Page, "alert('" + strCardTypeMessage + "');");
                }
                else
                {
                    if (!strOASACardNoList.Equals(string.Empty))
                    {
                        strOASACardNo = strOASACardNoList.Trim().Split(' ');

                        if (DoOASA(strOASACardNo))
                        {
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020400_005"));
                            //strMessage = strCardTypeMessage + " " + string.Format(MessageHelper.GetMessage("06_06020400_001"), strOASACardNo.Length);
                            strMessage = string.Format(MessageHelper.GetMessage("06_06020400_001"), strOASACardNo.Length);
                            jsBuilder.RegScript(this.Page, "alert('" + strMessage + "');");
                        }
                        else
                        {
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020400_006"));
                        }
                    }
                }
            }
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + string.Format(MessageHelper.GetMessage("06_06020400_002"),strNumMessage) + "');");
            }
        }
        else if (this.rad020402.Checked)
        {
            if (fulFilePath.HasFile)
            {
                #region 檢查上傳資料
                if (fulFilePath.PostedFile.FileName == "")
                {
                    MessageHelper.ShowMessage(this.UpdatePanel1, "06_06020400_013");
                    return;
                }

                if (fulFilePath.PostedFile.ContentLength <= 0)
                {
                    MessageHelper.ShowMessage(this.UpdatePanel1, "06_06020400_010");
                    return;
                }
                #endregion

                #region 取得上傳路徑
                //* 取得上傳路徑
                string strUploadPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath");

                //string strUploadPath = UtilHelper.GetAppSettings("UpLoadFilePath");

                if (!Directory.Exists(strUploadPath))
                {
                    //* Create the directory it does not exist.
                    Directory.CreateDirectory(strUploadPath);
                }
                #endregion

                #region 取得上傳信息
                UploadInfo Info = new UploadInfo();
                //* 大小
                Info.FILE_SIZE = fulFilePath.PostedFile.ContentLength / (double)1024.0;
                //* 原文件名
                Info.FILE_ORGINNAME = fulFilePath.PostedFile.FileName.Substring(fulFilePath.PostedFile.FileName.LastIndexOf("\\") + 1);
                //* 源文件擴展名
                Info.FILE_POSTNAME = fulFilePath.PostedFile.FileName.Substring(fulFilePath.PostedFile.FileName.LastIndexOf(".") + 1);
                //* 系統內另存為的新名稱
                Info.FILE_SYSNAME = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff") + "." + Info.FILE_POSTNAME;
                //* 系統內存放路徑

                Info.FILE_PATH = strUploadPath + "\\" + Info.FILE_SYSNAME;
                #endregion

                #region 檢查重複,並且儲存
                //* 檢查文檔是否已經存在
                if (File.Exists(Info.FILE_PATH))
                {
                    File.Delete(Info.FILE_PATH);
                }
                fulFilePath.PostedFile.SaveAs(Info.FILE_PATH);
                #endregion

                string strPath = Info.FILE_PATH.Trim();
                string strFunctionName = MessageHelper.GetMessage("06_06020400_008");
                string strMsgID = string.Empty;
                ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息

                DataTable dtDetail = null;                 //檢核結果列表
                //*檢核成功
                if (UploadCheck(strPath, strFunctionName, ref strMsgID, ref arrayErrorMsg, ref dtDetail))
                {
                    foreach (DataRow row in dtDetail.Rows)
                    {
                        if (CheckCardType(row["CardNo"].ToString().Trim()))
                        {
                            strOASACardNoList += row["CardNo"].ToString().Trim() + " ";
                        }
                        else
                        {
                            strCardTypeList += row["CardNo"].ToString().Trim() + " ";
                        }
                    }

                    if (!strCardTypeList.Equals(string.Empty))
                    {
                        strCardTypeMessage = string.Format(MessageHelper.GetMessage("06_06020400_000"), strCardTypeList);
                        jsBuilder.RegScript(this.Page, "alert('" + strCardTypeMessage + "');");
                    }
                    else
                    {
                        if (!strOASACardNoList.Equals(string.Empty))
                        {
                            strOASACardNo = strOASACardNoList.Trim().Split(' ');

                            if (DoOASA(strOASACardNo))
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020400_005"));
                                //strMessage = strCardTypeMessage + " " + string.Format(MessageHelper.GetMessage("06_06020400_001"), strOASACardNo.Length);
                                strMessage = string.Format(MessageHelper.GetMessage("06_06020400_001"), strOASACardNo.Length);
                                jsBuilder.RegScript(this.Page, "alert('" + strMessage + "');");
                            }
                            else
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020400_006"));
                            }
                        }
                    }
                }
                //*檢核失敗
                else
                {
                    string strError = string.Empty;
                    if (arrayErrorMsg.Count > 0)
                    {
                        for (int intError = 0; intError < arrayErrorMsg.Count; intError++)
                        {
                            strError += arrayErrorMsg[intError] + " ";
                        }
                        strError = string.Format(MessageHelper.GetMessage("06_06020400_011"), strError);
                    }
                    else
                    {
                        strError = Resources.JobResource.ResourceManager.GetString(strMsgID);
                    }

                    jsBuilder.RegScript(this.Page, "alert('" + strError + "');");
                }
            }
            else
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06020400_010");
            }

        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020400_007") + "');");
        }
    }

    protected void rad020401_CheckedChanged(object sender, EventArgs e)
    {
        fulFilePath.Enabled = false;
        txtCardNoList.Enabled = true;
    }
    protected void rad020402_CheckedChanged(object sender, EventArgs e)
    {
        fulFilePath.Enabled = true;
        txtCardNoList.Enabled = false;
    }



    /// <summary>
    /// 功能說明:綁定BlockCode
    /// 作    者:Linda
    /// 創建時間:2010/07/15
    /// 修改記錄:
    /// </summary>
    public void BindBlockCode()
    {
        string strMsgID = string.Empty;
        DataTable dtBlockCode = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "32", ref dtBlockCode))
        {
            this.dropBlkCode.DataSource = dtBlockCode;
            this.dropBlkCode.DataTextField = "PROPERTY_NAME";
            this.dropBlkCode.DataValueField = "PROPERTY_CODE";
            this.dropBlkCode.DataBind();
            this.dropBlkCode.SelectedIndex = 0;
        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
        }
    }
    #endregion
    #region 方法
    /// <summary>
    /// 功能說明:綁定BlockCode
    /// 作    者:Linda
    /// 創建時間:2010/07/15
    /// 修改記錄:
    /// </summary>
    public void BindMemo()
    {
        string strMsgID = string.Empty;
        DataTable dtMemo = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "33", ref dtMemo))
        {
            this.dropMemo.DataSource = dtMemo;
            this.dropMemo.DataTextField = "PROPERTY_NAME";
            this.dropMemo.DataValueField = "PROPERTY_CODE";
            this.dropMemo.DataBind();
            this.dropMemo.SelectedIndex = 1;
        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
        }
    }
    public void initTxt()
    {
        this.txtCardNoList.Text = "";
        this.txtMemo.Text = "";
        this.txtReasonCode.Text = "O";
        this.txtActionCode.Text = "05";
        this.txtCWB.Text = "0";

    }

    protected bool CheckCardType(string strCardNum)
    {
        switch (strCardNum.Substring(0,6))
        {
            case "589651":
                return false;
            case "447757":
                return false;
            case "515352":
                return false;
            case "888888":
                return false;
            default:
                return true;
        }
    }

    #region 匯入資料檢核
    /// <summary>
    /// 功能說明:匯入資料檢核
    /// 作    者:Linda
    /// 創建時間:2010/06/07
    /// 修改記錄:2021/02/25_Ares_Stanley-調整重複卡號文字內容; 2021/02/26_Ares_Stanley-調整重複卡號條件
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="strTpye"></param>
    /// <returns></returns>
    public bool UploadCheck(string strPath, string strFunctionName, ref string strMsgID, ref ArrayList arrayErrorMsg, ref DataTable dtDetail)
    {
        bool blnResult = true;
        #region base parameter
        string strUserID = "";
        string strFunctionKey = "06";
        string strUploadID = "06020400";
        string strTmpCardNo = "";
        string strCardNo = "";
        DateTime dtmThisDate = DateTime.Now;
        DataTable dtblBegin = new DataTable();
        DataTable dtblEnd = new DataTable();
        int intMax = 15000;
        ArrayList alTmpCardNo = new ArrayList();
        int firstCardNoIndex = 0;//第一筆重複資料的index
        Dictionary<string, string> dicErrCardNo = new Dictionary<string, string>();//卡號, 重複卡號第N筆
        #endregion

        dtDetail = BaseHelper.UploadCheck(strUserID, strFunctionKey, strUploadID,
                       dtmThisDate, strFunctionName, strPath, intMax, arrayErrorMsg, ref strMsgID, dtblBegin, dtblEnd);

        //*檢核成功
        if (strMsgID == "" && arrayErrorMsg.Count == 0)
        {
            blnResult = true;
            if (dtDetail.Rows.Count > 0)
            {
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    strCardNo = dtDetail.Rows[i]["CardNo"].ToString().Trim();
                    if (!(strCardNo.Length == 16 && ValidateHelper.IsNum(strCardNo)))
                    {
                        blnResult = false;
                        arrayErrorMsg.Add(string.Format(MessageHelper.GetMessage("06_06020400_009"), i + 1));
                    }

                    //以下用來判斷卡號不可重覆

                    if (strTmpCardNo.IndexOf(strCardNo, 0) >= 0)
                    {
                        blnResult = false;
                        if (dicErrCardNo.ContainsKey(strCardNo))
                        {
                            dicErrCardNo[strCardNo] = dicErrCardNo[strCardNo] + "及第" + (i + 1).ToString() + "筆";
                        }
                        else
                        {
                            dicErrCardNo.Add(strCardNo.ToString(), "及第" + (i + 1).ToString() + "筆");
                        }
                        //arrayErrorMsg.Add(string.Format(MessageHelper.GetMessage("06_06020400_009"), i + 1));
                    }
                    alTmpCardNo.Add(strCardNo);
                    strTmpCardNo += strCardNo + "|";
                }
                if (dicErrCardNo != null)
                {
                    foreach(KeyValuePair<string, string> errCardNo in dicErrCardNo)
                    {
                        for (int c = 0; c < alTmpCardNo.Count; c++)
                        {
                            if (errCardNo.Key == alTmpCardNo[c].ToString())
                            {
                                firstCardNoIndex = c + 1;
                                break;
                            }
                        }
                        arrayErrorMsg.Add(string.Format(MessageHelper.GetMessage("06_06020400_014"), firstCardNoIndex, errCardNo.Value));
                    }
                }
            }
        }
        //*檢核失敗
        else
        {
            blnResult = false;
        }
        return blnResult;
    }
    #endregion



    protected bool DoOASA(string[] strOASACardNo)
    {
        string strBlockCode = "";
        string strMemo = "";
        string strReasonCode = "";
        string strActionCode = "";
        string strCWBRegions = "";
        string strCardNo = "";
        string strSFFlg = "";
        string strOASAUserId = "";
        string strCancelOASAFile = "";
        string strMemoLog = "";
        string strBlockLog = "";
        int intSOASACount = 0;
        int intFOASACount = 0;

        try
        {
            strBlockCode = this.dropBlkCode.SelectedItem.Text.ToString().Trim();
            if (this.dropMemo.SelectedValue == "0")
            {
                strMemo = txtMemo.Text.ToString().Trim();
            }
            else
            {
                strMemo = dropMemo.SelectedItem.Text.ToString().Trim();
            }

            strReasonCode = txtReasonCode.Text.ToString().Trim();
            strActionCode = txtActionCode.Text.ToString().Trim();
            strCWBRegions = txtCWB.Text.ToString().Trim();

            if (!GetOASAFileName(ref strCancelOASAFile))
            {
                return false;
            }

            EntitySet<Entity_CancelOASA_Detail> SetCancelOASADetail = new EntitySet<Entity_CancelOASA_Detail>();

            for (int intOASA = 0; intOASA < strOASACardNo.Length; intOASA++)
            {
                strCardNo = strOASACardNo[intOASA].Trim();
                strMemoLog = "";
                strBlockLog = "";
                strOASAUserId = "";
                if (this.HtgOASAAdd(strCardNo, strBlockCode, strMemo, strReasonCode, strActionCode, ref strMemoLog, ref strBlockLog, ref strOASAUserId))
                {
                    intSOASACount++;
                    strSFFlg = "1";
                }
                else
                {
                    if (strBlockLog.Equals(string.Empty))
                    {
                        return false;
                    }
                    else
                    {
                        intFOASACount++;
                        strSFFlg = "2";
                    }
                    
                }

                Entity_CancelOASA_Detail CancelOASADetail = new Entity_CancelOASA_Detail();
                CancelOASADetail.CancelOASAFile = strCancelOASAFile;
                CancelOASADetail.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                CancelOASADetail.CardNo = strCardNo;
                CancelOASADetail.BlockCode = strBlockCode;
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
            CancelOASA.CancelOASASource = "1";
            SetCancelOASA.Add(CancelOASA);
            if (BRM_CancelOASA.BatInsert(SetCancelOASA))
            {
                string strMsgID = string.Empty;
                BRM_CancelOASADetail.BatInsert(SetCancelOASADetail, ref strMsgID);
            }

            return true;
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_011"));
            return false;
        }
    }

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
                //strErrorMsg = string.Format(MessageHelper.GetMessage("06_06020400_004"), strOASAFileName);
                return false;
            }
            else
            {
                return true;
            }

        }
        else
        {
            //strErrorMsg = string.Format(MessageHelper.GetMessage("06_06020400_003"), strOASAFileName);
            return false;
        }

    }
    #endregion

    #region 新增主機OASA資料
    /// <summary>
    /// 功能說明:新增主機OASA資料
    /// 作    者:Linda
    /// 創建時間:2010/07/07
    private bool HtgOASAAdd(string strCardNo, string strBlkCode, string strMemo, string strReasonCode, string strActionCode, ref string strErrorMsg, ref string strBLCLog, ref string strUserId)
    {

        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo = (EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]; //*Session變數集合

        //20120328 modified by charlotte 20120328 - start
        //string strPurgeDate = DateTime.Now.AddMonths(3).ToString("MMdd");
        string strPurgeDate = DateTime.Now.AddMonths(3).ToString("MMyy");
        //20120328 modified by charlotte 20120328 - end

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

        htInput.Add("OASA_PURGE_DATE", strPurgeDate);//*PURGE DATE

        //*提交OASA_P4_Submit主機資料

        Hashtable htResultA = MainFrameInfoOASA.GetMainFrameInfo(MainFrameInfoOASA.HtgType.P4_JCAX, htInput, false, "100", eAgentInfo);
        if (!htResultA.Contains("HtgMsg"))
        {
            strErrorMsg = "";//*主機返回成功訊息
            strBLCLog = "";
            strUserId = htResultA["USER_ID"].ToString().Trim();
            return true;
        }
        else
        {
            strErrorMsg=htResultA["HtgMsg"].ToString().Trim();
            if (htResultA.Count > 2)
            {
                strBLCLog = htResultA["OASA_BLOCK_CODE"].ToString().Trim();
                strUserId = htResultA["USER_ID"].ToString().Trim();
            }          
            return false;
        }

    }
    #endregion

    /// <summary>
    /// 上傳文件的信息

    /// </summary>
    public class UploadInfo
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        public double FILE_SIZE = 0;
        /// <summary>
        /// 文件擴展名(副檔名)
        /// </summary>
        public string FILE_POSTNAME = "";
        /// <summary>
        /// 原文件名
        /// </summary>
        public string FILE_ORGINNAME = "";
        /// <summary>
        /// 文件在本系統中的新名字

        /// </summary>
        public string FILE_SYSNAME = "";
        /// <summary>
        /// 文件在本系統中所在路徑(含名字)
        /// </summary>
        public string FILE_PATH = "";

    }
    #endregion 
}
