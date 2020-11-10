//******************************************************************
//*  功能說明：郵局查單申請處理UI層
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using EntityLayer;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;

public partial class P06020101 : PageBase
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

    /// <summary>
    /// 功能說明:頁面加載綁定數據
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
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
            if (Request.QueryString["flag"] == "View")
            {
                btnUpdateA.Visible = false;
                btnUpdateC.Visible = false;
                btnUpdateG.Visible = false;
                btnUpdateM.Visible = false;
                btnUpdateN.Visible = false;
                btnUpdateP.Visible = false;
            }
            BindData();
            m_Status = "Y";
            ViewState["FlgEdit"] = "FALSE";
        }
    }

    /// <summary>
    /// 功能說明:BindData
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
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
        SearchBack();
        //*綁定取卡方式
        BindKind();
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
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
    /// 功能說明:ModalPopupExtender confirm 
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    public void ModalPopupExtender()
    {
        //*此處需要判斷是否已有轉出作業單
        if (1 == 1)
        {
            this.btnSureN.Attributes.Add("onclick", "return confirm('已有未轉出作業單，修改後姓名 : XXX')");
        }
    }

    /// <summary>
    /// 功能說明:明細資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
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
            string strResultName2 = string.Empty;
            bool blnIsName = true;
            string strSnoN = string.Empty;
            SearchDataChange("NewName", ref blnIsName, strSourceName, ref strResultName, ref strResultName2, ref strSnoN);
            m_SNoN = strSnoN;
            this.hidN.Value = blnIsName.ToString();
            this.lblname1.Text = strSourceName; //*收件人姓名
            this.lblName1Ajax.Text = strSourceName;
            this.lblname2.Text = strResultName;
            if (!strResultName2.Equals(string.Empty))
            {
                this.lblname1.Text = strResultName2;
            }


            this.lblTrandate.Text = row["Trandate"].ToString(); //*轉檔日
            this.lblIndate1.Text = row["Indate1"].ToString(); //*製卡日

            
            this.lblCardno.Text = row["Cardno"].ToString(); //*卡號一 
            this.lblExpdate.Text = row["Expdate"].ToString(); //*有效期限一

            string strSourceZip = row["Zip"].ToString();
            string strResultZip = string.Empty;
            string strResultZip2 = string.Empty;
            bool blnIsZip = true;
            string strSnoZ = string.Empty;
            SearchDataChange("Newzip", ref blnIsZip, strSourceZip, ref strResultZip, ref strResultZip2, ref strSnoZ);
            this.lblZip.Text = strSourceZip; //*郵遞區號
            this.lblOldzipAjax.Text = strSourceZip;
            this.lblZipnew.Text = strResultZip;
            if (!strResultZip2.Equals(string.Empty))
            {
                this.lblZip.Text = strResultZip2;
            }

            //*地址一
            string strSourceAdd1 = row["Add1"].ToString();
            string strResultAdd1 = string.Empty;
            string strResultAdd12 = string.Empty;
            bool blnIsAdd1 = true;
            string strSnoA = string.Empty;
            SearchDataChange("Newadd1", ref blnIsAdd1, strSourceAdd1, ref strResultAdd1, ref strResultAdd12, ref strSnoA);
            m_SNoA = strSnoA;
            this.hidA.Value = blnIsAdd1.ToString();
            this.lblAdd1.Text = strSourceAdd1;
            this.lblAdd1Ajax.Text = strSourceAdd1;
            this.lblAdd1New.Text = strResultAdd1;
            if (!strResultAdd12.Equals(string.Empty))
            {
                this.lblAdd1.Text = strResultAdd12;
            }
            
            //*地址二
            string strSourceAdd2 = row["Add2"].ToString();
            string strResultAdd2 = string.Empty;
            string strResultAdd22 = string.Empty;
            bool blnIsAdd2 = true;
            string strSno = string.Empty;
            SearchDataChange("Newadd2", ref blnIsAdd2, strSourceAdd2, ref strResultAdd2, ref strResultAdd22, ref strSno);
            this.lblAdd2.Text = strSourceAdd2;
            this.lblAdd2Ajax.Text = strSourceAdd2;
            this.lblAdd2New.Text = strResultAdd2;
            if (!strResultAdd22.Equals(string.Empty))
            {
                this.lblAdd2.Text = strResultAdd22;
            }

            //*地址三
            string strSourceAdd3 = string.Empty;
            if (row["Add3"] != null)
            {
                strSourceAdd3 = row["Add3"].ToString();
            }
            string strResultAdd3 = string.Empty;
            string strResultAdd32 = string.Empty;
            bool blnIsAdd3 = true;
            string strSnos = string.Empty;
            SearchDataChange("Newadd3", ref blnIsAdd3, strSourceAdd3, ref strResultAdd3, ref strResultAdd32, ref strSnos);
            this.lblAdd3.Text = strSourceAdd3;
            this.lblAdd3Ajax.Text = strSourceAdd3;
            this.lblAdd3New.Text = strResultAdd3;
            if (!strResultAdd32.Equals(string.Empty))
            {
                this.lblAdd3.Text = strResultAdd32;
            }

            string strSelfPick_Type = string.Empty;
            GetKindName("4", row["SelfPick_Type"].ToString(), ref strSelfPick_Type);
            this.lblSelfPick_Type.Text = strSelfPick_Type; //*領卡方式 

            string strSourceIntoStore_Status = string.Empty;
            if (row["IntoStore_Status"] != null)
            {
                strSourceIntoStore_Status = row["IntoStore_Status"].ToString();
                if (strSourceIntoStore_Status == "1")
                {
                    string strSourceOutStore_Status = string.Empty;
                    if (row["OutStore_Status"] != null)
                    {
                        strSourceOutStore_Status = row["OutStore_Status"].ToString();
                        if (strSourceOutStore_Status == "0")
                        {
                            this.lblSelfPick_Type.Text = strSelfPick_Type + "　　　　　　　　　　　(可至永吉分行領卡)";
                        }
                    }


                }
            }

            this.lblSelfPick_Date.Text = row["SelfPick_Date"].ToString(); //*領卡日期 

            string strAction = string.Empty;
            GetKindName("1", row["Action"].ToString(), ref strAction);
            this.lblAction.Text = strAction; //*卡別

            this.lblCardtype.Text = row["Cardtype"].ToString(); //*TYPE

            string strAffinity = string.Empty;
            if (null != row["Affinity"] && !string.IsNullOrEmpty(row["Affinity"].ToString()))
            {
                GetAffName(row["Affinity"].ToString(),ref strAffinity);
            }
            this.lblAffinity.Text = strAffinity; //*認同代碼

            string strPhoto = string.Empty;
            if (null != row["Photo"] && !string.IsNullOrEmpty(row["Photo"].ToString()))
            {
                GetKindName("3", row["Photo"].ToString(), ref strPhoto);
            }
            this.lblPhoto.Text = strPhoto; //*PHOTO

            string strSourceMonlimit = row["Monlimit"].ToString();
            string strResultMonlimit = string.Empty;
            string strResultMonlimit2 = string.Empty;
            bool blnIsMonlimit = true;
            string strSnoM = string.Empty;
            SearchDataChange("Newmonlimit", ref blnIsMonlimit, strSourceMonlimit, ref strResultMonlimit, ref strResultMonlimit2, ref strSnoM);
            m_SNoM = strSnoM;
            this.hidM.Value = blnIsMonlimit.ToString();
            this.lblMonlimit.Text = strSourceMonlimit;//*舊額度
            this.lblMonlimitAjax.Text = strSourceMonlimit;//*舊額度
            this.lblMonlimit2.Text = strResultMonlimit;//*新額度
            if (!strResultMonlimit2.Equals(string.Empty))
            {
                this.lblMonlimit.Text = strResultMonlimit2;
            }
            //if (!String.IsNullOrEmpty(strResultMonlimit))
            //{
            //    this.lblMonlimit.Text = strResultMonlimit; //*新額度
            //}
            //else
            //{
            //    this.lblMonlimit.Text = strSourceMonlimit; //*舊額度
            //}

            //if (!String.IsNullOrEmpty(strResultMonlimit))
            //{
            //    this.lblMonlimitAjax.Text = strResultMonlimit;
            //}
            //else
            //{
            //    this.lblMonlimitAjax.Text = strSourceMonlimit;
            //}

            this.lblCardno2.Text = row["Cardno2"].ToString(); //*卡號二
            this.lblExpdate2.Text = row["Expdate2"].ToString(); //*有效期限

            string strSourceKind = row["Kind"].ToString();
            string strResultKind = string.Empty;
            string strResultKind2 = string.Empty;
            bool blnIsKind = true;
            string strSnoC = string.Empty;
            SearchDataChange("Newway", ref blnIsKind, strSourceKind, ref strResultKind, ref strResultKind2, ref strSnoC);
            m_SNoC = strSnoC;
            this.hidC.Value = blnIsKind.ToString();
            m_Kind = strResultKind;

            string strKindName = string.Empty;
            string strKindName2 = string.Empty;
            string strKindName22 = string.Empty;
            GetKindName("2", strSourceKind, ref strKindName);
            GetKindName("2", strResultKind, ref strKindName2);
            this.lblKind.Text = strKindName; //*取卡方式
            this.lblKindAjax.Text = strKindName;
            this.lblKind2.Text = strKindName2;

            if (!strResultKind2.Equals(string.Empty))
            {
                GetKindName("2", strResultKind2, ref strKindName22);
                this.lblKind.Text = strKindName22;
            }

            bool blUrgencyFlg = true;
            string strUrgencyFlg = string.Empty;
            string strResultUrgency = string.Empty;
            string strResultUrgency2 = string.Empty;
            string strSnoUrg = string.Empty;
            if (null != row["Urgency_Flg"] && !string.IsNullOrEmpty(row["Urgency_Flg"].ToString()))
            {
                strUrgencyFlg = row["Urgency_Flg"].ToString();
            }
            SearchDataChange("UrgencyFlg", ref blUrgencyFlg, strUrgencyFlg, ref strResultUrgency, ref strResultUrgency2, ref strSnoUrg);

            if (!string.IsNullOrEmpty(strResultUrgency))
            {
                if (strResultUrgency.Equals("1"))
                {
                    chkUrgencyFlg.Checked = true;
                }
                else
                {
                    chkUrgencyFlg.Checked = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(strUrgencyFlg))
                {
                    if (strUrgencyFlg.Equals("1"))
                    {
                        chkUrgencyFlg.Checked = true;
                    }
                    else
                    {
                        chkUrgencyFlg.Checked = false;
                    }
                }
            }

            string strSourceMailno = row["Mailno"].ToString();
            string strResultMailno = string.Empty;
            string strResultMailno2 = string.Empty;
            bool blnIsMailno = true;
            string strSnoG = string.Empty;
            SearchDataChange("Newmailno", ref blnIsMailno, strSourceMailno, ref strResultMailno, ref strResultMailno2, ref strSnoG);
            m_SNoG = strSnoG;
            this.hidG.Value = blnIsMailno.ToString();
            this.lblMailno.Text = strSourceMailno; //*掛號號碼
            this.lblMailnoAjax.Text = strSourceMailno;
            this.lblMailno2.Text = strResultMailno;
            if (!strResultMailno2.Equals(string.Empty))
            {
                this.lblMailno.Text = strResultMailno2;
            }
            this.lblMerch_Code.Text = row["Merch_CodeS"].ToString(); //*製卡廠名稱

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

            string strSourceMaildate = row["Maildate"].ToString();
            string strResultMaildate = string.Empty;
            string strResultMaildate2 = string.Empty;
            bool blnIsMaildate = true;
            string strSnoP = string.Empty;
            if (!string.IsNullOrEmpty(strSourceMaildate))
            {
                SearchDataChange("NewMailDate", ref blnIsMaildate, strSourceMaildate, ref strResultMaildate, ref strResultMaildate2, ref strSnoP);

                m_SNoP = strSnoP;
                this.hidP.Value = blnIsMaildate.ToString();
                this.lblMaildate.Text = strSourceMaildate; //*　郵寄日
                this.lblMaildateAjax.Text = strSourceMaildate;
                this.lblMaildate2.Text = strResultMaildate;
                if (!strResultMaildate2.Equals(string.Empty))
                {
                    this.lblMaildate.Text = strResultMaildate2;
                }


                string strFromDate = strSourceMaildate;
                string strToDate = DateTime.Now.ToString("yyyy/MM/dd");
                string strMsg = string.Empty;
                if (ValidateHelper.IsValidDate(strFromDate, strToDate, ref strMsg))
                {
                    this.hidP.Value = "Trues";
                    this.hidPostData.Value = strSourceMaildate;
                }
            }
            else
            {
                SearchDataChange("NewMailDate", ref blnIsMaildate, strSourceMaildate, ref strResultMaildate, ref strResultMaildate2, ref strSnoP);

                m_SNoP = strSnoP;
                this.hidP.Value = blnIsMaildate.ToString();
                this.lblMaildate.Text = strSourceMaildate; //*　郵寄日
                this.lblMaildateAjax.Text = strSourceMaildate;
                this.lblMaildate2.Text = strResultMaildate;
                if (!strResultMaildate2.Equals(string.Empty))
                {
                    this.lblMaildate.Text = strResultMaildate2;
                }
            }

        }
    }

    /// <summary>
    /// 功能說明:卡片基本資料查詢
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:2020/10/06 Ares Luke 應業務需求新增顯示重寄掛號號碼
    /// 
    /// </summary>
    private void SearchBack()
    {
        DataTable dtCardBackInfo = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);

        if (BRM_CardBackInfo.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardBackInfo, ref strMsgID))
        {
            if (dtCardBackInfo.Rows.Count > 0)
            {
                //*最後一次退件資料绑定
                DataRow row = dtCardBackInfo.Rows[0];
                this.lblbackdate.Text = row["Backdate"].ToString();                   //退件時間
                this.lblreason.Text =GetBackName(row["Reason"].ToString());           //退件原因
                this.lblm_date.Text = row["Enddate"].ToString();                      //處理日期
                this.lblpreenditem.Text =GetOperactionName(row["Enditem"].ToString());//處理方式
                this.txtRemarkFB.Text = row["Endnote"].ToString();                    //備注
                this.lblEndDate.Text = row["Closedate"].ToString();                   //結案日期
                this.lblBackMailNo.Text = row["mailno"].ToString();                       //重寄掛號號碼
            }
        }
    }

    /// <summary>
    /// 功能說明:查詢郵局資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
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
                    strM_date = dtPostSend.Rows[0]["M_date"].ToString().Replace("/","").Substring(0,8);//*處理日期取前8碼
                }
                if (dtPostSend.Rows[0]["Info1"].ToString().Equals("240") && dtPostSend.Rows[0]["Send_status_Code"].ToString().Equals("G2"))
                {
                    //*資訊代碼1=240 且 郵件狀態代號=’G2’才需顯示招領郵局欄位
                    blnPost = true;
                }
                strPost_Name = dtPostSend.Rows[0]["Post_Name"].ToString();
            }
        }
    }

    /// <summary>
    /// 功能說明:取消操作
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060201000001.aspx");
    }

    /// <summary>
    /// 功能說明:更新備注
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string strMsgID = string.Empty;
        CustButton btnSure = grvUserView.Rows[e.NewEditIndex].Cells[2].FindControl("btnSure") as CustButton;
        TextBox txtcNote = grvUserView.Rows[e.NewEditIndex].Cells[1].FindControl("txtcNote") as TextBox;
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
            CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChange.Sno.ToString());
            
            string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_023");
            BRM_Log.Insert(CardDataChange.UpdUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
            
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/07
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["flag"] == "View")
        {
            string strIndate1FromDate0510 = Session["Indate1FromDate0510"].ToString().Trim();
            string strIndate1ToDate0510 = Session["Indate1ToDate0510"].ToString().Trim();
            string strFromDate0510 = Session["FromDate0510"].ToString().Trim();
            string strToDate0510 = Session["ToDate0510"].ToString().Trim();
            string strPageIndex0510 = Session["PageIndex0510"].ToString().Trim();
            string strFactory0510 = Session["Factory0510"].ToString().Trim();

            Response.Redirect("P060510000001.aspx?Indate1FromDate0510=" + RedirectHelper.GetEncryptParam(strIndate1FromDate0510) + " &Indate1ToDate0510=" + RedirectHelper.GetEncryptParam(strIndate1ToDate0510) + " &FromDate0510=" + RedirectHelper.GetEncryptParam(strFromDate0510) + " &ToDate0510=" + RedirectHelper.GetEncryptParam(strToDate0510) + " &Factory0510=" + RedirectHelper.GetEncryptParam(strFactory0510) + " &PageIndex0510=" + RedirectHelper.GetEncryptParam(strPageIndex0510) + "");
        }
        else
        {
            if (Request.QueryString["searchID"] != null && 
                Request.QueryString["searchNo"] != null && 
                Request.QueryString["searchMailNo"] != null)
            {
                //* 傳遞參數解密
                string strID = RedirectHelper.GetDecryptString(Request.QueryString["searchID"].ToString());
                string strNo = RedirectHelper.GetDecryptString(Request.QueryString["searchNo"].ToString());
                string strMailNo = RedirectHelper.GetDecryptString(Request.QueryString["searchMailNo"].ToString());
                Response.Redirect("P060201000001.aspx?searchID=" + RedirectHelper.GetEncryptParam(strID) + "&searchNo=" + RedirectHelper.GetEncryptParam(strNo) + "&searchMailNo=" + RedirectHelper.GetEncryptParam(strMailNo) + "", false);
            }
            else
            {
                Response.Redirect("P060201000001.aspx");
            }
        }
    }

    /// <summary>
    /// 功能說明:修改收件人姓名
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureN_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
        bool blnResult = true;
        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        if (this.hidN.Value.Equals("True"))
        {
            //*修改異動單
            CardDataChange.Sno = int.Parse(m_SNoN);
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
        CardDataChange.OldName = this.lblName1Ajax.Text.Trim();
        CardDataChange.NewName = this.txtName1Ajax.Text.Trim();
        CardDataChange.CNote = this.txtCNoteN.Text.Trim();//*備註
        CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "收件人姓名", this.lblName1Ajax.Text.Trim(), this.txtName1Ajax.Text.Trim(), strUserName);//*異動記錄說明
        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.BaseFlg = "1";
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") +"："+ BaseHelper.GetShowText("06_06020101_041");
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
    /// 功能說明:修改郵寄日
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureP_Click(object sender, EventArgs e)
    {
        if (!BaseHelper.CheckDate(this.txtMaildateAjax.Text.Trim()))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_005");
            this.ModalPopupExtenderP.Show();
            return;
        }

        string strShowMsg = string.Empty;
        if (!ValidateHelper.IsValidDate(lblIndate1.Text, this.txtMaildateAjax.Text, ref strShowMsg))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_008");
            this.ModalPopupExtenderP.Show();
            return;
        }

        string strMsgID = string.Empty;
        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
        bool blnResult = true;
        if (this.hidP.Value.Equals("True"))
        {
            //*修改異動單
            CardDataChange.Sno = int.Parse(m_SNoP);
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
        CardDataChange.OldMailDate = this.lblMaildateAjax.Text.Trim();
        CardDataChange.NewMailDate = Convert.ToDateTime(this.txtMaildateAjax.Text.Trim()).ToString("yyyy/MM/dd");
        CardDataChange.CNote = this.txtCNoteP.Text.Trim();//*備註
        CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "郵寄日", this.lblMaildateAjax.Text.Trim(), this.txtMaildateAjax.Text.Trim(), strUserName);//*異動記錄說明
        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.BaseFlg = "1";
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_045");
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
    /// 功能說明:修改地址(1,2,3)
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureA_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
        bool blnResult = true;
        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        if (this.hidA.Value.Equals("True"))
        {
            //*修改異動單
            CardDataChange.Sno = int.Parse(m_SNoA);
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
            CardDataChange.NoteCaptions = CardDataChange.NoteCaptions.Replace('(', ' ').Replace(')', ' ').Replace(" ", "") + "(" + strUserName + ")";
        }
        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.BaseFlg = "1";
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_066");
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
    /// 功能說明:修改額度
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureM_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
        bool blnResult = true;
        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        if (this.hidM.Value.Equals("True"))
        {
            //*修改異動單
            CardDataChange.Sno = int.Parse(m_SNoM);
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
        if (!string.IsNullOrEmpty(lblMonlimitAjax.Text))
        {
            CardDataChange.OldMonlimit = int.Parse(this.lblMonlimitAjax.Text.Trim());
        }
        if (!string.IsNullOrEmpty(txtMonlimitAjax.Text))
        {
            CardDataChange.NewMonlimit = int.Parse(this.txtMonlimitAjax.Text.Trim());
        }
        CardDataChange.CNote = this.txtCNoteM.Text.Trim();//*備註
        CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "額度", this.lblMonlimitAjax.Text.Trim(), this.txtMonlimitAjax.Text.Trim(), strUserName);//*異動記錄說明
        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.BaseFlg = "1";
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_050");
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
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
        string strUrgencyFlg = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
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
        if (this.chkUrgencyFlg.Checked == true)
        {
            CardDataChange.UrgencyFlg = "1";//*1代表緊急製卡，0代表普通製卡
            strUrgencyFlg = MessageHelper.GetMessage("06_06020104_007");
        }
        else
        {
            CardDataChange.UrgencyFlg = "0";
        }
        CardDataChange.CNote = this.txtCNoteC.Text.Trim();//*備註
        //CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_004", this.lblKindAjax.Text.Trim(), this.dropKindAjax.SelectedItem.Value + " " + this.dropKindAjax.SelectedItem.Text);//*異動記錄說明
        CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_004", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), this.lblKindAjax.Text.Trim(), this.dropKindAjax.SelectedItem.Text + strUrgencyFlg, strUserName);//*異動記錄說明
        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.BaseFlg = "1";
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_058");
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
    /// 功能說明:修改掛號號碼
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureG_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        bool blnResult = true;
        if (this.hidG.Value.Equals("True"))
        {
            //*修改異動單
            CardDataChange.Sno = int.Parse(m_SNoG);
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
        CardDataChange.oldMailno = this.lblMailnoAjax.Text.Trim();
        CardDataChange.newmailno = this.txtMailnoAjax.Text.Trim();
        CardDataChange.CNote = this.txtCNoteG.Text.Trim();//*備註
        CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "掛號號碼", this.lblMailnoAjax.Text.Trim(), this.txtMailnoAjax.Text.Trim(), strUserName);//*異動記錄說明
        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
        CardDataChange.BaseFlg = "1";
        CardDataChange.OutputFlg = "N";
        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_062");
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strField">字段名稱</param>
    /// <param name="blnIs">是否已有作業單 true 有  false 沒有</param>
    private void SearchDataChange(string strField, ref bool blnIs, string strSource, ref string strResult, ref string strResult2, ref string strSno)
    {
        strResult2 =string.Empty;
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
        //sqlhelp.AddCondition(Entity_CardDataChange.M_Type_flg, Operator.NotEqual, DataTypeUtils.String,"A");
        if (BRM_CardDataChange.SearchByChange(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, strField))
        {
            //*如有未轉出作業單則用最近的一筆資料替代
            //max 只要有前一筆異動單都要退掉
            if (dtCardDataChange.Rows.Count > 0)
            {
                //if (dtCardDataChange.Rows[0]["BaseFlg"].ToString() == "1")
                //{
                //    strResult2 = dtCardDataChange.Rows[0][strField].ToString();
                //    strSno = dtCardDataChange.Rows[0]["Sno"].ToString();
                //    blnIs = false;
                //}
                //else
                //{
                    strResult = dtCardDataChange.Rows[0][strField].ToString();
                    strSno = dtCardDataChange.Rows[0]["Sno"].ToString();
                    blnIs = true;
                //}
            }
            else
            {
                //strResult = strSource;
                blnIs = false;
            }
        }
    }

    /// <summary>
    /// 功能說明:彈出收件人姓名Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateN_Click(object sender, EventArgs e)
    {
        InitControls();
        this.ModalPopupExtenderN.Show();
    }
    /// <summary>
    /// 功能說明:彈出郵寄日Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateP_Click(object sender, EventArgs e)
    {
        InitControls();
        this.ModalPopupExtenderP.Show();
    }

    /// <summary>
    /// 功能說明:彈出地址一Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateA_Click(object sender, EventArgs e)
    {
        InitControls();
        m_Status = "N";
        this.ModalPopupExtenderA.Show();
    }
    /// <summary>
    /// 功能說明:彈出額度Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateM_Click(object sender, EventArgs e)
    {
        InitControls();
        this.ModalPopupExtenderM.Show();
    }
    /// <summary>
    /// 功能說明:彈出取卡方式Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
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
    /// 功能說明:彈出掛號號碼Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateG_Click(object sender, EventArgs e)
    {
        InitControls();
        this.ModalPopupExtenderG.Show();
    }


    /// <summary>
    /// 功能說明:通過Code獲得Name
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
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
    /// 功能說明:通過退件原因Code獲得Name
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strResultKind"></param>
    /// <param name="strKindName"></param>
    public string GetBackName(string strCode)
    {
        string strMsgID = string.Empty;
        string strName = strCode;
        DataTable dtBackName = new DataTable();

        //*取卡方式顯示Code+Name
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "5", ref dtBackName))
        {
            DataRow[] rowBackName = dtBackName.Select("PROPERTY_CODE='" + strCode + "'");
            if ( null != rowBackName && rowBackName.Length > 0)
            {
                strName = rowBackName[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                strName = strCode;
            }
        }
        return strName;
    }

    /// <summary>
    /// 功能說明:獲取退件處理方式的中文
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strResultKind"></param>
    /// <param name="strKindName"></param>
    public string GetOperactionName(string strCode)
    {
        string strMsgID = string.Empty;
        string strName = strCode;
        DataTable dtBackName = new DataTable();

        //*取卡方式顯示Code+Name
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "16", ref dtBackName))
        {
            DataRow[] rowBackName = dtBackName.Select("PROPERTY_CODE='" + strCode + "'");
            if (null != rowBackName && rowBackName.Length > 0)
            {
                strName = rowBackName[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                strName = strCode;
            }
        }
        return strName;
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
    /// 功能說明:綁定取卡方式
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void BindKind()
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();
        dropKindAjax.Items.Clear();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKindName))
        {
            foreach (DataRow dr in dtKindName.Rows)
            {
                ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                dropKindAjax.Items.Add(liTmp);
            }
        }
    }

    /// <summary>
    /// 功能說明:綁定郵編
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void InitControls()
    {
        this.txtName1Ajax.Text = string.Empty;
        this.txtMaildateAjax.Text = string.Empty;
        this.txtAdd2Ajax.Text = string.Empty;
        this.txtAdd3Ajax.Text = string.Empty;
        this.txtMonlimitAjax.Text = string.Empty;
        this.txtMailnoAjax.Text = string.Empty;

        this.txtCNoteN.Text = string.Empty;
        this.txtCNoteP.Text = string.Empty;
        this.txtCNoteA.Text = string.Empty;
        this.txtCNoteM.Text = string.Empty;
        this.txtCNoteC.Text = string.Empty;
        this.txtCNoteG.Text = string.Empty;
    }
    /// <summary>
    /// 功能說明:MergeTable加載製卡廠名稱
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
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

    /// <summary>
    /// 功能說明:GridView行绑定事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustButton custbtn = e.Row.Cells[2].FindControl("btnSure") as CustButton;
            TextBox txtBox = e.Row.Cells[1].FindControl("txtcNote") as TextBox;
            TextBox txtUser = e.Row.Cells[3].FindControl("txtUser") as TextBox;
            if (Request.QueryString["flag"] == "View")
            {
                custbtn.Visible = false;
            }
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
    protected void btnCancelN_Click(object sender, EventArgs e)
    {
        this.ModalPopupExtenderN.Hide();
    }
    protected void btnCancelP_Click(object sender, EventArgs e)
    {
        this.ModalPopupExtenderP.Hide();
    }
    protected void btnCancelA_Click(object sender, EventArgs e)
    {
        this.ModalPopupExtenderA.Hide();
    }
    protected void btnCancelM_Click(object sender, EventArgs e)
    {
        this.ModalPopupExtenderM.Hide();
    }
    protected void btnCancelC_Click(object sender, EventArgs e)
    {
        this.ModalPopupExtenderC.Hide();
    }
    protected void btnCancelG_Click(object sender, EventArgs e)
    {
        this.ModalPopupExtenderG.Hide();
    }
}
