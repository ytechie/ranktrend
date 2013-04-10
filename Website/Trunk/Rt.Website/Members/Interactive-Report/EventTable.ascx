<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EventTable.ascx.cs" Inherits="Members_Interactive_Report_EventTable" %>

<asp:DataGrid runat="server" ID="dgEvents" AutoGenerateColumns="false" GridLines="None"
	CssClass="TabularTable" ItemStyle-CssClass="TabularTableItem" AlternatingItemStyle-CssClass="TabularTableAltItem">
	<Columns>
		<asp:BoundColumn DataField="Id" Visible="false" />
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:CheckBox runat="server" ID="chkSelected" ToolTip="Check to display events in this category" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="Name" HeaderText="Event Category" HeaderStyle-Width="200" />
		<asp:BoundColumn DataField="Url" HeaderText="Page" HeaderStyle-Width="200" />
	</Columns>
</asp:DataGrid>