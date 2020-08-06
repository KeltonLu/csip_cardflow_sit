//******************************************************************
//*  功能說明：郵局查單申請處理UI層
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using EntityLayer;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

public partial class P060401000003 : PageBase
{

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
    public string m_Sno
    {
        get { return ViewState["m_Sno"] as string; }
        set { ViewState["m_Sno"] = value; }
    }
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
            if (Request.QueryString["sno"] != null && Request.QueryString["sno"].ToString() != "")
            {
                m_Sno = RedirectHelper.GetDecryptString(Request.QueryString["sno"].ToString());
                //* NO解密
                SearchAndBind(m_Sno);
            }
        }
    }

    /// <summary>
    /// 功能說明:頁面數據初始化
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strNo"></param>
    public void SearchAndBind(string strSNo)
    {
        this.chkdelete.Text = BaseHelper.GetShowText("06_06040100_026");

        DataTable dtPostByNo = new DataTable();
        string strMsgID = string.Empty;
        if (SearchByNo(ref dtPostByNo, strSNo))
        {
            DataRow row = dtPostByNo.Rows[0];
            if (row != null)
            {
                InitWebControls(row);
            }
            else
            {
                MessageHelper.ShowMessageAndGoto(UpdatePanel1, "P060401000001.aspx", "06_06040100_014");
            }
        }
    }

    /// <summary>
    /// 功能說明:按卡號查詢
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strNo"></param>
    /// <param name="dtPostByNo"></param>
    /// <returns></returns>
    public bool SearchByNo(ref DataTable dtPostByNo, string strSNo)
    {
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition("p." + Entity_Post.M_Sno, Operator.Equal, DataTypeUtils.String, strSNo);
        return BRM_Post.SearchPostByNo(sqlhelp.GetFilterCondition(), ref dtPostByNo, ref strMsgID);
    }

    /// <summary>
    /// 功能說明:按卡號查詢資料更新頁面物件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="row"></param>
    public void InitWebControls(DataRow row)
    {
        //*頁面控件賦值
        this.lblcardno.Text = row["Cardno"].ToString();
        this.lblmailno.Text = row["Mailno"].ToString();
        //Action中文顯示
        string strAction = row["Action"].ToString();
        TranAction(strAction, ref strAction);
        this.lblaction.Text = strAction;
        this.lblpodate.Text = row["Podate"].ToString();
        this.txtbackdate.Text = row["Backdate"].ToString();
        this.lblendCaseFlgs.Text = row["EndCaseFlgs"].ToString();
        this.txtnote.Text = row["Note"].ToString();
        //*報表參數
        m_cardno = row["Cardno"].ToString();
        m_id = row["id"].ToString();
        m_action = row["Action"].ToString();


        //獲取當前用戶角色 看是否有權限刪除和更新
        bool blDelFlg = false;
        //string strOldRole = BRM_Post.SearchUserRoleId(row["Uid"].ToString());
        //if (!string.IsNullOrEmpty(strOldRole))
        //{
        //    string[] strNowRoleId = new string[] { };
        //    string strNowRoleIdList = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).roles;
        //    strNowRoleId = strNowRoleIdList.Replace("\'", "").Split(',');
        //    for (int i = 0; i < strNowRoleId.Length; i++)
        //    {
        //        if (strOldRole.Equals(strNowRoleId[i]))
        //        {
        //            blDelFlg = true;
        //        }
        //    }
        //}
        DataTable dtUserRoleId = new DataTable();
        if (BRM_Post.CheckUserRoleId(row["Uid"].ToString(), ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).roles, ref dtUserRoleId))
        {
            if (dtUserRoleId.Rows.Count > 0)
            {
                blDelFlg = true;
            }
        }

        //*卡片是否結案選項控制
        if (row["EndCaseFlg"].ToString() == "Y")
        {
            this.txtbackdate.Enable = false;
            this.btnPrintDate.Enabled = false;
            this.chkdelete.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnFinish.Enabled = false;
            this.txtnote.Enabled = false;
            this.LibPhrase.Enabled = false;
        }
        else
        {
            this.txtbackdate.Enable = true;
            this.btnPrintDate.Enabled = true;
            if (chkdelete.Enabled)
            {
                if (row["Stateflg"].ToString() == "N" && blDelFlg)
                {
                    this.chkdelete.Enabled = true;
                }
                else
                {
                    this.chkdelete.Enabled = false;
                }
            }

            if (btnUpdate.Enabled && blDelFlg)
            {
                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
            }


            this.btnFinish.Enabled = true;
            this.txtnote.Enabled = true;
            this.LibPhrase.Enabled = true;
        }
        //*加載片語
        BindPhrase();

    }

    /// <summary>
    /// 功能說明:確定更新或者刪除事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        UpdateOrDelete(false);
    }

    /// <summary>
    /// 功能說明:確定更新或者刪除
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    public void UpdateOrDelete(bool blnIsForm)
    {
        string strMsgID = string.Empty;
        Entity_Post Post = new Entity_Post();

        SqlHelper sqlhelp = new SqlHelper();
        if (chkdelete.Checked == true)
        {
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06040100_033");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "D");    

            Post.Sno = int.Parse(m_Sno);
            sqlhelp.AddCondition(Entity_Post.M_Sno, Operator.Equal, DataTypeUtils.Integer, Post.Sno.ToString());

            if (!BRM_Post.Delete(Post, sqlhelp.GetFilterCondition(), ref strMsgID))
            {
                //* 刪除不成功時，提示訊息
                MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
                return;
            }
            else
            {
                if (!blnIsForm)
                {
                    MessageHelper.ShowMessageAndGoto(UpdatePanel1, "P060401000001.aspx", strMsgID);
                }
                else
                {
                    MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
                }
            }
        }
        else
        {
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06040100_030");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

            if (string.IsNullOrEmpty(this.txtbackdate.Text.Trim()))
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_010");
                return;
            }
            if (string.IsNullOrEmpty(this.txtnote.Text.Trim()))
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_009");
                return;
            }

            Post.Sno =int.Parse(m_Sno);
            Post.Backdate = this.txtbackdate.Text.Trim();
            Post.Note = this.txtnote.Text.Trim();

            sqlhelp.AddCondition(Entity_Post.M_Sno, Operator.Equal, DataTypeUtils.Integer, Post.Sno.ToString());

            if (!BRM_Post.Update(Post, sqlhelp.GetFilterCondition(), ref strMsgID, "Backdate", "Note"))
            {
                //* 修改不成功時，提示訊息
                MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
                return;
            }
            else
            {
                if (!blnIsForm)
                {
                    MessageHelper.ShowMessageAndGoto(UpdatePanel1, "P060401000001.aspx", strMsgID);
                }
                else
                {
                    MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
                }
            }
        }
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
    /// 功能說明:結案操作
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnFinish_Click(object sender, EventArgs e)
    {
        SqlHelper sqlhelp = new SqlHelper();
        string strMsgID = string.Empty;
        Entity_Post Post = new Entity_Post();
        Post.Sno = int.Parse(m_Sno);
        Post.EndCaseFlg = "Y";

        sqlhelp.AddCondition(Entity_Post.M_Sno, Operator.Equal, DataTypeUtils.Integer, Post.Sno.ToString());

        if (!BRM_Post.Update(Post, sqlhelp.GetFilterCondition(), ref strMsgID, "EndCaseFlg"))
        {
            //* 結案不成功時，提示訊息
            MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_016");
            return;
        }
        else
        {
            MessageHelper.ShowMessageAndGoto(UpdatePanel1, "P060401000001.aspx", "06_06040100_015");
        }
    }

    /// <summary>
    /// 功能說明:回函日选择
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrintDate_Click(object sender, EventArgs e)
    {
        this.txtbackdate.Text = DateHelper.Today.ToString("yyyy/MM/dd");
    }

    /// <summary>
    /// 功能說明:刪除選擇事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkdelete_CheckedChanged(object sender, EventArgs e)
    {
        if (this.chkdelete.Checked == true)
        {
            this.btnUpdate.ShowID = "06_06040100_032";
            this.btnCreateForm.Enabled = false;
            this.btnFinish.Enabled = false;
        }
        else
        {
            this.btnUpdate.ShowID = "06_06040100_014";
            this.btnCreateForm.Enabled = true;
            this.btnFinish.Enabled = true;
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
        //*寫入產檔時間
        string strMsgID = string.Empty;
        Entity_Post Post = new Entity_Post();
        SqlHelper sqlhelp = new SqlHelper();
        Post.Sno = int.Parse(m_Sno);
        Post.OutPutDate = DateTime.Now.ToString("yyyy/MM/dd");
        Post.Stateflg = "Y";
        sqlhelp.AddCondition(Entity_Post.M_Sno, Operator.Equal, DataTypeUtils.Integer, Post.Sno.ToString());
        BRM_Post.Update(Post, sqlhelp.GetFilterCondition(), ref strMsgID, "OutPutDate", "Stateflg");

        try
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            #region 查詢條件參數

            if (m_cardno != null && m_id != null && m_action != null)
            {
                param.Add("CardNo", m_cardno);
                param.Add("Id", m_id);
                param.Add("Action", m_action);
            }
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06040100_028") + "');history.go(-1)");
                return;
            }

            #endregion

            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0401Report(param, ref strServerPathFile, ref strMsgID);

            if (result)
            {
                FileInfo fs = new FileInfo(strServerPathFile);
                Session["ServerFile"] = strServerPathFile;
                Session["ClientFile"] = fs.Name;
                string urlString = @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
            else
            {
                MessageHelper.ShowMessage(this, strMsgID);
            }
        }
        catch (Exception exp)
        {
            MessageHelper.ShowMessage(this, "06_06040100_031" + exp.Message);
        }
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
    /// 功能說明:Action轉換為中文顯示
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strCode"></param>
    /// <param name="strAction"></param>
    public void TranAction(string strCode, ref string strAction)
    {
        DataTable dtCode = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtCode))
        {
            if (dtCode.Rows.Count>0)
            {
                DataRow[] rowCode = dtCode.Select("PROPERTY_CODE='" + strCode + "'");

                if (rowCode != null && rowCode.Length > 0)
                {
                    strAction = rowCode[0]["PROPERTY_CODE"].ToString() + " " + rowCode[0]["PROPERTY_NAME"].ToString();
                }
                else
                {
                    strAction = strCode;
                }
  
            }
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
                this.txtnote.Text += DateTime.Now.ToString("yyyy/MM/dd");
                break;
            case "2":
                this.txtnote.Text += DateTime.Now.ToString("HH:mm:ss");
                break;
            case "3":
                this.txtnote.Text += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                break;
            default:
                this.txtnote.Text += this.LibPhrase.SelectedItem.Text.Trim();
                break;
        }
    }
}
