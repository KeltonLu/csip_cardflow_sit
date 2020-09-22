<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060207000001.aspx.cs" Inherits="P060207000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>自取庫存日結</title>

	<script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

	<script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

	<script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

	<script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

	<link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

	<script type="text/javascript">
		function Check(type) {
			var DailyClose = document.getElementById("radDailyClose");
			var StockInfo = document.getElementById("radStockInfo");
			var nowDate = new Date();
			var year = nowDate.getYear();
			var month = nowDate.getMonth() + 1;
			var day = nowDate.getDate();
			var strDate = "";
			strDate += year;
			strDate += "/"
			strDate += (month < 10) ? ("0" + month) : month;
			strDate += "/"
			strDate += (day < 10) ? ("0" + day) : day;

			switch (type) {
				case "0":
					if (!DailyClose.checked) {
						alert("請點選【自取日結日期】進行日結操作！");
						return false;
					}
					var CloseDate = document.getElementById("lblNowCloseDate");
					var nowDate = new Date();

					if (CloseDate.innerText == "") {
						alert("日結日期不可為空！");
						return false;
					}
					else {
						if (CloseDate.innerText > strDate) {
							alert("日結日期不過超過當日！");
							return false;
						}
					}
					break;

				case "1":
					if (!DailyClose.checked) {
						alert("請點選【自取日結日期】進行取消日結操作！");
						return false;
					}
					var LastCloseDate = document.getElementById("lblLastCloseDate");
					if (LastCloseDate.innerText == "") {
						alert("最後日結日期不可為空！");
						return false;
					}
					else {
						if (LastCloseDate.innerText != strDate) {
							alert("只能取消當日日結！");
							return false;
						}
					}
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
						<td colspan="2" style="height: 25px">
							<li>
								<cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
									IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
									SetBreak="False" SetOmit="False" ShowID="06_06020700_000" StickHeight="False"></cc1:CustLabel></li>
						</td>
					</tr>
					<tr class="trEven">
						<td style="height: 25px; width: 100%" colspan="2">
							<cc1:CustRadioButton ID="radDailyClose" runat="server" Checked="true" BorderStyle="None"
								GroupName="DailyClose" /></td>
					</tr>
					<tr class="trOdd">
						<td align="right" style="height: 25px; width: 15%">
							<cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
								IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
								SetOmit="False" StickHeight="False" ShowID="06_06020700_001" CurAlign="left"
								CurSymbol="£"></cc1:CustLabel></td>
						<td style="height: 25px; width: 85%">
							<cc1:CustLabel ID="lblLastCloseDate" runat="server" CurAlign="left" CurSymbol="&#163;"
								FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
								NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
					</tr>
					<tr class="trEven">
						<td align="right" style="height: 25px; width: 15%">
							<cc1:CustLabel ID="CustLabe2" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
								NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
								StickHeight="False" ShowID="06_06020700_002" CurAlign="left" CurSymbol="£"></cc1:CustLabel></td>
						<td style="height: 25px; width: 85%">
							<cc1:CustLabel ID="lblNowCloseDate" runat="server" CurAlign="left" CurSymbol="&#163;"
								FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
								NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
					</tr>
					<tr class="trOdd">
						<td style="height: 25px; width: 100%" colspan="2">
							<cc1:CustRadioButton ID="radStockInfo" runat="server" GroupName="DailyClose" /></td>
					</tr>
					<tr class="trEven">
						<td align="right" style="height: 25px; width: 15%">
							<cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
								IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
								SetOmit="False" StickHeight="False" ShowID="06_06020700_003" CurAlign="left"
								CurSymbol="£"></cc1:CustLabel></td>
						<td style="height: 25px; width: 85%">
							<cc1:DatePicker ID="dpCloseDate" runat="server">
							</cc1:DatePicker>
						</td>
					</tr>
					<tr align="center" class="itemTitle">
						<td colspan="2">
							<asp:Button ID="btnClose" runat="server" CssClass="smallButton" Style="width: 50px"
								OnClientClick="javascript:return Check('0');" OnClick="btnClose_Click" />&nbsp;&nbsp;
                            <asp:Button ID="btnCancelClose" runat="server" CssClass="smallButton" OnClientClick="javascript:return Check('1');"
								OnClick="btnCancelClose_Click" Width="80px" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
								ShowID="06_06020700_006" OnClick="btnSearch_Click" />&nbsp;&nbsp;
							<cc1:CustButton ID="btnPrint" runat="Server" class="smallButton" Style="width: 50px;"
								ShowID="06_06020700_009" OnClick="btnPrint_Click" />
						</td>
					</tr>
				</table>
			</ContentTemplate>
		</asp:UpdatePanel>

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

	</form>
</body>
</html>
