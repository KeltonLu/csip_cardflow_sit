//******************************************************************
//*  功能說明：手動異動檔處理 
//*  作    者：zhen chen
//*  創建日期：2010/06/25
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.IO;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using Framework.WebControls;
using BusinessRules;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

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
    /// 修改記錄:2020/10/15 Area Luke 因AP分離(WEB與BATCH)需求，調整業務功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            ViewState["FlgEdit"] = "TRUE";
            LinkButton lb = e.CommandSource as LinkButton;
            GridViewRow row = lb.NamingContainer as GridViewRow;
            CustLinkButton link = row.Cells[1].FindControl("lkbDetail") as CustLinkButton;
            HiddenField hid = row.Cells[1].FindControl("hidValue") as HiddenField;
            string fileName = link.Text.Trim();
            string strDBfilePath = hid.Value;
            String strLocalFilePath = "";

            if (!File.Exists(strDBfilePath))
            {
                CheckExistAndDownload(strDBfilePath, ref strLocalFilePath);
            }
            else
            {
                strLocalFilePath = strDBfilePath;
            }

            if (!File.Exists(strLocalFilePath))
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030100_006") + "');");
                Logging.Log("06_06030100_006", LogLayer.UI);
            }

            FileStream fsr = new FileStream(strLocalFilePath, FileMode.Open);
            if (fsr.Length > 0)
            {
                StreamReader sr = new StreamReader(fsr, Encoding.Default);
                this.txtInfo.Text = sr.ReadToEnd();
                fsr.Close();
                sr.Close();
                this.ModalPopupExtenderN.Show();
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
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
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
    /// 修改記錄:2020/10/15 Area Luke 因AP分離(WEB與BATCH)需求，調整業務功能
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

                        string strDBfilePath = hid.Value;
                        String strLocalFilePath = "";

                        if (!File.Exists(strDBfilePath))
                        {
                            CheckExistAndDownload(strDBfilePath, ref strLocalFilePath);
                        }
                        else
                        {
                            strLocalFilePath = strDBfilePath;
                        }

                        if (!File.Exists(strLocalFilePath))
                        {
                            //MessageHelper.ShowMessageWithParms(this.UpdatePanel1, "06_06030100_009", custlk.Text);
                            jsBuilder.RegScript(this.Page,
                                "alert('" + MessageHelper.GetMessage("06_06030100_006", custlk.Text) + "');");
                            return;
                        }

                        strFtpPath = string.Empty;
                        strFtpIp = string.Empty;
                        strFtpUserName = string.Empty;
                        strFtpPwd = string.Empty;

                        //把需要壓縮的文件的路徑添加到數組中
                        strUploadFile[0] = strLocalFilePath;
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


    /// <summary>
    /// 功能說明:檢查本地與BATCH是否有檔案，若本地無資料則去batchAP取得至本地。
    /// </summary>
    /// 作    者:Area Luke
    /// 創建時間:2020/10/15
    /// 修改記錄:
    /// <param name="strDBfilePath"></param>
    /// <param name="strLocalFilePath"></param>

    public void CheckExistAndDownload(String strDBfilePath, ref String strLocalFilePath)
    {
        //去檢查BATCH AP是否有檔案
        String[] strBatchFilePathArr = new string[] { };
        String basePath = "";

        try
        {
            if (strDBfilePath.Contains("\\" + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\"))
            {
                basePath = "UpLoadFilePath";
                strBatchFilePathArr =
                    strDBfilePath.Split(
                        new string[] {"\\" + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\"},
                        StringSplitOptions.None);
            }

            if (strBatchFilePathArr.Length == 2 && !string.IsNullOrWhiteSpace(basePath))
            {
                //結尾路徑
                String commonPath = strBatchFilePathArr[1].ToString();
                //本機儲存位子
                strLocalFilePath = AppDomain.CurrentDomain.BaseDirectory +
                                          UtilHelper.GetAppSettings(basePath) + "\\" + commonPath;
                //Batch Api url
                String batchUrl = UtilHelper.GetAppSettings("BatchUrl") + UtilHelper.GetAppSettings(basePath) +
                                  "/" + commonPath.Replace("\\", "/");

                HttpWebRequest request = null;
                HttpWebResponse response = null;
                request = (System.Net.HttpWebRequest) System.Net.WebRequest.Create(batchUrl);
                request.Timeout = 300000;

                response = (System.Net.HttpWebResponse) request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        jsBuilder.RegScript(this.Page,
                            "alert('" + MessageHelper.GetMessage("06_06030100_006") + "');");
                        Logging.Log("06_06030100_006", LogLayer.UI);
                    }
                    else
                    {
                        if (!File.Exists(strLocalFilePath))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(strLocalFilePath) ?? string.Empty);
                        }

                        using (var file = File.OpenWrite(strLocalFilePath))
                            stream.CopyTo(file);

                    }
                }
            }
        }
        catch (Exception exp)
        {
            string strExp = exp.Message;
            Logging.Log(strExp, LogLayer.UI);
        }
    }
}
