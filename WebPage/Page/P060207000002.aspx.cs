using System;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.Collections.Generic;
using System.IO;

public partial class P060207000002 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            try
            {
                string strMsgId = string.Empty;

                string strDailyCloseDate = string.Empty;

                if (Request.QueryString["DailyCloseDate"] != null)
                {
                    strDailyCloseDate = RedirectHelper.GetDecryptString(Request.QueryString["DailyCloseDate"]);
                }
                else
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');history.go(-1)");
                    return;
                }

                // 初始化報表參數
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("DailyCloseDate", strDailyCloseDate);

                string strServerPathFile =
                    this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0207Report(param, ref strServerPathFile, ref strMsgId);

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
    }

    /// <summary>
    /// 功能說明:回上頁
    /// 作    者:linda
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string strRadCheckFlg0207 = Session["RadCheckFlg0207"].ToString().Trim();
        string strStockDate0207 = Session["StockDate0207"].ToString().Trim();
        Response.Redirect("P060207000001.aspx?RadCheckFlg0207=" + RedirectHelper.GetEncryptParam(strRadCheckFlg0207) + " &StockDate0207=" + RedirectHelper.GetEncryptParam(strStockDate0207) + "");
    }
}
