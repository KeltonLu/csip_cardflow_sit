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
        string strMsgID = string.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            MessageHelper.ShowMessage(this, strMsgID);
            return;
        }

        try
        {
            #region 查詢條件參數

            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            //*匯入日期起日
            param.Add("startDate",txtImportStart.Text.Trim());

            //*匯入日期訖日
            param.Add("endDate",txtImportEnd.Text.Trim());

            //狀態
            param.Add("outFlg", ddlState.SelectedValue.Equals("0") ? "NULL" : this.ddlState.SelectedValue);

            #endregion

            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0503Report(param, ref strServerPathFile, ref strMsgID);

            if (result)
            {
                FileInfo fs = new FileInfo(strServerPathFile);
                Session["ServerFile"] = strServerPathFile;
                Session["ClientFile"] = fs.Name;
                string urlString = @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
            else {
                MessageHelper.ShowMessage(this, strMsgID);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            MessageHelper.ShowMessage(this, "06_05030000_003");

        }
    }
}
