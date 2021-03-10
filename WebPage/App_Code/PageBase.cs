//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：系統角色維護
//*  創建日期：2009/07/10
//*  修改記錄：2020/04/20(Luke)

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer;
using Framework.Common;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Logging;

/// <summary>
/// 要做權限判斷的頁面基礎類別
/// </summary>
public class PageBase : System.Web.UI.Page
{
    protected long ProgramBeginRunTime;
    protected long programRunTime;

    /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
    [Serializable()]
    public struct structPageInfo
    {
        public string strPageCode;//*網頁FunctionID
        public string strPageName;//*網頁名稱
    }
    /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */
    public PageBase()
    {
        this.ProgramBeginRunTime = System.Environment.TickCount; //*程序开始运行时间
    }

    /// <summary>
    /// 填充页面上显示程序运行时间的文本控件
    /// </summary>
    /// <param name="literal">显示程序运行时间的文本控件</param>
    private void ProgramRunTime()
    {
        long ProgramEndRunTime = System.Environment.TickCount;
        programRunTime = ProgramEndRunTime - this.ProgramBeginRunTime;
        // jsBuilder.RegScript(this.Page, "var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent.location!=window.opener.location?window.opener.parent:window.opener.opener.parent:window;local.document.all.runtime.innerText='" + programRunTime.ToString() + " 毫秒';");
        jsBuilder.RegScript(this.Page, "window.parent.postMessage({ func: 'ProgramRunTime', data: '" + programRunTime.ToString() + " 毫秒' }, '*');");
    }

    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        //* 如果Session中含有主機的SessionID,則發送電文關閉之,并清空網站中存的主機SessionID
        //MainFrameInfo.ClearSession();
        //* 顯示頁面執行時間
        this.ProgramRunTime();

