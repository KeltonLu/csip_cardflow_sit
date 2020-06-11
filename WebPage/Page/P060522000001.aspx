<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060522000001.aspx.cs" Inherits="P060522000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%--<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>緊急製卡檔案匯入報表</title>

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
                        <td colspan="4" style="width: 100%">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06052200_000" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 25%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" IsColon="True" ShowID="06_06052200_001"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 25%">
                            <cc1:DatePicker ID="txtStartDate" runat="server" Width="74">
                            </cc1:DatePicker>
                               ~
                                <cc1:DatePicker ID="txtEndDate" runat="server" Width="74">
                                </cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 25%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06052200_004" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 25%; height: 30px;">
                            <cc1:CustDropDownList ID="ddlResult" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 25%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06052200_002" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 25%">
                            <cc1:CustTextBox ID="txtid" runat="server" InputType="String" MaxLength="10"
                                Width="174px"></cc1:CustTextBox></td>
                        <td align="right" style="height: 25px; width: 25%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06052200_003" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 25%">
                            <cc1:CustTextBox ID="txtCardno" runat="server" InputType="String" MaxLength="19" Width="174px"></cc1:CustTextBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4" style="width: 100%">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06052200_005" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            &nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="06_06052200_006"  
                                OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                      <tr align="center" class="itemTitle">
                         
                        <td colspan="4" style="width: 100%">
                               <cc1:CustLabel ID="lblTotal" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06052200_007" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                             <cc1:CustLabel ID="lblTotalCount" runat="server" Text=""></cc1:CustLabel>

                               <cc1:CustLabel ID="lblSucc" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06052200_008" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                             <cc1:CustLabel ID="lblSuccCount" runat="server" Text=""></cc1:CustLabel>
                              <cc1:CustLabel ID="lblFail" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06052200_009" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                             <cc1:CustLabel ID="lblFailCount" runat="server" Text=""></cc1:CustLabel>
                        </td></tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbP02Record" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="indate1" HeaderText="作業日期">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="id" HeaderText="身分證字號">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cardno" HeaderText="卡號">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="kind" HeaderText="取卡方式">
                                        <ItemStyle Width="6%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="card_file" HeaderText="匯入檔名">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="result" HeaderText="匯入結果">
                                        <ItemStyle Width="6%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="fail_reason" HeaderText="失敗原因">
                                        <ItemStyle Width="14%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="reason" HeaderText="失敗原因說明">
                                        <ItemStyle Width="16%" HorizontalAlign="Center" />
                                    </asp:BoundField>  
                                    <asp:BoundField DataField="import_time" HeaderText="匯入時間">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="import_user" HeaderText="修改經辦">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged" PrevPageText="<前一頁"
                                SubmitButtonText="Go">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
