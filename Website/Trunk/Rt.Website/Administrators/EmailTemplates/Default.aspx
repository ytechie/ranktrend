<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Administrators_EmailTemplates_Default" Title="Email Templates" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <p style="text-align: right">
        <asp:HyperLink runat="server" NavigateUrl="~/Administrators/EmailTemplates/EditTemplate.aspx">New Template</asp:HyperLink>
    </p>
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <asp:GridView runat="server" ID="dgEmailTemplates" AutoGenerateColumns="false" CssClass="DataGridView">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" />
                <asp:TemplateField HeaderText="Message Body">
                    <ItemTemplate>
                        <%# htmlEncode(limit(DataBinder.Eval(Container.DataItem, "Message").ToString(), 500)) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="EditTemplate.aspx?Id={0}"
                    Text="Edit" HeaderText="" />
                <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="SendEmail.aspx?Id={0}"
                    Text="Send" HeaderText="" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            CommandName="Delete" Text="Delete" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem, "Locked") %>'
                            OnClick="Delete_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </ajax:AjaxPanel>
</asp:Content>
