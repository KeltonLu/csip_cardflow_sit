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

public partial class P060205000002 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            string strMerchDate = string.Empty;
            string strMerchDateSQL = string.Empty;
            string strFetchDate = string.Empty;
            string strFetchDateSQL1 = string.Empty;
            string strFetchDateSQL2 = string.Empty;
            try
            {
                if (Request.QueryString["PrintType"] != null)
                {
                    switch (RedirectHelper.GetDecryptString(Request.QueryString["PrintType"]))
                    {
                        case "1":

                            if (Request.QueryString["MerchDate"] != null && Request.QueryString["MerchDateSQL"] != null)
                            {
                                strMerchDate = RedirectHelper.GetDecryptString(Request.QueryString["MerchDate"]);
                                strMerchDateSQL = RedirectHelper.GetDecryptString(Request.QueryString["MerchDateSQL"]);
                            }
                            else
                            {
                                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');history.go(-1)");
                                return;
                            }

                            // this.ReportViewer0205.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
                            // this.ReportViewer0205.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "020501Report";
                            // this.ReportViewer0205.Visible = true;
                            //初始化報表參數,為Report View賦值參數
                            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[3];
                            // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("strMerchDate", strMerchDate);
                            // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("strMerchDateSQL", strMerchDateSQL);
                            if(Session["Merch0205"].ToString().Trim()=="0")
                            {
                                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("strMerch", "NULL");
                            }
                            else
                            {
                                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("strMerch", Session["Merch0205"].ToString().Trim());
                            }
                            // this.ReportViewer0205.ServerReport.SetParameters(Paras);
                            break;
                        case "2":
                            string strDailyCloseDate = string.Empty;


                            if (Request.QueryString["FetchDate"] != null && Request.QueryString["FetchDateSQL1"] != null && Request.QueryString["FetchDateSQL2"] != null)
                            {
                                strFetchDate = RedirectHelper.GetDecryptString(Request.QueryString["FetchDate"]);
                                strFetchDateSQL1 = RedirectHelper.GetDecryptString(Request.QueryString["FetchDateSQL1"]);
                                strFetchDateSQL2 = RedirectHelper.GetDecryptString(Request.QueryString["FetchDateSQL2"]);
                            }
                            else
                            {
                                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');history.go(-1)");
                                return;
                            }

                            // this.ReportViewer0205.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
                            // this.ReportViewer0205.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "020502Report";
                            // this.ReportViewer0205.Visible = true;
                            //初始化報表參數,為Report View賦值參數
                            // Microsoft.Reporting.WebForms.ReportParameter[] Paras2 = new Microsoft.Reporting.WebForms.ReportParameter[3];
                            // Paras2[0] = new Microsoft.Reporting.WebForms.ReportParameter("strFetchDate", strFetchDate);
                            // Paras2[1] = new Microsoft.Reporting.WebForms.ReportParameter("strFetchDateSQL1", strFetchDateSQL1);
                            // Paras2[2] = new Microsoft.Reporting.WebForms.ReportParameter("strFetchDateSQL2", strFetchDateSQL2);
                            // this.ReportViewer0205.ServerReport.SetParameters(Paras2);
                            break;

                    }
                }
            }
            catch (Exception exp)
            {
                MessageHelper.ShowMessage(this, "06_06020701_000" + exp.Message);
            }
        }
    }

    /// <summary>
    /// 功能說明:回上頁
    /// 作    者:linda
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string strRadCheckFlg0205 = Session["RadCheckFlg0205"].ToString().Trim();
        string strFetchDate0205 = Session["FetchDate0205"].ToString().Trim();
        string strMerchDate0205 = Session["MerchDate0205"].ToString().Trim();
        string strMerch0205 = Session["Merch0205"].ToString().Trim();  
        string strId0205 = Session["Id0205"].ToString().Trim();
        string strCardNo0205 = Session["CardNo0205"].ToString().Trim();
        string strFromDate0205 = Session["FromDate0205"].ToString().Trim();
        string strToDate0205 = Session["ToDate0205"].ToString().Trim();
        string strPageIndex0205 = Session["PageIndex0205"].ToString().Trim();
        Response.Redirect("P060205000001.aspx?RadCheckFlg0205=" + RedirectHelper.GetEncryptParam(strRadCheckFlg0205) + " &FetchDate0205=" + RedirectHelper.GetEncryptParam(strFetchDate0205) + " &MerchDate0205=" + RedirectHelper.GetEncryptParam(strMerchDate0205) + " &Merch0205=" + RedirectHelper.GetEncryptParam(strMerch0205) + " &Id0205=" + RedirectHelper.GetEncryptParam(strId0205) + " &CardNo0205=" + RedirectHelper.GetEncryptParam(strCardNo0205) + " &FromDate0205=" + RedirectHelper.GetEncryptParam(strFromDate0205) + " &ToDate0205=" + RedirectHelper.GetEncryptParam(strToDate0205) + " &PageIndex0205=" + RedirectHelper.GetEncryptParam(strPageIndex0205) + "");
    }
}
