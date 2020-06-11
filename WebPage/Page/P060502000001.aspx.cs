//******************************************************************
//*  功能說明：無法製卡檔查詢
//*  作    者：HAO CHEN
//*  創建日期：2020/03/27
//*  修改人員：Luke
//*  修改記錄：2020/03/30
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;

public partial class P060502000001 : PageBase
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
            strMsgID = "06_05020000_001";
            txtImportStart.Focus();
            return false;
        }

        if (string.IsNullOrEmpty(txtImportEnd.Text))
        {
            strMsgID = "06_05020000_002";
            txtImportEnd.Focus();
            return false;
        }

        // 匯入日期區間
        if (!ValidateHelper.IsValidDate(strStart, strEnd, ref strMsgID))
        {
            strMsgID = "06_05020000_000";
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

            //*匯入日期起日
            String startDate = this.txtImportStart.Text.Trim();

            //*匯入日期訖日
            String endDate = this.txtImportEnd.Text.Trim();

            //狀態
            String outFlg = this.ddlState.SelectedValue;
            if (this.ddlState.SelectedValue.Equals("0"))
                outFlg = "00";

            //廠商
            String factory = ddlFactory.SelectedValue;
            if (ddlFactory.SelectedValue.Equals("0"))
                factory = "00";

            #endregion

            string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0502Report(startDate, endDate,outFlg,  factory, ref strServerPathFile, ref strMsgID);

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
            //Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05020000_003");

        }
    }
}
