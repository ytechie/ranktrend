<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Subscription.aspx.cs" Inherits="Members_Profile_Subscription"
	Title="Subscription Management" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<p>
		Your current plan: <b>Free</b>
	</p>
	<p>
		Not sure which plan is right for you? Check out the
		<a href="~/Plans/">Plan Comparison</a>.
	</p>
	<asp:RadioButtonList runat="server" ID="lstPlans">
		<asp:ListItem Text="Free Plan" Value="1" />
		<asp:ListItem Text="$1.99/Month Plan Paid Monthly" Value="2M" />
		<asp:ListItem Text="$1.99/Month Plan Paid Yearly" Value="2Y" />
		<asp:ListItem Text="$19.99/Month Plan Paid Montly" Value="3M" />
		<asp:ListItem Text="$19.99/Month Plan Paid Yearly - Save 10%!" Value="3Y" Selected="true" />
	</asp:RadioButtonList><br />
	<asp:Button runat="server" ID="cmdChangePlan" Text="Change Plan" /><br />
	
	<p>
		We use the PayPal subscription service, which means that you never
		pay us directly, and you can always be confident that your personal
		information is secure. You don't even need a PayPal account to
		subscribe to one of our plans!
	</p>
	<p>
		When you upgrade your account to one of our premium paid options, you
		will be directed to the PayPal site, where you will fill in the apropriate
		information. When you are done, you will be directed back to our site. It may
		take a few minutes for our server to be notified by PayPal that the transaction
		is complete. If your account does not get upgraded within 24 hours, please
		contact us for assistance.
	</p>
	
</asp:Content>