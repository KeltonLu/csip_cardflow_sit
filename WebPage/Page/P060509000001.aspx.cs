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
using System.Configuration;
using System.IO;


public partial class P060509000001 : PageBase
{
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
        string strMsgId = string.Empty;

        if (txtMaildateStart.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return;
        }

        if (txtMaildateEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_000");
            return;
        }

        if (txtCode.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06050900_002");
            return;
        }

        try
        {
            #region 查詢條件參數

            Dictionary<string, string> param = new Dictionary<string, string>();

            // 郵寄日期
            param.Add("maildatefrom", txtMaildateStart.Text.Trim());
            param.Add("maildateto", txtMaildateEnd.Text.Trim());

            // 分行代碼
            param.Add("branchid", txtCode.Text.Trim());

            #endregion

            string strServerPathFile =
                this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());

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