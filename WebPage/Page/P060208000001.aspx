<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060208000001.aspx.cs" Inherits="P060208000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>VD卡整批結案</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
    function ConfirmMsg()
    {
        if (document.getElementById("IsAlert").value == "y") {
            if (confirm(document.getElementById("ShowMsg").value)) {
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    </script>

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
                                    SetBreak="False" SetOmit="False" ShowID="06_06020800_000" StickHeight="False">
                                </cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px;width:15%">
                            <cc1:CustLabel ID="lblVDCard" runat="server" FractionalDigit="2" IsColon="True"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_06040100_001" CurAlign="left" CurSymbol="£">
                            </cc1:CustLabel>
                        </td>
                        <td  style="height: 25px;width: 35%">
                            <cc1:CustDropDownList ID="dropVDCard" runat="server" >
                            </cc1:CustDropDownList>
                        </td>
                        <td align="right" style="height: 25px;width:15% ">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020800_002" CurAlign="left"
                                CurSymbol="£">
                            </cc1:CustLabel>
                        </td>
                        <td  style="height: 25px;width:35%">
                        <cc1:DatePicker ID="dpDate" MaxLength="10" runat="Server" Width="100">
                        </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" ShowID="06_06020800_003"
                                OnClick="btnSearch_Click" OnClientClick="return ConfirmMsg()" />
                            &nbsp;
                            <asp:Button ID="btnBatch" Text="06_06020800_004" runat="Server" OnClientClick="return ConfirmMsg();"
                                CssClass="smallButton" OnClick="btnBatch_Click" Enabled="false" />
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="SearchResult">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="grvResult" runat="Server" AllowSorting="true" AllowPaging="false"
                                PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" DataKeyNames="ID">
                                <RowStyle CssClass="Grid_Item" Wrap="true" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="false" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="false" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="CardNo">
                                        <itemstyle width="30%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cardtype">
                                        <itemstyle width="30%" horizontalalign="Center" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Backdate">
                                        <itemstyle width="30%" horizontalalign="Center" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ID" Visible="false"></asp:BoundField>
                                    <asp:BoundField DataField="Action" Visible="false"></asp:BoundField>
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
                <asp:HiddenField runat="Server" ID="IsAlert" />
                <asp:HiddenField runat="Server" ID="ShowMsg" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
