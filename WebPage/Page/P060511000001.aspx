<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060511000001.aspx.cs" Inherits="P060511000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>更改寄送方式記錄查詢</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet"/>
</head>
<body class="workingArea">
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <table width="100%" border="0" cellpadding="0" cellspacing="1">
                <tr class="itemTitle">
                    <td colspan="6">
                        <li>
                            <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                           IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                           SetBreak="False" SetOmit="False" ShowID="06_06051100_000" StickHeight="False">
                            </cc1:CustLabel>
                        </li>
                    </td>
                </tr>
                <tr class="trOdd">
                    <td align="right" style="height: 25px; width: 10%">
                        <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                       IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                       SetOmit="False" StickHeight="False" ShowID="06_06051100_001" CurAlign="left"
                                       CurSymbol="£">
                        </cc1:CustLabel>
                    </td>
                    <td style="height: 25px; width: 30%">
                        <cc1:DatePicker ID="txtBackdateStart" runat="server" Width="74">
                        </cc1:DatePicker>
                        ~
                        <cc1:DatePicker ID="txtBackdateEnd" runat="server" Width="74">
                        </cc1:DatePicker>
                    </td>
                    <td align="right" style="height: 25px; width: 10%">
                        <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                       IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                       SetOmit="False" StickHeight="False" ShowID="06_06051100_002" CurAlign="left"
                                       CurSymbol="£">
                        </cc1:CustLabel>
                    </td>
                    <td style="height: 25px; width: 30%">
                        <cc1:CustDropDownList ID="custddlType" runat="server">
                        </cc1:CustDropDownList>
                        <cc1:CustCheckBox ID="ckUrgency_Flg" runat="Server"/>
                    </td>
                    <td align="right" style="height: 25px; width: 10%">
                        <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                       IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                       SetOmit="False" StickHeight="False" ShowID="06_06051000_013" CurAlign="left"
                                       CurSymbol="£">
                        </cc1:CustLabel>
                    </td>
                    <td style="height: 25px; width: 10%">
                        <cc1:CustDropDownList ID="ddlFactory" runat="server">
                        </cc1:CustDropDownList>
                    </td>
                </tr>
                <tr align="center" class="itemTitle">
                    <td colspan="6">
                        <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                        ShowID="06_06051100_003" OnClick="btnSearch_Click"/>&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</form>
</body>
</html>