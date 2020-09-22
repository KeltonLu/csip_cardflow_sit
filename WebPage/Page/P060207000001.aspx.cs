//******************************************************************
//*  功能說明：綜合資料處理UI層
//*  作    者：Linda
//*  創建日期：2010/06/17
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P060207000001 : PageBase

{
    string strLastCloseDate = string.Empty;
    string strCloseDate = string.Empty;

    public DataTable m_dtCardDailyStockInfo
    {
        get { return ViewState["m_dtCardDailyStockInfo"] as DataTable; }
        set { ViewState["m_dtCardDailyStockInfo"] = value; }
    }

    /// <summary>
    /// 功能說明:頁面加載事件
    /// 作    者:Linda
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.btnClose.Text = BaseHelper.GetShowText("06_06020700_004");
            this.btnCancelClose.Text = BaseHelper.GetShowText("06_06020700_005");
            this.radDailyClose.Text = BaseHelper.GetShowText("06_06020700_007");
            this.radStockInfo.Text = BaseHelper.GetShowText("06_06020700_008");
            this.InitDate();
            if (!string.IsNullOrEmpty(Request.QueryString["RadCheckFlg0207"]) && !string.IsNullOrEmpty(Request.QueryString["StockDate0207"]))
            {
                if (RedirectHelper.GetDecryptString(Request.QueryString["RadCheckFlg0207"].Trim()) == "1")
                {
                    this.radStockInfo.Checked = true;
                }
                else
                {
                    this.radDailyClose.Checked = true;
                }
                this.dpCloseDate.Text = RedirectHelper.GetDecryptString(Request.QueryString["StockDate0207"].ToString().Trim());
            }

        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected Boolean chkCond(ref String strDailyCloseDate)
    {
        if (!this.radStockInfo.Checked)
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020700_012") + "');");
            return false;
        }
        DataTable dtCloseInfo = new DataTable();
        strDailyCloseDate = this.dpCloseDate.Text.Trim();

        if (this.radStockInfo.Checked)
        {
            this.Session["RadCheckFlg0207"] = "1";
        }
        else
        {
            this.Session["RadCheckFlg0207"] = "0";
        }
        this.Session["StockDate0207"] = strDailyCloseDate;

        if (strDailyCloseDate.Equals(string.Empty))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020700_007") + "');");
            return false;
        }
        else
        {
            if (BRM_CardDailyClose.GetCloseInfo(strDailyCloseDate, ref dtCloseInfo))
            {
                if (dtCloseInfo.Rows.Count > 0)
                {
                    strDailyCloseDate = RedirectHelper.GetEncryptParam(strDailyCloseDate);
                    return true;
                }
                else
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020700_008") + "');");
                    return false;
                }

            }
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020700_008") + "');");
                return false;
            }
        }
    }
    /// <summary>
    /// 功能說明:卡片自取庫存盤點表查詢
    /// 作    者:Linda
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// Ares JaJa 2020/09/03 原功能整合移至列印功能，調整為查詢顯示
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        String strDailyCloseDate = "";
        if (chkCond(ref strDailyCloseDate))
            Response.Redirect("P060207000002.aspx?DailyCloseDate=" + strDailyCloseDate + "&Type=Search");
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        String strDailyCloseDate = "";
        if (chkCond(ref strDailyCloseDate))
            Response.Redirect("P060207000002.aspx?DailyCloseDate=" + strDailyCloseDate + "&Type=Print");
    }
    /// <summary>
    /// 功能說明:日結操作
    /// 作    者:Linda
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClose_Click(object sender, EventArgs e)
    {
        strCloseDate=this.lblNowCloseDate.Text.ToString().Trim();;

        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020700_000");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");


        if (BRM_CardDailyClose.InsertDailyClose(strCloseDate))
        {
            if (BRM_CardDailyClose.InsertDailyStockInfo(strCloseDate))
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_001"));
                this.InitDate();
            }
            else
            {
                BRM_CardDailyClose.DeleteDailyClose(strCloseDate);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_009"));            
            }

        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_009"));
        }
    }
    /// <summary>
    /// 功能說明:取消日結操作
    /// 作    者:Linda
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelClose_Click(object sender, EventArgs e)
    {
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020700_005");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");
        strLastCloseDate=this.lblLastCloseDate.Text.ToString().Trim();
        if (BRM_CardDailyClose.DeleteDailyStockInfo(strLastCloseDate))
        {
            if (BRM_CardDailyClose.DeleteDailyClose(strLastCloseDate))
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_004")); 
                this.InitDate();
            }
            else
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_010"));
            }

        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_010"));
        }
    }
    /// <summary>
    /// 功能說明:預設欄位值
    /// 作    者:Linda
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>

    protected void InitDate()
    {
        try
        {
            GetCloseDate(ref strLastCloseDate);
            if (strLastCloseDate == "")
            {
                this.lblLastCloseDate.Text = "";
                this.lblNowCloseDate.Text = "";
                this.dpCloseDate.Text = "";
            }
            else
            {
                this.lblLastCloseDate.Text = strLastCloseDate;
                strCloseDate = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", strLastCloseDate.Replace("/", ""), 1);
                this.lblNowCloseDate.Text = strCloseDate.Substring(0, 4) + "/" + strCloseDate.Substring(4, 2) + "/" + strCloseDate.Substring(6, 2);
                this.dpCloseDate.Text = strLastCloseDate;
            }
        }
        catch
        {
            this.lblNowCloseDate.Text = "";
            this.dpCloseDate.Text = "";
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_011"));
        }

    }

    /// <summary>
    /// 功能說明:獲取上一個日結日期
    /// 作    者:Linda
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="strLastCloseDate"></param>

    private void GetCloseDate(ref string strLastCloseDate)
    {
        DataTable dtLastCloseDate = new DataTable();
        strLastCloseDate = string.Empty;
        if (BRM_CardDailyClose.GetLastCloseDate(ref dtLastCloseDate))
        {
            if (dtLastCloseDate.Rows.Count > 0)
            {
                strLastCloseDate = dtLastCloseDate.Rows[0]["DailyCloseDate"].ToString();
            }
            else
            {
                strLastCloseDate = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyy/MM/dd").Replace("/", ""), -1);
                strLastCloseDate = strLastCloseDate.Substring(0, 4) + "/" + strLastCloseDate.Substring(4, 2) + "/" + strLastCloseDate.Substring(6, 2);
            }
        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020700_000"));
        }

    }
}
