//******************************************************************
//*  功能說明：扣卡明細查詢
//*  作    者：JUN HU
//*  創建日期：2010/06/23
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P060510000002 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadReport();
    }

    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        string strMsgId = string.Empty;
        
        try
        {
             // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();
            
            // 製卡日期
            param.Add("indatefrom", Request.QueryString["indatefrom"].Equals("") ? "NULL" : RedirectHelper.GetDecryptString(Request.QueryString["indatefrom"]));
            param.Add("indateto", Request.QueryString["indateto"].Equals("") ? "NULL" : RedirectHelper.GetDecryptString(Request.QueryString["indateto"]));
            
            // 扣卡日期
            param.Add("maildate", Request.QueryString["maildate"].Equals("") ? "NULL" : RedirectHelper.GetDecryptString(Request.QueryString["maildate"]));
            param.Add("maildate1", Request.QueryString["maildate1"].Equals("") ? "NULL" : RedirectHelper.GetDecryptString(Request.QueryString["maildate1"]));
            

            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0510Report(param, ref strServerPathFile, ref strMsgId);

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
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_001"));
            return;
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string strIndate1FromDate0510 = Session["Indate1FromDate0510"].ToString().Trim();
        string strIndate1ToDate0510 = Session["Indate1ToDate0510"].ToString().Trim();
        string strFromDate0510 = Session["FromDate0510"].ToString().Trim();
        string strToDate0510 = Session["ToDate0510"].ToString().Trim();
        string strPageIndex0510 = Session["PageIndex0510"].ToString().Trim();
        string strFactory0510 = Session["Factory0510"].ToString().Trim();

        Response.Redirect("P060510000001.aspx?Indate1FromDate0510=" + RedirectHelper.GetEncryptParam(strIndate1FromDate0510) + " &Indate1ToDate0510=" + RedirectHelper.GetEncryptParam(strIndate1ToDate0510) + " &FromDate0510=" + RedirectHelper.GetEncryptParam(strFromDate0510) + " &ToDate0510=" + RedirectHelper.GetEncryptParam(strToDate0510) + " &Factory0510=" + RedirectHelper.GetEncryptParam(strFactory0510) + " &PageIndex0510=" + RedirectHelper.GetEncryptParam(strPageIndex0510) + "");
    }
}
