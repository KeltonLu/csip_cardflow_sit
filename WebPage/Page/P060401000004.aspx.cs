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
using Framework.Common.Message;
using Framework.Common.JavaScript;
using BusinessRules;
using EntityLayer;
using Framework.Data.OM;

public partial class P060401000004 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            try
            {
                string strCardNo = "";
                string strId = "";
                string strAction = "";

                if (Session["CardNo"] != null && Session["Id"] != null && Session["Action"] != null)
                {
                    strCardNo = Session["CardNo"].ToString();
                    strId = Session["Id"].ToString();
                    strAction = Session["Action"].ToString();
                }
                else
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06040100_028") + "');history.go(-1)");
                    return;
                }

                // this.ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
                // this.ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0401Report";
                // this.ReportViewer1.Visible = true;
                //初始化報表參數,為Report View賦值參數
                // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[3];
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("cardno", strCardNo);
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("id", strId);
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("action", strAction);
                // this.ReportViewer1.ServerReport.SetParameters(Paras);
            }
            catch (Exception exp)
            {
                MessageHelper.ShowMessage(this, "06_06040100_031" + exp.Message);
            }
        }
    }
}
