//******************************************************************
//*  功能說明：址更重寄異動記錄查詢
//*  作    者：Linda
//*  創建日期：2010/09/27
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using System.Data;

public partial class P060520000001 : PageBase
{
    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo; //*記錄登陸Session訊息
    private structPageInfo sPageInfo; //*記錄網頁訊息

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtUpdStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            txtUpdEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
        }

        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
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
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06052000_006");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06052000_007");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06052000_008");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06052000_009");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06052000_010");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06052000_011");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06052000_012");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06052000_013");
        this.grvUserView.Columns[8].HeaderText = BaseHelper.GetShowText("06_06052000_014");
        this.grvUserView.Columns[9].HeaderText = BaseHelper.GetShowText("06_06052000_015");
        this.grvUserView.Columns[10].HeaderText = BaseHelper.GetShowText("06_06052000_016");
        this.grvUserView.Columns[11].HeaderText = BaseHelper.GetShowText("06_06052000_017");

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
                Boolean result = BR_Excel_File.GetDataTable0520(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06052000_005"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06052000_006"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06052000_006"));
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
    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Linda
    /// 創建時間:2010/09/27
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
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        if (txtUpdStart.Text.Trim().Equals("") || txtUpdEnd.Text.Trim().Equals(""))
            return false;
        if (!this.txtMailNo.Text.Trim().Equals(""))
        {
            if (this.txtMailNo.Text.Trim().Length < 20)
                return false;
        }
        if (!this.txtId.Text.Trim().Equals(""))
        {
            if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
                return false;
        }
        // 異動日期起
        param.Add("strUpdFrom", txtUpdStart.Text.Trim().Equals("") ? "NULL" : txtUpdStart.Text.Trim());

        // 異動日期迄
        param.Add("strUpdTo", txtUpdEnd.Text.Trim().Equals("") ? "NULL" : txtUpdEnd.Text.Trim());

        // 掛號號碼 
        param.Add("strMailNo", txtMailNo.Text.Trim().Equals("") ? "NULL" : txtMailNo.Text.Trim());

        // 身分證字號
        param.Add("strId", txtId.Text.Trim().Equals("") ? "NULL" : txtId.Text.Trim());
        return true;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/08/17
    /// </summary>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log =
            BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtId.Text;
        BRL_AP_LOG.Add(log);
        // 初始化報表參數
        Dictionary<String, String> param = new Dictionary<String, String>();
        if (chkCond(ref param))
        {
            try
            {
                string strMsgId = string.Empty;
                string strServerPathFile = Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0520Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, "06_06052000_003");
            }
        }
    }
}