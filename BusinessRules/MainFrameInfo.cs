//******************************************************************
//*  作    者：余洋(rosicky)
//*  功能說明：主機信息作業
//*  創建日期：2009/07/28
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* 宋戈                2009/12/21      無                  1.整理格式
//*                                                         2.修改電文開關設置
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
using System.Collections;
using System.Xml;
using CSIPCommonModel.EntityLayer;
using Framework.Common.HTG;
using Framework.Common.JavaScript;
using CSIPCommonModel.BaseItem;
using System.IO;
using Framework.Common;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;

/// <summary>
/// 主機操作類
/// </summary>
public class MainFrameInfo
{


    /// <summary>
    /// 作者 余洋
    /// 創建日期：2009/10/23
    /// 取得電文參數
    /// </summary>
    /// <param name="strType">電文枚舉類型</param>
    /// <param name="strTemp">傳入參數</param>
    /// <returns>返回的字符串</returns>
    public static string GetStr(string strType,  string strTemp)
    {
        //* 欄位說明:
        //* strType 電文ID
        //* strTemp USER_ID         -   用戶ID
        //*         LINE_CNT        -   筆數欄位名稱
        //*         LINECNT         -   筆數欄位每頁最大數量
        //*         MESSAGE_TYPE    -   錯誤訊息返回欄位名稱
        //*         ISONLINE        -   是否已經上線
        switch (strType)
        {
            case "P4_JCAB":
                #region P4_JCAB
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";       
                    case "LINE_CNT":
                        //* 筆數欄位名稱
                        return "LINE_CNT";
                    case "LINECNT":
                        //* 筆數欄位每頁最大數量
                        return "0020";
                    case "MESSAGE_TYPE":
                        //* 錯誤訊息返回欄位名稱
                        return "MESSAGE_TYPE"; 
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAB_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion
            case "P4_JCAC":
                #region P4_JCAC
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAC_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion           
            case "P4_JCEH":
                #region P4_JCEH
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "LINECNT":
                        return "0007";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCEH_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion
            case "067050":
                #region 067050
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "MESSAGE_TYPE":
                        return "transactionId";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("067050_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCU9":
                #region P4_JCU9
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCU9_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion
            case "P4_JCII":
                #region P4_JCII
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCII_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion
            case "P4_JCFK":
                #region P4_JCFK
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCFK_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion
            case "P4_JCHN":
                #region P4_JCHN
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "LINECNT":
                        return "0030";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHN_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion
            case "P4_JCAS":
                #region P4_JCAS
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAS_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine"); 
                }
                break;
                #endregion
        }

        return "";
    }
    
    /// <summary>
    /// 作者 余洋
    /// 創建日期：2009/10/23
    /// 上傳并取得主機資料信息(含分頁)
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="stringArray">需要采集的欄位集合</param>
    /// <returns>主機返回的HashTable</returns>
    public static DataTable GetMainframeData(Hashtable htInput, string strType, ref string strMsg, string[] stringArray, String jobid = "")
    {
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo = (EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]; //*Session變數集合
        //*添加上傳主機信息
        htInput.Add("userId", eAgentInfo.agent_id);
        htInput.Add("passWord", eAgentInfo.agent_pwd);
        htInput.Add("racfId", eAgentInfo.agent_id_racf);
        htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        htInput.Add(GetStr(strType, "USER_ID"), eAgentInfo.agent_id_racf);
        htInput.Add(GetStr(strType, "LINE_CNT"), "0000");

        //* 根據傳入欄位建立DataTable
        DataTable dtblOutput = new DataTable();        
        for (int i = 0; i < stringArray.Length; i++)
        {
            dtblOutput.Columns.Add(stringArray[i], System.Type.GetType("System.String"));
        }               

        //*得到全部分頁主機傳回信息
        //strMsg = MainFrameInfo.GetMainFramePagesInfo(ref dtblOutput, strType, htInput, false, stringArray);
        strMsg = MainFrameInfo.GetMainFramePagesInfo(ref dtblOutput, strType, htInput, true, stringArray, jobid);

        return dtblOutput;
    }
    
