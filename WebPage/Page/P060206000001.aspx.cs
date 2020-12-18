//******************************************************************
//*  功能說明：退件處理
//*  作    者：
//*  創建日期：2010/06/04
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
//20161108 (U) by Tank, 調整取CardType中文方式

using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Text.RegularExpressions;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
//20161108 (U) by Tank
using Framework.Data;
using System.Data.SqlClient;
using CSIPCommonModel.EntityLayer;

public partial class Page_P060206000001 : PageBase
{
    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

    static int intDBAdd;
    #region table
    public DataTable m_dtCardBackInfo
    {
        get { return ViewState["m_dtCardBackInfo"] as DataTable; }
        set { ViewState["m_dtCardBackInfo"] = value; }
    }
    #endregion
    #region params
    public string m_Add1
    {
        get { return ViewState["m_Add1"] as string; }
        set { ViewState["m_Add1"] = value; }
    }
    public string m_Add2
    {
        get { return ViewState["m_Add2"] as string; }
        set { ViewState["m_Add2"] = value; }
    }
    public string m_Add3
    {
        get { return ViewState["m_Add3"] as string; }
        set { ViewState["m_Add3"] = value; }
    }
    public string m_AddChange
    {
        get { return ViewState["m_AddChange"] as string; }
        set { ViewState["m_AddChange"] = value; }
    }
    #endregion

    #region Event
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindStatus();
            BindEnditem();
            this.btnSearch.Text = BaseHelper.GetShowText("06_06020100_002");
            ShowControlsText();
            PanelVisibleFalse();
            this.gpList.Visible = false;
            this.grvUserView.Visible = false;
            Table2.Visible = false;
            // jsBuilder.RegScript(this.UpdatePanel1, "HostMsgShow('');");
            jsBuilder.RegScript(this.UpdatePanel1, "window.parent.postMessage({ func: 'HostMsgShow', data: ''}, '*');");

