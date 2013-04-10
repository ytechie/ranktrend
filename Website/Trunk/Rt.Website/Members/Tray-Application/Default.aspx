<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Members_Tray_Application_Default" Title="RankTrend Tray Application Download Page" %>

<asp:Content ContentPlaceHolderID="mainContent" runat="Server">
	<div class="left">
		<h3>
			RankTrend Tray Application Download</h3>
		<p>
			This tray application is the heart of RankTrend's data gathering process. This tiny
			little program, once installed on your computer, will act as a "middle man", retreive
			the data and report it to our system. For example, if you configured a datasource
			to track your page rank, this application will check your page rank periodically
			and report it to our system which records it for you. Since this is the heart of
			the data gathering process, the tray application <b>is required</b> for you to see
			data showing up in your datasources. <i>If this tray application is not running on
				your computer, it can't check the data and report it to RankTrend.</i> You'll
			want to install it on a computer that is run for at least a little bit each day
			and has an Internet connection.
		</p>
		<br />
		<div>
			<b>System Requirements</b>
			<asp:Panel runat="server" ID="WindowsRequirements">
				<ul style="list-style-type: disc; margin-left: 10px;" Visible="false">
					<li>Windows XP or 2003 - May run on Vista, but has not been validated yet.</li>
					<li>.NET Framework 2.0 or 3.0 - <a href="http://www.microsoft.com/downloads/details.aspx?familyid=0856eacb-4362-4b0d-8edd-aab15c5e04f5"
						title="Download .NET Framework 2.0">Free download from Microsoft</a>, also available
						on Windows Updates.
						<asp:Literal runat="server" ID="NetVersionMessage" /></li>
					<li>An Internet Connection - An "always on" Internet connection is highly recommended.</li>
				</ul>
			</asp:Panel>
			<asp:Panel runat="server" ID="MacOSRequirements" Visible="false">
				<ul style="list-style-type: disc; margin-left: 10px;">
					<li>Mac OS X (10.4.9 recommended, other versions have not been validated)</li>
					<li>JVM 1.5*</li>
					<li>An Internet Connection - An "always on" Internet connection is highly recommended.</li>
				</ul>
			</asp:Panel>
		</div>
		<br />
		<ajax:AjaxPanel ID="AjaxPanel1" runat="server">
			<asp:Panel runat="server" ID="LicenseAgreementPanel">
				<table border="0" style="width: 100%; height: 100%;">
					<tr>
						<td align="center">
							<table border="0" style="width: 400px;">
								<tr>
									<td>
										<asp:TextBox runat="server" ID="LicenseAgreement" TextMode="MultiLine" CssClass="LegalAgreementTextBox"
											ReadOnly="true" /><br />
									</td>
								</tr>
								<tr>
									<td align="right">
										<asp:Button runat="server" ID="Agree" Text="Agree and Proceed to Download" OnClick="Agree_Click" />
										<asp:Button runat="server" ID="Disagree" Text="Disagree" OnClick="Disagree_Click" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</asp:Panel>
			<asp:Panel runat="server" ID="DownloadApplicationPanel">
				<asp:ImageButton runat="server" ID="DownloadApplication" OnClick="DownloadApplication_Click" />
				<br />
				<br />
				<div>
					<b>Other Downloads</b>
					<ul style="list-style-type: disc; margin-left: 10px;">
						<li>
							<asp:HyperLink runat="server" NavigateUrl="~/Members/Tray-Application/?OS=WinXP" ToolTip="Download for Windows XP, 2003 or Vista (Vista is experimental)">Download for Windows XP, 2003 or Vista (Vista is experimental)</asp:HyperLink>
						</li>
						<li>
							<asp:HyperLink runat="server" NavigateUrl="~/Members/Tray-Application/?OS=MacOSX" ToolTip="Download for Mac OS X">Download for Mac OS X</asp:HyperLink>
						</li>
						<li>
							Linux (coming soon)
						</li>
					</ul>
				</div>
			</asp:Panel>
		</ajax:AjaxPanel>
	</div>
	<div class="right">
		<h3>
			FAQs</h3>
		<p>
			<b>Q: What is the tray application?</b><br />
			A: The tray application is the program that actually gathers the data you've requested
			and reports it to our system.
		</p>
		<p>
			<b>Q: What happens if I don't run the tray application?</b><br />
			A: The tray application is the means by which the data is gathered, so without it,
			no data will be gathered for your account.
		</p>
		<p>
			<b>Q: Why is this tray application required to gather data?</b><br />
			A: This tray application makes it so that the request for all this data is not coming
			from one place but rather from users and computers around the world like normal
			requests do.
		</p>
		<p>
			<b>Q: What computer should I install this tray application on?</b><br />
			A: Install the tray application on a computer that will be on and connected to the
			Internet for at least a few minutes each day. That way the tray application can
			gather the data at least once each day.
		</p>
		<p>
			<b>Q: Would it help if I installed this on multiple computers?</b><br />
			A: You can install the tray application on multiple computers, but it is unnecessary
			if your computer is on and has an Internet connection at least a few minutes each
			day, one install is enough at that point.
		</p>
		<p>
			<b>Q: Do I need to do anything with it after it is installed?</b><br />
			A: The tray application is configured to run on startup, so you should never have
			to manually launch the program.  At the end of the installation, it should prompt you
			for your RankTrend username and password.  Once that has been set, you should never
			have to worry about the application again unless notified that you have to update it
			(which should be very rare).
		</p>
		<p>
			<b>Q: How do I know that the tray application is working or running?</b><br />
			A: If the tray application shows up in the tray down
			by the clock, it should be working. If you have data points showing up for your
			different datasources, you know its working.
		</p>
		<p>
			<b>Q: I have .NET Framework 3.0 installed, is that good enough?</b><br />
			A: Yes, the tray application runs on the .NET Framework 3.0 as well as 2.0.
		</p>
	</div>
</asp:Content>
