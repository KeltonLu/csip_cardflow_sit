//******************************************************************
//*  功能說明：異動作業單新增畫面
//*  作    者：HAO CHEN
//*  創建日期：2010/06/17
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

public partial class P060202000001_NewWorkOrder : PageBase
{
    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

    public DataTable m_dtCardBaseInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }

    /// <summary>
    /// 功能說明:頁面加載事件
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            //*取消數據加載
            //BindGridView();
            this.grvUserView.Visible = false;
            ViewState["FlgEdit"] = "FALSE";
            this.Page.Title = BaseHelper.GetShowText("06_02020000_011");

            if (null != Session["0202_txtNo"] && !string.IsNullOrEmpty(Session["0202_txtNo"].ToString()))
            {
                txtNo.Text = Session["0202_txtNo"].ToString().Trim();
            }


            if (null != Session["0202_txtId"] && !string.IsNullOrEmpty(Session["0202_txtId"].ToString()))
            {
                txtId.Text = Session["0202_txtId"].ToString().Trim();
            }

            if (!string.IsNullOrEmpty(txtId.Text) || !string.IsNullOrEmpty(txtNo.Text))
            {
                BindGridView();
            }
        }
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_02020000_005");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_02020000_002");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_02020000_006");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_02020000_007");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_02020000_008");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_02020000_009");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_02020000_010");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int iTotalCount = 0;
        DataTable dtCardBaseInfo = new DataTable();
        try
        {
            //* 查詢不成功
            if (!BRM_CardDataChange.Search(GetFilterCondition(), ref dtCardBaseInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.grvUserView.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtCardBaseInfo);
                m_dtCardBaseInfo = dtCardBaseInfo;
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtCardBaseInfo;
                this.grvUserView.DataBind();
            }

            Session.Remove("0202_txtNo");
            Session.Remove("0202_txtId");
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_02020000_001"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        if (null != Session["0202_txtNo"] && !string.IsNullOrEmpty(Session["0202_txtNo"].ToString()))
        {
            txtNo.Text = Session["0202_txtNo"].ToString().Trim();
        }


        if (null != Session["0202_txtId"] && !string.IsNullOrEmpty(Session["0202_txtId"].ToString()))
        {
            txtId.Text = Session["0202_txtId"].ToString().Trim();
        }


        SqlHelper sqlhelp = new SqlHelper();
        if (this.txtId.Text.Trim() != "")
        {
            sqlhelp.AddCondition("T1." + Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, this.txtId.Text.Trim());
        }
        if (this.txtNo.Text.Trim() != "")
        {
            sqlhelp.AddCondition("T1." + Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, this.txtNo.Text.Trim());
        }
        return sqlhelp.GetFilterCondition();
    }

    /// <summary>
    /// 功能說明:編輯邏輯
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            ViewState["FlgEdit"] = "TRUE";
            HiddenField hidvalue = grvUserView.Rows[e.NewEditIndex].FindControl("hidType") as HiddenField;
            string strAction =  RedirectHelper.GetEncryptParam(m_dtCardBaseInfo.Rows[e.NewEditIndex]["action"].ToString());
            string strId = RedirectHelper.GetEncryptParam(m_dtCardBaseInfo.Rows[e.NewEditIndex]["id"].ToString());
            string strCardNo = RedirectHelper.GetEncryptParam(m_dtCardBaseInfo.Rows[e.NewEditIndex]["cardno"].ToString());
            string strTrandate = string.Empty;
            if (null != m_dtCardBaseInfo && m_dtCardBaseInfo.Rows.Count > 0 && null != m_dtCardBaseInfo.Rows[e.NewEditIndex]["trandate"])
            {
                //hidTrandate
                strTrandate = RedirectHelper.GetEncryptParam(m_dtCardBaseInfo.Rows[e.NewEditIndex]["trandate"].ToString());
            }
            else
            {
                strTrandate = "";
            }

            string strCommand = hidvalue.Value;
            //* A 進入強迫新增畫面
            if (null != hidvalue && hidvalue.Value.Equals("A"))
            {

                if (!string.IsNullOrEmpty(txtId.Text))
                {
                    Session["0202_txtId"] = txtId.Text.Trim();
                }

                if (!string.IsNullOrEmpty(txtNo.Text))
                {
                    Session["0202_txtNo"] = txtNo.Text.Trim();
                }
                Response.Redirect("P060202000003_NewWorkOrder.aspx?action=" + strAction + " &id=" + strId + "&cardno=" + strCardNo + "&trandate=" + strTrandate + "", false);
            }
            else
            {
                if (!string.IsNullOrEmpty(txtId.Text))
                {
                    Session["0202_txtId"] = txtId.Text.Trim();
                }

                if (!string.IsNullOrEmpty(txtNo.Text))
                {
                    Session["0202_txtNo"] = txtNo.Text.Trim();
                }
                //* 進入非強迫新增畫面
                Response.Redirect("P060202000002_NewWorkOrder.aspx?action=" + strAction + " &id=" + strId + "&cardno=" + strCardNo + "&trandate=" + strTrandate + "", false);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            return;
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {

        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtId.Text;
        log.Account_Nbr = this.txtNo.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string strMsgID = string.Empty;
        if (this.txtId.Text.Trim() != "")
        {
            if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
            {
                //*身份證字號驗證不通過
                MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_002");
                return;
            }
        }
        if (this.txtNo.Text.Trim() != "")
        {
            if (ValidateHelper.IsChinese(this.txtNo.Text.Trim()))
            {
                //*卡號驗證不通過
                MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_003");
                return;
            }
        }

        if (this.txtId.Text.Trim() == "" && this.txtNo.Text.Trim() == "" )
        {
            //*查詢條件必須輸入一項
            MessageHelper.ShowMessage(UpdatePanel1, "06_02020000_000");
            return;
        }
        BindGridView();
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    /// <summary>
    /// 功能說明:MergeTable加載卡別
    /// 作    者:HAO CHEN
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtCard)
    {
        string strMsgID = string.Empty;
        string strAffinity = string.Empty;
        dtCard.Columns.Add("Cardtypes");
        dtCard.Columns.Add("Photos");
        dtCard.Columns.Add("Affinitys");
        //*卡別(Action)
        DataTable dtCardtype = new DataTable();

        //*PHOTO TYPE
        DataTable dtPhoto = new DataTable();

        //*認同代碼
        DataTable dtAffinity = new DataTable();


        foreach (DataRow row in dtCard.Rows)
        {
            strAffinity = string.Empty;
            //*卡別顯示Code+Name
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtCardtype))
            {
                DataRow[] rowCardtype = dtCardtype.Select("PROPERTY_CODE='" + row["action"].ToString() + "'");
                if (rowCardtype != null && rowCardtype.Length > 0)
                {
                    //row["Cardtypes"] = rowCardtype[0]["PROPERTY_CODE"].ToString() + " " + rowCardtype[0]["PROPERTY_NAME"].ToString();
                    row["Cardtypes"] = rowCardtype[0]["PROPERTY_NAME"].ToString();
                }
                else
                {
                    row["Cardtypes"] = row["action"].ToString();
                }
            }
            //*Photo顯示Code+Name
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "3", ref dtPhoto))
            {
                DataRow[] rowPhoto = dtPhoto.Select("PROPERTY_CODE='" + row["Photo"].ToString() + "'");
                if (rowPhoto != null && rowPhoto.Length > 0)
                {
                    row["Photos"] = rowPhoto[0]["PROPERTY_CODE"].ToString() + " " + rowPhoto[0]["PROPERTY_NAME"].ToString();
                }
                else
                {
                    row["Photos"] = row["Photo"].ToString();
                }
            }
            //認同代碼顯示Code+Name
            if (null != row["Affinity"] && !string.IsNullOrEmpty(row["Affinity"].ToString()))
            {
                GetAffName(row["Affinity"].ToString(), ref strAffinity);
                row["Affinitys"] = strAffinity;
            }
            else
            {
                row["Affinitys"] = row["Affinity"].ToString();
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

    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hidKind = e.Row.Cells[1].FindControl("hidKind") as HiddenField;
            HiddenField hidUrgencyFlg = e.Row.Cells[1].FindControl("hidUrgencyFlg") as HiddenField;
            LinkButton culBtn = e.Row.Cells[1].FindControl("lkbDetail") as CustLinkButton;
            if (null != e.Row.Cells[2] && !string.IsNullOrEmpty(e.Row.Cells[2].Text) && !e.Row.Cells[2].Text.Equals("&nbsp;"))
            {
                string strKind = string.Empty;
                if (!string.IsNullOrEmpty(hidKind.Value))
                {
                    if (hidKind.Value.Trim().Equals("5"))
                    {
                        culBtn.Enabled = false;
                    }
                }

                //制卡日
                DateTime dtIndate = Convert.ToDateTime(e.Row.Cells[2].Text.Trim());
                //當天
                DateTime dtNow = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));
                TimeSpan ts = dtIndate - dtNow;
                //緊急製卡註記1：緊急  0 ：普通
                string strUrgency = string.Empty;
                //製卡日 小時：分鐘
                string strIndate = UtilHelper.GetAppSettings("Indate");
                if (!string.IsNullOrEmpty(hidUrgencyFlg.Value))
                {
                    if (hidUrgencyFlg.Value.Trim().Equals("1"))
                    {
                        //依據國俊的要求只要是緊急製卡，都不要讓客服修改資料
                        culBtn.Enabled = false;
                        //if (ts.Days < 0)
                        //{
                        //    culBtn.Enabled = false;
                        //}
                        //if (ts.Days == 0)
                        //{
                        //    int iHour = int.Parse(strIndate.Substring(0, 2));
                        //    int iMinute = int.Parse(strIndate.Substring(3, 2));
                        //    if (DateTime.Now.Hour == iHour)
                        //    {
                        //        if (DateTime.Now.Minute > iMinute)
                        //        {
                        //            culBtn.Enabled = false;
                        //        }
                        //    }

                        //    if (DateTime.Now.Hour > iHour)
                        //    {
                        //        culBtn.Enabled = false;
                        //    }
                        //}
                    }
                }
                //如果是新卡且取卡方式是保留，就不能修改
                if (null != e.Row.Cells[0] && !string.IsNullOrEmpty(e.Row.Cells[0].Text) && e.Row.Cells[0].Text.Equals("新卡") && hidKind.Value.Trim().Equals("6") )
                {
                    culBtn.Enabled = false;
                }
                //製卡為當天且超過當天的16:35  且為自取件，不能再異動了
                if (dtNow.ToString("yyyyMMdd") == dtIndate.ToString("yyyyMMdd") && hidKind.Value.Trim().Equals("1"))
                {

                          int iHour = int.Parse("16");
                        int iMinute = int.Parse("35");
                        if (DateTime.Now.Hour == iHour)
                        {
                            if (DateTime.Now.Minute > iMinute)
                            {
                                culBtn.Enabled = false;
                            }
                        }

                        if (DateTime.Now.Hour > iHour)
                        {
                            culBtn.Enabled = false;
                        }
                }
                if (dtNow> dtIndate & hidKind.Value.Trim().Equals("1"))
                {
                   culBtn.Enabled = false;
                }

                //製卡日下一個營業日

                DateTime dtNextWorkDay = Convert.ToDateTime(DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", dtIndate.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString("yyyy/MM/dd"));              
                ts = dtNextWorkDay - dtNow;
                if (ts.Days < 0)
                {
                    culBtn.Enabled = false;
                }
                if (ts.Days == 0)
                {
                    int iHour = int.Parse(strIndate.Substring(0, 2));
                    int iMinute = int.Parse(strIndate.Substring(3, 2));
                    if (DateTime.Now.Hour == iHour)
                    {
                        if (DateTime.Now.Minute > iMinute)
                        {
                            culBtn.Enabled = false;
                        }
                    }
                    
                    if ( DateTime.Now.Hour > iHour )
                    {
                        culBtn.Enabled = false;
                    }
                }

            }

            if (!culBtn.Enabled) {
                culBtn.ForeColor = System.Drawing.Color.Gray;
            }
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(txtId.Text))
        {
            Session["0202_txtId"] = txtId.Text.Trim();
        }

        if (!string.IsNullOrEmpty(txtNo.Text))
        {
            Session["0202_txtNo"] = txtNo.Text.Trim();
        }

        Response.Redirect("P060202000003_NewWorkOrder.aspx");
    }
}
