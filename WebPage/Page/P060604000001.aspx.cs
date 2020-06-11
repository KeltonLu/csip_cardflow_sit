//******************************************************************
//*  功能說明：操作Log記錄
//*  作    者：HAO CHEN
//*  創建日期：2010/07/15
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Cryptography;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using Framework.Data.OM.Collections;
using System.Text;

public partial class P060604000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowControlsText();
            BindState();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
        }
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06040000_005");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06040000_006");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06040000_007");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06040000_008");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        this.grvUserView.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
    }

    /// <summary>
    /// 功能說明:加載類型選項
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/15
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindState()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "31", ref dtState))
        {
            this.ddlType.DataSource = dtState;
            this.ddlType.DataTextField = "PROPERTY_NAME";
            this.ddlType.DataValueField = "PROPERTY_CODE";
            this.ddlType.DataBind();
        }
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int iTotalCount = 0;
        DataTable dtCardBaseInfo = new DataTable();
        try
        {
            //* 查詢不成功
            if (!BRM_Log.SearchLog(GetFilterCondition(), ref dtCardBaseInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.grvUserView.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06060000_004"));
                return;
            }
            //* 查詢成功
            else
            {
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtCardBaseInfo;
                this.grvUserView.DataBind();
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06060000_003"));
            }

        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06060000_004"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        StringBuilder sbSql = new StringBuilder();
        if (!string.IsNullOrEmpty(txtImportStart.Text.Trim()))
        {
            sbSql.Append(" and CONVERT(varchar(10), L.create_dt, 111) >='" + txtImportStart.Text.Trim() + "'");
        }
        if (!string.IsNullOrEmpty(txtImportEnd.Text.Trim()))
        {
            sbSql.Append(" and CONVERT(varchar(10), L.create_dt, 111) <='" + txtImportEnd.Text.Trim() + "'");
        }
        if (!ddlType.SelectedValue.Trim().Equals("0"))
        {
            sbSql.Append(" and L.type_flg='"+ ddlType.SelectedValue.Trim() +"'");
        }
        if (!string.IsNullOrEmpty(txtUserName.Text.Trim()))
        {
            sbSql.Append(" and L.create_user like '%" + txtUserName.Text.Trim() + "%'");
        }
        return sbSql.ToString();
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        if (string.IsNullOrEmpty(txtImportStart.Text.Trim()) )
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06060000_006") + "')");
            return;
        }

        if (string.IsNullOrEmpty(txtImportEnd.Text.Trim()))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06060000_007") + "')");
            return;
        }


        if (!string.IsNullOrEmpty(txtImportStart.Text.Trim()) && !string.IsNullOrEmpty(txtImportEnd.Text.Trim()))
        {
            //*匯入日期
            if (!ValidateHelper.IsValidDate(txtImportStart.Text.Trim(), txtImportEnd.Text.Trim(), ref strMsgID))
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06040000_005") + "')");
                return;
            }
        }
        BindGridView();
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        if (string.IsNullOrEmpty(txtImportStart.Text.Trim()))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06060000_006") + "')");
            return;
        }

        if (string.IsNullOrEmpty(txtImportEnd.Text.Trim()))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06060000_007") + "')");
            return;
        }


        if (!string.IsNullOrEmpty(txtImportStart.Text.Trim()) && !string.IsNullOrEmpty(txtImportEnd.Text.Trim()))
        {
            //*匯入日期
            if (!ValidateHelper.IsValidDate(txtImportStart.Text.Trim(), txtImportEnd.Text.Trim(), ref strMsgID))
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06040000_005") + "')");
                return;
            }
        }

        string strFlg = ddlType.SelectedValue;
        if (strFlg.Equals("0"))
        {
            strFlg = "";
        }

        //* 傳遞參數加密
        Response.Redirect("P060604000002.aspx?dtStart=" + RedirectHelper.GetEncryptParam(txtImportStart.Text) + "&dtEnd=" + RedirectHelper.GetEncryptParam(txtImportEnd.Text) + "&flg=" + RedirectHelper.GetEncryptParam(strFlg) + "&user=" + RedirectHelper.GetEncryptParam(txtUserName.Text));
    }
}
