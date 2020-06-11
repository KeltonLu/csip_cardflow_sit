//******************************************************************
//*  功能說明：址更重寄異動記錄查詢
//*  作    者：Linda
//*  創建日期：2010/09/27
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
using CSIPCommonModel.EntityLayer;

public partial class P060520000001 : PageBase
{
    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtUpdStart.Text=DateTime.Now.ToString("yyyy/MM/dd");
            txtUpdEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Linda
    /// 創建時間:2010/09/27
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtId.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        if (txtUpdStart.Text.Trim().Equals("")||txtUpdEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06052000_000");
            return;
        }
        if (!this.txtMailNo.Text.Trim().Equals(""))
        {
            if (this.txtMailNo.Text.Trim().Length < 20)
            {
                //*掛號號碼小於二十碼
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06052000_001");
                return;
            }
        }
        if (!this.txtId.Text.Trim().Equals(""))
        {
            if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
            {
                //*身份證字號驗證不通過
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06052000_002");
                return;
            }
        }

        LoadReport();
    }

    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:Linda
    /// 創建時間:2010/09/27
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        try
        {
            // this.ReportViewer0520.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0520.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0520Report";
            // this.ReportViewer0520.Visible = true;

            //初始化報表參數,為Report View賦值參數

            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[4];

            if (txtUpdStart.Text.Trim().Equals(""))
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("strUpdFrom", "NULL");
            }
            else
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("strUpdFrom", txtUpdStart.Text.Trim());
            }
            if (txtUpdEnd.Text.Trim().Equals(""))
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("strUpdTo", "NULL");
            }
            else
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("strUpdTo", txtUpdEnd.Text.Trim());
            }
            if (txtMailNo.Text.Trim().Equals(""))
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("strMailNo", "NULL");
            }
            else
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("strMailNo", txtMailNo.Text.Trim());
            }
            if (txtId.Text.Trim().Equals(""))
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("strId", "NULL");
            }
            else
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("strId", txtId.Text.Trim());
            }

            // this.ReportViewer0520.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            // this.ReportViewer0520.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06052000_003");
        }
    }
}
