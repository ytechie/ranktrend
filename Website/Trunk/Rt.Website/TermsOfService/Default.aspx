<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="TermsOfService_Default" Title="RankTrend Terms of Service" %>

<%@ OutputCache Duration="300" VaryByParam="*" VaryByCustom="staticWhenLoggedOut" Location="Server" %>

<asp:Content ContentPlaceHolderID="mainContent" runat="Server">
    <asp:Literal runat="server" ID="Tos" />
</asp:Content>
