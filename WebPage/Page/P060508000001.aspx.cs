//******************************************************************
//*  功能說明：郵局寄送資料查詢
//*  作    者：Simba Liu
//*  創建日期：2010/06/25
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using CSIPCommonModel.EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;

public partial class P060508000001 : PageBase
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
    /// 修改時間:2020/08/14
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06050800_001");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06050800_007");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06050800_002");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06050800_003");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06050800_004");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06050800_005");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
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
                Boolean result = BR_Excel_File.GetDataTable0508(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050800_001"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050800_002"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050800_002"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        //*身份證字號
        if (ValidateHelper.IsChinese(this.txtID.Text.Trim()))
        {
            strMsgID = "06_06050800_004";
            txtID.Focus();
            return false;
        }
        //*掛號號碼
        if (ValidateHelper.IsChinese(this.txtMailno.Text.Trim()))
        {
            strMsgID = "06_06050800_005";
            txtMailno.Focus();
            return false;
        }
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtID.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string strMsgId = string.Empty;

        if (!CheckCondition(ref strMsgId))
        {
            MessageHelper.ShowMessage(this, strMsgId);
            return false;
        }
        try
        {
            // 身份證字號
            String id = this.txtID.Text.Trim().Equals("") ? "NULL" : this.txtID.Text.Trim();
            param.Add("ID", id);

            // 匯入日期起日
            String impDateStart = this.txtProcessDateStart.Text.Trim().Equals("") ? "NULL" : this.txtProcessDateStart.Text.Trim();
            param.Add("Imp_DateStart", impDateStart);

            // 匯入日期訖日
            String impDateEnd = this.txtProcessDateEnd.Text.Trim().Equals("") ? "NULL" : this.txtProcessDateEnd.Text.Trim();
            param.Add("Imp_DateEnd", impDateEnd);


            // 交寄日期起日
            String maildateStart = this.txtMaildateStart.Text.Trim().Equals("") ? "NULL" : this.txtMaildateStart.Text.Trim();
            param.Add("MaildateStart", maildateStart);

            // 交寄日期訖日
            String maildateEnd = this.txtMaildateEnd.Text.Trim().Equals("") ? "NULL" : this.txtMaildateEnd.Text.Trim();
            param.Add("MaildateEnd", maildateEnd);

            // 掛號號碼
            String mailNo = this.txtMailno.Text.Trim().Equals("") ? "NULL" : this.txtMailno.Text.Trim();
            param.Add("Mailno", mailNo);

            // 狀態
            String nonSendCode = this.dropStatus.SelectedValue.Equals("XX") ? "NULL" : this.dropStatus.SelectedValue;
            param.Add("Non_Send_Code", nonSendCode);
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            MessageHelper.ShowMessage(this, "06_06050800_003");
            return false;
        }
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/14
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

                // 產生報表
                bool result = BR_Excel_File.CreateExcelFile_0508Report(param, ref strServerPathFile, ref strMsgId);

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
                Logging.Log(exp);
                MessageHelper.ShowMessage(this, "06_06050800_003");
            }
        }
    }
}
