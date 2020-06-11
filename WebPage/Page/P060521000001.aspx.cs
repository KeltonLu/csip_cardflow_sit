//******************************************************************
//*  功能說明：自取改限掛大宗掛號單
//*  作    者：Linda
//*  創建日期：2010/12/31
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
using Framework.Common.Message;

public partial class P060521000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.txtSelfPickDate.Text=DateTime.Now.ToString("yyyy/MM/dd");
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Linda
    /// 創建時間:2010/12/31
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (this.txtSelfPickDate.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_06052100_000");
            return;
        }
        LoadReport();
    }

    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:Linda
    /// 創建時間:2010/12/31
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        try
        {
            // this.ReportViewer0521.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0521.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0521Report";
            // this.ReportViewer0521.Visible = true;

            //初始化報表參數,為Report View賦值參數

            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[1];

            // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("SelfPickDate", txtSelfPickDate.Text.Trim());
           
            // this.ReportViewer0521.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            // this.ReportViewer0521.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06052100_001");
        }
    }
}
