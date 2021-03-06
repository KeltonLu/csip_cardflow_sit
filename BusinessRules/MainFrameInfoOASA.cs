//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：主機信息作業


//*  創建日期：2009/07/28
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
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
public class MainFrameInfoOASA
{

    /// <summary>
    /// 上傳并取得主機資料信息
    /// 修改紀錄:2020/12/15_Ares_Stanley-修改P4_JCAX 非0001 LOG層級; 2020/12/30_Ares_Stanley-復原LOG紀錄條件
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <param name="strType">電文欄位檢核類型</param>
    /// <returns>傳出參數的HashTable</returns>
    public static Hashtable GetMainFrameInfo(HtgType type, Hashtable htInput, bool blnIsClose, string strType, EntityAGENT_INFO eAgentInfo, String jobid = "")
    {
        MainFrameInfoOASA.AddSession(htInput, eAgentInfo, type);

        string strTransactionId = type.ToString();

        string strHtgMessage = "";

        string strErrorMessage = type.ToString();

        string strIsOnLine = GetStr(type, "ISONLINE");//*是否上線

        ArrayList arrRet = new ArrayList();

        Hashtable htOutput = new Hashtable();

        HTGCommunicator hc = new HTGCommunicator(jobid);

        string strFileName = "";
        if ((type == HtgType.P4_PCTI) || (type == HtgType.P4D_PCTI))
        {

            strFileName = Configure.HTGTempletPath + "req" + strTransactionId + "_" + htInput["FUNCTION_ID"].ToString().Substring(0, 4) + ".xml";
        }
        else
        {
            strFileName = Configure.HTGTempletPath + "req" + strTransactionId + ".xml";
        }


        string strMsg = "";

        string SessionId = "";

        //*取得電文的SessionId
        if (htInput.Contains("sessionId"))
        {
            SessionId = htInput["sessionId"].ToString();
        }

        //*如果SessionId為空,需要連接主機得到電文SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strIsOnLine))
            {
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");

                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
                htInput["sessionId"] = hc.SessionId;
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }


        if (HttpContext.Current != null)
        {
            HttpContext.Current.Session["sessionId"] = hc.SessionId;
        }

        #region 建立reqHost物件

        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            if ((type == HtgType.P4_PCTI) || (type == HtgType.P4D_PCTI))
            {
                hc.RequestHostCreatorDnc(strFileName, ref reqHost, htInput);
            }
            else
            {
                hc.RequestHostCreator(strFileName, ref reqHost, htInput);
            }
        }
        catch
        {
            strMsg = "req" + strTransactionId + ".xml格式不正確或文件不存在";
            Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
            htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
            if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
            {
                htOutput.Add("HtgMsgFlag", "2");
            }
            else
            {
                htOutput.Add("HtgMsgFlag", "1");//*顯示端末訊息標識
            }
            return htOutput;
        }
        #endregion


        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件

            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp"), reqHost, ref rtnHost, htInput, strIsOnLine);

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

