using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Framework.Common.Utility;
using Framework.Common.Logging;
using TIBCO.EMS;
using ESBOrderUp;
using System.IO;
using System.Xml;

/// <summary>
/// ConntoESB 的摘要描述
/// </summary>
public class ConntoESB
{/// <summary>
/// 
/// </summary>
/// <param name="group">ESB連線組別</param>
/// <param name="strXML">電文XML</param>
/// <param name="msgNull"></param>
/// <param name="ConnEndTime"></param>
/// <param name="SendupEndTime"></param>
/// <param name="ReceDownEndTime"></param>
/// <param name="ConnColseEndTime"></param>
/// <param name="Uat"></param>
/// <param name="timeout"></param>
/// <returns></returns>

    /// <summary>
    /// ESB電文處理
    /// </summary>
    /// <param name="esbObj">ESB 電文物件</param>/// 
    /// <param name="group">預設電文組別</param>/// 
    public static string ConnESB(ESBObject esbObj , string group = null)
    {
        //將預計上送的XML轉出並記錄
        string strXML = esbObj.getXML();
        
        //紀錄上送的LOG
        SaveESBLog(strXML.Replace("><", ">\r\n<"), "REQ");

        //抓ReTry 的次數
        int ESBRetry = Convert.ToInt32(UtilHelper.GetAppSettings("ESBRetry").ToString());

        string ConnEndTime = "";//  連線結束時間
        string SendupEndTime = "";//發送上行結束時間
        string ReceDownEndTime = "";//收到下行結束時間
        string ConnColseEndTime = "";//關閉連接結束時間
        string strESBMsg = "";
        bool msgNull = false;
        bool esbtimeout = false; // 紀錄ESB是否Timeout

        #region params
        //第一組
        string ServerUrl = string.Empty;
        string ServerPort = string.Empty;
        string UserName = string.Empty;
        string Password = string.Empty;
        string ESBSendQueueName = string.Empty;
        string ESBReceiveQueueName = string.Empty;
        #endregion
        if (group == "1" || group == null)
        {
            // 第一組
            ServerUrl = UtilHelper.GetAppSettings("ESB_ServerUrl").ToString();
            ServerPort = UtilHelper.GetAppSettings("ESB_ServerPort").ToString();
            UserName = UtilHelper.GetAppSettings("ESB_UserName").ToString();
            Password = UtilHelper.GetAppSettings("ESB_Password").ToString();
            ESBSendQueueName = UtilHelper.GetAppSettings("ESB_SendQueueName").ToString();
            ESBReceiveQueueName = UtilHelper.GetAppSettings("ESB_ReceiveQueueName").ToString();

        }
        if (group == "2")
        {
            // 第二組
            ServerUrl = UtilHelper.GetAppSettings("ESB_ServerUrl_1").ToString();
            ServerPort = UtilHelper.GetAppSettings("ESB_ServerPort_1").ToString();
            UserName = UtilHelper.GetAppSettings("ESB_UserName_1").ToString();
            Password = UtilHelper.GetAppSettings("ESB_Password_1").ToString();
            ESBSendQueueName = UtilHelper.GetAppSettings("ESB_SendQueueName_1").ToString();
            ESBReceiveQueueName = UtilHelper.GetAppSettings("ESB_ReceiveQueueName_1").ToString();
        }
        //當線路1 連線錯誤　& TimeOut msgNull = ture 跑線路2
        msgNull = false;
        string strResult = string.Empty;
        string _url = string.Empty;
        string _messageid = string.Empty;
        string tagValue = string.Empty;
        // ESB 設定Timeout秒數
        int ESBTimeout = Convert.ToInt32(UtilHelper.GetAppSettings("ESBTimeout").ToString());
        _url = "tcp://" + ServerUrl + ":" + ServerPort;
        /* 方法二,直接使用QueueConnectionFactory */

        QueueConnectionFactory factory = null;
        QueueConnection connection = null;
        try
        {
                for (int i = 1; i <= ESBRetry; i++)
                {
                        factory = new TIBCO.EMS.QueueConnectionFactory(_url);
                        connection = factory.CreateQueueConnection(UserName, Password);
                        QueueSession session = connection.CreateQueueSession(false, TIBCO.EMS.Session.AUTO_ACKNOWLEDGE);
                        TIBCO.EMS.Queue queue = session.CreateQueue(ESBSendQueueName);
                        QueueSender qsender = session.CreateSender(queue);
                        /* send messages */
                        TextMessage message = session.CreateTextMessage();
                        message.Text = strXML;
                        //一定要設定要reply的queue,這樣才收得到
                        message.ReplyTo = (TIBCO.EMS.Destination)session.CreateQueue(ESBReceiveQueueName);
                        ConnEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        qsender.Send(message);
                        SendupEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        _messageid = message.MessageID;

                        //receive message
                        String messageselector = null;
                        messageselector = "JMSCorrelationID = '" + _messageid + "'";
                        TIBCO.EMS.Queue receivequeue = session.CreateQueue(ESBReceiveQueueName);
                        QueueReceiver receiver = session.CreateReceiver(receivequeue, messageselector);
                        connection.Start();
                        //set up timeout 
                        TIBCO.EMS.Message msg = receiver.Receive(ESBTimeout * 1000);
                        //確認是否成功連線
                        if (msg == null)
                        {
                            //此狀況無法取得下行電文，StatusCode/RspCode/ErrorCode空值為正常
                            esbObj.ConnStatus = "F";
                            msgNull = true;
                            strResult = "ESB連線錯誤";
                            Logging.Log("電文TimeOut：\r\n", LogLayer.UI);
                        }
                        else
                        {
                            msg.Acknowledge();
                            if (msg is TextMessage)
                            {
                                TextMessage tm = (TextMessage)msg;
                                strResult = tm.Text;
                                //連線結果塞回物件
                                esbObj.getResult(strResult);
                                if (esbObj.StatusCode == "0")
                                {
                                    esbObj.ConnStatus = "S";
                                    //連線成功，紀錄RTN
                                    SaveESBLog(strResult, "RTN");
                                    break;
                        }
                                else
                                {
                                    esbObj.ConnStatus = "F";
                                }
                            }
                            else
                            {
                                strResult = msg.ToString();
                            }
                        }
                
                        Logging.Log(" 發查次數：" + i.ToString() + "；ConnESB("+ _url + ") 電文 Result：" + strResult, LogLayer.UI);
                }
                return strResult;
          }
          catch (Exception ex)
         {
            esbObj.ConnStatus = "F";
            Logging.Log("ESB連線錯誤\r\n" + ex.ToString(), LogLayer.UI);
            return "ESB連線錯誤";
         }
         finally
         {
            if (connection != null)
                connection.Close();
         }
    }

        /// <summary>
        /// 紀錄ESB上下行電文
        /// </summary>
        /// <param name="strESBSerializ">ESB電文內容</param>
        /// <param name="strNameCheckType">REQ、RTN</param>
        public static void SaveESBLog(string strESBSerializ, string strNameCheckType)
        {
            string strMsg = "\r\n[" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "] ---------------- " + strNameCheckType + " --------------------------\r\n";
            strMsg = strMsg + strESBSerializ;
            Logging.Log(strMsg, "Htg", LogState.Info);
        }

    public static string getTagValue(string xmlSource, string tagName)
    {
        string tagValue = string.Empty;
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlSource);
            if (doc.GetElementsByTagName(tagName).Count > 0)
            {
                tagValue = doc.GetElementsByTagName(tagName)[0].InnerText;
            }
            return tagValue;
        }
        catch(Exception ex)
        {
            Logging.Log("無法取得代碼", LogState.Info, LogLayer.UI);
            return tagValue;
        }
    }
}