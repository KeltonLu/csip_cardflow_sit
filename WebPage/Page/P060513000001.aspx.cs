using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using EntityLayer;
using Framework.Data.OM;
using BusinessRules;
using CSIPCommonModel.EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using Framework.WebControls;

public partial class P060513000001 : PageBase
{
    private List<CustGridView> cgvList = new List<CustGridView>();
    private List<Label> lbList = new List<Label>();

    protected void Page_Load(object sender, EventArgs e)
    {
        cgvList.Add(CustGridView1);
        cgvList.Add(CustGridView2);
        cgvList.Add(CustGridView3);
        cgvList.Add(CustGridView4);
        cgvList.Add(CustGridView5);
        cgvList.Add(CustGridView6);
        cgvList.Add(CustGridView7);
        cgvList.Add(CustGridView8);
        lbList.Add(Label1);
        lbList.Add(Label2);
        lbList.Add(Label3);
        lbList.Add(Label4);
        lbList.Add(Label5);
        lbList.Add(Label6);
        lbList.Add(Label7);
        lbList.Add(Label8);

        if (!IsPostBack)
        {
            BindControl();
            ShowControlsText();
            this.grvUserView.Visible = false;
            for (int i = 0; i < cgvList.Count; i++)
                cgvList[i].Visible = false;
            for (int i = 0; i < lbList.Count; i++)
                lbList[i].Visible = false;
            Label9.Visible = false;
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051300_008");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051300_009");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051300_010");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051300_011");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051300_012");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051300_013");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051300_014");

        for(int i = 0; i < 10; i++)
        {
            CustGridView1.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (15 + i).ToString());
            CustGridView2.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (15 + i).ToString());
            CustGridView3.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (15 + i).ToString());
            CustGridView4.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (15 + i).ToString());
        }

        for (int i = 0; i < 12; i++)
        {
            CustGridView5.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (25 + i).ToString());
            CustGridView6.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (25 + i).ToString());
            CustGridView7.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (25 + i).ToString());
            CustGridView8.Columns[i].HeaderText = BaseHelper.GetShowText("06_06051300_0" + (25 + i).ToString());
        }

        //* 設置一頁顯示最大筆數
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
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
                    for (int i = 0; i < cgvList.Count; i++)
                    {
                        cgvList[i].DataSource = null;
                        cgvList[i].DataBind();
                        cgvList[i].Visible = false;
                    }
                    for (int i = 0; i < lbList.Count; i++)
                        lbList[i].Visible = false;
                    Label9.Visible = false;

                    DataTable dt = new DataTable();
                    Boolean result = BR_Excel_File.GetDataTable0513(param, ref dt);
                    //* 查詢成功
                    if (result)
                    {
                        this.grvUserView.Visible = true;
                        this.grvUserView.DataSource = dt;
                        this.grvUserView.DataBind();
                        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05130000_001"));
                    }
                    //* 查詢不成功
                    else
                    {
                        this.grvUserView.DataSource = null;
                        this.grvUserView.DataBind();
                        this.grvUserView.Visible = false;
                        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05130000_002"));
                    }
                }
                else if (rbResult.Checked)
                {
                    if (ddlStatus.SelectedValue.Equals("1"))
                    {
                        this.grvUserView.DataSource = null;
                        this.grvUserView.DataBind();
                        this.grvUserView.Visible = false;
                        for (int i = 0; i < cgvList.Count; i++)
                        {
                            cgvList[i].DataSource = null;
                            cgvList[i].DataBind();
                            cgvList[i].Visible = false;
                        }
                        for (int i = 0; i < lbList.Count; i++)
                            lbList[i].Visible = false;
                        Label9.Visible = false;

                        List<DataTable> list = new List<DataTable>();
                        List<String> name = new List<String>();
                        String strMsgId = "";
                        Boolean result = BR_Excel_File.GetDataTable05130(param, ref list, ref name, ref strMsgId);
                        //* 查詢成功
                        if (result)
                        {
                            Int32 count = 0;
                            for(int i = 0; i < list.Count && i < name.Count; i++)
                            {
                                switch(name[i])
                                {
                                    case "強停(B)":
                                        {
                                            CustGridView1.Visible = true;
                                            CustGridView1.DataSource = list[i];
                                            CustGridView1.DataBind();
                                            Label1.Text = name[i] + " 成功 卡數：" + list[i].Rows.Count;
                                            Label1.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                    case "管制(R)":
                                        {
                                            CustGridView2.Visible = true;
                                            CustGridView2.DataSource = list[i];
                                            CustGridView2.DataBind();
                                            Label2.Text = name[i] + " 成功 卡數：" + list[i].Rows.Count;
                                            Label2.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                    case "管制(S)":
                                        {
                                            CustGridView3.Visible = true;
                                            CustGridView3.DataSource = list[i];
                                            CustGridView3.DataBind();
                                            Label3.Text = name[i] + " 成功 卡數：" + list[i].Rows.Count;
                                            Label3.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                    case "解管(R)":
                                        {
                                            CustGridView4.Visible = true;
                                            CustGridView4.DataSource = list[i];
                                            CustGridView4.DataBind();
                                            Label4.Text = name[i] + " 成功 卡數：" + list[i].Rows.Count;
                                            Label4.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                }
                            }
                            Label9.Text = "成功 總卡數：" + count;
                            Label9.Visible = true;
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05130000_001"));
                        }
                        //* 查詢不成功
                        else
                        {
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgId));
                        }
                    }
                    else if (ddlStatus.SelectedValue.Equals("2"))
                    {
                        this.grvUserView.DataSource = null;
                        this.grvUserView.DataBind();
                        this.grvUserView.Visible = false;
                        for (int i = 0; i < cgvList.Count; i++)
                        {
                            cgvList[i].DataSource = null;
                            cgvList[i].DataBind();
                            cgvList[i].Visible = false;
                        }
                        for (int i = 0; i < lbList.Count; i++)
                            lbList[i].Visible = false;
                        Label9.Visible = false;

                        List<DataTable> list = new List<DataTable>();
                        List<String> name = new List<String>();
                        String strMsgId = "";
                        Boolean result = BR_Excel_File.GetDataTable05132(param, ref list, ref name, ref strMsgId);
                        //* 查詢成功
                        if (result)
                        {
                            Int32 count = 0;
                            for (int i = 0; i < list.Count && i < name.Count; i++)
                            {
                                switch (name[i])
                                {
                                    case "強停(B)":
                                        {
                                            CustGridView5.Visible = true;
                                            CustGridView5.DataSource = list[i];
                                            CustGridView5.DataBind();
                                            Label5.Text = name[i] + " 失敗 卡數：" + list[i].Rows.Count;
                                            Label5.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                    case "管制(R)":
                                        {
                                            CustGridView6.Visible = true;
                                            CustGridView6.DataSource = list[i];
                                            CustGridView6.DataBind();
                                            Label6.Text = name[i] + " 失敗 卡數：" + list[i].Rows.Count;
                                            Label6.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                    case "管制(S)":
                                        {
                                            CustGridView7.Visible = true;
                                            CustGridView7.DataSource = list[i];
                                            CustGridView7.DataBind();
                                            Label7.Text = name[i] + " 失敗 卡數：" + list[i].Rows.Count;
                                            Label7.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                    case "解管(R)":
                                        {
                                            CustGridView8.Visible = true;
                                            CustGridView8.DataSource = list[i];
                                            CustGridView8.DataBind();
                                            Label8.Text = name[i] + " 失敗 卡數：" + list[i].Rows.Count;
                                            Label8.Visible = true;
                                            count += list[i].Rows.Count;
                                        }
                                        break;
                                }
                            }
                            Label9.Text = "失敗 總卡數：" + count;
                            Label9.Visible = true;
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05130000_001"));
                        }
                        //* 查詢不成功
                        else
                        {
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgId));
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05130000_002"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        if (this.txtdateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_05130000_003");
            txtdateStart.Focus();
            return false;
        }
        if (txtdateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_05130000_003");
            txtdateEnd.Focus();
            return false;
        }

        string strMsgId = string.Empty;

        try
        {
            if (rbCount.Checked)
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
            }

            else if (rbResult.Checked)
            {
                // 初始化報表參數
                param = new Dictionary<String, String>();

                //產生報表狀態

                string strMsgID = "";
                DataTable dtOASA = new DataTable();
                //初始化報表參數,為Report View賦值參數


                param.Add("OstartDate", txtdateStart.Text.Trim().Equals("") ? "NULL" : txtdateStart.Text.Trim());
                param.Add("OendDate", txtdateStart.Text.Trim().Equals("") ? "NULL" : txtdateEnd.Text.Trim());
                param.Add("Ouser", ((EntityAGENT_INFO)Session["Agent"]).agent_name);

                if (ddlStatus.SelectedValue.Equals("1"))
                {
                    param.Add("flag", "1");
                    param.Add("num",
                        BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID)
                            ? dtOASA.Rows.Count.ToString()
                            : "0");
                }
                else if (ddlStatus.SelectedValue.Equals("2"))
                {
                    param.Add("flag", "2");


                    param.Add("num",
                        BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID)
                            ? dtOASA.Rows.Count.ToString()
                            : "0");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05130000_004");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/13
    /// 修改記錄:2020/07/10 area Luke Report to Excel 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
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
                    bool result = BR_Excel_File.CreateExcelFile_0513Report(param, ref strServerPathFile, ref strMsgId);

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

                    if (ddlStatus.SelectedValue.Equals("1"))
                    {
                        result = BR_Excel_File.CreateExcelFile_0513_0Report(param, ref strServerPathFile, ref strMsgId);

                    }
                    else if (ddlStatus.SelectedValue.Equals("2"))
                    {
                        result = BR_Excel_File.CreateExcelFile_0513_2Report(param, ref strServerPathFile, ref strMsgId);
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
                MessageHelper.ShowMessage(this, "06_05130000_004");
            }
        }
    }

    private void BindControl()
    {
        ListItem lis = new ListItem(BaseHelper.GetShowText("06_06051300_004"), "1");
        ListItem lif = new ListItem(BaseHelper.GetShowText("06_06051300_005"), "2");
        ddlStatus.Items.Add(lis);
        ddlStatus.Items.Add(lif);
        ddlStatus.Items[0].Selected = true;
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:JUN HU
    /// 創建時間:2010/07/14
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_FileCode, Operator.In, DataTypeUtils.String, "'0','1','2'");
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_Success_Flag, Operator.Equal, DataTypeUtils.String, ddlStatus.SelectedValue);
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
