<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
	Inherits="Members_Import_Data_Wizard_Default" Title="Import Data Wizard" %>

<asp:Content ContentPlaceHolderID="mainContent" Runat="Server">
		<table width="100%">
			<tr>
				<td align="center">
					<asp:Wizard runat="Server" ID="wizard" CssClass="Wizard"
						HeaderStyle-CssClass="WizardHeader" NavigationStyle-CssClass="WizardNavigation"
						SideBarStyle-CssClass="WizardSideBar" StepStyle-CssClass="WizardStep"
						Height="312" Width="500"
						FinishDestinationPageUrl="../">
						<HeaderTemplate>
							<h1>Import Data Wizard</h1>
							<h2><%= wizard.ActiveStep.Title%></h2>
						</HeaderTemplate>
						<WizardSteps>
							<asp:WizardStep Title="1. Import File Type">
								What type of file would you like to import?<br />
								<asp:DropDownList runat="server" ID="ddlFileType">
									<asp:ListItem Text="Generic Comma Separated Text File" Value="2" />
									<asp:ListItem Text="Generic Tab Separated Text File" Value="3" />
									<asp:ListItem Text="Google Adsense CSV Report" Value="1" />
									<asp:ListItem Text="Digital Point Backlink Export" Value="4" />
								</asp:DropDownList>
							</asp:WizardStep>
							<asp:WizardStep Title="2. Upload Your Data File">
								Please choose the file that contains the data to import:<br />
								<asp:FileUpload runat="server" ID="csvUpload" CssClass="csvUploadControl" /><br />
								When you click "Next", the file will be uploaded, and it may take a moment
								to process the data.
							</asp:WizardStep>
							<asp:WizardStep Title="3. Data Selection">
								<asp:Panel runat="server" ID="pnlTypeDigitalPointBacklinks" Visible="false">
									Please select the website name to choose the data that you
									would like to import.<br />
									<asp:DropDownList runat="server" ID="ddlDPBacklinksSite" />
								</asp:Panel>
								<asp:Panel runat="server" ID="pnlFilterAdsense" Visible="false">
									Please select the channel name for the Adsense data that you
									would like to import.  If you do not need to select a channel, the
									list will be disabled, and you can continue to the next step.<br />
									<asp:DropDownList runat="server" ID="ddlChannelNames" />
								</asp:Panel>
								<asp:Panel runat="server" ID="pnlTypeGeneric" Visible="false">
									Select the column that contains the dates of the data points
									you would like to import:<br />
									<asp:DropDownList runat="server" ID="ddlTimestampColumn" /><br />
									Select the column that contains the values of the data points
									you would like to import:<br />
									<asp:DropDownList runat="server" ID="ddlValueColumn" />
								</asp:Panel>
								<asp:Panel runat="server" ID="pnlTypeError">
									<asp:Literal runat="server" ID="lblError" Text="Undefined Error" />
								</asp:Panel>
							</asp:WizardStep>
							<asp:WizardStep Title="4. Select Associated Datasource">
								Please select the datasource that the imported data should belong to:<br />
								Datasource: <asp:DropDownList runat="server" ID="ddlDatasources" />
							</asp:WizardStep>
						</WizardSteps>
					</asp:Wizard>
				</td>
			</tr>
		</table>
</asp:Content>

