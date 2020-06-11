//******************************************************************
//*  功能說明：退件原因統計表
//*  作    者：HAO CHEN
//*  創建日期：2010/07/14
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Cryptography;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using Framework.Data.OM.Collections;
using System.Configuration;

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
        string strMsgID = string.Empty;
        if (!CheckCondition(ref strMsgID))
        {
            // this.ReportViewer0517.Visible = false;
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }

        try
        {
            // this.ReportViewer0517.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0517.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0517Report";
            // this.ReportViewer0517.Visible = true;

            //初始化報表參數,為Report View賦值參數
            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[3];

            // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("MstatrDate", dateFrom.Text.Trim());
            // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("MendDate", dateTo.Text.Trim());

            if (ddlAction.SelectedValue.Equals("0"))
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("Action", "NULL");
            }
            else
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("Action", ddlAction.SelectedValue.Trim());
            }
           
            // this.ReportViewer0517.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            // this.ReportViewer0517.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05170000_003");
        }
    }
}
