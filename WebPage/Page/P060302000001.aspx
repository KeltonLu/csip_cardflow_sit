<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060302000001.aspx.cs" Inherits="P060302000001" %>

<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>匯入匯出作業>手動大宗檔錯誤處理</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
      function checkDate()
        {
            if(document.getElementById("dateFrom_foo").value=="")
            {
                alert("請輸入開始時間！");
                document.getElementById("dateFrom_foo").focus();
                return false;
            }
            if(document.getElementById("dateTo_foo").value=="")
            {
                alert("請輸入結束時間！");
                document.getElementById("dateTo_foo").focus();
                return false;
            }
            return true;
        }
    function del()
    {
        if(!confirm('確定要刪除該文件嗎？'))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    </script>

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
                    top: 289px; width: 100%; filter: Alpha(opacity=80); text-align: center; left: 12px;">
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnUpload" />
                <asp:PostBackTrigger ControlID="grvUserView" />
            </Triggers>
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="6" style="height: 25px">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="center" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06030200_000" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" style="width: 25%; height: 25px;">
                            <cc1:CustRadioButton ID="radError" runat="server" Checked="true" OnCheckedChanged="radError_CheckedChanged"
                                GroupName="File" AutoPostBack="true" />
                        </td>
                        <td align="right" style="width: 25%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06030200_004" CurAlign="right"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="4" style="width: 50%; height: 25px;">
                            <cc1:DatePicker ID="dateFrom" runat="server" Width="70">
                            </cc1:DatePicker>
                            ~
                            <cc1:DatePicker ID="dateTo" runat="server" Width="70">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="left" style="width: 25%; height: 25px;" rowspan="3">
                            <cc1:CustRadioButton ID="radUpload" runat="server" OnCheckedChanged="radUpload_CheckedChanged"
                                GroupName="File" AutoPostBack="true" />
                        </td>
                        <td align="right" style="width: 25%; height: 25px;">
                            <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06030200_003" CurAlign="right"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 20%; height: 25px;">
                            <asp:FileUpload ID="fupFile" runat="server" Enabled="false" Width="280px" />
                        </td>
                        <td style="width: 10%; height: 5px;">
                            <cc1:CustDropDownList ID="ddlCard" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                        <td style="width: 10%; height: 5px;">
                            <cc1:CustDropDownList ID="ddlFile" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                        <td style="height: 5px; width: 10%">
                            <cc1:CustDropDownList ID="ddlFactory" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06030200_020" CurAlign="right"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td colspan="4">
                            <cc1:DatePicker ID="dprTranDate" runat="Server">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06030200_021" CurAlign="right"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td colspan="4">
                            <cc1:DatePicker runat="Server" ID="dprInDate">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="6" style="height: 25px">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" Style="width: 50px;"
                                ShowID="06_06030200_009" OnClick="btnSearch_Click" OnClientClick="return  checkDate();"
                                DisabledWhenSubmit="false" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnUpload" runat="server" CssClass="smallButton" Style="width: 50px;"
                                ShowID="06_06030200_010" OnClick="btnUpload_Click" Enabled="FALSE" /></td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td colspan="6">
                            <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                                PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" OnRowCommand="grvUserView_RowCommand" DataKeyNames="ErrorID">
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="ImportTime">
                                        <itemstyle width="25%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ImportFileName">
                                        <itemstyle width="50%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="25%" />
                                        <itemtemplate> 
                                        <cc1:CustButton style="WIDTH: 50px" id="btnDownLoad"  runat="server" ShowID="06_06030200_014" CssClass="smallButton" __designer:wfdid="w7" CommandName="DownLoad" CommandArgument='<%# Container.DataItemIndex %>' ></cc1:CustButton>
                                        <asp:HiddenField id="hidFilePath" runat="server" Value='<%# Bind("LocalFilePath") %>'></asp:HiddenField> 
                                        <asp:HiddenField id="hidJob" runat="server" Value='<%# Bind("JobID") %>'></asp:HiddenField>
                                        &nbsp;&nbsp; 
                                        <cc1:CustButton style="WIDTH: 50px" id="btnDelete"  runat="server" ShowID="06_06030200_015" CssClass="smallButton" __designer:wfdid="w8" CommandName="Del" CommandArgument='<%# Container.DataItemIndex %>' OnClientClick="return del();"  DisabledWhenSubmit="false"></cc1:CustButton>
                                        <asp:HiddenField id="hidValue" runat="server" Value='<%# Bind("LoadFlag") %>' __designer:wfdid="w9"></asp:HiddenField> 
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
            </ContentTemplate>
        </asp:UpdatePanel>
        <iframe id="iDownLoadFrame1" width="0Px" height="0Px"></iframe>
    </form>
</body>
</html>
