<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
	Inherits="Members_Reports_Raw_Data_View_Default" Title="Raw Data View" %>

<%@ Register TagPrefix="rt" TagName="ConfiguredDatasourceList" Src="~/Members/CommonControls/ConfiguredDatasourceList.ascx" %>
<%@ Register TagPrefix="rt" Namespace="Rt.Framework.CommonControls.Web" Assembly="Rt.Framework" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	
		<rt:SiteList runat="Server" ID="siteList" AutoPostBack="true" /><br />
		<rt:ConfiguredDatasourceList runat="Server" id="configuredDatasourceList" AutoPostBack="true" /><br />
		<rt:DateRangeSelector runat="server" ID="dateSelector" /><br />
		Sort:
		<asp:DropDownList runat="server" ID="ddlSortDirection">
			<asp:ListItem Text="Ascending" Selected="true" />
			<asp:ListItem Text="Descending" Value="Desc" />
		</asp:DropDownList>
		<br />
		<asp:Button runat="Server" ID="cmdGo" Text="Go" />

		
	<asp:GridView runat="server" ID="dgRawData" EnableViewState="false" AutoGenerateColumns="false"
		CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem">
		<Columns>
			<asp:BoundField DataField="DatasourceName" HeaderText="Datasource Name" />
			<asp:BoundField DataField="Timestamp" HeaderText="Timestamp" />
			<asp:BoundField DataField="FloatValue" HeaderText="Value" />
			<asp:CheckBoxField DataField="Success" HeaderText="Success" />
			<asp:CheckBoxField DataField="Fuzzy" HeaderText="Fuzzy" />
		</Columns>
	</asp:GridView>
	
</asp:Content>