            if (Request.QueryString["Id0206"] != null && Request.QueryString["Stusts0206"] != null && Request.QueryString["PageIndex0206"] != null)
            {
                txtId.Text = RedirectHelper.GetDecryptString(Request.QueryString["Id0206"].ToString().Trim());
                dropStatus.SelectedValue = RedirectHelper.GetDecryptString(Request.QueryString["Stusts0206"].ToString().Trim());
                gpList.Visible = true;
                grvUserView.Visible = true;
                gpList.CurrentPageIndex = Convert.ToInt16(RedirectHelper.GetDecryptString(Request.QueryString["PageIndex0206"].ToString().Trim()));
                BindGridView();
            } 
        }
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    protected void dropEnditem_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.dropEnditem.SelectedValue.ToString() == "1" || this.dropEnditem.SelectedValue.ToString() == "2" || this.dropEnditem.SelectedValue.ToString() == "3" || this.dropEnditem.SelectedValue.ToString() == "4")
        {
            if (m_AddChange=="11")
            {
                this.InitpnlAddChange();
                CustAdd1_ChangeValues();
                this.pnlAddChange.Visible = true;
            }
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020600_013") + "');");
                this.pnlAddChange.Visible = false;
                return;
            }

        }
        else
        {
            this.pnlAddChange.Visible = false;
        }
    }
    protected void btnSearchAdd_Click(object sender, EventArgs e)
    {
        int intcheck = 0;
        string strMsg = string.Empty;
        intDBAdd = 0;
        this.pnlAddSource1.Visible = false;
        this.pnlAddSource2.Visible = false;
        this.pnlAddSource3.Visible = false;
        this.pnlAddSource4.Visible = false;
        this.pnlAddSource5.Visible = false;
        this.pnlAddSource6.Visible = false;
        this.pnlAddSource7.Visible = false;
        this.pnlAddSource8.Visible = false;

        string strCustName=string.Empty;
        string strAdd1=string.Empty;
        string strAdd2=string.Empty;
        string strAdd3=string.Empty;

        m_AddChange = "11";

        try
        {
            for (int i = 0; i < this.grvUserView.Rows.Count; i++)
            {
                HtmlInputCheckBox chkFlg = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[0].FindControl("chkSearchAdd");
                if (chkFlg.Checked)
                {
                    intcheck = intcheck + 1;
                    if (m_dtCardBackInfo.Rows[i]["CardBackStatus"].ToString()!="0"&&hidSearchAdd.Value=="0")//已處理且沒有提示過（或客戶選擇不處理的）
                    {
                        jsBuilder.RegScript(this.Page,"if (confirm('" + MessageHelper.GetMessage("06_06020600_012") + "')){document.getElementById('hidSearchAdd').value = '1';}else{document.getElementById('hidSearchAdd').value = '0';}");
                        return;
                    }
                }
            }
            if (intcheck == 0)
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020600_000") + "');");
                return;
            }
            else
            {
                intcheck = 0;
                //this.GetP4_JCEHDateVD(ref strCustName, ref strAdd1, ref strAdd2, ref strAdd3);
                //m_Add1 = strAdd1;
                //m_Add2 = strAdd2;
                //m_Add3 = strAdd3;

                for (int i = 0; i < this.grvUserView.Rows.Count; i++)
                {
                    HtmlInputCheckBox chkFlg = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[0].FindControl("chkSearchAdd");
                    if (chkFlg.Checked)
                    {
                        if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "VD卡")
                        {
                            this.GetP4_JCEHDateVD(ref strCustName, ref strAdd1, ref strAdd2, ref strAdd3);
                        }
                        else
                        {
                            this.GetP4_JCEHDate(ref strCustName, ref strAdd1, ref strAdd2, ref strAdd3);
                        }

                        intcheck = intcheck + 1;
                        switch (intcheck)
                        {
                            case 1:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel5.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel7.Text = BaseHelper.GetShowText("06_06020600_006"); 
                                }
                                else
                                {
                                    this.CustLabel5.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel7.Text = BaseHelper.GetShowText("06_06020600_025"); 
                                }
                                this.lblCardNo1.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd1.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName1.Text = strCustName;
                                this.lblMFAdd1.Text = strAdd1 + strAdd2 + strAdd3;
                                this.pnlAddSource1.Visible = true;
                                intDBAdd = i;
                                m_Add1 = strAdd1;
                                m_Add2 = strAdd2;
                                m_Add3 = strAdd3;
                                break;
                            case 2:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel15.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel17.Text = BaseHelper.GetShowText("06_06020600_006");
                                }
                                else
                                {
                                    this.CustLabel15.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel17.Text = BaseHelper.GetShowText("06_06020600_025");
                                }
                                this.lblCardNo2.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd2.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName2.Text = strCustName;
                                this.lblMFAdd2.Text = strAdd1 + strAdd2 + strAdd3;
                                this.pnlAddSource2.Visible = true;
                                if ((this.lblMFAdd1.Text != strAdd1 + strAdd2 + strAdd3) || (lblMFName1.Text != strCustName))
                                {
                                    m_AddChange = "00";
                                }
                                break;
                            case 3:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel23.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel25.Text = BaseHelper.GetShowText("06_06020600_006");
                                }
                                else
                                {
                                    this.CustLabel23.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel25.Text = BaseHelper.GetShowText("06_06020600_025");
                                }
                                this.lblCardNo3.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd3.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName3.Text = strCustName;
                                this.lblMFAdd3.Text = strAdd1+ strAdd2 + strAdd3;
                                this.pnlAddSource3.Visible = true;
                                if ((this.lblMFAdd1.Text != strAdd1 + strAdd2 + strAdd3) || (lblMFName1.Text != strCustName))
                                {
                                    m_AddChange = "00";
                                }
                                break;
                            case 4:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel31.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel33.Text = BaseHelper.GetShowText("06_06020600_006");
                                }
                                else
                                {
                                    this.CustLabel31.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel33.Text = BaseHelper.GetShowText("06_06020600_025");
                                }
                                this.lblCardNo4.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd4.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName4.Text = strCustName;
                                this.lblMFAdd4.Text = strAdd1 + strAdd2+ strAdd3;
                                this.pnlAddSource4.Visible = true;
                                if ((this.lblMFAdd1.Text != strAdd1 + strAdd2 + strAdd3) || (lblMFName1.Text != strCustName))
                                {
                                    m_AddChange = "00";
                                }
                                break;
                            case 5:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel39.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel41.Text = BaseHelper.GetShowText("06_06020600_006");
                                }
                                else
                                {
                                    this.CustLabel39.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel41.Text = BaseHelper.GetShowText("06_06020600_025");
                                }
                                this.lblCardNo5.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd5.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName5.Text = strCustName;
                                this.lblMFAdd5.Text = strAdd1+ strAdd2 + strAdd3;
                                this.pnlAddSource5.Visible = true;
                                if ((this.lblMFAdd1.Text != strAdd1 + strAdd2 + strAdd3) || (lblMFName1.Text != strCustName))
                                {
                                    m_AddChange = "00";
                                }
                                break;
                            case 6:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel20.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel24.Text = BaseHelper.GetShowText("06_06020600_006");
                                }
                                else
                                {
                                    this.CustLabel20.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel24.Text = BaseHelper.GetShowText("06_06020600_025");
                                }
                                this.lblCardNo6.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd6.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName6.Text = strCustName;
                                this.lblMFAdd6.Text = strAdd1+ strAdd2 + strAdd3;
                                this.pnlAddSource6.Visible = true;
                                if ((this.lblMFAdd1.Text != strAdd1 + strAdd2 + strAdd3) || (lblMFName1.Text != strCustName))
                                {
                                    m_AddChange = "00";
                                }
                                break;
                            case 7:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel36.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel40.Text = BaseHelper.GetShowText("06_06020600_006");
                                }
                                else
                                {
                                    this.CustLabel36.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel40.Text = BaseHelper.GetShowText("06_06020600_025");
                                }
                                this.lblCardNo7.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd7.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName7.Text = strCustName;
                                this.lblMFAdd7.Text = strAdd1+ strAdd2 + strAdd3;
                                this.pnlAddSource7.Visible = true;
                                if ((this.lblMFAdd1.Text != strAdd1 + strAdd2 + strAdd3) || (lblMFName1.Text != strCustName))
                                {
                                    m_AddChange = "00";
                                }
                                break;
                            case 8:
                                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "信用卡")
                                {
                                    this.CustLabel47.Text = BaseHelper.GetShowText("06_06020600_005");
                                    this.CustLabel49.Text = BaseHelper.GetShowText("06_06020600_006");
                                }
                                else
                                {
                                    this.CustLabel47.Text = BaseHelper.GetShowText("06_06020600_024");
                                    this.CustLabel49.Text = BaseHelper.GetShowText("06_06020600_025");
                                }
                                this.lblCardNo8.Text = m_dtCardBackInfo.Rows[i]["cardno"].ToString();
                                this.lblBackAdd8.Text = BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add1"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add2"].ToString().Trim()) + BaseHelper.ToSBC(m_dtCardBackInfo.Rows[i]["add3"].ToString().Trim());
                                this.lblMFName8.Text = strCustName;
                                this.lblMFAdd8.Text = strAdd1+ strAdd2 + strAdd3;
                                this.pnlAddSource8.Visible = true;
                                if ((this.lblMFAdd1.Text != strAdd1 + strAdd2 + strAdd3) || (lblMFName1.Text != strCustName))
                                {
                                    m_AddChange = "00";
                                }
                                break;
                        }

                        if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "VD卡")
                        {
                            string strTrandate = m_dtCardBackInfo.Rows[i]["Trandate"].ToString();
                            DateTime dtTrandate = DateTime.Parse(strTrandate).AddMonths(3);
                            if (string.Compare(DateTime.Now.ToString("yyyy/MM/dd"), dtTrandate.ToString("yyyy/MM/dd")) > 0)
                            {
                                strMsg += MessageHelper.GetMessage("06_06020600_001", m_dtCardBackInfo.Rows[i]["cardno"].ToString(), DateTime.Parse(strTrandate).AddMonths(4).ToString("yyyy/MM/dd"));
                            }

                        }

                    }
                }

                if (!strMsg.Equals(string.Empty))
                {
                    this.ModalPopupExtenderNotice1.Show();
                    this.lblNotice1.Text = strMsg;
                }

                this.BindEnditem();
                this.pnlEnditemDrop.Visible = true;

                if (this.dropEnditem.SelectedValue.ToString() == "1" || this.dropEnditem.SelectedValue.ToString() == "2" || this.dropEnditem.SelectedValue.ToString() == "3" || this.dropEnditem.SelectedValue.ToString() == "4")
                {
                    this.InitpnlAddChange();
                    CustAdd1_ChangeValues();
                    this.pnlAddChange.Visible = true;
                }
                else
                {
                    this.pnlAddChange.Visible = false;
                }

            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_002"));
            return;
        }
    }
    /// <summary>
    /// 修改紀錄:2020/12/17_Ares_Stanley-調整AP_LOG紀錄順序, 避免錯誤的查詢條件導致寫入AP_LOG失敗, 增加查詢條件錯誤的LOG
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        if (!CheckCondition(ref strMsgID))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
            string errorMsg = string.Format("退件處理查詢輸入異常：員編:{0}, 身分證字號: {1}", this.eAgentInfo.agent_id, this.txtId.Text);
            Logging.Log(errorMsg, LogState.Info, LogLayer.None);
            return;
        }

        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtId.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------

        PanelVisibleFalse();
        BindGridView();
        hidSearchAdd.Value = "0";
    }

    /// <summary>
    /// 功能說明:編輯邏輯
    /// 作    者:Linda
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {

            DataTable dtCloseInfo = new DataTable();
            DataTable dtlOutClose = new DataTable();

            string strAction = m_dtCardBackInfo.Rows[e.NewEditIndex]["action"].ToString();
            string strId = m_dtCardBackInfo.Rows[e.NewEditIndex]["id"].ToString();
            string strCardNo = m_dtCardBackInfo.Rows[e.NewEditIndex]["cardno"].ToString();
            string strTrandate = m_dtCardBackInfo.Rows[e.NewEditIndex]["trandate"].ToString();

            this.Session["Id0206"] = this.txtId.Text.Trim();
            this.Session["Stusts0206"] = this.dropStatus.SelectedValue.ToString().Trim();
            this.Session["PageIndex0206"] = this.gpList.CurrentPageIndex.ToString();
            this.Session["backpage"] = "0601";
            //* 傳遞參數加密
            Response.Redirect("P060205000003.aspx?backpage=" + RedirectHelper.GetEncryptParam("0601") + "&action=" + RedirectHelper.GetEncryptParam(strAction) + "&id=" + RedirectHelper.GetEncryptParam(strId) + "&cardno=" + RedirectHelper.GetEncryptParam(strCardNo) + "&trandate=" + RedirectHelper.GetEncryptParam(strTrandate) + "&intostoredate=" + RedirectHelper.GetEncryptParam("") + "&modifyflg=" + RedirectHelper.GetEncryptParam("0") + "", false);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010400_005"));
            return;
        }
    }

    protected void btSub_Click(object sender, EventArgs e)
    {
        //string strEnditem = string.Empty;
        string strZip = string.Empty;
        string strAdd1 = string.Empty;
        string strAdd2 = string.Empty;
        string strAdd3 = string.Empty;
        string strNode = string.Empty;
        string strName = string.Empty;
        string strMsg = string.Empty;
        string strMsgID = string.Empty;
        try
        {
            if (this.dropEnditem.SelectedValue.ToString() == "")
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020600_016") + "');");
                return;
            }
            if (this.dropEnditem.SelectedValue.ToString() == "1" || this.dropEnditem.SelectedValue.ToString() == "2" || this.dropEnditem.SelectedValue.ToString() == "3" || this.dropEnditem.SelectedValue.ToString() == "4")
            {
                if (m_AddChange == "00")
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020600_013") + "');");
                    this.pnlAddChange.Visible = false;
                    return;
                }

            }
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06020600_000");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");

            //strEnditem = this.dropEnditem.SelectedItem.Text.ToString().Trim();

            for (int i = 0; i < this.grvUserView.Rows.Count; i++)
            {

                if (m_dtCardBackInfo.Rows[i]["cardtypeS"].ToString() == "VD卡")
                {
                    string strTrandate = m_dtCardBackInfo.Rows[i]["Trandate"].ToString();
                    DateTime dtTrandate = DateTime.Parse(strTrandate).AddMonths(3);
                    if (string.Compare(DateTime.Now.ToString("yyyy/MM/dd"), dtTrandate.ToString("yyyy/MM/dd")) > 0)
                    {
                        strMsg += DateTime.Parse(strTrandate).AddMonths(4).ToString("yyyy/MM/dd") + " ";
                    }

                }

            }

            if (!strMsg.Equals(string.Empty))
            {
                strMsg = MessageHelper.GetMessage("06_06020600_002", strMsg);
                this.ModalPopupExtenderNotice2.Show();
                this.lblNotice2.Text = strMsg;
            }

            if (this.dropEnditem.SelectedValue.ToString() == "1" || this.dropEnditem.SelectedValue.ToString() == "2" || this.dropEnditem.SelectedValue.ToString() == "3" || this.dropEnditem.SelectedValue.ToString() == "4")
            {
                strZip = this.lblZip.Text.ToString().Trim();
                strAdd1 = this.CustAdd1.strAddress.Trim();
                strAdd2 = this.txtAdd2.Text.ToString().Trim();
                strAdd3 = this.txtAdd3.Text.ToString().Trim();
                strNode = this.txtNote.Text.ToString().Trim();
                strName = this.lblMFName1.Text.ToString().Trim();

                for (int i = 0; i < this.grvUserView.Rows.Count; i++)
                {
                    HtmlInputCheckBox chkFlg = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[0].FindControl("chkSearchAdd");
                    if (chkFlg.Checked)
                    {
                        Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();

                        CardBackInfo.serial_no = m_dtCardBackInfo.Rows[i]["serial_no"].ToString();
                        if (strName.Equals(string.Empty))
                        {
                            CardBackInfo.NewName = m_dtCardBackInfo.Rows[i]["CustName"].ToString();
                        }
                        else
                        {
                            CardBackInfo.NewName = strName;
                        }
                        CardBackInfo.NewZip = BaseHelper.ToSBC(strZip);
                        CardBackInfo.NewAdd1 = BaseHelper.ToSBC(strAdd1);
                        CardBackInfo.NewAdd2 = BaseHelper.ToSBC(strAdd2);
                        CardBackInfo.NewAdd3 = BaseHelper.ToSBC(strAdd3);
                        CardBackInfo.Endnote = strNode;
                        CardBackInfo.Enditem = this.dropEnditem.SelectedValue.ToString().Trim();
                        CardBackInfo.Enduid = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                        CardBackInfo.Enddate = DateTime.Now.ToString("yyyy/MM/dd");
                        CardBackInfo.Endtime = DateTime.Now.ToString("HH:mm");
                        CardBackInfo.EndFunction = "0";
                        CardBackInfo.CardBackStatus = "1";

                        SqlHelper sqlhelp = new SqlHelper();
                        sqlhelp.AddCondition(Entity_CardBackInfo.M_serial_no, Operator.Equal, DataTypeUtils.String, CardBackInfo.serial_no);
                        if (BRM_CardBackInfo.Update(CardBackInfo, sqlhelp.GetFilterCondition(), "NewName", "NewZip", "NewAdd1", "NewAdd2", "NewAdd3", "Endnote", "Enditem", "Enduid", "Enddate", "EndFunction", "CardBackStatus", "Endtime"))
                        {

                            //*新增異動單
                            Entity_CardDataChange CardDataChange = new Entity_CardDataChange();

                            string strUserName = string.Empty;
                            BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);

                            CardDataChange.action = m_dtCardBackInfo.Rows[i]["Action"].ToString();
                            CardDataChange.id = m_dtCardBackInfo.Rows[i]["Id"].ToString(); ;
                            CardDataChange.CardNo = m_dtCardBackInfo.Rows[i]["CardNo"].ToString();
                            CardDataChange.Trandate = m_dtCardBackInfo.Rows[i]["Trandate"].ToString();

                            CardDataChange.OldZip = m_dtCardBackInfo.Rows[i]["zip"].ToString();
                            CardDataChange.NewZip = BaseHelper.ToSBC(strZip);//*郵遞區號全碼
                            CardDataChange.OldAdd1 = m_dtCardBackInfo.Rows[i]["add1"].ToString();
                            CardDataChange.NewAdd1 = BaseHelper.ToSBC(strAdd1);     //*地址一全碼
                            CardDataChange.OldAdd2 = m_dtCardBackInfo.Rows[i]["add2"].ToString();
                            CardDataChange.NewAdd2 = BaseHelper.ToSBC(strAdd2); //*地址二全碼
                            CardDataChange.OldAdd3 = m_dtCardBackInfo.Rows[i]["add3"].ToString();
                            CardDataChange.NewAdd3 = BaseHelper.ToSBC(strAdd3); //*地址三全碼

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
                            CardDataChange.SourceType = "2";

                            if (BRM_CardDataChange.Insert(CardDataChange, ref strMsgID))
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020600_004"));
                                PanelVisibleFalse();
                                BindGridView();
                                
                            }
                            else
                            {
                                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020600_003"));
                            }
                        }
                        else
                        {
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020600_005"));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.grvUserView.Rows.Count; i++)
                {
                    HtmlInputCheckBox chkFlg = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[0].FindControl("chkSearchAdd");
                    if (chkFlg.Checked)
                    {
                        Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();

                        CardBackInfo.serial_no = m_dtCardBackInfo.Rows[i]["serial_no"].ToString();

                        CardBackInfo.Enditem = this.dropEnditem.SelectedValue.ToString().Trim();
                        CardBackInfo.Enduid = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                        CardBackInfo.Enddate = DateTime.Now.ToString("yyyy/MM/dd");
                        CardBackInfo.EndFunction = "0";
                        CardBackInfo.CardBackStatus = "1";

                        SqlHelper sqlhelp = new SqlHelper();
                        sqlhelp.AddCondition(Entity_CardBackInfo.M_serial_no, Operator.Equal, DataTypeUtils.String, CardBackInfo.serial_no);
                        if (BRM_CardBackInfo.Update(CardBackInfo, sqlhelp.GetFilterCondition(), "Endnote", "Enditem", "Enduid", "Enddate", "EndFunction", "CardBackStatus"))
                        {
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020600_004"));
                            PanelVisibleFalse();
                            BindGridView();
                        }
                        else
                        {
                            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020600_005"));
                        }
                    }
                }

            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020600_005"));
            return;
        }

    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        PanelVisibleFalse();
        BindGridView();
    }

    protected void btnPC_Click(object sender, EventArgs e)
    {
        this.CustAdd1.InitalAdd1_1("");
        this.CustAdd1.InitalAdd1_2("");
        this.CustAdd1.InitalAdd1(m_dtCardBackInfo.Rows[intDBAdd]["add1"].ToString());
        this.txtAdd2.Text = m_dtCardBackInfo.Rows[intDBAdd]["add2"].ToString();
        this.txtAdd3.Text = m_dtCardBackInfo.Rows[intDBAdd]["add3"].ToString();
    }

    protected void btnMFAdd_Click(object sender, EventArgs e)
    {
        this.CustAdd1.InitalAdd1_1("");
        this.CustAdd1.InitalAdd1_2("");
        if (this.m_Add1.Trim().Length > 3)
        {
            this.CustAdd1.InitalAdd1(this.m_Add1);
        }
        else
        {
            this.CustAdd1.InitalAdd1(this.m_Add1 + this.m_Add1);
        }
        this.txtAdd2.Text = this.m_Add2;
        this.txtAdd3.Text = this.m_Add3;
    }
    #endregion

    #region Function
    /// <summary>
    /// 功能說明:綁定退件狀態
    /// 作    者:Linda
    /// 創建時間:2010/06/29
    /// 修改記錄:
    /// </summary>
    public void BindStatus()
    {
        string strMsgID = string.Empty;
        DataTable dtStatusName = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "26", ref dtStatusName))
        {
            this.dropStatus.DataSource = dtStatusName;
            this.dropStatus.DataTextField = "PROPERTY_NAME";
            this.dropStatus.DataValueField = "PROPERTY_CODE";
            this.dropStatus.DataBind();
        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
        }
    }

    /// <summary>
    /// 功能說明:綁定退件狀態
    /// 作    者:Linda
    /// 創建時間:2010/06/29
    /// 修改記錄:
    /// </summary>
    public void BindEnditem()
    {
        string strMsgID = string.Empty;
        DataTable dtEnditemName = new DataTable();
        dropEnditem.Items.Clear();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "16", ref dtEnditemName))
        {
            foreach (DataRow dr in dtEnditemName.Rows)
            {
                ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                dropEnditem.Items.Add(liTmp);
            }
        }
        ListItem liEmptyTmp = new ListItem("", "");
        dropEnditem.Items.Add(liEmptyTmp);
        dropEnditem.SelectedValue = "";

    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:linda
    /// 創建時間:2010/06/30
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020600_015");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020600_016");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020600_017");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06020600_018");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06020600_019");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06020600_020");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06020600_021");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06020600_022");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 功能說明:綁定郵編
    /// 作    者:linda
    /// 創建時間:2010/06/30
    /// 修改記錄:
    /// </summary>
    protected void CustAdd1_ChangeValues()
    {
        this.lblZip.Text = this.CustAdd1.strZip;

    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:Linda
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strId = this.txtId.Text.ToString().Trim();
        string strBackStatus = this.dropStatus.SelectedValue.ToString().Trim();
        int iTotalCount = 0;
        DataTable dtCardBackInfo = new DataTable();
        try
        {
            //* 查詢不成功
            if (!BRM_CardBackInfo.GetBackInfoFor0206(ref dtCardBackInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, strId, strBackStatus))
            {
                this.gpList.RecordCount = 0;
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                this.gpList.Visible = false;
                this.btnSearchAdd.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020800_001"));
                return;
            }
            //* 查詢成功
            else
            {
                MergeTable(ref dtCardBackInfo);
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtCardBackInfo;
                this.grvUserView.DataBind();
                if (null != dtCardBackInfo && dtCardBackInfo.Rows.Count > 0)
                {
                    Table2.Visible = true;
                }
                else
                {
                    Table2.Visible = false;
                }
                VisibleCheckBox(dtCardBackInfo);
                m_dtCardBackInfo = dtCardBackInfo;
                if (iTotalCount > 0)
                {
                    this.btnSearchAdd.Visible = true;
                }
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
    /// 功能說明:MergeTable加載退件原因\處理狀態\卡種
    /// 作    者:Linda
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtCard)
    {
        string strMsgID = string.Empty;
        dtCard.Columns.Add("cardtypeS");
        dtCard.Columns.Add("ReasonS");
        dtCard.Columns.Add("CardBackStatusS");
        //*退件原因
        DataTable dtReason = new DataTable();
        //*處理狀態
        DataTable dtCardBackStatus = new DataTable();

        foreach (DataRow row in dtCard.Rows)
        {

            //20161108 (U) by Tank, 取CardType中文
            //switch (row["cardtype"].ToString().Trim())
            //{
            //    case "000":
            //        row["cardtypeS"] = "金融卡";
            //        break;
            //    case "013":
            //        row["cardtypeS"] = "VD卡";
            //        break;
            //    case "370":
            //        row["cardtypeS"] = "VD卡";
            //        break;
            //    case "035":
            //        row["cardtypeS"] = "VD卡";
            //        break;
            //    case "571":
            //        row["cardtypeS"] = "VD卡";
            //        break;
            //    case "018":
            //        row["cardtypeS"] = "現金卡";
            //        break;
            //    case "019":
            //        row["cardtypeS"] = "E-Cash";
            //        break;
            //    //max add 悠遊Debit 需求修改
            //    case "039":
            //        row["cardtypeS"] = "VD悠遊";
            //        break;
            //    case "040":
            //        row["cardtypeS"] = "MD悠遊";
            //        break;
            //    case "038":
            //        row["cardtypeS"] = "外勞ATM匯款卡";
            //        break;
            //    case "037":
            //        row["cardtypeS"] = "銀聯Debit 卡";
            //        break;

            //    default:
            //        row["cardtypeS"] = "信用卡";
            //        break;
            //}
            row["cardtypeS"] = GetCardTypeName(row["cardtype"].ToString().Trim());

            
            //*退件原因顯示
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "5", ref dtReason))
            {
                DataRow[] rowReason = dtReason.Select("PROPERTY_CODE='" + row["Reason"].ToString() + "'");
                if (rowReason != null && rowReason.Length > 0)
                {
                    row["ReasonS"] = rowReason[0]["PROPERTY_NAME"].ToString();
                }
                else
                {
                    row["ReasonS"] = row["Reason"].ToString();
                }
            }
            
            //*處理狀態
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "26", ref dtCardBackStatus))
            {
                DataRow[] rowCardBackStatus = dtCardBackStatus.Select("PROPERTY_CODE='" + row["CardBackStatus"].ToString() + "'");
                if (rowCardBackStatus != null && rowCardBackStatus.Length > 0)
                {
                    row["CardBackStatusS"] = rowCardBackStatus[0]["PROPERTY_NAME"].ToString();
                }
                else
                {
                    row["CardBackStatusS"] = row["CardBackStatus"].ToString();
                }
            }
        }
    }

    /// <summary>
    /// 功能說明:VisibleCheckBox設置複選框是否可見
    /// 作    者:Linda
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <param name="dtSelfPickInfo"></param>
    public void VisibleCheckBox(DataTable dtCardBackInfo)
    {
        for (int i = 0; i < this.grvUserView.Rows.Count; i++)
        {
            HtmlInputCheckBox chkEnable = (HtmlInputCheckBox)grvUserView.Rows[i].Cells[0].FindControl("chkSearchAdd");

            string strCardBackStatus = dtCardBackInfo.Rows[i]["CardBackStatus"].ToString();
            string strBlockcode = dtCardBackInfo.Rows[i]["Blockcode"].ToString();

            if (strCardBackStatus != "2" && strBlockcode.Equals(string.Empty))
            {
                chkEnable.Visible = true;
            }
            else
            {
                chkEnable.Visible = false;
            }

        }
    }

    protected void PanelVisibleFalse()
    {
        this.btnSearchAdd.Visible = false;
        this.pnlAddSource1.Visible = false;
        this.pnlAddSource2.Visible = false;
        this.pnlAddSource3.Visible = false;
        this.pnlAddSource4.Visible = false;
        this.pnlAddSource5.Visible = false;
        this.pnlAddSource6.Visible = false;
        this.pnlAddSource7.Visible = false;
        this.pnlAddSource8.Visible = false;
        this.pnlEnditemDrop.Visible = false;
        this.pnlAddChange.Visible = false;
    }

    protected void InitpnlAddChange()
    {
        this.lblZip.Text = "";
        this.txtAdd2.Text = "";
        this.txtAdd3.Text = "";
        this.txtNote.Text = "";
    }

    protected void GetP4_JCEHDate(ref string strCustName, ref string strAdd1, ref string strAdd2, ref string strAdd3)
    {
        strCustName = string.Empty;
        strAdd1 = string.Empty;
        strAdd2 = string.Empty;
        strAdd3 = string.Empty;
        try
        {
            string strMsg = "";             //* 錯誤訊息       
            Hashtable htInput = new Hashtable();    //* 電文上行查詢條件欄位列表
            DataTable dtblJCEH = new DataTable();   //* 1331(JCEH)電文查詢結果

            //* 1331(JCEH)電文欄位: 查詢起始筆數,訊息別,客戶姓名,客戶地址1,客戶地址2,客戶地址3,郵遞區號,卡號,卡人ID,歸戶ID,發卡日,卡人姓名,狀況碼,狀況碼日期,前況碼,前況碼日期,最後繳款日,餘額,不良資產是否已經出售
            string[] aryJCEHCol = new string[] { "LINE_CNT", "MESSAGE_TYPE", "SHORT_NAME", "CITY", "ADDR_1", "ADDR_2", "ZIP", "CARDHOLDER", "CUSTID", "ACCT_CUST_ID", "CARDHOLDER_NAME", "OPENED", "BLOCK", "BLOCK_DTE", "ALT_BLOCK", "ALT_BLOCK_DTE", "DTE_LST_PYMT", "CURR_BAL", "AMC_FLAG" };

            htInput.Add("ACCT_NBR", this.txtId.Text.Trim());
            dtblJCEH = MainFrameInfo.GetMainframeData(htInput, "P4_JCEH", ref strMsg, aryJCEHCol);

            if (!String.IsNullOrEmpty(strMsg))
            {
                //* 出錯
                //* 無此筆資料,重置表單
                if (strMsg == "該筆資料不存在" || strMsg == "目前電文主機上資料筆數為0")
                {
                    //* Alert("無此筆資料")
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_010"));
                    return;
                }
                else
                {
                    //* 1331下行失敗
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_009"));
                    return;
                }
            }
            else
            {
                strCustName = dtblJCEH.Rows[0]["SHORT_NAME"].ToString().Trim();
                strAdd1 = BaseHelper.ToSBC(dtblJCEH.Rows[0]["CITY"].ToString().Trim());
                strAdd2 = BaseHelper.ToSBC(dtblJCEH.Rows[0]["ADDR_1"].ToString().Trim());
                strAdd3 = BaseHelper.ToSBC(dtblJCEH.Rows[0]["ADDR_2"].ToString().Trim());
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_008"));//* 1331下行成功
            }
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_011"));
            return;
        }
    }
    protected void GetP4_JCEHDateVD(ref string strCustName, ref string strAdd1, ref string strAdd2, ref string strAdd3)
    {
        strCustName = string.Empty;
        strAdd1 = string.Empty;
        strAdd2 = string.Empty;
        strAdd3 = string.Empty;
        try
        {
            string strMsg = "";             //* 錯誤訊息       
            Hashtable htInput = new Hashtable();    //* 電文上行查詢條件欄位列表
            Hashtable ht0670500 = new Hashtable();   //* 067050電文查詢結果

            //* 067050電文欄位:客戶姓名,客戶地址1,客戶地址2,客戶地址3
            string[] aryJCEHCol = new string[] {"FIRST_NAME", "ADDRS_LINE_01", "ADDRS_LINE_02", "ADDRS_LINE_03"};

            htInput.Add("CUST_ID_NO", this.txtId.Text.Trim());
            //ht0670500 = MainFrameInfo.GetMainframeDataVD(htInput, "067050", ref strMsg, false,aryJCEHCol);
            ht0670500 = MainFrameInfo.GetMainframeDataVD(htInput, "067050", ref strMsg, true, aryJCEHCol);

            if (!String.IsNullOrEmpty(strMsg))
            {
                //* 出錯
                //* 無此筆資料,重置表單
                if (strMsg.Trim() == "找不到相關資料")
                {
                    //* Alert("無此筆資料")
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_010"));
                    return;
                }
                else
                {
                    //* 067050下行失敗
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_015"));
                    return;
                }
            }
            else
            {
                strCustName = ht0670500["FIRST_NAME"].ToString().Trim();
                strAdd1 = BaseHelper.ToSBC(ht0670500["ADDRS_LINE_01"].ToString().Trim());
                strAdd2 = BaseHelper.ToSBC(ht0670500["ADDRS_LINE_02"].ToString().Trim());
                strAdd3 = BaseHelper.ToSBC(ht0670500["ADDRS_LINE_03"].ToString().Trim());
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_014"));//* 067050下行成功
            }
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShow("06_06020600_011"));
            return;
        }
    }

        /// <summary>
    /// 身分證輸入欄位是否正確
    /// </summary>
    /// <param name="strMsgID">返回的錯誤ID</param>
    /// <param name="strUserID">客戶ID</param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID)
    {
        //* 客戶ID
        if (String.IsNullOrEmpty(this.txtId.Text.Trim()))
        {
            //* "請輸入身分證";
            strMsgID = "06_06020600_006";
            return false;
        }
        else
        {
            //* 如果身分證不是合法的格式
            if (!Regex.IsMatch(this.txtId.Text.Trim(), BaseHelper.GetShowText("06_00000000_000")))
            {
                strMsgID = "06_06020600_007";
                return false;
            }
            else
            {
                return true;
            }
        }
    }

#endregion

    //20161108 (U) by Tank, 取CardType中文
    protected string GetCardTypeName(string strCardType)
    {
        DataHelper dh = new DataHelper("Connection_System");
        SqlCommand sqlcmd = new SqlCommand();
        DataSet ds = new DataSet();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 240;
            sqlcmd.CommandText = @"select CardTypeName from tbl_CardType where CardType=@CardType ";

            SqlParameter ParCardType = new SqlParameter("@CardType", strCardType);
            sqlcmd.Parameters.Add(ParCardType);
            ds = dh.ExecuteDataSet(sqlcmd);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "信用卡";
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }
}

