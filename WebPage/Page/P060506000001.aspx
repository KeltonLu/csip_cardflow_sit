<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060506000001.aspx.cs" Inherits="P060506000001" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>註銷作業報表</title>

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
<%--        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>--%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06050600_000" StickHeight="False"></cc1:CustLabel></li></td>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050600_001" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 35%">
                                <cc1:DatePicker ID="txtBackdateStart" runat="server" Width="74">
                                </cc1:DatePicker>
                                ~
                                <cc1:DatePicker ID="txtBackdateEnd" runat="server" Width="74">
                                </cc1:DatePicker>
                            </td>
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050600_002" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 35%">
                                <asp:DropDownList ID="dropStatus" runat="server">
                                    <asp:ListItem Value="XX">全部</asp:ListItem>
                                    <asp:ListItem Value="1">成功</asp:ListItem>
                                    <asp:ListItem Value="2">失敗</asp:ListItem>
                                    <asp:ListItem Value="0">處理中</asp:ListItem>
                                </asp:DropDownList></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="lblUser_Id" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 35%">
                                <asp:DropDownList ID="dropBlockCode" runat="server">
                                    <asp:ListItem>D</asp:ListItem>
                                    <asp:ListItem>J</asp:ListItem>
                                    <asp:ListItem>W</asp:ListItem>
                                    <asp:ListItem>K</asp:ListItem>
                                </asp:DropDownList></td>
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 35%">
                                <asp:DropDownList ID="dropMEMO" runat="server">
                                    <asp:ListItem>DA</asp:ListItem>
                                    <asp:ListItem>DN</asp:ListItem>
                                    <asp:ListItem>DE</asp:ListItem>
                                    <asp:ListItem>其他</asp:ListItem>
                                </asp:DropDownList>
                                <cc1:CustTextBox ID="txtMemo" runat="server" MaxLength="20"></cc1:CustTextBox></td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="4">
                                <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                    ShowID="06_06050600_005" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            </td>
                        </tr>
                       
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
