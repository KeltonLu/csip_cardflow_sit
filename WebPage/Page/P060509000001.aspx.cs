//******************************************************************
//*  功能說明：分行郵寄資料查詢
//*  作    者：JUN HU
//*  創建日期：2010/06/30
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
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


public partial class P060509000001 : PageBase
{
    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/01
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (txtMaildateStart.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return;
        }
        if (txtMaildateEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return;
        }
        if (txtCode.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_002");
            return;
        }
        try
        {
            // this.ReportViewer0509.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0509.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0509Report";
            // this.ReportViewer0509.Visible = true;

            //初始化報表參數,為Report View賦值參數
            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[3];

            // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("maildatefrom", txtMaildateStart.Text.Trim());

            // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("maildateto", txtMaildateEnd.Text.Trim());

            // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("branchid", txtCode.Text.Trim());
            // this.ReportViewer0509.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050400_003");

        }
    }

    /// <summary>
    /// 功能說明:根據分行代碼帶出分行名稱
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        //*分行代碼
        DataTable dtCode = new DataTable();
        
        //*分行代碼帶出分行名稱
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "9", ref dtCode))
        {
            DataRow[] rowCode = dtCode.Select("PROPERTY_CODE='" + this.txtCode.Text.Trim() + "'");
            if (rowCode != null && rowCode.Length > 0)
            {
                this.txtName.Text = rowCode[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                this.txtName.Text = string.Empty;
            }
        }
    }
}
