<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060608000001.aspx.cs" Inherits="P060608000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mail警訊通知</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script language="javascript" type="text/javascript">
    
    //将源listbox选择项移动到目标listbox
    function moveSelect(sourceList, targetList,flag)
        {
            var lst1=window.document.getElementById(sourceList);
            var lst2=window.document.getElementById(targetList);
            var lst3=window.document.getElementById('hflist');
            var len = lst1.options.length;
            var va = '';
            for(var i=0;i<len;i++)
            {
               var lstindex=lst1.selectedIndex;
               if(lstindex<0)
               return;
               var v = lst1.options[lstindex].value;
               var t = lst1.options[lstindex].text;
               if(!hasValue(targetList,v))
               {
                  lst2.options[lst2.options.length] = new Option(t,v,true,true);
                  if(flag=='A')
                  {
                     va = v + ';' + va;   
                  }
               }
               removeSelect(sourceList);
               lst3.value += va;
               if(flag=='B')
               {
                  GetOption(sourceList);
               }  
            }     
        }
        
     function GetOption(listId)
     {
         var lst2=window.document.getElementById(listId);
         var lst3=window.document.getElementById('hflist');
         var va = '';
         lst3.value = '';
         for(var j=0;j<lst2.options.length;j++)
         {
             va = lst2.options[j].value + ';' + va; 
             lst3.value += va;
         }
     }   
     //移除源listbox选择项   
     function removeSelect(listId)
        {
            var lst2=window.document.getElementById(listId);
            var lstindex=lst2.selectedIndex;
            if(lstindex>=0)
            {
                var v = lst2.options[lstindex].value+";";
                lst2.options[lstindex].parentNode.removeChild(lst2.options[lstindex]);
            }
                
        }
    //判断listbox是否存在该项
    function hasValue(listId,v){
    var lst = window.document.getElementById(listId);
    var length = lst.options.length;
    for(var i = 0; i < length; i++){
     var vv = lst.options[i].value;
     if(v == vv){
      return true;
     }
    }
    return false;
   }
   
   //移除listbox所有项
    function removeAll(listId)
        {
            var lst=window.document.getElementById(listId);
            var length = lst.options.length;
            for(var i=length;i>0;i--)
            {
                lst.options[i-1].parentNode.removeChild(lst.options[i-1]);
            }    
        }
   //将源listbox选择项全部移动到目标listbox
       function moveAll(sourceList, targetList,flag){
            var lst1=window.document.getElementById(sourceList);
            var lst2=window.document.getElementById(targetList);
            var lst3=window.document.getElementById('hflist');
            var length = lst1.options.length;
            for(var i=0;i<length;i++)
            {
                var v = lst1.options[i].value;
                var t = lst1.options[i].text;
                if(!hasValue(targetList,v)){
                //目录列表中没有当前值，则加入
                lst2.options[lst2.options.length] = new Option(t,v,true,true); 
                if(flag=='A')
                {  
                   lst3.value = v+';'+ lst3.value;
                }
                }
            }
            //把源列表中的内容移除
            removeAll(sourceList);
            if(flag=='B')
            {
                lst3.value='';
            }
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
                                    SetBreak="False" SetOmit="False" ShowID="06_06060800_000" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td colspan="20">
                            <cc1:CustGridView ID="grvCardView" runat="server" AllowSorting="True" AllowPaging="False"
                                PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" DataKeyNames="MailID" >
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="JobName">
                                        <itemstyle width="15%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ConditionName">
                                        <itemstyle width="15%" horizontalalign="Left" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MailTittle">
                                        <itemstyle width="15%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MailContext">
                                        <itemstyle width="15%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                        <cc1:CustLabel id="lblToUser" runat="server" StickHeight="False" SetOmit="False" SetBreak="False" NumOmit="0" NumBreak="0" NeedDateFormat="False" IsCurrency="False" FractionalDigit="2" CurSymbol="£" CurAlign="left" Text="<%#Bind('ToUserName') %>" __designer:wfdid="w1"></cc1:CustLabel> <cc1:CustButton style="WIDTH: 50px" id="btnToUser" class="smallButton" runat="server" ShowID="06_06060800_009" __designer:wfdid="w2" OnClick="btnToUser_Click" CommandName="<%#Bind('MailID') %>" CommandArgument="<%#Container.DataItemIndex %>"></cc1:CustButton> 
                                        </itemtemplate>
                                        <itemstyle horizontalalign="Left" width="20%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                        <cc1:CustLabel id="lblCCUser" runat="server" StickHeight="False" SetOmit="False" SetBreak="False" NumOmit="0" NumBreak="0" NeedDateFormat="False" IsCurrency="False" FractionalDigit="2" CurSymbol="£" CurAlign="left" Text="<%#Bind('CcUserName') %>" __designer:wfdid="w3"></cc1:CustLabel> <cc1:CustButton style="WIDTH: 50px" id="btnCCUser" class="smallButton" runat="server" ShowID="06_06060800_009" __designer:wfdid="w4" OnClick="btnCCUser_Click" CommandName="<%#Bind('MailID') %>" CommandArgument="<%#Container.DataItemIndex %>"></cc1:CustButton> 
                                        </itemtemplate>
                                        <itemstyle horizontalalign="Left" width="20%" />
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
                <asp:ImageButton ID="DetailButton" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderM" runat="server" TargetControlID="DetailButton"
                    PopupControlID="PanM" BackgroundCssClass="modal" CancelControlID="btnCancelM"
                    DropShadow="False" />
                <asp:Panel ID="PanM" CssClass="workingArea" runat="server" Width="400px" Height="150px" Style="display: none">
                    <table border="0" cellpadding="0" cellspacing="1" style="width: 100%;">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li id="li3">
                                    <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_06060800_011" StickHeight="False"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="height: 25px; width: 15%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="06_06060800_007" CurAlign="left"
                                    CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 35%">
                                <cc1:CustDropDownList ID="ddlRole" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">
                                </cc1:CustDropDownList>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table2">
                        <tr>
                            <td style="width: 40%">
                                <asp:ListBox ID="lbSource" runat="server" SelectionMode="Multiple" Width="100%" Height="120px">
                                </asp:ListBox>
                                <asp:HiddenField ID="hflist" runat="server" />
                                <input id="Hidden1" type="hidden" />
                            </td>
                            <td style="width: 20%" align="center">
                                <table>
                                    <tr>
                                        <td>
                                        <asp:Button runat="server" ID="btnAddAll" Text=">>" CssClass="smallButton" style="width: 50px;" OnClick="btnAddAll_Click" />
                                         <%--   <input id="btnAddAll" type="button" value=">>" class="smallButton" style="width: 50px;" onclick="moveAll('lbSource','lbTarget','A')"/>--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        <asp:Button runat="server" ID="btnAdd" Text=">" CssClass="smallButton" style="width: 50px;" OnClick="btnAdd_Click" />
                                        <%--<input id="btnAdd" type="button" value=">" class="smallButton" style="width: 50px;" onclick="moveSelect('lbSource','lbTarget','A')"/>--%>
                                            <%--<asp:Button ID="btnAdd" runat="server" Text=">" class="smallButton" Style="width: 50px;"
                                                OnClientClick="moveSelect('lbSource','lbTarget')" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        <asp:Button runat="server" ID="btnRemove" Text="<" CssClass="smallButton" style="width: 50px;" OnClick="btnRemove_Click" />
                                        <%--    <input id="btnRemove" type="button" value="<" class="smallButton" style="width: 50px;" onclick="moveSelect('lbTarget','lbSource','B')" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        <asp:Button runat="server" ID="btnRemoveAll" Text="<<" CssClass="smallButton" style="width: 50px;" OnClick="btnRemoveAll_Click" />
                                         <%--   <input id="btnRemoveAll" type="button" value="<<" class="smallButton" style="width: 50px;" onclick="moveAll('lbTarget','lbSource','B')"/>--%>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 40%">
                                <asp:ListBox ID="lbTarget" runat="server" SelectionMode="Multiple" Width="100%" Height="120px">
                                </asp:ListBox>
                            </td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="3" align="center">
                                <asp:Button ID="btnSave" runat="server" Text="保存" CssClass="smallButton" OnClick="btnSave_Click" />
                                <asp:Button ID="btnCancelM" runat="server" Text="取消" CssClass="smallButton" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
