<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Keyword-Summary.aspx.cs"
	Inherits="Members_Reports_Keywords_Keyword_Summary" Title="Keyword Summary Report" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<rt:SiteList runat="Server" ID="siteList" AutoPostBack="true" /><br />
	<br />
	<asp:GridView runat="Server" id="dgKeywordTable" CssClass="TabularTable KeywordSummary"
		AutoGenerateColumns="false">
		<Columns>
			<asp:BoundField HeaderText="Keyword" DataField="TextValue" />
			<asp:TemplateField HeaderText="Yahoo" ItemStyle-CssClass="CheckImageColumn">
				<ItemTemplate>
					<asp:Image runat="server" ImageUrl="~/Images/Check.gif" AlternateText="True"
						ToolTip='<%# string.Format("Watching \"{0}\" in Yahoo.", DataBinder.Eval(Container.DataItem, "TextValue")) %>'
						Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "Yahoo") %>' />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Google" ItemStyle-CssClass="CheckImageColumn">
				<ItemTemplate>
					<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Check.gif" AlternateText="True"
						ToolTip='<%# string.Format("Watching \"{0}\" in Google.", DataBinder.Eval(Container.DataItem, "TextValue")) %>'
						Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "Google") %>' />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="MS Live" ItemStyle-CssClass="CheckImageColumn">
				<ItemTemplate>
					<asp:Image ID="Image2" runat="server" ImageUrl="~/Images/Check.gif" AlternateText="True"
						ToolTip='<%# string.Format("Watching \"{0}\" in MS Live.", DataBinder.Eval(Container.DataItem, "TextValue")) %>'
						Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "MS Live") %>' />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
	<h4>Jump to:</h4>
	<ul>
		<li><a runat="Server" href="~/Members/Keywords/Keyword-Bulk-Import.aspx">Bulk Keyword Importer</a></li>
	</ul>
	<asp:Panel runat="server" ID="noDataPanel">
		There are no keywords set up for the currently selected site.  Please add some keywords, or
		select a different site using the drop down list above.
	</asp:Panel>
</asp:Content>