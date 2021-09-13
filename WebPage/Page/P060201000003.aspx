<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060201000003.aspx.cs" Inherits="P060201000003" %>

<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>綜合資料新增 </title>

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
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020102_000" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtId" MaxLength="11" runat="server" InputType="String" Width="150px"></cc1:CustTextBox>&nbsp;
                            *</td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel10" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_002" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList runat="server" ID="dropAction">
                            </cc1:CustDropDownList></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_003" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <%--2020/12/31 陳永銘 收件者姓名:修改欄位最大長度,寬度--%>
                            <cc1:CustTextBox ID="txtCustname" MaxLength="50" runat="server" InputType="String"
                                Width="90%">
                            </cc1:CustTextBox>&nbsp; *</td>
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel11" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_004" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtCardtype" MaxLength="3" runat="server" InputType="String"
                                Width="150px"></cc1:CustTextBox>&nbsp; *</td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%; height: 25px;">
                            <%--2020/12/31 陳永銘 新增名稱:收件人姓名_羅馬拼音 BEGIN--%>
                            <cc1:CustLabel ID="CustLabel22" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_024" CurAlign="left"
                                CurSymbol="£">
                            </cc1:CustLabel>
                            <%--2020/12/31 陳永銘 新增名稱:收件人姓名_羅馬拼音 END--%>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <%--2020/12/31 陳永銘 新增欄位:收件人姓名_羅馬拼音 BEGIN--%>
                            <cc1:CustTextBox ID="txtCustname_Roma" MaxLength="50" runat="server" InputType="String"
                                Width="90%">
                            </cc1:CustTextBox></td>
                        <%--2020/12/31 陳永銘 新增欄位:收件人姓名_羅馬拼音 END--%>
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel12" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_006" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtAffinity" MaxLength="4" runat="server" InputType="Int" Width="150px"></cc1:CustTextBox>&nbsp;
                            *</td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_007" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:DatePicker ID="txtIndate1" runat="server" Width="150">
                            </cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel13" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_008" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtPhoto" MaxLength="2" runat="server" InputType="String" Width="150px"></cc1:CustTextBox>&nbsp;
                            *</td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_007" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="txtMaildate" runat="server" Width="150">
                            </cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel14" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_010" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtMonlimit" MaxLength="8" runat="server" InputType="Int" Width="150px"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_009" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="txtMaildate" runat="server" Width="150">
                            </cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel15" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_012" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtCardno2" MaxLength="19" runat="server" InputType="String"
                                Width="150px"></cc1:CustTextBox>&nbsp;
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_011" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtCardno" runat="server" InputType="String" MaxLength="19" Width="150px"></cc1:CustTextBox>
                            *&nbsp;&nbsp; </td>
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel8" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_014" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtExpdate2" runat="server" InputType="String" MaxLength="4"
                                MaxValue="" Width="38px"></cc1:CustTextBox>&nbsp; *
                            <asp:Label ID="Label2" runat="server" ForeColor="#00C0C0" Text="輸入格式MMYY" Width="91px"></asp:Label></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_013" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtExpdate" runat="server" InputType="String" MaxLength="4" MaxValue="" Width="38px"></cc1:CustTextBox>&nbsp; *
                            <asp:Label ID="Label1" runat="server" ForeColor="#00C0C0" Text="輸入格式MMYY" Width="91px"></asp:Label>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel16" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_016" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList runat="server" ID="dropKind">
                            </cc1:CustDropDownList></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_015" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel18" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_018" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtMailno" MaxLength="20" runat="server" InputType="String"
                                Width="150px"></cc1:CustTextBox>&nbsp;
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_017" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <uc1:CustAddress ID="dropAdd1" runat="server" OnChangeValues="dropAdd1_ChangeValues" />
                            &nbsp;
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel20" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020102_020" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList runat="server" ID="dropMerch_Code">
                            </cc1:CustDropDownList></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_019" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <asp:TextBox ID="txtAdd2" runat="server" MaxLength="40" onkeydown="textCounter(this,40)" onkeyup="textCounter(this,40)" Width="220px"></asp:TextBox>
                            &nbsp;
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="06_06020102_021" StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <asp:TextBox ID="txtAdd3" runat="server" IsValRange="False" onkeydown="textCounter(this,40)" onkeyup="textCounter(this,40)" Width="220px"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4" align="center">&nbsp; &nbsp;&nbsp;
                            <cc1:CustButton ID="btnAdd" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020102_022" OnClick="btnAdd_Click" />&nbsp;
                            <cc1:CustButton ID="btnCancel" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020102_023" OnClick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
