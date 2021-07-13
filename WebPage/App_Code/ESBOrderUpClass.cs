/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Framework.Common.Utility;
using System.Web.SessionState;
using Framework.Common.Logging;
using com.ctcb.ldap;
using System.Xml;

namespace ESBOrderUp
{
	public class ESBOrderUpClass : ESBObject
	{
		#region 基本參數(取得部門別/單位別、分機用)
		//LDAP的IP
		private static String IP = UtilHelper.GetAppSettings("LDAP_IP");
		//LDAP的端口號
		private static Int32 PORT = Convert.ToInt16(UtilHelper.GetAppSettings("LDAP_PORT"));
		//serviceID(就是AP註冊在LDAP的物件)的DN與密碼
		private static String SID_DN = UtilHelper.GetAppSettings("LDAP_SIDDN");
		private static String SID_PASS = UtilHelper.GetAppSettings("LDAP_SIDPass");
		private static String userID = string.Empty;
		private static String password = string.Empty;
		#endregion
		public ESBOrderUpClass(HttpSessionState session)
		{
			#region 取得部門別/單位別, 分機
			string userPart = string.Empty;
			string userUnit = string.Empty;
			string userExt = string.Empty;
			LdapBasic l = new LdapBasic();
			//****步驟一：ServiceID連線*****************************************
			l.bind(IP, PORT, SID_DN, SID_PASS);
			//****步驟二：驗證User的帳號與密碼*****************************************
			userID = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)session["Agent"]).agent_id;
			password = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)session["Agent"]).agent_pwd;
			String res = l.verifyUserPwd(userID, password);
            String status = (res.Length > 0 ? res.Substring(0, 1) : "");
			switch (status)
			{
				case "0":
					{
						//****步驟三：取得部門別/單位別、分機 *******************
						String[] orgChains = l.getUserOrgChainInfo(userID);
                        if (orgChains.Length == 0)
                        {
							Logging.Log("無法取得部門/單位資訊", LogState.Info, LogLayer.BusinessRule);
						}
                        //取單位
                        if (orgChains.Length > 0)
                        {
							com.ctcb.ldap.Attributes attUnit = l.getOUProperty(orgChains[0], new string[] { "FullName" });
							userUnit = dumpAttributes(attUnit, "FullName")[0];
                        }
						//取部門
						if (orgChains.Length > 1)
						{
							com.ctcb.ldap.Attributes attPart = l.getOUProperty(orgChains[1], new string[] { "FullName" });
							userPart = dumpAttributes(attPart, "FullName")[0];
						}

						com.ctcb.ldap.Attributes attExt = l.getUserProperty(userID, new string[] { "physicalDeliveryOfficeName" });
						String[] userExts = dumpAttributes(attExt, "physicalDeliveryOfficeName");

                        if (userExts.Length == 0) { 
							Logging.Log("無法取得分機號碼", LogState.Info, LogLayer.BusinessRule); 
						}

                        if (userExts.Length > 0)
                        {
                            try
                            {
                                if (!userExts[0].Contains("#"))
                                {
									Logging.Log("無法取得分機號碼", LogState.Info, LogLayer.BusinessRule);
                                }
                                else
                                {
									String[] arrExts = userExts[0].Split('#');
									userExt = arrExts[arrExts.Length - 1];
								}
								
                            }
                            catch (Exception ex)
                            {
								Logging.Log("無法取得分機號碼", LogState.Info, LogLayer.BusinessRule);
                            }
                        }
					}
					break;
				default:
					{
						Logging.Log("無法取得發單人部門/單位別/分機" + res, LogState.Info, LogLayer.BusinessRule);
					}
					break;
			}
			#endregion

			CaseNo = "";
			ErrorMessage = "";
			CASETYPEID = UtilHelper.GetAppSettings("CASETYPEID");
			OBJECTCODE = UtilHelper.GetAppSettings("OBJECTCODE");
			PARENTID = UtilHelper.GetAppSettings("PARENTID");
			ID = UtilHelper.GetAppSettings("takeChangeID");
			MODELID = UtilHelper.GetAppSettings("MODELID");
			DOMOBJECTCODE = UtilHelper.GetAppSettings("DOMOBJECTCODE");
			CDM_C0701_CREATECHANNEL = UtilHelper.GetAppSettings("CDM_C0701_CREATECHANNEL");//發單管道
			CDM_C0701_CREATETYPE = UtilHelper.GetAppSettings("CDM_C0701_CREATETYPE");//發單方式
			CDM_C0701_EMPLOYEEID = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)session["Agent"]).agent_id;//發單員編
			CDM_C0701_EMPLOYEEUNIT = userPart + "\\" + userUnit;//發單人單位+部門
			CDM_C0701_CASECREATOR = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)session["Agent"]).agent_name; // 發單人
			CDM_C0701_EXT = userExt;//發單人分機
			BUSINESSTYPE_ID = UtilHelper.GetAppSettings("BUSINESSTYPE_ID_0");//業務 _0:更改取卡方式/_1:永吉分行卡片自取改郵寄
			CUST_MESSAGETYPE_ID = UtilHelper.GetAppSettings("CUST_MESSAGETYPE_ID_2");//自動訊息通知 _0:Email/_1:簡訊/_2:數位優先決
			workOrder_StatusCode_Tag = "ns1:StatusCode";
			workOrder_RspCode_Tag = "ns2:RspCode";
			workOrder_ErrorCode_Tag = "ns2:ErrorCode";
			workOrder_CaseName_Tag = "ns2:CASENAME";
			workOrder_ErrorMessage_Tag = "ns2:ErrorMessage";
		}
		public string OBJECTCODE { get; set; }
		public string ID { get; set; }
		public string PARENTID { get; set; }
		public string PARENTCODE { get; set; }
		public string SUMMARY { get; set; }
		public string DESCRIPTION { get; set; }
		public string PRIORITY_ID { get; set; }
		public string CASESYSTYPE_ID { get; set; }
		public string DRAFT { get; set; }
		public string CASETYPEID { get; set; }
		public string MODELID { get; set; }
		public string DOMOBJECTCODE { get; set; }
		public string CDM_C0701_CHANGECARD { get; set; }
		public string CDM_C0701_EMERGENCYCARD { get; set; }
		public string CDM_C0701_CHANGECARDADDR { get; set; }
		public string CDM_C0701_CHANGECARDQUOTA { get; set; }
		public string CDM_C0701_DIFFICULTNAME { get; set; }
		public string CDM_C0701_CHANGENAME { get; set; }
		public string CDM_C0701_MAILSEARCH { get; set; }
		public string CDM_C0701_OVERSEASSENDCARD { get; set; }
		public string CDM_C0701_NOTICECSCTODOLIST { get; set; }
		public string CDM_C0701_WITHATTACH { get; set; }
		public string CDM_C0701_OVERSEASSENDCARDNOFEE { get; set; }
		public string CDM_C0701_CREATECHANNEL { get; set; }
		public string CDM_C0701_CREATETYPE { get; set; }
		public string CDM_C0701_EMPLOYEEID { get; set; }
		public string CDM_C0701_EMPLOYEEUNIT { get; set; }
		public string CDM_C0701_CASECREATOR { get; set; }
		public string CDM_C0701_EXT { get; set; }
		public string BUSINESSTYPE_ID { get; set; }
		public string CUST_MESSAGETYPE_ID { get; set; }
		public string CDM_C0701_PID { get; set; }
		public string CDM_C0701_NAME { get; set; }
		public string CDM_C0701_COMPANY { get; set; }
		public string CDM_C0701_EMAIL { get; set; }
		public string CDM_C0701_HOMEPHONE { get; set; }
		public string CDM_C0701_CELLPHONE { get; set; }
		public string CDM_C0701_HOMEADDR { get; set; }
		public string CDM_C0701_COMPANYPHONE { get; set; }
		public string CDM_C0701_CONTACTPERSON { get; set; }
		public string CDM_C0701_CONTACTCELLPHONE { get; set; }
		public string CDM_C0701_CONTACTPHONE { get; set; }
		public string CDM_C0701_CONTACTEMAIL { get; set; }
		public string CDM_C0701_CONTACTREMARK { get; set; }
		public string CDM_C0701_CARDNO { get; set; }
		public string DICT_CUSTOMWORD_ID { get; set; }
		public string ORIG_GETCARDTYPE_ID { get; set; }
		public string NEW_GETCARDTYPE_ID { get; set; }
		public string CDM_C0701_POSTALAREA { get; set; }
		public string CDM_C0701_ADDR1 { get; set; }
		public string CDM_C0701_ADDR2 { get; set; }
		public string CDM_C0701_ADDR3 { get; set; }
		public string CDM_C0701_QUOTAVALUE { get; set; }
		public string CDM_C0701_DIFFICULTNAMENO { get; set; }
		public string CDM_C0701_DIFFICULTNAMEVALUE { get; set; }
		public string CDM_C0701_CHANGENAMEVALUE { get; set; }
		public string POSTTYPE_ID { get; set; }
		public string CDM_C0701_OVERSEASADDR { get; set; }
		public string CDM_C0701_ENGLISHNAME { get; set; }
		public string CDM_C0701_OVERSEASCONTACTPHONE { get; set; }
		public string workOrder_StatusCode_Tag { get; set; }
		public string workOrder_RspCode_Tag { get; set; }
		public string workOrder_ErrorCode_Tag { get; set; }
		public string workOrder_CaseName_Tag { get; set; }
		public string workOrder_ErrorMessage_Tag { get; set; }


		public string CaseNo { get; set; }

		public override string getXML()
		{
			string strXML = string.Empty;
			DateTime curTime = DateTime.Now;
			string TransactionID = "CSIP" + curTime.ToString("yyyyMMdd") + curTime.ToString("HHmmss");
			string rqTimeStamp = curTime.ToString("yyyy-MM-dd" + "T" + "HH:mm:ss.ff" + "K");
			string PRIORITY_ID = UtilHelper.GetAppSettings("PRIORITY_ID");
			string SUMMARY = "更改取卡方式";
			string CASESYSTYPE_ID = UtilHelper.GetAppSettings("CASESYSTYPE_ID");
            //if (this.CDM_C0701_CHANGECARDADDR == "1")
            //{
            //    SUMMARY = "更改取卡方式-更改卡址";
            //}
            //if (this.CDM_C0701_NOTICECSCTODOLIST == "1")
            //{
            //    SUMMARY = "更改取卡方式-通知客服";
            //}

            try
			{
				strXML = string.Format("<?xml version=\"1.0\" encoding=\"UTF - 8\"?><ns5:ServiceEnvelope xmlns:ns5=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceEnvelope\" xmlns=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceHeader\" xmlns:ns2=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceBody\" xmlns:ns4=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceError\" xmlns:ns3=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/Common\"><ServiceHeader><ServiceName>csEMFSWorkOrdrAdd</ServiceName><ServiceVersion>01</ServiceVersion><SourceID>CSIP</SourceID><TransactionID>{55}</TransactionID><RqTimestamp>{56}</RqTimestamp></ServiceHeader><ns2:ServiceBody><ns99:csEMFSWorkOrdrAddRq xmlns:ns99=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/csEMFSWorkOrdrAddRq/01\"><ns99:REQHDR><ns99:TrnNum>{55}</ns99:TrnNum></ns99:REQHDR><ns99:REQBDY><ns99:InputXML>&lt;CustomData&gt;&lt;Attributes&gt;&lt;Object ObjectCode=&quot;CASE&quot;&gt;&lt;Item&gt;&lt;OBJECTCODE&gt;&lt;![CDATA[CASE]]&gt;&lt;/OBJECTCODE&gt;&lt;ID&gt;{4}&lt;/ID&gt;&lt;PARENTID&gt;{2}&lt;/PARENTID&gt;&lt;PARENTCODE&gt;{3}&lt;/PARENTCODE&gt;&lt;SUMMARY&gt;&lt;![CDATA[{58}]]&gt;&lt;/SUMMARY&gt;&lt;DESCRIPTION&gt;&lt;/DESCRIPTION&gt;&lt;PRIORITY_ID&gt;{57}&lt;/PRIORITY_ID&gt;&lt;CASESYSTYPE_ID&gt;{59}&lt;/CASESYSTYPE_ID&gt;&lt;DRAFT&gt;0&lt;/DRAFT&gt;&lt;/Item&gt;&lt;/Object&gt;&lt;Object ObjectCode=&quot;{1}&quot;&gt;&lt;Item&gt;&lt;CASETYPEID&gt;{0}&lt;/CASETYPEID&gt;&lt;OBJECTCODE&gt;&lt;![CDATA[{1}]]&gt;&lt;/OBJECTCODE&gt;&lt;PARENTID&gt;{2}&lt;/PARENTID&gt;&lt;ID&gt;{4}&lt;/ID&gt;&lt;PARENTCODE&gt;{3}&lt;/PARENTCODE&gt;&lt;MODELID&gt;{5}&lt;/MODELID&gt;&lt;DOMOBJECTCODE&gt;&lt;![CDATA[{6}]]&gt;&lt;/DOMOBJECTCODE&gt;&lt;CDM_C0701_CHANGECARD&gt;{7}&lt;/CDM_C0701_CHANGECARD&gt;&lt;CDM_C0701_EMERGENCYCARD&gt;{8}&lt;/CDM_C0701_EMERGENCYCARD&gt;&lt;CDM_C0701_CHANGECARDADDR&gt;{9}&lt;/CDM_C0701_CHANGECARDADDR&gt;&lt;CDM_C0701_CHANGECARDQUOTA&gt;{10}&lt;/CDM_C0701_CHANGECARDQUOTA&gt;&lt;CDM_C0701_DIFFICULTNAME&gt;{11}&lt;/CDM_C0701_DIFFICULTNAME&gt;&lt;CDM_C0701_CHANGENAME&gt;{12}&lt;/CDM_C0701_CHANGENAME&gt;&lt;CDM_C0701_MAILSEARCH&gt;{13}&lt;/CDM_C0701_MAILSEARCH&gt;&lt;CDM_C0701_OVERSEASSENDCARD&gt;{14}&lt;/CDM_C0701_OVERSEASSENDCARD&gt;&lt;CDM_C0701_NOTICECSCTODOLIST&gt;{15}&lt;/CDM_C0701_NOTICECSCTODOLIST&gt;&lt;CDM_C0701_WITHATTACH&gt;{17}&lt;/CDM_C0701_WITHATTACH&gt;&lt;CDM_C0701_OVERSEASSENDCARDNOFEE&gt;{16}&lt;/CDM_C0701_OVERSEASSENDCARDNOFEE&gt;&lt;CDM_C0701_CREATECHANNEL&gt;&lt;![CDATA[{18}]]&gt;&lt;/CDM_C0701_CREATECHANNEL&gt;&lt;CDM_C0701_CREATETYPE&gt;&lt;![CDATA[{19}]]&gt;&lt;/CDM_C0701_CREATETYPE&gt;&lt;CDM_C0701_EMPLOYEEID&gt;&lt;![CDATA[{20}]]&gt;&lt;/CDM_C0701_EMPLOYEEID&gt;&lt;CDM_C0701_EMPLOYEEUNIT&gt;&lt;![CDATA[{22}]]&gt;&lt;/CDM_C0701_EMPLOYEEUNIT&gt;&lt;CDM_C0701_CASECREATOR&gt;&lt;![CDATA[{21}]]&gt;&lt;/CDM_C0701_CASECREATOR&gt;&lt;CDM_C0701_EXT&gt;&lt;![CDATA[{23}]]&gt;&lt;/CDM_C0701_EXT&gt;&lt;BUSINESSTYPE_ID&gt;{52}&lt;/BUSINESSTYPE_ID&gt;&lt;CUST_MESSAGETYPE_ID&gt;{53}&lt;/CUST_MESSAGETYPE_ID&gt;&lt;CDM_C0701_PID&gt;&lt;![CDATA[{24}]]&gt;&lt;/CDM_C0701_PID&gt;&lt;CDM_C0701_NAME&gt;&lt;![CDATA[{25}]]&gt;&lt;/CDM_C0701_NAME&gt;&lt;CDM_C0701_COMPANY&gt;&lt;![CDATA[{26}]]&gt;&lt;/CDM_C0701_COMPANY&gt;&lt;CDM_C0701_EMAIL&gt;&lt;![CDATA[{27}]]&gt;&lt;/CDM_C0701_EMAIL&gt;&lt;CDM_C0701_HOMEPHONE&gt;&lt;![CDATA[{28}]]&gt;&lt;/CDM_C0701_HOMEPHONE&gt;&lt;CDM_C0701_CELLPHONE&gt;{29}&lt;/CDM_C0701_CELLPHONE&gt;&lt;CDM_C0701_HOMEADDR&gt;&lt;![CDATA[{30}]]&gt;&lt;/CDM_C0701_HOMEADDR&gt;&lt;CDM_C0701_COMPANYPHONE&gt;&lt;![CDATA[{31}]]&gt;&lt;/CDM_C0701_COMPANYPHONE&gt;&lt;CDM_C0701_CONTACTPERSON&gt;&lt;![CDATA[{32}]]&gt;&lt;/CDM_C0701_CONTACTPERSON&gt;&lt;CDM_C0701_CONTACTCELLPHONE&gt;{33}&lt;/CDM_C0701_CONTACTCELLPHONE&gt;&lt;CDM_C0701_CONTACTPHONE&gt;&lt;![CDATA[{34}]]&gt;&lt;/CDM_C0701_CONTACTPHONE&gt;&lt;CDM_C0701_CONTACTEMAIL&gt;&lt;![CDATA[{35}]]&gt;&lt;/CDM_C0701_CONTACTEMAIL&gt;&lt;CDM_C0701_CONTACTREMARK&gt;&lt;![CDATA[{36}]]&gt;&lt;/CDM_C0701_CONTACTREMARK&gt;&lt;CDM_C0701_CARDNO&gt;{37}&lt;/CDM_C0701_CARDNO&gt;&lt;DICT_CUSTOMWORD_ID&gt;{49}&lt;/DICT_CUSTOMWORD_ID&gt;&lt;ORIG_GETCARDTYPE_ID&gt;{50}&lt;/ORIG_GETCARDTYPE_ID&gt;&lt;NEW_GETCARDTYPE_ID&gt;{51}&lt;/NEW_GETCARDTYPE_ID&gt;&lt;CDM_C0701_POSTALAREA&gt;{38}&lt;/CDM_C0701_POSTALAREA&gt;&lt;CDM_C0701_ADDR1&gt;&lt;![CDATA[{39}]]&gt;&lt;/CDM_C0701_ADDR1&gt;&lt;CDM_C0701_ADDR2&gt;&lt;![CDATA[{40}]]&gt;&lt;/CDM_C0701_ADDR2&gt;&lt;CDM_C0701_ADDR3&gt;&lt;![CDATA[{41}]]&gt;&lt;/CDM_C0701_ADDR3&gt;&lt;CDM_C0701_QUOTAVALUE&gt;{42}&lt;/CDM_C0701_QUOTAVALUE&gt;&lt;CDM_C0701_DIFFICULTNAMENO&gt;{43}&lt;/CDM_C0701_DIFFICULTNAMENO&gt;&lt;CDM_C0701_DIFFICULTNAMEVALUE&gt;&lt;![CDATA[{44}]]&gt;&lt;/CDM_C0701_DIFFICULTNAMEVALUE&gt;&lt;CDM_C0701_CHANGENAMEVALUE&gt;&lt;![CDATA[{45}]]&gt;&lt;/CDM_C0701_CHANGENAMEVALUE&gt;&lt;POSTTYPE_ID&gt;{54}&lt;/POSTTYPE_ID&gt;&lt;CDM_C0701_OVERSEASADDR&gt;&lt;![CDATA[{46}]]&gt;&lt;/CDM_C0701_OVERSEASADDR&gt;&lt;CDM_C0701_ENGLISHNAME&gt;&lt;![CDATA[{47}]]&gt;&lt;/CDM_C0701_ENGLISHNAME&gt;&lt;CDM_C0701_OVERSEASCONTACTPHONE&gt;{48}&lt;/CDM_C0701_OVERSEASCONTACTPHONE&gt;&lt;/Item&gt;&lt;/Object&gt;&lt;/Attributes&gt;&lt;/CustomData&gt;</ns99:InputXML><ns99:Username>CSIP0001</ns99:Username></ns99:REQBDY></ns99:csEMFSWorkOrdrAddRq></ns2:ServiceBody></ns5:ServiceEnvelope>", this.CASETYPEID, this.OBJECTCODE, this.PARENTID, this.PARENTCODE, this.ID, this.MODELID, this.DOMOBJECTCODE, this.CDM_C0701_CHANGECARD, this.CDM_C0701_EMERGENCYCARD, this.CDM_C0701_CHANGECARDADDR, this.CDM_C0701_CHANGECARDQUOTA, this.CDM_C0701_DIFFICULTNAME, this.CDM_C0701_CHANGENAME, this.CDM_C0701_MAILSEARCH, this.CDM_C0701_OVERSEASSENDCARD, this.CDM_C0701_NOTICECSCTODOLIST, this.CDM_C0701_OVERSEASSENDCARDNOFEE, this.CDM_C0701_WITHATTACH, this.CDM_C0701_CREATECHANNEL, this.CDM_C0701_CREATETYPE, this.CDM_C0701_EMPLOYEEID, this.CDM_C0701_CASECREATOR, this.CDM_C0701_EMPLOYEEUNIT, this.CDM_C0701_EXT, this.CDM_C0701_PID, this.CDM_C0701_NAME, this.CDM_C0701_COMPANY, this.CDM_C0701_EMAIL, this.CDM_C0701_HOMEPHONE, this.CDM_C0701_CELLPHONE, this.CDM_C0701_HOMEADDR, this.CDM_C0701_COMPANYPHONE, this.CDM_C0701_CONTACTPERSON, this.CDM_C0701_CONTACTCELLPHONE, this.CDM_C0701_CONTACTPHONE, this.CDM_C0701_CONTACTEMAIL, this.CDM_C0701_CONTACTREMARK, this.CDM_C0701_CARDNO, this.CDM_C0701_POSTALAREA, this.CDM_C0701_ADDR1, this.CDM_C0701_ADDR2, this.CDM_C0701_ADDR3, this.CDM_C0701_QUOTAVALUE, this.CDM_C0701_DIFFICULTNAMENO, this.CDM_C0701_DIFFICULTNAMEVALUE, this.CDM_C0701_CHANGENAMEVALUE, this.CDM_C0701_OVERSEASADDR, this.CDM_C0701_ENGLISHNAME, this.CDM_C0701_OVERSEASCONTACTPHONE, this.DICT_CUSTOMWORD_ID, this.ORIG_GETCARDTYPE_ID, this.NEW_GETCARDTYPE_ID, this.BUSINESSTYPE_ID, this.CUST_MESSAGETYPE_ID, this.POSTTYPE_ID, TransactionID, rqTimeStamp, PRIORITY_ID, SUMMARY, CASESYSTYPE_ID);
				return strXML;
			}
			catch (Exception ex)
			{
				Logging.Log(ex);
				return strXML;
			}
		}

		public override bool CheckResult(string strResult)
		{
			bool result = false;
			string RspCode = "";

			if (strResult != "")
			{
				//中信主機回應 RspCode = -1 代表正確
				RspCode = strResult.Split(new string[] { "<ns2:RspCode>" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "</ns2:RspCode>" }, StringSplitOptions.RemoveEmptyEntries)[0];
			}

			if (RspCode == "-1")
				result = true;

			
			return result;
		}


		public override void getResult(string strResult)
		{
			this.CaseNo = "";
			this.ErrorMessage = "";
			this.StatusCode = "";
			this.RspCode = "";
			this.ErrorCode = "";

			//取得工單編號
			CaseNo = getTagValue(strResult, this.workOrder_CaseName_Tag);

			//取得錯誤訊息
			ErrorMessage = getTagValue(strResult, this.workOrder_ErrorMessage_Tag);

			//取得StatusCode
			StatusCode = getTagValue(strResult, this.workOrder_StatusCode_Tag);

			//取得RspCode
			RspCode = getTagValue(strResult, this.workOrder_RspCode_Tag);

			//取得ErrorCode
			ErrorCode = getTagValue(strResult, this.workOrder_ErrorCode_Tag);
		}
		private static String[] dumpAttributes(com.ctcb.ldap.Attributes h, String name)
		{
			if (h != null)
			{
				Attr attr = h.getAttribute(name);
				if (attr == null)
					return (new String[0]);
				else
					return attr.getAttrValues();
			}
			else
				return (new String[0]);
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
			catch (Exception ex)
			{
				Logging.Log("無法取得代碼", LogState.Info, LogLayer.UI);
				return tagValue;
			}
		}
		public override string getStatusCodeTag()
        {
			return this.workOrder_StatusCode_Tag;
        }
		public override string getRspCodeTag()
		{
			return this.workOrder_RspCode_Tag;
		}
		public override string getErrorCodeTag()
		{
			return this.workOrder_ErrorCode_Tag;
		}
        public override string getErrorMessageTag()
        {
			return this.workOrder_ErrorMessage_Tag;
        }
    }
}
