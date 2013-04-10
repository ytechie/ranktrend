<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Members_SubmitCase_Default" Title="RankTrend.com - Submit Case" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
	<ajax:AjaxPanel ID="AjaxPanel1" runat="server">
		<table class="changeUserInformation" border="0">
			<tr>
				<td>
					<asp:Panel runat="server" ID="SubmitCasePanel">
					<table border="0" style="height: 100%; width: 100%;">
						<tr>
							<td colspan="2">
								<asp:ValidationSummary runat="server" ID="SubmitCaseSummary" ShowSummary="true" ValidationGroup="SubmitCase" HeaderText="One or more required fields were not supplied." />
							</td>
						</tr>
						<tr>
							<td align="right">
								<asp:Label runat="server" ID="SubjectLabel" AssociatedControlID="Subject" Text="Title:" />
							</td>
							<td>
								<asp:TextBox runat="server" ID="Subject" Width="300px" />
								<asp:RequiredFieldValidator ID="SubjectRequired" runat="server" ControlToValidate="Subject"
                                    ErrorMessage="Title required." ValidationGroup="SubmitCase" Display="none" />
							</td>
							<td style="border-bottom: 1px solid #CCCCCC;">
							<b>Title</b><br />
								Enter a one line description of the problem.
							</td>
						</tr>
						<tr>
							<td align="right" valign="top">
								<asp:Label runat="server" ID="MessageLabel" AssociatedControlID="Message" Text="Description:" /></td>
							<td>
								<asp:TextBox runat="server" ID="Message" TextMode="MultiLine" Columns="50" Rows="20" />
								<asp:RequiredFieldValidator ID="MessageRequired" runat="server" ControlToValidate="Message"
                                    ErrorMessage="Description required." ValidationGroup="SubmitCase" Display="none" />
							</td>
							<td valign="top">
								<b>Description</b><br />
								Describe the bug or feature in detail.  Every bug report <b>must</b> include
								<ol>
									<li>steps to reproduce</li>
									<li>what you expected to happen</li>
									<li>and what happened instead.</li>
								</ol>
							</td>
						</tr>
						<tr>
							<td colspan="2" align="right">
								<asp:Button runat="server" ID="Submit" Text="Submit" ValidationGroup="SubmitCase" OnClick="Submit_Click" /></td>
						</tr>
					</table>
					</asp:Panel>
					<asp:Panel runat="server" ID="CaseSubmittedPanel" Visible="false">
						<p>Your case has been successfully submitted.  Thank you for your help in improving RankTrend.</p>
					</asp:Panel>
					<asp:Panel runat="server" ID="CaseNotSubmittedPanel" Visible="false">
						<p>
							We're sorry but there was a problem submitting your case to our support system.
							We have logged the problem so that we are aware of it and your
							<b>the case you entered has been logged along with the error so that it has not been lost</b>.
						</p>
					</asp:Panel>
				</td>
			</tr>
		</table>
	</ajax:AjaxPanel>
</asp:Content>
