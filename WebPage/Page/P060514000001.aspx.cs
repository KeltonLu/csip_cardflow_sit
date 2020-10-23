using System;
using System.Data;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using EntityLayer;
using Framework.Data.OM;
using BusinessRules;
using System.Collections.Generic;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.JavaScript;
using Framework.WebControls;

public partial class P060514000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        for (int i = 0; i < ddlStatus.Items.Count; i++)
            CreateTable(ddlStatus.Items[i].Value);

        if (!IsPostBack)
        {
            BindControl();
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
            Label1.Visible = false;
            for (int i = 0; i < UpdatePanel2.ContentTemplateContainer.Controls.Count; i++)
                UpdatePanel2.ContentTemplateContainer.Controls[i].Visible = false;
            for (int i = 0; i < UpdatePanel3.ContentTemplateContainer.Controls.Count; i++)
                UpdatePanel3.ContentTemplateContainer.Controls[i].Visible = false;
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051400_010");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051400_011");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051400_012");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051400_013");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051400_014");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051400_015");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051400_016");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    private void BindGridView()
    {
        // 初始化報表參數
        Dictionary<String, String> param = new Dictionary<String, String>();
        if (chkCond(ref param))
        {
            try
            {
                if (rbCount.Checked)
                {
                    Label1.Visible = false;
                    for (int i = 0; i < UpdatePanel2.ContentTemplateContainer.Controls.Count; i++)
                    {
                        if(i % 2 != 0)
                        {
                            ((CustGridView)UpdatePanel2.ContentTemplateContainer.Controls[i]).DataSource = null;
                            UpdatePanel2.ContentTemplateContainer.Controls[i].DataBind();
                        }
                        UpdatePanel2.ContentTemplateContainer.Controls[i].Visible = false;
                    }
                    for (int i = 0; i < UpdatePanel3.ContentTemplateContainer.Controls.Count; i++)
                    {
                        if (i % 2 != 0)
                        {
                            ((CustGridView)UpdatePanel3.ContentTemplateContainer.Controls[i]).DataSource = null;
                            UpdatePanel3.ContentTemplateContainer.Controls[i].DataBind();
                        }
                        UpdatePanel3.ContentTemplateContainer.Controls[i].Visible = false;
                    }

                    DataTable dt = new DataTable();
                    Int32 count = 0;
                    Boolean result = BR_Excel_File.GetDataTable0514(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                    //* 查詢成功
                    if (result)
                    {
                        this.gpList.Visible = true;
                        this.gpList.RecordCount = count;
                        this.grvUserView.Visible = true;
                        this.grvUserView.DataSource = dt;
                        this.grvUserView.DataBind();
                        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_001"));
                    }
                    //* 查詢不成功
                    else
                    {
                        this.gpList.RecordCount = 0;
                        this.grvUserView.DataSource = null;
                        this.grvUserView.DataBind();
                        this.gpList.Visible = false;
                        this.grvUserView.Visible = false;
                        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_002"));
                    }
                }
                else if (rbResult.Checked)
                {
                    if (ddlType.SelectedValue.Equals("1"))
                    {
                        this.gpList.RecordCount = 0;
                        this.grvUserView.DataSource = null;
                        this.grvUserView.DataBind();
                        this.gpList.Visible = false;
                        this.grvUserView.Visible = false;
                        Label1.Visible = false;
                        for (int i = 0; i < UpdatePanel2.ContentTemplateContainer.Controls.Count; i++)
                        {
                            if (i % 2 != 0)
                            {
                                ((CustGridView)UpdatePanel2.ContentTemplateContainer.Controls[i]).DataSource = null;
                                UpdatePanel2.ContentTemplateContainer.Controls[i].DataBind();
                            }
                            UpdatePanel2.ContentTemplateContainer.Controls[i].Visible = false;
                        }
                        for (int i = 0; i < UpdatePanel3.ContentTemplateContainer.Controls.Count; i++)
                        {
                            if (i % 2 != 0)
                            {
                                ((CustGridView)UpdatePanel3.ContentTemplateContainer.Controls[i]).DataSource = null;
                                UpdatePanel3.ContentTemplateContainer.Controls[i].DataBind();
                            }
                            UpdatePanel3.ContentTemplateContainer.Controls[i].Visible = false;
                        }

                        if (ddlStatus.SelectedValue.Equals("0"))
                        {
                            String strMsgId = "";
                            List<DataTable> list = new List<DataTable>();
                            List<String> name = new List<String>();
                            Boolean result = BR_Excel_File.GetDataTable05140(param, ref list, ref name, ref strMsgId);
                            //* 查詢成功
                            if (result)
                            {
                                Int32 count = 0;
                                for (int i = 0; i < list.Count && i < name.Count && i * 2 < UpdatePanel2.ContentTemplateContainer.Controls.Count; i++)
                                {
                                    for(int j = 0; j < ddlStatus.Items.Count; j++)
                                    {
                                        if(name[i] == ddlStatus.Items[j].Value)
                                        {
                                            ((Label)UpdatePanel2.ContentTemplateContainer.Controls[i * 2]).Text = name[i] + " 成功 卡數：" + list[i].Rows.Count;
                                            UpdatePanel2.ContentTemplateContainer.Controls[i * 2].Visible = true;
                                            UpdatePanel2.ContentTemplateContainer.Controls[i * 2 + 1].Visible = true;
                                            ((CustGridView)UpdatePanel2.ContentTemplateContainer.Controls[i * 2 + 1]).DataSource = list[i];
                                            UpdatePanel2.ContentTemplateContainer.Controls[i * 2 + 1].DataBind();
                                            count += list[i].Rows.Count;
                                        }
                                    }
                                }
                                Label1.Text = "成功 總卡數：" + count;
                                Label1.Visible = true;
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_001"));
                            }
                            //* 查詢不成功
                            else
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgId == "" ? "06_05140000_002" : strMsgId));
                            }
                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            Boolean result = BR_Excel_File.GetDataTable05141(param, ref dt);

                            //* 查詢成功
                            if (result)
                            {
                                ((Label)UpdatePanel2.ContentTemplateContainer.Controls[0]).Text = ddlStatus.SelectedValue + " 成功 卡數：" + dt.Rows.Count;
                                UpdatePanel2.ContentTemplateContainer.Controls[0].Visible = true;
                                UpdatePanel2.ContentTemplateContainer.Controls[1].Visible = true;
                                ((CustGridView)UpdatePanel2.ContentTemplateContainer.Controls[1]).DataSource = dt;
                                UpdatePanel2.ContentTemplateContainer.Controls[1].DataBind();
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_001"));
                            }
                            //* 查詢不成功
                            else
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_002"));
                            }
                        }
                    }
                    else if (ddlType.SelectedValue.Equals("2"))
                    {
                        this.gpList.RecordCount = 0;
                        this.grvUserView.DataSource = null;
                        this.grvUserView.DataBind();
                        this.gpList.Visible = false;
                        this.grvUserView.Visible = false;
                        Label1.Visible = false;
                        for (int i = 0; i < UpdatePanel2.ContentTemplateContainer.Controls.Count; i++)
                        {
                            if (i % 2 != 0)
                            {
                                ((CustGridView)UpdatePanel2.ContentTemplateContainer.Controls[i]).DataSource = null;
                                UpdatePanel2.ContentTemplateContainer.Controls[i].DataBind();
                            }
                            UpdatePanel2.ContentTemplateContainer.Controls[i].Visible = false;
                        }
                        for (int i = 0; i < UpdatePanel3.ContentTemplateContainer.Controls.Count; i++)
                        {
                            if (i % 2 != 0)
                            {
                                ((CustGridView)UpdatePanel3.ContentTemplateContainer.Controls[i]).DataSource = null;
                                UpdatePanel3.ContentTemplateContainer.Controls[i].DataBind();
                            }
                            UpdatePanel3.ContentTemplateContainer.Controls[i].Visible = false;
                        }

                        if (ddlStatus.SelectedValue.Equals("0"))
                        {
                            String strMsgId = "";
                            List<DataTable> list = new List<DataTable>();
                            List<String> name = new List<String>();
                            Boolean result = BR_Excel_File.GetDataTable05142(param, ref list, ref name, ref strMsgId);
                            //* 查詢成功
                            if (result)
                            {
                                Int32 count = 0;
                                for (int i = 0; i < list.Count && i < name.Count && i * 2 < UpdatePanel3.ContentTemplateContainer.Controls.Count; i++)
                                {
                                    for (int j = 0; j < ddlStatus.Items.Count; j++)
                                    {
                                        if (name[i] == ddlStatus.Items[j].Value)
                                        {
                                            ((Label)UpdatePanel3.ContentTemplateContainer.Controls[i * 2]).Text = name[i] + " 失敗 卡數：" + list[i].Rows.Count;
                                            UpdatePanel3.ContentTemplateContainer.Controls[i * 2].Visible = true;
                                            UpdatePanel3.ContentTemplateContainer.Controls[i * 2 + 1].Visible = true;
                                            ((CustGridView)UpdatePanel3.ContentTemplateContainer.Controls[i * 2 + 1]).DataSource = list[i];
                                            UpdatePanel3.ContentTemplateContainer.Controls[i * 2 + 1].DataBind();
                                            count += list[i].Rows.Count;
                                        }
                                    }
                                }
                                Label1.Text = "失敗 總卡數：" + count;
                                Label1.Visible = true;
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_001"));
                            }
                            //* 查詢不成功
                            else
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgId == "" ? "06_05140000_002" : strMsgId));
                            }
                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            Boolean result = BR_Excel_File.GetDataTable05143(param, ref dt);

                            //* 查詢成功
                            if (result)
                            {
                                ((Label)UpdatePanel3.ContentTemplateContainer.Controls[0]).Text = ddlStatus.SelectedValue + " 失敗 卡數：" + dt.Rows.Count;
                                UpdatePanel3.ContentTemplateContainer.Controls[0].Visible = true;
                                UpdatePanel3.ContentTemplateContainer.Controls[1].Visible = true;
                                ((CustGridView)UpdatePanel3.ContentTemplateContainer.Controls[1]).DataSource = dt;
                                UpdatePanel3.ContentTemplateContainer.Controls[1].DataBind();
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_001"));
                            }
                            //* 查詢不成功
                            else
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_002"));
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05140000_002"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        if (this.txtdateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_05140000_003");
            txtdateStart.Focus();
            return false;
        }
        if (txtdateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_05140000_003");
            txtdateEnd.Focus();
            return false;
        }

        String strMsgID = String.Empty;

        try
        {
            // 初始化報表參數
            param = new Dictionary<String, String>
                {
                    // 檔案產出日期 起
                    {"OstartDate", txtdateStart.Text.Trim().Equals("") ? "NULL" : txtdateStart.Text.Trim()},
                    
                    // 檔案產出日期 迄
                    {"OendDate", txtdateEnd.Text.Trim().Equals("") ? "NULL" : txtdateEnd.Text.Trim()},

                    {"Ouser", ((EntityAGENT_INFO)Session["Agent"]).agent_name}
                };
            if (rbResult.Checked)
            {
                DataTable dtOASA = new DataTable();

                if (ddlType.SelectedValue.Equals("1"))
                {
                    param.Add("flag", "1");

                    if (BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID))
                        param.Add("num", dtOASA.Rows.Count.ToString());
                    else
                        param.Add("num", "0");

                    if (!ddlStatus.SelectedValue.Equals("0"))
                        param.Add("BLKCode", ddlStatus.SelectedValue);
                }
                else if (ddlType.SelectedValue.Equals("2"))
                {
                    param.Add("flag", "2");

                    if (BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID))
                        param.Add("num", dtOASA.Rows.Count.ToString());
                    else
                        param.Add("num", "0");

                    if (!ddlStatus.SelectedValue.Equals("0"))
                        param.Add("BLKCode", ddlStatus.SelectedValue);
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05140000_004");
            return false;
        }
        return true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/18
    /// </summary>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        // 初始化報表參數
        Dictionary<String, String> param = new Dictionary<String, String>();
        if (chkCond(ref param))
        {
            try
            {
                string strMsgId = string.Empty;
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

                if (rbCount.Checked)
                {
                    //產生報表
                    bool result = BR_Excel_File.CreateExcelFile_0514Report(param, ref strServerPathFile, ref strMsgId);

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
                else if (rbResult.Checked)
                {
                    //產生報表狀態
                    bool result = false;

                    if (ddlType.SelectedValue.Equals("1"))
                    {
                        if (ddlStatus.SelectedValue.Equals("0"))
                            result = BR_Excel_File.CreateExcelFile_0514_0Report(param, ref strServerPathFile, ref strMsgId);
                        else
                            result = BR_Excel_File.CreateExcelFile_0514_1Report(param, ref strServerPathFile, ref strMsgId);

                    }
                    else if (ddlType.SelectedValue.Equals("2"))
                    {
                        if (ddlStatus.SelectedValue.Equals("0"))
                            result = BR_Excel_File.CreateExcelFile_0514_2Report(param, ref strServerPathFile, ref strMsgId);
                        else
                            result = BR_Excel_File.CreateExcelFile_0514_3Report(param, ref strServerPathFile, ref strMsgId);
                    }

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
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.BusinessRule);
                MessageHelper.ShowMessage(this, "06_05140000_004");
            }
        }
    }

    private void BindControl()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "30", ref dtState))
        {
            this.ddlStatus.DataSource = dtState;
            this.ddlStatus.DataTextField = "PROPERTY_NAME";
            this.ddlStatus.DataValueField = "PROPERTY_CODE";
            this.ddlStatus.DataBind();
        }

        ListItem lis = new ListItem(BaseHelper.GetShowText("06_06051400_007"), "1");
        ListItem lif = new ListItem(BaseHelper.GetShowText("06_06051400_008"), "2");
        ddlType.Items.Add(lis);
        ddlType.Items.Add(lif);
        ddlType.Items[0].Selected = true;
    }

    private void CreateTable(String name)
    {
        {
            Label lb = getNewLB(name);
            CustGridView cgv = getNewCGV();
            String[] arrStr = { "ROW_NUM", "OTYPE", "SENDDATE", "CARDNO", "NBLKCODE", "MEMO", "PERMITDATE", "SYS_DATE", "UPDUSER", "O1" };
            Int32[] arrInt = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            for (int i = 0; i < arrStr.Length && i < arrInt.Length; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = arrStr[i];
                bf.ItemStyle.Width = Unit.Percentage(arrInt[i]);
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                bf.HeaderText = BaseHelper.GetShowText("06_06051400_0" + (17 + i).ToString());
                cgv.Columns.Add(bf);
            }
            UpdatePanel2.ContentTemplateContainer.Controls.Add(lb);
            UpdatePanel2.ContentTemplateContainer.Controls.Add(cgv);
        }
        {
            Label lb = getNewLB(name);
            CustGridView cgv = getNewCGV();
            String[] arrStr = { "ROW_NUM", "OTYPE", "SENDDATE", "CARDNO", "FAIL_REASON", "NBLKCODE", "MEMO", "REASON_CODE", "ACTION_CODE", "CWB_REGIONS", "O1", "O2" };
            Int32[] arrInt = { 3, 7, 7, 10, 5, 10, 8, 10, 10, 10, 10, 10 };
            for (int i = 0; i < arrStr.Length && i < arrInt.Length; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = arrStr[i];
                bf.ItemStyle.Width = Unit.Percentage(arrInt[i]);
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                bf.HeaderText = BaseHelper.GetShowText("06_06051400_0" + (27 + i).ToString());
                cgv.Columns.Add(bf);
            }
            UpdatePanel3.ContentTemplateContainer.Controls.Add(lb);
            UpdatePanel3.ContentTemplateContainer.Controls.Add(cgv);
        }
    }
    private CustGridView getNewCGV()
    {
        CustGridView cgv = new CustGridView();
        cgv.AllowSorting = true;
        cgv.AllowPaging = false;
        cgv.Width = Unit.Percentage(100);
        cgv.BorderWidth = Unit.Pixel(0);
        cgv.CellPadding = 0;
        cgv.CellSpacing = 1;
        cgv.BorderStyle = BorderStyle.Solid;
        cgv.RowStyle.CssClass = "Grid_Item";
        cgv.RowStyle.Wrap = true;
        cgv.SelectedRowStyle.CssClass = "Grid_SelectedItem";
        cgv.HeaderStyle.CssClass = "Grid_Header";
        cgv.HeaderStyle.Wrap = false;
        cgv.AlternatingRowStyle.CssClass = "Grid_AlternatingItem";
        cgv.AlternatingRowStyle.Wrap = true;
        cgv.PagerSettings.Visible = false;
        cgv.EmptyDataRowStyle.HorizontalAlign = HorizontalAlign.Center;
        cgv.Visible = false;
        return cgv;
    }
    private Label getNewLB(String name)
    {
        Label lb = new Label();
        lb.Text = name;
        lb.Visible = false;
        return lb;
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:JUN HU
    /// 創建時間:2010/07/15
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_FileCode, Operator.Equal, DataTypeUtils.String, "3");
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_Success_Flag, Operator.Equal, DataTypeUtils.String, ddlType.SelectedValue);
        if (!ddlStatus.SelectedValue.Equals("0"))
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_NBLKCode, Operator.Equal, DataTypeUtils.String, ddlStatus.SelectedValue);
        }
        if (this.txtdateStart.Text.Trim() != "" && this.txtdateEnd.Text.Trim() == "")
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtdateStart.Text.Trim());
        }
        if (this.txtdateStart.Text.Trim() == "" && this.txtdateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.LessThanEqual, DataTypeUtils.String, this.txtdateEnd.Text.Trim());
        }
        if (this.txtdateStart.Text.Trim() != "" && this.txtdateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.LessThanEqual, DataTypeUtils.String, this.txtdateEnd.Text.Trim());
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtdateStart.Text.Trim());
        }
        return sqlhelp.GetFilterCondition();
    }
}
