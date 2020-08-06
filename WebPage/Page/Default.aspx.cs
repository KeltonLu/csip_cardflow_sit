//******************************************************************
//*  作    者：宋戈
//*  功能說明：預設畫面.重新載入Session
//*  創建日期：2009/08/10
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using Framework.Common.Utility;

public partial class Page_Default : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strMsg = "";
        string strTicketID = RedirectHelper.GetDecryptString(this.Page, "TicketID");
        getSessionFromDB(strTicketID, ref strMsg);
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
        Framework.Common.Logging.Logging.NewLogAgent(eAgentInfo.agent_id);
        return true;
    }
}
