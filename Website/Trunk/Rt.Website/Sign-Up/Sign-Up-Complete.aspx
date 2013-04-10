<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Sign-Up-Complete.aspx.cs" Inherits="Sign_Up_Sign_Up_Complete" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" Runat="Server">
	<table border="0">
		<tr>
			<td>
				<table border="0" style="height: 100%; width: 100%;">
					<tr>
						<td align="center" colspan="2">
							Complete</td>
					</tr>
					<tr>
						<td>
							Your account has been successfully created. Before your account can be activated
							though, you must confirm your email address. An email has been sent to you with
							a link that will confirm your email address with our system. Once you've clicked
							on this link, your account will become activated and you will then be able to log
							in.</td>
					</tr>
					<tr>
						<td>
							The email sent to you is from <b>
								<asp:Literal runat="server" ID="AdministratorEmail" /></b>. It is recommended
							that you add this email address to your Junk Email Filter's "White List" to verify
							that emails sent to you from RankTrend.com will successfully reach you.
						</td>
					</tr>
					<tr>
						<td align="right" colspan="2">
							<asp:Button runat="server" ID="cmdContinue" Text="Continue" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Content>
