//******************************************************************
//*  作    者：宋戈
//*  功能說明：
//*  創建日期：2009/11/05
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI;
using System.ComponentModel;
using Framework.Common.Logging;

public partial class Common_Controls_Address : System.Web.UI.UserControl
{
    private static string FuncKey = "06";
    #region 宣告變數/屬性
    private string _strAddress;
    private string _strZip;
    //* 地址
    public string strAddress
    {
        get
        {
            return this.dropAdd1_1.SelectedItem.Text + this.dropAdd1_2.SelectedItem.Text;
        }
    } 
    //* 郵遞區號
    public string strZip
    {
        get
        {
            return this.dropAdd1_2.SelectedValue;
        }
    }
    /// <summary>
    /// 设置是否ReadOnly
    /// </summary>
    [Category("CustProperty"), DefaultValue(true), Description("Disabled")]
    public bool Disabled
    {
        set 
        {
            this.dropAdd1_1.Attributes["Disabled"] = value ? "Disabled" : "";
            this.dropAdd1_2.Attributes["Disabled"] = value ? "Disabled" : "";
        }
    }

    public delegate void OnchangeHandler();
    public event OnchangeHandler ChangeValues;
    #endregion

    #region Event

    /// <summary>
    /// 載入元件,初始化下拉選單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.InitalAdd1_1();
        }
    }

    /// <summary>
    /// 當選擇第一個下拉選單.初始化相應的第二個下拉選單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropAdd1_1_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.InitalAdd1_2();
    }

    /// <summary>
    /// 當選擇第二個下拉選單.修改郵遞區號和地址
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropAdd1_2_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.changePrivateData();
    }

    #endregion

    #region Function

    /// <summary>
    /// 初始化兩個下拉選單(DB內Add1一般爲6碼文字前3字Add1_1,后3字Add1_2)
    /// 修改紀錄:2020/12/18_Ares_Luke-修正 Substring Exception 問題
    /// </summary>
    /// <param name="strAdd1_1">預設值文字</param>
    /// <returns>True - 成功; False - 失敗</returns>
    public bool InitalAdd1(string strAdd)
    {     
   
        try
        {
            if (string.IsNullOrWhiteSpace(strAdd) || strAdd.Length < 6)
            {
                Logging.Log("CustAddress.InitalAdd1: 地址長度不符。", LogLayer.UI);
                return false;
            }

            string strAdd1_1 = strAdd.Substring(0, 3);
            string strAdd1_2 = strAdd.Substring(3, 3);

            //BRM_PROPERTY_CODE.GetCommonProperty
            DataTable dtblProperty = new DataTable();
            //* 地址1_1
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(FuncKey, "ADD1_1", ref dtblProperty))
            {
                if (dtblProperty.Rows.Count > 0)
                {
                    this.dropAdd1_1.DataTextField = "PROPERTY_NAME";
                    this.dropAdd1_1.DataValueField = "PROPERTY_CODE";
                    this.dropAdd1_1.DataSource = dtblProperty;
                    this.dropAdd1_1.DataBind();
                    this.dropAdd1_1.SelectByText(strAdd1_1.Trim());
                }
            }
            else
            {
                //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            }
            InitalAdd1_2(strAdd1_2);
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            return false;
        }   
     
        return true;
    }


    /// <summary>
    /// 初始化第一個下拉選單(無預設值)
    /// </summary>
    /// <returns>True - 成功; False - 失敗</returns>
    public bool InitalAdd1_1()
    {
        try
        {
            DataTable dtblProperty = new DataTable();
            //* 地址1_1
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(FuncKey, "ADD1_1", ref dtblProperty))
            {
                if (dtblProperty.Rows.Count > 0)
                {
                    this.dropAdd1_1.DataTextField = "PROPERTY_NAME";
                    this.dropAdd1_1.DataValueField = "PROPERTY_CODE";
                    this.dropAdd1_1.DataSource = dtblProperty;
                    this.dropAdd1_1.DataBind();
                }
            }
            else
            {
                //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            }
        }
        catch(Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            return false;
        }
        InitalAdd1_2();
        return true;
    }

    /// <summary>
    /// 初始化第一個下拉選單(有預設值)
    /// </summary>
    /// <param name="strAdd1_1">預設值文字</param>
    /// <returns>True - 成功; False - 失敗</returns>
    public bool InitalAdd1_1(string strAdd1_1)
    {
        try
        {
            DataTable dtblProperty = new DataTable();
            //* 地址1_1
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(FuncKey, "ADD1_1", ref dtblProperty))
            {
                if (dtblProperty.Rows.Count > 0)
                {
                    this.dropAdd1_1.DataTextField = "PROPERTY_NAME";
                    this.dropAdd1_1.DataValueField = "PROPERTY_CODE";
                    this.dropAdd1_1.DataSource = dtblProperty;
                    this.dropAdd1_1.DataBind();
                    this.dropAdd1_1.SelectByText(strAdd1_1.Trim());
                }
            }
            else
            {
                //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            }
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            return false;
        }
        InitalAdd1_2();
        return true;
    }

    /// <summary>
    /// 初始化第二個下拉選單(無預設值)
    /// </summary>
    /// <returns>True - 成功; False - 失敗</returns>
    public bool InitalAdd1_2()
    {
        try
        {
            DataTable dtblProperty = new DataTable();                       //* 屬性結果列表
            string strCode;                                                 //* 地址1_1下拉選單的Value
            strCode = this.dropAdd1_1.SelectedValue.ToString();

            //* 如果不爲空,則查詢第二個下拉選單的內容并賦值
            if (!String.IsNullOrEmpty(strCode))
            {
                //* 地址1_2
                if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(FuncKey, strCode, ref dtblProperty))
                {
                    if (dtblProperty.Rows.Count > 0)
                    {
                        this.dropAdd1_2.DataTextField = "PROPERTY_NAME";
                        this.dropAdd1_2.DataValueField = "PROPERTY_CODE";
                        this.dropAdd1_2.DataSource = dtblProperty;
                        this.dropAdd1_2.DataBind();
                    }
                }
                else
                {
                    //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
                }
            }
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            return false;
        }
        changePrivateData();
        return true;
    }

    /// <summary>
    /// 初始化第二個下拉選單(無預設值)
    /// </summary>
    /// <returns>True - 成功; False - 失敗</returns>
    public bool InitalAdd1_2(string strAdd1_2)
    {
        try
        {
            DataTable dtblProperty = new DataTable();                       //* 屬性結果列表
            string strCode;                                                 //* 地址1_1下拉選單的Value
            strCode = this.dropAdd1_1.SelectedValue.ToString();

            //* 如果不爲空,則查詢第二個下拉選單的內容并賦值
            if (!String.IsNullOrEmpty(strCode))
            {
                //* 地址1_2
                if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(FuncKey, strCode, ref dtblProperty))
                {
                    if (dtblProperty.Rows.Count > 0)
                    {
                        this.dropAdd1_2.DataTextField = "PROPERTY_NAME";
                        this.dropAdd1_2.DataValueField = "PROPERTY_CODE";
                        this.dropAdd1_2.DataSource = dtblProperty;
                        this.dropAdd1_2.DataBind();
                        this.dropAdd1_2.SelectByText(strAdd1_2.Trim());
                    }
                }
                else
                {
                    //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
                }
            }
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            return false;
        }
        changePrivateData();
        return true;
    }

    /// <summary>
    /// 修改屬性資料,並且拋出事件
    /// </summary>
    /// <returns></returns>
    public bool changePrivateData()
    {
        //* 地址
        this._strAddress = this.dropAdd1_1.SelectedItem.Text + this.dropAdd1_2.SelectedItem.Text;
        //* 郵遞區號
        this._strZip = this.dropAdd1_2.SelectedValue.ToString();

        if (ChangeValues != null)
        {
            ChangeValues();
        }

        return true;
    }

    #endregion

}


