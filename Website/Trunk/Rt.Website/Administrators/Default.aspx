<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Administrators_Default" Title="Administration" %>

<%@ Register TagPrefix="Rt" TagName="ServiceControlPanelControl" Src="~/Administrators/ServiceControlPanelControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <div class="left ControlPanel">
        <h3>
            RankTrend Configuration</h3>
        <ul>
            <li class="cp_envelope">
                <asp:HyperLink runat="server" ID="lnkEmailTemplates" NavigateUrl="EmailTemplates/Default.aspx"
                    Text="Email Templates" />
            </li>
            <li class="cp_ini">
                <asp:HyperLink runat="server" ID="lnkGlobalSettings" NavigateUrl="GlobalSettings/Default.aspx"
                    Text="Global Settings" />
            </li>
            <li>
                <asp:HyperLink runat="server" ID="lnkLegalNotices" NavigateUrl="LegalNotices/Default.aspx"
                    Text="Legal Notices (Terms of Services)" />
            </li>
            <li class="cp_services">
							<asp:HyperLink runat="server" ID="lnkServices" NavigateUrl="Services/Default.aspx"
								Text="Service Configuration" />
            </li>
        </ul>
        <br />
        <h3>
            Administration</h3>
        <ul>
            <li>
                <ajax:AjaxPanel ID="AjaxPanel2" runat="server">
                    <asp:Button runat="server" ID="cmdUpdateDatasources" OnClick="updateDatasources" Text="Update All Datasources" />
                </ajax:AjaxPanel>
            </li>
            <li>
							<asp:HyperLink runat="server" ID="lnkCustomReports" NavigateUrl="./CustomReports/"
								Text="Send Custom Report" />
            </li>
        </ul>
        <br />
        <h3>
            System</h3>
        <ul>
						<li class="cp_text">
                <asp:HyperLink runat="server" ID="lnkLogViewer" NavigateUrl="./LogViewer/"
                    Text="Log Viewer" />
            </li>
            <li class="cp_envelope">
                <asp:HyperLink runat="server" ID="lnkEmailViewer" NavigateUrl="./EmailViewer/"
                    Text="Unsent Email Queue" />
            </li>
            <li>
							<asp:HyperLink runat="server" ID="lnkStatusReport" NavigateUrl="./Status-Report/" Text="System Statistics Report" />
            </li>
            <li>
							<asp:HyperLink runat="server" ID="lnkUserViewer" NavigateUrl="./UserViewer/"
								Text="User Status" />
            </li>
            <li>
							<a href="http://www.ranktrend.com/stats/current-month/awstats.ranktrend.com.html">AWStats for Current Month</a>
            </li>
            <li>
							<asp:HyperLink runat="server" NavigateUrl="./Datasource-Data-Queue/" Text="Datasource Data Queue" />
            </li>
        </ul>
        <br />
        <h3>
            Services</h3>
        <ul>
            <li class="cp_services">
							<Rt:ServiceControlPanelControl runat="server" Id="EmailEngineService" ServiceId="1" />
            </li>
            <li class="cp_services">
							<Rt:ServiceControlPanelControl runat="server" Id="WebEmailEngineService" ServiceId="2" />
            </li>
            <li class="cp_services">
							<Rt:ServiceControlPanelControl runat="server" Id="ReportEngineService" ServiceId="3" />
            </li>
            <li class="cp_services">
							<Rt:ServiceControlPanelControl runat="server" Id="WebReportEngineService" ServiceId="6" />
            </li>
            <li class="cp_services">
							<Rt:ServiceControlPanelControl runat="server" Id="RssEventEngineService" ServiceId="4" />
            </li>
            <li class="cp_services">
							<Rt:ServiceControlPanelControl runat="server" Id="WebRssEventEngineService" ServiceId="5" />
            </li>
        </ul>
    </div>
</asp:Content>
