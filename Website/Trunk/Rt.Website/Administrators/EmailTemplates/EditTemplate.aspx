<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="EditTemplate.aspx.cs" Inherits="Administrators_EmailTemplates_EditTemplate"
    Title="Email Template Editor" ValidateRequest="false" %>

<%@ Reference Page="~/Administrators/EmailTemplates/Default.aspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <table class="EmailTemplateEditor" border="0">
            <tr>
                <td>
                    <table border="0" style="height: 100%; width: 100%;">
                        <tr>
                            <td align="right">
                                <asp:Label runat="Server" ID="IdLabel" AssociatedControlID="EmailTemplateId" Text="ID:" /></td>
                            <td>
                                <asp:Label runat="server" ID="EmailTemplateId" /></td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label runat="server" ID="SubjectLabel" AssociatedControlID="Subject" Text="Subject:" /></td>
                            <td>
                                <asp:TextBox runat="server" ID="Subject" CssClass="EmailTemplateEditor_Subject" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Subject"
                                    ErrorMessage="*" ValidationGroup="EmailTemplate" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label runat="server" ID="HtmlLabel" AssociatedControlID="Html" Text="HTML:" /></td>
                            <td>
                                <asp:CheckBox runat="server" ID="Html" Checked="true" /></td>
                        </tr>
                         <tr>
                            <td align="right">
                                <asp:Label runat="server" ID="LockLabel" AssociatedControlID="Locked" Text="Locked:" /></td>
                            <td>
                                <asp:CheckBox runat="server" ID="Lock" Checked="true" Visible="true" />
                                <asp:Literal runat="server" ID="Locked" Text="Locked" Visible="False" />
                                </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <div class="EmailTemplateEditor_MessageHeader">
                                    Develop the email message body using an HTML editor like Visual Studio 2005, then
                                    copy and paste the HTML into the Message text box here. Click "Save" to simply save
                                    the template or click "Save and Email Preview" to save the template and also use
                                    that template to queue an email to the currently logged in user.
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label runat="server" ID="MessageLabel" AssociatedControlID="Message" Text="Message Body:" /></td>
                            <td>
                                <asp:TextBox runat="server" ID="Message" TextMode="multiLine" CssClass="EmailTemplateEditor_Message" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Message"
                                    ErrorMessage="*" ValidationGroup="EmailTemplate" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button runat="server" ID="Save" Text="Save" OnClick="Save_Click" ValidationGroup="EmailTemplate" />
                                <asp:Button runat="server" ID="SaveAndPreview" Text="Save and Email Preview" OnClick="SaveAndPreview_Click"
                                    ValidationGroup="EmailTemplate" />
                                <asp:Button runat="server" ID="Cancel" Text="Cancel" OnClick="Cancel_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="height: 20px;">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table class="EmailTemplateEditor_SupportedTokens" border="0" style="height: 100%;
                                    width: 100%;">
                                    <tr>
                                        <th colspan="3">
                                            Supported Tokens</th>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="center">
                                            <i>General</i>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="HomepageUrl" /></td>
                                        <td>
                                            Home Page URL
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="HomepageUrlExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="HomepageLink" /></td>
                                        <td>
                                            Home Page Link
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="HomepageLinkExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="Logo" /></td>
                                        <td>
                                            RankTrend.com Logo linked to RankTrend.com
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="LogoExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="center">
                                            <i>User Specific</i>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="UsersUid" /></td>
                                        <td>
                                            User's Unique Id</td>
                                        <td>
                                            <asp:Label runat="server" ID="UsersUidExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="UsersFirstName" /></td>
                                        <td>
                                            User's First Name</td>
                                        <td>
                                            <asp:Label runat="server" ID="UsersFirstNameExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="UsersLastName" /></td>
                                        <td>
                                            User's Last Name</td>
                                        <td>
                                            <asp:Label runat="server" ID="UsersLastNameExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="UsersFullName" /></td>
                                        <td>
                                            User's Full Name</td>
                                        <td>
                                            <asp:Label runat="server" ID="UsersFullNameExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="UsersUsername" /></td>
                                        <td>
                                            User's User Name</td>
                                        <td>
                                            <asp:Label runat="server" ID="UsersUsernameExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Literal runat="server" ID="UsersEmail" /></td>
                                        <td>
                                            User's Email Address</td>
                                        <td>
                                            <asp:Label runat="server" ID="UsersEmailExample" CssClass="EmailTemplateEditor_TokenExample" /></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </ajax:AjaxPanel>
</asp:Content>
