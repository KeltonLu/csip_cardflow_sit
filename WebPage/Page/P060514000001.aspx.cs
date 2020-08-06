using System;
using System.Data;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using EntityLayer;
using Framework.Data.OM;
using BusinessRules;
using System.Collections.Generic;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.JavaScript;

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

        string strMsgID = string.Empty;

        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

        try
        {
            bool result = false;

            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>
                {
                    // 檔案產出日期 起
                    {"OstartDate", txtdateStart.Text.Trim().Equals("") ? "NULL" : txtdateStart.Text.Trim()},
                    
                    // 檔案產出日期 迄
                    {"OendDate", txtdateEnd.Text.Trim().Equals("") ? "NULL" : txtdateEnd.Text.Trim()},

                    {"Ouser", ((EntityAGENT_INFO)Session["Agent"]).agent_name}
                };

            if (rbCount.Checked)
            {
                //產生報表
                result = BR_Excel_File.CreateExcelFile_0514Report(param, ref strServerPathFile, ref strMsgID);
            }
            else if (rbResult.Checked)
            {
                DataTable dtOASA = new DataTable();

                if (ddlType.SelectedValue.Equals("1"))
                {
                    param.Add("flag", "1");

                    if (BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID))
                    {
                        param.Add("num", dtOASA.Rows.Count.ToString());
                    }
                    else
                    {
                        param.Add("num", "0");
                    }

                    if (ddlStatus.SelectedValue.Equals("0"))
                    {
                        result = BR_Excel_File.CreateExcelFile_0514_0Report(param, ref strServerPathFile, ref strMsgID);
                    }
                    else
                    {
                        param.Add("BLKCode", ddlStatus.SelectedValue);
                        result = BR_Excel_File.CreateExcelFile_0514_1Report(param, ref strServerPathFile, ref strMsgID);
                    }

                }
                else if (ddlType.SelectedValue.Equals("2"))
                {
                    param.Add("flag", "2");

                    if (BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID))
                    {
                        param.Add("num", dtOASA.Rows.Count.ToString());
                    }
                    else
                    {
                        param.Add("num", "0");
                    }

                    if (ddlStatus.SelectedValue.Equals("0"))
                    {
                        result = BR_Excel_File.CreateExcelFile_0514_2Report(param, ref strServerPathFile, ref strMsgID);
                    }
                    else
                    {
                        param.Add("BLKCode", ddlStatus.SelectedValue);
                        result = BR_Excel_File.CreateExcelFile_0514_3Report(param, ref strServerPathFile, ref strMsgID);
                    }
                }
            }

            if (result)
            {
                FileInfo fs = new FileInfo(strServerPathFile);
                Session["ServerFile"] = strServerPathFile;
                Session["ClientFile"] = fs.Name;
                string urlString = @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
            else
            {
                MessageHelper.ShowMessage(this, strMsgID);
            }
        }
        catch (Exception exp)
        {
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
