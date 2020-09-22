//******************************************************************
//*  功能說明：分行郵寄資料查詢
//*  作    者：JUN HU
//*  創建日期：2010/06/30
//*  修改記錄：20200623: Luke,調整EXCEL報表
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.IO;

public partial class P060509000001 : PageBase
{
    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢加載事件
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
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
    /// 修改時間:2020/09/03
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06050900_006");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06050900_007");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06050900_008");
        this.grvUserView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06050900_009");

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
                Boolean result = BR_Excel_File.GetDataTable0509(param, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050900_006"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050900_007"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06050900_007"));
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
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增共用條件檢核需求功能
    /// 作    者:Ares JaJa
    /// 修改時間:2020/09/03
    /// </summary>
    protected Boolean chkCond(ref Dictionary<String, String> param)
    {
        string strMsgId = string.Empty;

        if (txtMaildateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return false;
        }

        if (txtMaildateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return false;
        }

        if (txtIndateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return false;
        }

        if (txtIndateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return false;
        }

        if (txtCode.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_002");
            return false;
        }

        try
        {
            #region 查詢條件參數

            param = new Dictionary<String, String>();

            // 郵寄日期
            param.Add("maildatefrom", txtMaildateStart.Text.Trim());
            param.Add("maildateto", txtMaildateEnd.Text.Trim());

            // 製卡日期
            param.Add("indatefrom", txtIndateStart.Text.Trim());
            param.Add("indateto", txtIndateEnd.Text.Trim());

            // 分行代碼
            param.Add("branchid", txtCode.Text.Trim());

            #endregion
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050900_004");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/01
    /// 修改記錄:
    /// 20200624: Luke, 調整為Excel報表
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
    /// 修改時間:2020/09/03
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
                bool result = BR_Excel_File.CreateExcelFile_0509Report(param, ref strServerPathFile, ref strMsgId);

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
                MessageHelper.ShowMessage(this, "06_06050900_004");
            }
        }
    }

    /// <summary>
    /// 功能說明:根據分行代碼帶出分行名稱
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        //*分行代碼
        DataTable dtCode = new DataTable();

        //*分行代碼帶出分行名稱
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "9", ref dtCode))
        {
            DataRow[] rowCode = dtCode.Select("PROPERTY_CODE='" + this.txtCode.Text.Trim() + "'");
            if (rowCode != null && rowCode.Length > 0)
            {
                this.txtName.Text = rowCode[0]["PROPERTY_NAME"].ToString();
            }
            else
            {
                this.txtName.Text = string.Empty;
            }
        }
    }
}