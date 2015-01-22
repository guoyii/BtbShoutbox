<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="BiteTheBullet.BtbShoutbox.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" width="650" cellpadding="2" border="0" summary="ModuleName1 Settings Design Table">
    
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblFilterProfanity" runat="server" controlname="chkFilter" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkFilter" runat="server" />
        </td>        
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblItemLimit" runat="server" controlname="txtItemLimit" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtItemLimit" cssclass="NormalTextBox" columns="5" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblCharacterLimit" runat="server" controlname="txtCharacterLimit" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtCharacterLimit" cssclass="NormalTextBox" columns="4" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblOnlyRegistered" runat="server" controlname="chkOnlyRegistered" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkOnlyRegistered" runat="server" />
        </td>        
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblAutoRefresh" runat="server" controlname="chkAutoRefresh" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkAutoRefresh" runat="server" />
        </td>        
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblTopOfModule" runat="server" controlname="chkTopOfModule" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkTopOfModule" runat="server" />
        </td>        
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblUserCaption" runat="server" controlname="rbDisplayName" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:RadioButton runat="server" ID="rbDisplayName" Checked="true" resourcekey="rbDisplayName" GroupName="rbgUserCaption" />
            <asp:RadioButton runat="server" ID="rbUsername" resourcekey="rbUsername" GroupName="rbgUserCaption" />
        </td>        
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblDisplayOlderLink" runat="server" controlname="chkDisplayOlderLink" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkDisplayOlderLink" runat="server" />
        </td>        
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblPurgeAgeLimit" runat="server" controlname="txtPurgeAgeLimit" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtPurgeAgeLimit" cssclass="NormalTextBox" columns="4" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblFloodControlEnabled" runat="server" controlname="chkFloodControlEnabled" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:CheckBox ID="chkFloodControlEnabled" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblTemplate" runat="server" controlname="txtTemplate" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtTemplate" cssclass="NormalTextBox" width="390" columns="30" textmode="MultiLine" rows="10" maxlength="2000" runat="server" />
            <asp:LinkButton runat="server" ID="lnkReloadDefault" 
                resourcekey="lnkReloadDefault" onclick="lnkReloadDefault_Click" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200" valign="top"><dnn:label id="lblEmailAdmin" runat="server" controlname="txtEmailAdmin" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtEmailAdmin" cssclass="NormalTextBox" columns="35" runat="server" />
        </td>
    </tr>
</table>