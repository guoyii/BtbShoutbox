<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InputControl.ascx.cs" Inherits="BiteTheBullet.BtbShoutbox.InputControl" %>

<div class="shoutMessageInput">
    <asp:Panel ID="pnlOpenUser" runat="server">
        <div>Username</div>
        <input type="text" id="txtUsername" class="txtUsername" runat="server" />
    </asp:Panel>

    <div>Message</div>
    <textarea runat="server" rows="2" cols="30" id="txtMessage" class="txtMessage"></textarea><br />
    <input type="button" class="shoutPostButton" id="shoutPostButton" runat="server" value="Shout" />
    <div id="divRefresh" runat="server"><a href="#" id="refreshLink">Refresh</a></div>

    <div id="profanityAlert">
        <img id="imgWarning" runat="server" src="img/dialog-warning.png" alt="Warning Icon" style="float:left;" />
        <asp:Label runat="server" ID="lblProfanityCaption" CssClass="shoutWarningMessage" resourcekey="lblProfanityCaption"></asp:Label>
    </div>

    <div id="voteFloodAlert">
        <img id="imgWarning2" runat="server" src="img/dialog-warning.png" alt="Warning Icon" style="float:left;" />
        <asp:Label runat="server" ID="lblFloodAlertCaption" CssClass="shoutWarningMessage" resourcekey="lblFloodAlertCaption"></asp:Label>    
    </div>
</div>