<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="EditLegalNotice.aspx.cs" Inherits="Administrators_LegalNotices_EditLegalNotice"
    Title="RankTrend.com - Legal Notice Editor" %>

<%@ Reference Page="~/Administrators/LegalNotices/Default.aspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <table class="EmailTemplateEditor" border="0">
            <tr>
                <td>
                    <table border="0" style="height: 100%; width: 100%;">
                        <tr>
                            <td align="right">
                                <asp:Label runat="Server" ID="IdLabel" AssociatedControlID="LegalNoticeId" Text="ID:" /></td>
                            <td>
                                <asp:Label runat="server" ID="LegalNoticeId" /></td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label runat="Server" ID="DescriptionLabel" AssociatedControlID="Description" Text="Description:" /></td>
                            <td>
                                <asp:Label runat="server" ID="Description" /></td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label runat="server" ID="NoticeLabel" AssociatedControlID="Notice" Text="Notice:" /></td>
                            <td>
                                <asp:TextBox runat="server" ID="Notice" TextMode="multiLine" CssClass="LegalAgreementTextBox" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Notice"
                                    ErrorMessage="*" ValidationGroup="LegalNotice" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button runat="server" ID="Save" Text="Save" OnClick="Save_Click" ValidationGroup="LegalNotice" />
                                <asp:Button runat="server" ID="Cancel" Text="Cancel" OnClick="Cancel_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </ajax:AjaxPanel>
</asp:Content>
