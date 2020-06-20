//******************************************************************
//*  功能說明：退件日報表
//*  作    者：HAO CHEN
//*  創建日期：2010/07/12
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using System.Configuration;
using System.IO;

public partial class P060516000001 : PageBase
{
    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        if (string.IsNullOrEmpty(dateFrom.Text.Trim()))
        {
            strMsgID = "06_05160000_000";
            dateFrom.Focus();
            return false;
        }

        return true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgId = string.Empty;
        if (!CheckCondition(ref strMsgId))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgId) + "')");
            return;
        }

        try
        {
            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            String Operaction = this.dateFrom.Text.Trim();
            param.Add("Operaction", Operaction);

            string strServerPathFile =
                this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0516Report(param, ref strServerPathFile, ref strMsgId);

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
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05160000_003");
        }
    }
}