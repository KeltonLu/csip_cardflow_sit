//******************************************************************
//*  功能說明：自取改限掛大宗掛號單
//*  作    者：Linda
//*  創建日期：2010/12/31
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P060521000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.txtSelfPickDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Linda
    /// 創建時間:2010/12/31
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (this.txtSelfPickDate.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(this, "06_06052100_000");
            return;
        }

        LoadReport();
    }

    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:Linda
    /// 創建時間:2010/12/31
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
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"SelfPickDate", txtSelfPickDate.Text.Trim()}
            };


            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));
            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0521Report(param, ref strServerPathFile, ref strMsgId);

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
            // this.ReportViewer0521.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06052100_001");
        }
    }
}