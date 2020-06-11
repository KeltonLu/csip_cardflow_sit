//******************************************************************
//*  功能說明：手動大宗檔錯誤文件下載 
//*  作    者：zhen chen
//*  創建日期：2010/07/07
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
using Framework.Common.Utility;
using Framework.Common.Message;

public partial class P060302000002 : PageBase
{
    /// <summary>
    /// 功能說明:下載大宗檔錯誤信息
    /// 作    者:zhen chen
    /// 創建時間:2010/07/07
    /// 修改記錄: 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //解密
        string strPathFile = RedirectHelper.GetDecryptString(Request.QueryString["Path"]);
        string strClientFileName = RedirectHelper.GetDecryptString(Request.QueryString["FileName"]);
        try
        {
            this.Response.Clear();
            this.Response.Buffer = true;
            this.Session.CodePage = 950;
            this.Response.ContentType = "text/plain";
            this.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strClientFileName, System.Text.Encoding.UTF8));
            this.Response.TransmitFile(strPathFile);
        }
        catch (System.Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
        finally
        {
            Response.End();
        }
    }
}
