<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OverviewTab.ascx.cs" Inherits="Members_Interactive_Report_OverviewTab" %>

<%@ Register TagPrefix="rt" Namespace="Rt.Framework.CommonControls.Web" Assembly="Rt.Framework" %>

<ajax:AjaxPanel runat="server">
	Name/Title:	<asp:TextBox runat="server" ID="txtReportName" Columns="50" MaxLength="100" /><br />
	<br />
	<rt:DateRangeSelector runat="server" ID="dateSelector" /><br />
	<br />
	Size: <asp:DropDownList runat="server" ID="sizeSelector">
		<asp:ListItem Text="Small" Value="0" />
		<asp:ListItem Text="Normal" Value="1" Selected="true" />
		<asp:ListItem Text="Large" Value="2" />
	</asp:DropDownList>
	<br />
	Created: <asp:Literal runat="server" ID="lblReportCreated" /><br />
	Last Saved: <asp:Literal runat="server" ID="lblReportLastSaved" /><br />
</ajax:AjaxPanel>