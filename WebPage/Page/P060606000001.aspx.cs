//******************************************************************
//*  功能說明：交換檔案設定 
//*  作    者：HAO CHEN
//*  創建日期：2010/07/21
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
using System.Configuration;

public partial class Page_P060606000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("06_06060000_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));
            this.tblJob.Visible = false;
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            Show();
            BindDDl();
            BindGridView();
        }
    }

    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void Show()
    {
        lblTitle.Text = BaseHelper.GetShowText("06_06060000_000");
        btnAdd.Text = BaseHelper.GetShowText("06_06060000_033");
        btnUpdate.Text = BaseHelper.GetShowText("06_06060000_034");
        btnDelete.Text = BaseHelper.GetShowText("06_06060000_035");
        btnSubMit.Text = BaseHelper.GetShowText("06_06060000_036");
        btnConcel.Text = BaseHelper.GetShowText("06_06060000_039");

        rdoAM.Text = BaseHelper.GetShowText("06_06060000_010");
        rdoPM.Text = BaseHelper.GetShowText("06_06060000_011");

        grvFUNCTION.Columns[1].HeaderText = BaseHelper.GetShowText("06_06060000_001");
        grvFUNCTION.Columns[2].HeaderText = BaseHelper.GetShowText("06_06060000_002");
        grvFUNCTION.Columns[3].HeaderText = BaseHelper.GetShowText("06_06060000_003");
        grvFUNCTION.Columns[4].HeaderText = BaseHelper.GetShowText("06_06060000_004");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        this.grvFUNCTION.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
    }

    /// <summary>
    /// 綁定頁面所有DropDownList數據源
    /// </summary>
    private void BindDDl()
    {
        string strMsgID = string.Empty;
        //綁定JOB選單
        DataTable dtJob = new DataTable();
        dtJob = BRM_AutoJob.SearchAllJob(ref strMsgID);
        if (string.IsNullOrEmpty(strMsgID))
        {
            ddlJobName.DataSource = dtJob;
            ddlJobName.DataTextField = "DESCRIPTION";
            ddlJobName.DataValueField = "JOB_ID";
            ddlJobName.DataBind();
            SelectSubFunction(ddlJobName.SelectedValue);
        }

        //綁定卡片類型
        DataTable dtCardType = new DataTable();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "19", ref dtCardType))
        {
            ddlCardType.DataSource = dtCardType;
            ddlCardType.DataTextField = "PROPERTY_NAME";
            ddlCardType.DataValueField = "PROPERTY_CODE";
            ddlCardType.DataBind();
        }

        //綁定使用狀態
        DataTable dtUsing = new DataTable();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "35", ref dtUsing))
        {
            ddlUsingType.DataSource = dtUsing;
            ddlUsingType.DataTextField = "PROPERTY_NAME";
            ddlUsingType.DataValueField = "PROPERTY_CODE";
            ddlUsingType.DataBind();
        }

        ////綁定子功能選單
        //DataTable dtSubFunction = new DataTable();
        //if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "37", ref dtSubFunction))
        //{
        //    ddlSubFunction.DataSource = dtSubFunction;
        //    ddlSubFunction.DataTextField = "PROPERTY_CODE";
        //    ddlSubFunction.DataValueField = "PROPERTY_NAME";
        //    ddlSubFunction.DataBind();
        //}

        //綁定招領/退件註記
        DataTable dtTakewalFlg = new DataTable();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "38", ref dtTakewalFlg))
        {
            ddlTakewalFlg.DataSource = dtTakewalFlg;
            ddlTakewalFlg.DataTextField = "PROPERTY_NAME";
            ddlTakewalFlg.DataValueField = "PROPERTY_CODE";
            ddlTakewalFlg.DataBind();
        }

        //綁定Ftp Server 選單
        ddlFtpIp.Items.Clear();
        string[] strValue = ConfigurationManager.AppSettings["FtpIP"].Split(';');
        for (int Item = 0; Item < strValue.Length; Item++)
        {
            ListItem liTemp = new ListItem(strValue[Item], strValue[Item]);
            ddlFtpIp.Items.Add(liTemp);
        }

        //綁定製卡廠選單
        DataTable dtMerch = new DataTable();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtMerch))
        {
            ddlMerch.DataSource = dtMerch;
            ddlMerch.DataTextField = "PROPERTY_NAME";
            ddlMerch.DataValueField = "PROPERTY_CODE";
            ddlMerch.DataBind();
        }
    }


    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        DataTable dtFileInfo = new DataTable();
        string strMsg = string.Empty;
        int iTotalCount = 0;
        try
        {
            if (BRM_FileInfo.Search("", ref dtFileInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsg))
            {
                MergeTable(ref dtFileInfo);
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvFUNCTION.DataSource = dtFileInfo;
                grvFUNCTION.Visible = true;
                this.grvFUNCTION.DataBind();
            }
            else
            {
                this.gpList.RecordCount = 0;
                this.gpList.Visible = false;
                this.grvFUNCTION.DataSource = null;
                this.grvFUNCTION.DataBind();
                this.grvFUNCTION.Visible = false;
            }
        }
        catch
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
    }

    /// <summary>
    /// 功能說明:MergeTable加載Job DESCRIPTION
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/21
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtFile)
    {
        string strMsgID = string.Empty;

        dtFile.Columns.Add("DESCRIPTION");
        DataTable dtInfo = BRM_AutoJob.SearchAllJob(ref strMsgID);
        if (string.IsNullOrEmpty(strMsgID))
        {
            foreach (DataRow row in dtFile.Rows)
            {
                DataRow[] rowInfo = dtInfo.Select("JOB_ID='" + row["JOB_ID"].ToString() + "'");
                if (null != rowInfo && rowInfo.Length > 0)
                {
                    row["DESCRIPTION"] = rowInfo[0]["DESCRIPTION"].ToString();
                }
                else
                {
                    row["DESCRIPTION"] = string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// 清空輸入欄位
    /// </summary>
    /// <returns></returns>
    private void ClearInput()
    {
        txtFileName.Text = "";
        //txtMerchCode.Text = "";
        //txtMerchName.Text = "";
        txtZipPwd.Text = "";
        txtImpSort.Text = "";

        txtBlockCode.Text = "";
        txtResonCode.Text = "";
        txtMemo.Text = "";
        txtActionCode.Text = "";
        txtCanCelTime.Text = "";
        txtCwbRegion.Text = "";
        txtImportDate.Text = "";
        txtFtpPath.Text = "";
        txtFtpUser.Text = "";
        txtFtpPwd.Text = "";
        BindDDl();
    }

    [System.Web.Services.WebMethod]
    public static string GetString(string strMsg)
    {
        return BaseHelper.GetShowText("06_06060000_037") + strMsg;
    }

    [System.Web.Services.WebMethod]
    public static string GetString1()
    {
        return BaseHelper.GetShowText("06_06060000_038");
    }

    /// <summary>
    /// 檢核輸入欄位
    /// </summary>
    /// <returns></returns>
    private bool CheckInput()
    {
        if (txtFileName.Text == "")
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_001");
            return false;
        }

        //if (txtMerchCode.Text == "")
        //{
        //    MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_003");
        //    return false;
        //}

        //if (txtMerchName.Text == "")
        //{
        //    MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_004");
        //    return false;
        //}

        if (ddlJobName.SelectedValue.Equals("0109"))
        {
            if (txtBlockCode.Text == "")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_005");
                return false;
            }

            if (txtResonCode.Text == "")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_006");
                return false;
            }

            if (txtMemo.Text == "")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_007");
                return false;
            }

            if (txtActionCode.Text == "")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_008");
                return false;
            }

            if (txtCwbRegion.Text == "")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_009");
                return false;
            }

            if (txtCanCelTime.Text == "")
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_010");
                return false;
            }
        }

        if (txtFtpPath.Text == "")
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_011");
            return false;
        }

        if (txtFtpUser.Text == "")
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_012");
            return false;
        }

        if (txtFtpPwd.Text == "")
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_013");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 得到查询的SQL语句
    /// </summary>
    /// <returns>SQL语句</returns>
    private string GetFilterCondition(string strFileId)
    {
        SqlHelper Sql = new SqlHelper();
        //*查詢該FunctionKey下的角色權限
        Sql.AddCondition(Entity_FileInfo.M_FileId, Operator.Equal, DataTypeUtils.Integer,strFileId);
        return Sql.GetFilterCondition();
    }


    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/22
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //Response.Redirect("P060606000001.aspx#chapter1");
        ViewState["Operaction"] = "ADD";
        this.tblJob.Visible = true;
        ClearInput();
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        //* 檢查是否有job行被選中
        bool blnSelected = false;
        string strFileID = string.Empty;
        for (int intLoop = 0; intLoop < this.grvFUNCTION.Rows.Count; intLoop++)
        {
            HtmlInputRadioButton radRole = (HtmlInputRadioButton)this.grvFUNCTION.Rows[intLoop].FindControl("radRole");
            HiddenField FileId = (HiddenField)this.grvFUNCTION.Rows[intLoop].FindControl("FileId");
            if (radRole.Checked == true)
            {
                blnSelected = true;
                strFileID = FileId.Value.Trim();
                break;
            }
        }

        //* 如果沒有勾選一筆需修改的角色
        if (!blnSelected)
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_018");
            return;
        }

        this.tblJob.Visible = true;
        ViewState["Operaction"] = "EDIT";
        DataTable dtblAutoJob = null;
        int IntTotalCount = 0;
        string strMsgID = string.Empty;

        if (!BRM_FileInfo.Search(GetFilterCondition(strFileID), ref dtblAutoJob, 1, 1, ref IntTotalCount, ref strMsgID))
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
            return;
        }
        else
        {

            //*賦值
            ddlJobName.SelectByValue(dtblAutoJob.Rows[0]["Job_ID"]);
            txtFileName.Text = dtblAutoJob.Rows[0]["FtpFileName"].ToString();
            if (null != dtblAutoJob.Rows[0]["AMPMFlg"])
            {
                if (dtblAutoJob.Rows[0]["AMPMFlg"].ToString().Equals("A"))
                {
                    rdoAM.Checked = true;
                }

                if (dtblAutoJob.Rows[0]["AMPMFlg"].ToString().Equals("P"))
                {
                    rdoPM.Checked = true;
                }
            }

            ddlMerch.SelectByValue(dtblAutoJob.Rows[0]["MerchCode"]);
            //txtMerchCode.Text = dtblAutoJob.Rows[0]["MerchCode"].ToString();
            //txtMerchName.Text = dtblAutoJob.Rows[0]["MerchName"].ToString();

            if (null != dtblAutoJob.Rows[0]["ZipPwd"])
            {
                txtZipPwd.Text = RedirectHelper.GetDecryptString(dtblAutoJob.Rows[0]["ZipPwd"].ToString());
            }


            if (null != dtblAutoJob.Rows[0]["ImpSort"])
            {
                txtImpSort.Text = dtblAutoJob.Rows[0]["ImpSort"].ToString();
            }

            ddlCardType.SelectByValue(dtblAutoJob.Rows[0]["CardType"].ToString());
            ddlUsingType.SelectByValue(dtblAutoJob.Rows[0]["Status"].ToString());
            ddlSubFunction.SelectByValue(dtblAutoJob.Rows[0]["FunctionFlg"].ToString());

            SelectSubFunction(ddlJobName.SelectedValue);
            if (null != dtblAutoJob.Rows[0]["PExpFlg"])
            {
                if (dtblAutoJob.Rows[0]["PExpFlg"].ToString().Equals("1"))
                {
                    ddlTakewalFlg.SelectByValue("1");
                }
            }
            if (null != dtblAutoJob.Rows[0]["BExpFlg"])
            {
                if (dtblAutoJob.Rows[0]["BExpFlg"].ToString().Equals("1"))
                {
                    ddlTakewalFlg.SelectByValue("2");
                }
            }

            if (null != dtblAutoJob.Rows[0]["PExpFlg"] && null != dtblAutoJob.Rows[0]["BExpFlg"])
            {
                if (dtblAutoJob.Rows[0]["PExpFlg"].ToString().Equals("1") && dtblAutoJob.Rows[0]["BExpFlg"].ToString().Equals("1"))
                {
                    ddlTakewalFlg.SelectByValue("0");
                }
            }

            if (null != dtblAutoJob.Rows[0]["BLKCode"])
            {
                txtBlockCode.Text = dtblAutoJob.Rows[0]["BLKCode"].ToString();
            }

            if (null != dtblAutoJob.Rows[0]["ReasonCode"])
            {
                txtResonCode.Text = dtblAutoJob.Rows[0]["ReasonCode"].ToString();
            }
            if (null != dtblAutoJob.Rows[0]["MEMO"])
            {
                txtMemo.Text = dtblAutoJob.Rows[0]["MEMO"].ToString();
            }
            if (null != dtblAutoJob.Rows[0]["ActionCode"])
            {
                txtActionCode.Text = dtblAutoJob.Rows[0]["ActionCode"].ToString();
            }
            if (null != dtblAutoJob.Rows[0]["CWBRegions"])
            {
                txtCwbRegion.Text = dtblAutoJob.Rows[0]["CWBRegions"].ToString();
            }
            if (null != dtblAutoJob.Rows[0]["CancelTime"])
            {
                txtCanCelTime.Text = dtblAutoJob.Rows[0]["CancelTime"].ToString();
            }
            if (null != dtblAutoJob.Rows[0]["ImportDate"])
            {
                txtImportDate.Text = dtblAutoJob.Rows[0]["ImportDate"].ToString();
            }

            ddlFtpIp.SelectByValue(dtblAutoJob.Rows[0]["FtpIP"].ToString());
            txtFtpPath.Text = dtblAutoJob.Rows[0]["FtpPath"].ToString();
            txtFtpUser.Text = dtblAutoJob.Rows[0]["FtpUserName"].ToString();
            txtFtpPwd.Text = dtblAutoJob.Rows[0]["FtpPwd"].ToString();
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //* 檢查是否有job行被選中
        bool blnSelected = false;
        string strFileID = string.Empty;
        for (int intLoop = 0; intLoop < this.grvFUNCTION.Rows.Count; intLoop++)
        {
            HtmlInputRadioButton radRole = (HtmlInputRadioButton)this.grvFUNCTION.Rows[intLoop].FindControl("radRole");
            HiddenField FileId = (HiddenField)this.grvFUNCTION.Rows[intLoop].FindControl("FileId");
            if (radRole.Checked == true)
            {
                blnSelected = true;
                strFileID = FileId.Value.Trim();
                break;
            }
        }

        //* 如果沒有勾選一筆需修改的角色
        if (!blnSelected)
        {
            MessageHelper.ShowMessage(this.UpdatePanel1, "06_06040000_018");
            return;
        }
        
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06060000_000") + "：" + BaseHelper.GetShowText("06_06060000_035");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "D");

        string strMsgID = "";
        Entity_FileInfo esFileInfo = new Entity_FileInfo();
        esFileInfo.FileId = Convert.ToInt16(strFileID); //FileID

        BRM_FileInfo.Delete(esFileInfo, ref strMsgID);
        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
        btnDelete.Attributes.Clear();

        BindGridView();
    }

    
    /// <summary>
    /// 【確認】按鈕點擊時的處理
    /// </summary>
    protected void btnOK_Click(object sender, System.EventArgs e)
    {
        #region ADD 交換檔資料
        if (ViewState["Operaction"].ToString() == "ADD")
        {
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06060000_000") + "：" + BaseHelper.GetShowText("06_06060000_033");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");

            if (CheckInput())
            {
                //string strMsgID = "";
                Entity_FileInfo esFileInfo = new Entity_FileInfo();

                esFileInfo.Job_ID = ddlJobName.SelectedValue.Trim(); //JobID
                esFileInfo.FtpFileName = txtFileName.Text.Trim();    //交換檔名稱
                esFileInfo.ZipPwd =RedirectHelper.GetEncryptParam(txtZipPwd.Text.Trim());           //交換檔壓縮密碼 加密
                esFileInfo.MerchCode = ddlMerch.SelectedValue.Trim();       //製卡廠代碼
                esFileInfo.MerchName = ddlMerch.SelectedItem.Text.Trim();   //製卡廠名稱

                if (rdoAM.Checked)
                {
                    esFileInfo.AMPMFlg = "A";                        //上/下午匯入註記
                }
                else
                {
                    if (rdoPM.Checked)
                    {
                        esFileInfo.AMPMFlg = "P";
                    }
                }
                esFileInfo.CardType = ddlCardType.SelectedValue.Trim();  //卡別
                esFileInfo.CancelTime = txtCanCelTime.Text.Trim();       //註消時間
                esFileInfo.BLKCode = txtBlockCode.Text.Trim();
                esFileInfo.MEMO = txtMemo.Text.Trim();
                esFileInfo.ReasonCode = txtResonCode.Text.Trim();
                esFileInfo.ActionCode = txtActionCode.Text.Trim();          
                esFileInfo.CWBRegions = txtCwbRegion.Text.Trim();         
                esFileInfo.ImpSort = txtImpSort.Text.Trim();              //匯檔順序

                esFileInfo.FunctionFlg = ddlSubFunction.SelectedValue.Trim(); //子功能標示
          
                if (ddlTakewalFlg.SelectedValue.Equals("1"))
                {
                    esFileInfo.PExpFlg = "1";
                }
                else
                {
                    esFileInfo.PExpFlg = "0";                           //招領資料匯出標識
                }
                if (ddlTakewalFlg.SelectedValue.Equals("2"))
                {
                    esFileInfo.BExpFlg = "1";
                }
                else
                {
                    esFileInfo.BExpFlg = "0";                           //退件資料匯出標識
                }
                if (ddlTakewalFlg.SelectedValue.Equals("0"))
                {
                    esFileInfo.PExpFlg = "1";
                    esFileInfo.PExpFlg = "1";
                } 

                esFileInfo.Status = ddlUsingType.SelectedValue.Trim();   //使用註記
                esFileInfo.ImportDate = txtImportDate.Text.Trim();       //匯入日期

                esFileInfo.FtpIP = ddlFtpIp.SelectedValue.Trim();        //FTP IP
                esFileInfo.FtpPath = txtFtpPath.Text.Trim();             //FTP路徑.
                esFileInfo.FtpUserName = txtFtpUser.Text.Trim();         //登陸FTP USER 
                //esFileInfo.FtpPwd = RedirectHelper.GetEncryptParam(txtFtpPwd.Text.Trim());
                esFileInfo.FtpPwd = txtFtpPwd.Text.Trim();               //登陸FTP PWD
                

                if (!BRM_FileInfo.insert(esFileInfo))
                {
                    //*更新不成功則在端末顯示
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06040000_014"));
                    return;
                }
                else
                {
                    //*清空畫面輸入欄位，便于下次添加
                    ClearInput();
                    BindGridView();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06040000_015"));
                }
            }
        }
        #endregion

        #region 更新交換檔資料
        if (ViewState["Operaction"].ToString() == "EDIT")
        {
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06060000_000") + "：" + BaseHelper.GetShowText("06_06060000_034");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
            
            if (CheckInput())
            {
                Entity_FileInfo esFileInfo = new Entity_FileInfo();
                esFileInfo.Job_ID = ddlJobName.SelectedValue.Trim(); //JobID
                esFileInfo.FtpFileName = txtFileName.Text.Trim();    //交換檔名稱
                if (!string.IsNullOrEmpty(txtZipPwd.Text))
                {
                    esFileInfo.ZipPwd = RedirectHelper.GetEncryptParam(txtZipPwd.Text.Trim());  //交換檔壓縮密碼 加密
                }
                else
                {
                    esFileInfo.ZipPwd = string.Empty;
                }
                esFileInfo.MerchCode = ddlMerch.SelectedValue.Trim();     //製卡廠代碼
                esFileInfo.MerchName = ddlMerch.SelectedItem.Text.Trim();     //製卡廠名稱

                if (rdoAM.Checked)
                {
                    esFileInfo.AMPMFlg = "A";                        //上/下午匯入註記
                }
                else
                {
                    if (rdoPM.Checked)
                    {
                        esFileInfo.AMPMFlg = "P";
                    }
                }
                esFileInfo.CardType = ddlCardType.SelectedValue.Trim();  //卡別
                esFileInfo.CancelTime = txtCanCelTime.Text.Trim();       //註消時間
                esFileInfo.BLKCode = txtBlockCode.Text.Trim();
                esFileInfo.MEMO = txtMemo.Text.Trim();
                esFileInfo.ReasonCode = txtResonCode.Text.Trim();
                esFileInfo.ActionCode = txtActionCode.Text.Trim();
                esFileInfo.CWBRegions = txtCwbRegion.Text.Trim();
                esFileInfo.ImpSort = txtImpSort.Text.Trim();              //匯檔順序

                esFileInfo.FunctionFlg = ddlSubFunction.SelectedValue.Trim(); //子功能標示
                if (ddlTakewalFlg.SelectedValue.Equals("1"))
                {
                    esFileInfo.PExpFlg = "1";
                }
                else
                {
                    esFileInfo.PExpFlg = "0";                           //招領資料匯出標識
                }

                if (ddlTakewalFlg.SelectedValue.Equals("2"))
                {
                    esFileInfo.BExpFlg = "1";
                }
                else
                {
                    esFileInfo.BExpFlg = "0";                           //退件資料匯出標識
                }

                if (ddlTakewalFlg.SelectedValue.Equals("0"))
                {
                    esFileInfo.PExpFlg = "1";
                    esFileInfo.BExpFlg = "1";
                }
  

                esFileInfo.Status = ddlUsingType.SelectedValue.Trim();   //使用註記
                esFileInfo.ImportDate = txtImportDate.Text.Trim();       //匯入日期

                esFileInfo.FtpIP = ddlFtpIp.SelectedValue.Trim();        //FTP IP
                esFileInfo.FtpPath = txtFtpPath.Text.Trim();             //FTP路徑.
                esFileInfo.FtpUserName = txtFtpUser.Text.Trim();         //登陸FTP USER 
                //esFileInfo.FtpPwd = RedirectHelper.GetEncryptParam(txtFtpPwd.Text.Trim());
                esFileInfo.FtpPwd = txtFtpPwd.Text.Trim();               //登陸FTP PWD


                string strFileID = string.Empty;
                for (int intLoop = 0; intLoop < this.grvFUNCTION.Rows.Count; intLoop++)
                {
                    HtmlInputRadioButton radRole = (HtmlInputRadioButton)this.grvFUNCTION.Rows[intLoop].FindControl("radRole");
                    HiddenField FileId = (HiddenField)this.grvFUNCTION.Rows[intLoop].FindControl("FileId");
                    if (radRole.Checked == true)
                    {
                        strFileID = FileId.Value.Trim();
                        break;
                    }
                }

                SqlHelper sqlCmd = new SqlHelper();
                sqlCmd.AddCondition(Entity_FileInfo.M_FileId, Operator.Equal, DataTypeUtils.String, strFileID);
                string[] strField = new string[] { Entity_FileInfo.M_FtpFileName, Entity_FileInfo.M_ZipPwd, 
                    Entity_FileInfo.M_MerchCode, Entity_FileInfo.M_MerchName, Entity_FileInfo.M_AMPMFlg,
                    Entity_FileInfo.M_CardType,Entity_FileInfo.M_CancelTime,Entity_FileInfo.M_BLKCode,
                    Entity_FileInfo.M_MEMO,Entity_FileInfo.M_ReasonCode,Entity_FileInfo.M_ActionCode,
                    Entity_FileInfo.M_CWBRegions,Entity_FileInfo.M_ImpSort,Entity_FileInfo.M_FunctionFlg,
                    Entity_FileInfo.M_PExpFlg,Entity_FileInfo.M_BExpFlg,Entity_FileInfo.M_Status,
                    Entity_FileInfo.M_ImportDate,Entity_FileInfo.M_FtpIP,Entity_FileInfo.M_FtpPath,
                    Entity_FileInfo.M_FtpUserName,Entity_FileInfo.M_FtpPwd};
                //MerchCode  MerchName  AMPMFlg  CardType  CancelTime  BLKCode  MEMO ReasonCode ActionCode CWBRegions
                //ImpSort  FunctionFlg PExpFlg  BExpFlg  Status ImportDate FtpIP FtpPath  FtpUserName  FtpPwd
                if (BRM_FileInfo.UpdateEntityByCondition(esFileInfo, sqlCmd.GetFilterCondition(), strField))
                {
                    ClearInput();
                    BindGridView();
                    btnDelete.Attributes.Clear();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06040000_022"));
                    UpdatePanel1.Update();
                }
                else
                {
                    //*更新不成功則在端末顯示
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06040000_021"));
                    return;
                }
            }

        }
        #endregion

        BindGridView();
    }

    protected void btnConcel_Click1(object sender, EventArgs e)
    {
        BindDDl();
        txtActionCode.Text = "";
        txtBlockCode.Text = "";
        txtCanCelTime.Text = "";
        txtCwbRegion.Text = "";
        txtFileName.Text = "";
        txtFtpPath.Text = "";
        txtFtpPwd.Text = "";
        txtFtpUser.Text = "";
        txtImportDate.Text = "";
        txtImpSort.Text = "";
        txtMemo.Text = "";
        //txtMerchCode.Text = "";
        //txtMerchName.Text = "";
        txtResonCode.Text = "";
        txtZipPwd.Text = "";
        this.tblJob.Visible = false;
    }
    protected void ddlJobName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (null != ddlJobName && !string.IsNullOrEmpty(ddlJobName.SelectedValue))
        {
            SelectSubFunction(ddlJobName.SelectedValue);
        }
        else
        {
            ddlSubFunction.Enabled = false;
        }

    }

    protected void SelectSubFunction(string strJobID)
    {
        string strSearchID = string.Empty;
        switch (ddlJobName.SelectedValue)
        {
            case "0113":
                strSearchID = "12";
                break;
            case "0115":
                strSearchID = "13";
                break;
            case "0118":
                strSearchID = "14";
                break;
            case "0119":
                strSearchID = "15";
                break;
            case "0120":
                strSearchID = "36";
                break;
            default:
                ddlSubFunction.Enabled = false;
                break;
        }

        if (!string.IsNullOrEmpty(strSearchID))
        {
            //綁定子功能選單
            DataTable dtSubFunction = new DataTable();
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", strSearchID, ref dtSubFunction))
            {
                ddlSubFunction.DataSource = dtSubFunction;
                ddlSubFunction.DataTextField = "PROPERTY_NAME";
                ddlSubFunction.DataValueField = "PROPERTY_CODE";
                ddlSubFunction.DataBind();
                ddlSubFunction.Enabled = true;
            }
        }
    }
}