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

public partial class P060207000002 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            try
            {
                string strDailyCloseDate = string.Empty;

                if (Request.QueryString["DailyCloseDate"] != null)
                {
                    strDailyCloseDate = RedirectHelper.GetDecryptString(Request.QueryString["DailyCloseDate"]);
                }
                else
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');history.go(-1)");
                    return;
                }

                // this.ReportViewer0207.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
                // this.ReportViewer0207.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0207Report";
                // this.ReportViewer0207.Visible = true;
                //初始化報表參數,為Report View賦值參數
                // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[1];
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("DailyCloseDate", strDailyCloseDate);
                // this.ReportViewer0207.ServerReport.SetParameters(Paras);
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
        string strRadCheckFlg0207 = Session["RadCheckFlg0207"].ToString().Trim();
        string strStockDate0207 = Session["StockDate0207"].ToString().Trim();
        Response.Redirect("P060207000001.aspx?RadCheckFlg0207=" + RedirectHelper.GetEncryptParam(strRadCheckFlg0207) + " &StockDate0207=" + RedirectHelper.GetEncryptParam(strStockDate0207) + "");
    }
}
