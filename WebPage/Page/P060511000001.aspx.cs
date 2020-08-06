//******************************************************************
//*  功能說明：更改寄送方式記錄查詢

//*  作    者：JUN HU
//*  創建日期：2010/06/24
//*  修改記錄：

//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P060511000001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ckUrgency_Flg.Text = BaseHelper.GetShowText("06_06051000_014");
            txtBackdateStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            txtBackdateEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            BindKind();
        }
    }

    /// <summary>
    /// 功能說明:綁定取卡方式
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/04
    /// 修改記錄:
    /// </summary>
    public void BindKind()
    {
        string strMsgID = string.Empty;
        DataTable dtKindName = new DataTable();
        custddlType.Items.Clear();
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKindName))
        {
            foreach (DataRow dr in dtKindName.Rows)
            {
                ListItem liTmp = new ListItem(dr["PROPERTY_CODE"].ToString() + "  " + dr["PROPERTY_NAME"].ToString(), dr["PROPERTY_CODE"].ToString());
                custddlType.Items.Add(liTmp);
            }
            ListItem li = new ListItem(BaseHelper.GetShowText("06_06051900_009"), "");
            custddlType.Items.Insert(0, li);
        }

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
    /// 功能說明:加載報表
    /// 作    者:JUN HU
    /// 創建時間:2010/06/24
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadReport()
    {
        try
        {
            
            string strMsgID = string.Empty;
            Dictionary<string, string> param = new Dictionary<string, string>();
            
            #region 查詢條件參數

            param.Add("indatefrom", txtBackdateStart.Text.Trim().Equals("") ? "NULL" : txtBackdateStart.Text.Trim());
            param.Add("indateto", txtBackdateEnd.Text.Trim().Equals("") ? "NULL" : txtBackdateEnd.Text.Trim());
          
            if (custddlType.SelectedItem.Value.Trim().Equals(""))
            {
                param.Add("newway", "00");
                param.Add("Enditem", "00");
            }
            else
            {
                param.Add("newway", custddlType.SelectedValue);

                switch (custddlType.SelectedValue.ToString().Trim())
                {
                    case "0":
                        param.Add("Enditem", "1");
                        break;
                    case "1":
                        param.Add("Enditem", "0");
                        break;
                    case "3":
                        param.Add("Enditem", "2");
                        break;
                    case "4":
                        param.Add("Enditem", "3");
                        break;
                    case "10":
                        param.Add("Enditem", "6");
                        break;
                    case "11":
                        param.Add("Enditem", "5");
                        break;
                    default:
                        param.Add("Enditem", "7");
                        break;
                }
            }
            
            param.Add("factory", ddlFactory.SelectedItem.Value.Trim().Equals("0") ? "00" : ddlFactory.SelectedValue);
            param.Add("UrgencyFlg", ckUrgency_Flg.Checked ? "1" : "NULL");
            
            #endregion
            
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath"));

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0511Report(param, ref strServerPathFile, ref strMsgID);

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
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050400_003");
        }
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:JUN HU
    /// 創建時間:2010/06/21
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (txtBackdateStart.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            MessageHelper.ShowMessage(this, "06_06051200_000");
            return;
        }
        if (txtBackdateEnd.Text.Trim().Equals(""))
        {
            //MessageHelper.ShowMessage(UpdatePanel1, "06_06051200_000");
            MessageHelper.ShowMessage(this, "06_06051200_000");
            return;
        }
        LoadReport();
    }
}
