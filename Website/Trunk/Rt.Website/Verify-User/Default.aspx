<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Verify_User_Default" Title="RankTrend.com - Verify User" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <asp:Panel runat="server" ID="UserApprovedPanel" Visible="false" CssClass="userVerificationControl">
        User <b><asp:Literal runat="Server" ID="UserName" /></b> has been approved.  You may now log in.
        <asp:HyperLink runat="server" ID="LoginLink" NavigateUrl="~/Login/" Text="Click here" /> to log in.
    </asp:Panel>
    <asp:Panel runat="server" ID="UserNotApprovedPanel" Visible="false" CssClass="userVerificationControl">
        User verification failed.  The membership key provided was unrecognized.
    </asp:Panel>
</asp:Content>
