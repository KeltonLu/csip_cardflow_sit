using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using EntityLayer;
using Framework.Data.OM;
using BusinessRules;
using CSIPCommonModel.EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Utility;

public partial class P060513000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindControl();
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/13
    /// 修改記錄:2020/07/10 area Luke Report to Excel 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

        string strMsgId = string.Empty;
        
        try
        {
            if (rbCount.Checked)
            {
                // 初始化報表參數
                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    // 檔案產出日期 起
                    {"OstartDate", txtdateStart.Text.Trim().Equals("") ? "NULL" : txtdateStart.Text.Trim()},
                    
                    // 檔案產出日期 迄
                    {"OendDate", txtdateEnd.Text.Trim().Equals("") ? "NULL" : txtdateEnd.Text.Trim()},
                    
                    {"Ouser", ((EntityAGENT_INFO)Session["Agent"]).agent_name}
                };
                
                string strServerPathFile =
                    this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());


                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0513Report(param, ref strServerPathFile, ref strMsgId);
                
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
                    MessageHelper.ShowMessage(this, strMsgId);
                }
            }
            
            else if (rbResult.Checked)
            {
                // 初始化報表參數
                Dictionary<string, string> param = new Dictionary<string, string>();

                string strServerPathFile =
                    this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());


                //產生報表狀態
                bool result = false;

                string strMsgID = "";
                DataTable dtOASA = new DataTable();
                //初始化報表參數,為Report View賦值參數


                param.Add("OstartDate", txtdateStart.Text.Trim().Equals("") ? "NULL" : txtdateStart.Text.Trim());
                param.Add("OendDate", txtdateStart.Text.Trim().Equals("") ? "NULL" : txtdateEnd.Text.Trim());
                param.Add("Ouser", ((EntityAGENT_INFO)Session["Agent"]).agent_name);

                if (ddlStatus.SelectedValue.Equals("1"))
                {
                    param.Add("flag", "1");
                    param.Add("num",
                        BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID)
                            ? dtOASA.Rows.Count.ToString()
                            : "0");
                }
                else if (ddlStatus.SelectedValue.Equals("2"))
                {
                    param.Add("flag", "2");


                    param.Add("num",
                        BRM_Report.SearchOASAG(GetFilterCondition(), ref dtOASA, ref strMsgID)
                            ? dtOASA.Rows.Count.ToString()
                            : "0");
                }

                if (ddlStatus.SelectedValue.Equals("1"))
                {
                    result = BR_Excel_File.CreateExcelFile_0513_0Report(param, ref strServerPathFile, ref strMsgId);

                }
                else if (ddlStatus.SelectedValue.Equals("2"))
                {
                    result = BR_Excel_File.CreateExcelFile_0513_2Report(param, ref strServerPathFile, ref strMsgId);
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
                    MessageHelper.ShowMessage(this, strMsgId);
                }

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
        ListItem lis = new ListItem(BaseHelper.GetShowText("06_06051300_004"), "1");
        ListItem lif = new ListItem(BaseHelper.GetShowText("06_06051300_005"), "2");
        ddlStatus.Items.Add(lis);
        ddlStatus.Items.Add(lif);
        ddlStatus.Items[0].Selected = true;
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:JUN HU
    /// 創建時間:2010/07/14
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_FileCode, Operator.In, DataTypeUtils.String, "'0','1','2'");
        sqlhelp.AddCondition(Entity_CancelOASAUd.M_Success_Flag, Operator.Equal, DataTypeUtils.String, ddlStatus.SelectedValue);
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
