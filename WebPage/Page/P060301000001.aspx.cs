//******************************************************************
//*  功能說明：手動異動檔處理 
//*  作    者：zhen chen
//*  創建日期：2010/06/25
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Quartz;
using Quartz.Impl;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Cryptography;
using Framework.Common.IO;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using Framework.WebControls;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using System.Text;


public partial class P060301000001 : PageBase
{

    #region 事件
    /// <summary>
    /// 功能說明:頁面加載
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Page.Title = BaseHelper.GetShowText("06_06030100_000");
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
            ShowControlsText();
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        this.btnUpload.Enabled = true;

        if (!string.IsNullOrEmpty(this.dateFrom.Text) || !string.IsNullOrEmpty(this.dateTo.Text))
        {

            if (!ValidateHelper.IsValidDate(this.dateFrom.Text.Trim(), this.dateTo.Text.Trim(), ref strMsgID))
            {
                //* 起迄日期驗證不通過
                strMsgID = "06_06030100_004";
                //MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
                jsBuilder.RegScript(this.Page,"alert('"+MessageHelper.GetMessage(strMsgID) +"');");
                this.btnUpload.Enabled = false;
                return;
            }
            else
            {
                BindGridView();
            }
        }
        else if (string.IsNullOrEmpty(this.dateFrom.Text) && string.IsNullOrEmpty(this.dateTo.Text))
        {
            strMsgID = "06_06030100_005";
            //MessageHelper.ShowMessage(UpdatePanel1, strMsgID);
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
            this.btnUpload.Enabled = false;
            return;
        }
        else
        {
            BindGridView();
        }
    }

    /// <summary>
    /// 功能說明:上傳文件
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06030100_000") + "：" + BaseHelper.GetShowText("06_06020101_066");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

        CustCheckBox chBox = null;
        for (int icount = 0; icount < grvUserView.Rows.Count; icount++)
        {
            chBox = grvUserView.Rows[icount].Cells[2].FindControl("chkFile") as CustCheckBox;
            if (chBox.Checked)
            {
                UploadZipFile();
            }
            else if (icount == grvUserView.Rows.Count - 1)
            {
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06030100_002");
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030100_002") + "');");
            }
            else
            {
                continue;
            }
        }
    }

    /// <summary>
    /// 功能說明:編輯邏輯
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            ViewState["FlgEdit"] = "TRUE";
            CustLinkButton link = grvUserView.Rows[e.NewEditIndex].Cells[1].FindControl("lkbDetail") as CustLinkButton;
            HiddenField hid = grvUserView.Rows[e.NewEditIndex].Cells[1].FindControl("hidValue") as HiddenField;
            string fileName = link.Text.Trim();
            string strPath = hid.Value;
            if (!File.Exists(strPath))
            {
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06030100_006");
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030100_006") + "');");
                return;
            }
            else
            {
                FileStream fsr = new FileStream(strPath, FileMode.Open);
                if (fsr.Length > 0)
                {
                    StreamReader sr = new StreamReader(fsr, Encoding.Default);
                    this.txtInfo.Text = sr.ReadToEnd();
                    fsr.Close();
                    sr.Close();
                }
                this.ModalPopupExtenderN.Show();

                //System.Diagnostics.Process.Start(strPath);
            }
           
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
        }
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:zhen chen
    /// 創建時間:2010/07/02
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }
    #endregion


    #region 方法
    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06030100_004");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06030100_005");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06030100_006");
        //*匯檔日期初始化
        this.dateTo.Text = DateHelper.Today.ToString("yyyy/MM/dd");
        this.btnCancelN.Text = BaseHelper.GetShowText("06_06030100_007");
         //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
        this.grvUserView.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());
    }

    

    
    /// <summary>
    /// 功能說明:綁定數據
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        string strDateFrom = dateFrom.Text.Trim();
        string strDateTo = dateTo.Text.Trim();
        string strMsgID =string.Empty;
        int iTotalCount = 0;
        DataTable dtFileInfo = null;
        try 
        {
            //查詢不成功
            if (!BRM_CardDataChange.SearchFileInfo(ref dtFileInfo, ref strDateFrom, ref strDateTo,this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount))
            {
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06030100_003"));
                return;
            }
            //查詢成功
            if (dtFileInfo.Rows.Count > 0 && dtFileInfo != null)
            {
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtFileInfo;
                this.grvUserView.DataBind();
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06030100_007"));
            }
            else
            {
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtFileInfo;
                this.grvUserView.DataBind();
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06030100_008"));
                return;
            }
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06030100_003"));
            return;
        }
    }

    

    /// <summary>
    /// 功能說明:上傳壓縮后的zip檔到FTP
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    protected void UploadZipFile()
    {
        try
        {
            JobHelper jobHelper = new JobHelper();
            DataTable dtFileInfo = new DataTable();
            CustCheckBox ckBox = null;
            string strJobId = "0104";//job的功能號
            string strFtpPath = string.Empty;//FTP路徑
            string strUploadFilePath = string.Empty;//本地需要txt檔的路徑
            string strZipFile = string.Empty;//本地需要zip檔的路徑
            string strFtpFileName = string.Empty;//FTP檔名
            FTPFactory objFtp = null;
            string strFtpIp = string.Empty;//登陸FTP地址
            string strFtpUserName = string.Empty;//登陸FTP用戶名
            string strFtpPwd = string.Empty;//登陸FTP密碼
            int intcheck=0;
            //實例化數組               
            string[] strUploadFile = new string[1];
            if (jobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                for (int icount = 0; icount < grvUserView.Rows.Count; icount++)
                {
                    ckBox = grvUserView.Rows[icount].FindControl("chkFile") as CustCheckBox;
                    if (ckBox.Checked)
                    {
                        intcheck = intcheck + 1;
                        //綁定LinkButton
                        CustLinkButton custlk = grvUserView.Rows[icount].Cells[1].FindControl("lkbDetail") as CustLinkButton;
                        HiddenField hid = grvUserView.Rows[icount].Cells[1].FindControl("hidValue") as HiddenField;

                        if (!File.Exists(hid.Value))
                        {
                            //MessageHelper.ShowMessageWithParms(this.UpdatePanel1, "06_06030100_009", custlk.Text);
                            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030100_006", custlk.Text) + "');");
                            return;
                        }

                        strFtpPath = string.Empty;
                        strFtpIp = string.Empty;
                        strFtpUserName = string.Empty;
                        strFtpPwd = string.Empty;

                        //把需要壓縮的文件的路徑添加到數組中
                        strUploadFile[0] = hid.Value;
                        //把txt檔的路徑轉為zip檔的路徑
                        strZipFile = strUploadFile[0].Replace(strUploadFile[0].Substring(strUploadFile[0].Length - 3, 3), "ZIP");

                        //獲取壓縮檔的文件名
                        strFtpFileName = custlk.Text.Replace(custlk.Text.Substring(custlk.Text.Length - 3, 3), "ZIP");



                        if (dtFileInfo.Rows.Count > 0)
                        {
                            DataRow[] drFileInfo = dtFileInfo.Select("MerchCode='" + strUploadFile[0].Substring(strUploadFile[0].Length - 5, 1) + "'");

                            strFtpIp = drFileInfo[0]["FtpIP"].ToString();
                            strFtpUserName = drFileInfo[0]["FtpUserName"].ToString();
                            strFtpPwd = drFileInfo[0]["FtpPwd"].ToString();
                            strFtpPath = drFileInfo[0]["FtpPath"].ToString();
                       }
                        //壓縮需要上傳到FTP的文件
                       JobHelper.Zip(strZipFile, strUploadFile, "", RedirectHelper.GetDecryptString(dtFileInfo.Rows[0]["ZipPwd"].ToString()), CompressToZip.CompressLevel.Level6);
                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");
                        //上傳文件到FTP
                        if (objFtp.Upload(strFtpPath, strFtpFileName, strZipFile))
                        {
                            //上傳檔案成功
                            Logging.Log("06_06030100_000", LogLayer.UI);
                            //MessageHelper.ShowMessage(UpdatePanel1, "06_06030100_000");
                            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030100_000") + "');");
                        }
                        else
                        {
                            //上傳檔案失敗
                            //MessageHelper.ShowMessage(UpdatePanel1, "06_06030100_001");
                            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030100_001") + "');");
                            Logging.Log("06_06030100_001", LogLayer.UI);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            if (intcheck==0)
            {
                //提示請選擇要上傳的檔案
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06030100_002");
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030100_002") + "');");
                Logging.Log("06_06030100_002", LogLayer.UI);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
        }
    }

    #endregion
}
