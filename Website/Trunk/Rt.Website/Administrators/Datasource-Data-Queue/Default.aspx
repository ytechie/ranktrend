<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="Administrators_Datasource_Data_Queue_Default"
	Title="Queued Datasource Data" %>

<%@ Register TagPrefix="YTJS" Namespace="YTech.General.Web.JSProxy" Assembly="YTech.General" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<YTJS:JSProxyGenerator runat="Server" />

	<asp:GridView runat="server" ID="dvQueue" AutoGenerateColumns="false"
		CssClass="TabularTable" RowStyle-CssClass="TabularTableItem"
		AlternatingRowStyle-CssClass="TabularTableAltItem" >
		<Columns>
			<asp:BoundField DataField="Id" HeaderText="Queue ID" />
			<asp:TemplateField HeaderText="Execute">
				<ItemTemplate>
					<asp:Button runat="server" ID="cmdReprocess" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
							CommandName="Reprocess" Text="Re-process this item" OnClick="Reprocess_Click" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:HyperLinkField HeaderText="View" DataNavigateUrlFormatString="View-Queue-Item.aspx?Id={0}"
				text="View this queue item" DataNavigateUrlFields="Id" />
			<asp:TemplateField HeaderText="Delete">
				<ItemTemplate>
					<asp:Button runat="server" ID="cmdDelete"
						CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
						CommandName="Delete" Text="Delete" OnClick="Delete_Click" />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
</asp:Content>