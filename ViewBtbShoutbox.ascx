<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewBtbShoutbox.ascx.cs" Inherits="BiteTheBullet.BtbShoutbox.ViewBtbShoutbox" %>

<asp:Literal ID="litScript" runat="server" />

<!-- IE 6 "fixes" -->
<!--[if lt IE 7]>
<asp:Literal ID="litCss" runat="server" />
<![endif]-->

<script type="text/javascript">
/* <![CDATA[ */

function HandlerUrl(){    
    return "<%= AjaxHandlerPath %>";
}


jQuery(document).ready(function(){

    <% if(EnableAutoRefresh){ %>
        autoRefresh();
    <%} %>

});
/* ]]> */
</script>

<asp:PlaceHolder runat="server" ID="phTopInput"></asp:PlaceHolder>

<asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
<HeaderTemplate>
    <ul class="shoutMessageList">
</HeaderTemplate>
<ItemTemplate>
    <li><asp:Literal runat="server" ID="chatItem"></asp:Literal></li>
</ItemTemplate>
<FooterTemplate>
    </ul>
</FooterTemplate>
</asp:Repeater>

<div runat="server" id="divDisplayOlder" class="shoutDisplayOlder"><a href="#"><asp:Label runat="server" ID="lblDisplayOlder" resourcekey="lblDisplayOlder" /></a></div>
<asp:HiddenField runat="server" ID="btbShoutModuleId" />
<asp:HiddenField runat="server" ID="btbShoutTabModuleId" />
<asp:PlaceHolder runat="server" ID="phBottomInput"></asp:PlaceHolder>