        base.Render(writer);
    }

    /// <summary>
    /// 验证用户是否登陆或登陆超时，重载二
    /// </summary>
    /// <param name="session"></param>
    public void ValidateLogin(object session)
    {
        if (session == null)
        {
            Response.Write("<script language='javascript'>window.location.href='/Error.aspx?errorId=1'</script>");
            Response.End();
        }
    }


    /// <summary>
    /// 頁面的Function_ID
    /// </summary>
    private String _Function_ID;


    public string M_Function_ID
    {
        get { return this._Function_ID; }
        set
        {
            //this._Function_ID = value; 
        }
    }

    /// <summary>
    /// 頁面的Function_Name
    /// </summary>
    private String _Function_Name;


    public string M_Function_Name
    {
        get { return this._Function_Name; }
        set
        {
            //this._Function_Name = value; 
        }
    }

    /// <summary>
    /// 黨頁面加載時
    /// </summary>
    /// <param name="e">事件參數</param>
    protected override void OnLoad(EventArgs e)
    {
        //檢核操作瀏覽器功能
        CensorPage();

        //* 取頁面的功能ID號(Function_ID)
        /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
        structPageInfo sPageInfo = new structPageInfo();
        /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */

        string strUrlError = UtilHelper.GetAppSettings("Error");
        string strMsg = "";
        #region 判斷Session是否存在及重新取Session值

        //* 判斷Session是否存在
        if (Session == null || HttpContext.Current.Session == null || this.Session["Agent"] == null)
        {
            //* Session不存在時，判斷TicketID是否存在
            if (string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "TicketID")))
            {
                //* TicketID不存在，顯示重新登入訊息，轉向重新登入畫面
                //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                //return;

                strMsg = "00_00000000_035";
                //*TicketID不存在，顯示重新登入訊息，轉向重新登入畫面
                Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg));
            }
            else
            {
                //* TicketID存在時，
                //* 取TicketID
                string strTicketID = RedirectHelper.GetDecryptString(this.Page, "TicketID");
                //* 以TicketID到DB中取Session資料。

                if (!getSessionFromDB(strTicketID, ref strMsg))
                {
                    //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                    //return;
                    Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg));

                }
            }
        }
        #endregion 判斷Session是否存在及重新取Session值
        #region 判斷用戶是否有使用該頁面的權限
        //* 取頁面的功能ID號(Function_ID)
        this._Function_ID = "88888888";
        string strPath = this.Server.MapPath(this.Request.Url.AbsolutePath).ToUpper();
        if (strPath.IndexOf("DEFAULT.ASPX") == -1)
        {
            PageAction pgaNow = PopedomManager.MainPopedomManager.PageSettings[strPath];
            this._Function_ID = pgaNow.FunctionID;   //* 頁面的功能ID號
           /*Session中增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
            sPageInfo.strPageCode = pgaNow.FunctionID;
            this.Session["PageInfo"] = sPageInfo;
            Logging.UpdateLogAgentFunctionId(sPageInfo.strPageCode);
            /*Session中增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */
            strMsg = "00_00000000_021";
            string strUrlLogon = UtilHelper.GetAppSettings("LOGOUT");
                        
            if (this.Session["Agent"] == null || ((EntityAGENT_INFO)this.Session["Agent"]).dtfunction == null ||
                ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows.Count <= 0)
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                return;
            }
            else
            {
                bool blCanUseAction = false;
                //* 檢查用戶的權限列表中是否存在當前頁面的Funcion_ID;
                for (int intLoop = 0; intLoop < ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows.Count; intLoop++)
                {
                    if (((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Function_ID"].ToString() == this._Function_ID)
                    {
                        this._Function_Name = ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Function_Name"].ToString();
                        blCanUseAction = true;

                        if (((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Action_List"].ToString() != "")
                        {
                            DataTable dtblAction = Framework.Common.Serialization.XMLSerialization<DataTable>.XmlStringToObject(((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Action_List"].ToString());

                        

                            for (int i = 0; i < dtblAction.Rows.Count; i++)
                            {
                                switch(dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString().Substring(0,3).ToLower())
                                {
                                    case "btn": //按鈕
                                        Button btnAction = (Button)this.Page.FindControl(dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString());
                                        SetControl(btnAction, dtblAction.Rows[i]["HadRight"].ToString());
                                        break;
                                    case "chk": //複選框
                                        CheckBox chkAction = (CheckBox)this.Page.FindControl(dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString());
                                        SetControl(chkAction, dtblAction.Rows[i]["HadRight"].ToString());
                                        break;
                                    case "txt": //輸入框
                                        TextBox txtAction = (TextBox)this.Page.FindControl(dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString());
                                        SetControl(txtAction, dtblAction.Rows[i]["HadRight"].ToString());
                                        break;
                                    case "ddl": //下拉Bar
                                        DropDownList ddlAction = (DropDownList)this.Page.FindControl(dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString());
                                        SetControl(ddlAction, dtblAction.Rows[i]["HadRight"].ToString());
                                        break;
                                }                               
                            }
                        }

                        break;
                    }
                }

                //* 沒有權限使用該功能ID
                if (!blCanUseAction)
                {
                    //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                    strMsg = "00_00000000_025";
                    Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg));

                    return;
                }
            }
        }
        #endregion

        base.OnLoad(e);
    }

    private void SetControl(System.Web.UI.WebControls.WebControl btnAction, string strHadRight)
    {
        if (btnAction != null)
        {
            if (strHadRight == "Y")
            {
                if (btnAction.Enabled)
                {
                    btnAction.Enabled = true;
                }
            }
            else
            {
                if (btnAction.Enabled)
                {
                    btnAction.Enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// 以TicketID到DB中取Session資料。
    /// </summary>
    /// <param name="strTicketID"></param>
    private bool getSessionFromDB(String strTicketID, ref string strMsg)
    {
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();

        EntitySESSION_INFO eSessionInfo = new EntitySESSION_INFO();

        eSessionInfo.TICKET_ID = strTicketID;

        //* 取Session訊息
        if (!BRSESSION_INFO.Search(eSessionInfo, ref eAgentInfo, ref strMsg))
        {
            return false;
        }

        //* 重新回覆當前Session的訊息
        this.Session["Agent"] = eAgentInfo;
        Logging.NewLogAgent(eAgentInfo.agent_id);

        //* 刪除DB中的TicketID對應的Session訊息
        if (!BRSESSION_INFO.Delete(eSessionInfo, ref strMsg))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 取得左邊多少多少位(因為.NET Substring 對於不足的取有缺陷
    /// </summary>
    /// <param name="strString">需要Left的字符串</param>
    /// <param name="iCount">需要的位數</param>
    /// <returns></returns>
    public static string StringLeft(string strString, int iCount)
    {
        string strTmp = strString.Trim().PadLeft(iCount, Convert.ToChar(" "));
        return strTmp.Substring(0, iCount).Trim();
    }

    /// <summary>
    /// 檢核操作瀏覽器
    /// (避免分頁多開導致Session異常)
    /// </summary>
    protected void CensorPage()
    {
        string strUrlErrorIframe = UtilHelper.GetAppSettings("ERROR_IFRAME");

        try
        {
            if (!IsPostBack)
            {
                String usrBrowser = Request.Browser.Browser;
                String GUID = Guid.NewGuid().ToString();

                HttpContext.Current.Session["usrBrowser"] = usrBrowser;
                this.ViewState["usrBrowser"] = usrBrowser;

                HttpContext.Current.Session["usrGUID"] = GUID;
                this.ViewState["usrGUID"] = GUID;
            }
            else
            {
                //檢核操作瀏覽器是否相符
                if (this.ViewState["usrBrowser"].Equals(Session["usrBrowser"]))
                {
                    //檢核KEY是否相符
                    if (!Session["usrGUID"].Equals(this.ViewState["usrGUID"]))
                    {
                        //2021/02/17_Ares_Stanley-增加false參數
                        Response.Redirect(strUrlErrorIframe, false);
                        //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.location.href='" + strUrlError2 + "';");
                        return;
                    }
                }
            }
        }
        catch (ThreadAbortException taex)
        {
            Logging.Log(taex, LogLayer.UI);
            Response.Redirect(strUrlErrorIframe);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            Response.Redirect(strUrlErrorIframe);
        }
    }

}