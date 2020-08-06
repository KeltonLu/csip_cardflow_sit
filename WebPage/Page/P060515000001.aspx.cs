//******************************************************************
//*  功能說明：無法製卡檔查詢
//*  作    者：HAO CHEN
//*  創建日期：2010/07/05
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P060515000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //*加載狀態選項
            BindFactory();
            //* 設定Tittle
            this.Page.Title = BaseHelper.GetShowText("06_06051500_000");
        }
    }

    /// <summary>
    /// 功能說明:加載製卡廠選項
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/05
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public void BindFactory()
    {
        DataTable dtState = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtState))
        {
            this.ddlFactory.DataSource = dtState;
            this.ddlFactory.DataTextField = "PROPERTY_NAME";
            this.ddlFactory.DataValueField = "PROPERTY_CODE";
            this.ddlFactory.DataBind();
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "0");
            ddlFactory.Items.Insert(0, li);
        }
    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:HAO CHEN
    /// 創建時間:2010/07/05
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        string strMStart = dpMakeStime.Text.Trim();
        string strMEnd = dpMakeEtime.Text.Trim();

        string strPStart = dpPostStime.Text.Trim();
        string strPEnd = dpPostEtime.Text.Trim();

        if (rdbMake.Checked)
        {
            if (string.IsNullOrEmpty(strMStart) && string.IsNullOrEmpty(strMEnd))
            {
                strMsgID = "06_05150000_002";
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(strMStart) && !string.IsNullOrEmpty(strMEnd))
                {
                    strMsgID = "06_05150000_004";
                    return false;
                }
                if (!string.IsNullOrEmpty(strMStart) && string.IsNullOrEmpty(strMEnd))
                {
                    strMsgID = "06_05150000_005";
                    return false;
                }
                if (!string.IsNullOrEmpty(strMStart) && !string.IsNullOrEmpty(strMEnd))
                {
                    //*製卡日期開始時間不能大于結束時間
                    if (!ValidateHelper.IsValidDate(strMStart, strMEnd, ref strMsgID))
                    {
                        strMsgID = "06_05150000_000";
                        return false;
                    }
                }
            }
        }

        if (rdbPost.Checked)
        {
            if (string.IsNullOrEmpty(strPStart) && string.IsNullOrEmpty(strPEnd))
            {
                strMsgID = "06_05150000_008";
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(strPStart) && !string.IsNullOrEmpty(strPEnd))
                {
                    strMsgID = "06_05150000_006";
                    return false;
                }
                if (!string.IsNullOrEmpty(strPStart) && string.IsNullOrEmpty(strPEnd))
                {
                    strMsgID = "06_05150000_007";
                    return false;
                }
                if (!string.IsNullOrEmpty(strPStart) && !string.IsNullOrEmpty(strPEnd))
                {
                    //*郵寄日期開始時間不能大于結束時間
                    if (!ValidateHelper.IsValidDate(strPStart, strPEnd, ref strMsgID))
                    {
                        strMsgID = "06_05150000_001";
                        return false;
                    }
                }
            }
        }
        return true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgId = string.Empty;
        if (!CheckCondition(ref strMsgId))
        {
            jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsgId) + "')");
            return;
        }

        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        
        try
        {
            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();
            
            //*製卡日期
            if (rdbMake.Checked)
            {
                param.Add("MstatrDate", this.dpMakeStime.Text.Trim());
                param.Add("MendDate", this.dpMakeEtime.Text.Trim());
                param.Add("CountS", this.dpMakeStime.Text.Trim());
                param.Add("CountE", this.dpMakeEtime.Text.Trim());
            }
            else
            {
                param.Add("MstatrDate", "NULL");
                param.Add("MendDate", "NULL");
            }

            //*郵寄日期
            if (rdbPost.Checked)
            {
                param.Add("PstatrDate", this.dpPostStime.Text.Trim());
                param.Add("PendDate", this.dpPostEtime.Text.Trim());
                param.Add("CountS", this.dpPostStime.Text.Trim());
                param.Add("CountE", this.dpPostEtime.Text.Trim());
            }
            else
            {
                param.Add("PstatrDate", "NULL");
                param.Add("PendDate", "NULL");
            }

            if (ddlFactory.SelectedValue.Equals("0"))
            {
                param.Add("Factory", "00");
                param.Add("FactoryName", BaseHelper.GetShowText("06_06051900_009"));
            }
            else
            {
                param.Add("Factory", this.ddlFactory.SelectedValue);
                param.Add("FactoryName", this.ddlFactory.SelectedItem.Text.Trim());
            }

            Boolean result = BR_Excel_File.CreateExcelFile_0515Report(param, ref strServerPathFile, ref strMsgId);
            
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
            MessageHelper.ShowMessage(this, "06_05150000_003");

        }
    }
}
