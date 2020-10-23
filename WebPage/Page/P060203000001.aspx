<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060203000001.aspx.cs" Inherits="Page_P060203000001" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
            <div id="divProgress" align="center" class="progress" style="position: absolute; top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">
                <div id="divProgress2" align="center" class="progress" style="background-color: White; width: 50%; margin: 0px auto;">
                    <br/>
                    <img alt="Please Wait..." src="../Common/images/Waiting.gif"/>
                    <br/>
                    <cc1:CustLabel ID="lblWaiting" runat="server" CurAlign="center" CurSymbol="£" FractionalDigit="2"
                                   IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                   SetBreak="False" SetOmit="False" ShowID="00_00000000_000" StickHeight="False">
                    </cc1:CustLabel>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02030001_000" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="lbId" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02030001_002" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%">
                            <cc1:CustTextBox ID="txtId" MaxLength="11" runat="server" InputType="String" Width="174px"></cc1:CustTextBox></td>
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="lbCardNo" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02030001_003" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%">
                            <cc1:CustTextBox ID="txtCardNo" runat="server" InputType="String" MaxLength="19"
                                Width="174px"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%" colspan="1">
                            <cc1:CustLabel ID="lbDateTime" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_02030001_004" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%" colspan="1">
                            <cc1:DatePicker ID="dpStart" MaxLength="10" runat="Server" Width="70">
                            </cc1:DatePicker>
                            ～
                            <cc1:DatePicker ID="dpEnd" MaxLength="10" runat="Server" Width="70">
                            </cc1:DatePicker>
                        </td>
                        <td align="right" style="height: 25px; width: 15%" colspan="1">
                            <cc1:CustLabel ID="lbState" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02030001_005" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%" colspan="1">
                            <cc1:CustDropDownList ID="ddlState" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="lbUserId" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02030001_006" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%">
                            <cc1:CustTextBox ID="txtUser" MaxLength="11" runat="server" InputType="String" Width="174px"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="lbChangeField" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_02030001_007" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%" colspan="1" rowspan="1">
                            <cc1:CustDropDownList ID="ddlChangeField" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02030001_026" CurAlign="left" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 35%" colspan="3">
                            <cc1:CustDropDownList ID="ddlFactory" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_02030001_018" OnClick="btnSearch_Click" />
                            <cc1:CustButton ID="btnPring" runat="server" class="smallButton" Style="width: 50px;"
                                            ShowID="06_02030001_027" OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td >
                            <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                                PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" OnRowCommand="grvUserView_RowCommand" DataKeyNames="cardno">
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                        <cc1:CustLinkButton id="lbtnCardNo" runat="server" CommandName="Select" Text='<%# Bind("CardNo") %>'>
                                        </cc1:CustLinkButton>
                                        </itemtemplate>
                                        <itemstyle horizontalalign="left" width="20%" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="UpdDate">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Datachange">
                                        <itemstyle width="10%" horizontalalign="Center" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="UpdUser">
                                        <itemstyle width="10%" horizontalalign="Left" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="OutputFlg">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="OutputFileName">
                                        <itemstyle width="15%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CNote">
                                        <itemstyle width="20%" horizontalalign="Left" />
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
                <!--取卡方式-->
                <asp:ImageButton ID="AddButtonC" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderC" runat="server" TargetControlID="AddButtonC"
                    PopupControlID="PanC" BackgroundCssClass="modal" 
                    DropShadow="False" />
                <asp:Panel ID="PanC" CssClass="workingArea" ScrollBars="Vertical" runat="server"
                    Style="display: none;" Width="450px" Height="400px">
                    <table border="0" cellpadding="0" cellspacing="1" style="width: 98%;">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li>
                                    <cc1:CustLabel ID="CustLabel59" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_02030001_015" StickHeight="False"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="center" style="height: 25px">
                                <cc1:CustGridView ID="CustGridView1" runat="server" Width="300" OnRowCommand="grvUserView_RowCommand"
                                    DataKeyNames="id" BorderStyle="Solid" CellSpacing="1" CellPadding="0" BorderWidth="0px"
                                    PagerID="gpList" AllowPaging="False" AllowSorting="True" Height="7px">
                                    <RowStyle CssClass="Grid_Item" Wrap="True" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                    <PagerSettings Visible="False" />
                                    <EmptyDataRowStyle HorizontalAlign="Center" />
                                    <Columns>
                                        <asp:BoundField DataField="NoteCaptions">
                                            <itemstyle width="40%" horizontalalign="Left" wrap="True" verticalalign="Top"></itemstyle>
                                        </asp:BoundField>
                                        <asp:TemplateField>
                                            <itemtemplate>
                                            <asp:TextBox id="txtcNote" runat="server" Text='<%# Bind("CNote") %>' Width="250px" TextMode="MultiLine" Height="51px"  MaxLength="200"></asp:TextBox>
                                            </itemtemplate>
                                            <itemstyle width="60%" horizontalalign="Left"></itemstyle>
                                        </asp:TemplateField>
                                    </Columns>
                                </cc1:CustGridView>
                            </td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="2" align="center">
                                <asp:Button ID="btnCancelC" runat="server" Text="關閉" CssClass="smallButton" OnClick="btnCancelC_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
