<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060209000001.aspx.cs" Inherits="Page_P060209000001" %>

<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>註銷記錄處理</title>
    
    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />   
    <script language="javascript" type="text/javascript">
		window.addEventListener("scroll", scroll, false);

      function   scroll()   

      {   

        $("#divProgress").css("top",290+document.documentElement.scrollTop);

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
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020900_000" StickHeight="False"></cc1:CustLabel></li></td>
                    </tr>
                    <tr class="trOdd">
                        <td  align="right" style="height: 25px; width: 40%;">
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020900_001" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px;width: 60%;">
                            &nbsp;&nbsp;<cc1:DatePicker ID="dpFrom"  runat="server" Width="74" ></cc1:DatePicker>&nbsp;&nbsp;
                        ~ 
                           &nbsp;&nbsp; <cc1:DatePicker ID="dpTo"  runat="server" Width="74" ></cc1:DatePicker>&nbsp;&nbsp;</td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="2">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020900_002" OnClick="btnSearch_Click"/>&nbsp;
                            <cc1:CustButton ID="btnCheck" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020900_003" OnClick="btnCheck_Click"/>
                            <cc1:CustButton ID="btnRelease" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020900_004" OnClick="btnRelease_Click"/>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td style="height: 25px">
                                 <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                               PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                               OnRowEditing="grvUserView_RowEditing" DataKeyNames="CancelOASAFile" OnRowUpdating="grvUserView_RowUpdating" OnRowDataBound="grvUserView_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="CancelOASADate">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CancelOASASourceName">
                                        <itemstyle width="10%" horizontalalign="Center" wrap="True" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="20%" />
                                        <itemtemplate>
<cc1:CustLinkButton id="lkbDetail" runat="server" 　 Text='<%# Bind("CancelOASAFile") %>' CommandName="Edit" __designer:wfdid="w1"></cc1:CustLinkButton> 
</itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TotalCount">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SCount">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="FCount">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="8%" />
                                        <itemtemplate>
                                         <input id="chkCheckFlg" type="checkbox" runat="server"  class="ChoiceButton"/>                                       
                                        </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="8%" />
                                        <itemtemplate>
                                         <input id="chkReleaseFlg" type="checkbox" runat="server"  class="ChoiceButton"/>                                         
                                        </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="20%" />
                                        <itemtemplate>
<cc1:CustLinkButton id="lkbLog" runat="server" 　 Text='<%# Bind("ModStautsLog") %>' CommandName="Update" __designer:wfdid="w2"></cc1:CustLinkButton> 
</itemtemplate>
                                    </asp:TemplateField>
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
                <asp:ImageButton ID="AddButtonLog" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLog" runat="server" TargetControlID="AddButtonLog"
                    PopupControlID="PanLog" BackgroundCssClass="modal" CancelControlID="btnCloseLog"
                    DropShadow="False" />
                <asp:Panel ID="PanLog" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="500px" Height="51px">
                    <table id="Table2" width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle">
                            <td style="height: 30px"> <li>
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020900_013" StickHeight="False"></cc1:CustLabel></li></td>
                        </tr>
                     </table>
                     <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table3">
                        <tr>
                            <td>
                               <cc1:CustGridView ID="grvLogView" runat="server" AllowSorting="True" AllowPaging="False"
                                   PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                    <RowStyle CssClass="Grid_Item" Wrap="True" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                    <PagerSettings Visible="False" />
                                    <EmptyDataRowStyle HorizontalAlign="Center" />
                                    <Columns>
                                        <asp:BoundField DataField="Date">
                                            <itemstyle width="30%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Name">
                                            <itemstyle width="30%" horizontalalign="Center" wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Note">
                                            <itemstyle width="40%" horizontalalign="Center" />
                                        </asp:BoundField>
                                    </Columns>
                                </cc1:CustGridView>                           
                            </td>
                        </tr>
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table4">
                        <tr align="left" class="itemTitle">
                            <td align="center" style="height: 25px; width: 102%;">
                                <asp:Button ID="btnCloseLog" runat="server" Text="關閉" CssClass="smallButton" /></td>
                        </tr>
                    </table>
                </asp:Panel>   
            </ContentTemplate>
        </asp:UpdatePanel>
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
       
    </form>
</body>
</html>

