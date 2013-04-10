<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Report-Editor.aspx.cs"
	Inherits="Members_Reports_Custom_Reports_Report_Editor" Title="Custom Report Editor" %>
	
<%@ Reference Page="Report-Viewer.aspx" %>
	
<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<table>
		<tr>
			<td>
				<asp:ListBox runat="server" ID="lstReports" CssClass="eventListBox" AutoPostBack="true" /><br />
				<asp:Button runat="server" ID="cmdAdd" text="Create New Report" />
				<asp:Button runat="server" ID="cmdDelete" text="Delete" />
			</td>
			<td valign="top">
				<b>Custom Report Details:</b><br />
				Name: <asp:TextBox runat="server" ID="txtName" Columns="40" /><br />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" Display="dynamic" ErrorMessage="*Required"
						ValidationGroup="details" />
				<br />
				Description:<br />
				<asp:TextBox runat="Server" ID="txtDescription" TextMode="MultiLine" Rows="3" Columns="40" /><br />
					
				Email Report:
				<asp:DropDownList runat="server" ID="ddlEmailInterval">
					<asp:ListItem Text="Never" Value="0" />
					<asp:ListItem Text="Daily" Value="1" />
					<asp:ListItem Text="Weekly" Value="7" />
					<asp:ListItem Text="Bi-Weekly" Value="14" />
					<asp:ListItem Text="Monthly" Value="30" />
				</asp:DropDownList><br />

				<asp:Button runat="server" ID="cmdSave" Text="Save" />
				
				<hr />
				
				<b>What this report contains:</b>
				<asp:HyperLink runat="server" ID="lnkPreview" Text="Preview" Target="_blank">(Click here to preview the report)</asp:HyperLink>
				<br />
				<asp:BulletedList runat="server" ID="blReportcomponents" /><br />
				<asp:HyperLink runat="server" ID="lnkEditReportComponents" Text="Click here to Add, Edit, or Delete Report Parts in this list"
					ToolTip="Click to jump to the report part list editor" />
			</td>
		</tr>
	</table>
</asp:Content>