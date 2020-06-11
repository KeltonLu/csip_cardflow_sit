﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060502000001.aspx.cs" Inherits="P060502000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

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

        <script language="javascript" type="text/javascript">
			window.addEventListener("scroll", scroll, false);
              function   scroll()   
              {   
                $("#divProgress").css("top",290+document.documentElement.scrollTop);
              }    
        </script>

        <asp:UpdateProgress ID="updateProgress1" runat="server">
            <ProgressTemplate>
                <div id="divProgress" align="center" class="progress" style="position: absolute;
                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">
                    <div id="divProgress2" align="center" class="progress" style="background-color: White;
                        width: 50%; margin: 0px auto;">
                        <br />
                        <img alt="Please Wait..." src="../Common/images/Waiting.gif" />
                        <br />
                        <cc1:CustLabel ID="lblWaiting" runat="server" CurAlign="center" CurSymbol="£" FractionalDigit="2"
                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                            SetBreak="False" SetOmit="False" ShowID="00_00000000_000" StickHeight="False"></cc1:CustLabel>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="6">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_05020000_000" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 10%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_05020000_002" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 30%">
                            <cc1:DatePicker ID="txtImportStart" runat="server" Width="74">
                            </cc1:DatePicker>
                            ~
                            <cc1:DatePicker ID="txtImportEnd" runat="server" Width="74">
                            </cc1:DatePicker>
                        </td>
                        <td align="right" style="height: 25px; width: 10%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_05020000_004" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 20%">
                            <cc1:CustDropDownList ID="ddlState" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                        <td align="right" style="height: 25px; width: 10%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_05020000_005" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 20%">
                            <cc1:CustDropDownList ID="ddlFactory" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="6">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_05020000_003" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
