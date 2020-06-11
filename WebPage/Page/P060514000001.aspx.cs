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
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Message;
using EntityLayer;
using Framework.Data.OM;
using BusinessRules;

public partial class P060514000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindControl();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        int intParas = 0;
        if (this.txtdateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_06051800_000");
            txtdateStart.Focus();
            return;
        }
        if (txtdateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_06051800_000");
            txtdateEnd.Focus();
            return;
        }

        try
        {
            if (rbCount.Checked)
            {
                // this.ReportViewer0514.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
                // this.ReportViewer0514.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0514Report";
                // this.ReportViewer0514.Visible = true;

                //初始化報表參數,為Report View賦值參數

                // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[3];

                if (txtdateStart.Text.Trim().Equals(""))
                {
                    // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("OstartDate", "NULL");
                }
                else
                {
                    // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("OstartDate", txtdateStart.Text.Trim());
                }
                if (txtdateEnd.Text.Trim().Equals(""))
                {
                    // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("OendDate", "NULL");
                }
                else
                {
                    // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("OendDate", txtdateEnd.Text.Trim());
                }

                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("Ouser", ((EntityAGENT_INFO)Session["Agent"]).agent_name);

                // this.ReportViewer0514.ServerReport.SetParameters(Paras);
            }
            else if (rbResult.Checked)
            {
                // this.ReportViewer0514.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
                if (ddlType.SelectedValue.Equals("1"))
                {
                    if (ddlStatus.SelectedValue.Equals("0"))
                    {
                        // this.ReportViewer0514.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0514_0Report";
                        intParas=5;
                    }
                    else
                    {
                        // this.ReportViewer0514.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0514_1Report";
                        intParas=6;
                    }

                }
                else if (ddlType.SelectedValue.Equals("2"))
                {
                    if (ddlStatus.SelectedValue.Equals("0"))
                    {
                        // this.ReportViewer0514.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0514_2Report";
                        intParas=5;
                    }
                    else
                    {
                        // this.ReportViewer0514.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "0514_3Report";
                        intParas=6;
                    }
                }
                // this.ReportViewer0514.Visible = true;

                string strMsgID = "";
                DataTable dtOASA = new DataTable();
                //初始化報表參數,為Report View賦值參數

                // Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[intParas];

                if (txtdateStart.Text.Trim().Equals(""))
                {
                    // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("OstartDate", "NULL");
                }
                else
                {
                    // Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("OstartDate", txtdateStart.Text.Trim());
                }
                if (txtdateEnd.Text.Trim().Equals(""))
                {
                    // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("OendDate", "NULL");
                }
                else
                {
                    // Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("OendDate", txtdateEnd.Text.Trim());
                }

                // Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("Ouser", ((EntityAGENT_INFO)Session["Agent"]).agent_name);

                if (ddlType.SelectedValue.Equals("1"))
                {
                    // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("flag", "1");

                    if (BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID))
                    {
                        // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("num", dtOASA.Rows.Count.ToString());
                    }
                    else
                    {
                        // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("num", "0");
                    }
                }
                else if (ddlType.SelectedValue.Equals("2"))
                {
                    // Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("flag", "2");

                    if (BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID))
                    {
                        // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("num", dtOASA.Rows.Count.ToString());
                    }
                    else
                    {
                        // Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("num", "0");
                    }
                }

                if (!ddlStatus.SelectedValue.Equals("0"))
                {
                    // Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("BLKCode", ddlStatus.SelectedValue);
                }
                // this.ReportViewer0514.ServerReport.SetParameters(Paras);
            }
        }
        catch (Exception exp)
        {
            // this.ReportViewer0514.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06051800_001");
        }
    }

    private void BindControl()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "30", ref dtState))
        {
            this.ddlStatus.DataSource = dtState;
            this.ddlStatus.DataTextField = "PROPERTY_NAME";
            this.ddlStatus.DataValueField = "PROPERTY_CODE";
            this.ddlStatus.DataBind();
        }

        ListItem lis = new ListItem(BaseHelper.GetShowText("06_06051400_007"), "1");
        ListItem lif = new ListItem(BaseHelper.GetShowText("06_06051400_008"), "2");
        ddlType.Items.Add(lis);
        ddlType.Items.Add(lif);
        ddlType.Items[0].Selected = true;
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:JUN HU
    /// 創建時間:2010/07/15
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_FileCode, Operator.Equal, DataTypeUtils.String, "3");
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_Success_Flag, Operator.Equal, DataTypeUtils.String, ddlType.SelectedValue);
        if (!ddlStatus.SelectedValue.Equals("0"))
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_NBLKCode, Operator.Equal, DataTypeUtils.String, ddlStatus.SelectedValue);
        }
        if (this.txtdateStart.Text.Trim() != "" && this.txtdateEnd.Text.Trim() == "")
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtdateStart.Text.Trim());
        }
        if (this.txtdateStart.Text.Trim() == "" && this.txtdateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.LessThanEqual, DataTypeUtils.String, this.txtdateEnd.Text.Trim());
        }
        if (this.txtdateStart.Text.Trim() != "" && this.txtdateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.LessThanEqual, DataTypeUtils.String, this.txtdateEnd.Text.Trim());
            sqlhelp.AddCondition(Entity_CancelOASAUd.M_File_Date, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtdateStart.Text.Trim());
        }
        return sqlhelp.GetFilterCondition();
    }
}
