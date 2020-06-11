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

public partial class Page_P060604000002 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadReport();
    }

    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:zhiyuan
    /// 創建時間:2010/08/03
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        try
        {
            // this.ReportViewer0604.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0604.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0604Report";
            // this.ReportViewer0604.Visible = true;

            //初始化報表參數,為Report View賦值參數

            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[4];

            if (Request.QueryString["dtStart"].Equals(""))
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("dtStart", "NULL");
            }
            else
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("dtStart", RedirectHelper.GetDecryptString(Request.QueryString["dtStart"]));
            }
            if (Request.QueryString["dtEnd"].Equals(""))
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("dtEnd", "NULL");
            }
            else
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("dtEnd", RedirectHelper.GetDecryptString(Request.QueryString["dtEnd"]));
            }
            if (Request.QueryString["flg"].Equals(""))
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("flg", "NULL");
            }
            else
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("flg", RedirectHelper.GetDecryptString(Request.QueryString["flg"]));
            }
            if (Request.QueryString["user"].Equals(""))
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("user", "NULL");
            }
            else
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("user", "%" + RedirectHelper.GetDecryptString(Request.QueryString["user"]) + "%");
            }
            // this.ReportViewer0604.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_001"));
            return;
        }
    }
}
