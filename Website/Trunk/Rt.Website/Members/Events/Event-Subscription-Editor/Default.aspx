<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Members_Events_Event_Subscription_Editor_Default"
	Title="Event Subscription Editor" %>

<%@ Register TagPrefix="rt" TagName="EventCategoryList" Src="~/Members/CommonControls/EventCategoryList.ascx" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<ajax:AjaxPanel runat="server">
		<rt:SiteList runat="server" id="siteList" AutoPostBack="True" />
		<hr />
		<table>
			<tr>
				<td>
					<asp:ListBox runat="server" ID="lstSubscriptions" CssClass="eventListBox" AutoPostBack="true" /><br />
					<asp:Button runat="server" ID="cmdAdd" text="Add" />
					<asp:Button runat="server" ID="cmdDelete" text="Delete" />
				</td>
				<td valign="top">
					<b>RSS Subscription Details:</b><br />
					Name: <asp:TextBox runat="server" ID="txtName" Columns="40" />
					<asp:LinkButton runat="server" ID="cmdReadFeedTitle" Text="Load Information from Feed" />
					<br />
					Description:<br />
					<asp:TextBox runat="server" ID="txtDescription" Rows="3" Columns="50" TextMode="MultiLine" /><br />
					RSS Url: <asp:TextBox runat="server" ID="txtRssUrl" Columns="60" /><br />
					Category: <rt:EventCategoryList runat="server" ID="categoryList" /><br />
					Last Check Time: <asp:Literal runat="server" ID="lblLastCheck" /><br />

					<asp:Button runat="server" ID="cmdSave" Text="Save" /><br />
					<b>Please Note:</b> Changes made to an RSS feed will not affect the events that have already been created.  It will
						only change the events that are created from now on.
				</td>
			</tr>
		</table>
	</ajax:AjaxPanel>
</asp:Content>