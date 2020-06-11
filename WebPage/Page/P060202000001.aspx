<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060202000001.aspx.cs" Inherits="P060202000001" %>

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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020000_000" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="lblUser_Id" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_02020000_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%">
                            <cc1:CustTextBox ID="txtId" MaxLength="16" runat="server" InputType="String" Width="174px"></cc1:CustTextBox></td>
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_02020000_002" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%">
                            <cc1:CustTextBox ID="txtNo" runat="server" InputType="String" MaxLength="19"
                                Width="174px"></cc1:CustTextBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_02020000_003" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnAdd" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_02020000_004" OnClick="btnAdd_Click" /></td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td colspan="20">
                            <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                                PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" OnRowEditing="grvUserView_RowEditing" DataKeyNames="cardno" OnRowDataBound="grvUserView_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="Cardtypes">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                        <cc1:CustLinkButton id="lkbDetail" runat="server" CommandName="Edit" Text='<%# Bind("cardno") %>'>
                                        </cc1:CustLinkButton>
                                        <asp:HiddenField ID="hidType" runat="server" Value='<%# Bind("TYPE_flg") %>' />
                                        <asp:HiddenField ID="hidTrandate" runat="server" Value='<%# Bind("Trandate") %>' />
                                        <asp:HiddenField ID="hidKind" runat="server" Value='<%# Bind("Kind") %>' />
                                        <asp:HiddenField ID="hidUrgencyFlg" runat="server" Value='<%# Bind("Urgency_Flg") %>' />
                                    </itemtemplate>
                                        <itemstyle horizontalalign="Left" width="20%" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Indate1">
                                        <itemstyle width="10%" horizontalalign="Left" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="maildate">
                                        <itemstyle width="10%" horizontalalign="Left" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailno">
                                        <itemstyle width="16%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="photos">
                                        <itemstyle width="17%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="affinitys">
                                        <itemstyle width="17%" horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
