<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Login_Default" Title="RankTrend.com - Log In" %>

<asp:Content ContentPlaceHolderID="mainContent" runat="Server">
	<asp:Login ID="loginControl" runat="server" CssClass="logonControl" DestinationPageUrl="../Members/" EnableViewState="true" OnLoggedIn="loginControl_LoggedIn"
		PasswordRecoveryUrl="~/Forgot-Password/" PasswordRecoveryText="Forgot Password" />
		<div class="logonControl" style="width: 220px;">
			<a id="A1" runat="server" href="~/Sign-Up/">Need an account?</a>
		</div>
</asp:Content>
