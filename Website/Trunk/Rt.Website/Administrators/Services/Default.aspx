<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Administrators_Services_Default" Title="RankTrend.com - Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" Runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <asp:SqlDataSource runat="server" ID="MySource" ConnectionString="<%$ ConnectionStrings:main %>"
            SelectCommand="Select * From Services" UpdateCommand="Update Services Set Description=@Description, RunIntervalMinutes=@RunIntervalMinutes, Owner=@Owner Where Id=@Id">
            <UpdateParameters>
                <asp:Parameter Name="Id" Type="int32" />
                <asp:Parameter Name="Description" Type="string" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="RunIntervalMinutes" Type="int32" ConvertEmptyStringToNull="true" />
                <asp:Parameter Name="Owner" Type="string" ConvertEmptyStringToNull="false" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:GridView runat="server" ID="MyGridView" DataSourceID="MySource" CssClass="DataGridView"
            DataKeyNames="Id" AutoGenerateEditButton="true" AutoGenerateColumns="false" CellPadding="3">
            <Columns>
                <asp:BoundField HeaderText="Description" DataField="Description" />
                <asp:BoundField HeaderText="Run Interval Minutes" DataField="RunIntervalMinutes" />
                <asp:BoundField HeaderText="Owner" DataField="Owner" />
            </Columns>
        </asp:GridView>
    </ajax:AjaxPanel>
</asp:Content>