    /// <summary>
    /// 上傳并取得主機資料信息(無分頁)
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <returns>傳出參數的HashTable</returns>
    public static Hashtable GetMainframeData(Hashtable htInput, string strType, ref string strMsg, bool blnIsClose, string[] stringArray, String jobid = "")
    {
        string strIsOnLine = GetStr(strType, "ISONLINE");           //* 該電文是否上線
        string strAuthOnLine = GetStr(strType, "AUTHONLINE");       //* HTG登入登出是否上線
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo = (EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]; //*Session變數集合
        #region 添加上傳主機信息
        //*添加上傳主機信息
        htInput.Add("userId", eAgentInfo.agent_id);
        htInput.Add("passWord", eAgentInfo.agent_pwd);
        htInput.Add("racfId", eAgentInfo.agent_id_racf);
        htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        
        Hashtable htOutput = new Hashtable();
        HTGCommunicator hc = new HTGCommunicator(jobid);
        string strFileName = Configure.HTGTempletPath + "req" + strType + ".xml";
        #endregion

        

        #region 取得電文的SessionId
        string SessionId = "";
        //*取得電文的SessionId
        if (System.Web.HttpContext.Current.Session["sessionId"] != null && System.Web.HttpContext.Current.Session["sessionId"].ToString() != "")
        {
            SessionId = System.Web.HttpContext.Current.Session["sessionId"].ToString();
        }
        //*如果SessionId為空,需要連接主機得到電文SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strAuthOnLine))
            {
                if (strMsg.Contains("rc value=606") || strMsg.Contains("rc value=704"))
                {
                    Logging.Log(strMsg, LogState.Info, LogLayer.HTG);
                }
                else
                {
                    Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                }
                return htOutput;
            }
            else
            {
                if (!blnIsClose)
                {
                    System.Web.HttpContext.Current.Session["sessionId"] = hc.SessionId;

                }
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (htInput.Contains("sessionId"))
        {
            htInput.Remove("sessionId");
        }
        htInput.Add("sessionId", SessionId);
        #endregion

        #region 建立reqHost物件

        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + strType + ".xml格式不正確或文件不存在";
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }
        #endregion


        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件
            
            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp"), reqHost, ref rtnHost, htInput, strIsOnLine);
            if (strMsg != "")
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            if (htOutput.Contains("sessionId"))
            {
                htOutput["sessionId"] = hc.SessionId;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionId"] = hc.SessionId;
            }
            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            #endregion

            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            #endregion

            #region 通過MessageType得到錯誤訊息
            string strMESSAGE_TYPE =GetStr(strType, "MESSAGE_TYPE");

            string strMessageType = "";
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgBody.data.QueryDataByID(strMESSAGE_TYPE, ref strMessageType);
                if (strMessageType != "")
                {
                    break;
                }
            }


