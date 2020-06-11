//******************************************************************
//*  功能說明：無法製卡檔查詢
//*  作    者：HAO CHEN
//*  創建日期：2010/07/05
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

public partial class P060515000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //*加載狀態選項
            BindFactory();
            //* 設定Tittle
            this.Page.Title = BaseHelper.GetShowText("06_06051500_000");
        }
    }

    /// <summary>
    /// 功能說明:加載製卡廠選項
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/05
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindFactory()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtState))
        {
            this.ddlFactory.DataSource = dtState;
            this.ddlFactory.DataTextField = "PROPERTY_NAME";
            this.ddlFactory.DataValueField = "PROPERTY_CODE";
            this.ddlFactory.DataBind();
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "0");
            ddlFactory.Items.Insert(0, li);
        }
    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/05
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        string strMStart = dpMakeStime.Text.Trim();
        string strMEnd = dpMakeEtime.Text.Trim();

        string strPStart = dpPostStime.Text.Trim();
        string strPEnd = dpPostEtime.Text.Trim();

        if (rdbMake.Checked)
        {
            if (string.IsNullOrEmpty(strMStart) && string.IsNullOrEmpty(strMEnd))
            {
                strMsgID = "06_05150000_002";
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(strMStart) && !string.IsNullOrEmpty(strMEnd))
                {
                    strMsgID = "06_05150000_004";
                    return false;
                }
                if (!string.IsNullOrEmpty(strMStart) && string.IsNullOrEmpty(strMEnd))
                {
                    strMsgID = "06_05150000_005";
                    return false;
                }
                if (!string.IsNullOrEmpty(strMStart) && !string.IsNullOrEmpty(strMEnd))
                {
                    //*製卡日期開始時間不能大于結束時間
                    if (!ValidateHelper.IsValidDate(strMStart, strMEnd, ref strMsgID))
                    {
                        strMsgID = "06_05150000_000";
                        return false;
                    }
                }
            }
        }

        if (rdbPost.Checked)
        {
            if (string.IsNullOrEmpty(strPStart) && string.IsNullOrEmpty(strPEnd))
            {
                strMsgID = "06_05150000_008";
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(strPStart) && !string.IsNullOrEmpty(strPEnd))
                {
                    strMsgID = "06_05150000_006";
                    return false;
                }
                if (!string.IsNullOrEmpty(strPStart) && string.IsNullOrEmpty(strPEnd))
                {
                    strMsgID = "06_05150000_007";
                    return false;
                }
                if (!string.IsNullOrEmpty(strPStart) && !string.IsNullOrEmpty(strPEnd))
                {
                    //*郵寄日期開始時間不能大于結束時間
                    if (!ValidateHelper.IsValidDate(strPStart, strPEnd, ref strMsgID))
                    {
                        strMsgID = "06_05150000_001";
                        return false;
                    }
                }
            }
        }
        return true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        if (!CheckCondition(ref strMsgID))
        {
            // this.ReportViewer0515.Visible = false;
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }

        try
        {
            // this.ReportViewer0515.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0515.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0515Report";
            // this.ReportViewer0515.Visible = true;

            //初始化報表參數,為Report View賦值參數
            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[8];

            //*製卡日期
            if (rdbMake.Checked)
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("MstatrDate", this.dpMakeStime.Text.Trim());
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("MendDate", this.dpMakeEtime.Text.Trim());

                // Paras[6] = new Microsoft.Reporting.WebForms.ReportParameter("CountS", dpMakeStime.Text.Trim());
                // Paras[7] = new Microsoft.Reporting.WebForms.ReportParameter("CountE", dpMakeEtime.Text.Trim());
            }
            else
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("MstatrDate", "NULL");
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("MendDate", "NULL");
            }

            //*郵寄日期
            if (rdbPost.Checked)
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("PstatrDate", this.dpPostStime.Text.Trim());
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("PendDate", this.dpPostEtime.Text.Trim());

                // Paras[6] = new Microsoft.Reporting.WebForms.ReportParameter("CountS", dpPostStime.Text.Trim());
                // Paras[7] = new Microsoft.Reporting.WebForms.ReportParameter("CountE", dpPostEtime.Text.Trim());
            }
            else
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("PstatrDate", "NULL");
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("PendDate", "NULL");
            }

            if (ddlFactory.SelectedValue.Equals("0"))
            {
                // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("Factory", "00");
                // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("FactoryName", BaseHelper.GetShowText("06_06051900_009"));
            }
            else
            {
                // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("Factory", this.ddlFactory.SelectedValue);
                // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("FactoryName", this.ddlFactory.SelectedItem.Text.Trim());
            }



            // this.ReportViewer0515.ServerReport.SetParameters(Paras);
        }
        catch (Exception exp)
        {
            // this.ReportViewer0515.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05150000_003");

        }
    }
}
