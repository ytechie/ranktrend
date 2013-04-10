<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
	Inherits="Members_Reports_Summary_by_Site_Default" Title="Summary Report by Site" %>
	
	<%@ Register TagPrefix="rt" TagName="SiteList" Src="~/Members/CommonControls/SiteList.ascx" %>
	<%@ Register Assembly="Rt.Framework" Namespace="Rt.Framework.Services.ReportEngine.ReportControls" TagPrefix="rt" %>
	
<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<rt:SiteList runat="Server" ID="siteList" AutoPostBack="true" /><br />
	<br />
	<rt:SiteSummaryTable runat="server" id="summaryTable" />
</asp:Content>