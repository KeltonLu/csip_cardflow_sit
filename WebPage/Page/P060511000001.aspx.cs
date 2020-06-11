//******************************************************************
//*  功能說明：更改寄送方式記錄查詢

//*  作    者：JUN HU
//*  創建日期：2010/06/24
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

public partial class P060511000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ckUrgency_Flg.Text = BaseHelper.GetShowText("06_06051000_014");
            txtBackdateStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            txtBackdateEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            BindKind();
        }
    }

    /// <summary>
    /// 功能說明:綁定取卡方式
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void BindKind()
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();
        custddlType.Items.Clear();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKindName))
        {
            foreach (DataRow dr in dtKindName.Rows)
            {
                ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                custddlType.Items.Add(liTmp);
            }
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "");
            custddlType.Items.Insert(0, li);
        }

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
    /// 功能說明:加載報表
    /// 作    者:JUN HU
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        try
        {
            // this.ReportViewer0511.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            // this.ReportViewer0511.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0511Report";
            // this.ReportViewer0511.Visible = true;

            //初始化報表參數,為Report View賦值參數

            // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[6];

            if (txtBackdateStart.Text.Trim().Equals(""))
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("indatefrom", "NULL");
            }
            else
            {
                // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("indatefrom", txtBackdateStart.Text.Trim());
            }
            if (txtBackdateEnd.Text.Trim().Equals(""))
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("indateto", "NULL");
            }
            else
            {
                // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("indateto", txtBackdateEnd.Text.Trim());
            }
            if (custddlType.SelectedItem.Value.Trim().Equals(""))
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("newway", "00");
                // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "00");
            }
            else
            {
                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("newway", custddlType.SelectedValue);

                switch (custddlType.SelectedValue.ToString().Trim())
                {
                    case "0":
                        // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "1");
                        break;
                    case "1":
                        // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "0"); 
                        break;
                    case "3":
                        // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "2"); 
                        break;
                    case "4":
                        // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "3"); 
                        break;
                    case "10":
                        // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "6"); 
                        break;
                    case "11":
                        // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "5");
                        break;
                    default:
                        // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("Enditem", "7"); 
                        break;
                }
            }
            if (ddlFactory.SelectedItem.Value.Trim().Equals("0"))
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("factory", "00");
            }
            else
            {
                // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("factory", ddlFactory.SelectedValue);
            }
            if (ckUrgency_Flg.Checked)
            {
                // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("UrgencyFlg", "1");
            }
            else
            {
                // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("UrgencyFlg", "NULL");
            }
            // this.ReportViewer0511.ServerReport.SetParameters(Paras);
        }
        catch(Exception exp)
        {
            // this.ReportViewer0511.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050400_003");
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/21
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (txtBackdateStart.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            MessageHelper.ShowMessage(this, "06_06051200_000");
            return;
        }
        if (txtBackdateEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            MessageHelper.ShowMessage(this, "06_06051200_000");
            return;
        }
        LoadReport();
    }
}
