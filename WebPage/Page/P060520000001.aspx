﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060520000001.aspx.cs" Inherits="P060520000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%-- <%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" --%>
<%--     Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %> --%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>址更重寄異動記錄查詢</title>

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
<%--        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>--%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="6" style="width: 100%">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06052000_000" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 30%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" IsColon="True" ShowID="06_06052000_001"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 70%">
                            <cc1:DatePicker ID="txtUpdStart" runat="server" Width="74">
                            </cc1:DatePicker>
                            ~
                            <cc1:DatePicker ID="txtUpdEnd" runat="server" Width="74">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 10%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" IsColon="True" ShowID="06_06052000_002"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 30%">
                            <cc1:CustTextBox ID="txtMailNo" runat="server" MaxLength="20"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 10%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" IsColon="True" ShowID="06_06052000_003"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 30%">
                            <cc1:CustTextBox ID="txtId" runat="server" MaxLength="11"></cc1:CustTextBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="6" style="width: 32%">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06052000_004" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
                <%-- <rsweb:reportviewer id="ReportViewer0520" runat="server" height="470px" width="100%" --%>
                <%--     font-names="Verdana" font-size="8pt" processingmode="Remote" showparameterprompts="False" --%>
                <%--     sizetoreportcontent="True" showdocumentmapbutton="False" showexportcontrols="True" --%>
                <%--     showfindcontrols="False" showpagenavigationcontrols="True" showprintbutton="True" --%>
                <%--     showpromptareabutton="False" showrefreshbutton="False" showzoomcontrol="False"> --%>
                <%-- <ServerReport ReportServerUrl="" /> --%>
                <%-- </rsweb:reportviewer> --%>
<%--            </ContentTemplate>
        </asp:UpdatePanel>--%>
    </form>
</body>
</html>