<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060521000001.aspx.cs" Inherits="P060521000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>自取改限掛大宗掛號單</title>

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
                        <td colspan="2" style="width: 100%">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06052100_000" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 50%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" IsColon="True" ShowID="06_06052100_001"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 50%">
                            <cc1:DatePicker ID="txtSelfPickDate" runat="server" Width="74">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="2" style="width: 100%">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06052100_002" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>