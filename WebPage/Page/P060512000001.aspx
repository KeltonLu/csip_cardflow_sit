<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060512000001.aspx.cs" Inherits="P060512000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>郵件交寄狀況檢核</title>

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
                    <td colspan="2">
                        <li>
                            <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                           IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                           SetBreak="False" SetOmit="False" ShowID="06_06051200_000" StickHeight="False">
                            </cc1:CustLabel>
                        </li>
                    </td>
                </tr>
                <tr class="trOdd">
                    <td align="right" style="height: 25px; width: 45%">
                        <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                       IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                       SetOmit="False" StickHeight="False" ShowID="06_06051200_001" CurAlign="left"
                                       CurSymbol="£">
                        </cc1:CustLabel>
                    </td>
                    <td style="height: 25px; width: 55%">
                        <cc1:DatePicker ID="txtBackdateStart" runat="server" Width="74">
                        </cc1:DatePicker>
                        ~
                        <cc1:DatePicker ID="txtBackdateEnd" runat="server" Width="74">
                        </cc1:DatePicker>
                    </td>
                </tr>
                <tr align="center" class="itemTitle">
                    <td colspan="2">
                        <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                        ShowID="06_06051200_003" OnClick="btnSearch_Click"/>&nbsp;&nbsp;
                        <cc1:CustButton ID="btnPrint" runat="server" class="smallButton" Style="width: 50px;"
                                        ShowID="06_06051200_011" OnClick="btnPrint_Click"/>&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                <tr>
                    <td colspan="20">
                        <cc1:CustGridView ID="grvCardView" runat="server" AllowSorting="True" AllowPaging="False"
                                          PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                          BorderStyle="Solid" DataKeyNames="card_file" OnRowDataBound="grvCardView_RowDataBound">
                            <RowStyle CssClass="Grid_Item" Wrap="True"/>
                            <SelectedRowStyle CssClass="Grid_SelectedItem"/>
                            <HeaderStyle CssClass="Grid_Header" Wrap="False"/>
                            <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True"/>
                            <PagerSettings Visible="False"/>
                            <EmptyDataRowStyle HorizontalAlign="Center"/>
                            <Columns>
                                <asp:TemplateField>
                                    <itemtemplate>
                                        <cc1:CustLinkButton id="lbcardfile" runat="server" Text='<%# Bind("card_file") %>' __designer:wfdid="w1" OnClick="lbcardfile_Click">
                                        </cc1:CustLinkButton> <cc1:CustLabel id="custlbCardfile" runat="server" Text='<%# Bind("card_file") %>' Visible="false" __designer:wfdid="w2"></cc1:CustLabel>
                                    </itemtemplate>
                                    <itemstyle horizontalalign="Left" width="35%"/>
                                </asp:TemplateField>
                                <asp:BoundField DataField="allnum">
                                    <itemstyle width="15%" horizontalalign="Center"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="snum">
                                    <itemstyle width="25%" horizontalalign="Center"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="fnum">
                                    <itemstyle width="25%" horizontalalign="Center"/>
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
            <asp:ImageButton ID="DetailButton" Style="display: none" runat="server"/>
            <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderM" runat="server" TargetControlID="DetailButton"
                                            PopupControlID="PanM" BackgroundCssClass="modal" CancelControlID="btnCancelM"
                                            DropShadow="False"/>
            <asp:Panel ID="PanM" CssClass="workingArea" runat="server" Style="display: none;"
                       Width="400px" Height="150px">
                <table border="0" cellpadding="0" cellspacing="1" style="width: 100%;">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li id="li3">
                                <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                               IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                               SetBreak="False" SetOmit="False" ShowID="06_06051200_013" StickHeight="False">
                                </cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table2">
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table3">
                    <tr align="center" class="itemTitle">
                        <td colspan="4" align="center">
                            <asp:Button ID="btnCancelM" runat="server" Text="取消" CssClass="smallButton"/>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</form>
</body>
</html>