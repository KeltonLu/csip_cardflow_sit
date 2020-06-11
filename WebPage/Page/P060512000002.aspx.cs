using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Framework.Common.Utility;
using Framework.Common.Logging;
using Framework.Common.JavaScript;

public partial class P060512000002 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadReport();
    }

    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        try
        {
            // this.ReportViewer0512.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0512.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0512_1Report";
            // this.ReportViewer0512.Visible = true;

            //初始化報表參數,為Report View賦值參數

            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[2];

            if (Request.QueryString["indatefrom"].Equals(""))
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("indatefrom", "NULL");
            }
            else
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("indatefrom", RedirectHelper.GetDecryptString(Request.QueryString["indatefrom"]));
            }
            if (Request.QueryString["indateto"].Equals(""))
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("indateto", "NULL");
            }
            else
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("indateto", RedirectHelper.GetDecryptString(Request.QueryString["indateto"]));
            }
            // this.ReportViewer0512.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_001"));
            return;
        }
    }
}
