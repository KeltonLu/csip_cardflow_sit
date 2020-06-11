<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060209000003.aspx.cs" Inherits="Page_P060209000003" %>

<%-- <%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" --%>
<%--     Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %> --%>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>註銷記錄處理 列印</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <table width="100%" border="0" cellpadding="0" cellspacing="1">
                <tr class="itemTitle">
                    <td style="height: 25px">
                        <li>
                            <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020902_000" StickHeight="False"></cc1:CustLabel></li></td>
                </tr>
                <tr>
                    <td >
                       <%-- <rsweb:ReportViewer ID="ReportViewer0209" runat="server" Height="434px" Width="1050px" --%>
                       <%--          Font-Names="Verdana" Font-Size="8pt" ProcessingMode="Remote" ShowParameterPrompts="False" --%>
                       <%--          SizeToReportContent="True" ShowDocumentMapButton="False" ShowExportControls="True" --%>
                       <%--          ShowFindControls="False" ShowPageNavigationControls="False" ShowPrintButton="True" --%>
                       <%--          ShowPromptAreaButton="False" ShowRefreshButton="False" ShowZoomControl="False"> --%>
                       <%--          <ServerReport ReportServerUrl="" /> --%>
                       <%--      </rsweb:ReportViewer> --%>
                    </td>
                </tr>
                <tr align="center" class="itemTitle">
                    <td colspan="2" style="height: 32px">
                        <cc1:CustButton ID="btnBack" runat="server" class="smallButton" Style="width: 50px;"
                            ShowID="06_06020902_002" OnClick="btnBack_Click" />&nbsp;&nbsp;
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>

