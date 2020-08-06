//******************************************************************
//*  功能說明：緊急製卡檔案匯入報表
//*  作    者：Linda
//*  創建日期：2010/12/31
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.Collections.Generic;
using System.IO;
using CSIPCommonModel.EntityLayer;
using Framework.Common.JavaScript;

public partial class P060522000001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.txtStartDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.txtEndDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            LoadDDl();
            gvpbP02Record.Visible = false;

        }
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize =  int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.gvpbP02Record.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    /// <summary>
    /// 載入下拉選單
    /// </summary>
    private void LoadDDl()
    {
        this.ddlResult.Items.Clear();
        ListItem A1 = new ListItem();
        A1.Text = "全部";
        A1.Value = "";
        ListItem A2 = new ListItem();
        A2.Text = "成功";
        A2.Value = "成功";
        ListItem A3 = new ListItem();
        A3.Text = "失敗";
        A3.Value = "失敗";

        ddlResult.Items.Add(A1);
        ddlResult.Items.Add(A2);
        ddlResult.Items.Add(A3);
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Linda
    /// 創建時間:2010/12/31
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (this.txtStartDate.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051900_000");
            txtStartDate.Focus();
            return;
        }
        if (txtEndDate.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051900_000");
            txtEndDate.Focus();
            return;
        }
        LoadReport();
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));
        if (!Directory.Exists(strServerPathFile))
        {
            Directory.CreateDirectory(strServerPathFile);
        }
        string msgID = "";
        List<BatchImport_UrgencyCard> RData = GetData(true);
        //將筆數訊息以:分隔帶入
        msgID = lblTotalCount.Text + ":" + lblSuccCount.Text + ":" + lblFailCount.Text;
        bool result = BRBatchImport_UrgencyCard.CreateExcelFile(RData, eAgentInfo.agent_name, ref strServerPathFile, ref msgID);
        if (result)
        {  
            //  strServerPathFile = @"D:\share\ExcelFile_AutoPayStatus_20190409094654.xls";
            FileInfo fs = new FileInfo(strServerPathFile);
            Session["ServerFile"] = strServerPathFile;
            //Session["ClientFile"] = fs.Name;
            Session["ClientFile"] = "EMCCardReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + msgID + ".xls";
            string urlString = @"location.href='DownLoadFile.aspx';";
            jsBuilder.RegScript(this.Page, urlString);
        }


    }
    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:Linda
    /// 創建時間:2010/12/31
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        try
        {
            ///有輸入身分證或卡號，就要寫SOC
            if (!string.IsNullOrEmpty(this.txtid.Text) || !string.IsNullOrEmpty(this.txtCardno.Text))
            {
                //------------------------------------------------------
                //AuditLog to SOC
                CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
                log.Customer_Id = this.txtid.Text;
                log.Account_Nbr = this.txtCardno.Text;
                BRL_AP_LOG.Add(log);
                //------------------------------------------------------                             
            }


            BindGridView();
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
        }
    }
    /// 功能說明:分頁事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
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
    /// 功能說明:綁定GridView
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
       
        try
        {
            List<BatchImport_UrgencyCard> dataColl = GetData(false);
           
            if (dataColl.Count > 0)
            {
                this.gpList.Visible = true;
              
                this.gvpbP02Record.Visible = true;
                this.gvpbP02Record.DataSource = dataColl;
                this.gvpbP02Record.DataBind();
            }
            else
            {
                //result.Text = "查無資料";
                this.gvpbP02Record.Visible = false;
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_00000000_007"));
            return;
        }
    }
    /// <summary>
    /// 取得頁面查詢資料
    /// </summary>
    /// <returns></returns>
    private List<BatchImport_UrgencyCard> GetData(bool isPrint)
    {
        
        BatchImport_UrgencyCard queryObj = new BatchImport_UrgencyCard();
        queryObj.indate1 = txtStartDate.Text.Trim();
        queryObj.indate2 = txtEndDate.Text.Trim();
        queryObj.result = ddlResult.SelectedValue;
        queryObj.id = txtid.Text.Trim();
        queryObj.cardno = txtCardno.Text.Trim();
        List<BatchImport_UrgencyCard> result = BRBatchImport_UrgencyCard.getReportColl(queryObj);

        int iTotalCount = result.Count;
        int iSucess = 0;
        int iFail = 0;
        //計算後分頁資料
        List<BatchImport_UrgencyCard> pageData = new List<BatchImport_UrgencyCard>();
        int curPage = gpList.CurrentPageIndex;

        int rowStart = ((curPage - 1) * gpList.PageSize)  ;
        int roeEnd = rowStart +  gpList.PageSize;
        int cnt = 0;
        foreach (BatchImport_UrgencyCard oitem in result)
        {
            if (cnt >= rowStart && cnt <= roeEnd) {
                pageData.Add(oitem);
            }
            if (oitem.result == "成功")
            {
                iSucess++;
            }
            else
            {
                iFail++;
            }
            cnt++;
        }
        lblTotalCount.Text = iTotalCount.ToString();
        lblSuccCount.Text = iSucess.ToString();
        lblFailCount.Text = iFail.ToString();
        this.gpList.RecordCount = iTotalCount;
        if (isPrint) {
            return result;
        }
        else
        {
            return pageData;
        }
    }
}
