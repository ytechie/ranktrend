<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Administrators_WebService_Default" Title="RankTrend.com - Web Service" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <h2>
            <asp:Literal runat="server" ID="ServiceName" /></h2>
        <asp:ImageButton runat="server" ID="Refresh" AlternateText="Refresh" ImageUrl="~/Images/Refresh.jpg" OnClick="Refresh_Click" /><br />
        <b>Web Service Status</b><br />
        <asp:Panel runat="server" ID="WebServiceInformation">
					<b><asp:Literal runat="server" ID="ServiceOwnerName" /></b><br />
					Heart Is <asp:Literal runat="server" ID="HeartBeating" />Beating<br />
					<asp:Literal runat="server" ID="IsRunning" /><br />
					Is Service Owner: <asp:Literal runat="server" ID="IsServiceOwner" />
        </asp:Panel>
        <br />
        <b>Database Service Status</b>
        <br />
				<asp:Panel runat="Server" ID="ServiceInformation">
					Service Owner: <b><asp:Literal ID="ServiceOwner" runat="server" /></b><br />
					Last run time: <asp:Literal ID="LastRunTime" runat="server" /><br />
					Last heartbeat: <asp:Literal ID="LastHeartbeat" runat="server" /><br />
					Server time: <asp:Literal ID="ServerTime" runat="server" /><br />
					Next run: <asp:Literal ID="NextRunTime" runat="server" /><br />
					<asp:Label runat="server" ID="EngineDatabaseStatus" Text="Status Unknown" />
				</asp:Panel>
        <br />
        <asp:LinkButton runat="server" ID="StartHeartbeat" OnClick="StartHeartbeat_Click"
            Text="Start Heartbeat" />
        <asp:LinkButton runat="server" ID="StopHeartbeat" OnClick="StopHeartbeat_Click"
            Text="Stop Heartbeat" />
        |
        <asp:LinkButton runat="server" ID="StartEngine" OnClick="StartEngine_Click" Text="Start" />
        <asp:LinkButton runat="server" ID="StopEngine" OnClick="StopEngine_Click" Text="Stop" />
        |
        <asp:LinkButton runat="server" ID="ForceEngineRun" OnClick="ForceEngineRun_Click"
            Text="Force Run" />
        |
        <asp:LinkButton runat="server" ID="ForceHeartbeat" OnClick="ForceHeartbeat_Click" Text="Force Heartbeat" />
        |
        <asp:LinkButton runat="server" ID="ForceConfigReload" OnClick="ForceConfigReload_Click" Text="Force Config Reload" />
    </ajax:AjaxPanel>
</asp:Content>
