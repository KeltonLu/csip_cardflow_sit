//******************************************************************
//*  功能說明：綜合資料處理UI層
//*  作    者：Simba Liu
//*  創建日期：2010/04/19
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;

public partial class P060201000001 : PageBase
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
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

            if (Request.QueryString["searchID"] != null && 
                Request.QueryString["searchNo"] != null && 
                Request.QueryString["searchMailNo"] != null)
            {
                //* 傳遞參數解密
                this.txtId.Text = RedirectHelper.GetDecryptString(Request.QueryString["searchID"].ToString());
                this.txtNo.Text = RedirectHelper.GetDecryptString(Request.QueryString["searchNo"].ToString());
                this.txtMailNo.Text = RedirectHelper.GetDecryptString(Request.QueryString["searchMailNo"].ToString());
                BindGridView();
            }
        }
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020100_003");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020100_001");
        //新增轉檔日欄位(Wallace) begin
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020100_010");
        //新增轉檔日欄位(Wallace) begin
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06020100_004");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06020100_005");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06020100_006");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06020100_007");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
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
            if (!BusinessRulesNew.BRM_TCardBaseInfo.Search(GetFilterCondition(), ref dtCardBaseInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.grvUserView.Visible = false ;
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

        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_00000000_007"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        string strMailno = string.Empty;
        if (this.txtId.Text.Trim() != "")
        {
            sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, this.txtId.Text.Trim());
        }
        if (this.txtNo.Text.Trim() != "")
        {
            sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, this.txtNo.Text.Trim());
        }
        if (this.txtMailNo.Text.Trim() != "")
        {
            //sqlhelp.AddCondition(Entity_CardBaseInfo.M_mailno, Operator.Like, DataTypeUtils.String, this.txtMailNo.Text.Trim());
            strMailno = " and Mailno like '" + this.txtMailNo.Text.Trim() + "%' ";
        }
        return sqlhelp.GetFilterCondition() + strMailno;
    }
    /// <summary>
    /// 功能說明:編輯邏輯
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            ViewState["FlgEdit"] = "TRUE";
            //CustLinkButton link = grvUserView.Rows[e.NewEditIndex].Cells[1].FindControl("lkbDetail") as CustLinkButton;
            ////* 進入編輯頁面
            //if (link != null)
            //{
            //    //* NO加密
            //    Response.Redirect("P06020101.aspx?no=" + RedirectHelper.GetEncryptParam(link.Text.Trim()) + "", false);
            //}
            string strAction = m_dtCardBaseInfo.Rows[e.NewEditIndex]["action"].ToString();
            string strId = m_dtCardBaseInfo.Rows[e.NewEditIndex]["id"].ToString();
            string strCardNo = m_dtCardBaseInfo.Rows[e.NewEditIndex]["cardno"].ToString();
            string strTrandate = m_dtCardBaseInfo.Rows[e.NewEditIndex]["trandate"].ToString();
            //* 傳遞參數加密
            Response.Redirect("P060201000002.aspx?action=" + RedirectHelper.GetEncryptParam(strAction) + " &id=" + RedirectHelper.GetEncryptParam(strId) + "&cardno=" + RedirectHelper.GetEncryptParam(strCardNo) + "&trandate=" + RedirectHelper.GetEncryptParam(strTrandate) + "&searchID=" + RedirectHelper.GetEncryptParam(txtId.Text) + "&searchNo="+RedirectHelper.GetEncryptParam(txtNo.Text) + "&searchMailNo="+RedirectHelper.GetEncryptParam(txtMailNo.Text) + "", false);
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010400_005"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:2020/12/17_Ares_Stanley-調整AP_LOG紀錄順序, 避免錯誤的查詢條件導致寫入AP_LOG失敗, 增加查詢條件錯誤的LOG
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        if (this.txtId.Text.Trim() != "")
        {
            if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
            {
                //*身份證字號驗證不通過
                MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_025");
                string errorMsg = string.Format("綜合資料處理查詢輸入異常：員編:{0}, 卡號: {1}, 身分證字號: {2}, 掛號號碼前6碼:{3}", this.eAgentInfo.agent_id, this.txtNo.Text, this.txtId.Text, this.txtMailNo.Text );
                Logging.Log(errorMsg, LogState.Info, LogLayer.None);
                return;
            }
        }
        if (this.txtNo.Text.Trim() != "")
        {
            if (ValidateHelper.IsChinese(this.txtNo.Text.Trim()))
            {
                //*卡號驗證不通過
                MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_013");
                string errorMsg = string.Format("綜合資料處理查詢輸入異常：員編:{0}, 卡號: {1}, 身分證字號: {2}, 掛號號碼前6碼:{3}", this.eAgentInfo.agent_id, this.txtNo.Text, this.txtId.Text, this.txtMailNo.Text);
                Logging.Log(errorMsg, LogState.Info, LogLayer.None);
                return;
            }
        }
        if (this.txtMailNo.Text.Trim() != "")
        {
            if (this.txtMailNo.Text.Trim().Length<6)
            {
                //*掛號號碼小於六碼
                MessageHelper.ShowMessage(UpdatePanel1, "06_06020100_000");
                string errorMsg = string.Format("綜合資料處理查詢輸入異常：員編:{0}, 卡號: {1}, 身分證字號: {2}, 掛號號碼前6碼:{3}", this.eAgentInfo.agent_id, this.txtNo.Text, this.txtId.Text, this.txtMailNo.Text);
                Logging.Log(errorMsg, LogState.Info, LogLayer.None);
                return;
            }
        }
        if (this.txtId.Text.Trim() == "" && this.txtNo.Text.Trim() == "" && this.txtMailNo.Text.Trim() == "")
        {
            //*查詢條件必須輸入一項
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020100_001");
            return;
        }
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtId.Text;
        log.Account_Nbr = this.txtNo.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        BindGridView();
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
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
    /// 作    者:Simba Liu
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
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1",ref dtCardtype))
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
    /// <summary>
    /// 功能說明:新增事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060201000003.aspx");
    }

    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //新增轉檔日欄位(Wallace) begin
//            string strTmp = e.Row.Cells[3].Text + "      ";
//            e.Row.Cells[3].Text = strTmp.Substring(0, 6).Trim();
              string strTmp = e.Row.Cells[4].Text + "      ";
              e.Row.Cells[4].Text = strTmp.Substring(0, 6).Trim();
            //新增轉檔日欄位(Wallace) end
        }
    }
}
