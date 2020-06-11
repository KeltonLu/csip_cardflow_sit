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


public partial class P060401000002 : PageBase
{

    public DataTable m_dtCardBaseInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }
    public string m_uid
    {
        get { return ViewState["m_uid"] as string; }
        set { ViewState["m_uid"] = value; }
    }

    public string m_cardno
    {
        get { return ViewState["m_cardno"] as string; }
        set { ViewState["m_cardno"] = value; }
    }
    public string m_id
    {
        get { return ViewState["m_id"] as string; }
        set { ViewState["m_id"] = value; }
    }
    public string m_action
    {
        get { return ViewState["m_action"] as string; }
        set { ViewState["m_action"] = value; }
    }
    /// <summary>
    /// 功能說明:頁面加載綁定數據
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
            ViewState["FlgEdit"] = "FALSE";
        }
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
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06040100_006");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06040100_021");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06040100_007");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06040100_028");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        this.grvUserView.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());

        //*查郵局日初始化
        this.txtPoDate.Text = DateHelper.Today.ToString("yyyy/MM/dd");

        //*加載片語
        BindPhrase();
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
        DataTable dtCardBaseInfo = null;
        try
        {
            //* 查詢不成功
            if (!BRM_TCardBaseInfo.SearchByCardNo(GetFilterCondition(), ref dtCardBaseInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
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
                m_dtCardBaseInfo = dtCardBaseInfo;
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;

                //Action顯示為Code+Name
                DataTable dtCardtype = new DataTable();
                
                //*卡別顯示Code+Name
                if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtCardtype))
                {
                    foreach (DataRow row in dtCardBaseInfo.Rows)
                    {
                        DataRow[] rowCardtype = dtCardtype.Select("PROPERTY_CODE='" + row["action"].ToString() + "'");
                        if (rowCardtype != null && rowCardtype.Length > 0)
                        {
                            row["action"] = rowCardtype[0]["PROPERTY_CODE"].ToString() + " " + rowCardtype[0]["PROPERTY_NAME"].ToString();
                        }
                        else
                        {
                            row["action"] = row["action"].ToString();
                        }
                    }
                }
                this.grvUserView.DataSource = dtCardBaseInfo;
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
    /// 功能說明:GetFilterCondition
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, this.txtNo.Text.Trim());
        return sqlhelp.GetFilterCondition();
    }

    /// <summary>
    /// 功能說明:綁定片語
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    public void BindPhrase()
    {
        DataTable dtPhrase = new DataTable();
        string strMsgID = string.Empty;
        LibPhrase.Items.Clear();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "7", ref dtPhrase))
        {
            if (dtPhrase.Rows.Count > 0)
            {
                this.LibPhrase.DataSource = dtPhrase;
                this.LibPhrase.DataValueField = "PROPERTY_CODE";
                this.LibPhrase.DataTextField = "PROPERTY_NAME";
                this.LibPhrase.DataBind();
            }
        }
        LibPhrase.Rows = 7;
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

    /// <summary>
    /// 功能說明:新增操作
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
            return;
        }

        Entity_Post Post = new Entity_Post();
        Post.Cardno = this.txtNo.Text.Trim();
        Post.Podate = this.txtPoDate.Text.Trim();
        Post.Note = this.txtNote.Text.Trim();
        Post.Uid = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        Post.InputDate = DateTime.Now.ToString("yyyy/MM/dd") ;
        Post.Stateflg = "N";
        Post.EndCaseFlg = "N";
        //*取消重複數據的判斷
        //if (BRM_Post.IsRepeat(Post))
        //{
        //    MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_027");
        //    return;
        //}
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06040100_031");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");




        if (!BRM_Post.Insert(Post, ref strMsgID))
        {
            //* 增加不成功時，提示訊息
            MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
        }
        else
        {
            MessageHelper.ShowMessageAndGoto(UpdatePanel1, "P060401000001.aspx", strMsgID);
        }
    }

    /// <summary>
    /// 功能說明:新增操作
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/02
    /// 修改記錄:
    /// </summary>
    public bool InsertPost()
    {
        string strMsgID = string.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
            return false;
        }

        Entity_Post Post = new Entity_Post();
        Post.Cardno = this.txtNo.Text.Trim();
        Post.Podate = this.txtPoDate.Text.Trim();
        Post.Note = this.txtNote.Text.Trim();
        Post.Uid = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        Post.OutPutDate = DateTime.Now.ToString("yyyy/MM/dd");
        Post.Stateflg = "Y";
        Post.EndCaseFlg = "N";

        //*取消重複數據的判斷
        //if (BRM_Post.IsRepeat(Post))
        //{
        //    MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_027");
        //    return;
        //}

        return BRM_Post.Insert(Post, ref strMsgID);

    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        //*卡號
        if (string.IsNullOrEmpty(this.txtNo.Text))
        {
            strMsgID = "06_06040100_011";
            txtNo.Focus();
            return false;
        }
        if (ValidateHelper.IsChinese(this.txtNo.Text.Trim()))
        {
            strMsgID = "06_06040100_013";
            txtNo.Focus();
            return false;
        }
        //*存在性驗證
        DataTable dtTCardBaseInfo = new DataTable();
        if (BRM_TCardBaseInfo.SearchByCardNo(ref dtTCardBaseInfo, this.txtNo.Text.Trim()))
        {
            if (dtTCardBaseInfo.Rows.Count == 0)
            {
                strMsgID = "06_06040100_026";
                return false;
            }
        }
        //*查郵局日
        if (string.IsNullOrEmpty(this.txtPoDate.Text))
        {
            strMsgID = "06_06040100_017";
            txtPoDate.Focus();
            return false;
        }
        //*備注
        if (string.IsNullOrEmpty(this.txtNote.Text.Trim()))
        {
            strMsgID = "06_06040100_022";
            txtNote.Focus();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 功能說明:取消操作
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("P060401000001.aspx");
    }

    /// <summary>
    /// 功能說明:卡號選擇確定事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        this.lblName.Text = m_dtCardBaseInfo.Rows[e.NewEditIndex]["custname"].ToString();
        this.lblAddress.Text = m_dtCardBaseInfo.Rows[e.NewEditIndex]["add1"].ToString().Trim()+m_dtCardBaseInfo.Rows[e.NewEditIndex]["add2"].ToString().Trim()+m_dtCardBaseInfo.Rows[e.NewEditIndex]["add3"].ToString().Trim();
        this.lblMailDate.Text = m_dtCardBaseInfo.Rows[e.NewEditIndex]["maildate"].ToString();
        this.lblMailNo.Text = m_dtCardBaseInfo.Rows[e.NewEditIndex]["mailno"].ToString();
        this.m_uid = m_dtCardBaseInfo.Rows[e.NewEditIndex]["id"].ToString();

        //*報表參數
        m_cardno = m_dtCardBaseInfo.Rows[e.NewEditIndex]["cardno"].ToString();
        m_id = m_dtCardBaseInfo.Rows[e.NewEditIndex]["id"].ToString();
        m_action = m_dtCardBaseInfo.Rows[e.NewEditIndex]["action"].ToString().Trim().Substring(0,1);//只取Code

        this.LibPhrase.Visible = true;
        this.ModalPopupExtender1.Hide();
    }
    /// <summary>
    /// 功能說明:卡片選擇
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (this.txtNo.Text.Trim() == "")
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_011");
            return;
        }
        DataTable dtCardBaseInfo = new DataTable();
        if (BRM_TCardBaseInfo.SearchByCardNo(ref dtCardBaseInfo, this.txtNo.Text.Trim()))
        {
            if (dtCardBaseInfo.Rows.Count > 0)
            {
                this.gpList.Visible = false;
                this.gpList.RecordCount = 0;
                BindGridView();
                this.ModalPopupExtender1.Show();
                this.LibPhrase.Visible = false;
            }
            else
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_026");
            }
        }
    }

    /// <summary>
    /// 功能說明:產出表單
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCreateForm_Click(object sender, EventArgs e)
    {
        //*為避免手動改卡號，所有從頁面取卡號
        if (string.IsNullOrEmpty(this.txtNo.Text.Trim()) || string.IsNullOrEmpty(m_id) || string.IsNullOrEmpty(m_action))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_029");
            return;
        }

        Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
        CardBaseInfo.cardno = this.txtNo.Text.Trim();

        CardBaseInfo.id = m_id;

        CardBaseInfo.action = m_action;

        //*判斷是否有卡片信息，沒有則無法產生查單
        if (!BRM_TCardBaseInfo.IsRepeatByNo(CardBaseInfo))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_030");
            return;
        }

        //*先做新增操作
        if (InsertPost())
        {
            if (Session["CardNo"] != null)
            {
                Session.Remove("CardNo");
            }
            if (Session["Id"] != null)
            {
                Session.Remove("Id");
            }
            if (Session["Action"] != null)
            {
                Session.Remove("Action");
            }
            Session["CardNo"] = m_cardno;
            Session["Id"] = m_id;
            Session["Action"] = m_action;
            jsBuilder.RegScript(this.Page, "window.open('P060401000004.aspx')");
        }
        else
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_012");
        }
    }

    /// <summary>
    /// 功能說明:片語選擇
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void LibPhrase_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (LibPhrase.SelectedValue.Trim())
        {
            case "1":
                this.txtNote.Text += DateTime.Now.ToString("yyyy/MM/dd");
                break;
            case "2":
                this.txtNote.Text += DateTime.Now.ToString("HH:mm:ss");
                break;
            case "3":
                this.txtNote.Text += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                break;
            default:
                this.txtNote.Text += this.LibPhrase.SelectedItem.Text.Trim();
                break;
        }
    }
    protected void btnCancels_Click(object sender, EventArgs e)
    {
        this.LibPhrase.Visible = true;
        this.ModalPopupExtender1.Hide();
    }
}
