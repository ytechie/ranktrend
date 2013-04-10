<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Sign_Up_Default" Title="RankTrend.com - Sign Up" %>

<%@ Reference Page="~/Verify-User/Default.aspx" %>
<%@ Register Assembly="YTech.General" Namespace="YTech.General.Web.Controls.Validators"
	TagPrefix="ytv" %>
<asp:Content ContentPlaceHolderID="mainContent" runat="Server">
	<asp:Panel runat="server" ID="SignupPanel">
		<asp:CreateUserWizard runat="server" ID="CreateUserWizard1" CssClass="createUserWizardControl"
			ContinueDestinationPageUrl="~/Members/" OnCreatedUser="CreateUserWizard1_CreatedUser">
			<WizardSteps>
				<asp:WizardStep ID="CreateUserWizardStep1" runat="server">
					<p>
						We are currently in Open Beta.  This means that we anyone can now join our service.
						The fact that we are still in beta mode means we are continually improving the site
						by adding and updating features and fixing any problems that may arrise.  Once
						we release, the site will be more stable in terms of when and how we add features
						and hopefully will experience 0 problems.  Please subscribe to our 
						<a href="http://www.RankTrend.com/Community/blogs/MainFeed.aspx"
							title="RSS Syndication">RSS Syndication</a> to find out when we launch and to
						stay up-to-date on our latest features.
					</p>
					Terms of Service<br />
					<asp:TextBox runat="server" ID="TermsOfService" TextMode="MultiLine" CssClass="LegalAgreementTextBox"
						ReadOnly="true" /><br />
					<asp:CheckBox runat="server" ID="chkAgree" Text="I agree to Terms of Service" /><br />
					<ytv:RequiredCheckboxValidator runat="server" ID="CheckBoxValidator1" ControlToValidate="chkAgree" EnableClientScript="false"
						MustBeChecked="true" Display="Dynamic" ErrorMessage="You must agree to the terms of service before you can continue." />
				</asp:WizardStep>
				<asp:CreateUserWizardStep ID="CreateUserWizardStep3" runat="server">
					<ContentTemplate>
						<table border="0" style="height: 100%; width: 100%;">
							<tr>
								<td align="center" colspan="2">
								Sign Up for Your New Account</th>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="UserName" ID="UserNameLabel">
                                  Username:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="UserName" />
									<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator9" ControlToValidate="UserName"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="Password" ID="PasswordLabel">
                                  Password:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="Password" TextMode="Password" />
									<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator10" ControlToValidate="Password"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="ConfirmPassword" ID="ConfirmPasswordLabel">
                                  Confirm Password:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />
									<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator13" ControlToValidate="ConfirmPassword"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<hr />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="FirstName" ID="FirstNameLabel">
                                  First Name:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="FirstName" />
									<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="FirstName"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="LastName" ID="LastNameLabel">
                                   Last Name:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="LastName" />
									<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="LastName"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="Email" ID="EmailLabel">
                                  Email:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="Email" />
									<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator11" ControlToValidate="Email"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="ConfirmEmail" ID="ConfirmEmailLabel">
                                  Confirm Email:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="ConfirmEmail" />
									<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="ConfirmEmail"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<hr />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="Question" ID="QuestionLabel">
                                  Security Question:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="Question" />
									<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator12" ControlToValidate="Question"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<asp:Label runat="server" AssociatedControlID="Answer" ID="AnswerLabel">
                                  Security Answer:</asp:Label></td>
								<td>
									<asp:TextBox runat="server" ID="Answer" />
									<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator14" ControlToValidate="Answer"
										ErrorMessage="*" ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
										ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
										ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<asp:CompareValidator ID="EmailCompare" runat="server" ControlToCompare="Email" ControlToValidate="ConfirmEmail"
										Display="Dynamic" ErrorMessage="The Email and Confirmation Email must match." ValidationGroup="CreateUserWizard1" />
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
								</td>
							</tr>
						</table>
					</ContentTemplate>
				</asp:CreateUserWizardStep>
			</WizardSteps>
		</asp:CreateUserWizard>
	</asp:Panel>
</asp:Content>