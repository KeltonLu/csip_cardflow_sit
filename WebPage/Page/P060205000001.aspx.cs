using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

public partial class P060205000001 : PageBase
{
    #region table
    public DataTable m_dtSelfPickInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }
    #endregion
    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region 事件
    protected void Page_Load(object sender, EventArgs e)
    {
        string strdpFetchDate = "";
        string strdpMerchDate = "";
        string strdpFrom = "";
        string strdpTo = "";
        if (!IsPostBack)
        {
            this.dpFetchDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.dpMerchDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.dpFrom.Text = DateTime.Now.AddDays(-15).ToString("yyyy/MM/dd");
            this.dpTo.Text = DateTime.Now.ToString("yyyy/MM/dd");
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
            this.btnInOutStore.Visible = false;
            BindFactory();
            if (Request.QueryString["RadCheckFlg0205"] != null && Request.QueryString["FetchDate0205"] != null && Request.QueryString["MerchDate0205"] != null && Request.QueryString["Id0205"] != null && Request.QueryString["CardNo0205"] != null && Request.QueryString["FromDate0205"] != null && Request.QueryString["ToDate0205"] != null && Request.QueryString["PageIndex0205"] != null)
            {
                switch (RedirectHelper.GetDecryptString(Request.QueryString["RadCheckFlg0205"].ToString().Trim()))
                {
                    case "1":
                        rad020501.Checked = true;
                        break;
                    case "2":
                        rad020502.Checked = true;
                        break;
                    case "3":
                        rad020503.Checked = true;
                        txtID.Text = RedirectHelper.GetDecryptString(Request.QueryString["Id0205"].ToString().Trim());
                        txtCardNo.Text = RedirectHelper.GetDecryptString(Request.QueryString["CardNo0205"].ToString().Trim());
                        break;
                    case "4":
                        rad020504.Checked = true;
                        txtID2.Text = RedirectHelper.GetDecryptString(Request.QueryString["Id0205"].ToString().Trim());
                        txtCardNo2.Text = RedirectHelper.GetDecryptString(Request.QueryString["CardNo0205"].ToString().Trim());
                        break;
                }
                strdpFetchDate = RedirectHelper.GetDecryptString(Request.QueryString["FetchDate0205"].ToString().Trim());
                strdpMerchDate = RedirectHelper.GetDecryptString(Request.QueryString["MerchDate0205"].ToString().Trim());
                strdpFrom = RedirectHelper.GetDecryptString(Request.QueryString["FromDate0205"].ToString().Trim());
                strdpTo = RedirectHelper.GetDecryptString(Request.QueryString["ToDate0205"].ToString().Trim());
                dpFetchDate.Text = strdpFetchDate;
                dpMerchDate.Text = strdpMerchDate;
                //txtID.Text = RedirectHelper.GetDecryptString(Request.QueryString["Id0205"].ToString().Trim());
                //txtCardNo.Text = RedirectHelper.GetDecryptString(Request.QueryString["CardNo0205"].ToString().Trim());
                dpFrom.Text = strdpFrom;
                dpTo.Text = strdpTo;
                ddlFactory.SelectedValue = RedirectHelper.GetDecryptString(Request.QueryString["Merch0205"].ToString().Trim());
                gpList.Visible = true;
                grvUserView.Visible = true;
                gpList.CurrentPageIndex = Convert.ToInt16(RedirectHelper.GetDecryptString(Request.QueryString["PageIndex0205"].ToString().Trim()));
                BindGridView(strdpFetchDate, strdpMerchDate, strdpFrom, strdpTo);
            }
            else
            {
                rad020503.Checked = true;            
            }
          
        }
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }
    protected void btnInOutStore_Click(object sender, EventArgs e)
    {
        bool bolStore = true;
        bool bolChecked = false;
        string strEorMsg="";
        try
        {
            for (int i = 0; i < this.grvUserView.Rows.Count; i++)
            {
                HtmlInputCheckBox chkEnable = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[6].FindControl("chkIntoStoreFlg");

                if (chkEnable.Checked)
                {
                    bolChecked = true;

                    string strAction = m_dtSelfPickInfo.Rows[i]["action"].ToString();
                    string strId = m_dtSelfPickInfo.Rows[i]["id"].ToString();
                    string strCardNo = m_dtSelfPickInfo.Rows[i]["cardno"].ToString();
                    string strCustname = m_dtSelfPickInfo.Rows[i]["custname"].ToString();
                    string strTrandate = m_dtSelfPickInfo.Rows[i]["trandate"].ToString();
                    string strIntoStoreDate = m_dtSelfPickInfo.Rows[i]["IntoStore_Date"].ToString();

                    string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                    string strLogMsg = BaseHelper.GetShowText("06_06020500_000") + "：" + BaseHelper.GetShowText("06_06020500_007");
                    BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");

                    if (this.rad020504.Checked)   //自取逾期改限掛
                    {
                        //先出庫 再入庫
                        //出庫
                        if (!BusinessRulesNew.BRM_CardStockInfo.OutStore(strAction, strId, strCardNo, strTrandate, strCustname, strIntoStoreDate))
                        {
                            bolStore = false;
                            strEorMsg += string.Format(MessageHelper.GetMessage("06_06020500_008"), strCardNo) + " ";
                        }

                        //入庫
                        if (!BusinessRules.BRM_CardStockInfo.IntoStore(strAction, strId, strCardNo, strTrandate, strCustname))
                        {
                            bolStore = false;
                            strEorMsg += string.Format(MessageHelper.GetMessage("06_06020500_007"), strCardNo) + " ";
                        }
                    }
                    else
                    {
                        if (strIntoStoreDate.Equals(string.Empty))
                        {
                            if (!BusinessRules.BRM_CardStockInfo.IntoStore(strAction, strId, strCardNo, strTrandate, strCustname))
                            {
                                bolStore = false;
                                strEorMsg += string.Format(MessageHelper.GetMessage("06_06020500_007"), strCardNo) + " ";
                            }
                        }
                        else
                        {
                            if (!BusinessRules.BRM_CardStockInfo.OutStore(strAction, strId, strCardNo, strTrandate, strCustname, strIntoStoreDate))
                            {
                                bolStore = false;
                                strEorMsg += string.Format(MessageHelper.GetMessage("06_06020500_008"), strCardNo) + " ";
                            }
                        }
                    }
                }

            }
            if (bolChecked)
            {
                if (bolStore)
                {
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020500_004"));
                    BindGridView();
                }
                else
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_005") + strEorMsg + "');");
                }
            }

        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020500_005"));
            return;
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            if (rad020501.Checked)
            {
                this.Session["RadCheckFlg0205"] = "1";
            }
            if (rad020502.Checked)
            {
                this.Session["RadCheckFlg0205"] = "2";
            }
            if (this.rad020503.Checked)
            {
                this.Session["RadCheckFlg0205"] = "3";
            }

            this.Session["FetchDate0205"] = this.dpFetchDate.Text.Trim();
            this.Session["MerchDate0205"] = this.dpMerchDate.Text.Trim();
            this.Session["Merch0205"] = this.ddlFactory.SelectedValue;
            this.Session["Id0205"] = this.txtID.Text.Trim();
            this.Session["CardNo0205"] = this.txtCardNo.Text.Trim();
            this.Session["FromDate0205"] = this.dpFrom.Text.Trim();
            this.Session["ToDate0205"] = this.dpTo.Text.Trim();
            this.Session["PageIndex0205"] = this.gpList.CurrentPageIndex.ToString();
            this.Session["backpage"] = "0501";

            if (this.rad020501.Checked)
            {
                if (this.dpMerchDate.Text == "")
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_001") + "');");
                    return;
                }
                else
                {
                    string strMerchDate = this.dpMerchDate.Text.Trim();
                    string strMerchDateSQL = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", strMerchDate.Replace("/", ""), -1);
                    strMerchDateSQL = strMerchDateSQL.Substring(0, 4) + "/" + strMerchDateSQL.Substring(4, 2) + "/" + strMerchDateSQL.Substring(6, 2);

                    string strMsgID = string.Empty;
                    Dictionary<string, string> param = new Dictionary<string, string>();

                    #region 查詢條件參數

                    if (strMerchDate != null && strMerchDateSQL != null)
                    {
                        param.Add("strMerchDate", strMerchDate);
                        param.Add("strMerchDateSQL", strMerchDateSQL);
                    }
                    else
                    {
                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_005") + "');");
                        return;
                    }

                    if (Session["Merch0205"].ToString().Trim() == "0")
                    {
                        param.Add("strMerch", "NULL");
                    }
                    else
                    {
                        param.Add("Merch0205", Session["Merch0205"].ToString().Trim());
                    }

                    #endregion

                    string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());


                    //產生報表
                    bool result = BR_Excel_File.CreateExcelFile_020501Report(param, ref strServerPathFile, ref strMsgID);

                    if (result)
                    {
                        FileInfo fs = new FileInfo(strServerPathFile);
                        Session["ServerFile"] = strServerPathFile;
                        Session["ClientFile"] = fs.Name;
                        string urlString = @"location.href='DownLoadFile.aspx';";
                        jsBuilder.RegScript(this.Page, urlString);
                    }
                    else
                    {
                        MessageHelper.ShowMessage(this, strMsgID);
                    }
                }

            }

            if (this.rad020502.Checked)
            {
                if (this.dpFetchDate.Text == "")
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_002") + "');");
                    return;
                }
                else
                {
                    string strFetchDate = this.dpFetchDate.Text.Trim();
                    string strFetchDateSQL1 = DateTime.Parse(strFetchDate).AddDays(-15).ToString("yyyy/MM/dd");
                    string strFetchDateSQL2 = DateTime.Parse(strFetchDate).AddDays(-14).ToString("yyyy/MM/dd");

                    string strMsgID = string.Empty;
                    Dictionary<string, string> param = new Dictionary<string, string>();

                    #region 查詢條件參數

                    if (strFetchDate != null && strFetchDateSQL1 != null && strFetchDateSQL2 != null)
                    {
                        param.Add("strFetchDate", strFetchDate);
                        param.Add("strFetchDateSQL1", strFetchDateSQL1);
                        param.Add("strFetchDateSQL2", strFetchDateSQL2);
                    }
                    else
                    {
                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');");
                        return;
                    }

                    #endregion

                    string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());


                    //產生報表
                    bool result = BR_Excel_File.CreateExcelFile_020502Report(param, ref strServerPathFile, ref strMsgID);

                    if (result)
                    {
                        FileInfo fs = new FileInfo(strServerPathFile);
                        Session["ServerFile"] = strServerPathFile;
                        Session["ClientFile"] = fs.Name;
                        string urlString = @"location.href='DownLoadFile.aspx';";
                        jsBuilder.RegScript(this.Page, urlString);
                    }
                    else
                    {
                        MessageHelper.ShowMessage(this, strMsgID);
                    }
                }

            }
            if (this.rad020503.Checked)
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_006") + "');");
                return;
            }

        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020701_001"));
            return;
        }
    }
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            //ViewState["FlgEdit"] = "TRUE";
            bool bolClose = false;
            string strModified = "1"; ///可修改

            DataTable dtCloseInfo = new DataTable();
            DataTable dtlOutClose = new DataTable();

            string strAction = m_dtSelfPickInfo.Rows[e.NewEditIndex]["action"].ToString();
            string strId = m_dtSelfPickInfo.Rows[e.NewEditIndex]["id"].ToString();
            string strCardNo = m_dtSelfPickInfo.Rows[e.NewEditIndex]["cardno"].ToString();
            string strTrandate = m_dtSelfPickInfo.Rows[e.NewEditIndex]["trandate"].ToString();
            string strIntoStoreDate = m_dtSelfPickInfo.Rows[e.NewEditIndex]["IntoStore_Date"].ToString();
            string strOutStoreDate = m_dtSelfPickInfo.Rows[e.NewEditIndex]["OutStore_Date"].ToString();

            if (BRM_CardDailyClose.GetCloseInfo(DateTime.Now.ToString("yyyy/MM/dd"), ref dtCloseInfo))
            {
                if (dtCloseInfo.Rows.Count > 0)
                {
                    bolClose = true;
                }

            }

            if (bolClose)
            {
                strModified = "0";///當日已日結

            }
            else
            {
                if (strIntoStoreDate.Equals(string.Empty))
                {
                    strModified = "0";///當日未日結、但未入庫的資料
                }
                else
                {
                    if (!strOutStoreDate.Equals(string.Empty))
                    {
                        if (BRM_CardDailyClose.GetCloseInfo(strOutStoreDate, ref dtlOutClose))
                        {
                            if (dtlOutClose.Rows.Count > 0)
                            {
                                strModified = "0";///當日未日結、但已出庫、且出庫日期已日結的已入庫資料
                            }

                        }
                    }
                }
            }

            if (rad020501.Checked)
            {
                this.Session["RadCheckFlg0205"] = "1";
            }
            if (rad020502.Checked)
            {
                this.Session["RadCheckFlg0205"] = "2";
            }
            if (this.rad020503.Checked)
            {
                this.Session["RadCheckFlg0205"] = "3";
                this.Session["Id0205"] = this.txtID.Text.Trim();
                this.Session["CardNo0205"] = this.txtCardNo.Text.Trim();
            }
            if (this.rad020504.Checked)
            {
                this.Session["RadCheckFlg0205"] = "4";
                this.Session["Id0205"] = this.txtID2.Text.Trim();
                this.Session["CardNo0205"] = this.txtCardNo2.Text.Trim();
            }
            this.Session["FetchDate0205"] = this.dpFetchDate.Text.Trim();
            this.Session["MerchDate0205"] = this.dpMerchDate.Text.Trim();
            this.Session["Merch0205"] = this.ddlFactory.SelectedValue;
            this.Session["FromDate0205"] = this.dpFrom.Text.Trim();
            this.Session["ToDate0205"] = this.dpTo.Text.Trim();
            this.Session["PageIndex0205"] = this.gpList.CurrentPageIndex.ToString();
            this.Session["backpage"] = "0501";

            //* 傳遞參數加密
            Response.Redirect("P060205000003.aspx?backpage=" + RedirectHelper.GetEncryptParam("0501") + "&action=" + RedirectHelper.GetEncryptParam(strAction) + "&id=" + RedirectHelper.GetEncryptParam(strId) + "&cardno=" + RedirectHelper.GetEncryptParam(strCardNo) + "&trandate=" + RedirectHelper.GetEncryptParam(strTrandate) + "&intostoredate=" + RedirectHelper.GetEncryptParam(strIntoStoreDate) + "&modifyflg=" + RedirectHelper.GetEncryptParam(strModified) + "", false);

        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010400_005"));
            return;
        }
    }
    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:Linda
    /// 創建時間:2010/06/22
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
    {  //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        //特別修改 只有此兩處有填值須寫入   另原語法 rad020504.checked  仍沿用 txtID txtCardNo 欄位，應予更正為 txtID2 txtCardNo2
        if (rad020503.Checked == true)
        {
            log.Customer_Id = this.txtID.Text;
            log.Account_Nbr = this.txtCardNo.Text;
            BRL_AP_LOG.Add(log);
        }
        if (rad020504.Checked == true)
        {
            log.Customer_Id = this.txtID2.Text;
            log.Account_Nbr = this.txtCardNo2.Text;
            BRL_AP_LOG.Add(log);
        }
        //------------------------------------------------------
        if (CheckData())
        {
            BindGridView();
        }
    }
    #endregion

    #region 方法
    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:Linda
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020500_009");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020500_010");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020500_011");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06020500_012");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06020500_013");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06020500_014");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06020500_015");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        this.grvUserView.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
    }
    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Linda
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    private void BindGridView(string strdpFetchDate, string strdpMerchDate, string strdpFrom, string strdpTo)
    {
        int iTotalCount = 0;
        DataTable dtSelfPickInfo = new DataTable();
        bool bolSelfPickInfo=false;

        try
        {
            if (this.rad020501.Checked)
            {
                if (!string.IsNullOrEmpty(strdpMerchDate))
                {
                    bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoMerch(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, strdpMerchDate, this.ddlFactory.SelectedValue);
                }
                else
                {
                    bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoMerch(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.dpMerchDate.Text.Trim(), this.ddlFactory.SelectedValue);
                }
            }
            if (this.rad020502.Checked)
            {
                if (!string.IsNullOrEmpty(strdpFetchDate))
                {
                    bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoFetch(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, strdpFetchDate);
                }
                else
                {
                    bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoFetch(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.dpFetchDate.Text.Trim());
                }
            }
            if (this.rad020503.Checked)
            {
                if ((!string.IsNullOrEmpty(strdpFrom)) || (!string.IsNullOrEmpty(strdpTo)))
                {
                    bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoOther(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.txtID.Text.Trim(), this.txtCardNo.Text.Trim(), strdpFrom, strdpTo);
                }
                else
                {
                    bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoOther(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.txtID.Text.Trim(), this.txtCardNo.Text.Trim(), this.dpFrom.Text.Trim(), this.dpTo.Text.Trim());
                }
            }
            if (this.rad020504.Checked)
            {
                bolSelfPickInfo = BusinessRulesNew.BRM_CardStockInfo.GetSelfPickInfoPost(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.txtID.Text.Trim(), this.txtCardNo.Text.Trim());
            }

            //* 查詢不成功
            if (!bolSelfPickInfo)
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.btnInOutStore.Visible = false;
                this.btnPrint.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtSelfPickInfo);
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtSelfPickInfo;
                this.grvUserView.DataBind();
                VisibleCheckBox(dtSelfPickInfo);
                m_dtSelfPickInfo = dtSelfPickInfo;
                if (iTotalCount > 0)
                {
                    this.btnInOutStore.Visible = true;
                }
                this.btnPrint.Visible = true;
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
    /// 功能說明:綁定GridView
    /// 作    者:Linda
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        int iTotalCount = 0;
        DataTable dtSelfPickInfo = new DataTable();
        bool bolSelfPickInfo = false;

        try
        {
            if (this.rad020501.Checked)
            {
                bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoMerch(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.dpMerchDate.Text.Trim(), this.ddlFactory.SelectedValue);
            }
            if (this.rad020502.Checked)
            {
                bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoFetch(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.dpFetchDate.Text.Trim());
            }
            if (this.rad020503.Checked)
            {
                bolSelfPickInfo = BusinessRules.BRM_CardStockInfo.GetSelfPickInfoOther(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.txtID.Text.Trim(), this.txtCardNo.Text.Trim(), this.dpFrom.Text.Trim(), this.dpTo.Text.Trim());
            }
            if (this.rad020504.Checked)
            {
                bolSelfPickInfo = BusinessRulesNew.BRM_CardStockInfo.GetSelfPickInfoPost(ref dtSelfPickInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.txtID2.Text.Trim(), this.txtCardNo2.Text.Trim());
            }

            //* 查詢不成功
            if (!bolSelfPickInfo)
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.btnInOutStore.Visible = false;
                this.btnPrint.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtSelfPickInfo);
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtSelfPickInfo;
                this.grvUserView.DataBind();
                VisibleCheckBox(dtSelfPickInfo);
                m_dtSelfPickInfo = dtSelfPickInfo;
                if (iTotalCount > 0)
                {
                    this.btnInOutStore.Visible = true;
                }
                this.btnPrint.Visible = true;
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
    /// 功能說明:MergeTable增加序號
    /// 作    者:Linda
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="dtSelfPickInfo"></param>
    public void MergeTable(ref DataTable dtSelfPickInfo)
    {
        int introw = 0;
        dtSelfPickInfo.Columns.Add("SerialNo");
        foreach (DataRow row in dtSelfPickInfo.Rows)
        {
            introw = introw + 1;
            row["SerialNo"] = introw;
        }
        
    }

    /// <summary>
    /// 功能說明:VisibleCheckBox設置複選框是否可見
    /// 作    者:Linda
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="dtSelfPickInfo"></param>
    public void VisibleCheckBox(DataTable dtSelfPickInfo)
    {
        for (int i = 0; i < this.grvUserView.Rows.Count; i++)
        {
            HtmlInputCheckBox chkEnable = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[6].FindControl("chkIntoStoreFlg");

            string strIntoStoreDate = dtSelfPickInfo.Rows[i]["IntoStore_Date"].ToString();
            string strOutStoreDate = dtSelfPickInfo.Rows[i]["OutStore_Date"].ToString();

            if (strIntoStoreDate.Equals(string.Empty))
            {
                DataTable dtCloseInfo = new DataTable();
                if (BRM_CardDailyClose.GetCloseInfo(DateTime.Now.ToString("yyyy/MM/dd"), ref dtCloseInfo))
                {
                    if (dtCloseInfo.Rows.Count > 0)
                    {
                        chkEnable.Visible = false;
                    }
                }
            }
            else
            {
                if (strOutStoreDate.Equals(string.Empty))
                {
                    DataTable dtCloseInfo = new DataTable();
                    if (BRM_CardDailyClose.GetCloseInfo(strIntoStoreDate, ref dtCloseInfo))
                    {
                        if (dtCloseInfo.Rows.Count > 0)
                        {
                            chkEnable.Visible = false;
                        }

                    }
                }
                else
                {

                    chkEnable.Visible = false;
                }
            }

            if (this.rad020504.Checked)   //自取逾期改限掛
            {
                //判斷是否已經日結
                DataTable dtCloseInfo = new DataTable();
                if (BRM_CardDailyClose.GetCloseInfo(DateTime.Now.ToString("yyyy/MM/dd"), ref dtCloseInfo))
                {
                    if (dtCloseInfo.Rows.Count > 0)
                    {
                        chkEnable.Visible = false;
                    }
                    else
                    {
                        chkEnable.Visible = true;
                    }
                }
            }

        }
    }
    /// <summary>
    /// 功能說明:編輯邏輯
    /// 作    者:Linda
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected bool CheckData()
    {
        string strMsgID = string.Empty;
        bool bolcheck=true;

        if (this.rad020501.Checked)
        {
            if (this.dpMerchDate.Text == "")
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_001") + "');");
                bolcheck=false;
            }
        }
        if (this.rad020502.Checked)
        {
            if (this.dpFetchDate.Text == "")
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_002") + "');");
                bolcheck = false;
            }
        }
        if (this.rad020503.Checked)
        {

            if (this.txtID.Text.Trim() == "" && this.txtCardNo.Text.Trim() == "" && this.dpFrom.Text.Trim() == "" && this.dpTo.Text.Trim() == "")
            {
                //*查詢條件必須輸入一項
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020100_001") + "');");
                bolcheck = false;
            }

            if (this.txtID.Text.Trim() != "")
            {
                if (ValidateHelper.IsChinese(this.txtID.Text.Trim()))
                {
                    //*身分證字號號驗證不通過
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020102_001") + "');");
                    bolcheck = false;
                }
            }
            if (this.txtCardNo.Text.Trim() != "")
            {
                if (!ValidateHelper.IsNum(this.txtCardNo.Text.Trim()))
                {
                    //*卡號驗證不通過
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_003") + "');");
                    bolcheck = false;
                }
            }
            if (this.dpFrom.Text.Trim() != "" && this.dpTo.Text.Trim() != "")
            {
                if (!ValidateHelper.IsValidDate(this.dpFrom.Text.Trim(), this.dpTo.Text.Trim(), ref strMsgID))
                {
                    //* 起迄日期驗證不通過
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                    bolcheck = false;
                }
            }
        }
        if (this.rad020504.Checked)
        {
            if (this.txtID2.Text.Trim() == "" && this.txtCardNo2.Text.Trim() == "")
            {
                //*查詢條件必須輸入一項
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020100_001") + "');");
                bolcheck = false;
            }

            if (this.txtID2.Text.Trim() != "")
            {
                if (ValidateHelper.IsChinese(this.txtID2.Text.Trim()))
                {
                    //*身分證字號號驗證不通過
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020102_001") + "');");
                    bolcheck = false;
                }
            }
            if (this.txtCardNo2.Text.Trim() != "")
            {
                if (!ValidateHelper.IsNum(this.txtCardNo2.Text.Trim()))
                {
                    //*卡號驗證不通過
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020500_003") + "');");
                    bolcheck = false;
                }
            }
        }
        return bolcheck;
    }


    /// <summary>
    /// 功能說明:加載廠商
    /// 作    者:Linda
    /// 創建時間:2010/09/15
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindFactory()
    {
        string strMsgID = string.Empty;
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtState))
        {
            this.ddlFactory.DataSource = dtState;
            this.ddlFactory.DataTextField = "PROPERTY_NAME";
            this.ddlFactory.DataValueField = "PROPERTY_CODE";
            this.ddlFactory.DataBind();
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "0");
            ddlFactory.Items.Insert(0, li);
        }
    }
    #endregion




}
