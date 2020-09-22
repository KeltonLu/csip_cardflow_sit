//******************************************************************
//*  功能說明：異動作業單查詢
//*  作    者：HAO CHEN
//*  創建日期：2010/06/25
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class Page_P060203000001 : PageBase
{
    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

    public DataTable m_dtCardBaseInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }

    public string m_Status
    {
        get { return ViewState["m_Status"] as string; }
        set { ViewState["m_Status"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
            m_Status = "Y";
            ViewState["FlgEdit"] = "FALSE";
            this.Page.Title = BaseHelper.GetShowText("06_02030001_001");
            //* 加載狀態選項
            BindState();
            //* 加載異動選項
            BindDataChange();
        }
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_02030001_008");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_02030001_009");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_02030001_010");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_02030001_011");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_02030001_012");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_02030001_013");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_02030001_014");

        //* 異動明細GridView的列頭標題
        this.CustGridView1.Columns[0].HeaderText = BaseHelper.GetShowText("06_02030001_016");
        this.CustGridView1.Columns[1].HeaderText = BaseHelper.GetShowText("06_02030001_017");


        //* 設定GridView自動換行
        grvUserView.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:加載狀態選項
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindState()
    {
        string strMsgID = string.Empty;
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "24", ref dtState))
        {
            this.ddlState.DataSource = dtState;
            this.ddlState.DataTextField = "PROPERTY_NAME";
            this.ddlState.DataValueField = "PROPERTY_CODE";
            this.ddlState.DataBind();
        }

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtState))
        {
            this.ddlFactory.DataSource = dtState;
            this.ddlFactory.DataTextField = "PROPERTY_NAME";
            this.ddlFactory.DataValueField = "PROPERTY_CODE";
            this.ddlFactory.DataBind();
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "0");
            ddlFactory.Items.Insert(0, li);
        }
    }

    /// <summary>
    /// 功能說明:加載異動欄位選項
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindDataChange()
    {
        string strMsgID = string.Empty;
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "27", ref dtState))
        {
            this.ddlChangeField.DataSource = dtState;
            this.ddlChangeField.DataTextField = "PROPERTY_NAME";
            this.ddlChangeField.DataValueField = "PROPERTY_CODE";
            this.ddlChangeField.DataBind();
        }
    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        //*身分證字哈不能有中文
        if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
        {
            strMsgID = "06_02030000_005";
            txtId.Focus();
            return false;
        }
        //*卡號不能有中文
        if (ValidateHelper.IsChinese(this.txtCardNo.Text.Trim()))
        {
            strMsgID = "06_02030000_006";
            txtCardNo.Focus();
            return false;
        }

        string strStart = dpStart.Text.Trim();
        string strEnd = dpEnd.Text.Trim();

        if (!string.IsNullOrEmpty(strStart) || !string.IsNullOrEmpty(strEnd))
        {
            if (string.IsNullOrEmpty(strStart))
            {
                strMsgID = "06_02030000_000";
                dpStart.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(strEnd))
            {
                strMsgID = "06_02030000_001";
                dpEnd.Focus();
                return false;
            }

            if (!string.IsNullOrEmpty(strStart) && !string.IsNullOrEmpty(strEnd))
            {
                //*匯入日期
                if (!ValidateHelper.IsValidDate(strStart, strEnd, ref strMsgID))
                {
                    strMsgID = "06_02030000_002";
                    dpStart.Focus();
                    return false;
                }
            }
        }
        return true;
    }


    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        string strCondition = string.Empty;

        if (!ddlState.SelectedValue.Equals("0"))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.Equal, DataTypeUtils.String, EncodeForSQL(ddlState.SelectedValue));
        }
        if (!string.IsNullOrEmpty(txtUser.Text))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_UpdUser, Operator.Equal, DataTypeUtils.String, EncodeForSQL(txtUser.Text.Trim()));
        }

        strCondition = sqlhelp.GetFilterCondition();
        if (!string.IsNullOrEmpty(txtId.Text))
        {
            strCondition += " And a.ID='" + EncodeForSQL(this.txtId.Text.Trim()) + "' ";
        }

        if (!string.IsNullOrEmpty(txtCardNo.Text))
        {
            strCondition += " And a.CardNo='" + EncodeForSQL(this.txtCardNo.Text.Trim()) + "' ";
            sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, this.txtCardNo.Text.Trim());
        }

        if (ddlFactory.SelectedIndex != 0)
        {
            strCondition += " And b.Merch_Code='" + EncodeForSQL(ddlFactory.SelectedValue) + "' ";
        }

        if (!ddlChangeField.SelectedValue.Equals("0"))
        {
            strCondition += " AND " + EncodeForSQL(ddlChangeField.SelectedValue) + " IS NOT NULL ";
        }

        if (!string.IsNullOrEmpty(dpStart.Text) && !string.IsNullOrEmpty(dpEnd.Text))
        {
            string strSdate = dpStart.Text.ToString();
            string strEnd = dpEnd.Text.ToString();
            strCondition += " AND UpdDate BETWEEN '" + EncodeForSQL(strSdate) + "' AND '" + EncodeForSQL(strEnd) + "'";
        }

        return strCondition;
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:HAO CHENs
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = string.Empty;
        int iTotalCount = 0;
        DataTable dtCardDataChange = new DataTable();
        try
        {
            //* 查詢不成功
            if (!BRM_CardDataChange.SearchFor0203(GetFilterCondition(), ref dtCardDataChange, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.grvUserView.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtCardDataChange);
                m_dtCardBaseInfo = dtCardDataChange;
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtCardDataChange;
                this.grvUserView.DataBind();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_02030000_003"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:MergeTable加載資料
    /// 作    者:HAO CHEN
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtCard)
    {
        string strMsgID = string.Empty;
        string strUserID = string.Empty;
        string strUserName = string.Empty;
        //*UserName
        foreach (DataRow row in dtCard.Rows)
        {
            //* 收件人姓名異動
            if (null != row["NewName"] && !string.IsNullOrEmpty(row["NewName"].ToString()))
            {
                row["Datachange"] = BaseHelper.GetShowText("06_02030001_020");
            }

            //* 郵寄日期異動
            if (null != row["NewMailDate"] && !string.IsNullOrEmpty(row["NewMailDate"].ToString()))
            {
                row["Datachange"] = BaseHelper.GetShowText("06_02030001_021");
            }

            //* 額度異動
            if (null != row["NewMonlimit"] && !string.IsNullOrEmpty(row["NewMonlimit"].ToString()))
            {
                row["Datachange"] = BaseHelper.GetShowText("06_02030001_022");
            }

            //* 取卡方式異動
            if (null != row["NewWay"] && !string.IsNullOrEmpty(row["NewWay"].ToString()))
            {
                row["Datachange"] = BaseHelper.GetShowText("06_02030001_023");
            }

            //* 掛號號碼異動
            if (null != row["newmailno"] && !string.IsNullOrEmpty(row["newmailno"].ToString()))
            {
                row["Datachange"] = BaseHelper.GetShowText("06_02030001_024");
            }

            //* 郵寄地址異動
            if (null != row["NewZip"] && !string.IsNullOrEmpty(row["NewZip"].ToString()))
            {
                row["Datachange"] = BaseHelper.GetShowText("06_02030001_025");
            }

            if (null != row["UpdUser"] && !string.IsNullOrEmpty(row["UpdUser"].ToString()))
            {
                strUserID = row["UpdUser"].ToString();
                if (BusinessRules.BRM_User.GetUserName(strUserID, ref strUserName))
                {
                    row["UpdUser"] = strUserID + (char)10 + strUserName;
                }
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtId.Text;
        log.Account_Nbr = this.txtCardNo.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string strMsgID = string.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "')");
            return;
        }
        BindGridView();
    }

    /// <summary>
    /// 功能說明:顯示異動明細
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {

        ViewState["FlgEdit"] = "TRUE";
        HiddenField hidvalue = grvUserView.Rows[e.NewEditIndex].FindControl("hidType") as HiddenField;
        string strAction = m_dtCardBaseInfo.Rows[e.NewEditIndex]["action"].ToString();
        string strId = string.Empty;
        if (null != m_dtCardBaseInfo.Rows[e.NewEditIndex]["id"] && !string.IsNullOrEmpty(m_dtCardBaseInfo.Rows[e.NewEditIndex]["id"].ToString()))
        {
            strId = m_dtCardBaseInfo.Rows[e.NewEditIndex]["id"].ToString();
        }

        string strCardNo = m_dtCardBaseInfo.Rows[e.NewEditIndex]["CardNo"].ToString();
        string strTrandate = m_dtCardBaseInfo.Rows[e.NewEditIndex]["Trandate"].ToString();

        DataTable dtCardDataChanges = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        if (!string.IsNullOrEmpty(strId))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, strId);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, strCardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, strAction);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, strTrandate);
        //sqlhelp.AddOrderCondition(Entity_CardDataChange.M_Sno, ESortType.DESC);

        if (BRM_CardDataChange.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardDataChanges, ref strMsgID))
        {
            this.CustGridView1.DataSource = dtCardDataChanges;
            this.CustGridView1.DataBind();
        }
        m_Status = "N";
        ddlState.Visible = false;
        ddlChangeField.Visible = false;
        this.ModalPopupExtenderC.Show();
    }

    ///// <summary>
    ///// 功能說明:彈出異動明細
    ///// 作    者:HAO CHEN
    ///// 創建時間:2010/06/25
    ///// 修改記錄:
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //protected void btnUpdateC_Click(object sender, EventArgs e)
    //{
    //    InitControls();
    //    this.ModalPopupExtenderC.Show();
    //}

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


    protected void btnCancelC_Click(object sender, EventArgs e)
    {
        ddlState.Visible = true;
        ddlChangeField.Visible = true;
        ModalPopupExtenderC.Hide();
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:SQL Injection
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/16
    /// </summary>
    public static String EncodeForSQL(String str, Int32 layer = 1)
    {
        List<String> oldList = new List<String>();
        List<String> newList = new List<String>();
        List<String> typeList = new List<String>();
        if (layer >= 0)
            AddReplaceList(
                new String[] { "'" },
                new String[] { "''" },
                new String[] { "" },
                ref oldList, ref newList, ref typeList);
        if (layer >= 1)
            AddReplaceList(
                new String[] {
                    ";", ",", "?", "<", ">",
                    "(", ")", "@", "--", "=",
                    "+", "*", "&", "#", "%",
                    "$" },
                new String[] {
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "" },
                new String[] {
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "" },
                ref oldList, ref newList, ref typeList);
        if (layer >= 2)
            AddReplaceList(
                new String[] {
                    "select", "insert", "delete from", "count", "drop table",
                    "truncate", "asc", "mid", "char", "xp_cmdshell",
                    "exec master", "net localgroup administrators", "and", "net user", "or",
                    "net", "delete", "drop", "script", "update",
                    "chr", "master", "declare", "exec" },
                new String[] {
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    "", "", "", "" },
                new String[] {
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1" },
                ref oldList, ref newList, ref typeList);
        for (int i = 0; i < oldList.Count && i < newList.Count && i < typeList.Count; i++)
            str = (typeList[i] == "" ? str.Replace(oldList[i], newList[i]) : Regex.Replace(str, oldList[i], newList[i], RegexOptions.IgnoreCase));
        return str;
    }
    public static void AddReplaceList(String[] oldArr, String[] newArr, String[] typeArr, ref List<String> oldList, ref List<String> newList, ref List<String> typeList)
    {
        oldList.AddRange(oldArr);
        newList.AddRange(newArr);
        typeList.AddRange(typeArr);
    }
}
