using System;
using System.Data;
using System.Web.UI.WebControls;
using EntityLayer;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;

public partial class P060205000003 : System.Web.UI.Page
{
    #region table
    public DataTable m_dtCardBaseInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }
    public DataTable m_dtPostSend
    {
        get { return ViewState["m_dtPostSend"] as DataTable; }
        set { ViewState["m_dtPostSend"] = value; }
    }
    public DataTable m_dtCardDataChange
    {
        get { return ViewState["m_dtCardDataChange"] as DataTable; }
        set { ViewState["m_dtCardDataChange"] = value; }
    }
    #endregion

    #region params
    public string m_Action
    {
        get { return ViewState["m_Action"] as string; }
        set { ViewState["m_Action"] = value; }
    }
    public string m_Id
    {
        get { return ViewState["m_Id"] as string; }
        set { ViewState["m_Id"] = value; }
    }
    public string m_CardNo
    {
        get { return ViewState["m_CardNo"] as string; }
        set { ViewState["m_CardNo"] = value; }
    }
    public string m_Trandate
    {
        get { return ViewState["m_Trandate"] as string; }
        set { ViewState["m_Trandate"] = value; }
    }
    public string m_Kind
    {
        get { return ViewState["m_Kind"] as string; }
        set { ViewState["m_Kind"] = value; }
    }
    public string m_SelfPickType
    {
        get { return ViewState["m_SelfPickType"] as string; }
        set { ViewState["m_SelfPickType"] = value; }
    }
    public string m_SelfPickName
    {
        get { return ViewState["m_SelfPickName"] as string; }
        set { ViewState["m_SelfPickName"] = value; }
    }
    public string m_Status
    {
        get { return ViewState["m_Status"] as string; }
        set { ViewState["m_Status"] = value; }
    }
    public string m_IntoStoreDate
    {
        get { return ViewState["m_IntoStoreDate"] as string; }
        set { ViewState["m_IntoStoreDate"] = value; }
    }
    #endregion

    #region SNo
    public string m_SNoN
    {
        get { return ViewState["m_SNoN"] as string; }
        set { ViewState["m_SNoN"] = value; }
    }
    public string m_SNoP
    {
        get { return ViewState["m_SNoP"] as string; }
        set { ViewState["m_SNoP"] = value; }
    }
    public string m_SNoA
    {
        get { return ViewState["m_SNoA"] as string; }
        set { ViewState["m_SNoA"] = value; }
    }
    public string m_SNoM
    {
        get { return ViewState["m_SNoM"] as string; }
        set { ViewState["m_SNoM"] = value; }
    }
    public string m_SNoC
    {
        get { return ViewState["m_SNoC"] as string; }
        set { ViewState["m_SNoC"] = value; }
    }
    public string m_SNoG
    {
        get { return ViewState["m_SNoG"] as string; }
        set { ViewState["m_SNoG"] = value; }
    }
    #endregion

    #region 事件
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
            string straction = Request.QueryString["action"];
            string strid = Request.QueryString["id"];
            string strcardno = Request.QueryString["cardno"];
            string strtrandate = Request.QueryString["trandate"];
            string strintostoredate = Request.QueryString["intostoredate"];
            string strmodifyflg = Request.QueryString["modifyflg"];

            this.btnUpdateC.Visible = false;
            this.btnUpdateA.Visible = false;
            this.btnUpdateG.Visible = false;

            pnlBackInfo2.Visible = false;
            pnlBackInfo3.Visible = false;

            //if (!straction.Equals(string.Empty) && !strid.Equals(string.Empty) && !strcardno.Equals(string.Empty) && !strtrandate.Equals(string.Empty))
            //{
            if (!straction.Equals(string.Empty) && !strcardno.Equals(string.Empty) && !strtrandate.Equals(string.Empty))
            {
                //* 傳遞參數解密
                this.m_Action = RedirectHelper.GetDecryptString(straction);
                this.m_Id = RedirectHelper.GetDecryptString(strid);
                this.m_CardNo = RedirectHelper.GetDecryptString(strcardno);
                this.m_Trandate = RedirectHelper.GetDecryptString(strtrandate);
                this.m_IntoStoreDate = RedirectHelper.GetDecryptString(strintostoredate);


                if (!strmodifyflg.Equals(string.Empty))
                {
                    if (RedirectHelper.GetDecryptString(strmodifyflg) == "0")
                    {
                        this.btnUpdateC.Visible = false;
                        this.btnUpdateA.Visible = false;
                        this.btnUpdateG.Visible = false;
                        this.btnUpdateM.Visible = false;
                    }

                }
                BindData();
                m_Status = "Y";
            }
            //else
            //{
            //    if (Session["backpage"].ToString().Trim() == "0501")
            //    {
            //        MessageHelper.ShowMessageAndGoto(this.UpdatePanel1,"P060205000001.ASPX", "06_06020102_016");                    
            //    }
            //    if (Session["backpage"].ToString().Trim() == "0601")
            //    {
            //        MessageHelper.ShowMessageAndGoto(this.UpdatePanel1, "P060206000001.ASPX", "06_06020102_016");  
            //    }
            //    m_Status = "Y";

            //}            
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
        if (Request.QueryString["backpage"] != null)
        {
            if (RedirectHelper.GetDecryptString(Request.QueryString["backpage"].ToString()) == "0501")
            {
                string strRadCheckFlg0205 = Session["RadCheckFlg0205"].ToString().Trim();
                string strFetchDate0205 = Session["FetchDate0205"].ToString().Trim();
                string strMerchDate0205 = Session["MerchDate0205"].ToString().Trim();
                string strMerch0205 = Session["Merch0205"].ToString().Trim();                
                string strId0205 = Session["Id0205"].ToString().Trim();
                string strCardNo0205 = Session["CardNo0205"].ToString().Trim();
                string strFromDate0205 = Session["FromDate0205"].ToString().Trim();
                string strToDate0205 = Session["ToDate0205"].ToString().Trim();
                string strPageIndex0205 = Session["PageIndex0205"].ToString().Trim();
                Response.Redirect("P060205000001.aspx?RadCheckFlg0205=" + RedirectHelper.GetEncryptParam(strRadCheckFlg0205) + " &FetchDate0205=" + RedirectHelper.GetEncryptParam(strFetchDate0205) + " &MerchDate0205=" + RedirectHelper.GetEncryptParam(strMerchDate0205) + " &Merch0205=" + RedirectHelper.GetEncryptParam(strMerch0205) + " &Id0205=" + RedirectHelper.GetEncryptParam(strId0205) + " &CardNo0205=" + RedirectHelper.GetEncryptParam(strCardNo0205) + " &FromDate0205=" + RedirectHelper.GetEncryptParam(strFromDate0205) + " &ToDate0205=" + RedirectHelper.GetEncryptParam(strToDate0205) + " &PageIndex0205=" + RedirectHelper.GetEncryptParam(strPageIndex0205) + "");
            }
            if (RedirectHelper.GetDecryptString(Request.QueryString["backpage"].ToString()) == "0601")
            {
                string strId0206 = Session["Id0206"].ToString().Trim();
                string strStusts0206 = Session["Stusts0206"].ToString().Trim();
                string strPageIndex0206 = Session["PageIndex0206"].ToString().Trim();
                Response.Redirect("P060206000001.aspx?Id0206=" + RedirectHelper.GetEncryptParam(strId0206) + " &Stusts0206=" + RedirectHelper.GetEncryptParam(strStusts0206) + " &PageIndex0206=" + RedirectHelper.GetEncryptParam(strPageIndex0206) + "");
            }
            jsBuilder.RegScript(this.UpdatePanel1, "");
        }
    }


    /// <summary>
    /// 功能說明:GridView行绑定事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustButton custbtn = e.Row.Cells[2].FindControl("btnSure") as CustButton;
            TextBox txtBox = e.Row.Cells[1].FindControl("txtcNote") as TextBox;
            TextBox txtUser = e.Row.Cells[3].FindControl("txtUser") as TextBox;
            if (Request.QueryString["modifyflg"] != null)
            {
                if (RedirectHelper.GetDecryptString(Request.QueryString["modifyflg"].ToString()) == "0")
                {
                    custbtn.Visible = false;
                }

            }
            if (!txtUser.Text.Equals(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id))
            {
                custbtn.Enabled = false;
                txtBox.Enabled = false;
            }
            else
            {
                custbtn.Enabled = true;
                txtBox.Enabled = true;
            }

        }

    }

    /// <summary>
    /// 功能說明:更新備注
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string strMsgID = string.Empty;
        CustButton btnSure = grvUserView.Rows[e.NewEditIndex].Cells[2].FindControl("btnSure") as CustButton;
        TextBox txtcNote = grvUserView.Rows[e.NewEditIndex].Cells[1].FindControl("txtcNote") as TextBox;
        if (txtcNote == null || string.IsNullOrEmpty(txtcNote.Text.Trim()))
        {
            //*備註不通過
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_000");
            return;
        }
        if (btnSure != null && txtcNote != null)
        {
            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
            //*修改異動單備註
            CardDataChange.Sno = int.Parse(btnSure.CommandArgument.ToString());
            CardDataChange.CNote = txtcNote.Text.Trim();
            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChange.Sno.ToString());
            
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_023");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
            
            
            if (BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "CNote"))
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_001");
                BindData();

            }
            else
            {
                MessageHelper.ShowMessage(UpdatePanel1, "06_06020104_002");
            }
        }
    }

    /// <summary>
    /// 功能說明:回上頁
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/07
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["backpage"] != null)
        {
            if (RedirectHelper.GetDecryptString(Request.QueryString["backpage"].ToString()) == "0501")
            {
                string strRadCheckFlg0205 = Session["RadCheckFlg0205"].ToString().Trim();
                string strFetchDate0205 = Session["FetchDate0205"].ToString().Trim();
                string strMerchDate0205 = Session["MerchDate0205"].ToString().Trim();
                string strMerch0205 = Session["Merch0205"].ToString().Trim();
                string strId0205 = Session["Id0205"].ToString().Trim();
                string strCardNo0205 = Session["CardNo0205"].ToString().Trim();
                string strFromDate0205 = Session["FromDate0205"].ToString().Trim();
                string strToDate0205 = Session["ToDate0205"].ToString().Trim();
                string strPageIndex0205 = Session["PageIndex0205"].ToString().Trim();
                Response.Redirect("P060205000001.aspx?RadCheckFlg0205=" + RedirectHelper.GetEncryptParam(strRadCheckFlg0205) + " &FetchDate0205=" + RedirectHelper.GetEncryptParam(strFetchDate0205) + " &MerchDate0205=" + RedirectHelper.GetEncryptParam(strMerchDate0205) + " &Merch0205=" + RedirectHelper.GetEncryptParam(strMerch0205) + " &Id0205=" + RedirectHelper.GetEncryptParam(strId0205) + " &CardNo0205=" + RedirectHelper.GetEncryptParam(strCardNo0205) + " &FromDate0205=" + RedirectHelper.GetEncryptParam(strFromDate0205) + " &ToDate0205=" + RedirectHelper.GetEncryptParam(strToDate0205) + " &PageIndex0205=" + RedirectHelper.GetEncryptParam(strPageIndex0205) + "");
            }
            if (RedirectHelper.GetDecryptString(Request.QueryString["backpage"].ToString()) == "0601")
            {
                string strId0206 = Session["Id0206"].ToString().Trim();
                string strStusts0206 = Session["Stusts0206"].ToString().Trim();
                string strPageIndex0206 = Session["PageIndex0206"].ToString().Trim();
                Response.Redirect("P060206000001.aspx?Id0206=" + RedirectHelper.GetEncryptParam(strId0206) + " &Stusts0206=" + RedirectHelper.GetEncryptParam(strStusts0206) + " &PageIndex0206=" + RedirectHelper.GetEncryptParam(strPageIndex0206) + "");
            }
            jsBuilder.RegScript(this.UpdatePanel1, "");
        }
    }

    /// <summary>
    /// 功能說明:修改地址(1,2,3)
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureA_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        bool blnCardBaseInfoResult = false;
        //bool blnCardStockInfo = false;
        bool blnCardDataChange = false;

        Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_066");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
        CardBaseInfo.action = m_Action;
        if (!string.IsNullOrEmpty(m_Id))
        {
            CardBaseInfo.id = m_Id;
        }

        CardBaseInfo.cardno = m_CardNo;
        CardBaseInfo.trandate = m_Trandate;
        CardBaseInfo.zip = BaseHelper.ToSBC(this.lblNewzipAjax.Text.Trim());//*郵遞區號全碼
        CardBaseInfo.add1 = BaseHelper.ToSBC(this.CustAdd1.strAddress);     //*地址一全碼
        CardBaseInfo.add2 = BaseHelper.ToSBC(this.txtAdd2Ajax.Text.Trim());     //*地址二全碼
        CardBaseInfo.add3 = BaseHelper.ToSBC(this.txtAdd3Ajax.Text.Trim());     //*地址三全碼
        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160
        //CardBaseInfo.OutStore_Status = "1";
        //CardBaseInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");
        //修改領卡方式後，若再回來修改其他資料(地址、取卡方式、掛號號碼)，要把領卡方式、領卡日期清空 BUG120
        CardBaseInfo.SelfPick_Type = ""; 
        CardBaseInfo.SelfPick_Date = "";

        SqlHelper sqlhelp1 = new SqlHelper();
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, CardBaseInfo.action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp1.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, CardBaseInfo.id);
        }
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardBaseInfo.cardno);
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Equal, DataTypeUtils.String, CardBaseInfo.trandate);
        blnCardBaseInfoResult = BRM_TCardBaseInfo.Update(CardBaseInfo, sqlhelp1.GetFilterCondition(), ref strMsgID, "zip", "add1", "add2", "add3", "OutStore_Status", "OutStore_Date", "SelfPick_Type", "SelfPick_Date");

        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 START
        //if (blnCardBaseInfoResult)
        //{
        //    Entity_CardStockInfo CardStockInfo = new Entity_CardStockInfo();
        //    CardStockInfo.IntoStore_Date = m_IntoStoreDate;
        //    CardStockInfo.cardno = m_CardNo;
        //    CardStockInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");
        //    CardStockInfo.OutStore_Status = "郵寄";

        //    SqlHelper sqlhelp2 = new SqlHelper();
        //    sqlhelp2.AddCondition(Entity_CardStockInfo.M_IntoStore_Date, Operator.Equal, DataTypeUtils.String, CardStockInfo.IntoStore_Date);
        //    sqlhelp2.AddCondition(Entity_CardStockInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardStockInfo.cardno);
        //    blnCardStockInfo = BRM_CardStockInfo.update(CardStockInfo, sqlhelp2.GetFilterCondition(), ref strMsgID, "OutStore_Status", "OutStore_Date");
        //}
        
        //if (blnCardBaseInfoResult && blnCardStockInfo)
        //{        
        if (blnCardBaseInfoResult)
        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 END
        {
            //*新增異動單
            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();

            string strUserName = string.Empty;
            BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);

            CardDataChange.action = m_Action;
            if (!string.IsNullOrEmpty(m_Id))
            {
                CardDataChange.id = m_Id;
            }
            CardDataChange.indate1 = lblIndate1.Text.Trim();
            CardDataChange.CardNo = m_CardNo;
            CardDataChange.Trandate = m_Trandate;

            CardDataChange.OldZip = this.lblOldzipAjax.Text.Trim();
            CardDataChange.NewZip = BaseHelper.ToSBC(this.lblNewzipAjax.Text.Trim());//*郵遞區號全碼
            CardDataChange.OldAdd1 = this.lblAdd1Ajax.Text.Trim();
            CardDataChange.NewAdd1 = BaseHelper.ToSBC(this.CustAdd1.strAddress);     //*地址一全碼
            CardDataChange.OldAdd2 = this.lblAdd2Ajax.Text.Trim();
            CardDataChange.NewAdd2 = BaseHelper.ToSBC(this.txtAdd2Ajax.Text.Trim()); //*地址二全碼
            CardDataChange.OldAdd3 = this.lblAdd3Ajax.Text.Trim();
            CardDataChange.NewAdd3 = BaseHelper.ToSBC(this.txtAdd3Ajax.Text.Trim()); //*地址三全碼

            if (!CardDataChange.OldAdd1.Equals(CardDataChange.NewAdd1))
            {
                CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址一", CardDataChange.OldAdd1, CardDataChange.NewAdd1, "");//*異動記錄說明 
            }

            if (!CardDataChange.OldAdd2.Equals(CardDataChange.NewAdd2))
            {
                if (string.IsNullOrEmpty(CardDataChange.NoteCaptions))
                {
                    CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址二", CardDataChange.OldAdd2, CardDataChange.NewAdd2, "");//*異動記錄說明 
                }
                else
                {
                    CardDataChange.NoteCaptions += ";  " + MessageHelper.GetMessage("06_06020104_006", "地址二", CardDataChange.OldAdd2, CardDataChange.NewAdd2);//*異動記錄說明 
                }
            }

            if (!CardDataChange.OldAdd3.Equals(CardDataChange.NewAdd3))
            {
                if (string.IsNullOrEmpty(CardDataChange.NoteCaptions))
                {
                    CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "地址三", CardDataChange.OldAdd3, CardDataChange.NewAdd3, "");//*異動記錄說明 
                }
                else
                {
                    CardDataChange.NoteCaptions += ";  " + MessageHelper.GetMessage("06_06020104_006", "地址三", CardDataChange.OldAdd3, CardDataChange.NewAdd3);//*異動記錄說明 
                }
            }

            if (null != CardDataChange.NoteCaptions && !string.IsNullOrEmpty(CardDataChange.NoteCaptions))
            {
                CardDataChange.NoteCaptions = CardDataChange.NoteCaptions.Replace('(', ' ').Replace(')', ' ').Replace(" ", "") + "(" + strUserName + ")";
            }
            
            CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
            CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            CardDataChange.OutputFlg = "N";
            CardDataChange.SourceType = "1";
            blnCardDataChange = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        }
        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 START
        //if (blnCardBaseInfoResult && blnCardStockInfo && blnCardDataChange)
        if (blnCardBaseInfoResult && blnCardDataChange)
        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 END
        {
            this.BindData();
            this.UpdatePanel1.Update();
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020502_000"));
        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020103_000") + "')");
        }
    }
    /// <summary>
    /// 功能說明:領卡方式
    /// 作    者:Linda
    /// 創建時間:2010/06/29
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureM_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strSelfPickAjax = this.dropSelfPickAjax.SelectedItem.Value;
        bool blnCardBaseInfoResult = false;
        bool blnCardStockInfo = false;
        bool blnCardDataChange = false;

        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_024") + BaseHelper.GetShowText("06_06020101_078");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");


        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);

        Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
        CardBaseInfo.action = m_Action;
        if (!string.IsNullOrEmpty(m_Id))
        {
            CardBaseInfo.id = m_Id;
        }
        CardBaseInfo.cardno = m_CardNo;
        CardBaseInfo.trandate = m_Trandate;
        CardBaseInfo.SelfPick_Type = strSelfPickAjax;
        CardBaseInfo.SelfPick_Date = DateTime.Now.ToString("yyyy/MM/dd");
        CardBaseInfo.OutStore_Status = "1";
        CardBaseInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");

        SqlHelper sqlhelp1 = new SqlHelper();
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, CardBaseInfo.action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp1.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, CardBaseInfo.id);
        }
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardBaseInfo.cardno);
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Equal, DataTypeUtils.String, CardBaseInfo.trandate);
        blnCardBaseInfoResult = BRM_TCardBaseInfo.Update(CardBaseInfo, sqlhelp1.GetFilterCondition(), ref strMsgID, "SelfPick_Type", "SelfPick_Date", "OutStore_Status", "OutStore_Date");

        if (blnCardBaseInfoResult)
        {
            Entity_CardStockInfo CardStockInfo = new Entity_CardStockInfo();
            CardStockInfo.IntoStore_Date = m_IntoStoreDate;
            CardStockInfo.cardno = m_CardNo;
            CardStockInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");
            //CardStockInfo.OutStore_Status = "領卡";
            switch (strSelfPickAjax)
            {
                case "1":
                    CardStockInfo.OutStore_Status = "領卡";
                    break;
                case "2":
                    CardStockInfo.OutStore_Status = "領卡";
                    break;
                case "3":
                    CardStockInfo.OutStore_Status = "領卡";
                    break;
                case "22":
                    CardStockInfo.OutStore_Status = "領卡";
                    break;
                case "5":
                    CardStockInfo.OutStore_Status = "註銷";
                    break;
                case "4":
                    CardStockInfo.OutStore_Status = "郵寄-限掛";
                    CardStockInfo.mailno = this.txtMailNo.Text.Trim();
                    break;
            }

            SqlHelper sqlhelp2 = new SqlHelper();
            sqlhelp2.AddCondition(Entity_CardStockInfo.M_IntoStore_Date, Operator.Equal, DataTypeUtils.String, CardStockInfo.IntoStore_Date);
            sqlhelp2.AddCondition(Entity_CardStockInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardStockInfo.cardno);
            blnCardStockInfo = BRM_CardStockInfo.update(CardStockInfo, sqlhelp2.GetFilterCondition(), ref strMsgID, "OutStore_Status", "OutStore_Date", "mailno");
        }

        if (blnCardBaseInfoResult && blnCardStockInfo)
        {

            //*新增異動單
            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
            CardDataChange.action = m_Action;
            if (!string.IsNullOrEmpty(m_Id))
            {
                CardDataChange.id = m_Id;
            }
            CardDataChange.indate1 = lblIndate1.Text.Trim();
            CardDataChange.CardNo = m_CardNo;
            CardDataChange.Trandate = m_Trandate;
            CardDataChange.OldSelfPickType = m_SelfPickType;//*領卡方式只能寫入Code
            CardDataChange.NewWay = strSelfPickAjax;
            
            CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "領卡方式", m_SelfPickName,this.dropSelfPickAjax.SelectedItem.Text, strUserName);//*異動記錄說明

            CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
            CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            CardDataChange.OutputFlg = "N";
            CardDataChange.SourceType = "1";
            blnCardDataChange = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        }

        if (blnCardBaseInfoResult && blnCardStockInfo && blnCardDataChange)
        {
            this.BindData();
            this.UpdatePanel1.Update();
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020502_000"));
        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020103_000") + "')");
        }
    }

    /// <summary>
    /// 功能說明:修改取卡方式
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureC_Click(object sender, EventArgs e)
    {

        string strMsgID = string.Empty;
        bool blnCardBaseInfoResult = false;
        bool blnCardStockInfo = false;
        bool blnCardDataChange = false;

        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_058");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");


        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);
        
        Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
        CardBaseInfo.action = m_Action;
        if (!string.IsNullOrEmpty(m_Id))
        {
            CardBaseInfo.id = m_Id;
        }
        CardBaseInfo.cardno = m_CardNo;
        CardBaseInfo.trandate = m_Trandate;
        CardBaseInfo.kind = this.dropKindAjax.SelectedItem.Value;
        CardBaseInfo.OutStore_Status = "1";
        CardBaseInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");
        //修改領卡方式後，若再回來修改其他資料(地址、取卡方式、掛號號碼)，要把領卡方式、領卡日期清空 B120
        CardBaseInfo.SelfPick_Type = "";
        CardBaseInfo.SelfPick_Date = "";

        SqlHelper sqlhelp1 = new SqlHelper();
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, CardBaseInfo.action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp1.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, CardBaseInfo.id);
        }
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardBaseInfo.cardno);
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Equal, DataTypeUtils.String, CardBaseInfo.trandate);
        blnCardBaseInfoResult = BRM_TCardBaseInfo.Update(CardBaseInfo, sqlhelp1.GetFilterCondition(), ref strMsgID, "kind", "OutStore_Status", "OutStore_Date","SelfPick_Type","SelfPick_Date");

        if (blnCardBaseInfoResult)
        {
            Entity_CardStockInfo CardStockInfo = new Entity_CardStockInfo();
            CardStockInfo.IntoStore_Date = m_IntoStoreDate;
            CardStockInfo.cardno = m_CardNo;
            CardStockInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");
            switch (this.dropKindAjax.SelectedItem.Value)
            {
                case "0":
                    CardStockInfo.OutStore_Status = "郵寄-普掛";
                    break;
                case "3":
                    CardStockInfo.OutStore_Status = "郵寄-限掛";
                    break;
                case "4":
                    CardStockInfo.OutStore_Status = "郵寄-快遞";
                    break;
                case "10":
                    CardStockInfo.OutStore_Status = "註銷";
                    break;
                case "11":
                    CardStockInfo.OutStore_Status = "註銷";
                    break;

            }

            SqlHelper sqlhelp2 = new SqlHelper();
            sqlhelp2.AddCondition(Entity_CardStockInfo.M_IntoStore_Date, Operator.Equal, DataTypeUtils.String, CardStockInfo.IntoStore_Date);
            sqlhelp2.AddCondition(Entity_CardStockInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardStockInfo.cardno);
            blnCardStockInfo = BRM_CardStockInfo.update(CardStockInfo, sqlhelp2.GetFilterCondition(), ref strMsgID, "OutStore_Status", "OutStore_Date");
        }

        if (blnCardBaseInfoResult && blnCardStockInfo)
        {

            //*新增異動單
            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
            CardDataChange.action = m_Action;
            if (!string.IsNullOrEmpty(m_Id))
            {
                CardDataChange.id = m_Id;
            }
            CardDataChange.indate1 = lblIndate1.Text.Trim();
            CardDataChange.CardNo = m_CardNo;
            CardDataChange.Trandate = m_Trandate;
            CardDataChange.OldWay = m_Kind;//*取卡方式只能寫入Code
            CardDataChange.NewWay = this.dropKindAjax.SelectedValue;
            CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "取卡方式", this.lblKindAjax.Text.ToString().Trim(), this.dropKindAjax.SelectedItem.Text,strUserName);//*異動記錄說明
            CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
            CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            CardDataChange.OutputFlg = "N";
            CardDataChange.SourceType = "1";
            blnCardDataChange = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        }

        if (blnCardBaseInfoResult && blnCardStockInfo && blnCardDataChange)
        {
            this.BindData();
            this.UpdatePanel1.Update();
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020502_000"));
        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020103_000") + "')");
        }
    }

    /// <summary>
    /// 功能說明:修改掛號號碼
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSureG_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        bool blnCardBaseInfoResult = false;
        //bool blnCardStockInfo = false;
        bool blnCardDataChange = false;

        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_062");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");


        string strUserName = string.Empty;
        BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);

        Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
        CardBaseInfo.action = m_Action;
        if (!string.IsNullOrEmpty(m_Id))
        {
            CardBaseInfo.id = m_Id;
        }
        CardBaseInfo.cardno = m_CardNo;
        CardBaseInfo.trandate = m_Trandate;
        CardBaseInfo.mailno = this.txtMailnoAjax.Text.Trim();
        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 START
        //CardBaseInfo.OutStore_Status = "1";
        //CardBaseInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");
        //修改領卡方式後，若再回來修改其他資料(地址、取卡方式、掛號號碼)，要把領卡方式、領卡日期清空 B120
        CardBaseInfo.SelfPick_Type = "";
        CardBaseInfo.SelfPick_Date = "";

        SqlHelper sqlhelp1 = new SqlHelper();
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, CardBaseInfo.action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp1.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, CardBaseInfo.id);
        }
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardBaseInfo.cardno);
        sqlhelp1.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Equal, DataTypeUtils.String, CardBaseInfo.trandate);
        blnCardBaseInfoResult = BRM_TCardBaseInfo.Update(CardBaseInfo, sqlhelp1.GetFilterCondition(), ref strMsgID, "mailno", "OutStore_Status", "OutStore_Date", "SelfPick_Type", "SelfPick_Date");

        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 START
        //if (blnCardBaseInfoResult)
        //{
        //    Entity_CardStockInfo CardStockInfo = new Entity_CardStockInfo();
        //    CardStockInfo.IntoStore_Date = m_IntoStoreDate;
        //    CardStockInfo.cardno = m_CardNo;
        //    CardStockInfo.OutStore_Date = DateTime.Now.ToString("yyyy/MM/dd");
        //    CardStockInfo.OutStore_Status = "郵寄";

        //    SqlHelper sqlhelp2 = new SqlHelper();
        //    sqlhelp2.AddCondition(Entity_CardStockInfo.M_IntoStore_Date, Operator.Equal, DataTypeUtils.String, CardStockInfo.IntoStore_Date);
        //    sqlhelp2.AddCondition(Entity_CardStockInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardStockInfo.cardno);
        //    blnCardStockInfo = BRM_CardStockInfo.update(CardStockInfo, sqlhelp2.GetFilterCondition(), ref strMsgID, "OutStore_Status", "OutStore_Date");
        //}

        ////*新增異動單
        ////blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        //if (blnCardBaseInfoResult && blnCardStockInfo)
        //{        
        if (blnCardBaseInfoResult)
        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 END
        {
            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
            CardDataChange.action = m_Action;
            if (!string.IsNullOrEmpty(m_Id))
            {
                CardDataChange.id = m_Id;
            }
            CardDataChange.indate1 = lblIndate1.Text.Trim();
            CardDataChange.CardNo = m_CardNo;
            CardDataChange.Trandate = m_Trandate;
            CardDataChange.oldMailno = this.lblMailnoAjax.Text.Trim();
            CardDataChange.newmailno = this.txtMailnoAjax.Text.Trim();

            CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_003", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), "掛號號碼", this.lblMailnoAjax.Text.Trim(), this.txtMailnoAjax.Text.Trim(), strUserName);//*異動記錄說明
            CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
            CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            CardDataChange.OutputFlg = "N";
            CardDataChange.SourceType = "1";
            blnCardDataChange = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
        }

        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 START
        //if (blnCardBaseInfoResult && blnCardStockInfo && blnCardDataChange)
        if (blnCardBaseInfoResult && blnCardDataChange)
        //修改取卡方式及領卡方式才算是出庫，修改地址和掛號號碼只是update基本資料，不算出庫(也不記出庫日) BUG160 END
        {
            this.BindData();
            this.UpdatePanel1.Update();
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020502_000"));
        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020103_000") + "')");
        }
    }

    /// <summary>
    /// 功能說明:彈出地址一Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateA_Click(object sender, EventArgs e)
    {
        InitControls();
        m_Status = "N";
        this.ModalPopupExtenderA.Show();
    }
    /// <summary>
    /// 功能說明:彈出額度Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateM_Click(object sender, EventArgs e)
    {
        DataTable dtMailNo =new DataTable();
        InitControls();
        if(BRM_CardStockInfo.SearchMailNo(ref dtMailNo,m_CardNo,m_IntoStoreDate))
        {
            this.txtMailNo.Text=dtMailNo.Rows[0][0].ToString().Trim();
        }        

        this.ModalPopupExtenderM.Show();
    }
    /// <summary>
    /// 功能說明:彈出取卡方式Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateC_Click(object sender, EventArgs e)
    {
        InitControls();
        this.ModalPopupExtenderC.Show();
    }
    /// <summary>
    /// 功能說明:彈出掛號號碼Panel
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateG_Click(object sender, EventArgs e)
    {
        InitControls();
        this.ModalPopupExtenderG.Show();
    }

    #endregion

    #region 方法

    ///// <summary>
    ///// 功能說明:備注-異動欄位
    ///// 作    者:Simba Liu
    ///// 創建時間:2010/04/09
    ///// 修改記錄:
    ///// </summary>
    ///// <param name="strField">字段名稱</param>
    ///// <param name="blnIs">是否已有作業單 true 有  false 沒有</param>
    //private void SearchDataChange(string strField, ref bool blnIs, string strSource, ref string strResult, ref string strSno)
    //{
    //    DataTable dtCardDataChange = new DataTable();
    //    string strMsgID = string.Empty;
    //    SqlHelper sqlhelp = new SqlHelper();
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
    //    sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);
    //    if (BRM_CardDataChange.SearchByChange(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, strField))
    //    {
    //        //*如有未轉出作業單則用最近的一筆資料替代
    //        if (dtCardDataChange.Rows.Count > 0)
    //        {
    //            strResult = dtCardDataChange.Rows[0][strField].ToString();
    //            strSno = dtCardDataChange.Rows[0]["Sno"].ToString();
    //            blnIs = true;
    //        }
    //        else
    //        {
    //            strResult = strSource;
    //            blnIs = false;
    //        }
    //    }
    //}

  
    /// <summary>
    /// 功能說明:BindData
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void BindData()
    {
        //*標題列印
        ShowControlsText();
        //*資料明細
        SearchCardBaseInfo();
        //*資料明細加載
        BindDataToControls();
        //*備註加載
        SearchDataChange();
        //*最近一次退件資料加載
        SearchBack();
        //*綁定取卡方式
        BindKind();
        //*綁定領卡方式
        BindSelfPick();
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
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020101_084");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020101_021");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020101_022");
    }

    /// <summary>
    /// 功能說明:ModalPopupExtender confirm 
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    public void ModalPopupExtender()
    {
        ////*此處需要判斷是否已有轉出作業單
        //if (1 == 1)
        //{
        //    this.btnSureN.Attributes.Add("onclick", "return confirm('已有未轉出作業單，修改後姓名 : XXX')");
        //}
    }

    /// <summary>
    /// 功能說明:明細資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    private void BindDataToControls()
    {
        if (m_dtCardBaseInfo != null && m_dtCardBaseInfo.Rows.Count > 0)
        {
            //*明细资料绑定
            DataRow row = m_dtCardBaseInfo.Rows[0];

            if (null != row["Id"] && !string.IsNullOrEmpty(row["Id"].ToString()))
            {
                this.lblId.Text = row["Id"].ToString(); //*身分證字號
            }
            this.lblname1.Text = row["custname"].ToString();  //*寄件人姓名
            this.lblTrandate.Text = row["Trandate"].ToString(); //*轉檔日
            this.lblIndate1.Text = row["Indate1"].ToString(); //*製卡日
            this.lblMaildate.Text = row["Maildate"].ToString(); //*　郵寄日
            this.lblCardno.Text = row["Cardno"].ToString(); //*卡號一 
            this.lblExpdate.Text = row["Expdate"].ToString(); //*有效期限一

            //*郵遞區號
            string strSourceZip = row["Zip"].ToString();
            this.lblZip.Text = strSourceZip; 
            this.lblOldzipAjax.Text = strSourceZip;

            //*地址一
            string strSourceAdd1 = row["Add1"].ToString();
            this.lblAdd1.Text = strSourceAdd1;
            this.lblAdd1Ajax.Text = strSourceAdd1;

            //*地址二
            string strSourceAdd2 = row["Add2"].ToString();
            this.lblAdd2.Text = strSourceAdd2;
            this.lblAdd2Ajax.Text = strSourceAdd2;

            //*地址三

            string strSourceAdd3 = string.Empty;
            if (row["Add3"] != null)
            {
                strSourceAdd3 = row["Add3"].ToString();
                this.lblAdd3.Text = strSourceAdd3;
                this.lblAdd3Ajax.Text = strSourceAdd3;
            }

            string strSelfPick_Type = row["SelfPick_Type"].ToString();
            string strSelfPick_Name = string.Empty;
            GetKindName("4", strSelfPick_Type, ref strSelfPick_Name);
            this.lblSelfPick_Type.Text = strSelfPick_Name; //*領卡方式 
            this.lblSelfPick_Date.Text = row["SelfPick_Date"].ToString(); //*領卡日期
            m_SelfPickType = strSelfPick_Type;
            m_SelfPickName = strSelfPick_Name;

            string strAction = string.Empty;
            GetKindName("1", row["Action"].ToString(), ref strAction);
            this.lblAction.Text = strAction; //*卡別

            this.lblCardtype.Text = row["Cardtype"].ToString(); //*TYPE

            if (null != row["Affinity"] && !string.IsNullOrEmpty(row["Affinity"].ToString()))
            {
                string strAffinity = string.Empty;
                GetAffName(row["Affinity"].ToString(), ref strAffinity);
                this.lblAffinity.Text = strAffinity; //*認同代碼
            }


            if (null != row["Photo"] && !string.IsNullOrEmpty(row["Photo"].ToString()))
            {
                string strPhoto = string.Empty;
                GetKindName("3", row["Photo"].ToString(), ref strPhoto);
                this.lblPhoto.Text = strPhoto; //*PHOTO
            }

            this.lblMonlimit.Text = row["Monlimit"].ToString();//*額度
            this.lblCardno2.Text = row["Cardno2"].ToString(); //*卡號二
            this.lblExpdate2.Text = row["Expdate2"].ToString(); //*有效期限

            string strSourceKind = row["Kind"].ToString();
            m_Kind = strSourceKind;
            string strKindName = string.Empty;
            GetKindName("2", strSourceKind, ref strKindName);
            this.lblKind.Text = strKindName; //*取卡方式
            this.lblKindAjax.Text = strKindName;

            //bool blUrgencyFlg = true;
            string strUrgencyFlg = string.Empty;
            string strResultUrgency = string.Empty;
            string strSnoUrg = string.Empty;
            if (null != row["Urgency_Flg"] && !string.IsNullOrEmpty(row["Urgency_Flg"].ToString()))
            {
                strUrgencyFlg = row["Urgency_Flg"].ToString();
            }
            
            //SearchDataChange("UrgencyFlg", ref blUrgencyFlg, strUrgencyFlg, ref strResultUrgency, ref strSnoUrg);

            //if (!string.IsNullOrEmpty(strResultUrgency))
            //{
            //    if (strResultUrgency.Equals("1"))
            //    {
            //        chkUrgencyFlg.Checked = true;
            //    }
            //    else
            //    {
            //        chkUrgencyFlg.Checked = false;
            //    }
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(strUrgencyFlg))
            //    {
            //        if (strUrgencyFlg.Equals("1"))
            //        {
            //            chkUrgencyFlg.Checked = true;
            //        }
            //        else
            //        {
            //            chkUrgencyFlg.Checked = false;
            //        }
            //    }
            //}

            string strSourceMailno = row["Mailno"].ToString();
            this.lblMailno.Text = strSourceMailno; //*掛號號碼
            this.lblMailnoAjax.Text = strSourceMailno;

            this.lblMerch_Code.Text = row["Merch_CodeS"].ToString(); //*製卡廠名稱

            //*來源于Post_Send
            string strSend_status_Name = string.Empty;
            string strM_date = string.Empty;
            string strPost_Name = string.Empty;
            bool blnPost = false;
            SearchPostSend(row["Mailno"].ToString(), row["Maildate"].ToString(), ref strSend_status_Name, ref strM_date, ref strPost_Name, ref blnPost);
            this.lblSend_status_Name.Text = strSend_status_Name; //*郵件狀態
            this.lblM_dates.Text = strM_date; //*郵局處理日
            this.lblPost_Name.Text = strPost_Name; //*招領郵局
            this.lblPost_Name.Visible = blnPost;

        }
    }

    /// <summary>
    /// 功能說明:備注-異動欄位
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    private void SearchCardBaseInfo()
    {
        DataTable dtCardBaseInfo = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Like, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Like, DataTypeUtils.String, m_Trandate);
        if (BRM_TCardBaseInfo.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardBaseInfo, ref strMsgID))
        {
            MergeTable(ref  dtCardBaseInfo);
            m_dtCardBaseInfo = dtCardBaseInfo;
        }
    }

    /// <summary>
    /// 功能說明:備注-異動欄位
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    private void SearchDataChange()
    {
        DataTable dtCardDataChange = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);
        //sqlhelp.AddOrderCondition(Entity_CardDataChange.M_Sno, ESortType.DESC);
        if (BRM_CardDataChange.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID))
        {
            this.grvUserView.DataSource = dtCardDataChange;
            this.grvUserView.DataBind();
        }
    }

    /// <summary>
    /// 功能說明:備注-異動欄位
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strField">字段名稱</param>
    /// <param name="blnIs">是否已有作業單 true 有  false 沒有</param>
    private void SearchDataChange(string strField, ref bool blnIs, string strSource, ref string strResult, ref string strSno)
    {
        DataTable dtCardDataChange = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);

        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);
        
        if (BRM_CardDataChange.SearchByChange(sqlhelp.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, strField))
        {
            //*如有未轉出作業單則用最近的一筆資料替代
            if (dtCardDataChange.Rows.Count > 0)
            {
                strResult = dtCardDataChange.Rows[0][strField].ToString();
                strSno = dtCardDataChange.Rows[0]["Sno"].ToString();
                blnIs = true;
            }
            else
            {
                strResult = strSource;
                blnIs = false;
            }
        }
    }

    /// <summary>
    /// 功能說明:最後一次退件資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    private void SearchBack()
    {
        DataTable dtCardBackInfo = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, m_Action);
        if (!string.IsNullOrEmpty(m_Id))
        {
            sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, m_Id);
        }
        sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, m_CardNo);
        sqlhelp.AddCondition(Entity_CardDataChange.M_Trandate, Operator.Equal, DataTypeUtils.String, m_Trandate);

        if (BRM_CardBackInfo.SearchByCardNo(sqlhelp.GetFilterCondition(), ref dtCardBackInfo, ref strMsgID))
        {
            if (dtCardBackInfo.Rows.Count > 0)
            {
                ////*最後一次退件資料绑定
                //DataRow row = dtCardBackInfo.Rows[0];
                //this.lblbackdate.Text = row["Backdate"].ToString();                   //退件時間
                //this.lblreason.Text = GetBackName(row["Reason"].ToString());           //退件原因
                //this.lblm_date.Text = row["Enddate"].ToString();                      //處理日期
                //this.lblpreenditem.Text = GetOperactionName(row["Enditem"].ToString());//處理方式
                //this.txtRemarkFB.Text = row["Endnote"].ToString();                    //備注
                //this.lblEndDate.Text = row["Closedate"].ToString();                   //結案日期

                ////*最近三次退件資料 start*////
                for (int intbackinfo = 0; intbackinfo < dtCardBackInfo.Rows.Count; intbackinfo++)
                {
                    if (intbackinfo > 2)
                    {
                        break;
                    }
                    switch (intbackinfo)
                    {
                        case 0:
                            pnlBackInfo1.Visible=true;
                            this.lblbackdate.Text = dtCardBackInfo.Rows[0]["Backdate"].ToString();                   //退件時間
                            this.lblreason.Text = GetBackName(dtCardBackInfo.Rows[0]["Reason"].ToString());           //退件原因
                            this.lblm_date.Text = dtCardBackInfo.Rows[0]["Enddate"].ToString();                      //處理日期
                            this.lblpreenditem.Text = GetOperactionName(dtCardBackInfo.Rows[0]["Enditem"].ToString());//處理方式
                            this.txtRemarkFB.Text = dtCardBackInfo.Rows[0]["Endnote"].ToString();                    //備注
                            this.lblEndDate.Text = dtCardBackInfo.Rows[0]["Closedate"].ToString();                   //結案日期
                            break;
                        case 1:
                            pnlBackInfo2.Visible = true;
                            this.lblbackdate2.Text = dtCardBackInfo.Rows[1]["Backdate"].ToString();                   //退件時間
                            this.lblreason2.Text = GetBackName(dtCardBackInfo.Rows[1]["Reason"].ToString());           //退件原因
                            this.lblm_date2.Text = dtCardBackInfo.Rows[1]["Enddate"].ToString();                      //處理日期
                            this.lblpreenditem2.Text = GetOperactionName(dtCardBackInfo.Rows[1]["Enditem"].ToString());//處理方式
                            this.txtRemarkFB2.Text = dtCardBackInfo.Rows[1]["Endnote"].ToString();                    //備注
                            this.lblEndDate2.Text = dtCardBackInfo.Rows[1]["Closedate"].ToString();                   //結案日期
                            break;
                        case 2:
                            pnlBackInfo3.Visible = true;
                            this.lblbackdate3.Text = dtCardBackInfo.Rows[2]["Backdate"].ToString();                   //退件時間
                            this.lblreason3.Text = GetBackName(dtCardBackInfo.Rows[2]["Reason"].ToString());           //退件原因
                            this.lblm_date3.Text = dtCardBackInfo.Rows[2]["Enddate"].ToString();                      //處理日期
                            this.lblpreenditem3.Text = GetOperactionName(dtCardBackInfo.Rows[2]["Enditem"].ToString());//處理方式
                            this.txtRemarkFB3.Text = dtCardBackInfo.Rows[2]["Endnote"].ToString();                    //備注
                            this.lblEndDate3.Text = dtCardBackInfo.Rows[2]["Closedate"].ToString();                   //結案日期
                            break;
                    }
                }
                ////*最近三次退件資料 end*////
            }
        }
    }

    /// <summary>
    /// 功能說明:查詢郵局資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strMailno">郵寄號碼</param>
    /// <param name="strMailDate">交寄日期</param>
    /// <param name="strSend_status_Name">郵件狀態</param>
    /// <param name="strM_date">郵局處理日</param>
    /// <param name="strPost_Name">招領郵局</param>
    private void SearchPostSend(string strMailno, string strMailDate, ref string strSend_status_Name, ref string strM_date, ref string strPost_Name, ref bool blnPost)
    {
        DataTable dtPostSend = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityM_PostSend.M_mailno, Operator.Equal, DataTypeUtils.String, strMailno);
        sqlhelp.AddCondition(EntityM_PostSend.M_maildate, Operator.Equal, DataTypeUtils.String, strMailDate);

        if (BRM_PostSend.SearchByMailNo(sqlhelp.GetFilterCondition(), ref dtPostSend, ref strMsgID))
        {
            if (dtPostSend.Rows.Count > 0)
            {
                strSend_status_Name = dtPostSend.Rows[0]["Info1"].ToString() + dtPostSend.Rows[0]["Send_status_Name"].ToString();//*資訊代號中文+郵件狀態中文)
                if (!string.IsNullOrEmpty(dtPostSend.Rows[0]["M_date"].ToString()))
                {
                    strM_date = dtPostSend.Rows[0]["M_date"].ToString().Replace("/", "").Substring(0, 8);//*處理日期取前8碼
                }
                if (dtPostSend.Rows[0]["Info1"].ToString().Equals("240") && dtPostSend.Rows[0]["Send_status_Code"].ToString().Equals("G2"))
                {
                    //*資訊代碼1=240且郵件狀態代號=’G2’才需顯示招領郵局欄位
                    blnPost = true;
                }
                strPost_Name = dtPostSend.Rows[0]["Post_Name"].ToString();
            }
        }
    }

    /// <summary>
    /// 功能說明:通過Code獲得Name
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strResultKind"></param>
    /// <param name="strKindName"></param>
    public void GetKindName(string strPropertyKey, string strCode, ref string strName)
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();
       
        //*取卡方式顯示Code+Name
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", strPropertyKey, ref dtKindName))
        {
            DataRow[] rowKindName = dtKindName.Select("PROPERTY_CODE='" + strCode + "'");
            if (rowKindName != null && rowKindName.Length > 0)
            {
                strName = rowKindName[0]["PROPERTY_CODE"].ToString() + " " + rowKindName[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                strName = strCode;
            }
        }
    }

    /// <summary>
    /// 功能說明:通過退件原因Code獲得Name
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strResultKind"></param>
    /// <param name="strKindName"></param>
    public string GetBackName(string strCode)
    {
        string strMsgID = string.Empty;
        string strName = strCode;
        DataTable dtBackName = new DataTable();

        //*取卡方式顯示Code+Name
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "5", ref dtBackName))
        {
            DataRow[] rowBackName = dtBackName.Select("PROPERTY_CODE='" + strCode + "'");
            if (null != rowBackName && rowBackName.Length > 0)
            {
                strName = rowBackName[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                strName = strCode;
            }
        }
        return strName;
    }

    /// <summary>
    /// 功能說明:獲取退件處理方式的中文
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strResultKind"></param>
    /// <param name="strKindName"></param>
    public string GetOperactionName(string strCode)
    {
        string strMsgID = string.Empty;
        string strName = strCode;
        DataTable dtBackName = new DataTable();

        //*取卡方式顯示Code+Name
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "16", ref dtBackName))
        {
            DataRow[] rowBackName = dtBackName.Select("PROPERTY_CODE='" + strCode + "'");
            if (null != rowBackName && rowBackName.Length > 0)
            {
                strName = rowBackName[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                strName = strCode;
            }
        }
        return strName;
    }

    /// <summary>
    /// 功能說明:綁定取卡方式
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void BindKind()
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();
        dropKindAjax.Items.Clear();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKindName))
        {
            foreach (DataRow dr in dtKindName.Rows)
            {
                ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                dropKindAjax.Items.Add(liTmp);
            }
        }
    }

    /// <summary>
    /// 功能說明:綁定領卡方式
    /// 作    者:Linda
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void BindSelfPick()
    {
        string strMsgID = string.Empty;
        DataTable dtSelfPick = new DataTable();
        dropSelfPickAjax.Items.Clear();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "4", ref dtSelfPick))
        {
            foreach (DataRow dr in dtSelfPick.Rows)
            {
                ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                dropSelfPickAjax.Items.Add(liTmp);
            }
        }
    }

    /// <summary>
    /// 功能說明:綁定郵編
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    protected void CustAdd1_ChangeValues()
    {
        this.lblNewzipAjax.Text = this.CustAdd1.strZip;
        if (m_Status.Equals("Y"))
        {
            this.ModalPopupExtenderA.Hide();
        }
        else
        {
            this.ModalPopupExtenderA.Show();
        }
    }

    /// <summary>
    /// 功能說明:初始化頁面控件的值
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void InitControls()
    {
        //this.txtName1Ajax.Text = string.Empty;
        //this.txtMaildateAjax.Text = string.Empty;
        this.txtAdd2Ajax.Text = string.Empty;
        this.txtAdd3Ajax.Text = string.Empty;
        //this.txtMonlimitAjax.Text = string.Empty;
        this.txtMailnoAjax.Text = string.Empty;
        this.lblSelfPickDateAjax.Text = DateTime.Now.ToString("yyyy/MM/dd");
        this.txtMailNo.Text = string.Empty;

        //this.txtCNoteN.Text = string.Empty;
        //this.txtCNoteP.Text = string.Empty;
        //this.txtCNoteA.Text = string.Empty;
        //this.txtCNoteM.Text = string.Empty;
        //this.txtCNoteC.Text = string.Empty;
        //this.txtCNoteG.Text = string.Empty;
    }
    /// <summary>
    /// 功能說明:MergeTable加載製卡廠名稱
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtCard)
    {
        string strMsgID = string.Empty;
        dtCard.Columns.Add("Merch_CodeS");
        //dtCard.Columns.Add("Photos");
        //dtCard.Columns.Add("Affinitys");
        //*製卡廠名稱(Merch_Code)
        DataTable dtMerch_Code = new DataTable();

        foreach (DataRow row in dtCard.Rows)
        {
            //*製卡廠名稱顯示Code+Name
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtMerch_Code))
            {
                DataRow[] rowMerch_Code = dtMerch_Code.Select("PROPERTY_CODE='" + row["Merch_Code"].ToString() + "'");
                if (rowMerch_Code != null && rowMerch_Code.Length > 0)
                {
                    row["Merch_CodeS"] = rowMerch_Code[0]["PROPERTY_CODE"].ToString() + " " + rowMerch_Code[0]["PROPERTY_NAME"].ToString();
                }
                else
                {
                    row["Merch_CodeS"] = row["Merch_Code"].ToString();
                }
            }
        }
    }

    /// <summary>
    /// 功能說明:通過Code獲得認同代碼Name
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    /// <param name="ref strAFFID">認同代碼ID</param>
    /// <param name="ref strAffName">認同代碼NAME</param>
    public void GetAffName(string strAFFID, ref string strAffName)
    {
        string strMsgID = string.Empty;
        DataTable dtAffName = new DataTable();

        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(Entity_AffName.M_AFFID, Operator.Equal, DataTypeUtils.String, strAFFID);
        strAffName = strAFFID;

        if (BRM_AffName.SearchByNo(sqlhelp.GetFilterCondition(), ref dtAffName, ref strMsgID))
        {
            if (null != dtAffName && dtAffName.Rows.Count > 0)
            {
                DataRow[] rowKindName = dtAffName.Select("AFFID='" + strAFFID + "'");
                if (rowKindName != null && rowKindName.Length > 0)
                {
                    strAffName = rowKindName[0]["AFFID"].ToString() + " " + rowKindName[0]["AFFName"].ToString();
                }
            }
        }
    }
    #endregion
}
