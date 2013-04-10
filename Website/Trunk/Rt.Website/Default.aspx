<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Default.aspx.cs" Inherits="_Default" Title="RankTrend.com - Search engine position analysis tools" %>

<%@ OutputCache Duration="300" VaryByParam="*" VaryByCustom="staticWhenLoggedOut" Location="Server" %>

<asp:Content runat="server" ContentPlaceHolderID="mainContent">
	<h2>Save Time, Automatically Monitor Your Rank</h2>
	<div style="display: block; height: 225px;">
		<img src="Images/Chart-Woman.jpg" style="float: left; margin: 10px; margin-right: 20px;" />
		<p>
			RankTrend automatically monitors your position in the major search engines for all of
			your keywords. You no longer have the painstaking task of monitoring you rank on your own. Using
			this data, you can start to make the right decisions about changes to your site, and
			you will learn what it takes to build a successful site.
		</p>
		<p>
			RankTrend is different than the services you've used in the past. Our reports are web
			based, so they are available anytime, anywhere. Our results are also more accurate,
			because our unique tray application captures
			the rank exactly as you would see it in your browser.
		</p>
		<a href="Sign-Up/">
			<img src="Images/Get-Started.gif" alt="Get Started" title="c'mon, click it, you know you want to!" style="margin-left: 100px; margin-top: 10px;" />
		</a>
	</div>
	<div class="featuresContainer">
		<div class="features">
			<div>
				<img runat="server" src="~/Images/features_automatic.jpg" alt="wind up toy" title="Automatic" />
				<span class="feature">Automatic</span>
				<p>We track your rankings every day.</p>
			</div>
			<div>
				<img runat="server" src="~/Images/features_simple.jpg" alt="simple" title="Simple" />
				<span class="feature">Simple</span>
				<p>Tell us what to track within minutes.</p>
			</div>
			<div>
				<img runat="server" src="~/Images/features_versitile.jpg" alt="versitile" title="Versitile" />
				<span class="feature">Versitile</span>
				<p>Track your rank, ad revenue, costs and visitors.</p>
			</div>
			<div>
				<img runat="server" src="~/Images/features_popular.jpg" alt="popular" title="Popular" />
				<span class="feature">Popular</span>
				<p>We support Google, Yahoo!, MS Live and more.</p>
			</div>
			<div>
				<img runat="server" src="~/Images/features_interactive.jpg" alt="interactive" title="Interactive" />
				<span class="feature">Interactive</span>
				<p>Explore your data.</p>
			</div>
			<div>
				<img runat="server" src="~/Images/features_free.jpg" alt="free" title="Free" />
				<span class="feature">Free</span>
				<p>We don't charge for anything.</p>
			</div>
		</div>
	</div>
</asp:Content>
