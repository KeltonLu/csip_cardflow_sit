using System;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.Collections.Generic;
using System.IO;
using System.Data;

public partial class P060207000002 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            try
            {
                ShowControlsText();
                this.gpList.Visible = false;
                this.gpList.RecordCount = 0;
                this.grvUserView.Visible = false;

                switch (Request.QueryString["Type"].ToString())
                {
                    case "Search": { Search(); }break;
                    case "Print": { Print(); } break;
                }
            }
            catch (Exception exp)
            {
                MessageHelper.ShowMessage(this, "06_06020701_000" + exp.Message);
            }
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020701_003");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020701_004");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020701_005");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06020701_006");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
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
                Boolean result = BR_Excel_File.GetDataTable0207(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020701_007"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06020701_008"));
                }
            }
            catch (Exception exp)
            {
                MessageHelper.ShowMessage(this, "06_06020701_000" + exp.Message);
            }
        }
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 功能說明:回上頁
    /// 作    者:linda
    /// 創建時間:2010/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string strRadCheckFlg0207 = Session["RadCheckFlg0207"].ToString().Trim();
        string strStockDate0207 = Session["StockDate0207"].ToString().Trim();
        Response.Redirect("P060207000001.aspx?RadCheckFlg0207=" + RedirectHelper.GetEncryptParam(strRadCheckFlg0207) + " &StockDate0207=" + RedirectHelper.GetEncryptParam(strStockDate0207) + "");
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        String strDailyCloseDate = string.Empty;

        if (Request.QueryString["DailyCloseDate"] != null)
        {
            strDailyCloseDate = RedirectHelper.GetDecryptString(Request.QueryString["DailyCloseDate"]);
        }
        else
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06020701_001") + "');history.go(-1)");
            return false;
        }

        // 初始化報表參數
        param = new Dictionary<String, String>();
        param.Add("DailyCloseDate", strDailyCloseDate);
        return true;
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢顯示
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected void Search()
    {
        BindGridView();
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢列印
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected void Print()
    {
        Dictionary<String, String> param = new Dictionary<String, String>();
        if (chkCond(ref param))
        {
            try
            {
                string strMsgId = string.Empty;
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

                //產生報表
                bool result = BR_Excel_File.CreateExcelFile_0207Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, "06_06020701_000" + exp.Message);
            }
        }
    }
}
