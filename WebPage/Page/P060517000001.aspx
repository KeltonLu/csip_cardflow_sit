<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060517000001.aspx.cs" Inherits="P060517000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>報表查詢>退件原因統計表</title>

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
			function scroll() {
				$("#divProgress").css("top", 290 + document.documentElement.scrollTop);
			}
		</script>

		<asp:UpdateProgress ID="updateProgress1" runat="server">
			<ProgressTemplate>
				<div id="divProgress" align="center" class="progress" style="position: absolute; top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">
					<div id="divProgress2" align="center" class="progress" style="background-color: White; width: 50%; margin: 0px auto;">
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
			<ContentTemplate>
				<table width="100%" border="0" cellpadding="0" cellspacing="1">
					<tr class="itemTitle">
						<td colspan="4" style="height: 26px">
							<li>
								<cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
									IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
									SetBreak="False" SetOmit="False" ShowID="06_06051700_000" StickHeight="False">
								</cc1:CustLabel>
							</li>
						</td>
					</tr>
					<tr class="trOdd">
						<td align="right" style="width: 15%">
							<cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
								IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
								SetOmit="False" StickHeight="False" ShowID="06_06051700_001" CurAlign="left"
								CurSymbol="£"></cc1:CustLabel>
						</td>
						<td style="width: 35%">
							<cc1:DatePicker ID="dateFrom" runat="server" Width="70">
							</cc1:DatePicker>
							~
                            <cc1:DatePicker ID="dateTo" runat="server" Width="70">
							</cc1:DatePicker>
						</td>
						<td align="right" style="width: 15%">
							<cc1:CustLabel ID="CustLabel2" runat="server" FractionalDigit="2" IsColon="True"
								IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
								SetOmit="False" StickHeight="False" ShowID="06_06051700_003" CurAlign="left"
								CurSymbol="£"></cc1:CustLabel>
						</td>
						<td align="left" style="width: 35%; height: 30px;">
							<cc1:CustDropDownList ID="ddlAction" runat="server">
							</cc1:CustDropDownList>
						</td>
					</tr>
					<tr align="center" class="itemTitle">
						<td colspan="4">
							<cc1:CustButton ID="btnSearh" runat="server" CssClass="smallButton" Style="width: 50px;"
								ShowID="06_06051700_002" OnClick="btnSearch_Click" />&nbsp;&nbsp;
								<cc1:CustButton ID="btnPrint" runat="Server" class="smallButton" Style="width: 50px;"
									ShowID="06_06051700_005" OnClick="btnPrint_Click" />
						</td>
					</tr>
				</table>
				<table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
					<tr>
						<td colspan="20">
							<cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
								PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ReasonName">
										<ItemStyle Width="22%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="Action1">
										<ItemStyle Width="13%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="Action2">
										<ItemStyle Width="13%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="Action3">
										<ItemStyle Width="13%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="Action4">
										<ItemStyle Width="13%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="Action5">
										<ItemStyle Width="13%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="Action6">
										<ItemStyle Width="13%" HorizontalAlign="Center" />
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
			</ContentTemplate>
		</asp:UpdatePanel>
	</form>
</body>
</html>
