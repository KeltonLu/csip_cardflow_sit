//******************************************************************
//*  功能說明：手動大宗檔錯誤處理 
//*  作    者：zhen chen
//*  創建日期：2010/07/06
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using Framework.WebControls;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using System.Net;

public partial class P060302000001 : PageBase
{
    #region 全局變量
    DataTable dtFileInfo = null;
    ArrayList arrListCard = new ArrayList();
    ArrayList arrListFile = new ArrayList();
    JobHelper jobHelper = new JobHelper();
    #endregion

    #region 事件

    /// <summary>
    /// 功能說明:頁面加載綁定數據
    /// 作    者:zhen chen
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Page.Title = BaseHelper.GetShowText("06_06030200_000");
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvUserView.Visible = false;
            ShowControlsText();

            radError_CheckedChanged();
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
        this.BindGirdView();
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:zhen chen
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        if (radError.Checked)
        {

            if (!string.IsNullOrEmpty(this.dateFrom.Text) || !string.IsNullOrEmpty(this.dateTo.Text))
            {
                if (!ValidateHelper.IsValidDate(this.dateFrom.Text.Trim(), this.dateTo.Text.Trim(), ref strMsgID))
                {
                    //* 起迄日期驗證不通過
                    //MessageHelper.ShowMessage(UpdatePanel1, "06_06030200_005");

                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_005") + "');");
                    return;
                }
                else
                {
                    BindGirdView();
                }
            } 
            else if (string.IsNullOrEmpty(this.dateFrom.Text) && string.IsNullOrEmpty(this.dateTo.Text))
            {
                strMsgID = "06_06030200_005";
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06030200_007");
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_007") + "');");
                return;
            }
            else
            {
                BindGirdView();
            }
        }
        else
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06030200_013");
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_013") + "');");
        }
    }

    /// <summary>
    /// 功能說明:資料上傳
    /// 作    者:zhen chen
    /// 創建時間:2010/07/06
    /// 修改記錄: 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            string strLocalPath = string.Empty;                             //文件路徑
            string strFileName = string.Empty;                              //文件名稱
            string strFuncName = BaseHelper.GetShowText("06_0606030200_000"); //功能名稱
            string strCardType = string.Empty;                              //卡片類別
            string strFactory = string.Empty;                               //廠商
            string strTimeFlag = string.Empty;                              //標記上午或下午時間
            string strFileType=string.Empty;                                //文檔類別
            string strMsgID = string.Empty;                                 //記錄錯誤ID
            AutoImportFiles getTrandate = new AutoImportFiles();
            
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06030200_000") + "：" + BaseHelper.GetShowText("06_06030200_002");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");
            
            if (fupFile.HasFile)
            {
                if (fupFile.PostedFile.ContentLength <= 0)
                {
                    //MessageHelper.ShowMessage(this.UpdatePanel1, "06_06030200_022");
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_022") + "');");
                    return;
                }
                strLocalPath = this.fupFile.PostedFile.FileName;
                strFileName = this.fupFile.FileName;
                Entity_CardBaseInfo eCardBaseInfo = new Entity_CardBaseInfo();
                //判斷今日是否為工作日,在得到轉當日期
                if (Convert.ToInt32(DateTime.Now.ToString("HH")) <= 12)
                {
                    strTimeFlag = "A";
                    eCardBaseInfo.trandate =getTrandate.GetTrandate(strTimeFlag);
                }
                else
                {
                    strTimeFlag = "P";
                    eCardBaseInfo.trandate =getTrandate.GetTrandate(strTimeFlag);
                }
                //eCardBaseInfo.trandate = DateHelper.Today.ToString("yyyy/MM/dd");  //*獲取轉檔日
                //eCardBaseInfo.Merch_Code = strFileName.Substring(strFileName.IndexOf(".") - 1, 1); //*獲取製卡廠代碼
                eCardBaseInfo.card_file = strFileName;//*卡片名稱
                //strTag = strFileName.Substring(strFileName.IndexOf(".") - 2, 6);
                //*txt檔名格式正確
                if (jobHelper.ValidateTxt(strLocalPath))
                {
                    strFileType = ddlFile.SelectedValue;
                    strCardType=ddlCard.SelectedValue;
                    strFactory = ddlFactory.SelectedValue;

                    switch (strFileType)
                    {
                        //選擇一般大宗檔
                        case "1":
                            //eCardBaseInfo.Merch_Code = strFileName.Substring(strFileName.IndexOf(".") - 1, 1); //*獲取製卡廠代碼
                            eCardBaseInfo.Merch_Code = strFactory;

                            switch (strCardType)
                            {
                                //選擇信用卡
                                case "1":
                                    if (!ImportCommonFiles(strFuncName, strFileName, strCardType, ref eCardBaseInfo,ref strMsgID))
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    else
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    break;
                                //選擇金融卡
                                case "2":
                                    if (!ImportCommonFiles(strFuncName, strFileName, strCardType, ref eCardBaseInfo, ref strMsgID))
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    else
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    break;
                            }
                            break;
                        //選擇缺卡大宗檔
                        case "2":
                            //eCardBaseInfo.Merch_Code = strFileName.Substring(strFileName.IndexOf(".") - 3, 1); //*獲取製卡廠代碼
                            eCardBaseInfo.Merch_Code = strFactory;
                            
                            switch (strCardType)
                            {
                                //選擇信用卡
                                case "1":
                                    if (!ImportLackFiles(strFuncName, strFileName, strCardType, ref eCardBaseInfo,ref strMsgID))
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    else
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    break;
                                //選擇金融卡
                                case "2":
                                    if (!ImportLackFiles(strFuncName, strFileName, strCardType, ref eCardBaseInfo, ref strMsgID))
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    else
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                                    }
                                    break;
                            }
                            break;
                        default:
                            break;
                    } 
                }
                //*txt檔名格式錯誤
                else
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_008") + "');");
                    jobHelper.SaveLog(strFileName + "06_06030200_008");
                }
            }
            //上傳的文件不存在
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_014") + "');");
                jobHelper.SaveLog(strFileName + "06_06030200_014");
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
        }
    }

    /// <summary>
    /// 功能說明:gridview的命令行事件
    ///          執行下載和刪除文件操作
    /// 作    者:zhen chen
    /// 創建時間:2010/07/01
    /// 修改記錄: 2020/10/08 Ares Luke 應SERVER與BATCH需求，調整檔案下載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvUserView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string strMsgID = string.Empty;
        string strFileName = string.Empty;                                             //文件名稱
        int    rowIndex = Convert.ToInt32(e.CommandArgument.ToString());               //取得行索引
        string strErrorID = string.Empty;                                              //錯誤文檔的ID號
        try
        {
            //點擊下載
            if (e.CommandName.Equals("DownLoad"))
            {
                string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                string strLogMsg = BaseHelper.GetShowText("06_06030200_000") + "：" + BaseHelper.GetShowText("06_06030200_014");
                BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");

                //獲取文件名稱
                strFileName = grvUserView.Rows[rowIndex].Cells[1].Text;
                //得到需要刪除那一行的ID號
                strErrorID = grvUserView.DataKeys[rowIndex].Value.ToString();
                //得到路徑
                HiddenField hidFilePath = grvUserView.Rows[rowIndex].Cells[2].FindControl("hidFilePath") as HiddenField;
                String strFilePath = hidFilePath.Value;

                //ServerAP有檔案則下載AP，反之去BatchServer
                if (File.Exists(strFilePath))
                {
                    
                    this.Response.Clear();

                    this.Response.Buffer = true;
                    this.Session.CodePage = 950;
                    this.Response.ContentType = "text/plain";
                    //this.Response.ContentType = "application/download";
                    this.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strFileName, System.Text.Encoding.UTF8));
                    this.Response.TransmitFile(strFilePath);


                    //更新下載狀態
                    BRM_InOutFile.UpdateLoadFlag(ref strErrorID);
                    CustButton btnDelete = this.grvUserView.Rows[rowIndex].Cells[2].FindControl("btnDelete") as CustButton;
                    btnDelete.Enabled = true;

                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else if (!string.IsNullOrWhiteSpace(strFilePath))
                {
                    try
                    {
                        String[] strBatchFilePathArr = new string[]{};
                        String basePath = "";

                        if (strFilePath.Contains("\\"+  UtilHelper.GetAppSettings("FileDownload") + "\\"))
                        {
                            //JOB: FileDownload
                            basePath = "FileDownload";
                            strBatchFilePathArr = strFilePath.Split(new string[] { "\\" + UtilHelper.GetAppSettings("FileDownload") + "\\" }, StringSplitOptions.None);
                        }
                        else if (strFilePath.Contains("\\" + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\"))
                        {
                            //手動上傳: UpLoadFilePath(FileUpload)
                            basePath = "UpLoadFilePath";
                            strBatchFilePathArr =
                                strFilePath.Split(
                                    new string[] {"\\" + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\"},
                                    StringSplitOptions.None);
                        }


                        if (strBatchFilePathArr.Length == 2 && !string.IsNullOrWhiteSpace(basePath))
                        {
                            //結尾路徑
                            String commonPath = strBatchFilePathArr[1].ToString();
                            //本機儲存位子
                            String strLocalFilePath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings(basePath) + "\\" + commonPath;
                            //Batch Api url
                            String batchUrl = UtilHelper.GetAppSettings("BatchUrl") + UtilHelper.GetAppSettings(basePath) + "/" + commonPath.Replace("\\", "/");

                            HttpWebRequest request = null;
                            HttpWebResponse response = null;
                            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(batchUrl);
                            request.Timeout = 300000;

                            response = (System.Net.HttpWebResponse)request.GetResponse();

                            
                            using (Stream stream = response.GetResponseStream())
                            {
                                if (stream == null)
                                {
                                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_014") + "');");
                                    Logging.Log("06_06030200_014", LogLayer.UI);
                                }
                                else
                                {
                                    if (!File.Exists(strLocalFilePath))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(strLocalFilePath) ?? string.Empty);
                                    }
                                    
                                    using (var file = File.OpenWrite(strLocalFilePath))
                                        stream.CopyTo(file);

                                    if (File.Exists(strLocalFilePath))
                                    {
                                        FileInfo fs = new FileInfo(strLocalFilePath);
                                        Session["ServerFile"] = strLocalFilePath;
                                        Session["ClientFile"] = fs.Name;
                                        string urlString = @"location.href='DownLoadFile.aspx';";
                                        jsBuilder.RegScript(this.Page, urlString);

                                        //更新下載狀態
                                        BRM_InOutFile.UpdateLoadFlag(ref strErrorID);
                                        CustButton btnDelete = this.grvUserView.Rows[rowIndex].Cells[2].FindControl("btnDelete") as CustButton;
                                        btnDelete.Enabled = true;
                                    }
                                    else
                                    {
                                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_014") + "');");
                                        Logging.Log("06_06030200_014", LogLayer.UI);
                                    }
                                }
                            }
                        }
                        else
                        {
                            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_014") + "');");
                            Logging.Log("06_06030200_014", LogLayer.UI);
                        }
                    }
                    catch (Exception exp)
                    {
                        string strExp = exp.Message;
                        Logging.Log(strExp, LogLayer.UI);
                        jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_014") + "');");
                    }
                }
                else {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_014") + "');");
                    Logging.Log("06_06030200_014", LogLayer.UI);
                }

            }
            //點擊刪除
            if (e.CommandName.Equals("Del"))
            {
                string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                string strLogMsg = BaseHelper.GetShowText("06_06030200_000") + "：" + BaseHelper.GetShowText("06_06030200_015");
                BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "D");


                //得到需要刪除那一行的ID號
                strErrorID=grvUserView.DataKeys[rowIndex].Value.ToString();
                //刪除成功
                if (BRM_InOutFile.DeleteDownLoadFile(ref strErrorID))
                {
                    MessageHelper.ShowMessage(this.UpdatePanel1, "06_06030200_001");
                    BindGirdView();
                    Logging.Log("06_06030200_001", LogLayer.UI);
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                    return;
                }
                //刪除失敗
                else
                {
                    //MessageHelper.ShowMessage(this.UpdatePanel1, "06_06030200_002");
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_002") + "');");
                }
            }
        }
        catch (Exception exp)
        {
            string strExp = exp.Message;
            Logging.Log(strExp, LogLayer.UI);
        }
    }

    /// <summary>
    /// 功能說明:選擇大宗檔上傳
    /// 作    者:zhen chen
    /// 創建時間:2010/07/01
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radUpload_CheckedChanged(object sender, EventArgs e)
    {
        this.fupFile.Enabled = true;
        this.btnUpload.Enabled = true;
        this.btnSearch.Enabled = false;
        this.ddlCard.Enabled = true;
        this.ddlFile.Enabled = true;
        this.dprInDate.Enable = true;
        this.dprTranDate.Enable = true;
        this.ddlFactory.Enabled = true;
    }

    /// <summary>
    /// 功能說明:選擇大宗檔錯誤資料查詢
    /// 作    者:zhen chen
    /// 創建時間:2010/07/01
    /// 修改記錄: 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radError_CheckedChanged(object sender, EventArgs e)
    {
        radError_CheckedChanged();
    }

    /// <summary>
    /// 功能說明:選擇大宗檔錯誤資料查詢-Init與Change
    /// 作    者:Area Luke
    /// 創建時間:2020/11/12
    /// 修改記錄: 
    /// </summary>
    protected void radError_CheckedChanged()
    {
        this.fupFile.Enabled = false;
        this.btnUpload.Enabled = false;
        this.btnSearch.Enabled = true;
        this.ddlCard.Enabled = false;
        this.ddlFile.Enabled = false;
        this.dprInDate.Enable = false;
        this.dprTranDate.Enable = false;
        this.ddlFactory.Enabled = false;
    }
    #endregion

    #region 方法
    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:zhen chen
    /// 創建時間:2010/06/28
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        this.grvUserView.Columns[0].HeaderText = BaseHelper.GetShowText("06_06030200_011");
        this.grvUserView.Columns[1].HeaderText = BaseHelper.GetShowText("06_06030200_012");
        this.grvUserView.Columns[2].HeaderText = BaseHelper.GetShowText("06_06030200_013");
        this.radUpload.Text = BaseHelper.GetShowText("06_06030200_017");
        this.radError.Text = BaseHelper.GetShowText("06_06030200_016");
        //*匯檔日期初始化
        this.dateTo.Text = DateHelper.Today.ToString("yyyy/MM/dd");
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        string strMsgID = string.Empty;
        //初始化DropDownList的數據
        DataTable dtCard = new DataTable(); 
        DataTable dtFile = new DataTable();
        DataTable dtState = new DataTable();


        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "41", ref dtCard))
        {
            this.ddlCard.DataSource = dtCard;
            this.ddlCard.DataTextField = "PROPERTY_NAME";
            this.ddlCard.DataValueField = "PROPERTY_CODE";
            this.ddlCard.DataBind();
        }

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "29", ref dtFile))
        {
            this.ddlFile.DataSource = dtFile;
            this.ddlFile.DataTextField = "PROPERTY_NAME";
            this.ddlFile.DataValueField = "PROPERTY_CODE";
            this.ddlFile.DataBind();
        }

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtState))
        {
            this.ddlFactory.DataSource = dtState;
            this.ddlFactory.DataTextField = "PROPERTY_NAME";
            this.ddlFactory.DataValueField = "PROPERTY_CODE";
            this.ddlFactory.DataBind();
        }
    }

    /// <summary>
    /// 功能說明:綁定數據
    /// 作    者:zhen chen
    /// 創建時間:2010/06/25
    /// 修改記錄:
    /// </summary>
    private void BindGirdView()
    {
        string strMsgID = string.Empty;
        string dateStart = this.dateFrom.Text.Trim();
        string dateEnd = this.dateTo.Text.Trim();
        int iTotalCount = 0;
        HiddenField hid = null;
        CustButton btnDelete = null;
        try
        {
            //查詢不成功
            if (!BRM_InOutFile.SearchInOutFile(ref dtFileInfo, ref dateStart, ref dateEnd, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount))
            {
                this.grvUserView.DataSource = null;
                this.grvUserView.DataBind();
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            //查詢成功
            else
            {
                //綁定數據
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvUserView.Visible = true;
                this.grvUserView.DataSource = dtFileInfo;
                this.grvUserView.DataBind();
                if (dtFileInfo != null && dtFileInfo.Rows.Count > 0)
                {
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06030200_019"));
                    for (int icount = 0; icount < grvUserView.Rows.Count; icount++)
                    {
                        //隱藏LoadFlag字段,判斷文件是否被下載過
                        hid = grvUserView.Rows[icount].Cells[2].FindControl("hidValue") as HiddenField;
                        if (hid.Value == "0")
                        {
                            btnDelete = this.grvUserView.Rows[icount].Cells[2].FindControl("btnDelete") as CustButton;
                            btnDelete.Enabled = false;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_015") + "');");
                }
            }

        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("06_06030200_003"));
            return;
        }
    }

    /// <summary>
    /// 功能說明:一般大宗檔上傳
    /// 作    者:zhen chen
    /// 創建時間:2010/07/06
    /// 修改記錄: 
    /// </summary>
    /// <param name="strPath">文件路徑</param>
    /// <param name="strFuncName">功能名稱</param>
    /// <param name="strFileName">文件名</param>
    /// <param name="strCardType">卡片種類</param>
    /// <param name="eCardBaseInfo"></param>
    bool ImportCommonFiles(string strFuncName, string strFileName, string strCardType, ref Entity_CardBaseInfo eCardBaseInfo,ref string strMsgID)
    {
        try
        {
            int No = 0;                                //*匯入之錯誤編號
            ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
            DataTable dtDetail = null;                 //檢核結果列表
            AutoImportFiles autoImport = new AutoImportFiles();
            JobHelper jobHelper = new JobHelper();
            string strJobId = "0101";                    //功能名
            string strSavePath = string.Empty;         //文檔路徑
            DataRow[] drError = null;
            Entity_JobErrorInfo JobErrInfo = new Entity_JobErrorInfo();

            //上傳文件至服務器
            strSavePath = FileUpload(this.fupFile.PostedFile, strJobId);

            if (string.IsNullOrEmpty(strSavePath))
            {
                strMsgID = "06_06030200_021";
                return false;
            }
            else
            {
                //*檢核資料不成功
                if (!autoImport.UploadCheck(strSavePath, strFuncName, strCardType, ref No, ref arrayErrorMsg, ref dtDetail))
                {
                    if (dtDetail == null)
                    {
                        jobHelper.SaveLog(DateTime.Now.ToString() + " 00_00000000_000");
                        strMsgID = "00_00000000_000";
                        return false;
                    }
                    
                    if (null != dtDetail && dtDetail.Rows.Count > 0)
                    {
                        drError = dtDetail.Select("action is null");
                        if (null != drError && drError.Length > 0)
                        {
                            //匯入檢核錯誤的資料至錯誤資料檔
                            jobHelper.SaveLog(DateTime.Now.ToString() + " 06_06030200_010");
                            for (int iError = 0; iError < drError.Length; iError++)
                            {
                                JobErrInfo.JobID = strJobId;                                    //JObid
                                JobErrInfo.ImportFileName = strFileName;                        //匯入檔名
                                JobErrInfo.ErrorContext = drError[iError]["Context"].ToString();//檢核錯誤的資料
                                JobErrInfo.ImportTime = DateTime.Now.ToString("yyyy/MM/dd");    //匯入時間
                                JobErrInfo.LocalFilePath = strSavePath;                             //檔案路徑
                                JobErrInfo.LoadFlag = "0";                                      //
                                JobErrInfo.Reason = Resources.JobResource.Job0000014;           //檢核失敗的原因
                                BRM_JobErrorInfo.Insert(JobErrInfo, ref strMsgID);
                                dtDetail.Rows.Remove(drError[iError]);
                                jobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003, strFileName));
                            }
                        }

                        jobHelper.SaveLog(DateTime.Now.ToString() + "06_06030200_016");
                        strMsgID = "06_06030200_016";
                        return false;
                    }
                    else 
                    {
                        strMsgID = "06_06030200_024";
                        return false;
                    }
                }
                //檢核資料成功
                else
                {
                    jobHelper.SaveLog(DateTime.Now.ToString() + "06_06030200_009");

                    // 修改JOB 0101全域變數
                    autoImport.BForWebErrInfo(strSavePath);

                    //*匯入檢核正確的資料至卡片基本檔
                    if (autoImport.ImportToDB(dtDetail, strFileName, eCardBaseInfo, strCardType,true,this.dprInDate.Text.Trim(),this.dprTranDate.Text.Trim()))
                    {
                        strMsgID = "06_06030200_023";
                        jobHelper.SaveLog(strFuncName + strFileName + "06_06030200_011");
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06030200_020";
                        jobHelper.SaveLog(strFuncName + strFileName + "06_06030200_020");
                        return false;
                    } 
                }
            }          
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            strMsgID = "00_00000000_000";
            return false;
        }
    }

    /// <summary>
    /// 功能說明:缺卡大宗檔上傳
    /// 作    者:zhen chen
    /// 創建時間:2010/07/06
    /// 修改記錄: 
    /// </summary>
    /// <param name="strFuncName">功能名稱</param>
    /// <param name="strFileName">文件名</param>
    /// <param name="strCardType">卡片種類</param>
    /// <param name="eCardBaseInfo"></param>
    bool ImportLackFiles(string strFuncName, string strFileName, string strCardType, ref Entity_CardBaseInfo eCardBaseInfo,ref string strMsgID)
    {
        try
        {
            int No = 0;                                //*匯入之錯誤編號
            ArrayList arrayErrorMsg = new ArrayList(); //*匯入之錯誤列表信息
            DataTable dtDetail = null;                 //檢核結果列表
            AutoImportLackFiles lackFiles = new AutoImportLackFiles();
            JobHelper jobHelper = new JobHelper();
            string strAMPMFlag = string.Empty;           //標記上午或下午
            string strJobId = "0103";                    //功能名
            string strSavePath = string.Empty;           //錯誤資料的文件路徑
            DataRow[] drError = null;

            Entity_JobErrorInfo JobErrInfo = new Entity_JobErrorInfo();

            //上傳文件至服務器
            strSavePath = FileUpload(this.fupFile.PostedFile, strJobId);
            if (string.IsNullOrEmpty(strSavePath))
            {
                //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_021") + "');");
                strMsgID = "06_06030200_021";
                return false;
            }
            else
            {
                //*檢核資料不成功
                if (!lackFiles.UploadCheck(strSavePath, strFuncName, strCardType, ref No, ref arrayErrorMsg, ref dtDetail))
                {
                    if (dtDetail == null)
                    {
                        jobHelper.SaveLog(DateTime.Now.ToString() + " 00_00000000_000");
                        //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_016") + "');");
                        strMsgID = "00_00000000_000";
                        return false;
                    }

                    drError = dtDetail.Select("action is null");
                    if (null != drError && drError.Length > 0)
                    {
                        //匯入檢核錯誤的資料至錯誤資料檔
                        jobHelper.SaveLog(DateTime.Now.ToString() + " 06_06030200_010");
                        for (int iError = 0; iError < drError.Length; iError++)
                        {
                            JobErrInfo.JobID = strJobId;                                    //JObid
                            JobErrInfo.ImportFileName = strFileName;                        //匯入檔名
                            JobErrInfo.ErrorContext = drError[iError]["Context"].ToString();//檢核錯誤的資料
                            JobErrInfo.ImportTime = DateTime.Now.ToString("yyyy/MM/dd");    //匯入時間
                            JobErrInfo.LocalFilePath = strSavePath;                             //檔案路徑
                            JobErrInfo.LoadFlag = "0";                                      //
                            JobErrInfo.Reason = Resources.JobResource.Job0000014;           //檢核失敗的原因
                            BRM_JobErrorInfo.Insert(JobErrInfo, ref strMsgID);
                            dtDetail.Rows.Remove(drError[iError]);
                            jobHelper.SaveLog(string.Format(Resources.JobResource.Job0101003, strFileName));
                        }
                        strMsgID = "06_06030200_016";
                        jobHelper.SaveLog(DateTime.Now.ToString() + "06_06030200_016");
                        return false;
                    }
                    else
                    {
                        strMsgID = "06_06030200_024";
                        return false;
                    }
                }
                //檢核資料成功
                else
                {
                    jobHelper.SaveLog(DateTime.Now.ToString() + " 06_06030200_009");
                    //上午匯入
                    if (Convert.ToInt32(DateTime.Now.ToString("HH")) <= 12)
                    {
                        //strAMPMFlag = "A";
                        //*匯入檢核正確的資料至卡片基本檔
                        if (lackFiles.ImportToDB(dtDetail, strFileName, eCardBaseInfo, strCardType,true,this.dprInDate.Text.Trim(),this.dprTranDate.Text.Trim()))
                        {
                            jobHelper.SaveLog(strFuncName + strFileName + "06_06030200_023"+" " +"06_06030200_011");
                            strMsgID = "06_06030200_023";
                            return true;
                        }
                        else
                        {
                            jobHelper.SaveLog(strFuncName + strFileName + "06_06030200_020");
                            strMsgID = "06_06030200_020";
                            return false;
                        }
                    }
                    //下午匯入
                    else
                    {
                        //strAMPMFlag = "P";
                        //*匯入檢核正確的資料至卡片基本檔
                        if (lackFiles.ImportToDB(dtDetail, strFileName, eCardBaseInfo, strCardType,true,this.dprInDate.Text.Trim(),this.dprTranDate.Text.Trim()))
                        {
                            jobHelper.SaveLog(strFuncName + strFileName + "06_06030200_023" + " " + "06_06030200_011");
                            strMsgID = "06_06030200_023";
                            return true;
                        }
                        else
                        {
                            jobHelper.SaveLog(strFuncName + strFileName + "06_06030200_020");
                            strMsgID = "06_06030200_020";
                            return false;
                        }
                    }
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.Util);
            strMsgID = "00_00000000_000";
            return false;
        }
    }
    
    /// <summary>
    /// 功能說明:判斷上傳txt文檔內容卡片種類是否為信用卡
    /// 作    者:zhen chen
    /// 創建時間:2010/07/07
    /// 修改記錄: 
    /// </summary>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    //private bool CreditCard(string FilePath)
    //{
    //    StreamReader sr = File.OpenText(FilePath);
    //    if (!sr.ReadLine().Contains("|"))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    /// <summary>
    /// 功能說明:判斷上傳txt文檔內容卡片種類是否為金融卡
    /// 作    者:zhen chen
    /// 創建時間:2010/07/07
    /// 修改記錄: 
    /// </summary>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    //private bool FinanceCard(string FilePath)
    //{
    //    StreamReader sr = File.OpenText(FilePath);
    //    if (sr.ReadLine().Contains("|"))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    /// <summary>
    /// 功能說明:根據設定的轉檔日屬性獲取轉檔日期
    /// 作    者:HaoChen
    /// 創建時間:2010/06/07
    /// 修改記錄:
    /// </summary>
    /// <param name="strMessage"></param>
    //public string GetTrandate(string strTrandate)
    //{
    //    switch (strTrandate)
    //    {
    //        case "A":
    //            return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd");
    //        case "P":
    //            if (BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
    //            {
    //                return DateTime.Now.ToString("yyyyMMdd");
    //            }
    //            else
    //            {
    //                return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString();
    //            }
    //        default:
    //            if (BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
    //            {
    //                return DateTime.Now.ToString("yyyyMMdd");
    //            }
    //            else
    //            {
    //                return DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), 1), "yyyyMMdd", null).ToString();
    //            }
    //    }

    //}

    /// <summary>
    /// 文件上傳功能
    /// </summary>
    /// <param name="hpfUploadFile">客戶端要上傳的文檔</param>
    /// <param name="strMsgID">返回出錯MessageID</param>
    /// <returns>返回值：""----上傳不成功；其他：上傳文檔服務器路徑及文檔名稱</returns>
    protected string FileUpload(HttpPostedFile hpfUploadFile,string strJobId)
    {
        string strServerPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath");

        string strDirPath = strServerPath + "\\" + strJobId;
        // Determine whether the directory exists.	
        if (!Directory.Exists(strDirPath))
        {
            // Create the directory it does not exist.
            Directory.CreateDirectory(strDirPath);
        }

        strServerPath = strDirPath + "\\" + hpfUploadFile.FileName.Substring(hpfUploadFile.FileName.LastIndexOf("\\") + 1);
        // 檢查文檔是否已經存在
        if (File.Exists(strServerPath))
        {
            File.Delete(strServerPath);
        }

        try
        {
            hpfUploadFile.SaveAs(strServerPath);
            return strServerPath; // 上傳成功      
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.Util);
        }
        return "";
    }

    /// <summary>
    /// 功能說明:上傳客戶端文件至服務器
    /// 作    者:zhen chen
    /// 創建時間:2010/07/08
    /// 修改記錄: 
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="strJobId"></param>
    /// <returns></returns>
    //protected string  UploadFile(string strFileName,string strJobId )
    //{
    //    try
    //    {
    //        //*本地存放目錄名(格式為yyyyMMddHHmmss+JobID)
    //        string strFolderName = strJobId + DateTime.Now.ToString("yyyyMMddHHmmss");
    //        //創建文件夾的路徑
    //        string strDirPath = UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\" + strFolderName;
    //        //創建文件夾
    //        Directory.CreateDirectory(strDirPath);
    //        //上傳文件存放至服務器的路徑
    //        string strLocalPath = strDirPath + "\\" + strFileName;
    //        //上傳文件保存至服務器
    //        this.fupFile.SaveAs(strLocalPath);
    //        //返回文件保存至服務器的路徑
    //        return strLocalPath;
    //    }
    //    catch (Exception exp)
    //    {
    //        Logging.SaveLog(ELogLayer.UI, exp);
    //        return null;
    //    }
    //}
    #endregion
}
