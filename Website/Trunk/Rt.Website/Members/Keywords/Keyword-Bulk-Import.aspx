<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Keyword-Bulk-Import.aspx.cs" Inherits="Members_Keywords_Keyword_Bulk_Import"
	Title="Keyword Bulk Importer" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<asp:Panel runat="server" ID="pnlStandard">
		<p>
			Enter the search terms you would like to track. Enter each phrase on a new line.
		</p>
		<p>
			If you enter search terms that are already being tracked, they will be ignored.
		</p>
		<asp:TextBox runat="server" ID="txtKeywords" TextMode="multiline" Rows="20" Columns="30" /><br />
		<br />
		<rt:SiteList runat="server" ID="siteList" AutoPostBack="false" /><br />
		<asp:CheckBoxList runat="server" ID="searchEngineList">
			<asp:ListItem Text="Yahoo!" Value="1" Selected="true" />
			<asp:ListItem Text="Google" Value="2" Selected="true" />
			<asp:ListItem Text="MSN/MS Live" Value="7" Selected="true" />
		</asp:CheckBoxList><br />
		<asp:Button runat="server" ID="cmdImport" Text="Import" /><br />
		<h4>Jump to:</h4>
		<ul>
			<li><a runat="Server" href="~/Members/Reports/Keywords/Keyword-Summary.aspx">Keyword Summary Report</a></li>
		</ul>
	</asp:Panel>
	<asp:Panel runat="server" ID="pnlNoSites" Visible="false">
		<p>
			You need to configure at least 1 site before using this tool.
		</p>
	</asp:Panel>
</asp:Content>