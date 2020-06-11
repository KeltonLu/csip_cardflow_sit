//******************************************************************
//*  功能說明：註銷作業報表
//*  作    者：Simba Liu
//*  創建日期：2010/06/21
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;


public partial class P060506000001 : PageBase
{


    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/21
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string strMsgID = string.Empty;
            Dictionary<string, string> param = new Dictionary<string, string>();
            
            #region 查詢條件參數

            // 處理日期起日
            String processDateStart =
                this.txtBackdateStart.Text.Trim().Equals("") ? "NULL" : txtBackdateStart.Text.Trim();
            param.Add("processDateStart",processDateStart);
            // 處理日期訖日
            String processDateEnd =
                this.txtBackdateEnd.Text.Trim().Equals("") ? "NULL" : txtBackdateEnd.Text.Trim();
            param.Add("processDateEnd",processDateEnd);
            // 狀態
            String status =
                dropStatus.SelectedValue.Equals("XX") ? "NULL" : dropStatus.SelectedValue;
            param.Add("status", status);
            // BLOCK CODE
            String blockCode = dropBlockCode.SelectedValue;
            param.Add("blockCode",blockCode);
            
            // MEMO
            String memo = dropMEMO.SelectedValue.Trim();
            if (this.dropMEMO.SelectedValue.Trim() == "其他")
            {
                if (txtMemo.Text.Trim().Equals(""))
                {
                    memo = " ";
                }
                else
                {
                    memo =this.txtMemo.Text.Trim();
                }
            }
            param.Add("memo",memo);
            
            #endregion
            
            string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0506Report(param, ref strServerPathFile, ref strMsgID);

            if (result)
            {
                FileInfo fs = new FileInfo(strServerPathFile);
                Session["ServerFile"] = strServerPathFile;
                Session["ClientFile"] = fs.Name;
                string urlString = @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
            else {
                MessageHelper.ShowMessage(this, strMsgID);
            }

            
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050400_003");

        }
    }
}
