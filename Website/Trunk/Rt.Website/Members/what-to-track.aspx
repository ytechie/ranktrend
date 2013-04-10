<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="what-to-track.aspx.cs"
	Inherits="Members_what_to_track" Title="What to Track" %>

<%@ Register TagPrefix="rt" TagName="SiteList" Src="~/Members/CommonControls/SiteList.ascx" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<rt:SiteList runat="Server" ID="siteList" AutoPostBack="true" /><br />
	<br />
	
	Keywords (1 per line):<br />
	<asp:Button runat="server" ID="cmdSave" Text="Save" /><br />
	<asp:TextBox runat="server" ID="txtKeywords" TextMode="MultiLine" Rows="20" Columns="50" /><br />
	<asp:Button runat="server" ID="cmdSave2" Text="Save" />
</asp:Content>