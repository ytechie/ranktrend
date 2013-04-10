<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Report-Viewer.aspx.cs"
	Inherits="Members_Reports_Custom_Reports_Report_Viewer" Title="Custom Report Viewer" %>
	
<%@ Reference Page="~/Members/Reports/Custom-Reports/Report-Part.aspx" %>
	
<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<asp:PlaceHolder runat="Server" ID="reportPlaceholder" />
</asp:Content>