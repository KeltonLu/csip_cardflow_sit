//******************************************************************
//*  功能說明：異動作業單強迫新增畫面
//*  作    者：HAO CHEN
//*  創建日期：2010/06/18
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
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
using CSIPCommonModel.EntityLayer;
//20210601_Ares_Stanley
using System.IO;
using TIBCO.EMS;
using ESBOrderUp;


public partial class P060202000003_NewWorkOrder : PageBase
{
    #region params
    public string indate1 = string.Empty;
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
    public bool blnIsName;
    public string strSno = string.Empty;

    public string m_SNoN
    {
        get { return ViewState["m_SNoN"] as string; }
        set { ViewState["m_SNoN"] = value; }
    }
    public string m_SNoA
    {
        get { return ViewState["m_SNoA"] as string; }
        set { ViewState["m_SNoA"] = value; }
    }

    public string m_SNoC
    {
        get { return ViewState["m_SNoC"] as string; }
        set { ViewState["m_SNoC"] = value; }
    }
    #endregion

    /// <summary>
    /// 功能說明:頁面加載事件
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
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
                txtId.Text = m_Id;
                txtCardNo.Text = m_CardNo;
            }
            if (null != Session["0202_txtId"] && !string.IsNullOrEmpty(Session["0202_txtId"].ToString()))
            {
                txtId.Text = Session["0202_txtId"].ToString();
            }

            if (null != Session["0202_txtNo"] && !string.IsNullOrEmpty(Session["0202_txtNo"].ToString()))
            {
                txtCardNo.Text = Session["0202_txtNo"].ToString();
            }
            m_Status = "Y";
            this.Page.Title = BaseHelper.GetShowText("06_02020003_011");
            BindData();
            //hidAdd.Value = "1";
        }
    }

    /// <summary>
    /// 功能說明:BindData
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    public void BindData()
    {
        //*標題列印
        ShowControlsText();

        //*綁定取卡方式
        BindKind();

        //*綁定卡別
        BindAction();

        //顯示卡片異動明細資料
        SearchDataChange();

        //取得indate1
        //SearchCardBaseInfo();
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //chkUrgencyFlg.Text = BaseHelper.GetShowText("06_02020003_021");
        btnSearch.Text = BaseHelper.GetShowText("06_02020003_003");
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_02020003_013");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_02020003_014");
    }

    /// <summary>
    /// 功能說明:綁定取卡方式
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
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
    /// 功能說明:綁定卡別
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    public void BindAction()
    {
        string strMsgID = string.Empty;
        DataTable dtActionName = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtActionName))
        {
            this.ddlAction.DataSource = dtActionName;
            this.ddlAction.DataTextField = "PROPERTY_NAME";
            this.ddlAction.DataValueField = "PROPERTY_CODE";
            this.ddlAction.DataBind();
        }
        if (!string.IsNullOrEmpty(m_Action))
        {
            ddlAction.SelectByValue(m_Action);
        }
    }

    /// <summary>
    /// 功能說明:取得製卡日
    /// 作    者:Ares_Stanley
    /// 創建時間:2021/07/19
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
            if (dtCardBaseInfo.Rows.Count > 0)
            {
                indate1 = !string.IsNullOrEmpty(dtCardBaseInfo.Rows[0]["indate1"].ToString()) ? dtCardBaseInfo.Rows[0]["indate1"].ToString() : "";
            }
        }
    }

    /// <summary>
    /// 功能說明:回上頁
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060202000001_NewWorkOrder.aspx");
    }

    /// <summary>
    /// 功能說明:初始化頁面控件的值
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    public void InitControls(string strType)
    {
        DataTable dtCardDataChange = new DataTable();
        m_Id = txtId.Text.Trim();
        m_CardNo = txtCardNo.Text.Trim();
        m_Action = ddlAction.SelectedValue.Trim();
        string strMsgID = string.Empty;
        switch (strType)
        {
            case "Way":
                SqlHelper sqlhelp = new SqlHelper();
                sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
                sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
                sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
                sqlhelp.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.NotEqual, DataTypeUtils.String, "T");
                sqlhelp.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.NotEqual, DataTypeUtils.String, "RS");
                sqlhelp.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.NotEqual, DataTypeUtils.String, "RL");
                sqlhelp.AddCondition(Entity_CardDataChange.M_NewWay, Operator.NotEqual, DataTypeUtils.String, "");

                if (BRM_CardDataChange.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID))
                {
                    if (dtCardDataChange.Rows.Count > 0)
                    {
                        DataTable dtBaseInfo = new DataTable();
                        dropKindAjax.SelectedValue = dtCardDataChange.Rows[0]["Newway"].ToString();
                        txtCNoteC.Text = dtCardDataChange.Rows[0]["CNote"].ToString();
                        //if (dtCardDataChange.Rows[0]["UrgencyFlg"].ToString().Equals("1"))
                        //{
                        //    chkUrgencyFlg.Checked = true;
                        //}
                        //else
                        //{
                        //    chkUrgencyFlg.Checked = false;
                        //}
                    }
                }
                break;
            case "Add":
                SqlHelper sqlhelpAdd = new SqlHelper();
                sqlhelpAdd.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
                sqlhelpAdd.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
                sqlhelpAdd.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
                sqlhelpAdd.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.NotEqual, DataTypeUtils.String, "T");
                sqlhelpAdd.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.NotEqual, DataTypeUtils.String, "RS");
                sqlhelpAdd.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.NotEqual, DataTypeUtils.String, "RL");
                sqlhelpAdd.AddCondition(Entity_CardDataChange.M_NewAdd1, Operator.NotEqual, DataTypeUtils.String, "");

                if (BRM_CardDataChange.SearchByCardNo(sqlhelpAdd.GetFilterCondition(), ref dtCardDataChange, ref strMsgID))
                {
                    if (dtCardDataChange.Rows.Count > 0)
                    {
                        CustAdd1.InitalAdd1(dtCardDataChange.Rows[0]["NewAdd1"].ToString());
                        txtAdd2Ajax.Text = dtCardDataChange.Rows[0]["NewAdd2"].ToString();
                        txtAdd3Ajax.Text = dtCardDataChange.Rows[0]["NewAdd3"].ToString();
                        txtCNoteA.Text = dtCardDataChange.Rows[0]["CNote"].ToString();
                    }
                }
                break;
        }

    }

    /// <summary>
    /// 功能說明:彈出取卡方式Panel
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateC_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        //檢核卡片基本資料是否存在
        if (BRM_TCardBaseInfo.IsExistByCard(txtId.Text.ToString().Trim(), txtCardNo.Text.ToString().Trim(), ddlAction.SelectedValue.Trim()))
        {
            jsBuilder.RegScript(this.UpdatePanel1, "alert('" + MessageHelper.GetMessage("06_02030000_007") + "')");
            return;
        }

        if (this.hidStatus.Value.Equals("") && hidWay.Value.Equals(""))
        {
            this.hidStatus.Value = "C";
            SearchDataChange();
            jsBuilder.RegScript(this.Page, "$('#btnUpKind').trigger('click');");
            //檢核畫面輸入欄位是否正確
            if (!CheckValue(ref strMsgID))
            {
                jsBuilder.RegScript(this.UpdatePanel1, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
                return;
            }
            //InitControls("Way");
            //this.ModalPopupExtenderC.Show();
            ////ClientScript.RegisterStartupScript(ClientScript.GetType(), "UpdateC", "$('#btnUpKind').trigger('click');");
            //return;
        }

        //檢核畫面輸入欄位是否正確
        if (!CheckValue(ref strMsgID))
        {
            jsBuilder.RegScript(this.UpdatePanel1, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }

        InitControls("Way");
        this.ModalPopupExtenderC.Show();
        //this.hidStatus.Value = "";
    }

    /// <summary>
    /// 功能說明:修改取卡方式
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureC_Click(object sender, EventArgs e)
    {
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_02020003_000") + "：" + BaseHelper.GetShowText("06_02020003_022");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
        UpdataDataChange("CC", "Newway");
    }


    /// <summary>
    /// 功能說明:彈出地址一Panel
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateA_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        //檢核卡片基本資料是否存在
        if (BRM_TCardBaseInfo.IsExistByCard(txtId.Text.ToString().Trim(), txtCardNo.Text.ToString().Trim(), ddlAction.SelectedValue.Trim()))
        {
            jsBuilder.RegScript(this.UpdatePanel1, "alert('" + MessageHelper.GetMessage("06_02030000_007") + "')");
            return;
        }

        if (this.hidStatus.Value.Equals(""))
        {
            this.hidStatus.Value = "A";
            SearchDataChange();
            jsBuilder.RegScript(this.Page, "$('#btnUpAdd').trigger('click');");
            //檢核畫面輸入欄位是否正確
            if (!CheckValue(ref strMsgID))
            {
                jsBuilder.RegScript(this.UpdatePanel1, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
                return;
            }
            InitControls("Add");
            m_Status = "N";
            category_tr.Visible = false;
            if (!string.IsNullOrEmpty(this.ddlAction.SelectedValue) && this.ddlAction.SelectedValue.ToString() == "1") //卡片類別為新卡
            {
                category_tr.Visible = true;
                this.category_Title.ForeColor = System.Drawing.Color.Red;
                this.newCardnewAccount.Text = BaseHelper.GetShowText("06_02020003_031"); //新卡新戶
                this.newCardoldAccount.Text = BaseHelper.GetShowText("06_02020003_032"); //新卡舊戶
            }
            this.ModalPopupExtenderA.Show();
            ClientScript.RegisterStartupScript(ClientScript.GetType(), "UpdateC", "$('#btnUpKind').trigger('click');");
            return;
        }


        //檢核畫面輸入欄位是否正確
        if (!CheckValue(ref strMsgID))
        {
            jsBuilder.RegScript(this.UpdatePanel1, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }

        InitControls("Add");
        m_Status = "N";
        this.ModalPopupExtenderA.Show();
        //this.hidStatus.Value = "";
    }

    /// <summary>
    /// 功能說明:修改地址(1,2,3)
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureA_Click(object sender, EventArgs e)
    {
        if (this.ddlAction.SelectedValue.Equals("1"))
        {
            //* 2設定取卡方式為保留
            UpdataDataChange("CC6", "Newway");
        }
        else
        {
            UpdataDataChange("CA", "NewAdd1");
        }
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_02020003_000") + "：" + BaseHelper.GetShowText("06_02020003_015");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
    }



    /// <summary>
    /// 功能說明:綁定郵編
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/18
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
    /// 功能說明:備注-異動欄位
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    private void SearchDataChange()
    {
        DataTable dtCardDataChange = new DataTable();
        m_Id = txtId.Text.Trim();
        m_CardNo = txtCardNo.Text.Trim();
        m_Action = ddlAction.SelectedValue.Trim();

        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);

        if (BRM_CardDataChange.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID))
        {
            this.grvUserView.DataSource = dtCardDataChange;
            this.grvUserView.DataBind();
        }
        if (dtCardDataChange.Rows.Count > 0)
        {
            DataRow[] drWay = dtCardDataChange.Select("Newway<>''");
            if (drWay.Length > 0)
            {
                hidWay.Value = "Show";
            }
            else
            {
                hidWay.Value = "";
            }
            DataRow[] drAdd = dtCardDataChange.Select("NewAdd1 <> ''");
            if (drAdd.Length > 0)
            {
                hidAdd.Value = "Show";
            }
            else
            {
                hidAdd.Value = "";
            }
        }
        else
        {
            hidWay.Value = "";
            hidAdd.Value = "";
        }
    }

    /// <summary>
    /// 功能說明:更新異動檔資料
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/22
    /// 修改記錄:2021/01/05_Ares_Stanley-修正CallEMFS SUCCESS的LOG層級
    /// </summary>
    /// <param name="strField">字段名稱</param>
    /// <param name="strChangeData">更新欄位項</param>
    private void UpdataDataChange(string strChangeData, string strField)
    {
        bool blnResult = false;
        DataTable dtCardDataChange = new DataTable();
        m_Id = txtId.Text.Trim();
        m_CardNo = txtCardNo.Text.Trim();
        m_Action = ddlAction.SelectedValue.Trim();

        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);

        BRM_CardDataChange.SearchByChange(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, strField);

        //*如有未轉出作業單則返回true 否則返回false
        if (null != dtCardDataChange && dtCardDataChange.Rows.Count > 0)
        {
            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
            //*修改異動單
            CardDataChange.Sno = int.Parse(dtCardDataChange.Rows[0]["Sno"].ToString());
            CardDataChange.OutputFlg = "T";
            SqlHelper sqlhelps = new SqlHelper();
            sqlhelps.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChange.Sno.ToString());
            sqlhelps.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.Equal, DataTypeUtils.String, "N");
            BRM_CardDataChange.update(CardDataChange, sqlhelps.GetFilterCondition(), ref strMsgID, "OutputFlg");
        }

        //*從基本資料檔取得最近的轉當日


        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        //*新增異動單
        Entity_CardDataChange CardDataChanges = new Entity_CardDataChange();
        CardDataChanges.id = m_Id;
        CardDataChanges.CardNo = m_CardNo;
        CardDataChanges.action = m_Action;
        CardDataChanges.Trandate = m_Trandate != "" ? m_Trandate : "";
        //CardDataChanges.indate1 = indate1;
        //CardDataChanges.Trandate = GetNewTranDate(m_Action, m_Id, m_CardNo);
        switch (strChangeData)
        {
            //*異動取卡方式
            case "CC":
                string strUrgencyFlg = string.Empty;
                if (null != dtCardDataChange && dtCardDataChange.Rows.Count > 0)
                {
                    m_Kind = dtCardDataChange.Rows[0]["NewWay"].ToString();
                }
                else
                {
                    m_Kind = "";
                }
                CardDataChanges.OldWay = m_Kind;//*取卡方式只能寫入Code
                CardDataChanges.NewWay = this.dropKindAjax.SelectedValue;
                //if (this.chkUrgencyFlg.Checked == true)
                //{
                //    CardDataChanges.UrgencyFlg = "1";//*1代表緊急製卡，0代表普通製卡
                //    strUrgencyFlg = MessageHelper.GetMessage("06_06020104_007");
                //}
                //else
                //{
                //    CardDataChanges.UrgencyFlg = "0";
                //}
                CardDataChanges.CNote = this.txtCNoteC.Text.Trim();//*備註
                string strKindName = string.Empty;
                GetKindName("2", m_Kind, ref strKindName);

                //CardDataChanges.NoteCaptions = MessageHelper.GetMessage("06_02020000_008", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), strKindName, this.dropKindAjax.SelectedItem.Value + this.dropKindAjax.SelectedItem.Text);//*異動記錄說明
                CardDataChanges.NoteCaptions = MessageHelper.GetMessage("06_06020104_004", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), strKindName, this.dropKindAjax.SelectedItem.Text + strUrgencyFlg, strUserName);//*異動記錄說明
                CardDataChanges.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChanges.UpdTime = DateTime.Now.ToString("HH:mm");
                CardDataChanges.BaseFlg = "0";
                CardDataChanges.Type_flg = "A";
                CardDataChanges.OutputFlg = "N";
                CardDataChanges.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                blnResult = BRM_CardDataChange.Insert(CardDataChanges, ref strMsgID);
                break;
            //*異動地址
            case "CA":
                if (null != dtCardDataChange && dtCardDataChange.Rows.Count > 0)
                {
                    CardDataChanges.OldZip = dtCardDataChange.Rows[0]["NewZip"].ToString();
                    CardDataChanges.OldAdd1 = dtCardDataChange.Rows[0]["NewAdd1"].ToString();
                    CardDataChanges.OldAdd2 = dtCardDataChange.Rows[0]["NewAdd2"].ToString();
                    CardDataChanges.OldAdd3 = dtCardDataChange.Rows[0]["NewAdd3"].ToString();
                }

                CardDataChanges.NewZip = BaseHelper.ToSBC(this.lblNewzipAjax.Text.Trim());//*郵遞區號全碼
                CardDataChanges.NewAdd1 = BaseHelper.ToSBC(this.CustAdd1.strAddress);     //*地址一全碼
                CardDataChanges.NewAdd2 = BaseHelper.ToSBC(this.txtAdd2Ajax.Text.Trim()); //*地址二全碼
                CardDataChanges.NewAdd3 = BaseHelper.ToSBC(this.txtAdd3Ajax.Text.Trim()); //*地址三全碼
                CardDataChanges.CNote = this.txtCNoteA.Text.Trim();//*備註

                string strOldAdd1 = "";
                string strOldAdd2 = "";
                string strOldAdd3 = "";
                if (!String.IsNullOrEmpty(CardDataChanges.OldAdd1))
                {
                    strOldAdd1 = CardDataChanges.OldAdd1;
                }
                if (!String.IsNullOrEmpty(CardDataChanges.OldAdd2))
                {
                    strOldAdd2 = CardDataChanges.OldAdd2;
                }
                if (!String.IsNullOrEmpty(CardDataChanges.OldAdd3))
                {
                    strOldAdd3 = CardDataChanges.OldAdd3;
                }
                if (!strOldAdd1.Equals(CardDataChanges.NewAdd1))
                {
                    CardDataChanges.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址一", strOldAdd1, CardDataChanges.NewAdd1, "");//*異動記錄說明 
                }

                if (!strOldAdd2.Equals(CardDataChanges.NewAdd2))
                {
                    if (string.IsNullOrEmpty(CardDataChanges.NoteCaptions))
                    {
                        CardDataChanges.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址二", strOldAdd2, CardDataChanges.NewAdd2, "");//*異動記錄說明 
                    }
                    else
                    {
                        CardDataChanges.NoteCaptions += ";  " + MessageHelper.GetMessage("06_06020104_006", "地址二", strOldAdd2, CardDataChanges.NewAdd2);//*異動記錄說明 
                    }
                }

                if (!strOldAdd3.Equals(CardDataChanges.NewAdd3))
                {
                    if (string.IsNullOrEmpty(CardDataChanges.NoteCaptions))
                    {
                        CardDataChanges.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址三", strOldAdd3, CardDataChanges.NewAdd3, "");//*異動記錄說明 
                    }
                    else
                    {
                        CardDataChanges.NoteCaptions += ";  " + MessageHelper.GetMessage("06_06020104_006", "地址三", strOldAdd3, CardDataChanges.NewAdd3);//*異動記錄說明 
                    }
                }
                CardDataChanges.NoteCaptions = CardDataChanges.NoteCaptions.Replace('(', ' ').Replace(')', ' ').Replace(" ", "") + "(" + strUserName + ")";

                CardDataChanges.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChanges.UpdTime = DateTime.Now.ToString("HH:mm");
                CardDataChanges.BaseFlg = "0";
                CardDataChanges.Type_flg = "A";
                CardDataChanges.OutputFlg = "N";
                CardDataChanges.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                blnResult = BRM_CardDataChange.Insert(CardDataChanges, ref strMsgID);
                break;

            //*異動取卡方式為保留
            case "CC6":
                #region 舊電文
                ////* 發送二代電訊單給徵信
                ////* 1調用web-service將異動后的地址經二代電訊單傳送給徵信覆核
                //CallEMFS.E00520 CallEmfs = new CallEMFS.E00520();
                ////CallEmfs.UserId = "Z00006660";                                  //* 從Session中讀取 Test
                //CallEmfs.UserId = ((EntityAGENT_INFO)Session["Agent"]).agent_id;  //* 從Session中讀取UserID
                //CallEmfs.CaseType = UtilHelper.GetAppSettings("CallEMFSType");  //*案件類型
                //CallEmfs.CardType = "新卡";                                     //* ACTION
                //CallEmfs.CustLevel = UtilHelper.GetAppSettings("CallEMFSLevel"); //*案件級別
                //CallEmfs.CustId = m_Id;                                         //* 歸戶ID
                //CallEmfs.CustName = txtUserName.Text.Trim();                    //* 歸戶姓名
                //CallEmfs.TelH = txtTel.Text.Trim();                             //* 歸戶電話
                //CallEmfs.CardList = m_CardNo;                                   //*卡號
                //CallEmfs.ChangeAddressYN = true;
                //CallEmfs.OthZipCode = BaseHelper.ToDBC(this.lblNewzipAjax.Text.Trim()); //*郵遞區號半碼
                //CallEmfs.OthAddress1 = this.CustAdd1.strAddress;
                //CallEmfs.OthAddress2 = this.txtAdd2Ajax.Text.Trim();
                //CallEmfs.OthAddress3 = this.txtAdd3Ajax.Text.Trim();
                //CallEmfs.SourceSystem = "CardImport";

                //CallEMFS.CreateProcess CreateEmfs = new CallEMFS.CreateProcess();
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
                #endregion 舊電文
                #region 20210601_Ares_Stanley新工單
                #region params
                string strResult = string.Empty;
                string caseNo = string.Empty;
                string resultErrorMsg = string.Empty;
                string resultRspCode = string.Empty;
                string resultErrorCode = string.Empty;

                #region inputXmlObject
                ESBOrderUpClass esborderup = new ESBOrderUpClass(Session);
                esborderup.CDM_C0701_CHANGECARDADDR = "1"; //是否為改卡單地址 0:否/1:是
                esborderup.CDM_C0701_PID = m_Id; //身分證字號
                esborderup.CDM_C0701_NAME = this.txtUserName.Text.Trim(); //姓名
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
                esborderup.ORIG_GETCARDTYPE_ID = !string.IsNullOrEmpty(takeWayOri) ? takeWayOri.Substring(0, 1) : ""; //原取卡方式
                esborderup.NEW_GETCARDTYPE_ID = !string.IsNullOrEmpty(takeWayNew) ? takeWayNew.Substring(0, 1) : ""; //新取卡方式
                #endregion inputXmlObject
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
                        break;
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
                    Logging.Log(DateTime.Now.ToString() + "Fail：" + resultErrorMsg, LogState.Error, LogLayer.UI);
                    Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esborderup.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Error, LogLayer.UI);
                    break;
                }
                #endregion

                if (null != dtCardDataChange && dtCardDataChange.Rows.Count > 0)
                {
                    m_Kind = dtCardDataChange.Rows[0]["NewWay"].ToString();
                }
                else
                {
                    m_Kind = "";
                }
                CardDataChanges.OldWay = m_Kind;//*取卡方式只能寫入Code
                CardDataChanges.NewWay = "6";

                string takWayNew = string.Empty;
                GetKindName("2", "6", ref takWayNew);

                string takWay = string.Empty;
                GetKindName("2", m_Kind, ref takWay);

                //CardDataChanges.CNote = this.txtCNoteC.Text.Trim();//*備註 !待確認!
                if (string.IsNullOrEmpty(this.txtCNoteA.Text.Trim())){
                    CardDataChanges.CNote = this.txtCNoteA.Text.Trim() + string.Format("類別：{0}，工單編號：{1}", this.newCardnewAccount.Checked ? "新卡新戶" : this.newCardoldAccount.Checked ? "新卡舊戶" : "", caseNo);//*備註
                }
                else
                {
                    CardDataChanges.CNote = this.txtCNoteA.Text.Trim() + string.Format(" 類別：{0}，工單編號：{1}", this.newCardnewAccount.Checked ? "新卡新戶" : this.newCardoldAccount.Checked ? "新卡舊戶" : "", caseNo);//*備註
                }
                
                CardDataChanges.NoteCaptions = MessageHelper.GetMessage("06_02020000_008", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), takWay, takWayNew);//*異動記錄說明
                CardDataChanges.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChanges.UpdTime = DateTime.Now.ToString("HH:mm");
                CardDataChanges.BaseFlg = "0";
                CardDataChanges.Type_flg = "A";
                CardDataChanges.OutputFlg = "N";
                CardDataChanges.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                blnResult = BRM_CardDataChange.Insert(CardDataChanges, ref strMsgID);
                break;
        }

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
    /// 按action,ID,cardno查找卡片基本資料檔，取得最新的轉檔日
    /// </summary>
    /// <param name="strAction"></param>
    /// <param name="strId"></param>
    /// <param name="strCardNo"></param>
    /// <returns></returns>
    protected string GetNewTranDate(string strAction, string strId, string strCardNo)
    {
        string strMsgID = string.Empty;
        DataTable dtCardInfo = new DataTable();
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, strAction);
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, strId);
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, strCardNo);

        if (BRM_TCardBaseInfo.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardInfo, ref strMsgID))
        {
            if (dtCardInfo.Rows.Count > 0)
            {
                DataRow[] drTmp = dtCardInfo.Select("", "TranDate desc");
                if (drTmp.Length > 0)
                {
                    return drTmp[0]["TranDate"].ToString();
                }
            }
        }
        return "";
    }

    ///// <summary>
    ///// 功能說明:更新備注
    ///// 作    者:HAO CHEN
    ///// 創建時間:2010/06/21
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
    //        MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_012");
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
    //        string strLogMsg = BaseHelper.GetShowText("06_02020003_000") + "：" + BaseHelper.GetShowText("06_06020003_010");
    //        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

    //        if (BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "CNote"))
    //        {
    //            MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_010");
    //            BindData();
    //        }
    //        else
    //        {
    //            MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_011");
    //        }
    //    }
    //}
    /// <summary>
    /// 功能說明:更新備注
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/21
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
            MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_012");
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
            string strLogMsg = BaseHelper.GetShowText("06_02020003_000") + "：" + BaseHelper.GetShowText("06_06020003_010");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

            if (BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "CNote"))
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_010");
                BindData();
            }
            else
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_011");
            }
        }
    }

    /// <summary>
    /// 功能說明:檢核畫面輸入欄位是否正確
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsg">返回錯誤消息</param>
    /// <returns>True:正確 False:不正確</returns>
    private bool CheckValue(ref string strMsg)
    {
        string strMsgID = string.Empty;
        if (!string.IsNullOrEmpty(txtId.Text))
        {
            if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
            {
                //*身份證字號驗證不通過
                strMsg = "06_02020000_002";
                return false;
            }
        }
        else
        {
            //*身份證字號不能為空
            strMsg = "06_02020000_006";
            return false;
        }

        if (!string.IsNullOrEmpty(txtCardNo.Text))
        {
            if (ValidateHelper.IsChinese(this.txtCardNo.Text.Trim()))
            {
                //*卡號驗證不通過
                strMsg = "06_02020000_003";
                return false;
            }
        }
        else
        {
            //*卡號不能為空
            strMsg = "06_02020000_007";
            return false;
        }
        return true;
    }

    /// <summary>
    /// 功能說明:檢核畫面輸入欄位是否正確
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/13
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsg">返回錯誤消息</param>
    /// <returns>True:正確 False:不正確</returns>
    private bool CheckModAdd(ref string strMsg)
    {
        string strMsgID = string.Empty;
        if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
        {
            //*連絡姓名不能為空
            strMsg = "06_02020000_013";
            return false;
        }
        if (string.IsNullOrEmpty(txtTel.Text.Trim()))
        {
            //*連絡電話不能為空
            strMsg = "06_02020000_014";
            return false;
        }
        return true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        //檢核畫面輸入欄位是否正確
        if (!CheckValue(ref strMsgID))
        {
            jsBuilder.RegScript(this.UpdatePanel1, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }

        //顯示卡片異動明細資料
        SearchDataChange();
    }

    /// <summary>
    /// 功能說明:通過Code獲得Name
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    /// <param name="strResultKind"></param>
    /// <param name="strKindName"></param>
    public void GetKindName(string strPropertyKey, string strCode, ref string strName)
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityLayer.EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, "06");
        sqlhelp.AddCondition(EntityLayer.EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, strPropertyKey);

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
    protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
    {
        SearchDataChange();
    }
}
