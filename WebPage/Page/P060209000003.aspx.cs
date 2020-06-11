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

public partial class Page_P060209000003 : System.Web.UI.Page
{
    #region params
    public string m_Source
    {
        get { return ViewState["m_Source"] as string; }
        set { ViewState["m_Source"] = value; }
    }
    public string m_File
    {
        get { return ViewState["m_File"] as string; }
        set { ViewState["m_File"] = value; }
    }
    public string m_Date
    {
        get { return ViewState["m_Date"] as string; }
        set { ViewState["m_Date"] = value; }
    }
    public string m_Visible
    {
        get { return ViewState["m_Visible"] as string; }
        set { ViewState["m_Visible"] = value; }
    }
    public string m_Status
    {
        get { return ViewState["m_Status"] as string; }
        set { ViewState["m_Status"] = value; }
    }
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
       m_Source = string.Empty;
       m_File = string.Empty;
       m_Date = string.Empty;
       m_Status = string.Empty;


        try
        {
            if (Request.QueryString["Source"] != null && Request.QueryString["File"] != null && Request.QueryString["Date"] != null && Request.QueryString["Visible"] != null && Request.QueryString["Status"] != null)
            {
                m_Source = RedirectHelper.GetDecryptString(Request.QueryString["Source"]);
                m_File = RedirectHelper.GetDecryptString(Request.QueryString["File"]);
                m_Date = RedirectHelper.GetDecryptString(Request.QueryString["Date"]);
                m_Visible = RedirectHelper.GetDecryptString(Request.QueryString["Visible"]);
                m_Status = RedirectHelper.GetDecryptString(Request.QueryString["Status"]);
            }
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');history.go(-1)");
                return;
            }

            // this.ReportViewer0209.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0209.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0209Report";
            // this.ReportViewer0209.Visible = true;
            //初始化報表參數,為Report View賦值參數
            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[3];
            // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("strSource", m_Source);
            // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("strFile", m_File);
            // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("strDate", m_Date);
            // this.ReportViewer0209.ServerReport.SetParameters(Paras);                           

        }
        catch (Exception exp)
        {
            MessageHelper.ShowMessage(this, "06_06020701_000" + exp.Message);
        }
     }

    protected void  btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060209000002.aspx?date=" + RedirectHelper.GetEncryptParam(m_Date) + "&source=" + RedirectHelper.GetEncryptParam(m_Source) + "&file=" + RedirectHelper.GetEncryptParam(m_File) + "&visible=" + RedirectHelper.GetEncryptParam(m_Visible) + "&Status=" + RedirectHelper.GetEncryptParam(m_Status) + "", false);
    }
}

