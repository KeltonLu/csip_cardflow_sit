//******************************************************************
//*  功能說明：綜合資料處理新增UI
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


public partial class P060201000003 : PageBase
{


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
            //*加載Action，取卡方式，製卡廠名稱
            BindAction();
            BindKind();
            BindMerchCode();

            ViewState["FlgEdit"] = "FALSE";
        }
    }

    /// <summary>
    /// 功能說明:綁定Action
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/03
    /// 修改記錄:
    /// </summary>
    public void BindAction()
    {
        DataTable dtAction = new DataTable();
        string strMsgID = string.Empty;

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtAction))
        {
            if (dtAction.Rows.Count > 0)
            {
                this.dropAction.DataSource = dtAction;
                this.dropAction.DataValueField = "PROPERTY_CODE";
                this.dropAction.DataTextField = "PROPERTY_NAME";
                this.dropAction.DataBind();
            }
        }
    }
    /// <summary>
    /// 功能說明:綁定取卡方式
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/03
    /// 修改記錄:
    /// </summary>
    public void BindKind()
    {
        DataTable dtKind = new DataTable();
        string strMsgID = string.Empty;

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKind))
        {
            foreach (DataRow dr in dtKind.Rows)
            {
                if (dr["PROPERTY_CODE"].ToString().Equals("0") ||
                    dr["PROPERTY_CODE"].ToString().Equals("1") ||
                    dr["PROPERTY_CODE"].ToString().Equals("2") ||
                    dr["PROPERTY_CODE"].ToString().Equals("3") ||
                    dr["PROPERTY_CODE"].ToString().Equals("4") ||
                    dr["PROPERTY_CODE"].ToString().Equals("10") ||
                    dr["PROPERTY_CODE"].ToString().Equals("11"))
                {
                    ListItem liTmp = new ListItem(dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                    dropKind.Items.Add(liTmp);
                }
            }
        }
    }
    /// <summary>
    /// 功能說明:綁定製卡廠名稱
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/03
    /// 修改記錄:
    /// </summary>
    public void BindMerchCode()
    {
        DataTable dtMerchCode = new DataTable();
        string strMsgID = string.Empty;

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtMerchCode))
        {
            if (dtMerchCode.Rows.Count > 0)
            {
                this.dropMerch_Code.DataSource = dtMerchCode;
                this.dropMerch_Code.DataValueField = "PROPERTY_CODE";
                this.dropMerch_Code.DataTextField = "PROPERTY_NAME";
                this.dropMerch_Code.DataBind();
            }
        }
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

        Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
        CardBaseInfo.is_LackCard = "1";                                    //*缺卡狀態(不缺卡)
        CardBaseInfo.disney_code = "";
        CardBaseInfo.OriginalDBflg = "0";
        CardBaseInfo.OutStore_Status = "0";
        CardBaseInfo.IntoStore_Status = "0";
        CardBaseInfo.Urgency_Flg = "0";
        CardBaseInfo.id = this.txtId.Text.Trim();                           //*身份證字號
        CardBaseInfo.action = this.dropAction.SelectedValue.Trim();         //*卡片類別
        CardBaseInfo.custname = this.txtCustname.Text.Trim();               //*收件人姓名
        CardBaseInfo.cardtype = this.txtCardtype.Text.Trim();               //*TYPE
        CardBaseInfo.trandate = this.txtTrandate.Text.Trim();               //*轉檔日
        CardBaseInfo.affinity = this.txtAffinity.Text.Trim();               //*認同代碼
        CardBaseInfo.indate1 = this.txtIndate1.Text.Trim();                 //*製卡日
        CardBaseInfo.photo = this.txtPhoto.Text.Trim();                     //*PHOTO
        CardBaseInfo.maildate = this.txtMaildate.Text.Trim();               //*郵寄日
        CardBaseInfo.monlimit = this.txtMonlimit.Text.Trim();               //*額度
        CardBaseInfo.cardno = this.txtCardno.Text.Trim();                   //*卡號一
        CardBaseInfo.cardno2 = this.txtCardno2.Text.Trim();                 //*卡號二
        CardBaseInfo.expdate = this.txtExpdate.Text.Trim();                 //*有效期限一
        CardBaseInfo.expdate2 = this.txtExpdate2.Text.Trim();               //*有效期限二
        CardBaseInfo.zip = this.lblZip.Text.Trim();                         //*郵遞區號
        CardBaseInfo.kind = this.dropKind.SelectedValue.Trim();             //*取卡方式
        CardBaseInfo.add1 = this.dropAdd1.strAddress.ToString();            //*地址一
        CardBaseInfo.mailno = this.txtMailno.Text.Trim();                   //*掛號號碼
        CardBaseInfo.add2 = this.txtAdd2.Text.Trim();                       //*地址二
        CardBaseInfo.Merch_Code = this.dropMerch_Code.SelectedValue.Trim(); //*製卡商名稱
        CardBaseInfo.add3 = this.txtAdd3.Text.Trim();                       //*地址三
        
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020102_000");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");
        //*重複數據的判斷
        if (BRM_TCardBaseInfo.IsRepeatByAll(CardBaseInfo))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06040100_027");
            return;
        }

        if (!BRM_TCardBaseInfo.Insert(CardBaseInfo, ref strMsgID))
        {
            //* 增加不成功時，提示訊息
            MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
        }
        else
        {
            MessageHelper.ShowMessageAndGoto(UpdatePanel1, "P060201000001.aspx", strMsgID);
        }
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
        //*身分證字號
        if (string.IsNullOrEmpty(this.txtId.Text))
        {
            strMsgID = "06_06020102_000";
            txtId.Focus();
            return false;
        }
        if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
        {
            strMsgID = "06_06020102_001";
            txtId.Focus();
            return false;
        }
        //*收件人姓名
        if (string.IsNullOrEmpty(this.txtCustname.Text))
        {
            strMsgID = "06_06020102_002";
            txtCustname.Focus();
            return false;
        }
        //*TYPE
        if (string.IsNullOrEmpty(this.txtCardtype.Text.Trim()))
        {
            strMsgID = "06_06020102_003";
            txtCardtype.Focus();
            return false;
        }
        //*TYPE
        if (!string.IsNullOrEmpty(this.txtCardtype.Text.Trim()))
        {
            if (ValidateHelper.IsChinese(this.txtCardtype.Text.Trim()))
            {
                strMsgID = "06_06020102_010";
                txtCardtype.Focus();
                return false;
            }
        }
        //*轉檔日
        if (string.IsNullOrEmpty(this.txtTrandate.Text.Trim()))
        {
            strMsgID = "06_06020102_004";
            txtTrandate.Focus();
            return false;
        }
        //*認同代碼
        if (string.IsNullOrEmpty(this.txtAffinity.Text.Trim()))
        {
            strMsgID = "06_06020102_005";
            txtAffinity.Focus();
            return false;
        }
        //*PHOTO
        if (string.IsNullOrEmpty(this.txtPhoto.Text.Trim()))
        {
            strMsgID = "06_06020102_006";
            txtPhoto.Focus();
            return false;
        }
        if (ValidateHelper.IsChinese(this.txtPhoto.Text.Trim()))
        {
            strMsgID = "06_06020102_011";
            txtCardno.Focus();
            return false;
        }
        //*卡號一
        if (string.IsNullOrEmpty(this.txtCardno.Text.Trim()))
        {
            strMsgID = "06_06020102_007";
            txtCardno.Focus();
            return false;
        }
        if (ValidateHelper.IsChinese(this.txtCardno.Text.Trim()))
        {
            strMsgID = "06_06020102_012";
            txtCardno.Focus();
            return false;
        }
        //*有效期限一
        if (string.IsNullOrEmpty(this.txtExpdate.Text.Trim()))
        {
            strMsgID = "06_06020102_014";
            txtExpdate.Focus();
            return false;
        }
        //*地址一
        if (string.IsNullOrEmpty(this.dropAdd1.strAddress))
        {
            strMsgID = "06_06020102_015";
            dropAdd1.Focus();
            return false;
        }

        //*卡號二
        if (!string.IsNullOrEmpty(this.txtCardno2.Text.Trim()))
        {
            if (ValidateHelper.IsChinese(this.txtCardno2.Text.Trim()))
            {
                strMsgID = "06_06020102_008";
                txtCardno2.Focus();
                return false;
            }
        }
        //*郵遞區號
        if (!string.IsNullOrEmpty(this.lblZip.Text.Trim()))
        {
            if (ValidateHelper.IsChinese(this.lblZip.Text.Trim()))
            {
                strMsgID = "06_06020102_013";
                lblZip.Focus();
                return false;
            }
        }
        //*掛號號碼
        if (!string.IsNullOrEmpty(this.txtMailno.Text.Trim()))
        {
            if (ValidateHelper.IsChinese(this.txtMailno.Text.Trim()))
            {
                strMsgID = "06_06020102_009";
                txtMailno.Focus();
                return false;
            }
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
        Response.Redirect("P060201000001.aspx");
    }
    /// <summary>
    /// 功能說明:綁定郵編
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    protected void dropAdd1_ChangeValues()
    {
        this.lblZip.Text = this.dropAdd1.strZip;
    }
}
