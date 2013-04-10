<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
	Inherits="Administrators_Status_Report_Default" Title="System Status Report" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<b>General Statistics:</b>
	<asp:GridView runat="server" id="dgGeneral" CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem" />
	<br />
	<b>Datasource Statistics:</b>
	<asp:GridView runat="server" ID="dgDatasources" CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem" />
	<br />
	<b>Database Stats:</b>
	<asp:GridView runat="server" ID="dgDbStats1" CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem" />
	<br />
	<asp:GridView runat="server" ID="dgDbStats2" CssClass="TabularTable" RowStyle-CssClass="TabularTableItem" AlternatingRowStyle-CssClass="TabularTableAltItem" />
</asp:Content>