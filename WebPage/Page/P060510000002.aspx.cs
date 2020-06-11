//******************************************************************
//*  功能說明：扣卡明細查詢
//*  作    者：JUN HU
//*  創建日期：2010/06/23
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
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
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Utility;

public partial class P060510000002 : PageBase
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
            // this.ReportViewer0510.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0510.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0510Report";
            // this.ReportViewer0510.Visible = true;

            //初始化報表參數,為Report View賦值參數
            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[4];

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


            if (Request.QueryString["maildate"].Equals(""))
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("UpdDatefrom", "NULL");
            }
            else
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("UpdDatefrom", RedirectHelper.GetDecryptString(Request.QueryString["maildate"]));
            }

            if (Request.QueryString["maildate1"].Equals(""))
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("UpdDateto", "NULL");
            }
            else
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("UpdDateto", RedirectHelper.GetDecryptString(Request.QueryString["maildate1"]));
            }


            // this.ReportViewer0510.ServerReport.SetParameters(Paras);
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_001"));
            return;
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string strIndate1FromDate0510 = Session["Indate1FromDate0510"].ToString().Trim();
        string strIndate1ToDate0510 = Session["Indate1ToDate0510"].ToString().Trim();
        string strFromDate0510 = Session["FromDate0510"].ToString().Trim();
        string strToDate0510 = Session["ToDate0510"].ToString().Trim();
        string strPageIndex0510 = Session["PageIndex0510"].ToString().Trim();
        string strFactory0510 = Session["Factory0510"].ToString().Trim();

        Response.Redirect("P060510000001.aspx?Indate1FromDate0510=" + RedirectHelper.GetEncryptParam(strIndate1FromDate0510) + " &Indate1ToDate0510=" + RedirectHelper.GetEncryptParam(strIndate1ToDate0510) + " &FromDate0510=" + RedirectHelper.GetEncryptParam(strFromDate0510) + " &ToDate0510=" + RedirectHelper.GetEncryptParam(strToDate0510) + " &Factory0510=" + RedirectHelper.GetEncryptParam(strFactory0510) + " &PageIndex0510=" + RedirectHelper.GetEncryptParam(strPageIndex0510) + "");
    }
}
