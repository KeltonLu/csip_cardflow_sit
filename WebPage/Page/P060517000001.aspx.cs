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
        string strMsgId = string.Empty;
        if (!CheckCondition(ref strMsgId))
        {
            // this.ReportViewer0517.Visible = false;
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgId) + "')");
            return;
        }

        try
        {
            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            // 退件日期起日
            String MstatrDate = dateFrom.Text.Trim();
            param.Add("MstatrDate",MstatrDate);

            // 退件日期迄日
            String MendDate = dateTo.Text.Trim();
            param.Add("MendDate", MendDate);

            // 卡別
            param.Add("Action", ddlAction.SelectedValue.Equals("0") ? "NULL" : ddlAction.SelectedValue.Trim());


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
