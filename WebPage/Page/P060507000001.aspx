<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060507000001.aspx.cs" Inherits="P060507000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%--<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>簡訊發送查詢</title>

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
                        <td colspan="6">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06050700_000" StickHeight="False"></cc1:CustLabel></li></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 12%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050700_001" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 21%">
                                <cc1:CustTextBox ID="txtID" runat="server" InputType="String" MaxLength="10" Width="174px"></cc1:CustTextBox></td>
                            <td align="right" style="height: 25px; width: 12%">
                                <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050700_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel>
                            </td>
                            <td style="height: 25px; width: 21%">
                                <cc1:CustTextBox ID="txtMailno" runat="server" InputType="String" MaxLength="20"
                                    Width="174px"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="height: 25px; width: 12%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050700_005" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel>                                
                            </td>
                            <td align="left" style="height: 25px; width: 22%">
                                <asp:DropDownList ID="dropStatus" runat="server">
                                    <asp:ListItem Value="XX">全部</asp:ListItem>
                                    <asp:ListItem>投遞不成功</asp:ListItem>
                                    <asp:ListItem>投遞成功</asp:ListItem>
                                    <asp:ListItem>招領中</asp:ListItem>
                                    <asp:ListItem>注銷投遞成功</asp:ListItem>
                                    <asp:ListItem>退件</asp:ListItem>
                                </asp:DropDownList>                               
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 12%">
                                <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050700_002" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel>
                                </td>
                            <td style="height: 25px; width: 21%">
                                <cc1:DatePicker ID="txtProcessDateStart" runat="server" Width="74">
                                </cc1:DatePicker>
                                ~
                                <cc1:DatePicker ID="txtProcessDateEnd" runat="server" Width="74">
                                </cc1:DatePicker>
                            </td>
                            <td align="right" style="height: 25px; width: 12%">
                                <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06050700_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel>
                            </td>
                            <td style="height: 25px;" colspan="3">
                                <cc1:DatePicker ID="txtMaildateStart" runat="server" Width="74">
                                </cc1:DatePicker>
                                ~
                                <cc1:DatePicker ID="txtMaildateEnd" runat="server" Width="74">
                                </cc1:DatePicker>
                            </td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="6">
                                <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                    ShowID="06_06050700_006" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            </td>
                        </tr>
                </table>
                <%--<rsweb:ReportViewer ID="ReportViewer0507" runat="server" Height="430px" Width="100%"
                    Font-Names="Verdana" Font-Size="8pt" ProcessingMode="Remote" ShowParameterPrompts="False"
                    SizeToReportContent="True" ShowDocumentMapButton="False" ShowExportControls="True"
                    ShowFindControls="False" ShowPageNavigationControls="True" ShowPrintButton="True"
                    ShowPromptAreaButton="False" ShowRefreshButton="False" ShowZoomControl="False">
                    <ServerReport ReportServerUrl="" />
                </rsweb:ReportViewer>--%>
<%--            </ContentTemplate>
        </asp:UpdatePanel>--%>
    </form>
</body>
</html>
