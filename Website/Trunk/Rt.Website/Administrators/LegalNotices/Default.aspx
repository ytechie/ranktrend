<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Administrators_LegalNotices_Default" Title="RankTrend.com - Legal Notices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <asp:SqlDataSource runat="server" ID="GlobalSettingsSource" ConnectionString="<%$ ConnectionStrings:main %>"
            SelectCommand="SELECT * FROM LegalNotices"></asp:SqlDataSource>
        <asp:GridView runat="server" ID="MyGridView" DataSourceID="GlobalSettingsSource"
            CssClass="DataGridView" DataKeyNames="Id" AutoGenerateColumns="false" CellPadding="3">
            <Columns>
                <asp:BoundField HeaderText="Id" DataField="Id" />
                <asp:BoundField HeaderText="Description" DataField="Description" />
                <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="EditLegalNotice.aspx?Id={0}"
                    Text="Edit" HeaderText="" />
            </Columns>
        </asp:GridView>
    </ajax:AjaxPanel>
</asp:Content>
