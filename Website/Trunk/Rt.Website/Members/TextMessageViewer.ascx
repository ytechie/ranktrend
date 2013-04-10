<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TextMessageViewer.ascx.cs" Inherits="Members_TextMessageViewer" %>
	<asp:Panel runat="server" ID="TextMessagePanel" CssClass="TextMessage">
		<h6>Message</h6>
		<p><asp:Label runat="server" ID="TextMessageTimestamp" CssClass="TextMessage_Date" /></p>
		<p><asp:Literal runat="server" ID="TextMessage" /></p>
		<p style="text-align: right;"><asp:LinkButton runat="server" ID="Acknowledge" Text="Delete" OnClick="Acknowledge_Click" /></p>
	</asp:Panel>
