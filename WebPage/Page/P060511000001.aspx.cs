//******************************************************************
//*  功能說明：更改寄送方式記錄查詢

//*  作    者：JUN HU
//*  創建日期：2010/06/24
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P060511000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ckUrgency_Flg.Text = BaseHelper.GetShowText("06_06051000_014");
            txtBackdateStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            txtBackdateEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            BindKind();
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051100_005");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051100_006");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051100_007");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051100_008");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051100_009");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051100_010");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051100_011");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06051100_012");
        this.grvUserView.Columns[8].HeaderText = BaseHelper.GetShowText("06_06051100_013");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    private void BindGridView()
    {
        // 初始化報表參數
        Dictionary<String, String> param = new Dictionary<String, String>();
        if (chkCond(ref param))
        {
            try
            {
                DataTable dt = new DataTable();
                Int32 count = 0;
                Boolean result = BR_Excel_File.GetDataTable0511(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051100_000"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051100_001"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051100_001"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
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
        custddlType.Items.Clear();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKindName))
        {
            foreach (DataRow dr in dtKindName.Rows)
            {
                ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                custddlType.Items.Add(liTmp);
            }
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "");
            custddlType.Items.Insert(0, li);
        }

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
    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/21
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        if (txtBackdateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_06051200_000");
            return false;
        }
        if (txtBackdateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_06051200_000");
            return false;
        }

        #region 查詢條件參數

        param.Add("indatefrom", txtBackdateStart.Text.Trim().Equals("") ? "NULL" : txtBackdateStart.Text.Trim());
        param.Add("indateto", txtBackdateEnd.Text.Trim().Equals("") ? "NULL" : txtBackdateEnd.Text.Trim());

        if (custddlType.SelectedItem.Value.Trim().Equals(""))
        {
            param.Add("newway", "00");
            param.Add("Enditem", "00");
        }
        else
        {
            param.Add("newway", custddlType.SelectedValue);

            switch (custddlType.SelectedValue.ToString().Trim())
            {
                case "0":
                    param.Add("Enditem", "1");
                    break;
                case "1":
                    param.Add("Enditem", "0");
                    break;
                case "3":
                    param.Add("Enditem", "2");
                    break;
                case "4":
                    param.Add("Enditem", "3");
                    break;
                case "10":
                    param.Add("Enditem", "6");
                    break;
                case "11":
                    param.Add("Enditem", "5");
                    break;
                default:
                    param.Add("Enditem", "7");
                    break;
            }
        }

        param.Add("factory", ddlFactory.SelectedItem.Value.Trim().Equals("0") ? "00" : ddlFactory.SelectedValue);
        param.Add("UrgencyFlg", ckUrgency_Flg.Checked ? "1" : "NULL");

        #endregion
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
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

                // 產生報表
                bool result = BR_Excel_File.CreateExcelFile_0511Report(param, ref strServerPathFile, ref strMsgId);

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
                Logging.Log(exp);
                MessageHelper.ShowMessage(this, "06_06051100_002");
            }
        }
    }
}
