<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060401000003.aspx.cs"
    Inherits="P060401000003" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>郵局查單申請處理修改</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/DIVDialog.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>

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
                <table width="100%" border="0" cellpadding="0" cellspacing="1" runat="server" id="Table1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li id="liTittle">
                                <cc1:CustLabel ID="CustLabel9" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06040100_030" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></li></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 85%" colspan="3">
                            <asp:Label ID="lblcardno" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_007" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td colspan="3"  style="width: 85%">
                            <asp:Label ID="lblmailno" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_006" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td colspan="3"  style="width: 85%">
                            <asp:Label ID="lblaction" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_003" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td colspan="3"  style="width: 85%">
                            <asp:Label ID="lblpodate" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel5" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_010" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td colspan="3"  style="width: 85%">
                            
                                <cc1:DatePicker ID="txtbackdate"  runat="server" 
                                Width="70"></cc1:DatePicker> 
                                <cc1:CustButton ID="btnPrintDate" runat="server" class="smallButton"
                                    Style="width: 50px;" ShowID="06_06040100_010" OnClick="btnPrintDate_Click" /></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel6" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_009" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td colspan="3"  style="width: 85%">
                            <asp:Label ID="lblendCaseFlgs" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%; height: 25px" valign="top">
                            <cc1:CustLabel ID="CustLabel7" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_022" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 35%;" valign="top" align="left">
                            <asp:TextBox ID="txtnote" runat="server" Height="112px" TextMode="MultiLine" Width="256px"></asp:TextBox></td>
                        <td valign="top" align="right"  style="width: 15%">
                            <cc1:CustLabel ID="CustLabel8" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_023" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td  style="width: 85%" valign="top" align="left">
                            <asp:ListBox ID="LibPhrase" runat="server" Height="127px" Width="260px" AutoPostBack="True" OnSelectedIndexChanged="LibPhrase_SelectedIndexChanged"></asp:ListBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="left" colspan="4">
                            &nbsp; &nbsp; &nbsp; <cc1:CustCheckBox ID="chkdelete" runat="server" AutoPostBack="True" OnCheckedChanged="chkdelete_CheckedChanged"/>
                            &nbsp;</td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4" align="center" style="height: 25px">
                            &nbsp;&nbsp;
                            <cc1:CustButton ID="btnUpdate" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06040100_014" OnClick="btnUpdate_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnCreateForm" runat="server" class="smallButton"
                                ShowID="06_06040100_024" Width="67px" OnClick="btnCreateForm_Click" />&nbsp;&nbsp;
                                <cc1:CustButton ID="btnFinish" OnClick="btnFinish_Click" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06040100_025" Width="60px" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnCancel" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="02_00000000_017" OnClick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
