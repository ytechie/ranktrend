<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Members_Events_Event_Editor_Default"
	Title="Event Editor" %>
	
<%@ Register TagPrefix="ColorDD" Namespace="UNLV.IAP.WebControls" Assembly="UNLV.IAP.WebControls.HtmlColorDropDown" %>
<%@ Register Assembly="eWorld.UI" Namespace="eWorld.UI" TagPrefix="ew" %>

<%@ Register TagPrefix="rt" TagName="EventCategoryList" Src="~/Members/CommonControls/EventCategoryList.ascx" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<rt:SiteList runat="server" id="siteList" />
	<hr />
	<rt:EventCategoryList runat="server" ID="categoryList" />
	<table>
		<tr>
			<td>
				<asp:ListBox runat="server" ID="lstEvents" CssClass="eventListBox" /><br />
				<asp:Button runat="server" ID="cmdAdd" text="Add" />
				<asp:Button runat="server" ID="cmdDelete" text="Delete" />
			</td>
			<td valign="top">
				<b>Event Details:</b><br />
				Name: <asp:TextBox runat="server" ID="txtName" /><br />
				Description:<br />
				<asp:TextBox runat="server" ID="txtDescription" Rows="3" Columns="50" TextMode="MultiLine" /><br />
				Start: <asp:TextBox runat="server" ID="txtStart" /><br />
				End: <asp:TextBox runat="Server" ID="txtEnd" />
				<asp:CheckBox runat="server" ID="chkHasEnd" Text="Has End Date" /><br />
				Drawing Color: <ColorDD:HtmlColorDropDown runat="server" ID="ddlColor" Palette="AllNamedColors"
					DisplaySelectColorItemText="Auto Select" DisplaySelectColorItem="true" /><br />

				<asp:Button runat="server" ID="cmdSave" Text="Save" />
				<span id="saveSuccessMsg" style="display: none;">Save Successful</span>
			</td>
		</tr>
	</table>
</asp:Content>