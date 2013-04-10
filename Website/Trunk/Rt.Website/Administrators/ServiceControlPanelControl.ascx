<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ServiceControlPanelControl.ascx.cs" Inherits="Administrators_ServiceControlPanelControl" %>

<ajax:AjaxPanel ID="AjaxPanel1" runat="server">
    <b><asp:HyperLink ID="WebServiceLink" runat="server" /></b>
			(Runs every <asp:Literal runat="server" ID="Interval" /> minutes)<br />
			<asp:Panel runat="Server" ID="ServiceInformation">
			Last run time: <asp:Literal ID="LastRunTime" runat="server" /><br />
			Last heartbeat: <asp:Literal ID="LastHeartbeat" runat="server" /><br />
			Server time: <asp:Literal ID="ServerTime" runat="server" /><br />
			Next run: <asp:Literal ID="NextRunTime" runat="server" /><br />
			<asp:Label runat="server" ID="EngineStatus" Text="Status Unknown" />
    </asp:Panel>
	Schedule: 
    <asp:LinkButton runat="server" ID="StartEngine" OnClick="StartEngine_Click" Text="Enable" />
    <asp:LinkButton runat="server" ID="StopEngine" OnClick="StopEngine_Click" Text="Disable" /> |
    <asp:LinkButton runat="server" ID="ForceEngineRun" OnClick="ForceEngineRun_Click" Text="Forced Run" /> |
    <asp:LinkButton runat="server" ID="ReloadEngineConfig" OnClick="ReloadEngineConfig_Click" Text="Reload Config" /> |
    <asp:LinkButton runat="server" ID="ClearFlags" OnClick="ClearFlags_Click" Text="Clear Flags (FR and RC)" />
</ajax:AjaxPanel>
