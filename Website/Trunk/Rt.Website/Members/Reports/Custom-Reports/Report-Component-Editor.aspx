<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Report-Component-Editor.aspx.cs"
	Inherits="Members_Reports_Custom_Reports_Report_Component_Editor" Title="Custom Report Part Editor" %>
	
<%@ Register TagPrefix="rt" Namespace="Rt.Framework.CommonControls.Web" Assembly="Rt.Framework" %>
	
<%@ Reference Page="Report-Viewer.aspx" %>
	
<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<table>
		<tr>
			<td>
				<b>Part List:</b>
				<asp:HyperLink runat="Server" Target="_blank" ID="lnkPreview">(preview report)</asp:HyperLink><br />
				<asp:ListBox runat="server" ID="lstReportParts" CssClass="eventListBox" AutoPostBack="true" /><br />
				<asp:Button runat="server" ID="cmdAdd" text="Add Report Part" />
				<asp:Button runat="server" ID="cmdDelete" text="Delete" />
			</td>
			<td valign="top">
				<b>Selected Part Details:</b><br />
				Type:
				<asp:DropDownList runat="server" ID="ddlReportType" AutoPostBack="true">
					<asp:ListItem Text="Saved Trend Report" Value="1" />
					<asp:ListItem Text="Site Summary" Value="2" />
				</asp:DropDownList><br />
				
				<%-- Step 2 panels --%>
				<asp:Panel runat="server" ID="pnlSavedTrends" Visible="false">
					Saved Trend: <asp:DropDownList runat="server" ID="ddlSavedTrends" />
				</asp:Panel>
				
				<asp:Panel runat="server" ID="pnlDataType" Visible="false">
					
				</asp:Panel>
				
				<asp:Panel runat="server" ID="pnlSiteSelection" Visible="false">
					<rt:SiteList runat="server" ID="siteList" />
				</asp:Panel>
				
				<%-- Step 3 panels --%>
				<asp:Panel runat="server" ID="pnlTimeSelection" Visible="false">
					<rt:DateRangeSelector runat="server" id="dateRangeSelector" />
				</asp:Panel>
				
				<%-- Detail footer --%>
				<asp:Button runat="server" ID="cmdSave" Text="Save" />
			</td>
		</tr>
	</table>
</asp:Content>

