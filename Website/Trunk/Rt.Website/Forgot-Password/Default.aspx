<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Forgot_Password_Default" Title="RankTrend.com - Forgot Passord" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
	<asp:PasswordRecovery ID="PasswordRecovery1" runat="server" CssClass="forgotPasswordControl"
		SubmitButtonText="Submit" SubmitButtonType="Button" OnSendingMail="PasswordRecovvery1_SendingMail">
		<SuccessTemplate>
			<table border="0">
				<tr>
					<td>
						Your password has been sent to you.
					</td>
				</tr>
			</table>
		</SuccessTemplate>
		<MailDefinition From="SystemMailer@RankTrend.com" Subject="Your RankTrend.com Password"
			IsBodyHtml="false" BodyFileName="PasswordMail.txt" />
	</asp:PasswordRecovery>
	<div style="width: 100%; text-align: center;">
		<asp:HyperLink runat="server" NavigateUrl="~/Forgot-Username/" Text="Forgot User Name" />
	</div>
</asp:Content>