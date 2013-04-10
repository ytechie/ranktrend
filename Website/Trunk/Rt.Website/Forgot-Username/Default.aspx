<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Forgot_Username_Default" Title="RankTrend.com - Forgot User Name" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
	<table class="forgotUsernameControl" cellspacing="0" cellpadding="1" border="0" style="border-collapse: collapse;">
		<tr>
			<td>
				<table border="0">
					<tr>
						<td align="center">
							Forgot your User Name?</td>
					</tr>
					<tr>
						<td align="center">
							Enter your e-mail address to receive your account information.</td>
					</tr>
					<tr>
						<td align="center">
							<asp:Label runat="server" ID="EmailAddressLabel" Text="E-Mail Address:" AssociatedControlID="EmailAddress" />
							<asp:TextBox ID="EmailAddress" runat="server" />
						</td>
					</tr>
					<tr>
						<td colspan="2" align="right">
							<asp:Button runat="server" ID="LookupUsername" Text="Submit" OnClick="LookupUsername_Click" />
						</td>
					</tr>
					<tr>
						<td align="center">
							<asp:RequiredFieldValidator runat="server" ID="EmailAddressRequired" ControlToValidate="EmailAddress"
								Text="You must enter your e-mail address to recover your username." />
							<asp:Panel runat="server" ID="ForgotUsername_Success" Visible="false">
								We sent an email to your email address with your user information and which will
								allow you to re-verify your email address with our system. The email sent to you
								is from <b>
									<asp:Literal runat="server" ID="AdministratorEmail" /></b>. It is recommended
								that you add this email address to your Junk Email Filter's "White List" to verify
								that emails sent to you from RankTrend.com will successfully reach you.
							</asp:Panel>
							<asp:Panel runat="server" ID="ForgotUsername_UsernameLookupFailure" Visible="false">
								We were unable to find your user information.
							</asp:Panel>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Content>
