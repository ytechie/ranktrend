<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Members_Events_Event_Category_Editor_Default"
	Title="Event Category Editor" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<ajax:AjaxPanel runat="server">
		<rt:SiteList runat="server" id="siteList" AutoPostBack="True" />
		<hr />
		<table>
			<tr>
				<td>
					<asp:ListBox runat="server" ID="lstCategories" CssClass="eventListBox" AutoPostBack="true" /><br />
					<asp:Button runat="server" ID="cmdAdd" text="Add" />
					<asp:Button runat="server" ID="cmdDelete" text="Delete" />
				</td>
				<td valign="top">
					<b>Event Category Details:</b><br />
					Name: <asp:TextBox runat="server" ID="txtName" /><br />

					<asp:Button runat="server" ID="cmdSave" Text="Save" />
				</td>
			</tr>
		</table>
	</ajax:AjaxPanel>
</asp:Content>