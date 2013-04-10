<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
	Inherits="Members_Reports_Thumbnail_Dashboard_Default" Title="Thumbnail Data Source Dashboard" %>

<%@ Reference Control="Thumbnail-Section.ascx" %>

<%@ Register TagPrefix="rt" Namespace="Rt.Framework.CommonControls.Web" Assembly="Rt.Framework" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<table>
		<tr>
			<td style="width: 75%;">
				<rt:SiteList runat="server" ID="siteList" AutoPostBack="true" /><br />
				<rt:DateRangeSelector runat="server" ID="dateSelector" />
				<asp:Button runat="server" ID="cmdRefresh" Text="Refresh" />
			</td>
			<td align="right" valign="bottom">
				<div style="text-align: left;">
					<b>Trend Legend:</b>
					<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/TrendLegend.gif" AlternateText="Trend Legend" BorderStyle="Solid" BorderColor="Black" BorderWidth="1" />
				</div>
			</td>
		</tr>
	</table>
	<br />
	<br />
	<asp:PlaceHolder runat="server" ID="thumbnails" /><br />
	<asp:Button runat="server" ID="cmdDrillDownMulti" text="Plot Selected" />
</asp:Content>