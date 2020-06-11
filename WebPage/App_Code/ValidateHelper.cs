//******************************************************************
//*  功能說明：頁面數據合法性驗證公共函數
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
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
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using Framework.Common.JavaScript;
using Framework.Common.IO;
using System.Text.RegularExpressions;
using Framework.Common.Utility;
using Microsoft.VisualBasic;

public class ValidateHelper
{

    /// <summary>
    /// 功能說明:判斷日期的格式和起迄日是否合理
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="txtFrom">起日</param>
    /// <param name="txtTo">迄日</param>
    /// <param name="title">strMsgID</param>
    /// <returns></returns>
    public static bool IsValidDate(string txtFrom, string txtTo, ref string strMsgID)
    {
        if (string.IsNullOrEmpty(txtFrom) || string.IsNullOrEmpty(txtTo))
        {
            strMsgID = "06_06040100_024";
            return false;
        }

        DateTime dt1 = Convert.ToDateTime(txtFrom);
        DateTime dt2 = Convert.ToDateTime(txtTo);

        int iMinus = dt1.CompareTo(dt2);

        if (iMinus > 0)
        {
            strMsgID = "06_06040100_024";
            return false;
        }
        return true;
    }



    /// <summary>
    /// 功能說明:小數驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strVal"></param>
    /// <returns></returns>
    public static bool IsNumeric(string strVal, ref string strMsgID)
    {
        if (string.IsNullOrEmpty(strVal))
        {
            strMsgID = "06_00000000_003";
            return false;
        }
        else
        {
            if (Regex.IsMatch(strVal, "^[-]?\\d+[.]?\\d*$"))
            {
                strMsgID = "06_00000000_003";
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// 功能說明:中文字符驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strChinese"></param>
    /// <returns></returns>
    public static bool IsChinese(string strVal)
    {
        if (string.IsNullOrEmpty(strVal))
        {

            return false;
        }
        else
        {
            if (!Regex.IsMatch(strVal, "[^\u4E00-\u9FA5]+"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 功能說明:數字驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="strChinese"></param>
    /// <returns></returns>
    public static bool IsNum(string strVal)
    {
        if (string.IsNullOrEmpty(strVal))
        {
            return false;
        }
        else
        {
            if (Regex.IsMatch(strVal, "[0-9]"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}

