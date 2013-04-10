<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Members_TermsOfService_Default" Title="RankTrend.com - Terms of Service Agreement Renewal" %>

<%@ Register Assembly="YTech.General" Namespace="YTech.General.Web.Controls.Validators"
    TagPrefix="ytv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="Server">
    <ajax:AjaxPanel ID="AjaxPanel1" runat="server">
        <p>
            The Terms of Service has changed since you last agreed to them. To continue using
            RankTrend.com we ask that you renew your agreement to the Terms of Service.
        </p>
        <table border="0" style="width: 100%; height: 100%;">
            <tr>
                <td align="center">
                    <table border="0" style="width: 400px;">
                        <tr>
                            <td>
                                <asp:TextBox runat="server" ID="TermsOfService" TextMode="MultiLine" CssClass="LegalAgreementTextBox"
                                    ReadOnly="true" /><br />
                                <asp:CheckBox runat="server" ID="chkAgree" Text="I agree to Terms of Service" /><br />
                                <ytv:RequiredCheckboxValidator runat="server" ID="CheckBoxValidator1" ControlToValidate="chkAgree"
                                    MustBeChecked="true" Display="Dynamic" ErrorMessage="You must agree to the terms of service before you can continue." />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Button runat="server" ID="Save" Text="Save" OnClick="Save_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </ajax:AjaxPanel>
</asp:Content>
