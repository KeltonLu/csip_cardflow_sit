//******************************************************************
//*  功能說明：退件日報表
//*  作    者：HAO CHEN
//*  創建日期：2010/07/12
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

public partial class P060516000001 : PageBase
{
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
        if (string.IsNullOrEmpty(dateFrom.Text.Trim()))
        {
            strMsgID = "06_05160000_000";
            dateFrom.Focus();
            return false;
        }

        return true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        if (!CheckCondition(ref strMsgID))
        {
            // this.ReportViewer0516.Visible = false;
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }

        try
        {
            // this.ReportViewer0516.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0516.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0516Report";
            // this.ReportViewer0516.Visible = true;

            //初始化報表參數,為Report View賦值參數
            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[1];

            //*處理日期
            // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("Operaction", this.dateFrom.Text.Trim());

            // this.ReportViewer0516.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            // this.ReportViewer0516.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06051800_001");

        }
    }
}
