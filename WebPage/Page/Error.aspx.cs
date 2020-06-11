//******************************************************************
//*  作    者：yangyu(rosicky)
//*  功能說明：錯誤畫面，重定向到登陸畫面
//*  創建日期：2009/10/01
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
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
using Framework.Common.Utility;
using Framework.Common.JavaScript;
using Framework.Common.Message;

public partial class Page_Error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strUrlLogon = System.Configuration.ConfigurationManager.AppSettings["LOGOUT"].ToString();

        if (string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "MsgID")))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("00_00000000_035") + "');var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.location.href='" + strUrlLogon + "';");
        }
        else
        {
            string strMsgID = RedirectHelper.GetDecryptString(this.Page, "MsgID");

            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.location.href='" + strUrlLogon + "';");
            
        }
    }
}
