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
        }

        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO) this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo) this.Session["PageInfo"]; //*記錄網頁訊息
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
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log =
            BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtId.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        if (txtUpdStart.Text.Trim().Equals("") || txtUpdEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06052000_000");
            return;
        }

        if (!this.txtMailNo.Text.Trim().Equals(""))
        {
            if (this.txtMailNo.Text.Trim().Length < 20)
            {
                //*掛號號碼小於二十碼
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06052000_001");
                return;
            }
        }

        if (!this.txtId.Text.Trim().Equals(""))
        {
            if (ValidateHelper.IsChinese(this.txtId.Text.Trim()))
            {
                //*身份證字號驗證不通過
                //MessageHelper.ShowMessage(UpdatePanel1, "06_06052000_002");
                return;
            }
        }

        LoadReport();
    }

    /// <summary>
    /// 功能說明:加載報表
    /// 作    者:Linda
    /// 創建時間:2010/09/27
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        string strMsgId = string.Empty;

        try
        {
            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            // 異動日期起
            param.Add("strUpdFrom", txtUpdStart.Text.Trim().Equals("") ? "NULL" : txtUpdStart.Text.Trim());

            // 異動日期迄
            param.Add("strUpdTo", txtUpdEnd.Text.Trim().Equals("") ? "NULL" : txtUpdEnd.Text.Trim());

            // 掛號號碼 
            param.Add("strMailNo", txtMailNo.Text.Trim().Equals("") ? "NULL" : txtMailNo.Text.Trim());

            // 身分證字號
            param.Add("strId", txtId.Text.Trim().Equals("") ? "NULL" : txtId.Text.Trim());

            string strServerPathFile =
                this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

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