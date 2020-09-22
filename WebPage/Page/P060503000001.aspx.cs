//******************************************************************
//*  功能說明：換卡異動檔查詢
//*  作    者：HAO CHEN
//*  創建日期：2010/06/30
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

public partial class P060503000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //*加載狀態選項
            BindState();
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
        }
    }

    /// <summary>
    /// 功能說明:加載狀態選項
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindState()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "25", ref dtState))
        {
            this.ddlState.DataSource = dtState;
            this.ddlState.DataTextField = "PROPERTY_NAME";
            this.ddlState.DataValueField = "PROPERTY_CODE";
            this.ddlState.DataBind();
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/07
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_05030000_002");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_05030000_006");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_05030000_007");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_05030000_004");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/07
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
                Boolean result = BR_Excel_File.GetDataTable0503(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05030000_005"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05030000_006"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_05030000_006"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/07
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
    /// 修改時間:2020/09/07
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        String strMsgID = String.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            MessageHelper.ShowMessage(this, strMsgID);
            return false;
        }

        try
        {
            #region 查詢條件參數

            // 初始化報表參數
            param = new Dictionary<String, String>();

            //*匯入日期起日
            param.Add("startDate", txtImportStart.Text.Trim());

            //*匯入日期訖日
            param.Add("endDate", txtImportEnd.Text.Trim());

            //狀態
            param.Add("outFlg", ddlState.SelectedValue.Equals("0") ? "NULL" : this.ddlState.SelectedValue);

            #endregion
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            MessageHelper.ShowMessage(this, "06_05030000_003");
            return false;
        }
        return true;
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
        string strStart = txtImportStart.Text.Trim();
        string strEnd = txtImportEnd.Text.Trim();

        if (string.IsNullOrEmpty(txtImportStart.Text))
        {
            strMsgID = "06_05030000_001";
            txtImportStart.Focus();
            return false;
        }

        if (string.IsNullOrEmpty(txtImportEnd.Text))
        {
            strMsgID = "06_05030000_002";
            txtImportEnd.Focus();
            return false;
        }

        //*匯入日期
        if (!ValidateHelper.IsValidDate(strStart, strEnd, ref strMsgID))
        {
            strMsgID = "06_05030000_000";
            txtImportStart.Focus();
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
    /// 修改時間:2020/09/07
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
                bool result = BR_Excel_File.CreateExcelFile_0503Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, "06_05030000_003");
            }
        }
    }
}
