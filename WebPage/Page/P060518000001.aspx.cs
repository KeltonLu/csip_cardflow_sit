//******************************************************************
//*  功能說明：退件連絡報表
//*  作    者：JUN HU
//*  創建日期：2010/07/02
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//  Joe               20210120        RQ-2019-008159-003     配合長姓名作業修改
//*******************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using System.Web.UI.WebControls;

public partial class P060518000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindState();
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
    /// 修改時間:2020/08/17
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051800_005");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051800_006");
        // 2021/02/02 增加羅馬拼音 陳永銘
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_05020000_013");

        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051800_007");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051800_008");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051800_009");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051800_010");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06051800_011");
        this.grvUserView.Columns[8].HeaderText = BaseHelper.GetShowText("06_06051800_012");
        this.grvUserView.Columns[9].HeaderText = BaseHelper.GetShowText("06_06051800_013");
        this.grvUserView.Columns[10].HeaderText = BaseHelper.GetShowText("06_06051800_014");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
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
                Boolean result = BR_Excel_File.GetDataTable0518(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    // 2021/02/02 無查詢資料則隱藏欄位 陳永銘
                    if (count == 0)
                    {
                        this.grvUserView.Columns[2].Visible = false;
                    }
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051800_004"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051800_005"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051800_005"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/02
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
    /// 修改時間:2020/08/17
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        String strMsgId = String.Empty;
        if (txtMaildateStart.Text.Trim().Equals(""))
        {
            txtMaildateStart.Focus();
            return false;
        }
        if (txtMaildateEnd.Text.Trim().Equals(""))
        {
            txtMaildateEnd.Focus();
            return false;
        }
        if (!ValidateHelper.IsValidDate(txtMaildateStart.Text.Trim(), DateTime.Now.ToString("yyyy/MM/dd"),
            ref strMsgId) || !ValidateHelper.IsValidDate(txtMaildateEnd.Text.Trim(),
            DateTime.Now.ToString("yyyy/MM/dd"), ref strMsgId))
            return false;
        // 查詢日期起日
        param.Add("datefrom", txtMaildateStart.Text.Trim());

        // 查詢日期迄日
        param.Add("dateto", txtMaildateEnd.Text.Trim());

        // 卡別
        param.Add("backtype", ddlState.SelectedValue);
        return true;
    }
    /// <summary>
    /// 功能說明:加載狀態選項
    /// 作    者:JUN HU
    /// 創建時間:2010/07/02
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindState()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "28", ref dtState))
        {
            this.ddlState.DataSource = dtState;
            this.ddlState.DataTextField = "PROPERTY_NAME";
            this.ddlState.DataValueField = "PROPERTY_CODE";
            this.ddlState.DataBind();
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
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
                string strServerPathFile = Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0518Report(param, ref strServerPathFile, ref strMsgId);

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
                Logging.Log(exp, LogLayer.BusinessRule);
                MessageHelper.ShowMessage(this, "06_06051800_001");
            }
        }
    }

    //20210131 陳永銘 收件者姓名替換
    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string name = e.Row.Cells[1].Text.Replace("&nbsp;", "").Trim();
        string name_roma = e.Row.Cells[2].Text.Replace("&nbsp;", "").Trim();

        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[2].Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Visible = false;

            if (name.Length >= 5 || name_roma != string.Empty)
            {
                int length = name.Length >= 5 ? 5 : name.Length;
                e.Row.Cells[1].Text = name.Substring(0, length) + "...";
                name += Environment.NewLine + name_roma;
                e.Row.Cells[1].Attributes.Add("title", name);
            }
        }
    }
}