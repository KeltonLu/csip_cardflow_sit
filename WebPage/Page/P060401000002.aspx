<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060401000002.aspx.cs"
    Inherits="P060401000002" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>郵局查單申請處理新增</title>
     <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script language="javascript" type="text/javascript">
    
    function visabledrop()
    {
       document.getElementById("DropDownList1").style.display="none";
    }
    function unvisabledrop()
    {
       document.getElementById("DropDownList1").style.display="";
    }
    </script>

</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" runat="server" id="Table1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li id="liTittle">
                                <%--<cc1:CustLabel ID="CustLabel9" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06040100_031" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel>--%>
                                    
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06040100_031" StickHeight="False"></cc1:CustLabel>
                                    
                                    </li></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 85%" colspan="3">
                            <cc1:CustTextBox ID="txtNo" MaxLength="19" runat="server" InputType="String" Width="150px"></cc1:CustTextBox>
                            <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="smallButton" OnClick="btnSearch_Click" />
                            </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_019" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td colspan="3" style="width: 85%">
                            &nbsp;
                            <asp:Label ID="lblName" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_020" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td colspan="3" style="width: 85%">
                            &nbsp;
                            <asp:Label ID="lblAddress" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_021" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 85%">
                            &nbsp;
                            <asp:Label ID="lblMailDate" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel5" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_007" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 85%">
                            &nbsp;
                            <asp:Label ID="lblMailNo" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel6" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_003" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td colspan="3" style="width: 15%">
                            <cc1:DatePicker ID="txtPoDate" runat="server" Width="74">
                            </cc1:DatePicker>
                            </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%; height: 25px" valign="top">
                            <cc1:CustLabel ID="CustLabel7" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_022" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 35%;" valign="top" align="left">
                            <asp:TextBox ID="txtNote" runat="server" Height="112px" TextMode="MultiLine" Width="256px"></asp:TextBox></td>
                        <td valign="top" align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel8" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06040100_023" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="width: 35%; height: 25px;" valign="top" align="left"> 
                            <asp:ListBox ID="LibPhrase" runat="server" Height="127px" Width="260px" OnSelectedIndexChanged="LibPhrase_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4" align="center">
                            &nbsp; &nbsp;&nbsp;
                            <cc1:CustButton ID="btnAdd" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06040100_014" OnClick="btnAdd_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnCreateForm" runat="server" class="smallButton"
                                ShowID="02_00000000_018" Width="67px" OnClick="btnCreateForm_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnCancel" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="02_00000000_017" OnClick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
                <asp:ImageButton ID="AddButton" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="AddButton"
                    PopupControlID="Panel1" BackgroundCssClass="modal" 
                    DropShadow="False"/>
                <asp:Panel ID="Panel1" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="100px" Height="100px">
                    <table id="tbContext" border="0" cellpadding="0" cellspacing="0" style="width: 404%;
                        height: 144px">
                        <tr class="itemTitle">
                            <td colspan="4" style="height: 30px">
                                <li id="li1">
                                        <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06040100_029" StickHeight="False"></cc1:CustLabel>
                                       
                                        </li></td>
                        </tr>
                        <tr >
                            <td colspan="4" align="center" style="height: 25px">
                                <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                                    PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                    BorderStyle="Solid" DataKeyNames="cardno"  OnRowEditing="grvUserView_RowEditing">
                                    <RowStyle CssClass="Grid_Item" Wrap="True" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                    <PagerSettings Visible="False" />
                                    <EmptyDataRowStyle HorizontalAlign="Center" />
                                    <Columns>
                                        <asp:BoundField DataField="action" >
                                            <itemstyle width="15%" horizontalalign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="maildate" >
                                            <itemstyle width="15%" horizontalalign="Left" wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="mailno" >
                                            <itemstyle width="15%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:TemplateField >
                                            <itemtemplate>
<cc1:CustButton style="WIDTH: 50px" id="btnSure" class="smallButton" runat="server" ShowID="06_06040100_014" CommandName="edit"></cc1:CustButton> 
</itemtemplate>
                                            <itemstyle width="15%" horizontalalign="Left" />
                                        </asp:TemplateField>
                                    </Columns>
                                </cc1:CustGridView>
                            </td>
                            </tr>
                             <tr >
                            <td colspan="4" align="center" style="height: 16px">
                             <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                            </td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="4" align="center">
                                &nbsp;<asp:Button ID="btnCancels" runat="server" Text="關閉" CssClass="smallButton" OnClick="btnCancels_Click"  />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
            </Triggers>
            <Triggers>
            </Triggers>
            <Triggers>
            </Triggers>
            <Triggers>
            </Triggers>
            <Triggers>
            </Triggers>
            <Triggers>
            </Triggers>
            <Triggers>
            </Triggers>
            <Triggers>
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
