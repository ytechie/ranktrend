<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Administrators_GlobalSettings_Default"
    MasterPageFile="~/MasterPage.master" Title="RankTrend.com - Global Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    Note: Empty string will automatically translate to <i>{NULL}</i>.
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <asp:SqlDataSource runat="server" ID="GlobalSettingsSource" ConnectionString="<%$ ConnectionStrings:main %>"
            SelectCommand="SELECT * FROM GlobalSettings" UpdateCommand="Update GlobalSettings Set IntValue=@IntValue, TextValue=@TextValue Where Id=@Id">
            <UpdateParameters>
                <asp:Parameter Name="Id" Type="int32" />
                <asp:Parameter Name="IntValue" Type="int32" ConvertEmptyStringToNull="true" />
                <asp:Parameter Name="TextValue" Type="string" ConvertEmptyStringToNull="true" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:GridView runat="server" ID="MyGridView" DataSourceID="GlobalSettingsSource" CssClass="DataGridView"
            DataKeyNames="Id" AutoGenerateEditButton="true" AutoGenerateColumns="false" CellPadding="3">
            <Columns>
                <asp:BoundField HeaderText="Description" DataField="Description" ReadOnly="true" />
                <asp:BoundField HeaderText="Int Value" DataField="IntValue" NullDisplayText="<i>{NULL}</i>" />
                <asp:BoundField HeaderText="Text Value" DataField="TextValue" NullDisplayText="<i>{NULL}</i>" />
            </Columns>
        </asp:GridView>
    </ajax:AjaxPanel>
</asp:Content>
