using System;
using System.Collections.Generic;
using System.IO;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class Page_P060209000003 : System.Web.UI.Page
{
    #region params

    public string m_Source
    {
        get { return ViewState["m_Source"] as string; }
        set { ViewState["m_Source"] = value; }
    }

    public string m_File
    {
        get { return ViewState["m_File"] as string; }
        set { ViewState["m_File"] = value; }
    }

    public string m_Date
    {
        get { return ViewState["m_Date"] as string; }
        set { ViewState["m_Date"] = value; }
    }

    public string m_Visible
    {
        get { return ViewState["m_Visible"] as string; }
        set { ViewState["m_Visible"] = value; }
    }

    public string m_Status
    {
        get { return ViewState["m_Status"] as string; }
        set { ViewState["m_Status"] = value; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        m_Source = string.Empty;
        m_File = string.Empty;
        m_Date = string.Empty;
        m_Status = string.Empty;

        try
        {
            if (Request.QueryString["Source"] != null && Request.QueryString["File"] != null &&
                Request.QueryString["Date"] != null && Request.QueryString["Visible"] != null &&
                Request.QueryString["Status"] != null)
            {
                m_Source = RedirectHelper.GetDecryptString(Request.QueryString["Source"]);
                m_File = RedirectHelper.GetDecryptString(Request.QueryString["File"]);
                m_Date = RedirectHelper.GetDecryptString(Request.QueryString["Date"]);
                m_Visible = RedirectHelper.GetDecryptString(Request.QueryString["Visible"]);
                m_Status = RedirectHelper.GetDecryptString(Request.QueryString["Status"]);
            }
            else
            {
                jsBuilder.RegScript(this.Page,
                    "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');history.go(-1)");
                return;
            }


            string strMsgId = string.Empty;

            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            param.Add("strSource", m_Source);
            param.Add("strFile", m_File);
            param.Add("strDate", m_Date);
            

            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0209Report(param, ref strServerPathFile, ref strMsgId);

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
        catch (Exception exp)
        {
            MessageHelper.ShowMessage(this, "06_06020701_000" + exp.Message);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060209000002.aspx?date=" + RedirectHelper.GetEncryptParam(m_Date) + "&source="
                          + RedirectHelper.GetEncryptParam(m_Source) + "&file=" +
                          RedirectHelper.GetEncryptParam(m_File) + "&visible="
                          + RedirectHelper.GetEncryptParam(m_Visible) + "&Status=" +
                          RedirectHelper.GetEncryptParam(m_Status) + "", false);
    }
}