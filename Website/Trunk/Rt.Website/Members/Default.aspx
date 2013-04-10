<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Members_Default" Title="Control Panel" Async="true" %>

<%@ Register TagPrefix="Rt" TagName="ServiceControlPanelControl" Src="~/Members/TextMessageViewer.ascx" %>

<asp:Content ContentPlaceHolderID="mainContent" runat="Server">
	<h2>
		Notice: We're doing a major overhaul right now, so many things have been removed, and
		most items do not work right now. Please stay tuned!
	</h2>

	<h2>Setup</h2>
		<a href="Profile/"><img src="CPIcons/Profile.gif" alt="Profile" /></a>
		<a href="what-to-track.aspx"><img src="CPIcons/What-To-Track.gif" alt="What to Track" /></a>
		<a href="Tray-Application/"><img src="CPIcons/Install-Data-Fetcher.gif" alt="Install Data Fetcher" /></a>
		<br />
		<br />
		<h2>Reports</h2>
		<a href="Reports/Thumbnail-Dashboard/"><img src="CPIcons/Data-Dashboard.gif" alt="Data Dashboard" /></a>
		<a href="Advanced-Charting/"><img src="CPIcons/Advanced-Charting.gif" alt="Advanced Charting" /></a>
		<a href="Reports/Summary-by-Site/"><img src="CPIcons/Data-Summary.gif" alt="Data Summary" /></a>
		<a href="Reports/Raw-Data-View/"><img src="CPIcons/Raw-Data.gif" alt="Raw Data" /></a>

<%--	<div class="left ControlPanel">
		
		
		<table border="0" cellpadding="0" cellspacing="0">
			<tr>
				<td style="width: 30px;">
					<img src="CPIcons/Administration.gif" alt="Configure" style="height: 29; width: 26;" />
				</td>
				<td>
					<h3>Configure</h3>
					<ul class="ControlPanelSection">
						<li class="cp_users">
							<asp:HyperLink runat="server" ID="lnkProfile" NavigateUrl="./Profile/" Text="Profile & Account Maintenance" />
						</li>
						<li class="cp_users">
							<asp:HyperLink runat="server" NavigateUrl="#" Text="What to Track" />
						</li>
					</ul>
				</td>
			</tr>
				
			<tr>
				<td style="width: 30px;">
					<img src="CPIcons/Account.gif" alt="Install" style="height: 26; width: 29;" />
				</td>
				<td>
					<h3>Install</h3>
					<ul class="ControlPanelSection">
						<li>
							<asp:Image runat="server" ID="imgTrayAppExclamation"
								ToolTip="No data has been collected, which may mean that you have not yet installed the tray application"
								ImageUrl="CPIcons/ExclamationIcon.gif" Visible="true" style="height: 17; width: 16;" />
							<a href="Tray-Application/">Download the tray application</a>
						</li>
					</ul>
				</td>
			</tr>
			<tr>
				<td style="width: 30px;">
					<img src="CPIcons/Reporting.gif" alt="Learn" style="height: 26; width: 29;" />
				</td>
				<td>
					<h3>Learn</h3>
					<ul class="ControlPanelSection">
						<li>
							<a href="Reports/Thumbnail-Dashboard/">Data Dashboard</a>
						</li>
						<li>
							<a href="Reports/???/">Advanced Charting</a>
						</li>
						<li>
							<a href="Reports/Raw-Data-View/">Raw Data View</a>
						</li>
						<li>
							<a href="Reports/Summary-by-Site/">Summary Data View</a>
						</li>

					</ul>
				</td>
			</tr>
		</table>
	</div>
	<div class="right">
		<ajax:AjaxPanel ID="AjaxPanel1" runat="server">
			<asp:Panel runat="server" ID="TextMessages">
			
			</asp:Panel>
		</ajax:AjaxPanel>
	</div>--%>
</asp:Content>
