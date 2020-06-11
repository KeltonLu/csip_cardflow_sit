//******************************************************************
//*  功能說明：退件連絡報表
//*  作    者：JUN HU
//*  創建日期：2010/07/02
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
using EntityLayer;
using Framework.Data.OM;
using Framework.Common.Message;
using Framework.Common.Logging;
using BusinessRules;

public partial class P060518000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindState();
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/02
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        if (txtMaildateStart.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051800_000");
            txtMaildateStart.Focus();
            return;
        }
        if (txtMaildateEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051800_000");
            txtMaildateEnd.Focus();
            return;
        }

        if (!ValidateHelper.IsValidDate(txtMaildateStart.Text.Trim(), DateTime.Now.ToString("yyyy/MM/dd"), ref strMsgID) || !ValidateHelper.IsValidDate(txtMaildateEnd.Text.Trim(), DateTime.Now.ToString("yyyy/MM/dd"), ref strMsgID))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051800_002");
            return;
        }


        try
        {
            // this.ReportViewer0518.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0518.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0518Report";
            // this.ReportViewer0518.Visible = true;

            //初始化報表參數,為Report View賦值參數

            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[3];

            // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("datefrom", txtMaildateStart.Text.Trim());

            // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("dateto", txtMaildateEnd.Text.Trim());

            // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("backtype", ddlState.SelectedValue);

            // this.ReportViewer0518.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            // this.ReportViewer0518.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06051800_001");
        }
    }

    /// <summary>
    /// 功能說明:加載狀態選項

    /// 作    者:JUN HU
    /// 創建時間:2010/07/02
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindState()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "28", ref dtState))
        {
            this.ddlState.DataSource = dtState;
            this.ddlState.DataTextField = "PROPERTY_NAME";
            this.ddlState.DataValueField = "PROPERTY_CODE";
            this.ddlState.DataBind();
        }
    }
}
