using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.Logging;

/// <summary>
/// Summary description for BaseHelper
/// </summary>
public sealed class BaseHelper
{
    #region GetScript
    /// <summary>
    /// 蚚誧Session隍囮眳綴泐蛌善HomePage珜醱褐掛
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static string GetScriptForUserSessionOut(Page page)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("alert('")
        .Append(MessageHelper.GetMessage("0040"))
        .Append("');")
        .Append("window.location.href = '")
        .Append(page.ResolveUrl("~/Default.aspx")).Append("';");
        return sbScript.ToString();
    }

    /// <summary>
    /// 壽敕赻撩甜芃陔虜珜醱
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static string GetScriptForUserSessionOut_CloseMe(Page page)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("if(window.opener != null && window.opener != undefined)")
        .Append("window.opener.location.reload();")
        .Append("window.close();");
        return sbScript.ToString();
    }

    public static string GetScriptForCloseMeAndGotoURL(Page page, string URL)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("if(window.opener != null && window.opener != undefined)")
        .Append("window.opener.location.replace('" + URL + "');")
        .Append("window.close();");
        return sbScript.ToString();
    }
    #endregion

    #region Set Control
    /// <summary>
    /// 扢离Cancel偌聽枑尨
    /// </summary>
    /// <param name="btnCancel"></param>
    public static void SetCancelBtn(Framework.WebControls.CustButton btnCancel)
    {
        btnCancel.ConfirmMsg = MessageHelper.GetMessage("0028");
    }


    /// <summary>
    /// 獲取控件顯示值
    /// </summary>
    /// <param name="ShowID"></param>
    public static string GetShowText(string ShowID)
    {
        return WebHelper.GetShowText(ShowID);
    }

    /// <summary>
    /// 顯示端末信息
    /// </summary>
    /// <param name="ShowID"></param>
    public static string ClientMsgShow(string strMsgID)
    {
        //return "ClientMsgShow('" + MessageHelper.GetMessage(strMsgID) + "');";
        return "window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage(strMsgID) + "' }, '*');";
    }

    /// <summary>
    /// 顯示主機信息
    /// </summary>
    /// <param name="strMsgID"></param>
    public static string HostMsgShow(string strMsgID)
    {
        //return "HostMsgShow('" + MessageHelper.GetMessage(strMsgID) + "');";
        return "window.parent.postMessage({ func: 'HostMsgShow', data: '" + MessageHelper.GetMessage(strMsgID) + "' }, '*');";
    }
    #endregion



    #region 匯入文檔格式檢核

    /// <summary>
    /// 記錄錯誤行數輸出
    /// </summary>
    /// <param name="intTemp">行索引</param>
    /// <param name="blnIsNote"> 是否已記錄</param>
    /// <param name="arrlErrorRow">錯誤行數數組</param>
    public static void AddErrorMsg(int intTemp, int intColumn, string strMsgID, ref ArrayList arrListMsg)
    {
        arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000010 + Resources.JobResource.Job0000009 + Convert.ToString(intColumn + 1) +Resources.JobResource.ResourceManager.GetString(strMsgID));
    }

    /// <summary>
    /// 根據字符串的字節長度截取字符串
    /// </summary>
    /// <param name="strReadLine">字符串對象</param>
    /// <param name="begin">開始位置</param>
    /// <param name="length">截取長度</param>
    /// <param name="nextBegin">字符串對象</param>
    /// <returns>int</returns>
    public static string GetSubstringByByte(string strReadLine, int begin, int length, out int nextBegin)
    {

        string strTemp1 = getStrLenB(strReadLine, begin, length);
        nextBegin = begin + length;
        return strTemp1;
    }

    //中英文混合字符串截取指定长度,startidx从0开始
    public static string getStrLenB(string str, int startidx, int len)
    {
        int Lengthb = getLengthb(str);
        if (startidx + 1 > Lengthb)
        {
            return "";
        }
        int j = 0;
        int l = 0;
        int strw = 0;//字符的宽度
        bool b = false;
        string rstr = "";
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (j >= startidx)
            {
                rstr = rstr + c;
                b = true;
            }
            if (IsChinese(c))
            {
                strw = 2;
            }
            else
            {
                strw = 1;
            }
            j = j + strw;
            if (b)
            {
                l = l + strw;
                if ((l + 1) > len) break;
            }
        }
        return rstr;
    }

    //字符是否为雙字節
    public static bool IsChinese(char c)
    {
        int iCharLength = System.Text.Encoding.Default.GetByteCount(c.ToString());
        if (iCharLength > 1)
        {
            return true;
        }
        return false;
    }

    //获得字节长度
    public static int getLengthb(string str)
    {
        return System.Text.Encoding.Default.GetByteCount(str);
    }

    /// <summary>
    /// 根據默認的編碼取得字符串的字節長度
    /// </summary>
    /// <param name="text">字符串對象</param>
    /// <returns>int</returns>
    public static int GetByteLength(string text)
    {
        return System.Text.Encoding.Default.GetBytes(text).Length;
    }


    /// <summary>
    /// 記錄匯入日志
    /// </summary>
    /// <param name="eLUpload">匯入日志</param>
    /// <param name="eLUploadDetail">匯入錯誤日志</param>
    /// <param name="strMsgID">錯誤ID</param>
    /// <returns>int</returns>
    public static void LogUpload(EntityL_UPLOAD eLUpload, EntityL_UPLOAD_DETAIL eLUploadDetail, string strMsgID)
    {
        eLUploadDetail.FAIL_REASON =Resources.JobResource.ResourceManager.GetString(strMsgID);
        BRL_UPLOAD.Add(eLUpload, eLUploadDetail, ref strMsgID);

    }

    /// <summary>
    /// 記錄匯入日志
    /// </summary>
    /// <param name="eLUploadDetail">匯入錯誤日志</param>
    /// <param name="intRow">錯誤行號</param>
    /// <param name="strMsg">錯誤信息</param>
    /// <returns>int</returns>
    public static void LogUpload(EntityL_UPLOAD_DETAIL eLUploadDetail, int intRow, string strMsg)
    {
        eLUploadDetail.FAIL_REC_NO = intRow.ToString();
        eLUploadDetail.FAIL_REASON = strMsg;

        BRL_UPLOAD_DETAIL.Add(eLUploadDetail, ref  strMsg);

    }

    /// <summary>
    /// 匯入檢核
    /// </summary>
    /// <param name="strUserID"> 用戶ID</param>
    /// <param name="strFunctionKey">系統權限</param>
    /// <param name="strUploadID"> 匯入作業編號</param>
    /// <param name="dtmThisDate"> 匯入作業時間</param>
    /// <param name="strUploadName"> 匯入作業名稱</param>
    /// <param name="strFilePath">上傳文件地址</param>
    /// <param name="intMax">最大筆數</param>
    /// <param name="arrListMsg">檢核回傳信息</param>
    /// <param name="strMsgID">錯誤信息ID</param>
    /// <param name="dtblBegin">頭筆數數據</param>
    /// <param name="dtblEnd">尾筆數數據</param>
    /// <returns>DataTable</returns>
    public static DataTable UploadCheck(string strUserID, string strFunctionKey, string strUploadID, DateTime dtmThisDate, string strUploadName, string strFilePath, int intMax, ArrayList arrListMsg, ref string strMsgID, DataTable dtblBegin, DataTable dtblEnd)
    {
        EntityL_UPLOAD eLUpload = new EntityL_UPLOAD();

        //* 匯入日志欄位賦值
        eLUpload.CHANGED_USER = strUserID;
        eLUpload.FUNCTION_KEY = strFunctionKey;
        eLUpload.UPLOAD_ID = strUploadID;
        eLUpload.UPLOAD_NAME = strUploadName;
        eLUpload.UPLOAD_DATE = dtmThisDate;
        eLUpload.UPLOAD_STATUS = "Y";
        eLUpload.FILE_NAME = "";

        EntityL_UPLOAD_DETAIL eLUploadDetail = new EntityL_UPLOAD_DETAIL();

        //* 匯入失敗日志欄位賦值
        eLUploadDetail.FUNCTION_KEY = strFunctionKey;
        eLUploadDetail.UPLOAD_ID = strUploadID;
        eLUploadDetail.UPLOAD_DATE = dtmThisDate;
        eLUploadDetail.FAIL_REC_NO = "";



        DataTable dtblUpload = new DataTable();

        //#region  檔案名稱檢核

        //if (Regex.Match(strFilePath, "[\u4E00-\u9FA5]+").Length > 0 )
        //{
        //    strMsgID = "Job0000011";

        //    LogUpload(eLUpload, eLUploadDetail, strMsgID);

        //    return dtblUpload;
        //}
        //#endregion

        #region  檔案類型檢核

        FileInfo file = new FileInfo(strFilePath);

        eLUpload.FILE_NAME = file.Name;

        DataTable dtblUploadCheck = null;

        //* 判斷檔案是否存在
        if (!file.Exists)
        {
            strMsgID = Resources.JobResource.Job0000012;
            eLUpload.UPLOAD_STATUS = "N";
            LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }
        else
        {
            try
            {
                dtblUploadCheck = BRM_UPLOAD_CHECK.Search(strFunctionKey, strUploadID);
            }
            catch
            {
                strMsgID = Resources.JobResource.Job0000013;
                eLUpload.UPLOAD_STATUS = "N";
                LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }

            //* 判斷該匯入檢核有無類型判斷數據
            if (dtblUploadCheck.Rows.Count > 0)
            {
                //* 判斷檔案類型
                if (file.Extension.ToUpper() != dtblUploadCheck.Rows[0]["EXTEND_NAME"].ToString())
                {
                    strMsgID = Resources.JobResource.Job0000014;
                    eLUpload.UPLOAD_STATUS = "N";
                    LogUpload(eLUpload, eLUploadDetail, strMsgID);

                    return dtblUpload;
                }
            }
            else
            {
                strMsgID = Resources.JobResource.Job0000015;
                eLUpload.UPLOAD_STATUS = "N";
                LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }
        }
        #endregion

        int intBeginCount = int.Parse(dtblUploadCheck.Rows[0]["BEGIN_COUNT"].ToString());
        int intEndCount = int.Parse(dtblUploadCheck.Rows[0]["END_COUNT"].ToString());

        int intBeginColumn = int.Parse(dtblUploadCheck.Rows[0]["BEGIN_COLUMN"].ToString());
        int intEndColumn = int.Parse(dtblUploadCheck.Rows[0]["END_COLUMN"].ToString());

        #region  資料庫欄位類型定義檢核

        DataTable dtblUploadType = null;

        try
        {
            dtblUploadType = BRM_UPLOAD_TYPE.Search(strFunctionKey, strUploadID);
        }
        catch
        {
            strMsgID = Resources.JobResource.Job0000013;
            eLUpload.UPLOAD_STATUS = "N";
            LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }


        if (dtblUploadType.Rows.Count > 0)
        {
            //* 生成輸出表的欄位
            for (int i = 0; i < dtblUploadType.Rows.Count; i++)
            {
                DataColumn dcolUpload = new DataColumn(dtblUploadType.Rows[i]["FIELD_NAME"].ToString());

                dtblUpload.Columns.Add(dcolUpload);
            }
        }
        else
        {
            strMsgID = Resources.JobResource.Job0000015;
            eLUpload.UPLOAD_STATUS = "N";
            LogUpload(eLUpload, eLUploadDetail, strMsgID);

            return dtblUpload;
        }


        #endregion

        int intTemp = 0;

        int intOut = 0;

        string strTemp = "";

        decimal decOut = 0;
        string strUpload = "";
        string strField = "";

        int intFieldLength = 0;

        int intDecimalDigits = 0;


        int intUploadTotalCount = 0;

        //*.CSV文件用ODBC做檢核
        if (dtblUploadCheck.Rows[0]["EXTEND_NAME"].ToString().ToUpper() == ".CSV")
        {
            #region  檔案筆數檢核

            DataTable dtblCsv = null;

            try
            {
                dtblCsv = Function.CsvToDtbl(file.DirectoryName, file.Name, dtblUploadType.Rows.Count);
            }
            catch
            {
                strMsgID = Resources.JobResource.Job0000016;
                eLUpload.UPLOAD_STATUS = "N";
                LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }

            intUploadTotalCount = dtblCsv.Rows.Count;

            eLUpload.UPLOAD_TOTAL_COUNT = intUploadTotalCount - intBeginCount - intEndCount;

            //* 資料行數大于15000,提示錯誤
            if (intUploadTotalCount - intBeginCount - intEndCount > intMax)
            {
                strMsgID = Resources.JobResource.Job0000017;
                eLUpload.UPLOAD_STATUS = "N";
                LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }


            #endregion

            #region  檔案欄位檢核

            string strMessage = "";
            BRL_UPLOAD.Add(eLUpload, ref strMessage);

            //* 頭筆數數據
            for (int i = 0; i < intBeginColumn; i++)
            {
                dtblBegin.Columns.Add("begin" + i.ToString());
            }

            //* 尾筆數數據
            for (int i = 0; i < intEndColumn; i++)
            {
                dtblEnd.Columns.Add("end" + i.ToString());
            }

            for (int j = 0; j < dtblCsv.Rows.Count; j++)
            {
                intTemp++;

                if (intTemp > intBeginCount && intTemp <= intUploadTotalCount - intEndCount)
                {
                    DataRow drowUpload = dtblUpload.NewRow();

                    //* 資料庫中欄位檢核個數與文件中的個數不等
                    if (dtblUploadType.Rows.Count > dtblCsv.Columns.Count)
                    {
                        dtblUpload.Rows.Add(drowUpload);

                        arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000018);

                        //* 資料庫中欄位檢核個數與文件中的個數不等,記錄進檢核日志
                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000018);
                    }
                    else
                    {
                        for (int i = 0; i < dtblUploadType.Rows.Count; i++)
                        {
                            strUpload = dtblCsv.Rows[j][i].ToString().Trim();

                            intFieldLength = int.Parse(dtblUploadType.Rows[i]["FIELD_LENGTH"].ToString());

                            intDecimalDigits = int.Parse(dtblUploadType.Rows[i]["DECIMAL_DIGITS"].ToString());

                            switch (dtblUploadType.Rows[i]["FIELD_TYPE"].ToString().ToUpper())
                            {
                                //* 字符類型
                                case "STRING":
                                    if (GetByteLength(strUpload) > intFieldLength)
                                    {
                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000019, ref arrListMsg);

                                        //* 欄位長度錯誤,記錄進檢核日志
                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000019);
                                    }
                                    break;

                                //* 整數類型
                                case "INT":
                                    if (!int.TryParse(strUpload == "" ? "0" : strUpload, out intOut))
                                    {
                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                        //* 欄位類型錯誤,記錄進檢核日志
                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                    }
                                    else
                                    {
                                        if (strUpload.Length > intFieldLength)
                                        {
                                            AddErrorMsg(intTemp, i, Resources.JobResource.Job0000019, ref arrListMsg);

                                            //* 欄位長度錯誤,記錄進檢核日志
                                            LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000019);
                                        }
                                    }
                                    break;

                                //* 時間日期類型
                                case "DATETIME":
                                    strField = strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "");
                                    if (!int.TryParse(strField == "" ? "0" : strField, out intOut))
                                    {
                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                        //* 欄位類型錯誤,記錄進檢核日志
                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                    }
                                    break;

                                //* 數字類型
                                case "DECIMAL":
                                    if (!decimal.TryParse(strUpload == "" ? "0" : strUpload, out decOut))
                                    {
                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                        //* 欄位類型錯誤,記錄進檢核日志
                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                    }
                                    else
                                    {
                                        if (strUpload.Split('.').Length > 1)
                                        {
                                            strField = strUpload.Split('.')[0];
                                            if (strField.Length > intFieldLength - intDecimalDigits - 1)
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                //* 欄位整數位數錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            }
                                            else
                                            {
                                                strField = strUpload.Split('.')[1];

                                                if (strField.Length > intDecimalDigits)
                                                {
                                                    AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                    //* 欄位小數位數錯誤,記錄進檢核日志
                                                    LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (strUpload.Length > intFieldLength - intDecimalDigits - 1)
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                //* 欄位整數位數錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            }
                                        }
                                    }

                                    break;


                                //* 百分比類型
                                case "PERCENT":
                                    strField = strUpload.Replace("%", "");

                                    if (!decimal.TryParse(strField == "" ? "0" : strField, out decOut))
                                    {
                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                        //* 欄位類型錯誤,記錄進檢核日志
                                        LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                    }
                                    else
                                    {
                                        if (strField.Split('.').Length > 1)
                                        {
                                            strTemp = strField.Split('.')[0];
                                            if (strTemp.Length > intFieldLength - intDecimalDigits - 2)
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);
                                                //* 欄位整數位數錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            }
                                            else
                                            {
                                                strTemp = strField.Split('.')[1];

                                                if (strTemp.Length > intDecimalDigits)
                                                {
                                                    AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                    //* 欄位小數位數錯誤,記錄進檢核日志
                                                    LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (strField.Length > intFieldLength - intDecimalDigits - 2)
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                //* 欄位整數位數錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, arrListMsg[arrListMsg.Count - 1].ToString());
                                            }
                                        }
                                    }

                                    break;

                            }
                            drowUpload[i] = strUpload;


                        }

                        dtblUpload.Rows.Add(drowUpload);
                    }
                }
                else if (intTemp <= intBeginCount)
                {
                    DataRow drowBegin = dtblBegin.NewRow();


                    for (int i = 0; i < dtblBegin.Columns.Count; i++)
                    {
                        if (dtblCsv.Rows[j][i] != null)
                            drowBegin[i] = dtblCsv.Rows[j][i].ToString().Trim();
                    }

                    dtblBegin.Rows.Add(drowBegin);
                }
                else
                {
                    DataRow drowEnd = dtblEnd.NewRow();

                    for (int i = 0; i < dtblEnd.Columns.Count; i++)
                    {
                        if (dtblCsv.Rows[j][i] != null)
                            drowEnd[i] = dtblCsv.Rows[j][i].ToString().Trim();
                    }

                    dtblEnd.Rows.Add(drowEnd);
                }
            }
            #endregion

            return dtblUpload;
        }
        else
        {

            #region  檔案筆數檢核
            StreamReader objStreamReader = null;
            //* 讀取文件,記錄行數
            try
            {
                objStreamReader = file.OpenText();

                while (objStreamReader.Peek() != -1)
                {
                    objStreamReader.ReadLine();
                    intUploadTotalCount++;
                }

                eLUpload.UPLOAD_TOTAL_COUNT = intUploadTotalCount - intBeginCount - intEndCount;
            }
            catch
            {
                strMsgID = Resources.JobResource.Job0000016;
                eLUpload.UPLOAD_STATUS = "N";
                LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }
            finally
            {
                objStreamReader.Close();
                file = null;
            }

            //* 資料行數大于15000,提示錯誤
            if (intUploadTotalCount - intBeginCount - intEndCount > intMax)
            {
                strMsgID = Resources.JobResource.Job0000017;
                eLUpload.UPLOAD_STATUS = "N";
                LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }

            #endregion

            #region  檔案欄位檢核

            try
            {
                string strMessage = "";
                BRL_UPLOAD.Add(eLUpload, ref strMessage);

                objStreamReader = new StreamReader(strFilePath, System.Text.Encoding.Default);

                string strString = "";


                string strSplit = dtblUploadCheck.Rows[0]["LIST_SEPARATOR"].ToString();

                #region 有分隔符
                if (strSplit != "")
                {
                    //* 頭筆數數據
                    for (int i = 0; i < intBeginColumn; i++)
                    {
                        dtblBegin.Columns.Add("begin" + i.ToString());
                    }

                    //* 尾筆數數據
                    for (int i = 0; i < intEndColumn; i++)
                    {
                        dtblEnd.Columns.Add("end" + i.ToString());
                    }

                    while (objStreamReader.Peek() != -1)
                    {
                        intTemp++;

                        strString = objStreamReader.ReadLine();

                        string[] strUploads = strString.Split(strSplit.ToCharArray());

                        if (intTemp > intBeginCount && intTemp <= intUploadTotalCount - intEndCount)
                        {
                            DataRow drowUpload = dtblUpload.NewRow();

                            //* 資料庫中欄位檢核個數與文件中的個數不等
                            if (dtblUploadType.Rows.Count > strUploads.Length)
                            {
                                dtblUpload.Rows.Add(drowUpload);

                                arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000018);
                                strMsgID = Resources.JobResource.Job0000009;
                                //* 資料庫中欄位檢核個數與文件中的個數不等,記錄進檢核日志
                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000018);
                            }
                            else
                            {
                                for (int i = 0; i < dtblUploadType.Rows.Count; i++)
                                {
                                    strUpload = strUploads[i].Trim();

                                    intFieldLength = int.Parse(dtblUploadType.Rows[i]["FIELD_LENGTH"].ToString());

                                    intDecimalDigits = int.Parse(dtblUploadType.Rows[i]["DECIMAL_DIGITS"].ToString());

                                    switch (dtblUploadType.Rows[i]["FIELD_TYPE"].ToString().ToUpper())
                                    {
                                        //* 字符類型
                                        case "STRING":
                                            if (GetByteLength(strUpload) > intFieldLength)
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000019, ref arrListMsg);

                                                //* 欄位長度錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000019);
                                            }
                                            break;

                                        //* 整數類型
                                        case "INT":
                                            if (!int.TryParse(strUpload == "" ? "0" : strUpload, out intOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            else
                                            {
                                                if (strUpload.Length > intFieldLength)
                                                {
                                                    AddErrorMsg(intTemp, i, Resources.JobResource.Job0000019, ref arrListMsg);

                                                    //* 欄位長度錯誤,記錄進檢核日志
                                                    LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000019);
                                                }
                                            }
                                            break;

                                        //* 時間日期類型
                                        case "DATETIME":
                                            strField = strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "");
                                            if (!int.TryParse(strField == "" ? "0" : strField, out intOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            break;

                                        //* 數字類型
                                        case "DECIMAL":
                                            if (!decimal.TryParse(strUpload == "" ? "0" : strUpload, out decOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            else
                                            {
                                                if (strUpload.Split('.').Length > 1)
                                                {
                                                    strField = strUpload.Split('.')[0];
                                                    if (strField.Length > intFieldLength - intDecimalDigits - 1)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                    else
                                                    {
                                                        strField = strUpload.Split('.')[1];

                                                        if (strField.Length > intDecimalDigits)
                                                        {
                                                            AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                            //* 欄位小數位數錯誤,記錄進檢核日志
                                                            LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000022);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (strUpload.Length > intFieldLength - intDecimalDigits - 1)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                }
                                            }

                                            break;


                                        //* 百分比類型
                                        case "PERCENT":
                                            strField = strUpload.Replace("%", "");

                                            if (!decimal.TryParse(strField == "" ? "0" : strField, out decOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            else
                                            {
                                                if (strField.Split('.').Length > 1)
                                                {
                                                    strTemp = strField.Split('.')[0];
                                                    if (strTemp.Length > intFieldLength - intDecimalDigits - 2)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                    else
                                                    {
                                                        strTemp = strField.Split('.')[1];

                                                        if (strTemp.Length > intDecimalDigits)
                                                        {
                                                            AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                            //* 欄位小數位數錯誤,記錄進檢核日志
                                                            LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000022);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (strField.Length > intFieldLength - intDecimalDigits - 2)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                }
                                            }

                                            break;

                                    }
                                    drowUpload[i] = strUpload;


                                }

                                dtblUpload.Rows.Add(drowUpload);
                            }
                        }
                        else if (intTemp <= intBeginCount)
                        {
                            DataRow drowBegin = dtblBegin.NewRow();


                            for (int i = 0; i < dtblBegin.Columns.Count; i++)
                            {
                                if (strUploads[i] != null)
                                    drowBegin[i] = strUploads[i];
                            }

                            dtblBegin.Rows.Add(drowBegin);
                        }
                        else
                        {
                            DataRow drowEnd = dtblEnd.NewRow();

                            for (int i = 0; i < dtblEnd.Columns.Count; i++)
                            {
                                if (strUploads[i] != null)
                                    drowEnd[i] = strUploads[i];
                            }

                            dtblEnd.Rows.Add(drowEnd);
                        }
                    }
                }
                #endregion
                #region 無分隔符
                else
                {
                    //* 頭筆數數據
                    dtblBegin.Columns.Add("begin");

                    //* 尾筆數數據
                    dtblEnd.Columns.Add("end");

                    int intRowTotal = 0;
                    //* 每行允許的總長度
                    for (int i = 0; i < dtblUploadType.Rows.Count; i++)
                    {
                        intRowTotal = intRowTotal + Convert.ToInt32(dtblUploadType.Rows[i]["FIELD_LENGTH"].ToString());
                    }

                    while (objStreamReader.Peek() != -1)
                    {
                        intTemp++;
                        strString = objStreamReader.ReadLine();

                        if (intTemp > intBeginCount && intTemp <= intUploadTotalCount - intEndCount)
                        {

                            DataRow drowUpload = dtblUpload.NewRow();

                            if (GetByteLength(strString) < intRowTotal)
                            {
                                dtblUpload.Rows.Add(drowUpload);

                                arrListMsg.Add(Resources.JobResource.Job0000009 + intTemp.ToString() + Resources.JobResource.Job0000023);
                                strMsgID = Resources.JobResource.Job0000009;
                                //* 欄位長度錯誤,記錄進檢核日志
                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000023);
                            }
                            else
                            {
                                int intNextBegin = 0;
                                for (int i = 0; i < dtblUploadType.Rows.Count; i++)
                                {

                                    intFieldLength = int.Parse(dtblUploadType.Rows[i]["FIELD_LENGTH"].ToString());

                                    intDecimalDigits = int.Parse(dtblUploadType.Rows[i]["DECIMAL_DIGITS"].ToString());


                                    //*截取需要檢核的欄位
                                    strUpload = GetSubstringByByte(strString, intNextBegin, intFieldLength, out intNextBegin).Trim();

                                    switch (dtblUploadType.Rows[i]["FIELD_TYPE"].ToString().ToUpper())
                                    {
                                        //* 整數類型
                                        case "INT":

                                            if (!int.TryParse(strUpload == "" ? "0" : strUpload, out intOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            break;

                                        //* 時間日期類型
                                        case "DATETIME":
                                            if (!int.TryParse(strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "") == "" ? "0" : strUpload.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", ""), out intOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            break;

                                        //* 數字類型
                                        case "DECIMAL":

                                            if (!decimal.TryParse(strUpload == "" ? "0" : strField, out decOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            else
                                            {
                                                if (strUpload.Split('.').Length > 1)
                                                {
                                                    strField = strUpload.Split('.')[0];
                                                    if (strField.Length > intFieldLength - intDecimalDigits - 1)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                    else
                                                    {
                                                        strField = strUpload.Split('.')[1];

                                                        if (strField.Length > intDecimalDigits)
                                                        {
                                                            AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                            //* 欄位小數位數錯誤,記錄進檢核日志
                                                            LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000022);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (strUpload.Length > intFieldLength - intDecimalDigits - 1)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                }
                                            }

                                            break;

                                        //* 百分比類型
                                        case "PERCENT":
                                            strField = strUpload.Replace("%", "");

                                            if (!decimal.TryParse(strField == "" ? "0" : strField, out decOut))
                                            {
                                                AddErrorMsg(intTemp, i, Resources.JobResource.Job0000020, ref arrListMsg);

                                                //* 欄位類型錯誤,記錄進檢核日志
                                                LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000020);
                                            }
                                            else
                                            {
                                                if (strField.Split('.').Length > 1)
                                                {
                                                    strTemp = strField.Split('.')[0];
                                                    if (strTemp.Length > intFieldLength - intDecimalDigits - 2)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                    else
                                                    {
                                                        strTemp = strField.Split('.')[1];

                                                        if (strTemp.Length > intDecimalDigits)
                                                        {
                                                            AddErrorMsg(intTemp, i, Resources.JobResource.Job0000022, ref arrListMsg);
                                                            //* 欄位小數位數錯誤,記錄進檢核日志
                                                            LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000022);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (strField.Length > intFieldLength - intDecimalDigits - 2)
                                                    {
                                                        AddErrorMsg(intTemp, i, Resources.JobResource.Job0000021, ref arrListMsg);
                                                        //* 欄位整數位數錯誤,記錄進檢核日志
                                                        LogUpload(eLUploadDetail, intTemp, Resources.JobResource.Job0000021);
                                                    }
                                                }
                                            }

                                            break;

                                    }
                                    drowUpload[i] = strUpload;

                                }
                                dtblUpload.Rows.Add(drowUpload);
                            }
                        }
                        else if (intTemp <= intBeginCount)
                        {
                            DataRow drowBegin = dtblBegin.NewRow();

                            drowBegin[0] = strString;

                            dtblBegin.Rows.Add(drowBegin);

                        }
                        else
                        {
                            DataRow drowEnd = dtblEnd.NewRow();

                            drowEnd[0] = strString;

                            dtblEnd.Rows.Add(drowEnd);
                        }

                    }
                }
                #endregion
            }
            catch
            {
                strMsgID = Resources.JobResource.Job0000016;
                eLUpload.UPLOAD_STATUS = "N";
                LogUpload(eLUpload, eLUploadDetail, strMsgID);

                return dtblUpload;
            }
            finally
            {
                objStreamReader.Close();
            }

            #endregion

            return dtblUpload;
        }
    }

    #endregion 匯入文檔格式檢核

    /// <summary>
    /// 半角轉全角
    /// </summary>
    /// <param name="strInput">待轉換字符串</param>
    /// <returns >全角字符串</returns>
    public static string ToSBC(string strInput)
    {
        char[] c = strInput.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new string(c);
    }

    /// <summary>
    /// 驗證日期格式是否正確
    /// </summary>
    /// <param name="strDate">待判斷的字符串</param>
    /// <returns >True:正確 False:不正確</returns>
    public static bool CheckDate(string strDate)
    {
        try
        {
            Convert.ToDateTime(strDate).ToString("yyyy/MM/dd");
            return true;
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            return false;
        }
    }
    /// <summary>
    /// 轉半角的方法(DBC case)
    /// </summary>
    /// <param name="strInput">待轉換字符串</param>
    /// <returns >半角字符串</returns>
    /// <remarks>
    /// 全角空格為12288，半角空格為32
    /// 其他字符半角(33-126)與全角(65281-65374)的對應關系是：均相差65248
    /// </remarks>
    public static string ToDBC(string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c);
    }

    /// <summary>
    /// 將Object對象值轉換成String類型並返回
    /// </summary>
    /// <param name="objValue">待轉換對象</param>
    /// <returns >TYPE:String</returns>
    /// <remarks>
    /// </remarks>
    public static string ObjToString(object objValue)
    {
        string strOutValue = string.Empty;
        if (null != objValue)
        {
            if (!string.IsNullOrEmpty(objValue.ToString()))
            {
                strOutValue = objValue.ToString();
            }
        }
        return strOutValue;
    }

}