            if (strMsg != "")
            {
                //*"Session超時,..."
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }

            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }
            #endregion

            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                //*"下行電文為空"
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }

            #region 將資料塞入HashTable
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                if (rtnHost.line[i].msgBody.data != null)
                {
                    for (int j = 0; j < rtnHost.line[i].msgBody.data.Count; j++)
                    {
                        htOutput.Add(rtnHost.line[i].msgBody.data[j].ID.Trim(), rtnHost.line[0].msgBody.data[j].Value.Trim());
                    }
                }
            }
            #endregion

            string strLog = "";//*記錄錯誤日志變數
            switch (type)
            {
                case HtgType.P4_JCAX:
                    //*OASA
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "RTN_CD", "ERROR_DESC", "SOURCE_CODE", 
                                                                                "FUNCTION_CODE", "ACCT_NBR", "INHOUSE_INQ_FLAG", "NCCC_INQ_FLAG", "COUNTERFEIT_FLAG", 
                                                                                "OASA_BRAND", "FILLER5", "OASA_DEST1", "OASA_DEST2", "OASA_DEST3", "OASA_DEST4", "OASA_BLOCK_CODE", 
                                                                                "OASA_PURGE_DATE", "OASA_MEMO", "OASA_REASON_CODE", "OASA_ACTION_CODE", "OASA_DATE_REPORT", 
                                                                                "OASA_TIME_REPORT", "OASA_LAST_CHNG_DT", "OASA_OPID", "OASA_USER_ID", "OASA_RTNCD1", "OASA_RTNCD2", 
                                                                                "OASA_RTNCD3", "OASA_RTNCD4", "FILLER6", "GUAX_VISA_FILLER", "MASTER_VIP_AMT", "VISA_CWB_REGION1", "VISA_CWB_REGION2", 
                                                                                "VISA_CWB_REGION3", "VISA_CWB_REGION4", "VISA_CWB_REGION5", "VISA_CWB_REGION6","VISA_CWB_REGION7", "VISA_CWB_REGION8", 
                                                                                "VISA_CWB_REGION9", "MASTER_M_PURGE_DT1", "MASTER_M_EFT_DT1", "MASTER_USER_PURGE_DT1", "MASTER_FILLER", "JCB_CRB_REGION1", 
                                                                                "MASTER_M_PURGE_DT2", "MASTER_M_EFT_DT2", "MASTER_USER_PURGE_DT2","JCB_CRB_REGION2", 
                                                                                "MASTER_USER_PURGE_DT3", "MASTER_M_EFT_DT3", "MASTER_M_PURGE_DT3","JCB_CRB_REGION3", 
                                                                                "MASTER_USER_PURGE_DT4", "MASTER_M_EFT_DT4", "MASTER_M_PURGE_DT4", "JCB_CRB_REGION4", 
                                                                                "MASTER_USER_PURGE_DT5", "MASTER_M_EFT_DT5", "MASTER_M_PURGE_DT5", "JCB_CRB_REGION5", 
                                                                                "MASTER_USER_PURGE_DT6", "MASTER_M_EFT_DT6", "MASTER_M_PURGE_DT6"});
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "RTN_CD", "ERROR_DESC" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["RTN_CD"].ToString() != "0001" && htOutput["RTN_CD"].ToString() != "0000")
                    {
                        if (htOutput["ERROR_DESC"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["ERROR_DESC"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["RTN_CD"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["RTN_CD"].ToString() + " " + GetMessageType(type, htOutput["RTN_CD"].ToString()));
                        if (htOutput["ERROR_DESC"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["RTN_CD"].ToString() + " " + htOutput["ERROR_DESC"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["RTN_CD"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029"));
                        }
                    }
                    break;

                case HtgType.P4_JCAW:
                    //*EXMS1231
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", 
                                                                                "CARD_NO", "SELF_TAKE", "EMBNAME", "EMBTYPE", "EXPDATE_MM", 
                                                                                "EXPDATE_YY", "MEMNO", "CARD_NO_NEW"});
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCF7:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "PROGRAM_VERSION","USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR",
                                                             "PYMT_FLAG","FIXED_PYMT_AMNT","CURR_DUE","PAST_DUE","30DAYS_DELQ","60DAYS_DELQ","90DAYS_DELQ",
                                                             "120DAYS_DELQ","150DAYS_DELQ","180DAYS_DELQ","210DAYS_DELQ","USER_CODE","USER_CODE_2",
                                                             "BLOCK_CODE","CHGOFF_STATUS_FLAG","SHORT_NAME","CARD_EXPIR_DTE","DELQ_HIST1","DELQ_HIST2",
                                                              "DELQ_HIST3","DELQ_HIST4","DELQ_HIST5","DELQ_HIST6","DELQ_HIST7","DELQ_HIST8","DELQ_HIST9",
                                                              "DELQ_HIST10","DELQ_HIST11","DELQ_HIST12","DELQ_HIST13","DELQ_HIST14",
                                                              "DELQ_HIST15","DELQ_HIST16","DELQ_HIST17","DELQ_HIST18","DELQ_HIST19","DELQ_HIST20","DELQ_HIST21",
                                                              "DELQ_HIST22","DELQ_HIST23","DELQ_HIST24"});
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4A_JCGX:
                case HtgType.P4_JCGX:
                    //*EXMS 6063
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT","MERCH_ACCT", "CARD_TYPE", "AGENT_BANK_NMBR", 
                                                                                "DISCOUNT_RATE", "AGENT_DESC", "MESSAGE_CHI","AGENT_BANK_NMBR1","DISCOUNT_RATE1","CARD_TYPE1","CHANGE_ID1",
                                                                                "CHANGE_DATE1","CHANGE_ID_B1","CHANGE_DATE_B1","AGENT_DESC1","AGENT_BANK_NMBR2","DISCOUNT_RATE2","CARD_TYPE2",
                                                                                "CHANGE_ID2","CHANGE_DATE2","CHANGE_ID_B2","CHANGE_DATE_B2","AGENT_DESC2","AGENT_BANK_NMBR3","DISCOUNT_RATE3",
                                                                                "CARD_TYPE3","CHANGE_ID3","CHANGE_DATE3","CHANGE_ID_B3","CHANGE_DATE_B3","AGENT_DESC3","AGENT_BANK_NMBR4",
                                                                                "DISCOUNT_RATE4","CARD_TYPE4","CHANGE_ID4","CHANGE_DATE4","CHANGE_ID_B4","CHANGE_DATE_B4","AGENT_DESC4","AGENT_BANK_NMBR5",
                                                                                "DISCOUNT_RATE5","CARD_TYPE5","CHANGE_ID5","CHANGE_DATE5","CHANGE_ID_B5","CHANGE_DATE_B5","AGENT_DESC5","LINE_CNT"});
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000" && htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString());
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }

                    }
                    break;

                case HtgType.P4A_JCGQ:
                case HtgType.P4_JCGQ:
                    //*EXMS 6001
                    if (strType == "11")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE", "FORCE_FLAG", "CORP_NO", "APPL_NO",
                                                                                "BUILD_DATE","CREDIT_NO","CAPITAL","REG_NAME","ORGAN_TYPE","BUSINESS_NAME","RISK_FLAG","MARGIN_FLAG",
                                                                                "OWNER_NAME","OWNER_ID","OWNER_PHONE_AREA","OWNER_PHONE_NO","OWNER_PHONE_EXT","CHANGE_DATE1",
                                                                               "CHANGE_FLAG1","BIRTHDAY1","PHOTO_FLAG1","AT1","OWNER_CITY","OWNER_ADDR1",
                                                                               "OWNER_ADDR2","MANAGER_NAME","MANAGER_ID","MANAGER_PHONE_AREA","MANAGER_PHONE_NO",
                                                                               "MANAGER_PHONE_EXT","CHANGE_DATE2","CHANGE_FLAG2","BIRTHDAY2","PHOTO_FLAG2","AT2","CONTACT_NAME",
                                                                               "CONTACT_PHONE_AREA","CONTACT_PHONE_NO","CONTACT_PHONE_EXT","FAX_AREA",
                                                                               "FAX_PHONE_NO","REMITTANCE_NAME","REMITTANCE_ID","CHANGE_DATE3","CHANGE_FLAG3",
                                                                               "BIRTHDAY3","PHOTO_FLAG3","AT3","REG_CITY","REG_ADDR1","REG_ADDR2","REAL_CITY","REAL_ADDR1","REAL_ADDR2",
                                                                               "REAL_ZIP","MARKETING_FLAG1","MARKETING_FLAG2","NOSIGN_AMT","JCIC_CODE","DDA_BANK_NAME",
                                                                               "DDA_BANK_BRANCH","DDA_BANK_NAME_3RD","DDA_ACCT_NAME","DDA_BANK_BRANCH_3RD",
                                                                               "DDA_ACCT_NAME_3RD","IPMR_PREV_DESC","SALE_NAME","INVOICE_CYCLE","REDEEM_CYCLE",
                                                                                "UPDATE_DATE","UPDATE_USER","CREATE_DATE","CREATE_USER"});
                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0001" || htOutput["MESSAGE_TYPE"].ToString() == "0000"))
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }

                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString());
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }

                    }
                    break;

                case HtgType.P4_JCHO:
                    //* EXMS  1255
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "ID", "NAME" });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4D_JCF6:
                case HtgType.P4_JCF6:
                    //* JCF6
                    if (strType == "1" || strType == "12" || strType == "100")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "PROGRAM_VERSION", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                              "LINE_CNT","ACCT_NBR","NAME_1","CITY","ADDR_1","ADDR_2","EMPLOYER","HOME_PHONE","OFFICE_PHONE",
                                                              "MANAGE_ZIP_CODE","ZIP","MEMO_1","MEMO_2","CO_OWNER","CO_TAX_ID_TYPE","DD_ID","BILLING_CYCLE",
                                                              "BIRTH_DATE","NAME_1_2","EU_NBR_OF_DEPS","MEMBER_SINCE","OFF_PHONE_FLAG","EU_DIRECT_MAIL","WAIVE_FLAG",
                                                             "EU_CUSTOMER_CLASS","GRADUATE_YYMM","SHORT_NAME1","SHORT_NAME2","SHORT_NAME3","SHORT_NAME4","SHORT_NAME5",
                                                             "SHORT_NAME6","SHORT_NAME7","SHORT_NAME8","SHORT_NAME9","SHORT_NAME10","SHORT_NAME11","SHORT_NAME12",
                                                             "SHORT_NAME13","SHORT_NAME14","SHORT_NAME15","SHORT_NAME16","SHORT_NAME17","SHORT_NAME18","EMBOSSER_NAME_11",
                                                             "EMBOSSER_NAME_12","EMBOSSER_NAME_13","EMBOSSER_NAME_14","EMBOSSER_NAME_15","EMBOSSER_NAME_16",
                                                              "EMBOSSER_NAME_17","EMBOSSER_NAME_18","EMBOSSER_NAME_19","EMBOSSER_NAME_110","EMBOSSER_NAME_111","EMBOSSER_NAME_112",
                                                               "EMBOSSER_NAME_113","EMBOSSER_NAME_114","EMBOSSER_NAME_115","EMBOSSER_NAME_116","EMBOSSER_NAME_117",
                                                              "EMBOSSER_NAME_118","CARD_NMBR1","CARD_NMBR2","CARD_NMBR3","CARD_NMBR4","CARD_NMBR5","CARD_NMBR6","CARD_NMBR7",
                                                               "CARD_NMBR8","CARD_NMBR9","CARD_NMBR10","CARD_NMBR11","CARD_NMBR12","CARD_NMBR13","CARD_NMBR14","CARD_NMBR15",
                                                               "CARD_NMBR16","CARD_NMBR17","CARD_NMBR18","CARD_ID1","CARD_ID2","CARD_ID3","CARD_ID4","CARD_ID5","CARD_ID6",
                                                             "CARD_ID7","CARD_ID8","CARD_ID9","CARD_ID10","CARD_ID11","CARD_ID12","CARD_ID13","CARD_ID14","CARD_ID15",
                                                             "CARD_ID16","CARD_ID17","CARD_ID18","TYPE1","TYPE2","TYPE3","TYPE4","TYPE5","TYPE6","TYPE7","TYPE8","TYPE9",
                                                             "TYPE10","TYPE11","TYPE12" ,"TYPE13","TYPE14","TYPE15","TYPE16","TYPE17","TYPE18"});
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCDK:
                    //*P4_JCDK
                    if (strType == "1" || strType == "100")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "PROGRAM_VERSION", "USER_ID", "MESSAGE_TYPE" ,
                                                                                    "FUNCTION_CODE","LINE_CNT","ACCT_NBR", "MOBILE_PHONE","8DCALL","PAGER","EMAIL",
                                                                                    "TITLE","TEL_PERM","E_SERVICE_CODE","CUS2_PERM_ZIP","CUS2_PERM_CITY","CUS2_PERM_ADDR_1",
                                                                                    "CUS2_PERM_ADDR_2","CUS2_INFORMANT_NAME_1","CUS2_INFORMANT_ZONE_1","CUS2_INFORMANT_NO_1",
                                                                                    "CUS2_RELATIVE_1","CUS2_INFORMANT_NAME_2","CUS2_INFORMANT_ZONE_2","CUS2_INFORMANT_NO_2",
                                                                                    "CUS2_RELATIVE_2","CUS2_PARENT_NAME","CUS2_PARENT_ZIP","CUS2_PARENT_CITY","CUS2_PARENT_ADDR_1",
                                                                                    "CUS2_PARENT_ADDR_2","CUS2_PARENT_ZONE","CUS2_PARENT_NO"});
                    }

                    if (strType == "2" || strType == "200")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_PCTI:
                    if (strType == "2" || strType == "200")
                    {
                        arrRet = new ArrayList(new object[] { "ERR_MSG_DATA", "ERR_MSG_NO", "ERR_MSG_CODE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "200")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (htOutput["ERR_MSG_DATA"].ToString() != "00999")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["ERR_MSG_DATA"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["ERR_MSG_DATA"].ToString() + " " + GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString()));
                    }
                    break;

                case HtgType.P4D_PCTI:
                    if (strType == "22")
                    {
                        arrRet = new ArrayList(new object[] { "ERR_MSG_DATA", "ERR_MSG_NO", "ERR_MSG_CODE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["ERR_MSG_DATA"].ToString() != "00999")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["ERR_MSG_DATA"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["ERR_MSG_DATA"].ToString() + " " + GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString()));
                    }
                    break;

                case HtgType.P4_JCIL:
                    //*EXMS_P4_sessionA
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", 
                                                                                    "LINE_CNT", "MER_NO", "MER_NEME", "CORP_NO", "OWNER_NAME", "BANK_NAME", 
                                                                                    "ACCT_NEME", "ACCT_NO", "CONTACT_TEL", "CONTACT_FAX", "ADDRESS1", "ADDRESS2", 
                                                                                    "ADDRESS3", "DESCRIPTION", "CREATE_DATE", "CREATE_USERID" });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCAA:

                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                                                    "LINE_CNT", "ACCT_NBR", "DTE_BIRTH","HOME_PHONE","EMPLOYER","CUST_GRADE",
                                                                                    "EU_TITLE_1","AVAIL_CASH","OFFICE_PHON","TITLE","DELQ","PASSWORD","SHORT_NAME",
                                                                                    "MOBILE_PHONE","CITY","ADDR_1","ADDR_2","CO_OWNER", "SINCE", "CRG","CYCLE","EMAIL",
                                                                                    "ARG","SERV_GRAD","E_SERVICE_CODE","ZIP","ADDR_DATE","SMSA","FAX_NO", "TYPE1","TYPE2",
                                                                                    "TYPE3","TYPE4","TYPE5","TYPE6","TYPE7","TYPE8","TYPE9","TYPE10","TYPE11","TYPE12","TYPE13",
                                                                                    "TYPE14","TYPE15","TYPE16","TYPE17","TYPE18","TYPE19","TYPE20","CARD_NMBR1","CARD_NMBR2",
                                                                                    "CARD_NMBR3","CARD_NMBR4","CARD_NMBR5","CARD_NMBR6","CARD_NMBR7","CARD_NMBR8",
                                                                                    "CARD_NMBR9","CARD_NMBR10","CARD_NMBR11","CARD_NMBR12","CARD_NMBR13","CARD_NMBR14",
                                                                                    "CARD_NMBR15","CARD_NMBR16","CARD_NMBR17","CARD_NMBR18","CARD_NMBR19","CARD_NMBR20",
                                                                                    "BLOCK_CODE1","BLOCK_CODE2","BLOCK_CODE3","BLOCK_CODE4","BLOCK_CODE5","BLOCK_CODE6",
                                                                                    "BLOCK_CODE7","BLOCK_CODE8","BLOCK_CODE9","BLOCK_CODE10","BLOCK_CODE11","BLOCK_CODE12",
                                                                                    "BLOCK_CODE13","BLOCK_CODE14","BLOCK_CODE15","BLOCK_CODE16","BLOCK_CODE17","BLOCK_CODE18",
                                                                                    "BLOCK_CODE19","BLOCK_CODE20","CUSTID1","CUSTID2","CUSTID3","CUSTID4","CUSTID5","CUSTID6","CUSTID7",
                                                                                    "CUSTID8","CUSTID9","CUSTID10","CUSTID11","CUSTID12","CUSTID13","CUSTID14","CUSTID15","CUSTID16","CUSTID17",
                                                                                    "CUSTID18","CUSTID19","CUSTID20","NAME11","NAME12","NAME13","NAME14","NAME15","NAME16","NAME17","NAME18",
                                                                                    "NAME19","NAME110","NAME111","NAME112","NAME113","NAME114","NAME115","NAME116","NAME117","NAME118",
                                                                                    "NAME119","NAME120","USER_CODE_31","USER_CODE_32","USER_CODE_33","USER_CODE_34","USER_CODE_35","USER_CODE_36",
                                                                                    "USER_CODE_37","USER_CODE_38","USER_CODE_39","USER_CODE_310","USER_CODE_311","USER_CODE_312","USER_CODE_313",
                                                                                    "USER_CODE_314","USER_CODE_315","USER_CODE_316","USER_CODE_317","USER_CODE_318","USER_CODE_319","USER_CODE_320"});
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCEM:

                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                                                    "LINE_CNT", "ACCT_NBR", "CARD_TAKE","EMBOSS_NAME","EMBOSS_TYPE",
                                                                                    "EXPIRE_DATE","XFR_CARD_NO","BLOCK_CODE","STATUS","PREV_ACTION",
                                                                                    "DTE_LST_REQ","MEM_NO","CARDHOLDER_ID","CARDHOLDER_NAME","CUSTOMER_NAME",
                                                                                    "CITY","ADDR_1","ADDR_2","EMBOSS_DATE","EMBOSS_TIME"});
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4A_JCGR:
                    //*EXMS_P4A_sessionB
                    if (strType == "11")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE", 
                                                                                    "MERCHANT_NO", "BUILD_DATE", "MERCHANDISE", "CREDIT_NO", "CAPITAL", "REG_NAME" ,
                                                                                    "ORGAN_TYPE","MERCH_PICT","BUSINESS_NAME","RISK_FLAG","DISOBEY","DDA_BANK_NAME",
                                                                                    "DDA_BANK_BRANCH","CHECK_CODE","MERCH_VCR","DDA_ACCT_NAME","OWNER_NAME",
                                                                                    "OWNER_ID","OWNER_PHONE_AREA","OWNER_PHONE_NO","OWNER_PHONE_EXT","OWNER_CITY",
                                                                                    "OWNER_ADDR1","OWNER_ADDR2","MANAGER_NAME","MANAGER_ID","MANAGER_PHONE_AREA",
                                                                                    "MANAGER_PHONE_NO","MANAGER_PHONE_EXT","REMITTANCE_NAME","REMITTANCE_ID","PROJECT_NO",
                                                                                    "CONTACT_NAME","CONTACT_PHONE_AREA","CONTACT_PHONE_NO","CONTACT_PHONE_EXT","FAX_AREA",
                                                                                    "FAX_PHONE_NO","REG_CITY","REG_ADDR1","REG_ADDR2","REAL_CITY","REAL_ADDR1","REAL_ADDR2",
                                                                                    "REAL_ZIP","REDEEM_CYCLE","IMPRINTER_TYPE1","IMPRINTER_TYPE2","IMPRINTER_QTY1","IMPRINTER_QTY2",
                                                                                    "IMPRINTER_DEPO","POS_TYPE1","POS_TYPE2","POS_QTY1","POS_QTY2","POS_DEPO","EDC_TYPE1","EDC_TYPE2",
                                                                                    "EDC_QTY1","EDC_QTY2","EDC_DEPO","BLACK_CODE","BLACK_QTY","SALE_NAME","JOINT_TYPE","INVOICE_CYCLKE",
                                                                                    "CORP_NO","CORP_SEQ","APPL_NO","MERCHANT_TYPE","MARGIN_FLAG","USER_DATA","FILE_NO","UPDATE_DATE",
                                                                                    "UPDATE_USER","APPROVED_DATE"});
                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000" && htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString());
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;

                case HtgType.P4_JCIJ:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE","LINE_CNT", "MER_NO", "MER_NEME", 
                                                                                    "CORP_NO", "OWNER_NAME", "BANK_NAME", "ACCT_NEME", "ACCT_NO", "CONTACT_TEL", 
                                                                                    "CONTACT_FAX", "ADDRESS1", "ADDRESS2", "ADDRESS3", "DESCRIPTION", "CREATE_DATE", "CREATE_USERID" 
                                                                                  });
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4A_JCHQ:
                case HtgType.P4_JCHQ:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE", "ORGN", 
                                                                                    "ACCT", "ID_NAME", "ID_CITY", "CONTACT", "MERCH_MEMO", "PHONE_NMBR1", "PHONE_NMBR2", "PHONE_NMBR3", 
                                                                                    "OFFICER_ID", "DB_ACCT_NMBR","MERCH_TYPE","MERCHANT_NAME","CHAIN_STORE","ADDRESS1","HOLD_STMT_FLAG","ADDRESS2",
                                                                                    "ADDRESS3","ZIP_CODE","MCC","CHAIN_MER_NBR","CHAIN_MER_LEVEL","NBR_IMPRINTER1","NBR_IMPRINTER2",
                                                                                    "NBR_IMPRINTER3","NBR_POS_DEV1","NBR_POS_DEV2","NBR_POS_DEV3","PROJ_AVG_TKT","PROJ_MTH_VOLUME",
                                                                                    "AGENT_BANK","BRANCH","ROUTE_TRANSIT","CHAIN_STMT_IND","CHAIN_REPRT_IND","CHAIN_SETT_IND","CHAIN_DISC_IND",
                                                                                    "CHAIN_FEES_IND","CHAIN_DD_IND","USER_DATA1","USER_DATA2","VISA_INTCHG_FLAG","VISA_SPECL_COND_1","VISA_SPECL_COND_2",
                                                                                    "VISA_MAIL_PHONE_IND","POS_CAP","POS_MODE","AUTH_SOURCE","CH_ID","MC_INTCHG_FLAG","CARD_STATUS1","CARD_DISC_RATE1",
                                                                                    "CARD_STATUS2","CARD_DISC_RATE2","CARD_STATUS3","CARD_DISC_RATE3","CARD_STATUS4","CARD_DISC_RATE4","CARD_STATUS5",
                                                                                    "CARD_DISC_RATE5","CARD_STATUS6","CARD_DISC_RATE6","CARD_STATUS7","CARD_DISC_RATE7","CARD_STATUS8","CARD_DISC_RATE8",
                                                                                    "STATUS_FLAG","DTE_LST_RTE_ADJ","DTE_USER_1","DTE_USER_2","USER_CODE_1","USER_CODE_2","USER_CODE_3","AVG_TKT_RANGES1",
                                                                                    "AVG_TKT_RANGES2","AVG_TKT_RANGES3","AVG_TKT_RANGES4","STMT_MSG_SUPPRESS","INTCHG_REJECT","CARD_STATUS9","CARD_DISC_RATE9",
                                                                                    "CARD_STATUS10","CARD_DISC_RATE10","CARD_STATUS11","CARD_DISC_RATE11","CARD_STATUS12","CARD_DISC_RATE12","CARD_STATUS13",
                                                                                    "CARD_DISC_RATE13","CARD_STATUS14","CARD_DISC_RATE14","CARD_STATUS15","CARD_DISC_RATE15"});
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;

                case HtgType.P4A_JCHR:
                case HtgType.P4_JCHR:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE", "ORGN", 
                                                                                    "ACCT", "STATUS_FLAG","ID_NAME", "ID_CITY", "CONTACT", "MERCH_MEMO", "PHONE_NMBR1", "PHONE_NMBR2",
                                                                                    "PHONE_NMBR3", "OFFICER_ID", "DB_ACCT_NMBR","MERCH_TYPE","MERCHANT_NAME","ADDRESS1","HOLD_STMT_FLAG","ADDRESS2",
                                                                                    "ADDRESS3","ZIP_CODE","MCC","CHAIN_MER_NBR","CHAIN_MER_LEVEL","NBR_IMPRINTER1","NBR_IMPRINTER2",
                                                                                    "NBR_IMPRINTER3","NBR_POS_DEV1","NBR_POS_DEV2","NBR_POS_DEV3", "DTE_LST_RTE_ADJ","DTE_USER_1",
                                                                                    "DTE_USER_2","USER_CODE_1","USER_CODE_2","USER_CODE_3","PROJ_AVG_TKT","PROJ_MTH_VOLUME",
                                                                                    "AVG_TKT_RANGES1","AVG_TKT_RANGES2","AVG_TKT_RANGES3","AVG_TKT_RANGES4","AGENT_BANK",
                                                                                    "BRANCH","ROUTE_TRANSIT","STMT_MSG_SUPPRESS","INTCHG_REJECT","CHAIN_STMT_IND","CHAIN_REPRT_IND",
                                                                                    "CHAIN_SETT_IND","CHAIN_DISC_IND","CHAIN_FEES_IND","CHAIN_DD_IND","USER_DATA1","USER_DATA2",
                                                                                    "VISA_INTCHG_FLAG","VISA_SPECL_COND_1","VISA_SPECL_COND_2","VISA_MAIL_PHONE_IND","POS_CAP",
                                                                                    "POS_MODE","AUTH_SOURCE","CH_ID","MC_INTCHG_FLAG","CARD_STATUS1","CARD_STATUS2","CARD_STATUS3",
                                                                                    "CARD_STATUS4","CARD_STATUS5","CARD_STATUS6","CARD_STATUS7","CARD_STATUS8","CARD_DISC_RATE1",
                                                                                    "CARD_DISC_RATE2","CARD_DISC_RATE3","CARD_DISC_RATE4","CARD_DISC_RATE5","CARD_DISC_RATE6",
                                                                                    "CARD_DISC_RATE7","CARD_DISC_RATE8","CARD_STATUS9","CARD_DISC_RATE9","CARD_STATUS10",
                                                                                    "CARD_DISC_RATE10","CARD_STATUS11","CARD_DISC_RATE11","CARD_STATUS12","CARD_DISC_RATE12",
                                                                                    "CARD_STATUS13","CARD_DISC_RATE13","CARD_STATUS14","CARD_DISC_RATE14","CARD_STATUS15","CARD_DISC_RATE15"
                                                               });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;

                case HtgType.P4A_JCCA:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID","PROGRAM_ID","USER_ID","MESSAGE_TYPE","FUNCTION_CODE","MERCH_ACCT",
                                                                                    "MERCH_NAME","BUSINESS_NAME","MESSAGE_CHI","CARD_STATUS1","CARD_DESCR1",
                                                                                    "CARD_DESCR_SHORT1","DISC_RATE1","CARD_UPDATE_DATE1","CARD_STATUS2","CARD_DESCR2",
                                                                                    "CARD_DESCR_SHORT2","DISC_RATE2","CARD_UPDATE_DATE2","CARD_STATUS3","CARD_DESCR3",
                                                                                    "CARD_DESCR_SHORT3","DISC_RATE3","CARD_UPDATE_DATE3","CARD_STATUS4","CARD_DESCR4",
                                                                                    "CARD_DESCR_SHORT4","DISC_RATE4","CARD_UPDATE_DATE4","CARD_STATUS5","CARD_DESCR5",
                                                                                    "CARD_DESCR_SHORT5","DISC_RATE5","CARD_UPDATE_DATE5","CARD_STATUS6","CARD_DESCR6",
                                                                                    "CARD_DESCR_SHORT6","DISC_RATE6","CARD_UPDATE_DATE6","CARD_STATUS7","CARD_DESCR7",
                                                                                    "CARD_DESCR_SHORT7","DISC_RATE7","CARD_UPDATE_DATE7","CARD_STATUS8","CARD_DESCR8",
                                                                                    "CARD_DESCR_SHORT8","DISC_RATE8","CARD_UPDATE_DATE8","CARD_STATUS9","CARD_DESCR9",
                                                                                    "CARD_DESCR_SHORT9","DISC_RATE9","CARD_UPDATE_DATE9","CARD_STATUS10","CARD_DESCR10",
                                                                                    "CARD_DESCR_SHORT10","DISC_RATE10","CARD_UPDATE_DATE10","CREATE_USER","CREATE_DATE",
                                                                                    "CREATE_TIME","MAINTAIN_USER","MAINTAIN_DATE","MAINTAIN_TIME"
                                                               });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;
                case HtgType.P4L_LGOR:
                    if ("100" == strType)
                    {
                        arrRet = new ArrayList(new object[] { "MSG_SEQ", "MSG_ERR" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (!(htOutput["MSG_SEQ"].ToString() == "0000" && htOutput["MSG_ERR"].ToString() == "00"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString() + " " + GetMessageType(type, htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString()));
                    }
                    break;
                case HtgType.P4L_LGAT:
                    if ("100" == strType)
                    {
                        arrRet = new ArrayList(new object[] { "MSG_SEQ", "MSG_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (!(htOutput["MSG_SEQ"].ToString() == "0000" && htOutput["MSG_TYPE"].ToString() == "00"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MSG_SEQ"].ToString() + htOutput["MSG_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MSG_SEQ"].ToString() + htOutput["MSG_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MSG_SEQ"].ToString() + htOutput["MSG_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MSG_SEQ"].ToString() + htOutput["MSG_TYPE"].ToString()));
                    }
                    break;
            }

            if (htOutput.Contains("HtgMsg"))
            {
                if (type == HtgType.P4_PCTI || type == HtgType.P4D_PCTI)
                {
                    htOutput["HtgMsg"] = type.ToString() + "_" + htInput["FUNCTION_ID"].ToString().Substring(0, 4) + ":" + strLog;
                }
                else
                {
                    htOutput["HtgMsg"] = type.ToString() + ":" + strLog;
                }

                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Info, LogLayer.HTG);
                
                return htOutput;
            }
            else
            {
                if (type == HtgType.P4_PCTI || type == HtgType.P4D_PCTI)
                {
                    htOutput["HtgSuccess"] = type.ToString() + "_" + htInput["FUNCTION_ID"].ToString().Substring(0, 4) + ":" + htOutput["HtgSuccess"].ToString();
                }
                else
                {
                    htOutput["HtgSuccess"] = type.ToString() + ":" + htOutput["HtgSuccess"].ToString();
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");

            if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
            {
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "2");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "2";//*顯示主機訊息標識
                }
            }
            else
            {
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "1");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "1";//*顯示主機訊息標識
                }
            }
            Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                if (!hc.CloseSession(ref strMsg))
                {
                    if (htOutput.Contains("HtgMsg"))
                    {
                        htOutput["HtgMsg"] = htOutput["HtgMsg"].ToString() + "  " + strMsg;
                    }
                    else
                    {
                        htOutput.Add("HtgMsg", strMsg);
                    }
                }
                else
                {
                    if (htOutput.Contains("sessionId"))
                    {
                        htOutput["sessionId"] = "";
                    }
                    else
                    {
                        htOutput.Add("sessionId", "");
                    }
                }
            }
        }
        return htOutput;
    }

    /// <summary>
    /// 檢核主機回傳欄位
    /// </summary>
    /// <param name="htOutput">主機回傳Hashtbale</param>
    /// <param name="arrRet">欄位集合</param>
    /// <param name="strErrorMessage">錯誤提示信息</param>
    /// <returns>true成功，false失敗</returns>
    public static bool CheckHtgColumn(ref Hashtable htOutput, ArrayList arrRet, string strErrorMessage)
    {
        foreach (string strTemp in arrRet)
        {
            if (!htOutput.ContainsKey(strTemp))
            {
                string strMsg = "主機電文錯誤";
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "1");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "1";//*顯示主機訊息標識
                }
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
                return false;
            }
        }
        return true;
    }

    //*作者 趙呂梁
    //*創建日期：2009/12/22
    //*修改日期：2009/12/22
    /// <summary>
    /// 獲取信息說明中文內容
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="strMessageCode">代碼</param>
    /// <returns>中文說明</returns>
    public static string GetMessageType(HtgType type, string strMessageCode)
    {
        string strMessage = "";

        switch (type)
        {
            case HtgType.P4D_PCTI:
            case HtgType.P4_PCTI:
                switch (strMessageCode)
                {
                    case "00999":
                        strMessage = "主機作業成功! ";
                        break;
                    default:
                        strMessage = "系統異常 ! ";
                        break;
                }
                break;

            case HtgType.P4D_JCF6:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "OK. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCF6:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "OK. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JCAX:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "OK. ";
                        break;
                    case "7777":
                        strMessage = "ERROR. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCF7:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "OK. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCHO:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "9999":
                        strMessage = "系統異常! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCGQ:
            case HtgType.P4A_JCGQ:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCAA:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCEM:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "9999":
                        strMessage = "系統異常! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCIL:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCGR:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "異常. ";
                        break;
                    case "9999":
                        strMessage = "檔案未開啟. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCIJ:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCGX:
            case HtgType.P4_JCGX:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "異常. ";
                        break;
                    case "9999":
                        strMessage = "檔案未開啟. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCCA:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "ERROR. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCAW:
                switch (strMessageCode)
                {
                    #region
                    case "0000":
                    case "0001":
                        strMessage = "正常. ";
                        break;
                    case "1001":
                        strMessage = "取卡方式不正確. ";
                        break;
                    case "1002":
                        strMessage = "製卡姓名不正確. ";
                        break;
                    case "1003":
                        strMessage = "製卡式樣不正確. ";
                        break;
                    case "1004":
                        strMessage = "有效期限不正確. ";
                        break;
                    case "1005":
                        strMessage = "卡號不正確. ";
                        break;
                    case "1006":
                        strMessage = "卡號型別無法判別. ";
                        break;
                    case "1007":
                        strMessage = "卡號型別有誤(TYPE=160). ";
                        break;
                    case "1008":
                        strMessage = "世足卡（含手錶）須為WAVE卡. ";
                        break;
                    case "1009":
                        strMessage = "世足手錶不可掛補. ";
                        break;
                    case "1010":
                        strMessage = "世足手錶超過一年，已不能毀補. ";
                        break;
                    case "1011":
                        strMessage = "世足卡（含手錶）不能展期. ";
                        break;
                    case "2001":
                        strMessage = "卡片檔內資料不存在. ";
                        break;
                    case "2002":
                        strMessage = "掛失資料未移轉. ";
                        break;
                    case "2003":
                        strMessage = "製卡檔資料不存在. ";
                        break;
                    case "2004":
                        strMessage = "卡人檔內資料不存在. ";
                        break;
                    case "2005":
                        strMessage = "百事達會員編號檔內資料不存在. ";
                        break;
                    case "2006":
                        strMessage = "系統檔資料不存在. ";
                        break;
                    case "2007":
                        strMessage = "補發卡號檔內資料不存在. ";
                        break;
                    case "2008":
                        strMessage = "認同代碼不存在. ";
                        break;
                    case "3001":
                        strMessage = "寫入製卡檔資料重覆. ";
                        break;
                    case "3002":
                        strMessage = "掛補卡號維護資料重覆寫入. ";
                        break;
                    case "3003":
                        strMessage = "製卡姓名維護資料重覆寫入. ";
                        break;
                    case "3004":
                        strMessage = "製卡有效期維護資料重覆寫入. ";
                        break;
                    case "3005":
                        strMessage = "MAINTLOG檔資料重覆寫入. ";
                        break;
                    case "4001":
                        strMessage = "卡片狀態不為1OR2. ";
                        break;
                    case "4002":
                        strMessage = "已完成掛補STATUS=1. ";
                        break;
                    case "4003":
                        strMessage = "已完成掛補STATUS=2. ";
                        break;
                    case "4004":
                        strMessage = "卡片檔ＡＣＴＩＯＮ不等於０. ";
                        break;
                    case "4005":
                        strMessage = "卡片BLOCK需為LORX. ";
                        break;
                    case "4006":
                        strMessage = "卡片有催收狀態. ";
                        break;
                    case "4007":
                        strMessage = "卡片非相片卡，製卡樣式不得為01. ";
                        break;
                    case "4008":
                        strMessage = "出帳單日不可做掛失. ";
                        break;
                    case "4009":
                        strMessage = "卡片BLOCKCODE不符. ";
                        break;
                    case "4010":
                        strMessage = "此卡已作過補發. ";
                        break;
                    case "4011":
                        strMessage = "此卡未改過密碼不能補發密碼函. ";
                        break;
                    case "4012":
                        strMessage = "百視達會員編號須為空白. ";
                        break;
                    case "4013":
                        strMessage = "百視達會員編號有誤. ";
                        break;
                    case "4017":
                        strMessage = "聯名團體解約不可掛毀補請執行１２９６. ";
                        break;
                    case "8001":
                        strMessage = "卡片檔未開啟. ";
                        break;
                    case "8002":
                        strMessage = "製卡檔未開啟. ";
                        break;
                    case "8003":
                        strMessage = "卡人檔未開啟資料不存在. ";
                        break;
                    case "8004":
                        strMessage = "百事達會員檔未開啟. ";
                        break;
                    case "8005":
                        strMessage = "系統檔未開啟資料. ";
                        break;
                    case "8006":
                        strMessage = "ＭＡＩＮＴ檔未開啟. ";
                        break;
                    case "8007":
                        strMessage = "補發卡號檔未開啟. ";
                        break;
                    case "8008":
                        strMessage = "偽卡檔未開啟. ";
                        break;
                    case "8009":
                        strMessage = "開卡檔未開啟. ";
                        break;
                    case "8010":
                        strMessage = "CPCMA未開啟. ";
                        break;
                    case "8011":
                        strMessage = "認同代碼檔未開啟. ";
                        break;
                    case "8012":
                        strMessage = "MTC2 檔未開啟. ";
                        break;
                    case "8013":
                        strMessage = "MTC1 檔未開啟. ";
                        break;
                    case "9001":
                        strMessage = "製卡檔LENGERR. ";
                        break;
                    case "9002":
                        strMessage = "卡人檔LENGERR. ";
                        break;
                    case "9003":
                        strMessage = "寫入百視達檔ERROR. ";
                        break;
                    case "9999":
                        strMessage = "系統異常. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCDK:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "OK. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "8001":
                        strMessage = "電話檢核錯誤. ";
                        break;
                    case "9999":
                        strMessage = " NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCHQ:
            case HtgType.P4_JCHQ:
                switch (strMessageCode)
                {
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCHR:
            case HtgType.P4_JCHR:
                switch (strMessageCode)
                {
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4L_LGOR:
            case HtgType.P4L_LGAT:
                switch (strMessageCode)
                {
                    case "000000":
                        strMessage = "執行結果正確，成功返回";
                        break;
                    case "900101":
                        strMessage = "若NOTOPEN或DISABLED";
                        break;
                    case "900102":
                        strMessage = "NOTFND";
                        break;
                    case "900103":
                        strMessage = "讀檔異常";
                        break;
                    case "900104":
                        strMessage = "讀檔異常";
                        break;
                    case "900105":
                        strMessage = "讀檔異常";
                        break;
                    case "900106":
                        strMessage = "讀檔異常";
                        break;
                    case "900107":
                        strMessage = "讀檔異常";
                        break;
                    case "900108":
                        strMessage = "讀檔異常";
                        break;
                    case "900109":
                        strMessage = "讀檔異常";
                        break;
                    case "900201":
                        strMessage = "NOTFND";
                        break;
                    case "900202":
                        strMessage = "若NOTOPEN或DISABLED或其他讀檔錯誤";
                        break;
                    case "900203":
                        strMessage = "功能碼為A(新增時)，檔案資料已存在";
                        break;
                    case "900204":
                        strMessage = "寫檔或覆寫檔案錯誤";
                        break;
                    case "900301":
                        strMessage = "NOTFND";
                        break;
                    case "900302":
                        strMessage = "NOTOPEN或DISABLED或其他讀檔錯誤";
                        break;
                    case "999001":
                        strMessage = "程式錯誤";
                        break;
                    case "999901":
                        strMessage = "系統異常";
                        break;
                    case "000101":
                        strMessage = "ORGANISATION需為數值且非0、非999";
                        break;
                    case "000201":
                        strMessage = "MERCHANT需為數值且非0、非999999999";
                        break;
                    case "000301":
                        strMessage = "CARDTYPE需為數值且非0";
                        break;
                    case "000401":
                        strMessage = "PRODCODE需為數值且非0";
                        break;
                    case "000501":
                        strMessage = "功能碼需為A、C、I";
                        break;
                    case "000601":
                        strMessage = "PROGRAMID需為數值";
                        break;
                    case "000701":
                        strMessage = "MERCHANT%需為數值且大於0且小於10000";
                        break;
                    case "000801":
                        strMessage = "REDEMPTIONLIMIT%需為數值且大於0且小於10000";
                        break;
                    case "000901":
                        strMessage = "C/HPOINTS需為數值";
                        break;
                    case "001001":
                        strMessage = "C/HAMOUNT需為數值";
                        break;
                    case "001101":
                        strMessage = "BIRTHMONTHFUNCTION需為00或01或02或03";
                        break;
                    case "001201":
                        strMessage = "BIRTHMONTHFUNCTION起始日期需為數值";
                        break;
                    case "001202":
                        strMessage = "BIRTHMONTHFUNCTION起始日期需符合日期格式";
                        break;
                    case "001301":
                        strMessage = "BIRTHMONTHFUNCTION結束日期需為數值";
                        break;
                    case "001302":
                        strMessage = "BIRTHMONTHFUNCTION結束日期需符合日期格式";
                        break;
                    case "001303":
                        strMessage = "BIRTHMONTHFUNCTION起始日期不可大於結束日期";
                        break;
                    case "001401":
                        strMessage = "BIRTHMONTHFUNCTIONLIMIT%需為數值且大於0且小於10000";
                        break;
                    case "001501":
                        strMessage = "BIRTHMONTHFUNCTIONC/HPOINTS需為數值";
                        break;
                    case "001601":
                        strMessage = "BIRTHMONTHFUNCTIONC/HAMOUNT需為數值";
                        break;
                    case "001701":
                        strMessage = "USREXITFUNCTION需為00或01或02或03";
                        break;
                    case "001801":
                        strMessage = "USREXITCYCLECODE(第一碼需為'M'OR'W'OR'D')或為'000000'";
                        break;
                    case "001901":
                        strMessage = "USREXITFUNCTION起始日期需為數值";
                        break;
                    case "001902":
                        strMessage = "USREXITFUNCTION起始日期需符合日期格式";
                        break;
                    case "002001":
                        strMessage = "USREXITFUNCTION結束日期需為數值";
                        break;
                    case "002002":
                        strMessage = "USREXITFUNCTION結束日期需符合日期格式";
                        break;
                    case "002003":
                        strMessage = "USREXITFUNCTION起始日期不可大於結束日期";
                        break;
                    case "002101":
                        strMessage = "USREXITFUNCTIONLIMIT%需為數值且大於0且小於10000";
                        break;
                    case "002201":
                        strMessage = "USREXITFUNCTIONC/HPOINTS需為數值";
                        break;
                    case "002301":
                        strMessage = "USREXITFUNCTIONC/HAMOUNT需為數值";
                        break;
                    case "002701":
                        strMessage = "程式代碼需為CLGU110";
                        break;
                    case "002801":
                        strMessage = "USERID不可為空白或LOW-VALUES";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
                    #endregion

            default:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
        }
        return strMessage;
    }

    //*作者 趙呂梁
    //*創建日期：2009/12/22
    //*修改日期：2009/12/22 
    /// <summary>
    /// 得到操作電文提示信息
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="strType">電文檢核類型</param>
    /// <returns>提示信息</returns>
    public static string GetHtgMessage(HtgType type, string strType)
    {
        string strCondition = "";//*操作電文的環境
        string strWorkType = "";//*操作電文的類型

        switch (strType)
        {
            case "1":
                strWorkType = "查詢";
                strCondition = "P4";
                break;

            case "11":
                strWorkType = "查詢";
                strCondition = "P4A";
                break;

            case "12":
                strWorkType = "查詢";
                strCondition = "P4D";
                break;

            case "2":
                strWorkType = "異動";
                strCondition = "P4";
                break;

            case "21":
                strWorkType = "異動";
                strCondition = "P4A";
                break;

            case "22":
                strWorkType = "異動";
                strCondition = "P4D";
                break;
        }
        return strWorkType + type.ToString() + strCondition;
    }

    //*作者 趙呂梁
    //*創建日期：2009/12/22
    //*修改日期：2009/12/22
    /// <summary>
    /// 取得電文參數
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="strTemp">傳入參數</param>
    /// <returns>返回的字符串</returns>
    public static string GetStr(HtgType type, string strTemp)
    {
        switch (type)
        {
            case HtgType.P4_PCTI:
                #region P4_PCTI
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_PCTI_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4D_PCTI:
                #region P4D_PCTI
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4D_PCTI_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCF7:
                #region P4_JCF7
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCF7_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCAW:
                #region P4_JCAW
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAW_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCAX:
                #region P4_JCAX
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAX_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCHO:
                #region P4_JCHO
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHO_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4A_JCGQ:
                #region P4A_JCGQ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCGQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCGQ:
                #region P4_JCGQ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCGQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4A_JCGX:
                #region P4A_JCGX
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCGX_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCGX:
                #region P4_JCGX
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCGX_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCIL:
                #region P4_JCIL
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCIL_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4A_JCGR:
                #region P4A_JCGR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCGR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCIJ:
                #region P4_JCIJ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCIJ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCAA:
                #region P4_JCAA
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAA_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCEM:
                #region P4_JCEM
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCEM_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4D_JCF6:
                #region P4D_JCF6
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4D_JCF6_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCF6:
                #region P4_JCF6
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCF6_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCDK:
                #region P4_JCDK
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCDK_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCHR:
                #region P4_JCHR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4A_JCHR:
                #region P4A_JCHR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCHR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4A_JCHQ:
                #region P4A_JCHR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCHQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4_JCHQ:
                #region P4_JCHQ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4A_JCCA:
                #region P4A_JCCA
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCCA_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion

            case HtgType.P4L_LGOR:
                #region P4L_LGOR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4L_LGOR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                    default:
                        break;
                }
                break;
                #endregion P4L_LGOR

            case HtgType.P4L_LGAT:
                #region P4L_LGAT
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4L_LGAT_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                    default:
                        break;
                }
                break;
                #endregion P4L_LGAT

            default:
                #region
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return "FALSE";

                    case "AUTHONLINE":
                        return "FALSE";
                    default:
                        return "";
                }
                #endregion
        }
        return "";
    }

    /// <summary>
    /// 添加上傳主機SESSION信息
    /// </summary>
    /// <param name="htInput">上傳主機HashTable</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="type">電文類型</param>
    public static void AddSession(Hashtable htInput, EntityAGENT_INFO eAgentInfo, HtgType type)
    {
        if (htInput.ContainsKey("userId"))
        {
            htInput["userId"] = eAgentInfo.agent_id;
        }
        else
        {
            htInput.Add("userId", eAgentInfo.agent_id);
        }

        if (htInput.ContainsKey("passWord"))
        {
            htInput["passWord"] = eAgentInfo.agent_pwd;
        }
        else
        {
            htInput.Add("passWord", eAgentInfo.agent_pwd);
        }


        if (htInput.ContainsKey("racfId"))
        {
            htInput["racfId"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add("racfId", eAgentInfo.agent_id_racf);
        }

        if (htInput.ContainsKey("racfPassWord"))
        {
            htInput["racfPassWord"] = eAgentInfo.agent_id_racf_pwd;
        }
        else
        {
            htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        }

        if (htInput.ContainsKey("USER_ID"))
        {
            htInput["USER_ID"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add(GetStr(type, "USER_ID"), eAgentInfo.agent_id_racf);
        }


        if (HttpContext.Current != null)//*判斷是JOB還是網頁中上傳主機
        {
            if (HttpContext.Current.Session["sessionId"] != null)
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = HttpContext.Current.Session["sessionId"].ToString().Trim();
                }
                else
                {
                    htInput.Add("sessionId", HttpContext.Current.Session["sessionId"].ToString().Trim());
                }
            }
            else
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = "";
                }
                else
                {
                    htInput.Add("sessionId", "");

                }
            }
        }
        else
        {
            if (!htInput.ContainsKey("sessionId"))
            {
                htInput.Add("sessionId", "");
            }
        }
    }


    /// <summary>
    /// 添加上傳主機SESSION信息
    /// </summary>
    /// <param name="htInput">上傳主機HashTable</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="type">電文類型</param>
    public static void AddSessionJob(Hashtable htInput, EntityAGENT_INFO eAgentInfo, HtgType type, string strSession)
    {
        if (htInput.ContainsKey("userId"))
        {
            htInput["userId"] = eAgentInfo.agent_id;
        }
        else
        {
            htInput.Add("userId", eAgentInfo.agent_id);
        }

        if (htInput.ContainsKey("passWord"))
        {
            htInput["passWord"] = eAgentInfo.agent_pwd;
        }
        else
        {
            htInput.Add("passWord", eAgentInfo.agent_pwd);
        }


        if (htInput.ContainsKey("racfId"))
        {
            htInput["racfId"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add("racfId", eAgentInfo.agent_id_racf);
        }

        if (htInput.ContainsKey("racfPassWord"))
        {
            htInput["racfPassWord"] = eAgentInfo.agent_id_racf_pwd;
        }
        else
        {
            htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        }

        if (htInput.ContainsKey("USER_ID"))
        {
            htInput["USER_ID"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add(GetStr(type, "USER_ID"), eAgentInfo.agent_id_racf);
        }


        if (HttpContext.Current != null)//*判斷是JOB還是網頁中上傳主機
        {
            if (HttpContext.Current.Session["sessionId"] != null)
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = HttpContext.Current.Session["sessionId"].ToString().Trim();
                }
                else
                {
                    htInput.Add("sessionId", HttpContext.Current.Session["sessionId"].ToString().Trim());
                }
            }
            else
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = strSession;
                }
                else
                {
                    htInput.Add("sessionId", strSession);

                }
            }
        }
        else
        {
            if (!htInput.ContainsKey("sessionId"))
            {
                htInput.Add("sessionId", "");
            }
        }
    }

    /// <summary>
    /// 清空主機Session
    /// </summary>
    public static bool ClearHtgSession()
    {
        //string strMsg = "";
        //string strIsOnline = UtilHelper.GetAppSettings("AUTH_IsOnLine");
        //try
        //{
        //    //* 取得Session中存的主機SessionID
        //    string strSessionID = (HttpContext.Current.Session["sessionId"] + "").Trim();
        //    if (!string.IsNullOrEmpty(strSessionID))
        //    {
        //        //* 如果不爲空.發送主機電文,關掉
        //        HTGCommunicator hc = new HTGCommunicator();
        //        hc.SessionId = strSessionID;
        //        hc.CloseSession(ref strMsg, strIsOnline);
        //        //* 清空Session中的主機SessionID
        //        //System.Web.HttpContext.Current.Session["sessionId"] = "";
        //        HttpContext.Current.Session["sessionId"] = null;
        //    }

        //}
        //catch
        //{
        //    Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
        //    return false;
        //}
        return true;
    }



    /// <summary>
    /// 清空主機Session
    /// </summary>
    public static bool ClearHtgSessionJob(ref string SessionID, String jobid = "")
    {
        string strMsg = "";
        string strIsOnline = UtilHelper.GetAppSettings("AUTH_IsOnLine");
        try
        {
            //* 取得Session中存的主機SessionID
            string strSessionID = SessionID;
            if (!string.IsNullOrEmpty(strSessionID))
            {
                //* 如果不爲空.發送主機電文,關掉
                HTGCommunicator hc = new HTGCommunicator(jobid);
                hc.SessionId = strSessionID;
                hc.CloseSession(ref strMsg, strIsOnline);
                //* 清空Session中的主機SessionID
                SessionID = "";

            }
        }
        catch
        {
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return false;
        }
        return true;
    }


    /// <summary>
    /// 依據JCF6組合PCTI
    /// </summary>
    public static void ChangeJCF6toPCTI(Hashtable htInput, Hashtable output, ArrayList arrayName)
    {

        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;

                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*ID
                    GetNewHashTable(output, "ID_DATA", "822" + strValue);
                    break;

                case "CITY": //*城市名
                    GetNewHashTable(output, "CITY", strValue);
                    break;

                case "ADDR_1"://*帳單地址第一段
                    GetNewHashTable(output, "ADDR_LINE_1", strValue);
                    break;

                case "ADDR_2"://*帳單地址第二段
                    GetNewHashTable(output, "ADDR_LINE_2", strValue);
                    break;

                case "EMPLOYER"://*公司名稱
                    GetNewHashTable(output, "OWNER", strValue);
                    break;

                case "HOME_PHONE"://*住家電話
                    GetNewHashTable(output, "TELEPHONE", strValue);
                    break;

                case "OFFICE_PHONE"://*公司電話
                    GetNewHashTable(output, "WORK_PHONE", strValue);
                    break;

                case "MANAGE_ZIP_CODE":  //*管理郵區
                    GetNewHashTable(output, "MAIL_IND", strValue);
                    break;
                case "ZIP"://*郵遞區號
                    GetNewHashTable(output, "ZIP", strValue);
                    break;

                case "MEMO_1"://*註記一
                    GetNewHashTable(output, "MEMO_1", strValue);
                    break;

                case "MEMO_2"://*註記二
                    GetNewHashTable(output, "MEMO_2", strValue);
                    break;

                case "BIRTH_DATE"://*生日
                    GetNewHashTable(output, "BIRTH_DATE", strValue);
                    break;

                case "NAME_1"://*姓名
                    GetNewHashTable(output, "NAME_LINE_1", strValue);
                    break;

                case "NAME_1_2"://*別名
                    GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "OFF_PHONE_FLAG"://*電子帳單
                    GetNewHashTable(output, "MAIL", strValue);
                    break;

                case "CO_OWNER"://*扣繳帳號第一段+第二段
                    GetNewHashTable(output, "BK_ID_AC", strValue);
                    break;

                case "CO_TAX_ID_TYPE"://*繳款狀況
                    GetNewHashTable(output, "LAST_CR_LINE_IND", strValue);
                    break;

                case "DD_ID"://*帳戶ID
                    GetNewHashTable(output, "DD_ID", strValue);
                    break;

                case "BILLING_CYCLE"://*帳單週期
                    GetNewHashTable(output, "BILLING_CYCLE", strValue);
                    break;

                case "EU_CUSTOMER_CLASS"://*學歷
                    GetNewHashTable(output, "EDUCATION_CODE", strValue);
                    break;

                case "GRADUATE_YYMM"://*畢業西元年月
                    GetNewHashTable(output, "GRADUATION_DATE", strValue);
                    break;

                case "EU_NBR_OF_DEPS"://*族群碼
                    GetNewHashTable(output, "EU_NBR_OF_DEPS", strValue);
                    break;

                default://*族群碼
                    break;

            }
        }
    }

    /// <summary>
    /// 依據JCDH組合PCTI
    /// </summary>
    public static void ChangeJCF7toPCTI(Hashtable htInput, Hashtable output, ArrayList arrayName, string strType)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*ID
                    GetNewHashTable(output, "CARD_DATA", "822" + strType + strValue);
                    break;

                case "PYMT_FLAG"://*繳款方式
                    GetNewHashTable(output, "PAYMENT_TYPE", strValue);
                    break;

                case "FIXED_PYMT_AMNT"://*固定繳款額
                    GetNewHashTable(output, "FIXED_PAYMENT", strValue);
                    break;

                case "CURR_DUE"://*本期應繳
                    GetNewHashTable(output, "CURR", strValue);
                    break;

                case "PAST_DUE"://*預期一個月內
                    GetNewHashTable(output, "XDAY", strValue);
                    break;

                case "30DAYS_DELQ"://*30 DAYS
                    GetNewHashTable(output, "30DAY", strValue);
                    break;

                case "60DAYS_DELQ"://*60 DAYS
                    GetNewHashTable(output, "60DAY", strValue);
                    break;

                case "90DAYS_DELQ"://*90 DAYS
                    GetNewHashTable(output, "90DAY", strValue);
                    break;

                case "120DAYS_DELQ"://*120 DAYS
                    GetNewHashTable(output, "120DAY", strValue);
                    break;

                case "150DAYS_DELQ"://*150 DAYS
                    GetNewHashTable(output, "150DAY", strValue);
                    break;

                case "180DAYS_DELQ"://*180 DAYS
                    GetNewHashTable(output, "180DAY", strValue);
                    break;

                case "210DAYS_DELQ"://*210 DAYS
                    GetNewHashTable(output, "210DAY", strValue);
                    break;

                case "USER_CODE"://*優惠碼
                    GetNewHashTable(output, "CHPM_CODE", strValue);
                    break;

                case "USER_CODE_2"://*卡人類別
                    GetNewHashTable(output, "CATEGORY", strValue);
                    break;

                case "CHGOFF_STATUS_FLAG"://*BLK CODE
                    GetNewHashTable(output, "STATUS_FLAG", strValue);
                    break;

                case "SHORT_NAME"://*姓名
                    GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "NAME_1":
                    GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "EMBOSSER_NAME_11"://*英文姓名
                    GetNewHashTable(output, "E1", strValue);
                    break;

                case "CARD_EXPIR_DTE"://*卡片有效日
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST1"://*繳款評等1-24
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST2":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;
                case "DELQ_HIST3":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST4":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST5":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST6":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST7":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST8":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST9":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST10":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST11":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST12":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST13":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST14":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST15":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST16":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST17":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST18":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST19":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST20":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST21":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST22":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST23":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST24":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 依據JCDH組合PCTI
    /// </summary>
    public static void ChangeJCAAtoPCTI(Hashtable htInput, Hashtable output, ArrayList arrayName, string strType)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*ID
                    GetNewHashTable(output, "CARD_DATA", "822" + strType + strValue);
                    break;

                case "BLOCK_CODE":
                    GetNewHashTable(output, "STATUS_FLAG", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 依據JCEM組合JCAW
    /// </summary>
    public static void ChangeJCEMtoJCAW(Hashtable htInput, Hashtable output, ArrayList arrayName)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*卡號
                    GetNewHashTable(output, "CARD_NO", strValue);
                    break;

                case "CARD_TAKE"://*取卡方式
                    GetNewHashTable(output, "SELF_TAKE", strValue);
                    break;

                case "EMBOSS_NAME"://*製卡英文姓名
                    GetNewHashTable(output, "EMBNAME", strValue);
                    break;

                case "EMBOSS_TYPE"://*製卡樣式
                    GetNewHashTable(output, "EMBTYPE", strValue);
                    break;

                case "MEM_NO"://*會員編號
                    GetNewHashTable(output, "MEMNO", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 依據JCGR組合JCIL
    /// </summary>
    public static void ChangeJCGRtoJCIL(Hashtable htInput, Hashtable output, ArrayList arrayName)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "MERCHANT_NO": //*商店代號
                    GetNewHashTable(output, "MER_NO", strValue);
                    break;

                case "BUSINESS_NAME"://*商店營業名稱
                    GetNewHashTable(output, "MER_NEME", strValue);
                    break;

                case "CONTACT_NAME"://*聯絡人姓名
                    GetNewHashTable(output, "OWNER_NAME", strValue);
                    break;

                case "CONTACT_PHONE_AREA"://*聯絡人電話區域號碼 + 聯絡人電話號碼 + 聯絡人電話分機號碼
                    strValue = htInput["CONTACT_PHONE_AREA"].ToString() + "-" + htInput["CONTACT_PHONE_NO"].ToString() + "-" + htInput["CONTACT_PHONE_EXT"].ToString();
                    GetNewHashTable(output, "CONTACT_TEL", strValue);
                    break;

                case "FAX_AREA"://*聯絡人fax
                    strValue = htInput["FAX_AREA"].ToString() + "-" + htInput["FAX_PHONE_NO"].ToString();
                    GetNewHashTable(output, "CONTACT_FAX", strValue);
                    break;

                case "REAL_CITY"://*商店營業地址1
                    GetNewHashTable(output, "ADDRESS1", strValue);
                    break;

                case "REAL_ADDR1"://*商店營業地址2
                    GetNewHashTable(output, "ADDRESS2", strValue);
                    break;

                case "REAL_ADDR2"://*商店營業地址3
                    GetNewHashTable(output, "ADDRESS3", strValue);
                    break;

                case "DDA_BANK_NAME"://*銀行名稱
                    strValue = htInput["DDA_BANK_NAME"].ToString() + htInput["DDA_BANK_BRANCH"].ToString();
                    GetNewHashTable(output, "BANK_NAME", strValue);
                    break;

                case "DDA_ACCT_NAME"://*戶名
                    GetNewHashTable(output, "ACCT_NEME", strValue);
                    break;

                case "USER_DATA"://*帳號(2)
                    GetNewHashTable(output, "ACCT_NO", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 添加轉化后的HashTable
    /// </summary>
    /// <param name="htOutput">HashTable</param>
    /// <param name="strKey">鍵</param>
    /// <param name="strValue">值</param>
    public static void GetNewHashTable(Hashtable htOutput, string strKey, string strValue)
    {
        if (htOutput.ContainsKey(strKey))
        {
            htOutput[strKey] = strValue;
        }
        else
        {
            htOutput.Add(strKey, strValue);
        }
    }

    public enum HtgType
    {
        /// <summary>
        /// EXMS 1610
        /// </summary>
        P4_JCIL,

        /// <summary>
        /// EXMS 6015
        /// </summary>
        P4_JCIJ,
        P4A_JCGR,

        /// <summary>
        /// JCDHJCGU019
        /// </summary>
        P4_JCF7,

        /// <summary>
        /// P4_JCF6
        /// </summary>
        P4_JCF6,

        /// <summary>
        /// P4D_JCF6
        /// </summary>
        P4D_JCF6,

        /// <summary>
        /// P4_JCDK
        /// </summary>
        P4_JCDK,

        /// <summary>
        /// PCMC、PCMH
        /// </summary>
        P4_PCTI,
        P4D_PCTI,

        /// <summary>
        /// OASA_P4
        /// </summary>
        P4_JCAX,

        /// <summary>
        /// EXMS_1231_P4
        /// </summary>
        P4_JCAW,

        /// <summary>
        /// EXMS_6063_P4
        /// </summary>
        P4_JCGX,
        P4A_JCGX,

        /// <summary>
        /// EXMS_1255
        /// </summary>
        P4_JCHO,

        /// <summary>
        /// EXMS_6001
        /// </summary>
        P4_JCGQ,
        P4A_JCGQ,

        /// <summary>
        /// P4_JCAA
        /// </summary>
        P4_JCAA,

        /// <summary>
        /// EXMS_1231_P4
        /// </summary>
        P4_JCEM,

        /// <summary>
        /// PCAM_P4_Submit
        /// </summary>
        P4_JCHQ,

        /// <summary>
        /// PCAM_P4A_Submit
        /// </summary>
        P4A_JCHQ,

        /// <summary>
        /// PCMM、PCIM作業P4
        /// </summary>
        P4_JCHR,

        /// <summary>
        /// PCMM、PCIM作業P4A
        /// </summary>
        P4A_JCHR,

        /// <summary>
        /// P4A_JCCA
        /// </summary>
        P4A_JCCA,

        /// <summary>
        /// P4L_LGOR
        /// </summary>
        P4L_LGOR,

        /// <summary>
        /// P4L_LGAT
        /// </summary>
        P4L_LGAT
    }

}