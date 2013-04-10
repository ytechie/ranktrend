<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
	Inherits="Members_Reports_Correlation_Default" Title="Data Source Correlation Report" %>
	
<%@ Reference Page="~/Common/SessionImage.aspx" %>
	
<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
	<table>
		<tr>
			<td style="width: 400px;">
				<rt:SiteList runat="server" ID="siteList" AutoPostBack="true" /><br />
			</td>
			<td>
				<div class="helpHeader showHelpHeader">
					<img id="Img1" runat="server" src="~/Images/Help-Icon.gif" alt="Help" /> Help
				</div>
				<div class="help">
					<p>
						<b>Question:</b> What does this report tell me?<br />
						<b>Answer:</b> This report is designed to be a visual way of understanding
							the correlations between the various data sources you have
							been tracking. For example, you may be able to learn which
							data sources are most important for earning you income.
					</p>
					<p>
						<b>Question:</b> What is the significance of the different line thicknesses
							of the connections?<br />
						<b>Answer:</b> The thicker the line, the stronger the correlation according
							to <a href="http://en.wikipedia.org/wiki/Correlation">standard statistical methods.</a>
					</p>
					<p>
						<b>Question:</b> Can I click on the data sources?<br />
						<b>Answer:</b> Yes, you can click on the data sources. You will be taken to the
							interactive trend report, which will show the data source you clicked on
							as well as all data sources that are connected in the chart. This will let you
							get a visual understanding of how the data sources relate.
					</p>
				</div>
			</td>
		</tr>
	</table>
	
	<asp:PlaceHolder runat="server" ID="imagesPlaceholder" />
	<asp:Panel runat="server" ID="pnlNoCorrelations">
		<br />
		No correlations could be found in the data for the selected site.  Please select another site,
		or wait for more data to become available for patterns to appear.
	</asp:Panel>
</asp:Content>