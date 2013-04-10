<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SavedReportTab.ascx.cs" Inherits="Members_Interactive_Report_SavedReportTab" %>

<b>Saved Report Settings (double-click to load):</b><br />
<asp:ListBox runat="server" ID="lstReports" style="height: 230px; width: 780px;" /><br />
<asp:Button runat="server" ID="cmdDelete" Text="Delete" />
<asp:Button runat="server" ID="cmdLoad" Text="Load" />