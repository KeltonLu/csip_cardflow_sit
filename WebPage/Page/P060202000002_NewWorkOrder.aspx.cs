//******************************************************************
//*  功能說明：綜合資料修改異動單新增
//*  作    者：HAO CHEN
//*  創建日期：2010/06/24
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//  Joe               20210120        RQ-2019-008159-003     配合長姓名作業修改
//*******************************************************************
//20161108 (U) by Tank, 調整判斷信用卡方式

using System;
using System.Data;
using System.Web.UI.WebControls;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
//20161108 (U) by Tank
using Framework.Data;
using System.Data.SqlClient;
//20210601_Ares_Stanley
using System.IO; 
using TIBCO.EMS;
using ESBOrderUp;


public partial class Page_P060202000002_NewWorkOrder : PageBase
{
    #region table
    public DataTable m_dtCardBaseInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }
    public DataTable m_dtPostSend
    {
        get { return ViewState["m_dtPostSend"] as DataTable; }
        set { ViewState["m_dtPostSend"] = value; }
    }
    public DataTable m_dtCardDataChange
    {
        get { return ViewState["m_dtCardDataChange"] as DataTable; }
        set { ViewState["m_dtCardDataChange"] = value; }
    }
    #endregion

    #region params
    public string m_Action
    {
        get { return ViewState["m_Action"] as string; }
        set { ViewState["m_Action"] = value; }
    }
    public string m_Id
    {
        get { return ViewState["m_Id"] as string; }
        set { ViewState["m_Id"] = value; }
    }
    public string m_CardNo
    {
        get { return ViewState["m_CardNo"] as string; }
        set { ViewState["m_CardNo"] = value; }
    }
    public string m_Trandate
    {
        get { return ViewState["m_Trandate"] as string; }
        set { ViewState["m_Trandate"] = value; }
    }
    public string m_Kind
    {
        get { return ViewState["m_Kind"] as string; }
        set { ViewState["m_Kind"] = value; }
    }
    public string m_Status
    {
        get { return ViewState["m_Status"] as string; }
        set { ViewState["m_Status"] = value; }
    }
    #endregion

    #region SNo
    public string m_SNoN
    {
        get { return ViewState["m_SNoN"] as string; }
        set { ViewState["m_SNoN"] = value; }
    }
    public string m_SNoP
    {
        get { return ViewState["m_SNoP"] as string; }
        set { ViewState["m_SNoP"] = value; }
    }
    public string m_SNoA
    {
        get { return ViewState["m_SNoA"] as string; }
        set { ViewState["m_SNoA"] = value; }
    }
    public string m_SNoM
    {
        get { return ViewState["m_SNoM"] as string; }
        set { ViewState["m_SNoM"] = value; }
    }
    public string m_SNoC
    {
        get { return ViewState["m_SNoC"] as string; }
        set { ViewState["m_SNoC"] = value; }
    }
    public string m_SNoG
    {
        get { return ViewState["m_SNoG"] as string; }
        set { ViewState["m_SNoG"] = value; }
    }
    #endregion

    #region LogPath
    //2021/04/07_Ares_Stanley-增加ESB LOG路徑
    private string logPath_ESB = "NameCheckInfoLog";
    private string logPath_ESB_TG = "HtgNameCheckInfoLog";
    #endregion

    /// <summary>
    /// 功能說明:頁面加載綁定數據
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Request.QueryString["action"] != null && Request.QueryString["id"] != null && Request.QueryString["cardno"] != null && Request.QueryString["trandate"] != null)
            {
                //* 傳遞參數解密
                this.m_Action = RedirectHelper.GetDecryptString(Request.QueryString["action"].ToString());
                this.m_Id = RedirectHelper.GetDecryptString(Request.QueryString["id"].ToString());
                this.m_CardNo = RedirectHelper.GetDecryptString(Request.QueryString["cardno"].ToString());
                this.m_Trandate = RedirectHelper.GetDecryptString(Request.QueryString["trandate"].ToString());
            }
            BindData();
            m_Status = "Y";
            ViewState["FlgEdit"] = "FALSE";
        }
    }

    /// <summary>
    /// 功能說明:BindData
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    public void BindData()
    {
        //*標題列印
        ShowControlsText();
        //*資料明細
        SearchCardBaseInfo();
        //*資料明細加載
        BindDataToControls();
        //*備註加載
        SearchDataChange();
        //*最近一次退件資料加載
        //SearchBack();
        //*綁定取卡方式
        BindKind();
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020101_084");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020101_021");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020101_022");
    }

    /// <summary>
    /// 功能說明:明細資料
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    private void BindDataToControls()
    {
        if (m_dtCardBaseInfo != null && m_dtCardBaseInfo.Rows.Count > 0)
        {
            //*明细资料绑定
            DataRow row = m_dtCardBaseInfo.Rows[0];

            if (null != row["Id"] && !string.IsNullOrEmpty(row["Id"].ToString()))
            {
                this.lblId.Text = row["Id"].ToString(); //*身分證字號
            }

            string strSourceName = row["custname"].ToString();
            string strResultName = string.Empty;
            bool blnIsName = true;
            string strSnoN = string.Empty;
            SearchDataChange("NewName", ref blnIsName, strSourceName, ref strResultName, ref strSnoN);
            m_SNoN = strSnoN;
            this.hidN.Value = blnIsName.ToString();
            //this.lblname1.Text = strSourceName; //*寄件人姓名
            //this.lblname2.Text = strResultName; //*寄件人姓名

            //2021/01/05 陳永銘 新增標籤:收件人姓名(隱藏)
            this.lblname1_Hide.Text = strSourceName;

            string strResultName2 = string.Empty;
            string strResultName_Roma = string.Empty;
            string strSourceName_Roma = row["custname_roma"].ToString();//歸戶姓名_羅馬拼音
            SearchDataChange2("NewName_Roma", ref blnIsName, strSourceName, ref strResultName_Roma, ref strResultName2, ref strSnoN);
            this.lblname1_Roma_Hide.Text = strSourceName_Roma;          //收件人姓名_羅馬拼音(隱藏)

            //2021/01/05 陳永銘 修改標籤:文字超過第六個或含有羅馬拼音以...顯示並增加文字提示 BEGIN
            this.lblname1.Text = strSourceName;//*收件人姓名
            if (strSourceName.Length >= 5 || strSourceName_Roma != string.Empty)
            {
                int length = strSourceName.Length >= 5 ? 5 : strSourceName.Length;
                this.lblname1.Text = strSourceName.Substring(0, length) + "...";                 //*收件人姓名
                this.lblname1.ToolTip = strSourceName + Environment.NewLine + strSourceName_Roma;//收件人姓名換行羅馬拼音
            }

            this.lblname2.Text = strResultName;//*新收件人姓名
            if (strResultName.Length >= 5 || strResultName_Roma != string.Empty)
            {
                int length = strResultName.Length >= 5 ? 5 : strResultName.Length;
                this.lblname2.Text = strResultName.Substring(0, length) + "...";            //*新收件人姓名
                this.lblname2.ToolTip = strResultName + Environment.NewLine + strResultName_Roma;//新收件人姓名換行羅馬拼音
            }
            //2021/01/05 陳永銘 修改標籤:文字超過第六個或含有羅馬拼音以...顯示並增加文字提示 END

            if (null != row["Trandate"] && !string.IsNullOrEmpty(row["Trandate"].ToString()))
            {
                this.lblTrandate.Text = row["Trandate"].ToString(); //*轉檔日
            }

            if (null != row["Indate1"] && !string.IsNullOrEmpty(row["Indate1"].ToString()))
            {
                this.lblIndate1.Text = row["Indate1"].ToString(); //*製卡日
            }

            this.lblCardno.Text = row["Cardno"].ToString(); //*卡號一 
            this.lblExpdate.Text = row["Expdate"].ToString(); //*有效期限一

            if (null != row["Zip"] && !string.IsNullOrEmpty(row["Zip"].ToString()))
            {
                string strSourceZip = row["Zip"].ToString();
                string strResultZip = string.Empty;
                bool blnIsZip = true;
                string strSnoZ = string.Empty;
                SearchDataChange("Newzip", ref blnIsZip, strSourceZip, ref strResultZip, ref strSnoZ);
                if (!string.IsNullOrEmpty(strResultZip))
                {
                    this.lblZip.Text = strResultZip; //*郵遞區號
                    this.lblOldzipAjax.Text = strResultZip;
                }
                else
                {
                    this.lblZip.Text = strSourceZip; //*郵遞區號
                    this.lblOldzipAjax.Text = strSourceZip;
                }

            }


            //*地址一
            string strSourceAdd1 = row["Add1"].ToString();
            string strResultAdd1 = string.Empty;

            bool blnIsAdd1 = true;
            string strSnoA = string.Empty;
            string strBaseFlg = "";
            //依據國俊的需求只要委管有異動過就不能給客服異動
            SearchDataChange2("Newadd1", ref blnIsAdd1, strSourceAdd1, ref strResultAdd1, ref strSnoA, ref strBaseFlg);
            if (blnIsAdd1 && strBaseFlg.Equals("1"))
            {
                this.btnUpdateA.Enabled = false;
                this.btnUpdateC.Enabled = false;
            }

            //SearchDataChange("Newadd1", ref blnIsAdd1, strSourceAdd1, ref strResultAdd1, ref strSnoA);
            m_SNoA = strSnoA;
            this.hidA.Value = blnIsAdd1.ToString();

            DateTime dtMailDate;
            DateTime dtNow;
            TimeSpan ts;
            if (null != row["Mailno"] && !string.IsNullOrEmpty(row["Mailno"].ToString()))
            {
                dtMailDate = Convert.ToDateTime(row["MailDate"].ToString());
                dtNow = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));
                ts = dtMailDate - dtNow;
                if (ts.Days == 0)
                {
                    string strMailTime = UtilHelper.GetAppSettings("MailTime");
                    int iHour = int.Parse(strMailTime.Substring(0, 2));
                    int iMinute = int.Parse(strMailTime.Substring(3, 2));
                    if (DateTime.Now.Hour <= iHour)
                    {
                        if (DateTime.Now.Minute <= iMinute)
                        {
                            //this.hidA.Value = "xx";
                        }
                    }
                }
            }

            this.lblAdd1.Text = strSourceAdd1;
            this.lblAdd1New.Text = strResultAdd1;
            this.lblAdd1Ajax.Text = strSourceAdd1;

            //*地址二
            string strSourceAdd2 = row["Add2"].ToString();
            string strResultAdd2 = string.Empty;
            bool blnIsAdd2 = true;
            string strSno = string.Empty;
            //依據國俊的需求只要委管有異動過就不能給客服異動
            strBaseFlg = "";
            SearchDataChange2("Newadd2", ref blnIsAdd2, strSourceAdd2, ref strResultAdd2, ref strSno, ref strBaseFlg);
            if (blnIsAdd2 && strBaseFlg.Equals("1"))
            {
                this.btnUpdateA.Enabled = false;
                this.btnUpdateC.Enabled = false;
            }
           // SearchDataChange("Newadd2", ref blnIsAdd2, strSourceAdd2, ref strResultAdd2, ref strSno);
            this.lblAdd2.Text = strSourceAdd2;
            this.lblAdd2New.Text = strResultAdd2;
            this.lblAdd2Ajax.Text = strSourceAdd2;

            //*地址三
            string strSourceAdd3 = string.Empty;
            if (row["Add3"] != null)
            {
                strSourceAdd3 = row["Add3"].ToString();
            }
            string strResultAdd3 = string.Empty;
            bool blnIsAdd3 = true;
            string strSnos = string.Empty;
            //依據國俊的需求只要委管有異動過就不能給客服異動
            strBaseFlg = "";
            SearchDataChange2("Newadd3", ref blnIsAdd3, strSourceAdd3, ref strResultAdd3, ref strSnos, ref strBaseFlg);
            if (blnIsAdd3 && strBaseFlg.Equals("1"))
            {
                this.btnUpdateA.Enabled = false;
                this.btnUpdateC.Enabled = false;
            }
          //  SearchDataChange("Newadd3", ref blnIsAdd3, strSourceAdd3, ref strResultAdd3, ref strSnos);
            this.lblAdd3.Text = strSourceAdd3;
            this.lblAdd3New.Text = strResultAdd3;
            this.lblAdd3Ajax.Text = strSourceAdd3;

            if (null != row["SelfPick_Type"] && !string.IsNullOrEmpty(row["SelfPick_Type"].ToString()))
            {

                string strSelfPick_Type = string.Empty;
                GetKindName("4", row["SelfPick_Type"].ToString(), ref strSelfPick_Type);
                this.lblSelfPick_Type.Text = strSelfPick_Type; //*領卡方式 
            }

            if (null != row["SelfPick_Date"] && !string.IsNullOrEmpty(row["SelfPick_Date"].ToString()))
            {
                this.lblSelfPick_Date.Text = row["SelfPick_Date"].ToString(); //*領卡日期 
            }

            if (null != row["Action"] && !string.IsNullOrEmpty(row["Action"].ToString()))
            {
                string strAction = string.Empty;
                GetKindName("1", row["Action"].ToString(), ref strAction);
                this.lblAction.Text = strAction; //*卡別
            }

            if (null != row["Cardtype"] && !string.IsNullOrEmpty(row["Cardtype"].ToString()))
            {
                this.lblCardtype.Text = row["Cardtype"].ToString(); //*TYPE
            }

            if (null != row["Affinity"] && !string.IsNullOrEmpty(row["Affinity"].ToString()))
            {
                string strAffinity = string.Empty;
                GetAffName(row["Affinity"].ToString(), ref strAffinity);
                this.lblAffinity.Text = strAffinity; //*認同代碼
            }


            if (null != row["Photo"] && !string.IsNullOrEmpty(row["Photo"].ToString()))
            {
                string strPhoto = string.Empty;
                GetKindName("3", row["Photo"].ToString(), ref strPhoto);
                this.lblPhoto.Text = strPhoto; //*PHOTO
            }

            if (null != row["Monlimit"] && !string.IsNullOrEmpty(row["Monlimit"].ToString()))
            {
                string strSourceMonlimit = row["Monlimit"].ToString();
                string strResultMonlimit = string.Empty;
                bool blnIsMonlimit = true;
                string strSnoM = string.Empty;
                SearchDataChange("Newmonlimit", ref blnIsMonlimit, strSourceMonlimit, ref strResultMonlimit, ref strSnoM);
                m_SNoM = strSnoM;
                this.hidM.Value = blnIsMonlimit.ToString();
                if (!string.IsNullOrEmpty(strResultMonlimit))
                {
                    this.lblMonlimit.Text = strResultMonlimit; //*額度
                }
                else
                {
                    this.lblMonlimit.Text = strSourceMonlimit; //*額度
                }
            }

            if (null != row["Cardno2"] && !string.IsNullOrEmpty(row["Cardno2"].ToString()))
            {
                this.lblCardno2.Text = row["Cardno2"].ToString(); //*卡號二
            }

            if (null != row["Expdate2"] && !string.IsNullOrEmpty(row["Expdate2"].ToString()))
            {
                this.lblExpdate2.Text = row["Expdate2"].ToString(); //*有效期限
            }

            if (null != row["Kind"] && !string.IsNullOrEmpty(row["Kind"].ToString()))
            {
                string strSourceKind = row["Kind"].ToString();
                string strResultKind = string.Empty;
                bool blnIsKind = true;
                string strSnoC = string.Empty;
                strBaseFlg = "";
                SearchDataChange2("Newway", ref blnIsKind, strSourceKind, ref strResultKind, ref strSnoC, ref strBaseFlg);
                //SearchDataChange("Newway", ref blnIsKind, strSourceKind, ref strResultKind, ref strSnoC);
                m_SNoC = strSnoC;
                this.hidC.Value = blnIsKind.ToString();
                m_Kind = strResultKind;

                string strKindName1 = string.Empty;
                string strKindName2 = string.Empty;
                GetKindName("2", strSourceKind, ref strKindName1);
                GetKindName("2", strResultKind, ref strKindName2);

                this.lblKind.Text = strKindName1; //*取卡方式
                this.lblKind2.Text = strKindName2;
                this.lblKindAjax.Text = strKindName1;
                //國俊要求只要綜合資料目前有異動且還沒送出去給卡廠，客服就不能修改
                if (blnIsKind && strBaseFlg.Equals("1"))
                {
                    this.btnUpdateA.Enabled = false;
                    this.btnUpdateC.Enabled = false;
                }

                //max 如果取卡方式是保留就不讓客服 修改內容
                if (strSourceKind.Equals("6") || strResultKind.Equals("6"))
                {
                    this.btnUpdateA.Enabled = false;
                    this.btnUpdateC.Enabled = false;
                }


                bool blUrgencyFlg = true;
                string strUrgencyFlg = string.Empty;
                string strResultUrgency = string.Empty;
                string strSnoUrg = string.Empty;
                if (null != row["Urgency_Flg"] && !string.IsNullOrEmpty(row["Urgency_Flg"].ToString()))
                {
                    strUrgencyFlg = row["Urgency_Flg"].ToString();
                }
                SearchDataChange("UrgencyFlg", ref blUrgencyFlg, strUrgencyFlg, ref strResultUrgency, ref strSnoUrg);

                if (!string.IsNullOrEmpty(strResultUrgency))
                {
                    //if (strResultUrgency.Equals("1"))
                    //{
                    //    chkUrgencyFlg.Checked = true;
                    //}
                    //else
                    //{
                    //    chkUrgencyFlg.Checked = false;
                    //}
                }
                else
                {
                    if (!string.IsNullOrEmpty(strUrgencyFlg))
                    {
                        //if (strUrgencyFlg.Equals("1"))
                        //{
                        //    chkUrgencyFlg.Checked = true;
                        //}
                        //else
                        //{
                        //    chkUrgencyFlg.Checked = false;
                        //}
                    }
                }
            }

            if (null != row["Mailno"] && !string.IsNullOrEmpty(row["Mailno"].ToString()))
            {
                string strSourceMailno = row["Mailno"].ToString();
                string strResultMailno = string.Empty;
                bool blnIsMailno = true;
                string strSnoG = string.Empty;
                SearchDataChange("Newmailno", ref blnIsMailno, strSourceMailno, ref strResultMailno, ref strSnoG);
                m_SNoG = strSnoG;
                this.hidG.Value = blnIsMailno.ToString();
                this.lblMailno.Text = strResultMailno; //*掛號號碼
            }

            if (null != row["Merch_CodeS"] && !string.IsNullOrEmpty(row["Merch_CodeS"].ToString()))
            {
                this.lblMerch_Code.Text = row["Merch_CodeS"].ToString(); //*製卡廠名稱
            }

            if (null != row["Mailno"] && !string.IsNullOrEmpty(row["Mailno"].ToString()) && null != row["Maildate"] && !string.IsNullOrEmpty(row["Maildate"].ToString()))
            {
                //*來源于Post_Send
                string strSend_status_Name = string.Empty;
                string strM_date = string.Empty;
                string strPost_Name = string.Empty;
                bool blnPost = false;
                SearchPostSend(row["Mailno"].ToString(), row["Maildate"].ToString(), ref strSend_status_Name, ref strM_date, ref strPost_Name, ref blnPost);
                this.lblSend_status_Name.Text = strSend_status_Name; //*郵件狀態
                this.lblM_dates.Text = strM_date; //*郵局處理日
                this.lblPost_Name.Text = strPost_Name; //*招領郵局
                this.lblPost_Name.Visible = blnPost;
            }

            if (null != row["Maildate"] && !string.IsNullOrEmpty(row["Maildate"].ToString()))
            {

                string strSourceMaildate = row["Maildate"].ToString();
                string strResultMaildate = string.Empty;
                bool blnIsMaildate = true;
                string strSnoP = string.Empty;
                if (!string.IsNullOrEmpty(strSourceMaildate))
                {
                    SearchDataChange("NewMailDate", ref blnIsMaildate, strSourceMaildate, ref strResultMaildate, ref strSnoP);
                    m_SNoP = strSnoP;
                    this.hidP.Value = blnIsMaildate.ToString();
                    //if (string.IsNullOrEmpty(strResultMaildate))
                    //{
                    //    strResultMaildate = strSourceMaildate;
                    //}
                    this.lblMaildate.Text = strSourceMaildate; //*　郵寄日
                    this.lblMaildate2.Text = strResultMaildate;

                    string strFromDate = strSourceMaildate;
                    string strToDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string strMsg = string.Empty;
                    if (ValidateHelper.IsValidDate(strFromDate, strToDate, ref strMsg))
                    {
                        m_SNoP = strSnoP;
                        //this.hidP.Value = "Trues";
                        //this.hidN.Value = "Trues";
                        //this.hidM.Value = "Trues";
                        //this.hidA.Value = "Trues";
                        //this.hidC.Value = "Trues";
                        //this.hidG.Value = "Trues";
                        this.hidPostData.Value = strSourceMaildate;
                    }

                    dtMailDate = Convert.ToDateTime(strFromDate);
                    dtNow = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));
                    ts = dtMailDate - dtNow;
                    if (ts.Days == 0)
                    {
                        string strMailTime = UtilHelper.GetAppSettings("MailTime");
                        int iHour = int.Parse(strMailTime.Substring(0, 2));
                        int iMinute = int.Parse(strMailTime.Substring(3, 2));
                        if (DateTime.Now.Hour <= iHour)
                        {
                            if (DateTime.Now.Minute <= iMinute)
                            {
                                //this.hidP.Value = "xx";
                            }
                        }
                    }
                }
                else
                {
                    SearchDataChange("NewMailDate", ref blnIsMaildate, strSourceMaildate, ref strResultMaildate, ref strSnoP);

                    m_SNoP = strSnoP;
                    this.hidP.Value = blnIsMaildate.ToString();
                    //if (string.IsNullOrEmpty(strResultMaildate))
                    //{
                    //    strResultMaildate = strSourceMaildate;
                    //}
                    this.lblMaildate.Text = strSourceMaildate; //*　郵寄日
                    this.lblMaildate2.Text = strResultMaildate;
                }
            }

        }
    }

    /// <summary>
    /// 功能說明:通過Code獲得認同代碼Name
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="ref strAFFID">認同代碼ID</param>
    /// <param name="ref strAffName">認同代碼NAME</param>
    public void GetAffName(string strAFFID, ref string strAffName)
    {
        string strMsgID = string.Empty;
        DataTable dtAffName = new DataTable();

        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_AffName.M_AFFID, Operator.Equal, DataTypeUtils.String, strAFFID);
        strAffName = strAFFID;

        if (BRM_AffName.SearchByNo(sqlhelp.GetFilterCondition(), ref dtAffName, ref strMsgID))
        {
            if (null != dtAffName && dtAffName.Rows.Count > 0)
            {
                DataRow[] rowKindName = dtAffName.Select("AFFID='" + strAFFID + "'");
                if (rowKindName != null && rowKindName.Length > 0)
                {
                    strAffName = rowKindName[0]["AFFID"].ToString() + " " + rowKindName[0]["AFFName"].ToString();
                }
            }
        }
    }

    /// <summary>
    /// 功能說明:備注-異動欄位
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    private void SearchCardBaseInfo()
    {
        DataTable dtCardBaseInfo = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Like, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Like, DataTypeUtils.String, m_Trandate);
        if (BRM_TCardBaseInfo.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardBaseInfo, ref strMsgID))
        {
            MergeTable(ref  dtCardBaseInfo);
            m_dtCardBaseInfo = dtCardBaseInfo;
        }
    }

    /// <summary>
    /// 功能說明:備注-異動欄位
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    private void SearchDataChange()
    {
        DataTable dtCardDataChange = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);
        //sqlhelp.AddOrderCondition(Entity_CardDataChange.M_Sno, ESortType.DESC); 
        if (BRM_CardDataChange.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID))
        {
            this.grvUserView.DataSource = dtCardDataChange;
            this.grvUserView.DataBind();
        }
    }

    /// <summary>
    /// 功能說明:最後一次退件資料
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    //private void SearchBack()
    //{
    //    DataTable dtCardBackInfo = new DataTable();
    //    string strMsgID = string.Empty;
    //    SqlHelper sqlhelp = new SqlHelper();
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);

    //    if (BRM_CardBackInfo.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardBackInfo, ref strMsgID))
    //    {
    //        if (dtCardBackInfo.Rows.Count > 0)
    //        {
    //            //*最後一次退件資料绑定
    //            DataRow row = dtCardBackInfo.Rows[0];
    //            this.lblbackdate.Text = row["Backdate"].ToString();                   //退件時間
    //            this.lblreason.Text = row["Reason"].ToString();                       //退件原因
    //            this.lblm_date.Text = row["Enddate"].ToString();                      //處理日期
    //            this.lblpreenditem.Text = row["Enditem"].ToString();                  //處理方式
    //            this.txtRemarkFB.Text = row["Endnote"].ToString();                    //備注
    //        }
    //    }
    //}

    /// <summary>
    /// 功能說明:查詢郵局資料
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="strMailno">郵寄號碼</param>
    /// <param name="strMailDate">交寄日期</param>
    /// <param name="strSend_status_Name">郵件狀態</param>
    /// <param name="strM_date">郵局處理日</param>
    /// <param name="strPost_Name">招領郵局</param>
    private void SearchPostSend(string strMailno, string strMailDate, ref string strSend_status_Name, ref string strM_date, ref string strPost_Name, ref bool blnPost)
    {
        DataTable dtPostSend = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityM_PostSend.M_mailno, Operator.Equal, DataTypeUtils.String, strMailno);
        sqlhelp.AddCondition(EntityM_PostSend.M_maildate, Operator.Equal, DataTypeUtils.String, strMailDate);

        if (BRM_PostSend.SearchByMailNo(sqlhelp.GetFilterCondition(), ref dtPostSend, ref strMsgID))
        {
            if (dtPostSend.Rows.Count > 0)
            {
                strSend_status_Name = dtPostSend.Rows[0]["Info1"].ToString() + dtPostSend.Rows[0]["Send_status_Name"].ToString();//*資訊代號中文+郵件狀態中文)
                if (!string.IsNullOrEmpty(dtPostSend.Rows[0]["M_date"].ToString()))
                {
                    strM_date = dtPostSend.Rows[0]["M_date"].ToString().Replace("/", "").Substring(0, 8);//*處理日期取前8碼
                }
                if (dtPostSend.Rows[0]["Info1"].ToString().Equals("240") && dtPostSend.Rows[0]["Send_status_Code"].ToString().Equals("G2"))
                {
                    //*資訊代碼1=240且郵件狀態代號=’G2’才需顯示招領郵局欄位
                    blnPost = true;
                }
                strPost_Name = dtPostSend.Rows[0]["Post_Name"].ToString();
            }
        }
    }

    /// <summary>
    /// 功能說明:取消操作
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060202000001.aspx");
    }

    ///// <summary>
    ///// 功能說明:更新備注
    ///// 作    者:HAO CHEN
    ///// 創建時間:2010/06/24
    ///// 修改記錄:
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    //{
    //    string strMsgID = string.Empty;
    //    CustButton btnSure = grvUserView.Rows[e.NewEditIndex].Cells[2].FindControl("btnSure") as CustButton;
    //    TextBox txtcNote = grvUserView.Rows[e.NewEditIndex].Cells[1].FindControl("txtcNote") as TextBox;
    //    if (txtcNote == null || string.IsNullOrEmpty(txtcNote.Text.Trim()))
    //    {
    //        //*備註不通過
    //        MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_000");
    //        return;
    //    }
    //    if (btnSure != null && txtcNote != null)
    //    {
    //        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
    //        //*修改異動單備註
    //        CardDataChange.Sno = int.Parse(btnSure.CommandArgument.ToString());
    //        CardDataChange.CNote = txtcNote.Text.Trim();
    //        SqlHelper sqlhelp = new SqlHelper();
    //        sqlhelp.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChange.Sno.ToString());

    //        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
    //        string strLogMsg = BaseHelper.GetShowText("06_02020000_000") + "：" + BaseHelper.GetShowText("06_06020003_010");
    //        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

    //        if (BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "CNote"))
    //        {
    //            MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_001");
    //            BindData();
    //        }
    //        else
    //        {
    //            MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_002");
    //        }
    //    }
    //}
    /// <summary>
    /// 功能說明:更新備注
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:20210701_Ares_Stanley-更新方始由Editing改為Selecting避免文字顯示欄位變成可編輯
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowSelecting(object sender, GridViewCommandEventArgs e)
    {
        string strMsgID = string.Empty;
        Button btn = (Button)e.CommandSource;
        GridViewRow row = btn.NamingContainer as GridViewRow;
        Int32 idx = row.RowIndex;
        CustButton btnSure = grvUserView.Rows[idx].Cells[2].FindControl("btnSure") as CustButton;
        TextBox txtcNote = grvUserView.Rows[idx].Cells[1].FindControl("txtcNote") as TextBox;
        if (txtcNote == null || string.IsNullOrEmpty(txtcNote.Text.Trim()))
        {
            //*備註不通過
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_000");
            return;
        }
        if (btnSure != null && txtcNote != null)
        {
            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
            //*修改異動單備註
            CardDataChange.Sno = int.Parse(btnSure.CommandArgument.ToString());
            CardDataChange.CNote = txtcNote.Text.Trim();
            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChange.Sno.ToString());

            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_02020000_000") + "：" + BaseHelper.GetShowText("06_06020003_010");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

            if (BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "CNote"))
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_001");
                BindData();
            }
            else
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_002");
            }
        }
    }

    /// <summary>
    /// 功能說明:回上頁
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060202000001_NewWorkOrder.aspx");
    }

    /// <summary>
    /// 功能說明:修改地址(1,2,3)
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:2020/12/15_Ares_Stanley-修改CallEMFS_SUCCESS LOG層級; 2021/06/_Ares_Stanley-新增工單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureA_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
        bool blnResult = true;
        string strUserName = string.Empty;
        //檢查類別是否有填寫
        if (this.category_tr.Visible == true)
        {
            if (this.newCardnewAccount.Checked == false && this.newCardoldAccount.Checked == false)
            {
                jsBuilder.RegScript(this.Page, "alert('類別為必填欄位！')");
                this.ModalPopupExtenderA.Show();
                return;
            }
        }        
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);

        //*新增異動單
        CardDataChange.action = m_Action;
        if (!string.IsNullOrEmpty(m_Id))
        {
            CardDataChange.id = m_Id;
        }

        CardDataChange.indate1 = lblIndate1.Text.Trim();
        CardDataChange.CardNo = m_CardNo;
        CardDataChange.Trandate = m_Trandate;
        //max add 悠遊Debit 需求修改
        //20161108 (U) by Tank, 調整判斷信用卡方式
        //if (null != m_Action && m_Action.Equals("1") && !lblCardtype.Text.Equals("000") && !lblCardtype.Text.Equals("013") && !lblCardtype.Text.Equals("018") && !lblCardtype.Text.Equals("370") && !lblCardtype.Text.Equals("035") && !lblCardtype.Text.Equals("570") && !lblCardtype.Text.Equals("039") && !lblCardtype.Text.Equals("040") && !lblCardtype.Text.Equals("038") && !lblCardtype.Text.Equals("037"))
        if (null != m_Action && m_Action.Equals("1") && funCheckCreditCard(lblCardtype.Text))
        {
            #region 舊工單
            ////*異動取卡方式為保留
            ////* 發送二代電訊單給徵信
            ////* 1調用web-service將異動后的地址經二代電訊單傳送給徵信覆核
            //CallEMFS.E00520 CallEmfs = new CallEMFS.E00520();
            ////CallEmfs.UserId = "Z00006660";                                         //* 從Session中讀取 Test
            //CallEmfs.UserId = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;  //* 從Session中讀取UserID
            //CallEmfs.CaseType = UtilHelper.GetAppSettings("CallEMFSType");    //*案件類型
            //CallEmfs.CardType = "新卡";                                              //* ACTION
            //CallEmfs.CustLevel = UtilHelper.GetAppSettings("CallEMFSLevel");  //*案件級別
            //CallEmfs.CustId = m_Id;                                                  //* 歸戶ID
            //CallEmfs.CustName = lblname1.Text.Trim();                                //* 歸戶姓名
            //CallEmfs.TelH = txtTel.Text.Trim();                                      //* 歸戶電話
            //CallEmfs.CardList = m_CardNo;                                            //*卡號
            //CallEmfs.ChangeAddressYN = true;
            //CallEmfs.OthZipCode = BaseHelper.ToDBC(this.lblNewzipAjax.Text.Trim());  //*郵遞區號半碼
            //CallEmfs.OthAddress1 = this.CustAdd1.strAddress;
            //CallEmfs.OthAddress2 = this.txtAdd2Ajax.Text.Trim();
            //CallEmfs.OthAddress3 = this.txtAdd3Ajax.Text.Trim();
            //CallEmfs.SourceSystem = "CardImport";
            //CallEmfs.Memo = this.txtCNoteA.Text.Trim();

            //CallEMFS.CreateProcess CreateEmfs = new CallEMFS.CreateProcess();

            ////如果要測試，就要把下面這段電訊單註記起來
            //string strMsgbox = CreateEmfs.CreateProcessE00520(CallEmfs);
            //if (!string.IsNullOrEmpty(strMsgbox))
            //{
            //    jsBuilder.RegScript(this.Page, "alert('" + strMsgbox + "')");
            //    Logging.Log(DateTime.Now.ToString() + "Fail：" + strMsgbox, LogState.Error, LogLayer.UI);
            //}
            //else
            //{
            //    Logging.Log(DateTime.Now.ToString() + "：CallEMFS SUCCESS", LogState.Info, LogLayer.UI);
            //}
            //如果要測試，就要把上面這段電訊單註記起來  END
            #endregion 舊工單
            #region 20210601_Ares_Stanley新工單
            #region params
            string strResult = string.Empty;
            string caseNo = string.Empty;
            string resultErrorMsg = string.Empty;
            string resultRspCode = string.Empty;
            string resultErrorCode = string.Empty;

            //變更取卡方式電文參數
            ESBOrderUpClass esborderup = new ESBOrderUpClass(Session);
            esborderup.CDM_C0701_CHANGECARDADDR = "1"; //是否為改卡單地址 0:否/1:是
            esborderup.CDM_C0701_PID = m_Id; //身分證字號
            esborderup.CDM_C0701_NAME = this.lblname1.Text.Trim(); //姓名
            esborderup.CDM_C0701_CELLPHONE = this.txtTel.Text.Trim(); //CellPhone
            esborderup.CDM_C0701_MEMO = this.txtCNoteA.Text.Trim();//聯絡人備註
            esborderup.CDM_C0701_CARDNO = m_CardNo; //卡號
            esborderup.CDM_C0701_POSTALAREA = BaseHelper.ToDBC(this.lblNewzipAjax.Text.Trim()); //郵遞區號*改卡單地址為必填*
            esborderup.CDM_C0701_ADDR1 = this.CustAdd1.strAddress; //地址1*改卡單地址為必填*
            esborderup.CDM_C0701_ADDR2 = this.txtAdd2Ajax.Text.Trim(); //地址2*改卡單地址為必填*
            esborderup.CDM_C0701_ADDR3 = this.txtAdd3Ajax.Text.Trim(); //地址3
            //卡片類別_0:新卡新戶/_1:新卡舊戶/_2:掛毀補/_3:年度換卡/_4:卡退回
            esborderup.DICT_CUSTOMWORD_ID = this.newCardnewAccount.Checked == true ? UtilHelper.GetAppSettings("DICT_CUSTOMWORD_ID_0") : this.newCardoldAccount.Checked == true ? UtilHelper.GetAppSettings("DICT_CUSTOMWORD_ID_1") : "";
            //原取卡方式
            string takeWayOri = string.Empty;
            GetKindName("2", m_Kind, ref takeWayOri);
            string takeWayNew = string.Empty;
            GetKindName("2", "6", ref takeWayNew);
            esborderup.ORIG_GETCARDTYPE_ID = !string.IsNullOrEmpty(takeWayOri)?takeWayOri.Substring(0,1):""; //原取卡方式
            esborderup.NEW_GETCARDTYPE_ID = !string.IsNullOrEmpty(takeWayNew)?takeWayNew.Substring(0,1):""; //新取卡方式
            #endregion params


            strResult = ConntoESB.ConnESB(esborderup, "1");
                   
            //當線路1 連線失敗 ,再換線路2
            if (esborderup.ConnStatus == "F")
                strResult = ConntoESB.ConnESB(esborderup, "2");

            //取資料
            caseNo = esborderup.CaseNo;
            resultErrorMsg = esborderup.ErrorMessage;
            resultRspCode = esborderup.RspCode;
            resultErrorCode = esborderup.ErrorCode;

            if (esborderup.ConnStatus == "S")
            {
                if (resultRspCode != "-1" || string.IsNullOrEmpty(caseNo))
                {
                    jsBuilder.RegScript(this.Page, string.Format("alert('ESB電文發送失敗：{0}；錯誤代碼：{1}')", resultErrorMsg, resultErrorCode));
                    Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esborderup.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Error, LogLayer.UI);
                    return;
                }
                if (resultRspCode == "-1" && !(string.IsNullOrEmpty(caseNo)))
                {
                    jsBuilder.RegScript(this.Page, "alert('ESB 電文發送成功')");
                    Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；CallESB SUCCESS；CardFlow_Default_CallEMFS SUCCESS", esborderup.StatusCode, resultRspCode), LogState.Info, LogLayer.UI);
                }
            }
            else
            {
                jsBuilder.RegScript(this.Page, string.Format("alert('ESB電文發送失敗：{0}；錯誤代碼：{1}')", resultErrorMsg, resultErrorCode));
                Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esborderup.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Error, LogLayer.UI);
                return;
            }
            #endregion

            CardDataChange.NewWay = "6";

            string takWayNew = string.Empty;
            GetKindName("2", "6", ref takWayNew);

            string takWay = string.Empty;
            GetKindName("2", m_Kind, ref takWay);

            //CardDataChange.CNote = this.txtCNoteC.Text.Trim();//*備註
            CardDataChange.CNote = this.txtCNoteA.Text.Trim() + string.Format(" 類別：{0}，工單編號：{1}",this.newCardnewAccount.Checked?"新卡新戶":this.newCardoldAccount.Checked?"新卡舊戶":"",  caseNo);//*備註

            CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_02020000_008", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), takWay, takWayNew + "  (新卡改址)");//*異動記錄說明
            CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
            CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
            //CardDataChange.Type_Flg = "A";
            CardDataChange.OutputFlg = "N";
            CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            CardDataChange.CardNo = m_CardNo;
            CardDataChange.action = m_Action;
            CardDataChange.Trandate = m_Trandate;
            CardDataChange.indate1 = this.lblIndate1.Text;
            CardDataChange.id = m_Id;

            //新卡改址時，如果有未轉出的取卡方式異動則做退單處理。
            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, "1");

            if (!string.IsNullOrEmpty(m_Id))
            {
                sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
            }


            sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
            sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);
            sqlhelp.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.Equal, DataTypeUtils.String, "N");
            //sqlhelp.AddCondition(Entity_CardDataChange.M_Type_flg, Operator.NotEqual, DataTypeUtils.String,"A");
            DataTable dtCardDataChange = new DataTable();
            if (BRM_CardDataChange.SearchByChange(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, "Newway"))
            {
                if (dtCardDataChange.Rows.Count > 0)
                {
                    //*修改異動單
                    Entity_CardDataChange CardDataChanges = new Entity_CardDataChange();
                    CardDataChanges.Sno = int.Parse(dtCardDataChange.Rows[0]["Sno"].ToString());
                    CardDataChanges.OutputFlg = "T";
                    SqlHelper sqlhelps = new SqlHelper();
                    sqlhelps.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChanges.Sno.ToString());
                    BRM_CardDataChange.update(CardDataChanges, sqlhelps.GetFilterCondition(), ref strMsgID, "OutputFlg");
                }
            }

            blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
            //max add 感覺之前有問題 
            if (blnResult)
            {
                this.BindData();
                this.UpdatePanel1.Update();
            }
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020103_000") + "')");
            }
            return;
        }


        //if (this.hidA.Value.Equals("True") || this.hidA.Value.Equals("Trues"))
        if (this.hidA.Value.Equals("True"))
        {
            //*修改異動單
            CardDataChange.Sno = int.Parse(m_SNoA);
            CardDataChange.OutputFlg = "T";
            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChange.Sno.ToString());
            BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg");
        }

        CardDataChange.OldZip = this.lblOldzipAjax.Text.Trim();
        CardDataChange.NewZip = BaseHelper.ToSBC(this.lblNewzipAjax.Text.Trim());//*郵遞區號全碼
        CardDataChange.OldAdd1 = this.lblAdd1Ajax.Text.Trim();
        CardDataChange.NewAdd1 = BaseHelper.ToSBC(this.CustAdd1.strAddress);     //*地址一全碼
        CardDataChange.OldAdd2 = this.lblAdd2Ajax.Text.Trim();
        CardDataChange.NewAdd2 = BaseHelper.ToSBC(this.txtAdd2Ajax.Text.Trim()); //*地址二全碼
        CardDataChange.OldAdd3 = this.lblAdd3Ajax.Text.Trim();
        CardDataChange.NewAdd3 = BaseHelper.ToSBC(this.txtAdd3Ajax.Text.Trim()); //*地址三全碼
        CardDataChange.CNote = this.txtCNoteA.Text.Trim();//*備註

        if (!CardDataChange.OldAdd1.Equals(CardDataChange.NewAdd1))
        {
            CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址一", CardDataChange.OldAdd1, CardDataChange.NewAdd1, "");//*異動記錄說明 
        }

        if (!CardDataChange.OldAdd2.Equals(CardDataChange.NewAdd2))
        {
            if (string.IsNullOrEmpty(CardDataChange.NoteCaptions))
            {
                CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址二", CardDataChange.OldAdd2, CardDataChange.NewAdd2, "");//*異動記錄說明 
            }
            else
            {
                CardDataChange.NoteCaptions += ";  " + MessageHelper.GetMessage("06_06020104_006", "地址二", CardDataChange.OldAdd2, CardDataChange.NewAdd2);//*異動記錄說明 
            }
        }

        if (!CardDataChange.OldAdd3.Equals(CardDataChange.NewAdd3))
        {
            if (string.IsNullOrEmpty(CardDataChange.NoteCaptions))
            {
                CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址三", CardDataChange.OldAdd3, CardDataChange.NewAdd3, "");//*異動記錄說明 
            }
            else
            {
                CardDataChange.NoteCaptions += ";  " + MessageHelper.GetMessage("06_06020104_006", "地址三", CardDataChange.OldAdd3, CardDataChange.NewAdd3);//*異動記錄說明 
            }
        }
        if (null != CardDataChange.NoteCaptions && !string.IsNullOrEmpty(CardDataChange.NoteCaptions))
        {
         if (this.hidC.Value.Equals("True"))
         {
             CardDataChange.NoteCaptions = CardDataChange.NoteCaptions.Replace('(', ' ').Replace(')', ' ').Replace(" ", "") + "(" + strUserName + ")  (踢退前異動)";
          }else{
             CardDataChange.NoteCaptions = CardDataChange.NoteCaptions.Replace('(', ' ').Replace(')', ' ').Replace(" ", "") + "(" + strUserName + ")";
         }
        }


        //CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), BaseHelper.GetShowText("06_06020101_074"), lblAdd1Ajax.Text.Trim(), this.CustAdd1.strAddress);//*異動記錄說明

        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);


        string strLogMsg = BaseHelper.GetShowText("06_02020000_000") + "：" + BaseHelper.GetShowText("06_02020003_015");
        BRM_Log.Insert(CardDataChange.UpdUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

        if (blnResult)
        {
            this.BindData();
            this.UpdatePanel1.Update();
        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020103_000") + "')");
        }
    }


    /// <summary>
    /// 功能說明:修改取卡方式
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureC_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
        bool blnResult = true;
        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        //if (this.hidC.Value.Equals("True") || this.hidC.Value.Equals("Trues"))
        if (this.hidC.Value.Equals("True"))
        {
            //*修改異動單
            CardDataChange.Sno = int.Parse(m_SNoC);
            CardDataChange.OutputFlg = "T";
            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChange.Sno.ToString());
            BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg");
        }

        //*新增異動單
        CardDataChange.action = m_Action;
        if (!string.IsNullOrEmpty(m_Id))
        {
            CardDataChange.id = m_Id;
        }
        CardDataChange.indate1 = lblIndate1.Text.Trim();
        CardDataChange.CardNo = m_CardNo;
        CardDataChange.Trandate = m_Trandate;
        CardDataChange.OldWay = m_Kind;//*取卡方式只能寫入Code
        CardDataChange.NewWay = this.dropKindAjax.SelectedValue;
        //if (this.chkUrgencyFlg.Checked == true)
        //{
        //    CardDataChange.UrgencyFlg = "1";//*1代表緊急製卡，0代表普通製卡
        //}
        //else
        //{
        //    CardDataChange.UrgencyFlg = "0";
        //}
        CardDataChange.CNote = this.txtCNoteC.Text.Trim();//*備註
        //max 增加註記明顯，剔退前單要特別備註
        if (this.hidC.Value.Equals("True"))
        {
            CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_004", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), this.lblKindAjax.Text.Trim(), this.dropKindAjax.SelectedItem.Value + " " + this.dropKindAjax.SelectedItem.Text + "  (踢退前異動)", strUserName);//*異動記錄說明
        }
        else
        {
            CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_004", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), this.lblKindAjax.Text.Trim(), this.dropKindAjax.SelectedItem.Value + " " + this.dropKindAjax.SelectedItem.Text, strUserName);//*異動記錄說明
        }

        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);

        string strLogMsg = BaseHelper.GetShowText("06_02020000_000") + "：" + BaseHelper.GetShowText("06_02020003_022");
        BRM_Log.Insert(CardDataChange.UpdUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

        if (blnResult)
        {
            this.BindData();
            this.UpdatePanel1.Update();
        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020103_000") + "')");
        }


    }

    /// <summary>
    /// 功能說明:備注-異動欄位
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="strField">字段名稱</param>
    /// <param name="blnIs">是否已有作業單 true 有  false 沒有</param>
    private void SearchDataChange(string strField, ref bool blnIs, string strSource, ref string strResult, ref string strSno)
    {
        DataTable dtCardDataChange = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);

        if (BRM_CardDataChange.SearchByChange(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, strField))
        {
            //*如有未轉出作業單則用最近的一筆資料替代
            if (dtCardDataChange.Rows.Count > 0)
            {
                strResult = dtCardDataChange.Rows[0][strField].ToString();
                strSno = dtCardDataChange.Rows[0]["Sno"].ToString();
                blnIs = true;
            }
            else
            {
                //strResult = strSource;
                blnIs = false;
            }
        }
    }

    /// <summary>
    /// 功能說明:取得異動欄位的內容，多判斷是否前一筆的異動為綜和資料異動
    /// 作    者:Max Lu
    /// 創建時間:2011/1/23
    /// 修改記錄:
    /// </summary>
    /// <param name="strField">字段名稱</param>
    /// <param name="blnIs">是否已有作業單 true 有  false 沒有</param>
    private void SearchDataChange2(string strField, ref bool blnIs, string strSource, ref string strResult, ref string strSno, ref string strBaseFlg)
    {
        DataTable dtCardDataChange = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);

        if (BRM_CardDataChange.SearchByChange(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, strField))
        {
            //*如有未轉出作業單則用最近的一筆資料替代
            if (dtCardDataChange.Rows.Count > 0)
            {
                strResult = dtCardDataChange.Rows[0][strField].ToString();
                strSno = dtCardDataChange.Rows[0]["Sno"].ToString();
                strBaseFlg = dtCardDataChange.Rows[0]["BaseFlg"].ToString();
                blnIs = true;
            }
            else
            {
                //strResult = strSource;
                blnIs = false;
            }
        }
    }

    /// <summary>
    /// 功能說明:彈出地址一Panel
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateA_Click(object sender, EventArgs e)
    {
        InitControls();
        m_Status = "N";
        category_tr.Visible = false;
        if (!string.IsNullOrEmpty(this.lblAction.Text) && this.lblAction.Text.Substring(0, 1) == "1" && !string.IsNullOrEmpty(this.lblCardtype.Text) && funCheckCreditCard(this.lblCardtype.Text)) //卡片類別為新卡且為信用卡
        {
            category_tr.Visible = true;
            category.ForeColor = System.Drawing.Color.Red;
            this.newCardnewAccount.Text = BaseHelper.GetShowText("06_06020101_087"); //新卡新戶
            this.newCardoldAccount.Text = BaseHelper.GetShowText("06_06020101_088"); //新卡舊戶
        }        
        this.ModalPopupExtenderA.Show();
    }

    /// <summary>
    /// 功能說明:彈出取卡方式Panel
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateC_Click(object sender, EventArgs e)
    {
        InitControls();
        this.ModalPopupExtenderC.Show();
    }

    /// <summary>
    /// 功能說明:通過Code獲得Name
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="strResultKind"></param>
    /// <param name="strKindName"></param>
    public void GetKindName(string strPropertyKey, string strCode, ref string strName)
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();

        //*取卡方式顯示Code+Name
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", strPropertyKey, ref dtKindName))
        {
            DataRow[] rowKindName = dtKindName.Select("PROPERTY_CODE='" + strCode + "'");
            if (rowKindName != null && rowKindName.Length > 0)
            {
                strName = rowKindName[0]["PROPERTY_CODE"].ToString() + " " + rowKindName[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                strName = strCode;
            }
        }
    }

    /// <summary>
    /// 功能說明:綁定取卡方式
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    public void BindKind()
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKindName))
        {
            foreach (DataRow dr in dtKindName.Rows)
            {
                if (dr["PROPERTY_CODE"].ToString().Equals("0") ||
                    dr["PROPERTY_CODE"].ToString().Equals("1") ||
                    dr["PROPERTY_CODE"].ToString().Equals("2") ||
                    dr["PROPERTY_CODE"].ToString().Equals("3") ||
                    dr["PROPERTY_CODE"].ToString().Equals("4") ||
                    dr["PROPERTY_CODE"].ToString().Equals("10") ||
                    dr["PROPERTY_CODE"].ToString().Equals("11"))
                {

                    ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                    dropKindAjax.Items.Add(liTmp);
                }
            }
        }
    }

    /// <summary>
    /// 功能說明:綁定郵編
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    protected void CustAdd1_ChangeValues()
    {
        this.lblNewzipAjax.Text = this.CustAdd1.strZip;
        if (m_Status.Equals("Y"))
        {
            this.ModalPopupExtenderA.Hide();
        }
        else
        {
            this.ModalPopupExtenderA.Show();
        }
    }

    /// <summary>
    /// 功能說明:初始化頁面控件的值
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    public void InitControls()
    {
        this.txtAdd2Ajax.Text = string.Empty;
        this.txtAdd3Ajax.Text = string.Empty;

        this.txtCNoteA.Text = string.Empty;
        this.txtCNoteC.Text = string.Empty;
    }
    /// <summary>
    /// 功能說明:MergeTable加載製卡廠名稱
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtCard)
    {
        string strMsgID = string.Empty;
        dtCard.Columns.Add("Merch_CodeS");
        //*製卡廠名稱(Merch_Code)
        DataTable dtMerch_Code = new DataTable();

        foreach (DataRow row in dtCard.Rows)
        {

            //*製卡廠名稱顯示Code+Name
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtMerch_Code))
            {
                DataRow[] rowMerch_Code = dtMerch_Code.Select("PROPERTY_CODE='" + row["Merch_Code"].ToString() + "'");
                if (rowMerch_Code != null && rowMerch_Code.Length > 0)
                {
                    row["Merch_CodeS"] = rowMerch_Code[0]["PROPERTY_CODE"].ToString() + " " + rowMerch_Code[0]["PROPERTY_NAME"].ToString();
                }
                else
                {
                    row["Merch_CodeS"] = row["Merch_Code"].ToString();
                }
            }
        }
    }
    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustButton custbtn = e.Row.Cells[2].FindControl("btnSure") as CustButton;
            TextBox txtBox = e.Row.Cells[1].FindControl("txtcNote") as TextBox;
            TextBox txtUser = e.Row.Cells[3].FindControl("txtUser") as TextBox;
            if (!txtUser.Text.Equals(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id))
            {
                custbtn.Enabled = false;
                txtBox.Enabled = false;
            }
            else
            {
                custbtn.Enabled = true;
                txtBox.Enabled = true;
            }
        }
    }

    //20161108 (U) by Tank, 判斷是否為信用卡
    protected bool funCheckCreditCard(string strCardType)
    {
        DataHelper dh = new DataHelper();
        SqlCommand sqlcmd = new SqlCommand();
        DataSet ds = new DataSet();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 240;
            sqlcmd.CommandText = @"select CardTypeName from tbl_CardType where BankCardFlag='Y' and CardType=@CardType ";

            SqlParameter ParCardType = new SqlParameter("@CardType", strCardType);
            sqlcmd.Parameters.Add(ParCardType);
            ds = dh.ExecuteDataSet(sqlcmd);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw ex;
        }
    }
}
