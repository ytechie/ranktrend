<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Members_Profile_Default" Title="RankTrend.com - Profile" %>

<asp:Content ContentPlaceHolderID="mainContent" runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <asp:ChangePassword ID="ChangePassword1" runat="server" CssClass="changePasswordControl"
            ContinueDestinationPageUrl="~/Members/" CancelDestinationPageUrl="~/Members/" />
    </ajax:AjaxPanel>
    <hr />
    <ajax:AjaxPanel ID="AjaxPanel2" runat="server">
        <table class="changeUserInformation" border="0">
            <tr>
                <td>
                    <table border="0" style="height: 100%; width: 100%;">
                        <tr>
                            <td align="right">
                                <asp:Label runat="server" AssociatedControlID="FirstName" ID="FirstNameLabel">
                                    First Name:</asp:Label></td>
                            <td>
                                <asp:TextBox runat="server" ID="FirstName" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="FirstName"
                                    ErrorMessage="*" ValidationGroup="UserInformation" />
                            </td>
                        </tr>
                        
                        <tr>
                            <td align="right">
                                <asp:Label runat="server" AssociatedControlID="LastName" ID="LastNameLabel">
                                     Last Name:</asp:Label></td>
                            <td>
                                <asp:TextBox runat="server" ID="LastName" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="LastName"
                                    ErrorMessage="*" ValidationGroup="UserInformation" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button runat="server" ID="SaveUserInformationButton" Text="Save" OnClick="SaveUserInformationButton_Click"
                                 validationgroup="UserInformation" />
                                <asp:Button runat="server" ID="CancelUserInformation" Text="Cancel" OnClick="CancelUserInformationButton_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
     </ajax:AjaxPanel>
      <hr />
      <ajax:AjaxPanel runat="server">
        <table class="changeUserInformation">
					<tr>
							<td align="right">
								<asp:Label runat="server" AssociatedControlID="ddlTimezone">Time Zone:</asp:Label>
								</td>
							<td>
									<asp:DropDownList runat="server" ID="ddlTimezone" />
							</td>
					</tr>
					<tr>
							<td colspan="2" align="center">
									<asp:Button runat="server" id="cmdUpdateTimezone" text="Save Timezone" />
							</td>
					</tr>
        </table>
    </ajax:AjaxPanel>
    <hr />
      <ajax:AjaxPanel ID="AjaxPanel3" runat="server">
        <table class="changeUserInformation">
					<tr>
						<td>
							If you are unsatisfied with our service you can use the button below to request that your account
							be removed from the system.  If you choose to cancel your account, it will take a few days to process
							the request.  During this time, you may still receive emails.  However, once the request has been processed,
							you will no longer receive automated emails from our system.
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label runat="server" ID="AccountCancellationReasonLabel" AssociatedControlID="AccountCancellationReason" Text="Please briefly describe why you are cancelling your account." />
							<br />
							<asp:TextBox runat="server" ID="AccountCancellationReason" TextMode="multiLine" Columns="50" Rows="5" />
						</td>
					</tr>
					<tr>
							<td colspan="2" align="center">
									<asp:Button runat="server" ID="CancelAccount" Text="Cancel Account" OnClick="CancelAccount_Click" />
							</td>
					</tr>
        </table>
    </ajax:AjaxPanel>
</asp:Content>
