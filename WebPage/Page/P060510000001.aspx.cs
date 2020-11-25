//******************************************************************
//*  功能說明：扣卡明細查詢
//*  作    者：JUN HU
//*  創建日期：2010/06/23
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using CSIPCommonModel.BusinessRules;

public partial class P060510000001 : PageBase
{
    #region 成員
    public DataTable m_dtCardBaseInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }
    #endregion

    #region 事件
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            grvCardView.Visible = false;
            ShowControlsText();
            gpList.Visible = false;
            gpList.RecordCount = 0;
            this.btnPrint.Enabled = false;
            BindFactory();

            if (Request.QueryString["Indate1FromDate0510"] != null && Request.QueryString["Indate1ToDate0510"] != null && Request.QueryString["PageIndex0510"] != null)
            {
                string strIndate1FromDate0510 = RedirectHelper.GetDecryptString(Request.QueryString["Indate1FromDate0510"].ToString().Trim());
                string strIndate1ToDate0510 = RedirectHelper.GetDecryptString(Request.QueryString["Indate1ToDate0510"].ToString().Trim());
                string strFromDate0510 = RedirectHelper.GetDecryptString(Request.QueryString["FromDate0510"].ToString().Trim());
                string strToDate0510 = RedirectHelper.GetDecryptString(Request.QueryString["ToDate0510"].ToString().Trim());
                this.ddlFactory.SelectedValue = RedirectHelper.GetDecryptString(Request.QueryString["Factory0510"].ToString().Trim());

                this.txtBackdateStart.Text = strIndate1FromDate0510;
                this.txtBackdateEnd.Text = strIndate1ToDate0510;
                this.txtClosedateStart.Text=strFromDate0510;
                this.txtClosedateEnd.Text = strToDate0510;
                gpList.CurrentPageIndex = Convert.ToInt16(RedirectHelper.GetDecryptString(Request.QueryString["PageIndex0510"].ToString().Trim()));
                BindGridView(strIndate1FromDate0510, strIndate1ToDate0510, strFromDate0510, strToDate0510);
            }
        }
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
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
    /// 功能說明:查詢按鈕單擊事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }
        BindGridView();
    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        string strStart = txtBackdateStart.Text.Trim();
        string strEnd = txtBackdateEnd.Text.Trim();


        if (string.IsNullOrEmpty(strStart))
        {
            strMsgID = "06_06051000_002";
            txtBackdateStart.Focus();
            return false;
        }

        if (string.IsNullOrEmpty(strEnd))
        {
            strMsgID = "06_06051000_003";
            txtBackdateEnd.Focus();
            return false;
        }

        //*製卡日期
        if (!ValidateHelper.IsValidDate(strStart, strEnd, ref strMsgID))
        {
            strMsgID = "06_06051000_004";
            txtBackdateStart.Focus();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 功能說明:Linkbutton單擊事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbcardno_Click(object sender, EventArgs e)
    {
        try
        {
            CustLinkButton lkbtn = (CustLinkButton)sender;
            int rowIndex = int.Parse(lkbtn.CommandArgument);
            string strAction = m_dtCardBaseInfo.Rows[rowIndex]["action"].ToString();
            string strId = m_dtCardBaseInfo.Rows[rowIndex]["id"].ToString();
            string strCardNo = m_dtCardBaseInfo.Rows[rowIndex]["cardno"].ToString();
            string strTrandate = m_dtCardBaseInfo.Rows[rowIndex]["trandate"].ToString();

            this.Session["Indate1FromDate0510"] = this.txtBackdateStart.Text.Trim();
            this.Session["Indate1ToDate0510"] = this.txtBackdateEnd.Text.Trim();
            this.Session["FromDate0510"] = this.txtClosedateStart.Text.Trim();
            this.Session["ToDate0510"] = this.txtClosedateEnd.Text.Trim();
            this.Session["PageIndex0510"] = this.gpList.CurrentPageIndex.ToString();
            this.Session["Factory0510"] = this.ddlFactory.SelectedValue;

            //* 傳遞參數加密
            Response.Redirect("P060201000002.aspx?flag=View&action=" + RedirectHelper.GetEncryptParam(strAction) + " &id=" + RedirectHelper.GetEncryptParam(strId) + "&cardno=" + RedirectHelper.GetEncryptParam(strCardNo) + "&trandate=" + RedirectHelper.GetEncryptParam(strTrandate) + "", false);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
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
    protected void grvCardView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton lkbutton = e.Row.Cells[2].FindControl("lbcardno") as CustLinkButton;
            lkbutton.CommandArgument = e.Row.RowIndex.ToString();
        }
    }

    /// <summary>
    /// 功能說明:列印按钮单击事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:2020/11/24_Ares_Stanley-移除產生報表時畫面重新導向，於原頁面產生報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        this.Session["Indate1FromDate0510"] = this.txtBackdateStart.Text.Trim();
        this.Session["Indate1ToDate0510"] = this.txtBackdateEnd.Text.Trim();
        this.Session["FromDate0510"] = this.txtClosedateStart.Text.Trim();
        this.Session["ToDate0510"] = this.txtClosedateEnd.Text.Trim();
        this.Session["PageIndex0510"] = this.gpList.CurrentPageIndex.ToString();
        this.Session["Factory0510"] = this.ddlFactory.SelectedValue;

        this.GetPrintData();

        //* 傳遞參數加密
        //Response.Redirect("P060510000002.aspx?indatefrom=" + RedirectHelper.GetEncryptParam(txtBackdateStart.Text.Trim()) + " &indateto=" + RedirectHelper.GetEncryptParam(txtBackdateEnd.Text.Trim()) + "&maildate=" + RedirectHelper.GetEncryptParam(txtClosedateStart.Text.Trim()) + "&maildate1=" + RedirectHelper.GetEncryptParam(txtClosedateEnd.Text.Trim()) + "", false);
        string strMsgId = string.Empty;

        try
        {
            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            // 製卡日期
            String inDateFrom = txtBackdateStart.Text.Trim().Equals("") ? "NULL" : txtBackdateStart.Text.Trim();
            param.Add("indatefrom", inDateFrom);
            String inDateTo = txtBackdateEnd.Text.Trim().Equals("") ? "NULL" : txtBackdateEnd.Text.Trim();
            param.Add("indateto", inDateTo);

            // 扣卡日期
            string maildate = txtClosedateStart.Text.Trim().Equals("") ? "NULL" : txtClosedateStart.Text.Trim();
            param.Add("maildate", maildate);
            string maildate1 = txtClosedateEnd.Text.Trim().Equals("") ? "NULL" : txtClosedateEnd.Text.Trim();
            param.Add("maildate1", maildate1);

            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0510Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, strMsgId);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_001"));
            return;
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 功能說明:標題列印
    /// 作    者：JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvCardView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051000_004");
        this.grvCardView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051000_005");
        this.grvCardView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051000_006");
        this.grvCardView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051000_007");
        this.grvCardView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051000_008");
        this.grvCardView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051000_009");
        this.grvCardView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051000_010");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvCardView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
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
            if (!BRM_Report.SearchHoldCard(GetFilterCondition(), ref dtCardBaseInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvCardView.DataSource = null;
                this.grvCardView.DataBind();
                this.gpList.Visible = false;
                this.grvCardView.Visible = false;
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
                if (iTotalCount > 0)
                {
                    this.btnPrint.Enabled = true;
                }
                else
                {
                    this.btnPrint.Enabled = false;
                }
                this.grvCardView.Visible = true;
                this.grvCardView.DataSource = dtCardBaseInfo;
                this.grvCardView.DataBind();
            }
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_000"));
            return;
        }
    }


    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Linda
    /// 創建時間:2010/09/08
    /// 修改記錄:
    /// </summary>
    private void BindGridView(string strIndate1FromDate, string strIndate1ToDate, string strFromDate, string strToDate)
    {
        string strMsgID = "";
        int iTotalCount = 0;
        DataTable dtCardBaseInfo = new DataTable();
        try
        {
            //* 查詢不成功
            if (!BRM_Report.SearchHoldCard(GetFilterCondition(strIndate1FromDate,strIndate1ToDate,strFromDate,strToDate), ref dtCardBaseInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvCardView.DataSource = null;
                this.grvCardView.DataBind();
                this.gpList.Visible = false;
                this.grvCardView.Visible = false;
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
                if (iTotalCount > 0)
                {
                    this.btnPrint.Enabled = true;
                }
                else
                {
                    this.btnPrint.Enabled = false;
                }
                this.grvCardView.Visible = true;
                this.grvCardView.DataSource = dtCardBaseInfo;
                this.grvCardView.DataBind();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_000"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_kind, Operator.Equal, DataTypeUtils.String, "6");
        
        if (this.txtBackdateStart.Text.Trim() != "" && this.txtBackdateEnd.Text.Trim() == "")
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtBackdateStart.Text.Trim());
        }

        if (this.txtBackdateStart.Text.Trim() == "" && this.txtBackdateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.LessThanEqual, DataTypeUtils.String, this.txtBackdateEnd.Text.Trim());
        }

        if (this.txtBackdateStart.Text.Trim() != "" && this.txtBackdateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtBackdateStart.Text.Trim());
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.LessThanEqual, DataTypeUtils.String, this.txtBackdateEnd.Text.Trim());
        }

        if (this.txtClosedateStart.Text.Trim() != "" && this.txtClosedateEnd.Text.Trim() == "")
        {
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.Equal, DataTypeUtils.String, this.txtClosedateStart.Text.Trim());
        }


        if (this.txtClosedateStart.Text.Trim() == "" && this.txtClosedateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.LessThanEqual, DataTypeUtils.String, this.txtClosedateEnd.Text.Trim());
        }

        if (this.txtClosedateStart.Text.Trim() != "" && this.txtClosedateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtClosedateStart.Text.Trim());
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.LessThanEqual, DataTypeUtils.String, this.txtClosedateEnd.Text.Trim());
        }

        if (ddlFactory.SelectedIndex != 0)
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_Merch_Code, Operator.Equal, DataTypeUtils.String, ddlFactory.SelectedValue);
        }

        return sqlhelp.GetFilterCondition();
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:Linda
    /// 創建時間:2010/09/08
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition(string strIndate1FromDate, string strIndate1ToDate, string strFromDate, string strToDate)
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_kind, Operator.Equal, DataTypeUtils.String, "6");

        if (strIndate1FromDate.Trim() != "" && strIndate1ToDate.Trim() == "")
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.GreaterThanEqual, DataTypeUtils.String, strIndate1FromDate.Trim());
        }

        if (strIndate1FromDate.Trim() == "" && strIndate1ToDate.Trim() != "")
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.LessThanEqual, DataTypeUtils.String, strIndate1ToDate.Trim());
        }

        if (strIndate1FromDate.Trim() != "" && strIndate1ToDate.Trim() != "")
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.GreaterThanEqual, DataTypeUtils.String, strIndate1FromDate.Trim());
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.LessThanEqual, DataTypeUtils.String, strIndate1ToDate.Trim());
        }

        if (!string.IsNullOrEmpty(strFromDate) && string.IsNullOrEmpty(strToDate))
        {
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.Equal, DataTypeUtils.String, strFromDate.Trim());
        }


        if (string.IsNullOrEmpty(strFromDate)&& !string.IsNullOrEmpty(strToDate))
        {
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.LessThanEqual, DataTypeUtils.String, strToDate.Trim());
        }

        if (!string.IsNullOrEmpty(strFromDate) && !string.IsNullOrEmpty(strToDate))
        {
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.GreaterThanEqual, DataTypeUtils.String, strFromDate.Trim());
            sqlhelp.AddCondition("c." + Entity_CardDataChange.M_UpdDate, Operator.LessThanEqual, DataTypeUtils.String, strToDate.Trim());
        }
 
        if (ddlFactory.SelectedIndex != 0)
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_Merch_Code, Operator.Equal, DataTypeUtils.String, ddlFactory.SelectedValue);
        }

        return sqlhelp.GetFilterCondition();
    }
    #endregion

    #region 加載扣卡天數
    /// <summary>
    /// 功能說明:MergeTable加載扣卡天數
    /// 作    者:linda
    /// 創建時間:2010/09/14
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtCardBaseInfo)
    {
        try
        {
            dtCardBaseInfo.Columns.Add("kktime");

            foreach (DataRow row in dtCardBaseInfo.Rows)
            {
                if (!string.IsNullOrEmpty(row["UpdDate"].ToString()))
                {
                    row["kktime"] = Convert.ToInt32(BRWORK_DATE.QueryByDate("06", row["UpdDate"].ToString().Trim().Replace("/", ""), DateTime.Now.ToString("yyyyMMdd"))) - 1;
                }
                else
                {
                    row["kktime"] = "0";
                }
               
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
        }
    }
    #endregion

    #region 獲取打印資料
    /// <summary>
    /// 功能說明:將打印資料寫入表tbl_HoldCard
    /// 作    者:linda
    /// 創建時間:2010/09/14
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void GetPrintData()
    {
       DataTable dtHoldCard = new DataTable();
        try
        {
            if(BRM_Report.SearchHoldCard(GetFilterCondition(), ref dtHoldCard))
            {
                if (dtHoldCard.Rows.Count > 0)
                {
                    dtHoldCard.Columns.Add("kktime");

                    if (BRM_Report.ClearHoldCard())
                    {
                        foreach (DataRow row in dtHoldCard.Rows)
                        {
                            if (!string.IsNullOrEmpty(row["UpdDate"].ToString()))
                            {
                                row["kktime"] = Convert.ToInt32(BRWORK_DATE.QueryByDate("06", row["UpdDate"].ToString().Trim().Replace("/", ""), DateTime.Now.ToString("yyyyMMdd"))) - 1;
                            }
                            else
                            {
                                row["kktime"] = "0";
                            }
                            BRM_Report.InsetHoldCard(row["Id"].ToString().Trim(), row["Custname"].ToString().Trim(), row["Cardno"].ToString().Trim(), row["Indate1"].ToString().Trim(), row["UpdDate"].ToString().Trim(), row["CNote"].ToString().Trim(), row["kktime"].ToString().Trim());
                        }
                    }
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
        }
    }
    #endregion

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

}
