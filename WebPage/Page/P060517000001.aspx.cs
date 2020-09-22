//******************************************************************
//*  功能說明：退件原因統計表
//*  作    者：HAO CHEN
//*  創建日期：2010/07/14
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.IO;

public partial class P060517000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {//*加載卡別Action
            BindAction();
            //* 設定Tittle
            this.Page.Title = BaseHelper.GetShowText("06_06051700_000");
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
        }
    }

    /// <summary>
    /// 功能說明:加載卡別Action
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/14
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindAction()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtState))
        {
            this.ddlAction.DataSource = dtState;
            this.ddlAction.DataTextField = "PROPERTY_NAME";
            this.ddlAction.DataValueField = "PROPERTY_CODE";
            this.ddlAction.DataBind();
        }
        ListItem liTmp = new ListItem(BaseHelper.GetShowText("06_06051700_004"), "0");
        ddlAction.Items.Insert(0, liTmp);
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/04
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051700_006");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051700_007");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051700_008");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051700_009");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051700_010");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051700_011");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051700_012");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/04
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
                Boolean result = BR_Excel_File.GetDataTable0517(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051700_001"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051700_002"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051700_002"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/04
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
    /// 修改時間:2020/09/04
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        string strMsgId = string.Empty;
        if (!CheckCondition(ref strMsgId))
        {
            // this.ReportViewer0517.Visible = false;
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgId) + "')");
            return false;
        }

        try
        {
            // 初始化報表參數
            param = new Dictionary<string, string>();

            // 退件日期起日
            String MstatrDate = dateFrom.Text.Trim();
            param.Add("MstatrDate", MstatrDate);

            // 退件日期迄日
            String MendDate = dateTo.Text.Trim();
            param.Add("MendDate", MendDate);

            // 卡別
            param.Add("Action", ddlAction.SelectedValue.Equals("0") ? "NULL" : ddlAction.SelectedValue.Trim());
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05170000_003");
            return false;
        }
        return true;
    }


    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/14
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        string strMStart = dateFrom.Text.Trim();
        string strMEnd = dateTo.Text.Trim();

        if (string.IsNullOrEmpty(strMStart))
        {
            strMsgID = "06_05170000_000";
            return false;
        }
        if (string.IsNullOrEmpty(strMEnd))
        {
            strMsgID = "06_05170000_001";
            return false;
        }
        if (!string.IsNullOrEmpty(strMStart) && !string.IsNullOrEmpty(strMEnd))
        {
            //*退件日期開始時間不能大于結束時間
            if (!ValidateHelper.IsValidDate(strMStart, strMEnd, ref strMsgID))
            {
                strMsgID = "06_05170000_002";
                return false;
            }
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
    /// 修改時間:2020/09/04
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

                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0517Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, "06_05170000_003");
            }
        }
    }
}
