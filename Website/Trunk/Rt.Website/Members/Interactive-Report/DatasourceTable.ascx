<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DatasourceTable.ascx.cs" Inherits="Members_Interactive_Report_DatasourceTable" %>

<%@ Register TagPrefix="ColorDD" Namespace="UNLV.IAP.WebControls" Assembly="UNLV.IAP.WebControls.HtmlColorDropDown" %>

<asp:DataGrid runat="server" ID="dgDatasources" AutoGenerateColumns="false" GridLines="None"
	CssClass="TabularTable" ItemStyle-CssClass="TabularTableItem" AlternatingItemStyle-CssClass="TabularTableAltItem">
	<Columns>
		<asp:BoundColumn DataField="Id" Visible="false" />
		<asp:BoundColumn DataField="SubTypeId" Visible="false" />
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:CheckBox runat="server" ID="chkSelected" ToolTip="Check to display this data source" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="Data Type" HeaderText="Data Type" HeaderStyle-Width="200" />
		<asp:BoundColumn DataField="Url" HeaderText="Page" HeaderStyle-Width="200" />
		<asp:TemplateColumn HeaderText="Trend Line"
			HeaderStyle-Width="35" HeaderStyle-Wrap="true" HeaderStyle-HorizontalAlign="center" ItemStyle-HorizontalAlign="center">
			<ItemTemplate>
				<asp:CheckBox ID="chkTrendLine" runat="server" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Curve Fit"
			HeaderStyle-Width="35" HeaderStyle-Wrap="true" HeaderStyle-HorizontalAlign="center" ItemStyle-HorizontalAlign="center">
			<ItemTemplate>
				<asp:CheckBox ID="chkCurveFit" runat="server" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Color">
			<ItemTemplate>
				<ColorDD:HtmlColorDropDown runat="server" ID="ddlColor" Palette="Simple"
					DisplaySelectColorItemText="Auto Select" DisplaySelectColorItem="true" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Thickness">
			<ItemTemplate>
				<asp:DropDownList runat="server" ID="ddlLineThickness">
					<asp:ListItem Text="Thin" Value="1" />
					<asp:ListItem Text="Medium" Value="2" />
					<asp:ListItem Text="Thick" Value="3" />
				</asp:DropDownList>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>