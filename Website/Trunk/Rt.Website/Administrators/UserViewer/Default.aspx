<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Administrators_UserViewer_Default" Title="RankTrend.com - User Viewer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" Runat="Server">
	<ajax:AjaxPanel ID="AjaxPanel1" runat="server">
		<asp:Label runat="server" ID="UserLabel" AssociatedControlID="txtUser" Text="User:" />
		<asp:TextBox runat="server" ID="txtUser" />
		<asp:Button runat="server" ID="Search" Text="Search" OnClick="Search_Click" />
		<br /><br />
		<asp:Panel runat="server" ID="UserInformationPanel" Visible="false">
			<b>User's Information:</b>
			<asp:Button runat="server" ID="cmdLogon" Text="Log In as this user" />
			<asp:DetailsView runat="server" ID="UserInformationDetails1" CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem" />
			<br />
			<b>Sites:</b>
			<asp:GridView runat="server" ID="UserInformationDetails2" CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem" />
			<br />
			<b>Datasources</b>
			<asp:GridView runat="server" ID="UserInformationDetails3" CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem" />
		</asp:Panel>
		<asp:Panel runat="server" ID="UserNotFoundPanel" Visible="false">
			Could not find the user specified.
		</asp:Panel>
	</ajax:AjaxPanel>
</asp:Content>

