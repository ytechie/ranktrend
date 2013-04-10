<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
		CodeFile="Default.aspx.cs" Inherits="Members_Interactive_Report_Default"
		Title="Interactive Report" %>

<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>

<%@ Register TagPrefix="rt" TagName="DatasourceTable" Src="DatasourceTable.ascx" %>
<%@ Register TagPrefix="rt" TagName="EventCategoryTable" Src="~/Members/Interactive-Report/EventTable.ascx" %>
<%@ Register TagPrefix="rt" TagName="SavedReportTab" Src="~/Members/Interactive-Report/SavedReportTab.ascx" %>
<%@ Register TagPrefix="rt" TagName="OverviewTab" Src="OverviewTab.ascx" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<chart:WebChartViewer id="chartViewer" runat="server" />
	
	<table class="TabTable">
		<tr>
			<td runat="server"><a runat="server" id="cmdSaved"><img src="Tab-Icons/Saved.gif" alt="Saved" />Saved</a></td>
			<td runat="server"><a runat="server" id="cmdOverview"><img src="Tab-Icons/Overview.gif" alt="Overview" />Overview</a></td>
			<td runat="server"><a runat="server" id="cmdData"><img src="Tab-Icons/Data.gif" alt="Data" />Data</a></td>
			<td runat="server"><a runat="server" id="cmdEvents"><img src="Tab-Icons/Events.gif" alt="Events" />Events</a></td>
		</tr>
	</table>
	<asp:Panel runat="server" ID="pnlSaved" Visible="true">
		<rt:SavedReportTab runat="server" ID="savedReportTab" />
	</asp:Panel>
	<asp:Panel runat="server" ID="pnlOverview" Visible="false" Height="250" Width="760" BorderWidth="1" BorderColor="gainsboro" CssClass="ir_pnlOverview">
		<rt:OverviewTab runat="server" ID="overviewTab" />
	</asp:Panel>
	<asp:Panel runat="server" ID="pnlData" Visible="false" CssClass="tabBody">
		<asp:Panel runat="Server" ScrollBars="Vertical" Height="200" Width="800" BorderWidth="0" BorderColor="gainsboro">
			<rt:DatasourceTable runat="server" ID="datasourceList" />
		</asp:Panel>
	</asp:Panel>
	<asp:Panel runat="server" ID="pnlEvents" Visible="false">
		<asp:Panel runat="Server" ScrollBars="Vertical" Height="200" Width="800" BorderWidth="0" BorderColor="gainsboro">
			<rt:EventCategoryTable runat="server" ID="eventCategoryList" />
		</asp:Panel>
	</asp:Panel>
</asp:Content>