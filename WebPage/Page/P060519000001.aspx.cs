//******************************************************************
//*  功能說明：製卡相關資料查詢

//*  作    者：JUN HU
//*  創建日期：2010/07/04
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using System.Collections.Generic;
using System.IO;
using Framework.Common.JavaScript;
using Framework.Common.Utility;

public partial class P060519000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindState();
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051900_012");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051900_013");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051900_014");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051900_015");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051900_016");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051900_017");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051900_018");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06051900_019");
        this.grvUserView.Columns[8].HeaderText = BaseHelper.GetShowText("06_06051900_020");
        this.grvUserView.Columns[9].HeaderText = BaseHelper.GetShowText("06_06051900_021");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    private void BindGridView()
    {
        // 初始化報表參數
        Dictionary<String, String> param = new Dictionary<String, String>();
        if (chkCond(ref param))
        {
            try
            {
                DataTable dt = new DataTable();
                Int32 count = 0;
                Boolean result = BR_Excel_File.GetDataTable0519(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051900_004"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051900_005"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051900_005"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        String strMsgId = String.Empty;
        if (txtMaildateStart.Text.Trim().Equals(""))
        {
            txtMaildateStart.Focus();
            return false;
        }
        if (txtMaildateEnd.Text.Trim().Equals(""))
        {
            txtMaildateEnd.Focus();
            return false;
        }
        // 交寄日期起
        String inDateFrom = txtMaildateStart.Text.Trim().Equals("") ? "NULL" : this.txtMaildateStart.Text.Trim();
        param.Add("inDateFrom", inDateFrom);

        // 交寄日期訖
        String inDateTo = txtMaildateEnd.Text.Trim().Equals("") ? "NULL" : this.txtMaildateEnd.Text.Trim();
        param.Add("inDateTo", inDateTo);

        // 退件日期起迄
        if (txtBackDateEnd.Text.Trim().Equals("") && txtBackDateStart.Text.Trim().Equals(""))
        {
            param.Add("backDate", "NULL");
            param.Add("bDateFrom", "NULL");
            param.Add("bDateTo", "NULL");
        }
        else
        {
            param.Add("backDate", "0");
            if (txtBackDateStart.Text.Trim().Equals("") && !txtBackDateEnd.Text.Trim().Equals(""))
            {
                param.Add("bDateFrom", "NULL");
                param.Add("bDateTo", txtBackDateEnd.Text.Trim());
            }
            else if (!txtBackDateStart.Text.Trim().Equals("") && txtBackDateEnd.Text.Trim().Equals(""))
            {
                param.Add("bDateFrom", txtBackDateStart.Text.Trim());
                param.Add("bDateTo", "NULL");
            }
            else
            {
                param.Add("bDateFrom", txtBackDateStart.Text.Trim());
                param.Add("bDateTo", txtBackDateEnd.Text.Trim());
            }
        }

        // Action 起日
        String actionStart = txtActionStart.Text.Trim().Equals("") ? "00" : txtActionStart.Text.Trim();
        param.Add("actionStart", actionStart);

        // Action 訖日
        String actionEnd = txtActionEnd.Text.Trim().Equals("") ? "00" : txtActionEnd.Text.Trim();
        param.Add("actionEnd", actionEnd);

        // Card Type 起
        String cardTypeStart = txtCardTypeStart.Text.Trim().Equals("") ? "00" : txtCardTypeStart.Text.Trim();
        param.Add("cardTypeStart", cardTypeStart);

        // Card Type 訖
        String cardTypeEnd = txtCardTypeEnd.Text.Trim().Equals("") ? "00" : txtCardTypeEnd.Text.Trim();
        param.Add("cardTypeEnd", cardTypeEnd);

        // 認同代號 起
        String affinityStart = txtConfirmCodeStart.Text.Trim().Equals("") ? "00" : txtConfirmCodeStart.Text.Trim();
        param.Add("affinityStart", affinityStart);

        // 認同代號 訖
        String affinityEnd = txtConfirmCodeEnd.Text.Trim().Equals("") ? "00" : txtConfirmCodeEnd.Text.Trim();
        param.Add("affinityEnd", affinityEnd);

        // PhotoType 起
        String photoStart = txtPhotoTypeStart.Text.Trim().Equals("") ? "00" : txtPhotoTypeStart.Text.Trim();
        param.Add("photoStart", photoStart);

        // PhotoType 訖
        String photoEnd = txtPhotoTypeEnd.Text.Trim().Equals("") ? "00" : txtPhotoTypeEnd.Text.Trim();
        param.Add("photoEnd", photoEnd);

        // 製卡廠
        String factory = ddlFactory.SelectedValue.Equals("0") ? "00" : ddlFactory.SelectedValue;
        param.Add("factory", factory);

        // Take card flag 起            
        String kindStart = txtKindStart.Text.Trim().Equals("") ? "00" : txtKindStart.Text.Trim();
        param.Add("kindStart", kindStart);

        // Take card flag 訖
        String kindEnd = txtKindEnd.Text.Trim().Equals("") ? "00" : txtKindEnd.Text.Trim();
        param.Add("kindEnd", kindEnd);
        return true;
    }
    /// <summary>
    /// 功能說明:加載狀態選項

    /// 作    者:JUN HU
    /// 創建時間:2010/07/04
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindState()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtState))
        {
            this.ddlFactory.DataSource = dtState;
            this.ddlFactory.DataTextField = "PROPERTY_NAME";
            this.ddlFactory.DataValueField = "PROPERTY_CODE";
            this.ddlFactory.DataBind();
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"),"0");
            ddlFactory.Items.Insert(0, li);
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        // 初始化報表參數
        Dictionary<String, String> param = new Dictionary<String, String>();
        if (chkCond(ref param))
        {
            try
            {
                string strMsgId = string.Empty;
                string strServerPathFile = Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0519Report(param, ref strServerPathFile, ref strMsgId);

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
                    MessageHelper.ShowMessage(this, strMsgId);
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.BusinessRule);
                MessageHelper.ShowMessage(this, "06_06051900_001");
            }
        }
    }
}
