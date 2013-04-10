<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
	Inherits="Members_Advanced_Charting_Default" Title="Advanced Charting" %>

<%@ Register Src="~/Members/CommonControls/SiteList.ascx" TagPrefix="rt" TagName="SiteList" %>
<%@ Register Assembly="FlashControl" Namespace="Bewise.Web.UI.WebControls" TagPrefix="Bewise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" Runat="Server">
	<rt:SiteList runat="server" id="siteList" AutoPostBack="True" /><br />
	<Bewise:FlashControl ID="flash" runat="server" MovieUrl="ChartApp.swf" InternetExplorerCodeEnabled="true" Width="100%"
	 Height="500px" />
</asp:Content>

