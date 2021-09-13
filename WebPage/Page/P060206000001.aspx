<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060206000001.aspx.cs" Inherits="Page_P060206000001" %>

<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>綜合資料處理</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script language="javascript" type="text/javascript">
        function check(type)
        {
            var name =  document.getElementById("txtId").value;
            if(name=="")
            {  
                alert("身分證字號不可以為空！");
                return false;
            }
        }
    </script>

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
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020600_000" StickHeight="False"></cc1:CustLabel></li></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="lblUser_Id" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020100_000" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%">
                            <cc1:CustTextBox ID="txtId" MaxLength="11" runat="server" InputType="String" Width="174px"></cc1:CustTextBox></td>
                        <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020600_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%">
                            <asp:DropDownList ID="dropStatus" runat="server" Width="128px">
                            </asp:DropDownList></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020100_002" OnClick="btnSearch_Click" OnClientClick="return check()" />&nbsp;&nbsp;
                            <%--<cc1:CustButton ID="btnSearchAdd" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020600_002" OnClick="btnSearchAdd_Click" />&nbsp;&nbsp;--%>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td colspan="20">
                            <%--2021/01/11 陳永銘 新增事件:OnRowDataBound/EnableModelValidation/CustName_Roma--%>
                            <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                                PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" OnRowEditing="grvUserView_RowEditing" DataKeyNames="cardno" OnRowDataBound="grvUserView_RowDataBound" EnableModelValidation="True">
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="5%" />
                                        <itemtemplate>
                                         <input id="chkSearchAdd" type="checkbox" runat="server"  class="ChoiceButton"/> 
                                        </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CustName">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" Wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CustName_Roma">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" Wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cardtypeS">
                                        <itemstyle width="10%" horizontalalign="Left" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Backdate">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ReasonS">
                                        <itemstyle width="20%" horizontalalign="Left" wrap="True" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="25%" />
                                        <itemtemplate>
                                        <cc1:CustLinkButton id="lkbDetail" runat="server" CommandName="Edit" Text='<%# Bind("cardno") %>'　></cc1:CustLinkButton> 
                                        </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Mailno">
                                        <itemstyle width="10%" horizontalalign="Left" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CardBackStatusS">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
                
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table2" runat="server">
                    <tr align="center" class="itemTitle" id="trSearchAdd" runat="server">
                        <td colspan="20">
                            <cc1:CustButton ID="btnSearchAdd" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020600_002" OnClick="btnSearchAdd_Click" />&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
                
                <cc1:CustPanel ID="pnlAddSource1" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel2" runat="server" IsColon="True" ShowID="06_06020600_003"
                                    CurSymbol="£">11233313233</cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo1" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd1" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel5" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd1" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName1" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddSource2" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel9" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo2" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel13" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd2" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel15" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd2" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel17" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName2" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddSource3" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel19" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo3" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel21" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd3" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel23" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd3" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel25" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName3" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddSource4" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel27" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo4" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel29" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd4" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel31" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd4" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel33" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName4" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddSource5" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel35" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo5" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel37" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd5" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel39" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd5" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel41" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName5" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddSource6" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo6" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel16" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd6" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel20" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd6" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel24" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName6" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddSource7" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel28" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo7" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel32" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd7" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel36" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd7" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel40" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName7" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddSource8" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel43" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_003" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblCardNo8" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel45" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_004" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblBackAdd8" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel47" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFAdd8" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel49" runat="server" IsColon="True"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <cc1:CustLabel ID="lblMFName8" runat="server" FractionalDigit="2" IsColon="False"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" CurAlign="left" CurSymbol="£" ShowID=""></cc1:CustLabel></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlEnditemDrop" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr align="center" class="itemTitle">
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_007" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%">
                                <asp:DropDownList ID="dropEnditem" runat="server" AutoPostBack="True" Width="142px"
                                    OnSelectedIndexChanged="dropEnditem_SelectedIndexChanged">
                                </asp:DropDownList></td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="2">
                                <cc1:CustButton ID="btSub" runat="server" class="smallButton" Style="width: 50px;"
                                    ShowID="06_06020600_014" OnClick="btSub_Click" />
                                <cc1:CustButton ID="btCancel" runat="server" class="smallButton" Style="width: 50px;"
                                    ShowID="06_06020600_023" OnClick="btCancel_Click"/>
                                </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <cc1:CustPanel ID="pnlAddChange" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel8" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_008" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="width: 42.5%">
                                <cc1:CustButton ID="btnMFAdd" runat="server" class="smallButton" Style="width: 80px;"
                                    ShowID="06_06020600_009" OnClick="btnMFAdd_Click" />&nbsp;&nbsp;
                            </td>
                            <td style="width: 42.5%">
                                <cc1:CustButton ID="btnDBAdd" runat="server" class="smallButton" Style="width: 80px;"
                                    ShowID="06_06020600_010" OnClick="btnPC_Click" />&nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_011" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 85%" colspan="2">
                                <cc1:CustLabel ID="lblZip" runat="server" FractionalDigit="2" IsColon="False" IsCurrency="False"
                                    NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel12" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_012" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px" colspan="2">
                                &nbsp;<uc1:CustAddress ID="CustAdd1" runat="server" OnChangeValues="CustAdd1_ChangeValues" />
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                            </td>
                            <td style="height: 25px;" colspan="2">
                                <cc1:CustTextBox ID="txtAdd2" MaxLength="40" runat="server" InputType="String" Width="285px"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                            </td>
                            <td style="height: 25px;" colspan="2">
                                <cc1:CustTextBox ID="txtAdd3" MaxLength="40" runat="server" InputType="String" Width="285px"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06020600_013" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px;" colspan="2">
                                <cc1:CustTextBox ID="txtNote" MaxLength="40" runat="server" InputType="String" Width="285px"></cc1:CustTextBox></td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <asp:ImageButton ID="AddButtonNotice1" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderNotice1" runat="server" TargetControlID="AddButtonNotice1"
                    PopupControlID="PanNotice1" BackgroundCssClass="modal" CancelControlID="btnCancelNotice1"
                    DropShadow="False" />
                <asp:Panel ID="PanNotice1" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="683px" Height="51px">
                    <table id="tbNotice1" width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle">
                            <td style="height: 30px">
                                <p>
                                    <cc1:CustLabel ID="lblNotice1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel></p>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="left" style="width: 100%;">
                                1.告知客戶開卡日截止日期，並建議客戶至分行申請新卡（本人帶身分證及原開戶印鑑）。</td>
                        </tr>
                        <tr class="trEven">
                            <td align="left" style="width: 100%;">
                                2.若客戶同意至分行申請新卡，請告知客戶，線上立即將卡片作廢，新舊卡均無法使用，且作廢後卡片即無法再恢復使用。</td>
                        </tr>
                        <tr class="trOdd">
                            <td align="left" style="width: 100%;">
                                3.若客戶不同意至分行申請新卡，仍可執行退卡重寄。</td>
                        </tr>
                        <tr align="left" class="itemTitle">
                            <td align="center" style="height: 25px">
                                <asp:Button ID="btnCancelNotice1" runat="server" Text="關閉" CssClass="smallButton" /></td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:ImageButton ID="AddButtonNotice2" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderNotice2" runat="server" TargetControlID="AddButtonNotice2"
                    PopupControlID="PanNotice2" BackgroundCssClass="modal" CancelControlID="btnCancelNotice2"
                    DropShadow="False" />
                <asp:Panel ID="PanNotice2" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="336px" Height="83px">
                    <table border="0" cellpadding="0" cellspacing="1" style="width: 122%;">
                        <tr class="itemTitle">
                            <td style="height: 30px">
                                <p>
                                    回報注意事項</p>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="left" style="width: 100%">
                                <cc1:CustLabel ID="lblNotice2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="left" style="width: 100%;">
                                若您未在截止日前完成開卡，卡片將自動作廢無法使用，</td>
                        </tr>
                        <tr class="trOdd">
                            <td align="left" style="width: 100%;">
                                到時需請您本人帶您的身分證及原開戶印鑑，到就近分行重新申請。</td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td align="center">
                                &nbsp;<asp:Button ID="btnCancelNotice2" runat="server" Text="關閉" CssClass="smallButton" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:HiddenField ID="hidSearchAdd" runat="server" />
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
