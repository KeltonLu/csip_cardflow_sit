//******************************************************************
//*  功能說明：無法製卡檔查詢
//*  作    者：HAO CHEN
//*  創建日期：2010/07/05
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P060515000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //*加載狀態選項
            BindFactory();
            //* 設定Tittle
            this.Page.Title = BaseHelper.GetShowText("06_06051500_000");
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
    /// 修改時間:2020/09/17
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051500_008");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051500_009");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051500_010");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051500_011");
        this.grvUserView.Columns[4].HeaderText = BaseHelper.GetShowText("06_06051500_012");
        this.grvUserView.Columns[5].HeaderText = BaseHelper.GetShowText("06_06051500_013");
        this.grvUserView.Columns[6].HeaderText = BaseHelper.GetShowText("06_06051500_014");
        this.grvUserView.Columns[7].HeaderText = BaseHelper.GetShowText("06_06051500_015");
        this.grvUserView.Columns[8].HeaderText = BaseHelper.GetShowText("06_06051500_016");
        this.grvUserView.Columns[9].HeaderText = BaseHelper.GetShowText("06_06051500_017");
        this.grvUserView.Columns[10].HeaderText = BaseHelper.GetShowText("06_06051500_018");
        this.grvUserView.Columns[11].HeaderText = BaseHelper.GetShowText("06_06051500_019");
        this.grvUserView.Columns[12].HeaderText = BaseHelper.GetShowText("06_06051500_020");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
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
                Boolean result = BR_Excel_File.GetDataTable0515(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051500_001"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051500_002"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051500_002"));
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
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
    /// 修改時間:2020/09/17
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        string strMsgId = string.Empty;
        if (!CheckCondition(ref strMsgId))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgId) + "')");
            return false;
        }

        try
        {
            // 初始化報表參數
            param = new Dictionary<String, String>();

            //*製卡日期
            if (rdbMake.Checked)
            {
                param.Add("MstatrDate", this.dpMakeStime.Text.Trim());
                param.Add("MendDate", this.dpMakeEtime.Text.Trim());
                param.Add("CountS", this.dpMakeStime.Text.Trim());
                param.Add("CountE", this.dpMakeEtime.Text.Trim());
            }
            else
            {
                param.Add("MstatrDate", "NULL");
                param.Add("MendDate", "NULL");
            }

            //*郵寄日期
            if (rdbPost.Checked)
            {
                param.Add("PstatrDate", this.dpPostStime.Text.Trim());
                param.Add("PendDate", this.dpPostEtime.Text.Trim());
                param.Add("CountS", this.dpPostStime.Text.Trim());
                param.Add("CountE", this.dpPostEtime.Text.Trim());
            }
            else
            {
                param.Add("PstatrDate", "NULL");
                param.Add("PendDate", "NULL");
            }

            if (ddlFactory.SelectedValue.Equals("0"))
            {
                param.Add("Factory", "00");
                param.Add("FactoryName", BaseHelper.GetShowText("06_06051900_009"));
            }
            else
            {
                param.Add("Factory", this.ddlFactory.SelectedValue);
                param.Add("FactoryName", this.ddlFactory.SelectedItem.Text.Trim());
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_05150000_003");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 功能說明:加載製卡廠選項
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/05
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindFactory()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtState))
        {
            this.ddlFactory.DataSource = dtState;
            this.ddlFactory.DataTextField = "PROPERTY_NAME";
            this.ddlFactory.DataValueField = "PROPERTY_CODE";
            this.ddlFactory.DataBind();
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "0");
            ddlFactory.Items.Insert(0, li);
        }
    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/05
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        string strMStart = dpMakeStime.Text.Trim();
        string strMEnd = dpMakeEtime.Text.Trim();

        string strPStart = dpPostStime.Text.Trim();
        string strPEnd = dpPostEtime.Text.Trim();

        if (rdbMake.Checked)
        {
            if (string.IsNullOrEmpty(strMStart) && string.IsNullOrEmpty(strMEnd))
            {
                strMsgID = "06_05150000_002";
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(strMStart) && !string.IsNullOrEmpty(strMEnd))
                {
                    strMsgID = "06_05150000_004";
                    return false;
                }
                if (!string.IsNullOrEmpty(strMStart) && string.IsNullOrEmpty(strMEnd))
                {
                    strMsgID = "06_05150000_005";
                    return false;
                }
                if (!string.IsNullOrEmpty(strMStart) && !string.IsNullOrEmpty(strMEnd))
                {
                    //*製卡日期開始時間不能大于結束時間
                    if (!ValidateHelper.IsValidDate(strMStart, strMEnd, ref strMsgID))
                    {
                        strMsgID = "06_05150000_000";
                        return false;
                    }
                }
            }
        }

        if (rdbPost.Checked)
        {
            if (string.IsNullOrEmpty(strPStart) && string.IsNullOrEmpty(strPEnd))
            {
                strMsgID = "06_05150000_008";
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(strPStart) && !string.IsNullOrEmpty(strPEnd))
                {
                    strMsgID = "06_05150000_006";
                    return false;
                }
                if (!string.IsNullOrEmpty(strPStart) && string.IsNullOrEmpty(strPEnd))
                {
                    strMsgID = "06_05150000_007";
                    return false;
                }
                if (!string.IsNullOrEmpty(strPStart) && !string.IsNullOrEmpty(strPEnd))
                {
                    //*郵寄日期開始時間不能大于結束時間
                    if (!ValidateHelper.IsValidDate(strPStart, strPEnd, ref strMsgID))
                    {
                        strMsgID = "06_05150000_001";
                        return false;
                    }
                }
            }
        }
        return true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增列印需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/17
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
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

                //產生報表
                Boolean result = BR_Excel_File.CreateExcelFile_0515Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, "06_05150000_003");
            }
        }
    }
}
