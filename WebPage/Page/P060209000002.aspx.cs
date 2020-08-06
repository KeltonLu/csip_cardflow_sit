using System;
using System.Data;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class Page_P060209000002 : System.Web.UI.Page
{
    #region table
    public DataTable m_dtDetailInfo
    {
        get { return ViewState["m_dtDetailInfo"] as DataTable; }
        set { ViewState["m_dtDetailInfo"] = value; }
    }
    #endregion

    #region 事件
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strVisible = "";
            string strDate = "";
            string strSource = "";
            string strFile = "";
            string strStatus = "";
            if (!(string.IsNullOrEmpty(RedirectHelper.GetDecryptString(Request.QueryString["date"])) || string.IsNullOrEmpty(RedirectHelper.GetDecryptString(Request.QueryString["source"])) || string.IsNullOrEmpty(RedirectHelper.GetDecryptString(Request.QueryString["file"]))))
            {
                strDate = RedirectHelper.GetDecryptString(Request.QueryString["date"].Trim());
                strSource = RedirectHelper.GetDecryptString(Request.QueryString["source"].Trim());
                strFile = RedirectHelper.GetDecryptString(Request.QueryString["file"].Trim());
                //* 傳遞參數解密
                this.lblDate.Text = strDate;
                this.hidSource.Value = strSource;
                this.lblFile.Text = strFile;

                switch (strSource)
                {
                    case "0":
                        lblSource.Text = "OU檔註銷";
                        break;
                    case "1":
                        lblSource.Text = "人工註銷";
                        break;
                    default:
                        lblSource.Text = "退件註銷";
                        break;
                }

            }
            else
            {
                MessageHelper.ShowMessageAndGoto(this.UpdatePanel1, "P060209000001.ASPX", "06_06020901_002"); 
            }

            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(Request.QueryString["visible"])))
            {
                strVisible = RedirectHelper.GetDecryptString(Request.QueryString["visible"].Trim());
                this.hidVisible.Value = strVisible;

                if (strVisible == "0")
                {
                    btnSub.Visible = false;
                }
            }
            BindSFFlg();
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(Request.QueryString["Status"])))
            {
                strStatus = RedirectHelper.GetDecryptString(Request.QueryString["Status"].Trim());
                switch (strStatus)
                {
                    case "1":
                        this.dropSFFlg.SelectedIndex = 1;
                        break;
                    case "2":
                        this.dropSFFlg.SelectedIndex = 2;
                        break;
                    case "3":
                        this.dropSFFlg.SelectedIndex = 3;
                        break;
                    case "4":
                        this.dropSFFlg.SelectedIndex = 0;
                        break;
                }
            }
            else
            {
                this.dropSFFlg.SelectedIndex = 0;
            }
            ShowControlsText();
            BindGridView();
            if (this.gpList.RecordCount == 0)
            {
                this.btnPrint.Visible = false;
            }
            else
            {
                this.btnPrint.Visible = true;
            }
        }
    }
    /// <summary>
    /// 綁定資料
    /// </summary>
    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (m_dtDetailInfo.Rows[e.Row.RowIndex]["SFFlg"].ToString() != "1")
                {
                    string strMsgID = string.Empty;
                    DataTable dtSFFlg = new DataTable();

                    CustLabel LblSFFlgName = (CustLabel)e.Row.FindControl("lblSFFlgName");
                    LblSFFlgName.Visible = false;

                    CustDropDownList DropSFFlgDetail = (CustDropDownList)e.Row.FindControl("dropSFFlgDetail");
                    DropSFFlgDetail.Visible = true;

                    if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "40", ref dtSFFlg))
                    {
                        DropSFFlgDetail.DataSource = dtSFFlg;
                        DropSFFlgDetail.DataTextField = "PROPERTY_NAME";
                        DropSFFlgDetail.DataValueField = "PROPERTY_CODE";
                        DropSFFlgDetail.DataBind();
                        DropSFFlgDetail.SelectedIndex = Convert.ToInt16(m_dtDetailInfo.Rows[e.Row.RowIndex]["SFFlg"].ToString())-2;
                    }
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
            return;
        }

    }
    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string strDateFrom0209=Session["DateFrom0209"].ToString().Trim();
        string strDateTo0209 = Session["DateTo0209"].ToString().Trim();
        string strPageIndex0209 = Session["PageIndex0209"].ToString().Trim();
        Response.Redirect("P060209000001.aspx?DateFrom0209=" + RedirectHelper.GetEncryptParam(strDateFrom0209) + " &DateTo0209=" + RedirectHelper.GetEncryptParam(strDateTo0209) + " &PageIndex0209=" + RedirectHelper.GetEncryptParam(strPageIndex0209)+"");
    }
    protected void btnSub_Click(object sender, EventArgs e)
    {
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020901_000");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");


        DataTable dtUpdateDetail = new DataTable();
        dtUpdateDetail.Columns.Add("CardNo");
        dtUpdateDetail.Columns.Add("SFFlg");
        int intScount = 0;
        int intFcount = 0;
        string strLblSFFlg = "";
        string strDropSFFlg = "";
        string strDropSFFlgCode = "";
        string strLogUserId = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strDate = lblDate.Text.Trim();
        string strFile = lblFile.Text.Trim();

        for (int i = 0; i < this.grvUserView.Rows.Count; i++)
        {
            if (m_dtDetailInfo.Rows[i]["SFFlg"].ToString() != "1")
            {
                CustLabel LblSFFlgName = (CustLabel)grvUserView.Rows[i].FindControl("lblSFFlgName");
                strLblSFFlg = LblSFFlgName.Text.Trim();
                CustDropDownList DropSFFlgDetail = (CustDropDownList)grvUserView.Rows[i].FindControl("dropSFFlgDetail");
                strDropSFFlg = DropSFFlgDetail.SelectedItem.Text.ToString().Trim();
                strDropSFFlgCode = DropSFFlgDetail.SelectedValue.ToString().Trim();

                if (strLblSFFlg != strDropSFFlg)
                {
                    DataRow row = dtUpdateDetail.NewRow();
                    row["CardNo"] = m_dtDetailInfo.Rows[i]["CardNo"].ToString().Trim();
                    row["SFFlg"] = strDropSFFlgCode;
                    dtUpdateDetail.Rows.Add(row);
                    if (strDropSFFlgCode == "3")
                    {
                        intScount++;
                    }
                    else
                    {
                        intFcount++;
                    }
                }
            }
        }

        if (BRM_CancelOASADetail.BatDetailUpdate(dtUpdateDetail, strDate, strFile, strLogUserId) || (intScount == 0 && intFcount == 0))
        {
            string strNote = ";" + DateTime.Now.ToString("yyyy/MM/dd") + "," + ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_name + "," + "記錄確認";

            if (BRM_CancelOASA.UpdateFor020902(strDate, strFile, strLogUserId, Convert.ToString(intScount), Convert.ToString(intFcount), strNote))
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020901_000"));
            }
            else
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020901_001"));
            }

        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020901_001"));
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            string strSource = this.hidSource.Value.ToString().Trim();
            string strVisible = this.hidVisible.Value.ToString().Trim();
            string strFile = this.lblFile.Text.Trim();
            string strDate = this.lblDate.Text.Trim();
            string strStatus = this.dropSFFlg.SelectedItem.Value.ToString().Trim();
            Response.Redirect("P060209000003.aspx?Source=" + RedirectHelper.GetEncryptParam(strSource) + " &Visible=" + RedirectHelper.GetEncryptParam(strVisible) + " &File=" + RedirectHelper.GetEncryptParam(strFile) + " &Date=" + RedirectHelper.GetEncryptParam(strDate) + " &Status=" + RedirectHelper.GetEncryptParam(strStatus) + "");
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020701_001"));
            return;
        }

    }
    #endregion
    #region 方法
    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020901_009");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020901_010");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020901_011");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06020901_012");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:綁定注銷狀態
    /// 作    者:Linda
    /// 創建時間:2010/07/15
    /// 修改記錄:
    /// </summary>
    public void BindSFFlg()
    {
        string strMsgID = string.Empty;
        DataTable dtSFFlg = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "39", ref dtSFFlg))
        {
            this.dropSFFlg.DataSource = dtSFFlg;
            this.dropSFFlg.DataTextField = "PROPERTY_NAME";
            this.dropSFFlg.DataValueField = "PROPERTY_CODE";
            this.dropSFFlg.DataBind();
        }
    }
    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        int iTotalCount = 0;
        DataTable dtDetailInfo = new DataTable();

        try
        {
            //* 查詢不成功
            if (!BRM_CancelOASADetail.GetDetailInfo(ref dtDetailInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.lblDate.Text.Trim(), this.lblFile.Text.Trim(), this.dropSFFlg.SelectedValue.ToString().Trim()))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.btnPrint.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
                return;
            }
            //* 查詢成功
            else
            {
                m_dtDetailInfo = dtDetailInfo;
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.btnPrint.Visible = true;
                this.grvUserView.DataSource = dtDetailInfo;
                this.grvUserView.DataBind();
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_002"));
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:取得用戶中文名稱
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/09
    /// 修改記錄:
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="dtUserList"></param>
    /// <returns></returns>
    private string GetUserName(string UserId, DataTable dtUserList)
    {
        foreach (DataRow dr in dtUserList.Rows)
        {
            if (UserId.Equals(dr["USER_ID"].ToString()))
            {
                return dr["USER_NAME"].ToString();
            }
        }
        return "";
    }
    #endregion
}