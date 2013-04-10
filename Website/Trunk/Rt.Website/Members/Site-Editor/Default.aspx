<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Members_Site_Editor_Default"
	Title="Site Editor" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<ajax:AjaxPanel runat="server">
		<table>
			<tr>
				<td>
					<asp:ListBox runat="server" ID="lstSites" CssClass="eventListBox" AutoPostBack="true" /><br />
					<asp:Button runat="server" ID="cmdAdd" text="Add" />
					<asp:Button runat="server" ID="cmdDelete" text="Delete" />
				</td>
				<td valign="top">
					<b>Site Details:</b><br />
					Site URL: <asp:TextBox runat="server" ID="txtSiteUrl" Columns="40" /><br />

					<asp:Button runat="server" ID="cmdSave" Text="Save" /><br />
					<b>Please Note:</b> It is highly recommended that you do not change a site that already
						has data or other settings associated with it.  Data collected from now on will be
						affected.  If you are moving a page to a different URL, it is recommended that you
						add a
						<a href="../Events/Event-Editor/">custom event</a>
						so that you can keep track of when the change occurred.
				</td>
			</tr>
		</table>
	</ajax:AjaxPanel>
</asp:Content>