<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060202000003.aspx.cs" Inherits="P060202000003" %>

<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
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

    <script language="javascript" type="text/javascript">
    
    function visabledrop()
    {
       document.getElementById("DropDownList1").style.display="none";
    }
    
    function unvisabledrop()
    {
       document.getElementById("DropDownList1").style.display="";
    }
       
    function check(type)
    {
        switch(type)
        {
            case "A":
                var add2 =  document.getElementById("txtAdd2Ajax").value;
                if(add2=="")
                {  
                    alert("地址二不能為空");
                    return false;
                }
                var ddlAction = document.getElementById("ddlAction");
                if(ddlAction.value == "1")
                {
                    var UserName = document.getElementById("txtUserName");
                    var Tel = document.getElementById("txtTel");
                    var newCardnewAccount = document.getElementById("newCardnewAccount");
                    var newCardoldAccount = document.getElementById("newCardoldAccount");
                    if(UserName.value == "")
                    {
                        alert("連絡姓名不能為空");
                        return false;
                    }
                    if(Tel.value == "")
                    {
                        alert("連絡電話不能為空");
                        return false;
                    }
                    if (newCardnewAccount.checked === false && newCardoldAccount.checked === false) {
                        alert("類別為必填！");
                        return false;
                    }
                }
            break
            case "C":
                var name =  document.getElementById("dropKindAjax");
                var index = name.selectedIndex;
                var value = name.options[index].text;
                if(value=="")
                {  
                    alert("取卡方式不能為空");
                    return false;
                }
            break
        }
    }
    
    function IsConfirm(type)
    {
        var hidWay =  document.getElementById("hidWay");    
        var hidAdd =  document.getElementById("hidAdd"); 
        switch(type)
        {
            case "K":
                if (hidWay.value == "Show") 
                {
                    return confirm("已有未轉出作業單，確認修改?");
                }
                break;
            case "A":
                var bConfirm;
                if (hidAdd.value == "Show") 
                {
                    bConfirm = confirm("已有未轉出作業單，確認修改?");
//                    if (bConfirm && document.getElementById("ddlAction").value == "1") {
//                        alert("請輸入聯絡電話");
//                    }
                    return bConfirm;
                }
//                if (document.getElementById("ddlAction").value == "1" && hidAdd.value != "1") {
//                    alert("請輸入聯絡電話");
//                }
                break;
        }
    }
    </script>
    <script language="javascript" type="text/javascript">
        window.addEventListener("scroll", scroll, false);

        function scroll() {

            $("#divProgress").css("top", 290 + document.documentElement.scrollTop);

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
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_000" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" width="15%" style="height: 25px">
                            <cc1:CustLabel ID="lblId" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02020003_001" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <cc1:CustTextBox ID="txtId" MaxLength="16" runat="server" InputType="String" Width="174px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" width="15%" style="height: 25px">
                            <cc1:CustLabel ID="lblCardNo" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02020003_002" CurAlign="left" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <cc1:CustTextBox ID="txtCardNo" MaxLength="19" runat="server" InputType="String"
                                Width="174px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" width="15%" style="height: 25px">
                            <cc1:CustLabel ID="lblAction" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02020003_005" CurAlign="left" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <cc1:CustDropDownList runat="server" ID="ddlAction" AutoPostBack="true" OnSelectedIndexChanged="ddlAction_SelectedIndexChanged">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px" width="15%">
                        </td>
                        <td style="width: 35%; height: 25px" width="35%">
                            <asp:Button ID="btnSearch" runat="server" CssClass="smallButton" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="lblKind" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02020003_004" CurAlign="left" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <asp:Button ID="btnUpKind" runat="server" Text="修改" class="smallButton" Style="width: 50px;"
                                     OnClick="btnUpdateC_Click" OnClientClick="return IsConfirm('K')"/></div>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td width="15%" align="right" style="height: 25px">
                            <cc1:CustLabel ID="lblZipTl" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02020003_007" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblMaildateAjax" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" width="15%" style="height: 25px">
                            <cc1:CustLabel ID="lblAdd" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02020003_008" CurAlign="left" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <asp:Button ID="btnUpAdd" runat="server" Text="修改" class="smallButton" Style="width: 50px;"
                                     OnClick="btnUpdateA_Click" OnClientClick="return IsConfirm('A')" />
                            </div>
                        </td>
                    </tr>
                </table>
                <table id="tblChangeData" width="100%" border="0" cellpadding="0" cellspacing="1"
                    runat="server">
                    <tr class="itemTitle" runat="server" id="trl">
                        <td colspan="3">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_02020003_009" StickHeight="False"></cc1:CustLabel></li>
                            </li>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="height: 25px">
                            <cc1:CustGridView ID="grvUserView" runat="server" Width="100%" OnRowCommand="grvUserView_RowSelecting"
                                DataKeyNames="id" BorderStyle="Solid" CellSpacing="1" CellPadding="0" BorderWidth="0px"
                                PagerID="gpList" AllowPaging="False" AllowSorting="True" Height="1px" OnRowDataBound="grvUserView_RowDataBound">
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
                                    <asp:TextBox id="txtcNote" runat="server" Text='<%# Bind("CNote") %>' Width="280px" TextMode="MultiLine" onPropertyChange="textCounter(this,200)" Height="70px"  MaxLength="200"></asp:TextBox>
                                    </itemtemplate>
                                        <itemstyle width="40%" horizontalalign="Left"></itemstyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                    <cc1:CustButton id="btnSure" class="smallButton" runat="server" ShowID="06_06020003_010" Width="64px" CommandName="select" CommandArgument='<%# Bind("SNo") %>'></cc1:CustButton> 
                                    </itemtemplate>
                                        <itemstyle width="20%" horizontalalign="Center"></itemstyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <itemstyle width="40%" horizontalalign="Left"></itemstyle>
                                        <itemtemplate>
<asp:TextBox id="txtUser" runat="server" Text='<%# Bind("UpdUser") %>' Width="20px"></asp:TextBox> 
</itemtemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr align="center" class="itemTitle">
                        <td colspan="3" align="center" style="height: 25px">
                            <cc1:CustButton ID="btnBack" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_02020003_012" OnClick="btnBack_Click" />
                        </td>
                    </tr>
                </table>
                <!--地址一-->
                <asp:ImageButton ID="AddButtonA" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderA" runat="server" TargetControlID="AddButtonA"
                    PopupControlID="PanA" BackgroundCssClass="modal" CancelControlID="btnCancelA"
                    DropShadow="False" Y="150" />
                <asp:Panel ID="PanA" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="473px" Height="1px">
                    <table id="tbContextMailAdd" width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle">
                            <td colspan="2" style="height: 30px">
                                <li id="li7">
                                    <cc1:CustLabel ID="CustLabel69" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_02020003_015" StickHeight="False"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="center" colspan="2" style="width: 50%; height: 25px">
                                <cc1:CustLabel ID="CustLabel80" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_016" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd" id="category_tr" runat="server">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="category_Title" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_030" StickHeight="False">
                                </cc1:CustLabel>
                            </td>
                            <td align="left" style="width:30%; height:25px;">
                                <cc1:CustRadioButton ID="newCardnewAccount" runat="server" Checked="false" BorderStyle="None"
								GroupName="categroy_Item"/>
                                <cc1:CustRadioButton ID="newCardoldAccount" runat="server" Checked="false" BorderStyle="None"
								GroupName="categroy_Item" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel75" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_017" StickHeight="False"></cc1:CustLabel>
                                <label>
                                </label>
                            </td>
                            <td align="left" style="width: 30%; height: 25px">
                                <cc1:CustLabel ID="lblNewzipAjax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel76" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_018" StickHeight="False"></cc1:CustLabel>
                                <label>
                                </label>
                            </td>
                            <td align="left" style="width: 30%; height: 25px">
                                <uc1:CustAddress ID="CustAdd1" runat="server" OnChangeValues="CustAdd1_ChangeValues" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel77" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_019" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <label>
                                    <asp:TextBox ID="txtAdd2Ajax" runat="server" Width="240px"></asp:TextBox></label></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel78" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_020" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <label>
                                    <asp:TextBox ID="txtAdd3Ajax" runat="server" Width="240px"></asp:TextBox></label></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_028" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <label>
                                    <cc1:CustTextBox ID="txtUserName" runat="server" MaxLength="20" Width="240px"></cc1:CustTextBox></label>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_029" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <label>
                                    <cc1:CustTextBox ID="txtTel" runat="server" MaxLength="10" Width="240px"></cc1:CustTextBox></label>
                            </td>
                        </tr>
                        <tr class="trEven" align="center">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_014" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 31%; height: 25px">
                                <span style="width: 100%; height: 25px">
                                    <asp:TextBox ID="txtCNoteA" runat="server" Height="60px" TextMode="MultiLine" onPropertyChange="textCounter(this,200)"
                                        Width="266px" MaxLength="200"></asp:TextBox></span></td>
                        </tr>
                        <tr align="left" class="itemTitle">
                            <td colspan="2" align="center" style="height: 25px">
                                &nbsp;<asp:Button ID="btnSureA" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureA_Click"
                                    OnClientClick="return check('A')" />
                                <asp:Button ID="btnCancelA" runat="server" Text="取消" CssClass="smallButton" /></td>
                        </tr>
                    </table>
                </asp:Panel>
                &nbsp;<!--取卡方式-->
                <asp:ImageButton ID="AddButtonC" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderC" runat="server" TargetControlID="AddButtonC"
                    PopupControlID="PanC" BackgroundCssClass="modal" CancelControlID="btnCancelC"
                    DropShadow="False" />
                <asp:Panel ID="PanC" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="336px" Height="83px">
                    <table border="0" cellpadding="0" cellspacing="1" style="width: 122%;">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li id="li5">
                                    <cc1:CustLabel ID="CustLabel59" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_02020003_022" StickHeight="False"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel61" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_023" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 50%;">
                                <asp:DropDownList ID="dropKindAjax" runat="server" Width="160px">
                                </asp:DropDownList></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel63" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_014" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 50%;">
                                <asp:TextBox ID="txtCNoteC" runat="server" Height="60px" TextMode="MultiLine" onPropertyChange="textCounter(this,200)"
                                    Width="288px" MaxLength="200"></asp:TextBox></td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="2" align="center" style="height: 25px">
                                <asp:Button ID="btnSureC" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureC_Click"
                                    OnClientClick="return check('C')" />
                                <asp:Button ID="btnCancelC" runat="server" Text="取消" CssClass="smallButton" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:HiddenField ID="hidWay" runat="server" />
                <asp:HiddenField ID="hidAdd" runat="server" />
                <asp:HiddenField ID="hidStatus" runat="Server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="updateProgress1" runat="server">

            <ProgressTemplate>

                <div id="divProgress" align="center" class="progress" style="position: absolute;

                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center; z-index:10003;">

                    <div id="divProgress2" align="center" class="progress" style="background-color: White;

                        width: 50%; margin: 0px auto; z-index:10004;">

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
