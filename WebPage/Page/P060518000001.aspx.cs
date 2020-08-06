//******************************************************************
//*  功能說明：退件連絡報表
//*  作    者：JUN HU
//*  創建日期：2010/07/02
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Utility;

public partial class P060518000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindState();
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/07/02
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgId = string.Empty;

        if (txtMaildateStart.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051800_000");
            txtMaildateStart.Focus();
            return;
        }

        if (txtMaildateEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051800_000");
            txtMaildateEnd.Focus();
            return;
        }

        if (!ValidateHelper.IsValidDate(txtMaildateStart.Text.Trim(), DateTime.Now.ToString("yyyy/MM/dd"),
            ref strMsgId) || !ValidateHelper.IsValidDate(txtMaildateEnd.Text.Trim(),
            DateTime.Now.ToString("yyyy/MM/dd"), ref strMsgId))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051800_002");
            return;
        }

        try
        {
            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            // 查詢日期起日
            param.Add("datefrom", txtMaildateStart.Text.Trim());

            // 查詢日期迄日
            param.Add("dateto", txtMaildateEnd.Text.Trim());

            // 卡別
            param.Add("backtype", ddlState.SelectedValue);


            string strServerPathFile =
                this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0518Report(param, ref strServerPathFile, ref strMsgId);

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
            MessageHelper.ShowMessage(this, "06_06051800_001");
        }
    }

    /// <summary>
    /// 功能說明:加載狀態選項
    /// 作    者:JUN HU
    /// 創建時間:2010/07/02
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindState()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "28", ref dtState))
        {
            this.ddlState.DataSource = dtState;
            this.ddlState.DataTextField = "PROPERTY_NAME";
            this.ddlState.DataValueField = "PROPERTY_CODE";
            this.ddlState.DataBind();
        }
    }
}