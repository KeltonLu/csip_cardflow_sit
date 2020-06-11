//******************************************************************
//*  功能說明：郵局查單申請處理UI層
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
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
using CSIPCommonModel.EntityLayer;
public partial class P060401000001 : PageBase
{

    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    /// <summary>
    /// 功能說明:頁面加載事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
            BindGridView();
            ViewState["FlgEdit"] = "FALSE";
        }
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06040100_003");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06040100_001");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06040100_006");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06040100_007");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06040100_008");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06040100_009");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        this.grvUserView.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int iTotalCount = 0;
        DataTable dtPost = null;
        try
        {
            //* 查詢不成功
            if (!BRM_Post.SearchPost(GetFilterCondition(), ref dtPost, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtPost);
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtPost;
                this.grvUserView.DataBind();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_00000000_007"));
            return;
        }
    }


    /// <summary>
    /// 功能說明:MergeTable加載卡別
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtPost)
    {
        dtPost.Columns.Add("Actions");
        DataTable dtCode = new DataTable();
        string strMsgID = string.Empty;

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtCode))
        {
            foreach (DataRow row in dtPost.Rows)
            {
                DataRow[] rowCode = dtCode.Select("PROPERTY_CODE='" + row["action"].ToString() + "'");
                if (rowCode != null && rowCode.Length > 0)
                {
                    row["Actions"] = rowCode[0]["PROPERTY_CODE"].ToString() + " " + rowCode[0]["PROPERTY_NAME"].ToString();
                }
            }
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
        SqlHelper sqlhelp = new SqlHelper();

        if (this.txtNo.Text.Trim() != "")
        {
            sqlhelp.AddCondition("p." + Entity_Post.M_Cardno, Operator.Equal, DataTypeUtils.String, this.txtNo.Text.Trim());

        }
        if (this.dropState.SelectedValue.Trim() != "全部")
        {
            sqlhelp.AddCondition("p." + Entity_Post.M_EndCaseFlg, Operator.Equal, DataTypeUtils.String, this.dropState.SelectedValue.Trim());
        }
        if (this.txtFrom.Text.Trim() != "" && this.txtTo.Text.Trim() == "")
        {
            sqlhelp.AddCondition("p." + Entity_Post.M_Podate, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtFrom.Text.Trim());
        }
        if (this.txtTo.Text.Trim() != "" && this.txtFrom.Text.Trim() == "")
        {
            sqlhelp.AddCondition("p." + Entity_Post.M_Podate, Operator.LessThanEqual, DataTypeUtils.String, this.txtTo.Text.Trim());
        }
        if (this.txtFrom.Text.Trim() != "" && this.txtTo.Text.Trim() != "")
        {
            sqlhelp.AddCondition("p." + Entity_Post.M_Podate, Operator.GreaterThanEqual, DataTypeUtils.String, this.txtFrom.Text.Trim());
            sqlhelp.AddCondition("p." + Entity_Post.M_Podate, Operator.LessThanEqual, DataTypeUtils.String, this.txtTo.Text.Trim());
        }

        return sqlhelp.GetFilterCondition();
    }

    /// <summary>
    /// 功能說明:綁定EndCaseFlg
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    private void BindEndCaseFlg()
    {
        //DataTable dtEndCaseFlg = new DataTable();
        //if (BRM_Post.selectEndCaseFlg(ref dtEndCaseFlg))
        //{
        //    if (dtEndCaseFlg.Rows.Count > 0)
        //    {
        //        this.dropState.DataSource = dtEndCaseFlg;
        //        this.dropState.DataValueField = "Value";
        //        this.dropState.DataTextField = "Text";
        //    }
        //}
    }

    /// <summary>
    /// 功能說明:新增事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060401000002.aspx");
    }

    /// <summary>
    /// 功能說明:編輯邏輯
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            ViewState["FlgEdit"] = "TRUE";

            CustLinkButton link = grvUserView.Rows[e.NewEditIndex].Cells[1].FindControl("lkbDetail") as CustLinkButton;
            //* 進入編輯頁面
            if (link != null)
            {
                //* NO加密
                Response.Redirect("P060401000003.aspx?sno=" + RedirectHelper.GetEncryptParam(link.CommandArgument.ToString()) + "", false);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010400_005"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {

        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Account_Nbr = this.txtNo.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string strMsgID = string.Empty;

        if (this.txtNo.Text.Trim() != "")
        {
            if (ValidateHelper.IsChinese(this.txtNo.Text.Trim()))
            {
                //*卡號驗證不通過
                MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_013");
                return;
            }
        }
        if (this.txtFrom.Text.Trim() != "" && this.txtTo.Text.Trim() != "")
        {
            if (!ValidateHelper.IsValidDate(this.txtFrom.Text.Trim(), this.txtTo.Text.Trim(), ref strMsgID))
            {
                //* 起迄日期驗證不通過
                MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
                return;
            }
        }
        BindGridView();
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }
}
