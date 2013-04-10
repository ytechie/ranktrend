<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Thumbnail-Section.ascx.cs"
	Inherits="Members_Reports_Thumbnail_Dashboard_Thumbnail_Section" %>

<%@ Reference Page="~/Common/SessionImage.aspx" %>

<span class="thumb_OuterContainer">
	<a runat="Server" href="" id="lnkDrillDown">
		<img runat="server" src="" alt="" id="imgThumb" class="thumb_Chart" />
	</a>
	<asp:CheckBox runat="server" ID="chkTitle" CssClass="thumb_titleCheckbox" />
</span>