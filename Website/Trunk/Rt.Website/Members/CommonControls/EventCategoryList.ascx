<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EventCategoryList.ascx.cs"
	Inherits="Members_CommonControls_EventCategoryList" %>

<%@ Reference Control="~/Members/CommonControls/SiteList.ascx" %>

Event Category: <asp:DropDownList runat="server" ID="ddlCategory" />&nbsp;
<a id="A1" runat="server" href="../Events/Event-Category-Editor/" title="Click to edit your event categories">(edit event categories)</a>