            switch (strType)
            {
                //*主機P4_JCAB作業
                case "P4_JCEH":
                    switch (strMessageType)
                    {
                        case "0000":
                        case "0001":
                            break;
                        case "6666":
                            strMsg = "卡片檔NOT FOUND";
                            return htOutput;
                        case "7777":
                            strMsg = "卡人卡片關係檔NOT FOUND";
                            return htOutput;
                        case "8888":
                            strMsg = "該筆資料不存在";
                            return htOutput;
                        case "9999":
                            strMsg = "檔案未開";
                            return htOutput;
                        default:
                            strMsg = "其他錯誤";
                            return htOutput;
                    }
                    break;
                //*主機P4_JCAS作業
                case "P4_JCAS":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "6002":
                            strMsg = "結清日期必須正確";
                            return htOutput;
                        case "6011":
                            strMsg = "已結清或出完最後一期";
                            return htOutput;
                        case "6012":
                            strMsg = "試算結清日，必須不小於當天日期 ";
                            return htOutput;
                        case "6013":
                            strMsg = "日期計算錯誤";
                            return htOutput;
                        case "6021":
                            strMsg = "已結清，不可做REVERSE";
                            return htOutput;
                        case "6022":
                            strMsg = "未做提前結清試算註記，不用做 REVERSE";
                            return htOutput;
                        case "6023":
                            strMsg = "預定清償日為前二個作日內，不能還原";
                            return htOutput;
                        case "6024":
                            strMsg = "LOANP-STATUS=0 OR 5不可做借新還舊";
                            return htOutput;
                        case "8888":
                            strMsg = "NOT FOUND";
                            return htOutput;
                        case "9999":
                            strMsg = "NOT OPEN";
                            return htOutput;
                        default:
                            strMsg = "其他錯誤";
                            return htOutput;
                    }
                    break;
            }
            #endregion

            #region 將資料塞入HashTable
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                for (int j = 0; j < rtnHost.line[i].msgBody.data.Count; j++)
                {
                    htOutput.Add(rtnHost.line[i].msgBody.data[j].ID, rtnHost.line[0].msgBody.data[j].Value);
                }

            }
            #endregion

            #region 判斷所需傳回欄位是否都已傳回

            for (int i = 0; i < stringArray.Length; i++)
            {
                if (!htOutput.Contains(stringArray[i]))
                {
                    strMsg = "獲取主機資料失敗";
                    Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                    return htOutput;
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                string strMessage = "";
                if (!hc.CloseSession(ref strMessage, strAuthOnLine))
                {
                    strMsg = strMsg + "  " + strMessage;
                }
                System.Web.HttpContext.Current.Session["sessionId"] = "";
            }
        }
        return htOutput;

    }

    /// <summary>
    /// 上傳并取得主機資料信息(無分頁)
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <returns>傳出參數的HashTable</returns>
    public static Hashtable GetMainframeDataVD(Hashtable htInput, string strType, ref string strMsg, bool blnIsClose, string[] stringArray, String jobid = "")
    {
        string strIsOnLine = GetStr(strType, "ISONLINE");           //* 該電文是否上線
        string strAuthOnLine = GetStr(strType, "AUTHONLINE");       //* HTG登入登出是否上線
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo = (EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]; //*Session變數集合
        #region 添加上傳主機信息
        //*添加上傳主機信息
        htInput.Add("userId", eAgentInfo.agent_id);
        htInput.Add("passWord", eAgentInfo.agent_pwd);
        htInput.Add("racfId", eAgentInfo.agent_id_racf);
        htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        htInput.Add("signOnLU0", "yes");

        Hashtable htOutput = new Hashtable();
        HTGCommunicator hc = new HTGCommunicator(jobid);
        string strFileName = Configure.HTGTempletPath + "req" + strType + ".xml";
        #endregion



        #region 取得電文的SessionId
        string SessionId = "";
        //*取得電文的SessionId
        if (System.Web.HttpContext.Current.Session["sessionId"] != null && System.Web.HttpContext.Current.Session["sessionId"].ToString() != "")
        {
            SessionId = System.Web.HttpContext.Current.Session["sessionId"].ToString();
        }
        //*如果SessionId為空,需要連接主機得到電文SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strAuthOnLine))
            {
                if(strMsg.Contains("rc value=606") || strMsg.Contains("rc value=704"))
                {
                    Logging.Log(strMsg, LogState.Info, LogLayer.HTG);
                }
                else
                {
                    Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                }
                return htOutput;
            }
            else
            {
                if (!blnIsClose)
                {
                    System.Web.HttpContext.Current.Session["sessionId"] = hc.SessionId;

                }
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (htInput.Contains("sessionId"))
        {
            htInput.Remove("sessionId");
        }
        htInput.Add("sessionId", SessionId);
        #endregion

        #region 建立reqHost物件

        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + strType + ".xml格式不正確或文件不存在";
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }
        #endregion


        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件

            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp"), reqHost, ref rtnHost, htInput, strIsOnLine);
            if (strMsg != "")
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            if (htOutput.Contains("sessionId"))
            {
                htOutput["sessionId"] = hc.SessionId;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionId"] = hc.SessionId;
            }
            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            #endregion

            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            #endregion

            #region 通過MessageType得到錯誤訊息
            string strMESSAGE_TYPE = GetStr(strType, "MESSAGE_TYPE");

            string strMessageType = "";
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgHeader.data.QueryDataByID(strMESSAGE_TYPE, ref strMessageType);
                if (strMessageType != "")
                {
                    break;
                }
            }


            switch (strType)
            {
                //*主機067050作業
                case "067050":
                    switch (strMessageType)
                    {
                        case "067000":
                            break;
                        case "067050":
                            rtnHost.line[0].msgBody.data.QueryDataByID("ERRORMESSAGETEXT_OC01", ref strMsg);
                            return htOutput;
                    }
                    break;
            }
            #endregion

            #region 將資料塞入HashTable
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                for (int j = 0; j < rtnHost.line[i].msgBody.data.Count; j++)
                {
                    htOutput.Add(rtnHost.line[i].msgBody.data[j].ID, rtnHost.line[0].msgBody.data[j].Value);
                }

            }
            #endregion

            #region 判斷所需傳回欄位是否都已傳回

            for (int i = 0; i < stringArray.Length; i++)
            {
                if (!htOutput.Contains(stringArray[i]))
                {
                    strMsg = "獲取主機資料失敗";
                    Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                    return htOutput;
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                string strMessage = "";
                if (!hc.CloseSession(ref strMessage, strAuthOnLine))
                {
                    strMsg = strMsg + "  " + strMessage;
                }
                System.Web.HttpContext.Current.Session["sessionId"] = "";
            }
        }
        return htOutput;

    }


    /// <summary>
    /// 主機分頁公共方法
    /// </summary>
    /// <param name="DataTable">傳出的信息</param>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <param name="stringArray">需要采集的欄位集合</param>
    /// <returns>傳出的錯誤信息</returns>
    private static string GetMainFramePagesInfo(ref DataTable dtblOutput, string strType, Hashtable htInput, bool blnIsClose, string[] stringArray, String jobid = "")
    {
        int intLineCNT = 0;
        int intOldLineCNT = 0;
        int intMaxLineCNT = int.Parse(GetStr(strType, "LINECNT"));
        string strLineCNT = "";
        string strMsg = "";
        string strLINE_CNT = GetStr(strType, "LINE_CNT");
        int intTimes = 0;
        try
        {
            do
            {
                

                //* 上次電文得到的line
                strLineCNT = intLineCNT.ToString();
                strLineCNT = strLineCNT.PadLeft(4, '0');
                intOldLineCNT = int.Parse(strLineCNT);             
                htInput[strLINE_CNT] = strLineCNT;
                //*連接主機得到
                //strMsg = MainFrameInfo.GetMainFrameInfo(ref dtblOutput, strType, htInput, false, stringArray, intOldLineCNT, (intTimes * intMaxLineCNT) );
                strMsg = MainFrameInfo.GetMainFrameInfo(ref dtblOutput, strType, htInput, blnIsClose, stringArray, intOldLineCNT, (intTimes * intMaxLineCNT), jobid);
                intTimes = intTimes + 1;
                if (strMsg != "")
                {
                    return strMsg;
                }

                for (int i = dtblOutput.Rows.Count-1; i >= 0; i--)
                {
                    if (dtblOutput.Rows[i][strLINE_CNT].ToString().Trim() != "")
                    {
                        intLineCNT = int.Parse(dtblOutput.Rows[i][strLINE_CNT].ToString().Trim());
                        break;
                    }
                }

            } while (intLineCNT > (intTimes * intMaxLineCNT));
        }
        catch
        {
            strMsg="下行電文解析錯誤";
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return strMsg;
        }
        return "";
    }

    /// <summary>
    /// 上傳并取得主機資料信息(分頁用)
    /// </summary>
    /// <param name="DataTable">傳出的信息</param>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <param name="stringArray">需要采集的欄位集合</param>
    /// <returns>傳出參數的DataTable</returns>
    private static string GetMainFrameInfo(ref DataTable dtblOutput, string strTransactionId, Hashtable htInput, bool blnIsClose, string[] stringArray, int intOldLineCNT, int intLastGet, String jobid = "")
    {
        HTGCommunicator hc = new HTGCommunicator(jobid);
        string strFileName = Configure.HTGTempletPath + "req" + strTransactionId + ".xml";
        string strMsg = "";
        string SessionId = "";
        string strIsOnLine = GetStr(strTransactionId, "ISONLINE");  //* 電文是否上線
        string strAuthOnLine = GetStr(strTransactionId, "AUTHONLINE");       //* HTG登入登出是否上線

              
        #region 請求的Session
        //*取得電文的SessionId
        if (System.Web.HttpContext.Current.Session["sessionId"] != null && System.Web.HttpContext.Current.Session["sessionId"].ToString()!="")
        {
            SessionId = System.Web.HttpContext.Current.Session["sessionId"].ToString();
        }

        //*如果SessionId為空,需要連接主機得到電文SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strAuthOnLine))
            {
                if (strMsg.Contains("rc value=606") || strMsg.Contains("rc value=704"))
                {
                    Logging.Log(strMsg, LogState.Info, LogLayer.HTG);
                }
                else
                {
                    Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                }
                return strMsg;
            }
            else
            {
                if (!blnIsClose)
                {
                    //* 如果不是需要關閉的則將sessionid存入Session中
                    System.Web.HttpContext.Current.Session["sessionId"] = hc.SessionId;

                }
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (htInput.Contains("sessionId"))
        {
            htInput.Remove("sessionId");
        }
        htInput.Add("sessionId", SessionId);
        #endregion

        #region 建立reqHost物件
        //* 創建XML頭



        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + strTransactionId + ".xml格式不正確或文件不存在";
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return strMsg;
        }
        #endregion


        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件
            //* 連接HTG查詢資料
            
            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp"), reqHost, ref rtnHost, htInput, strIsOnLine);
            //* 如果strMsg不爲空代表出錯
            if (strMsg != "")
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return strMsg;
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionId"] = hc.SessionId;
            }
            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return strMsg;
            }
            #endregion

            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return strMsg;
            }
            string strLINE_CNT = "";
            string strMESSAGE_TYPE = "";

            strLINE_CNT = GetStr(strTransactionId, "LINE_CNT");
            strMESSAGE_TYPE = GetStr(strTransactionId, "MESSAGE_TYPE");
          
            #endregion

            #region 取得筆數
   
            string strCount = "";
            //* 取得筆數
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgBody.data.QueryDataByID(strLINE_CNT, ref strCount);
                if (strCount != "")
                {
                    break;
                }
            }
            //* 如果筆數爲0則返回
            if (strCount == "0" || strCount == "0000" || strCount == "")
            {
                strMsg="目前電文主機上資料筆數為0";
                Logging.Log(strTransactionId + strMsg, LogLayer.HTG);
                return strMsg;
            }

            //* 取得消息類型
            string strMessageType = "";
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgBody.data.QueryDataByID(strMESSAGE_TYPE, ref strMessageType);
                if (strMessageType != "")
                {
                    break;
                }
            }
            #endregion

            #region 根據電文,返回的MessageType不同,返回不同的消息
            //* 根據電文,返回的MessageType不同,返回不同的消息
            switch (strTransactionId)
            {
                //*主機P4_JCAB作業
                case "P4_JCAB":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "8888":
                            strMsg = "該筆資料不存在";
                            return strMsg;
                        case "9999":
                            strMsg = "主機系統錯誤";
                            return strMsg;
                        default:
                            strMsg = "其他錯誤";
                            return strMsg;
                    }
                    break;
                //*主機P4_JCAC作業
                case "P4_JCAC":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "8888":
                            strMsg = "該筆資料不存在";
                            return strMsg;
                        case "9999":
                            strMsg = "主機系統錯誤";
                            return strMsg;
                        default:
                            strMsg = "其他錯誤";
                            return strMsg;
                    }
                    break;                
                //*主機P4_JCEH作業
                case "P4_JCEH":
                    switch (strMessageType)
                    {
                        case "0000":
                        case "0001":
                            break;
                        case "6666":
                            strMsg = "卡片檔NOT FOUND";
                            return strMsg;
                        case "7777":
                            strMsg = "卡人卡片關係檔NOT FOUND";
                            return strMsg;
                        case "8888":
                            strMsg = "該筆資料不存在";
                            return strMsg;
                        case "9999":
                            strMsg = "檔案未開";
                            return strMsg;
                        default:
                            strMsg = "其他錯誤";
                            return strMsg;
                    }
                    break;
                //*主機P4_JCII作業
                case "P4_JCII":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "7001":
                            strMsg = "訂單檔JCVKIPOL未開";
                            return strMsg;
                        case "7002":
                            strMsg = "訂單檔JCVHIPOL未開";
                            return strMsg;
                        case "7003":
                            strMsg = "特店檔JCVKIPMR未開";
                            return strMsg;
                        case "8001":
                            strMsg = "查無此訂單檔JCVKIPOL資料";
                            return strMsg;
                        case "8002":
                            strMsg = "查無此訂單檔JCVHIPOL資料";
                            return strMsg;
                        case "8003":
                            strMsg = "查無此特店檔JCVKIPMR資料";
                            return strMsg;
                        case "9999":
                            strMsg = "系統異常 !";
                            return strMsg;
                        default:
                            strMsg = "其他錯誤";
                            return strMsg;
                    }
                    break;
                //*主機P4_JCFK作業
                case "P4_JCFK":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "1002":
                            strMsg = "功能碼有誤";
                            return strMsg;
                        case "1003":
                            strMsg = "ID未輸入";
                            return strMsg;
                        case "1004":
                            strMsg = "JCVKIDRL檔案未開啟";
                            return strMsg;
                        case "1005":
                            strMsg = "IDRL　START　NOT FOUND";
                            return strMsg;
                        case "8888":
                            strMsg = "無此筆資料，請重新輸入";
                            return strMsg;
                        case "9999":
                            strMsg = "其他錯誤";
                            return strMsg;
                        default:
                            strMsg = "其他錯誤";
                            return strMsg;
                    }
                    break;
                //*主機P4_JCU9作業
                case "P4_JCU9":
                    switch (strMessageType)
                    {
                        case "0000":
                        case "0001":
                            break;
                        case "8888":
                            strMsg = "無此筆資料，請重新輸入";
                            return strMsg;
                        case "9999":
                            strMsg = "其他錯誤";
                            return strMsg;
                        default:
                            strMsg = "其他錯誤";
                            return strMsg;
                    }
                    break;
                case "P4_JCHN":
                    switch (strMessageType)
                    {
                        case "0000":
                            break;
                        case "1001":
                            //strMsg = "己無次頁資訊";
                            break;
                        case "8001":
                            strMsg = "歸戶ID 輸入錯誤";
                            return strMsg;
                        case "8002":
                            strMsg = "檔案未開啟";
                            return strMsg;
                        case "8883":
                            strMsg = "ID NOT FOUND";
                            return strMsg;
                        case "8884":
                            strMsg = "卡片 NOT FOUND";
                            return strMsg;
                        case "9999":
                            strMsg = "其他錯誤";
                            return strMsg;
                        default:
                            strMsg = "其他錯誤";
                            return strMsg;
                    }
                    break;
            }
            #endregion
            
            int intIndex =0;
            string strTemp = "";

            int intCount=int.Parse(strCount);

            int intMaxCount = int.Parse(GetStr(strTransactionId, "LINECNT"));
            

            intCount = intCount - intLastGet;
            //* 如果返回總筆數>每頁筆數
            if (intCount > intMaxCount)
            {
                intCount = intMaxCount;
                //if (intOldLineCNT == intCount)
                //{
                //    intCount = 0;
                //}
                //else
                //{
                    //if (intCount % intMaxCount == 0 || intCount % intMaxCount == 1)
                    //{
                    //    intCount = intMaxCount;
                    //}
                    //else
                    //{
                    //    intCount = intCount % intMaxCount;
                    //}
                //}
                
            }

            #region 將資料塞入DataTable
            for (int cnt = 1; cnt <= intCount; cnt++)
            {
                DataRow drowOutput = dtblOutput.NewRow();

                for (int j = 0; j < stringArray.Length; j++)
                {
                    for (int i = 0; i < rtnHost.line.Count; i++)
                    {
                        strTemp = stringArray[j] + cnt.ToString();

                        intIndex = rtnHost.line[i].msgBody.data.QueryDataByID(strTemp);

                        if (intIndex != -1)
                        {
                            drowOutput[stringArray[j]] = rtnHost.line[i].msgBody.data[intIndex].Value;
                            break;
                        }
                        else
                        {
                            if (cnt == 1)
                            {
                                strTemp = strTemp.Substring(0, strTemp.Length - 1);
                                intIndex = rtnHost.line[i].msgBody.data.QueryDataByID(strTemp);
                                if (intIndex != -1)
                                {
                                    drowOutput[stringArray[j]] = rtnHost.line[i].msgBody.data[intIndex].Value;
                                    break;
                                }
                                else
                                {
                                    drowOutput[stringArray[j]] = "";
                                }
                            }
                            else
                            {
                                drowOutput[stringArray[j]] = "";
                            }
                        }
                    }
                   
                }
                 dtblOutput.Rows.Add(drowOutput);
            }
            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            return strMsg;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                string strMessage = "";
                if (!hc.CloseSession(ref strMessage, strAuthOnLine))
                {
                   strMsg= strMsg + "  "+strMessage;
                }
                System.Web.HttpContext.Current.Session["sessionId"] = "";
            }
        }
        return strMsg;

    }

     /// <summary>
    /// 檢查Session中是否有主機SessionID,如果有則發送電文關閉,並且清空這個SessionID
    /// </summary>
    /// <returns></returns>
    public static bool ClearSession()
    {
        //string strMsg = "";
        //string strIsOnline = UtilHelper.GetAppSettings("AUTH_IsOnLine");
        //try
        //{
        //    //* 取得Session中存的主機SessionID
        //    string strSessionID = (System.Web.HttpContext.Current.Session["sessionId"] + "").Trim();
        //    if (!string.IsNullOrEmpty(strSessionID))
        //    {
        //        //* 如果不爲空.發送主機電文,關掉
        //        HTGCommunicator hc = new HTGCommunicator();
        //        hc.SessionId = strSessionID;
        //        hc.CloseSession(ref strMsg, strIsOnline);
        //        //* 清空Session中的主機SessionID
        //        System.Web.HttpContext.Current.Session["sessionId"] = "";
        //    }
        //}
        //catch
        //{
        //    Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
        //    return false;
        //}
        return true;
    }
}

   
