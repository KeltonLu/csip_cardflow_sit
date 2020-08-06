//******************************************************************
//*  功能說明：郵件交寄狀況檢核

//*  作    者：JUN HU
//*  創建日期：2010/06/28
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using BusinessRules;
using EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.WebControls;
using Framework.Data.OM;
using Framework.Common.Utility;

public partial class P060512000001 : PageBase
{
    #region 成員

    public DataTable m_dtCardBaseInfo
    {
        get { return ViewState["m_dtCardBaseInfo"] as DataTable; }
        set { ViewState["m_dtCardBaseInfo"] = value; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            grvCardView.Visible = false;
            ShowControlsText();
            gpList.Visible = false;
            gpList.RecordCount = 0;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (this.txtBackdateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            return;
        }

        if (txtBackdateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            return;
        }

        BindGridView();
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者：JUN HU
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題

        this.grvCardView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06051200_002");
        this.grvCardView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06051200_004");
        this.grvCardView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06051200_005");
        this.grvCardView.Columns[3].HeaderText = BaseHelper.GetShowText("06_06051200_006");

        //* 設置一頁顯示最大筆數

        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvCardView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:綁定GridView
    /// 作    者:JUN HU
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int iTotalCount = 0;
        DataTable dtCardBaseInfo = new DataTable();
        try
        {
            //* 查詢不成功

            if (!BRM_Report.SearchSendStatus(txtBackdateStart.Text.Trim(), txtBackdateEnd.Text.Trim(),
                ref dtCardBaseInfo, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                this.gpList.RecordCount = 0;
                this.grvCardView.DataSource = null;
                this.grvCardView.DataBind();
                this.gpList.Visible = false;
                this.grvCardView.Visible = false;
                //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            //* 查詢成功
            else
            {
                //MergeTable(ref dtCardBaseInfo);
                m_dtCardBaseInfo = dtCardBaseInfo;
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvCardView.Visible = true;
                this.grvCardView.DataSource = dtCardBaseInfo;
                this.grvCardView.DataBind();
                ViewState["indatefrom"] = txtBackdateStart.Text.Trim();
                ViewState["indateto"] = txtBackdateEnd.Text.Trim();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_000"));
            return;
        }
    }

    protected void grvCardView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton custlk = e.Row.Cells[0].FindControl("lbcardfile") as CustLinkButton;
            CustLabel custlb = e.Row.Cells[0].FindControl("custlbCardfile") as CustLabel;
            if (int.Parse(e.Row.Cells[3].Text) > 0)
            {
                custlk.Visible = true;
                custlb.Visible = false;
            }
            else
            {
                custlk.Visible = false;
                custlb.Visible = true;
            }

            custlk.CommandArgument = e.Row.RowIndex.ToString();
        }
    }

    protected void lbcardfile_Click(object sender, EventArgs e)
    {
        try
        {
            CustLinkButton lkbtn = (CustLinkButton) sender;
            int rowIndex = int.Parse(lkbtn.CommandArgument);
            string strCardfile = m_dtCardBaseInfo.Rows[rowIndex]["card_file"].ToString();
            ViewState["strCardfile"] = strCardfile;
            BindDetailView();
            ModalPopupExtenderM.Show();
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
        }
    }

    /// <summary>
    /// 功能說明:GetFilterCondition
    /// 作    者:JUN HU
    /// 創建時間:2010/06/23
    /// 修改記錄:
    /// </summary>
    /// <returns></returns>
    private string GetFilterCondition()
    {
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_kind, Operator.NotIn, DataTypeUtils.String,
            "'1','2','9','10','11'");

        if (this.txtBackdateStart.Text.Trim() != "" && this.txtBackdateEnd.Text.Trim() != "")
        {
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.LessThanEqual, DataTypeUtils.String,
                this.txtBackdateEnd.Text.Trim());
            sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_indate1, Operator.GreaterThanEqual, DataTypeUtils.String,
                this.txtBackdateStart.Text.Trim());
        }

        sqlhelp.AddCondition("a." + Entity_CardBaseInfo.M_card_file, Operator.Equal, DataTypeUtils.String,
            ViewState["strCardfile"].ToString());

        return sqlhelp.GetFilterCondition();
    }

    private void BindDetailView()
    {
        try
        {
            string strMsgId = string.Empty;
            Dictionary<string, string> param = new Dictionary<string, string>();
            
            #region 查詢條件參數

            param.Add("indatestart", this.txtBackdateStart.Text.Trim().Equals("") ? "NULL" : this.txtBackdateStart.Text.Trim());
            param.Add("indateend", this.txtBackdateEnd.Text.Trim().Equals("") ? "NULL" : this.txtBackdateEnd.Text.Trim());
            param.Add("strCardfile", ViewState["strCardfile"].ToString().Equals("") ? "NULL" : ViewState["strCardfile"].ToString());

            #endregion
            
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0512_2Report(param, ref strServerPathFile, ref strMsgId);

            if (result)
            {
                FileInfo fs = new FileInfo(strServerPathFile);
                Session["ServerFile"] = strServerPathFile;
                Session["ClientFile"] = fs.Name;
                string urlString = @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
            else {
                MessageHelper.ShowMessage(this, strMsgId);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_001"));
            return;
        }
        //string strMsgID = "";
        //int iTotalCount = 0;
        //DataTable dtCardBaseInfo = new DataTable();
        //string strCardfile = ViewState["strCardfile"].ToString();
        //try
        //{
        //    //* 查詢不成功

        //    if (!BRM_Report.SearchSendDetail(GetFilterCondition(), ref dtCardBaseInfo, this.gpDetail.CurrentPageIndex, this.gpDetail.PageSize, ref iTotalCount, ref strMsgID))
        //    {
        //        this.gpDetail.RecordCount = 0;
        //        this.grvDetail.DataSource = null;
        //        this.grvDetail.DataBind();
        //        this.gpDetail.Visible = false;
        //        this.grvDetail.Visible = false;
        //        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
        //        return;
        //    }
        //    //* 查詢成功
        //    else
        //    {
        //        this.gpDetail.Visible = true;
        //        this.gpDetail.RecordCount = iTotalCount;
        //        this.grvDetail.Visible = true;
        //        this.grvDetail.DataSource = dtCardBaseInfo;
        //        this.grvDetail.DataBind();
        //    }
        //}
        //catch(Exception exp)
        //{
        //    Logging.SaveLog(ELogLayer.UI, exp);
        //    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06051000_000"));
        //    return;
        //}
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (this.txtBackdateStart.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            return;
        }

        if (txtBackdateEnd.Text.Trim().Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            return;
        }

        try
        {
            string strMsgId = string.Empty;
            Dictionary<string, string> param = new Dictionary<string, string>();

            #region 查詢條件參數

            param.Add("indatefrom", txtBackdateStart.Text.Trim().Equals("") ? "NULL" : txtBackdateStart.Text.Trim());
            param.Add("indateto", txtBackdateEnd.Text.Trim().Equals("") ? "NULL" : txtBackdateEnd.Text.Trim());

            #endregion

            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0512_1Report(param, ref strServerPathFile, ref strMsgId);

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
            Logging.Log(exp, LogLayer.UI);
            return;
        }
    }
}