//******************************************************************
//*  功能說明：Mail警訊通知
//*  作    者：JUN HU
//*  創建日期：2010/07/12
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;
using BusinessRules;
using Framework.Common.JavaScript;
using Framework.Data.OM;
using EntityLayer;
using Framework.Common.Logging;
using Framework.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.Text;

public partial class P060608000001 : PageBase
{
    public DataTable m_dtLogmailInfo
    {
        get { return ViewState["m_dtLogmailInfo"] as DataTable; }
        set { ViewState["m_dtLogmailInfo"] = value; }
    }

    public string index
    {
        get { return ViewState["index"] as string; }
        set { ViewState["index"] = value; }
    }

    public string is_Type
    {
        get { return ViewState["is_Type"] as string; }
        set { ViewState["is_Type"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            grvCardView.Visible = false;
            ShowControlsText();
            gpList.Visible = false;
            gpList.RecordCount = 0;
            BindControl();
            BindGridView();
        }
    }

    /// <summary>
    /// 功能說明:初始化控件

    /// 作    者:JUN HU
    /// 創建時間:2010/07/09
    /// 修改記錄:
    /// </summary>
    private void BindControl()
    {
        string strMsgID = string.Empty;
        DataTable dtRole = new DataTable();
        if (BRM_Report.SearchRole("", ref dtRole, ref strMsgID))
        {
            this.ddlRole.DataSource = dtRole;
            this.ddlRole.DataTextField = "ROLE_ID";
            this.ddlRole.DataValueField = "ROLE_ID";
            this.ddlRole.DataBind();
        }
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/09
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
    /// 功能說明:綁定GridView
    /// 作    者:JUN HU
    /// 創建時間:2010/07/09
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int iTotalCount = 0;
        DataTable dtLogMail = new DataTable();
        try
        {
            //* 查詢不成功
            if (!BRM_Report.SearchLogMail("", ref dtLogMail, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvCardView.DataSource = null;
                this.grvCardView.DataBind();
                this.gpList.Visible = false;
                this.grvCardView.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            //* 查詢成功
            else
            {
                //MergeTable(ref dtLogMail);
                m_dtLogmailInfo = dtLogMail;
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvCardView.Visible = true;
                this.grvCardView.DataSource = dtLogMail;
                this.grvCardView.DataBind();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06080000_004"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者：JUN HU
    /// 創建時間:2010/07/09
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題

        this.grvCardView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06060800_001");
        this.grvCardView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06060800_002");
        this.grvCardView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06060800_003");
        this.grvCardView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06060800_004");
        this.grvCardView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06060800_005");
        this.grvCardView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06060800_006");

        //* 設置一頁顯示最大筆數

        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvCardView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:MergeTable加載发送人和CC人

    /// 作    者:JUN HU
    /// 創建時間:2010/07/09
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtLogMail)
    {
        try
        {
            string strMsgID = string.Empty;
            dtLogMail.Columns.Add("ToNames");
            dtLogMail.Columns.Add("CCNames");
            foreach (DataRow row in dtLogMail.Rows)
            {
                string ToUser = row["ToUsers"].ToString();
                string CcUser = row["CcUsers"].ToString();
                string[] ToUsers = ToUser.Split(new Char[] { ';' });
                string[] CcUsers = CcUser.Split(new Char[] { ';' });
                string Ccnames = string.Empty;
                string Tonames = string.Empty;
                foreach (string user in ToUsers)
                {
                    DataTable dtToUser = new DataTable();
                    SqlHelper sqlhelpt = new SqlHelper();
                    sqlhelpt.AddCondition("USER_MAIL", Operator.Equal, DataTypeUtils.String, user);
                    if (BRM_Report.SearchUser(sqlhelpt.GetFilterCondition(), ref dtToUser, ref strMsgID))
                    {
                        if (dtToUser.Rows.Count > 0)
                        {
                            if (Tonames.Equals(""))
                            {
                                Tonames = Tonames + dtToUser.Rows[0]["USER_NAME"].ToString();
                            }
                            else
                            {
                                Tonames = Tonames + ";" + dtToUser.Rows[0]["USER_NAME"].ToString();
                            }
                        }
                    }
                }
                row["ToNames"] = Tonames;


                foreach (string user in CcUsers)
                {
                    DataTable dtCcUsers = new DataTable();
                    SqlHelper sqlhelpc = new SqlHelper();
                    sqlhelpc.AddCondition("USER_MAIL", Operator.Equal, DataTypeUtils.String, user);
                    if (BRM_Report.SearchUser(sqlhelpc.GetFilterCondition(), ref dtCcUsers, ref strMsgID))
                    {
                        if (dtCcUsers.Rows.Count > 0)
                        {
                            if (Ccnames.Equals(""))
                            {
                                Ccnames = Ccnames + dtCcUsers.Rows[0]["USER_NAME"].ToString();
                            }
                            else
                            {
                                Ccnames = Ccnames + ";" + dtCcUsers.Rows[0]["USER_NAME"].ToString();
                            }
                        }
                    }
                }
                row["CCNames"] = Ccnames;
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06080000_004"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:GridView發送用戶欄修改按鈕單擊事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/12
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnToUser_Click(object sender, EventArgs e)
    {
        is_Type = "T";
        hflist.Value = string.Empty;
        CustButton custbtn = (CustButton)sender;
        index = custbtn.CommandArgument;
        string strMailID = custbtn.CommandName;
        SearchUser();
        GetUser(strMailID,is_Type);
        ModalPopupExtenderM.Show();
    }

    /// <summary>
    /// 功能說明:GridView CC用戶欄修改按鈕單擊事件

    /// 作    者:JUN HU
    /// 創建時間:2010/07/12
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCCUser_Click(object sender, EventArgs e)
    {
        is_Type = "C";
        hflist.Value = string.Empty;
        CustButton custbtn = (CustButton)sender;
        index = custbtn.CommandArgument;
        string strMailID = custbtn.CommandName;
        SearchUser();
        GetUser(strMailID, is_Type);
        ModalPopupExtenderM.Show();
    }

    /// <summary>
    /// 功能說明:綁定角色ID下拉框
    /// 作    者:JUN HU
    /// 創建時間:2010/07/12
    /// 修改記錄:
    /// </summary>
    private void SearchUser()
    {
        string strMsgID = string.Empty;
        DataTable dtUser = new DataTable();
        SqlHelper sqlhelp = new SqlHelper();
        lbSource.Items.Clear();
        sqlhelp.AddCondition("ROLE_ID", Operator.Equal, DataTypeUtils.String, ddlRole.SelectedValue);
        if (BRM_Report.SearchUser(sqlhelp.GetFilterCondition(), ref dtUser, ref strMsgID))
        {
            lbSource.DataTextField = "USER_NAME";
            lbSource.DataValueField = "USER_MAIL";
            lbSource.DataSource = dtUser;
            lbSource.DataBind();
        }
        lbSource.Rows = 7;
    }

    /// <summary>
    /// 功能說明:設定警訊用戶
    /// 作    者:JUN HU
    /// 創建時間:2010/07/12
    /// 修改記錄:
    /// </summary>
    private void GetUser(string strMailID, string strType)
    {
        string strMsgID = string.Empty;
        DataTable dtUser = new DataTable();
        SqlHelper sqlhelp = new SqlHelper();
        lbTarget.Items.Clear();
        //ToUserName ToUsers
        //CcUserName CcUsers
        string[] strToUsers = { };
        string[] strToUserName = { };

        string[] strCcUsers ={ };
        string[] strCcUserName ={ };

        sqlhelp.AddCondition("MailID", Operator.Equal, DataTypeUtils.String, strMailID);
        if (BRM_CallMail.SearchMail(sqlhelp.GetFilterCondition(),ref dtUser,ref strMsgID))
        {
            foreach (DataRow row in dtUser.Rows)
            {
                if (null != row["ToUsers"] && !String.IsNullOrEmpty(row["ToUsers"].ToString()))
                {
                    strToUsers = row["ToUsers"].ToString().Split(new Char[] { ';' });
                }

                if (null != row["ToUserName"] && !String.IsNullOrEmpty(row["ToUserName"].ToString()))
                {
                    strToUserName = row["ToUserName"].ToString().Split(new Char[] { ';' });
                }


                if (null != row["CcUsers"] && !String.IsNullOrEmpty(row["CcUsers"].ToString()))
                {
                    strCcUsers = row["CcUsers"].ToString().Split(new Char[] { ';' });
                }
                if (null != row["CcUserName"] && !String.IsNullOrEmpty(row["CcUserName"].ToString()))
                {
                    strCcUserName = row["CcUserName"].ToString().Split(new Char[] { ';' });
                }
            }
        }

        switch (strType)
        {
            case "T":
                for (int iCount = 0; iCount < strToUsers.Length; iCount++)
                {
                    ListItem liTempT  = new ListItem(strToUserName[iCount],strToUsers[iCount]);
                    lbTarget.Items.Add(liTempT);
                }
                
                break;
            case "C":
                for (int iCount = 0; iCount < strCcUsers.Length; iCount++)
                {
                    ListItem liTempC = new ListItem(strCcUserName[iCount], strCcUsers[iCount]);
                    lbTarget.Items.Add(liTempC);
                }
                break;
        }
        lbTarget.Rows = 7;
    }

    /// <summary>
    /// 功能說明:角色ID下拉框選擇事件

    /// 作    者:JUN HU
    /// 創建時間:2010/07/12
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
    {
        SearchUser();
        ModalPopupExtenderM.Show();
    }

    /// <summary>
    /// 功能說明:修改發送人保存事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/12
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (lbTarget.Items.Count <= 0 && is_Type.Equals("T"))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06080000_001");
            ModalPopupExtenderM.Show();
            return;
        }
        string mailId = string.Empty;
        string names = string.Empty;
        string strMsgID = string.Empty;
        try
        {
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06060800_000");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
            
            
            ArrayList alUsers = new ArrayList();
            ArrayList alUserName = new ArrayList();

            foreach (ListItem liTmp in lbTarget.Items)
            {
                alUsers.Add(liTmp.Value);
                alUserName.Add(liTmp.Text);
            }

            StringBuilder sbUsers = new StringBuilder();
            StringBuilder sbUserName = new StringBuilder();
            for (int iUser = 0; iUser < alUsers.Count; iUser++)
            {
                if (sbUsers.Length <= 0)
                {
                    sbUsers.Append(alUsers[iUser].ToString());
                }
                else
                {
                    sbUsers.Append(";");
                    sbUsers.Append(alUsers[iUser].ToString());
                }
            }

            for (int iUserName = 0; iUserName < alUserName.Count; iUserName++)
            {
                if (sbUserName.Length <= 0)
                {
                    sbUserName.Append(alUserName[iUserName].ToString());
                }
                else
                {
                    sbUserName.Append(";");
                    sbUserName.Append(alUserName[iUserName].ToString());
                }
            }


            mailId = m_dtLogmailInfo.Rows[int.Parse(index)]["MailID"].ToString();
            EntityM_CallMail callMail = new EntityM_CallMail();

            callMail.UpdataDate = System.DateTime.Now.ToString("yyyy/MM/dd");
            callMail.UpdataUser = ((EntityAGENT_INFO)Session["Agent"]).agent_id;

            SqlHelper sqlhelps = new SqlHelper();
            sqlhelps.AddCondition(EntityM_CallMail.M_MailID, Operator.Equal, DataTypeUtils.String, mailId);
            if (is_Type.Equals("T"))
            {
                callMail.ToUsers = sbUsers.ToString();
                callMail.ToUserName = sbUserName.ToString();
                BRM_CallMail.Update(callMail, sqlhelps.GetFilterCondition(), ref strMsgID, "ToUsers", "ToUserName", "UpdataDate", "UpdataUser");
            }
            else if (is_Type.Equals("C"))
            {
                callMail.CcUsers = sbUsers.ToString();
                callMail.CcUserName = sbUserName.ToString();
                BRM_CallMail.Update(callMail, sqlhelps.GetFilterCondition(), ref strMsgID, "CcUsers", "CcUserName", "UpdataDate", "UpdataUser");
            }
            BindGridView();
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06080000_002"));
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06080000_003"));
            return;
        }
    }

    protected void btnAddAll_Click(object sender, EventArgs e)
    {
        lbTarget.Items.Clear();
        foreach (ListItem liTmp in lbSource.Items)
        {
            lbTarget.Items.Add(liTmp);
        }
        ModalPopupExtenderM.Show();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {

        foreach (ListItem liTmp in lbSource.Items)
        {
            if (liTmp.Selected)
            {
                lbTarget.Items.Remove(lbTarget.Items.FindByValue(liTmp.Value));
                lbTarget.Items.Add(liTmp);
            }
        }
        ModalPopupExtenderM.Show();
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        ArrayList al = new ArrayList();
        foreach (ListItem liTmp in lbTarget.Items)
        {
            if (liTmp.Selected)
            {
                al.Add(liTmp);                
            }
        }

        for (int iTem = 0; iTem < al.Count; iTem++)
        {
            lbTarget.Items.Remove((ListItem)al[iTem]);
        }
        ModalPopupExtenderM.Show();
    }


    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        lbTarget.Items.Clear();
        ModalPopupExtenderM.Show();
    }
}
