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

public partial class Page_P060209000001 : PageBase
{
    #region table
    public DataTable m_dtCancelOASAInfo
    {
        get { return ViewState["m_dtCancelOASAInfo"] as DataTable; }
        set { ViewState["m_dtCancelOASAInfo"] = value; }
    }

    #endregion

    #region 事件
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.dpFrom.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.dpTo.Text = DateTime.Now.ToString("yyyy/MM/dd");
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;

            if (Request.QueryString["DateFrom0209"] != null && Request.QueryString["DateTo0209"] != null && Request.QueryString["PageIndex0209"] != null)
            {
                dpFrom.Text = RedirectHelper.GetDecryptString(Request.QueryString["DateFrom0209"].ToString().Trim());
                dpTo.Text = RedirectHelper.GetDecryptString(Request.QueryString["DateTo0209"].ToString().Trim());
                gpList.Visible = true;
                grvUserView.Visible = true;
                gpList.CurrentPageIndex = Convert.ToInt16(RedirectHelper.GetDecryptString(Request.QueryString["PageIndex0209"].ToString().Trim()));
                BindGridView(RedirectHelper.GetDecryptString(Request.QueryString["DateFrom0209"].ToString().Trim()), RedirectHelper.GetDecryptString(Request.QueryString["DateTo0209"].ToString().Trim()));
            } 
        }
    }

    /// <summary>
    /// 點擊修改
    /// </summary>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string strVisible = "0";
        string strCancelOASADate = m_dtCancelOASAInfo.Rows[e.NewEditIndex]["CancelOASADate"].ToString();
        string strCancelOASASource = m_dtCancelOASAInfo.Rows[e.NewEditIndex]["CancelOASASource"].ToString();
        string strCancelOASAFile = m_dtCancelOASAInfo.Rows[e.NewEditIndex]["CancelOASAFile"].ToString();
        string strStauts = m_dtCancelOASAInfo.Rows[e.NewEditIndex]["Stauts"].ToString().Trim();
        string strConfirmUser = m_dtCancelOASAInfo.Rows[e.NewEditIndex]["ConfirmUser"].ToString().Trim();
        string strUserId = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;

        if (strStauts == "1" && strConfirmUser == strUserId)
        {
            strVisible = "1";
        }

        if (strStauts == "0")
        {
            strVisible = "1";
        }

        this.Session["DateFrom0209"] = this.dpFrom.Text.Trim();
        this.Session["DateTo0209"] = this.dpTo.Text.Trim();
        this.Session["PageIndex0209"] = this.gpList.CurrentPageIndex.ToString();

        Response.Redirect("P060209000002.aspx?date=" + RedirectHelper.GetEncryptParam(strCancelOASADate) + "&source=" + RedirectHelper.GetEncryptParam(strCancelOASASource) + "&file=" + RedirectHelper.GetEncryptParam(strCancelOASAFile) + "&visible=" + RedirectHelper.GetEncryptParam(strVisible) + "", false);
    }

    protected void grvUserView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string[] strChangeNote = new string[] { };
        string[] strNote = new string[] { };
        DataTable dtNote = new DataTable();
        dtNote.Columns.Add("Date");
        dtNote.Columns.Add("Name");
        dtNote.Columns.Add("Note");

        string strChangeNoteList = m_dtCancelOASAInfo.Rows[e.RowIndex]["ChangeNote"].ToString().Trim();
        strChangeNoteList = strChangeNoteList.Substring(1, strChangeNoteList.Length - 1);
        strChangeNote = strChangeNoteList.Split(';');
        for (int intChangeNote = 0; intChangeNote < strChangeNote.Length; intChangeNote++)
        {
            DataRow rowNote = dtNote.NewRow();
            strNote = strChangeNote[intChangeNote].ToString().Trim().Split(',');
            rowNote["Date"] = strNote[0].ToString().Trim();
            rowNote["Name"] = strNote[1].ToString().Trim();
            rowNote["Note"] = strNote[2].ToString().Trim();
            dtNote.Rows.Add(rowNote);
        }
        this.grvLogView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020900_014");
        this.grvLogView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020900_015");
        this.grvLogView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020900_013");
        this.grvLogView.Visible = true;
        this.grvLogView.DataSource = dtNote;
        this.grvLogView.DataBind();
        this.ModalPopupExtenderLog.Show();
    }

    protected void btnCheck_Click(object sender, EventArgs e)
    {
        DataTable dtUpdateStauts = new DataTable();
        dtUpdateStauts.Columns.Add("CancelOASAFile");
        dtUpdateStauts.Columns.Add("CancelOASADate");
        dtUpdateStauts.Columns.Add("Stauts");
        dtUpdateStauts.Columns.Add("ChangeNote");
        bool bolchkEnable1 = false;

        try
        {
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06020900_000") + "：" + BaseHelper.GetShowText("06_06020900_003");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
            
            for (int i = 0; i < this.grvUserView.Rows.Count; i++)
            {
                HtmlInputCheckBox chkEnable1 = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[6].FindControl("chkCheckFlg");
                HtmlInputCheckBox chkEnable2 = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[7].FindControl("chkReleaseFlg");
                string StautsOld = m_dtCancelOASAInfo.Rows[i]["Stauts"].ToString().Trim();

                if (chkEnable1.Checked && chkEnable2.Checked)
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020900_001") + "');");
                    return;
                }
                else if (chkEnable1.Checked)
                {
                    bolchkEnable1 = true;
                    DataRow row = dtUpdateStauts.NewRow();
                    row["CancelOASAFile"] = m_dtCancelOASAInfo.Rows[i]["CancelOASAFile"].ToString().Trim();
                    row["CancelOASADate"] = m_dtCancelOASAInfo.Rows[i]["CancelOASADate"].ToString().Trim();

                    if (StautsOld == "1" || StautsOld == "3")
                    {
                        row["Stauts"] = "2";
                        row["ChangeNote"] = m_dtCancelOASAInfo.Rows[i]["ChangeNote"].ToString().Trim() + ";" + DateTime.Now.ToString("yyyy/MM/dd") + "," + ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_name + "," + "覆核";
                    }
                    else if (StautsOld == "2" || StautsOld == "5")
                    {
                        row["Stauts"] = "3";
                        row["ChangeNote"] = m_dtCancelOASAInfo.Rows[i]["ChangeNote"].ToString().Trim() + ";" + DateTime.Now.ToString("yyyy/MM/dd") + "," + ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_name + "," + "取消覆核";
                    }

                    dtUpdateStauts.Rows.Add(row);
                }

            }
            if(!bolchkEnable1)
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020900_006") + "');");
                return;
            }

            if (BRM_CancelOASA.BatStautsUpdate(dtUpdateStauts, ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id))
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020900_002"));
                BindGridView();
            }
            else
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020900_003"));
            }
        }
        catch
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020900_003"));
        }
    }
    protected void btnRelease_Click(object sender, EventArgs e)
    {
        DataTable dtUpdateStauts = new DataTable();
        dtUpdateStauts.Columns.Add("CancelOASAFile");
        dtUpdateStauts.Columns.Add("CancelOASADate");
        dtUpdateStauts.Columns.Add("Stauts");
        dtUpdateStauts.Columns.Add("ChangeNote");
        bool bolchkEnable2 = false;
        try
        {
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06020900_000") + "：" + BaseHelper.GetShowText("06_06020900_004");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
            
            for (int i = 0; i < this.grvUserView.Rows.Count; i++)
            {
                HtmlInputCheckBox chkEnable1 = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[6].FindControl("chkCheckFlg");
                HtmlInputCheckBox chkEnable2 = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[7].FindControl("chkReleaseFlg");
                string StautsOld = m_dtCancelOASAInfo.Rows[i]["Stauts"].ToString().Trim();

                if (chkEnable1.Checked && chkEnable2.Checked)
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020900_001") + "');");
                    return;
                }
                else if (chkEnable2.Checked)
                {
                    bolchkEnable2 = true;
                    DataRow row = dtUpdateStauts.NewRow();
                    row["CancelOASAFile"] = m_dtCancelOASAInfo.Rows[i]["CancelOASAFile"].ToString().Trim();
                    row["CancelOASADate"] = m_dtCancelOASAInfo.Rows[i]["CancelOASADate"].ToString().Trim();

                    if (StautsOld == "2" || StautsOld == "5")
                    {
                        row["Stauts"] = "4";
                        row["ChangeNote"] = m_dtCancelOASAInfo.Rows[i]["ChangeNote"].ToString().Trim() + ";" + DateTime.Now.ToString("yyyy/MM/dd") + "," + ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_name + "," + "放行";
                    }
                    else if (StautsOld == "4")
                    {
                        row["Stauts"] = "5";
                        row["ChangeNote"] = m_dtCancelOASAInfo.Rows[i]["ChangeNote"].ToString().Trim() + ";" + DateTime.Now.ToString("yyyy/MM/dd") + "," + ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_name + "," + "取消放行";
                    }

                    dtUpdateStauts.Rows.Add(row);
                }
            }
            if (!bolchkEnable2)
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020900_007") + "');");
                return;
            }

            if (BRM_CancelOASA.BatStautsUpdate(dtUpdateStauts, ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id))
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020900_004"));
                BindGridView();
            }
            else
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020900_005"));
            }
        }
        catch
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020900_005"));
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = "";
        if (this.dpFrom.Text.Trim() != "" && this.dpTo.Text.Trim() != "")
        {
            if (!ValidateHelper.IsValidDate(this.dpFrom.Text.Trim(), this.dpTo.Text.Trim(), ref strMsgID))
            {
                //* 起迄日期驗證不通過
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                return;
            }

        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020900_000") + "');");
            return;
        }
        
        BindGridView();
    }
    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }
    #endregion
    #region 方法
    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020900_005");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020900_006");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020900_007");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06020900_008");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06020900_009");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06020900_010");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06020900_011");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06020900_012");
        this.grvUserView.Columns[8].HeaderText = BaseHelper.GetShowText("06_06020900_013");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        this.grvUserView.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
    }
    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    private void BindGridView(string strdpFrom, string strdpTo)
    {
        int iTotalCount = 0;
        DataTable dtCancelOASAInfo = new DataTable();

        try
        {
            //* 查詢不成功
            if (!BRM_CancelOASA.GetCancelOASAInfo(ref dtCancelOASAInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, strdpFrom, strdpTo))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtCancelOASAInfo);
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtCancelOASAInfo;
                this.grvUserView.DataBind();
                VisibleCheckBox(dtCancelOASAInfo);
                m_dtCancelOASAInfo = dtCancelOASAInfo;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_002"));
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        int iTotalCount = 0;
        DataTable dtCancelOASAInfo = new DataTable();

        try
        {
            //* 查詢不成功
            if (!BRM_CancelOASA.GetCancelOASAInfo(ref dtCancelOASAInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, this.dpFrom.Text.Trim(), this.dpTo.Text.Trim()))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtCancelOASAInfo);
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtCancelOASAInfo;
                this.grvUserView.DataBind();
                VisibleCheckBox(dtCancelOASAInfo);
                m_dtCancelOASAInfo = dtCancelOASAInfo;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_002"));
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
            return;
        }
    }
    /// <summary>
    /// 功能說明:取得用戶中文名稱
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/09
    /// 修改記錄:
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="dtUserList"></param>
    /// <returns></returns>
    private string GetUserName(string UserId, DataTable dtUserList)
    {
        foreach (DataRow dr in dtUserList.Rows)
        {
            if (UserId.Equals(dr["USER_ID"].ToString()))
            {
                return dr["USER_NAME"].ToString();
            }
        }
        return "";
    }
    /// <summary>
    /// 功能說明:MergeTable增加序號
    /// 作    者:Linda
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="dtSelfPickInfo"></param>
    public void MergeTable(ref DataTable dtCancelOASAInfo)
    {
        dtCancelOASAInfo.Columns.Add("ModStautsLog");
        DataTable dtUserName = new DataTable();
        JobHelper.GetUserNameByUserId(dtCancelOASAInfo, "ModStautsUser", ref dtUserName);
        foreach (DataRow row in dtCancelOASAInfo.Rows)
        {
            if (!row["ModStautsDate"].ToString().Equals(string.Empty))
            {
                row["ModStautsLog"] = row["ModStautsDate"] + " " + GetUserName(row["ModStautsUser"].ToString(), dtUserName) + " " + row["StautsName"];
            }
            else
            {
                row["ModStautsLog"] = "";
            }
        }
    }
    /// <summary>
    /// 功能說明:VisibleCheckBox設置複選框是否可見
    /// 作    者:Linda
    /// 創建時間:2010/07/20
    /// 修改記錄:
    /// </summary>
    /// <param name="dtCancelOASAInfo"></param>
    public void VisibleCheckBox(DataTable dtCancelOASAInfo)
    {
        for (int i = 0; i < this.grvUserView.Rows.Count; i++)
        {
            HtmlInputCheckBox chkEnable1 = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[6].FindControl("chkCheckFlg");
            HtmlInputCheckBox chkEnable2 = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[7].FindControl("chkReleaseFlg");

            string strConfirmUser = dtCancelOASAInfo.Rows[i]["ConfirmUser"].ToString().Trim();
            string strModStautsUser = dtCancelOASAInfo.Rows[i]["ModStautsUser"].ToString().Trim();
            string strStauts = dtCancelOASAInfo.Rows[i]["Stauts"].ToString().Trim();
            string strUserId=((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;


            if ((this.btnCheck.Enabled == false) || (strStauts != "1" && strStauts != "2" && strStauts != "3" && strStauts != "5"))
            {
                chkEnable1.Visible = false;
            }
            else
            {
                if ((strStauts == "1" || strStauts == "3" || strStauts == "5") && (strUserId == strConfirmUser))
                {
                    chkEnable1.Visible = false;
                }
                if (strStauts == "2" && (strUserId != strModStautsUser))
                {
                    chkEnable1.Visible = false;
                }
            }
            if ((this.btnRelease.Enabled == false) || (strStauts != "2" && strStauts != "4" && strStauts != "5"))
            {
                chkEnable2.Visible = false;
            }
            else
            {
                if (strStauts == "4" && (strUserId != strModStautsUser))
                {
                    chkEnable2.Visible = false;
                }
            }           
        }
    }
    #endregion

    /// <summary>
    /// 調整顯示
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            System.Data.DataRowView rowView = (System.Data.DataRowView)e.Row.DataItem;
            // -1時，變更為NA且移除超連結
            if (rowView["TotalCount"].ToString() == "-1")
            {
                
                e.Row.Cells[2].Text = rowView["CancelOASAFile"].ToString();
                e.Row.Cells[3].Text = "NA";
                e.Row.Cells[4].Text = "NA";
                e.Row.Cells[5].Text = "NA";
            }
            if (rowView["TotalCount"].ToString() == "-2")
            {

                e.Row.Cells[2].Text = rowView["CancelOASAFile"].ToString();
                e.Row.Cells[3].Text = "格式異常";
                e.Row.Cells[4].Text = "格式異常";
                e.Row.Cells[5].Text = "格式異常";
            }
        }
    }
}
