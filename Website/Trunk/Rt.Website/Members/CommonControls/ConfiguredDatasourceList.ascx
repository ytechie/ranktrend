<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ConfiguredDatasourceList.ascx.cs"
	Inherits="Members_CommonControls_ConfiguredDatasourceList" %>

Data Type:
<asp:DropDownList runat="server" ID="ddlDatasources" />&nbsp;
<a runat="server" href="~/Members/Datasources/Edit-Datasources.aspx" title="Click to edit your data sources">(edit data sources)</a>
<asp:Panel runat="server" ID="pnlSubTypes" Visible="false">
	Sub Type:
	<asp:DropDownList runat="server" ID="ddlSubTypes" />
</asp:Panel>