//******************************************************************
//*  作    者：宋戈
//*  功能說明：Session自刷新頁面
//*  創建日期：2009/11/17
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Web.UI;

public partial class Page_SessionRefresher : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentTime.Text = DateTime.Now.ToLongTimeString();
    }
    protected void ScriptManager1_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
    {
        this.ScriptManager1.AsyncPostBackErrorMessage = "";
    }
}
