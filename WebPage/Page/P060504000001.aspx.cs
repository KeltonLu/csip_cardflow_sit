//******************************************************************
//*  功能說明：郵局退件資料查詢
//*  作    者：Simba Liu
//*  創建日期：2010/06/17
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.IO;
using CSIPCommonModel.EntityLayer;
using System.Data;

public partial class P060504000001 : PageBase
{
    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

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
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
        if (!IsPostBack)
        {
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
    /// 修改時間:2020/09/08
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06050400_003");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06050400_008");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06050400_009");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06050400_001");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06050400_010");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06050400_005");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06050400_002");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06050400_011");
        this.grvUserView.Columns[8].HeaderText = BaseHelper.GetShowText("06_06050400_012");
        this.grvUserView.Columns[9].HeaderText = BaseHelper.GetShowText("06_06050400_013");
        this.grvUserView.Columns[10].HeaderText = BaseHelper.GetShowText("06_06050400_014");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
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
                Boolean result = BR_Excel_File.GetDataTable0504(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050400_005"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050400_006"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050400_006"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtid.Text;
        log.Account_Nbr = this.txtCardno.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        String strMsgID = String.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            MessageHelper.ShowMessage(this, strMsgID);
            return false;
        }

        try
        {
            #region 查詢條件參數
            param = new Dictionary<String, String>();

            // 退件日期起日
            String backDateStart = txtBackdateStart.Text.Trim();
            if (txtBackdateStart.Text.Trim().Equals(""))
                backDateStart = "NULL";
            param.Add("backDateStart", backDateStart);

            // 退件日期訖日
            String backDateEnd = txtBackdateEnd.Text.Trim();
            if (txtBackdateEnd.Text.Trim().Equals(""))
                backDateEnd = "NULL";
            param.Add("backDateEnd", backDateEnd);

            // 結案日期起日
            String closeDateStart = txtClosedateStart.Text.Trim();
            if (txtClosedateStart.Text.Trim().Equals(""))
                closeDateStart = "NULL";
            param.Add("closeDateStart", closeDateStart);

            // 結案日期訖日
            String closeDateEnd = txtClosedateEnd.Text.Trim();
            if (txtClosedateEnd.Text.Trim().Equals(""))
                closeDateEnd = "NULL";
            param.Add("closeDateEnd", closeDateEnd);

            // 退件流水編號
            String serialNo = txtserial_no.Text.Trim();
            if (txtserial_no.Text.Trim().Equals(""))
                serialNo = "NULL";
            param.Add("serial_no", serialNo);

            // 身份證字號 
            String id = txtid.Text.Trim();
            if (txtid.Text.Trim().Equals(""))
                id = "NULL";
            param.Add("id", id);

            // 卡號
            String cardNo = txtCardno.Text.Trim();
            if (txtCardno.Text.Trim().Equals(""))
                cardNo = "NULL";
            param.Add("cardNo", cardNo);

            #endregion
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050400_003");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/08
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
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0504Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, "06_06050400_003");
            }
        }
    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        //*退件流水編號
        if (ValidateHelper.IsChinese(this.txtserial_no.Text.Trim()))
        {
            strMsgID = "06_06050400_000";
            txtserial_no.Focus();
            return false;
        }
        //*身份證字號
        if (ValidateHelper.IsChinese(this.txtid.Text.Trim()))
        {
            strMsgID = "06_06050400_001";
            txtid.Focus();
            return false;
        }
        //*卡號
        if (ValidateHelper.IsChinese(this.txtCardno.Text.Trim()))
        {
            strMsgID = "06_06050400_002";
            txtCardno.Focus();
            return false;
        }


        return true;
    }
}
