<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="SendEmail.aspx.cs" Inherits="Administrators_EmailTemplates_SendEmail"
    Title="Send Email" %>

<%@ Reference Page="~/Administrators/EmailTemplates/Default.aspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <table class="EmailTemplateEditor" border="0">
            <tr>
                <td>
                    <table border="0" style="height: 100%; width: 100%;">
                        <tr>
                            <th align="right" style="width: 100px;">
                                <asp:Label runat="server" ID="IdLabel" AssociatedControlID="EmailTemplateId" Text="ID:" />
                            </th>
                            <td>
                                <asp:Literal runat="server" ID="EmailTemplateId" />
                            </td>
                        </tr>
                        <tr>
                            <th align="right">
                                <asp:Label runat="server" ID="FromLabel" AssociatedControlID="From" Text="From:" />
                            </th>
                            <td>
                                <asp:Literal runat="server" ID="From" />
                            </td>
                        </tr>
                        <tr>
                            <th align="right">
                                <asp:Label runat="server" ID="ToLabel" AssociatedControlID="To" Text="To:" /></th>
                            <td>
                                <asp:DropDownList runat="server" ID="To" />
                            </td>
                        </tr>
                        <tr>
                            <th align="right">
                                <asp:Label runat="server" ID="SubjectLabel" AssociatedControlID="Subject" Text="Subject:" />
                            </th>
                            <td>
                                <asp:Literal runat="server" ID="Subject" />
                            </td>
                        </tr>
                        <tr>
                            <th align="right">
                                <asp:Label runat="server" ID="MessageFormatLabel" AssociatedControlID="MessageFormat" Text="Message Format:" />
                            </th>
                            <td>
                                <asp:Literal runat="server" ID="MessageFormat" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="SendEmail_MessageBodyMessage">
                                    Note: The preview below is being affected by the stylesheets applied to the RankTrend.com website.
                                    Do not rely on this as the actual preview.  To see a true preview, go to the Edit Template page
                                    and click 'Save and Email Preview' button.
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="SendEmail_MessageBody">
                                    <asp:Literal runat="server" ID="MessageBody" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:Button runat="server" ID="Send" Text="Send" OnClick="Send_Click" />
                                <asp:Button runat="server" ID="Cancel" Text="Cancel" OnClick="Cancel_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </ajax:AjaxPanel>
</asp:Content>
