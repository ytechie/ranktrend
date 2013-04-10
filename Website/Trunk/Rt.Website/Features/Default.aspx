<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Features_Default" Title="RankTrend.com Features" %>

<%@ OutputCache Duration="300" VaryByParam="*" VaryByCustom="staticWhenLoggedOut" Location="Server" %>

<asp:Content ContentPlaceHolderID="mainContent" runat="Server">

    <script language="javascript" type="text/javascript">
// <!CDATA[

function IMG1_onclick() {

}

function IMG2_onclick() {

}

// ]]>
    </script>

    <h2>
        Feature Overview - Follow the Trend</h2>
    <p>
        Whether you’re an ecommerce, blog, forum, affiliate, AdSense or simply a personal
        site, RankTrend.com can not only track your search engine rank in the big 3 but
        more importantly it can help you recognize valuable relationships between your data.
        Here are some of the features and benefits of RankTrend.com:
    </p>
    <ul>
        <li><strong>Reporting Features</strong>
            <br />
            RankTrend.com is all about graphically displaying your data. Here are the ways we
            do that:
            <ul style="list-style-position: outside; list-style-type: circle">
                <li>Interactive Custom Charts <a href="Images/InteractiveChartEditor-large.jpg">
                    <img id="ChartEditor" runat="Server" src="Images/InteractiveChartEditor-Small.jpg"
                        alt="RankTrend.com Interactive Chart Editor" style="clear: right; float: right;
                        visibility: visible;" /></a><br />
                    Quickly build custom charts interactively; save them to view anytime. Add 1, 2 or
                    more datasources to your charts so you can see the correlation between the data.
                </li>
                <li>Line Creation Options
                    <br />
                    Display all the points or just the trend of your data.</li><li>Multiple Plots on the
                        Same Chart
                        <br />
                        Displaying the data on the same chart shows patterns of correlation between the
                        data. This is more than interesting; it’s important. For example, with RankTrend.com
                        you can see the effect of your actions on your keyword rank and therefore your AdSense
                        and other revenue. </li>
                <li>Auto Scaling – it’s all relative
                    <br />
                    Charts with more than 2 datasources on them will automatically convert display the
                    data in a relative format so you can pick out trends more easily </li>
                <li>Chart Dashboard Pages
                    <br />
                    Put multiple charts on the same page, create a bookmark or make it your homepage.
                    Since the data is updated automatically, your dashboard is ready to view anytime.
                </li>
                <li>Periodic Chart Emails
                    <br />
                    Receive daily, weekly or monthly emails of any dashboard you create. </li>
                <li>NEW – Correlation Diagram
                    <br />
                    <a href="Images/CorrelationDiag-Large.jpg">
                        <img id="CorrelationDiagram" src="Images/CorrelationDiag-Smallest.jpg" alt="RankTrend.com Correlation Diagram"
                            style="float: left; page-break-after: always; clip: rect(auto auto auto auto);
                            padding-top: 10px;" /></a><div style="position: ; width: 400px; padding-top: 10px;">
                                Visually see correlations between data with our new Correlation Diagram. The thicker
                                the line the more correlated the 2 pieces of data are. This screen shot just doesn't
                                do it justice - <a runat="server" href="~/Sign-Up/">sign up</a> and create your
                                own powerful correlation diagram.</div>
                    &nbsp;</li></ul>
        </li>
        <li style="list-style-type: none;"></li>
        <br />
        <br />
        <br />
        <li><strong>Automated Tracking Features</strong>
            <br />
            Set it and forget it. RankTrend.com will retrieve the data to report on automatically
            every day through the following built in "datasources".<ul style="list-style-position: outside;
                list-style-type: circle">
                <li>Track Any Site
                    <img id="IMG2" src="Images/Data-Configuration-Control-Panel-Snippet.gif" style="float: right;
                        padding-right: 50px;" onclick="return IMG2_onclick()" /><br />
                    RankTrend.com does not require any scripts on your web pages so you can track your
                    own sites, your competitors sites or just sites you are interested in. </li>
                <li>Rank by Keyword
                    <br />
                    Track a website’s rank for any keyword on any or all of the 3 major search engines:
                    Google, Yahoo or MSN Live </li>
                <li>Diggs
                    <br />
                    Did somebody Digg one of your pages? Track the number of Diggs you receive. </li>
                <li>Google AdSense Revenue
                    <br />
                    See how your AdSense revenue increases now that RankTrend.com tells you where to
                    focus your efforts. </li>
                <br />
                <img src="Images/GoogleRank.jpg" style="padding-right: 20px; float: none; clip: rect(auto auto auto auto)" />
                <br />
                <li>Google PageRank
                    <br />
                    It’s still a valuable piece of data and since it’s checked every day you will know
                    exactly when it changed. </li>
                <li>Del.icio.us Tags
                    <br />
                    Watch how many people are tagging your web pages. </li>
                <li>Backlinks
                    <br />
                    See when the number of links reported by Google, Yahoo or MSN Live change. </li>
                <li>Blog Entries
                    <br />
                    When it comes to blogs, RankTrend.com not only adds a high cool factor but it does
                    it automatically through the RSS feed of any blog. This feed is used to add an “event”
                    for each blog post – the event is named after the title of the blog entry. </li>
                <li>Blog Comments
                    <br />
                    Using the same RSS feed, and if the feed supports it, RankTrend.com will track the
                    number of comments on each blog post. Combine this data with the Blog Entry Events
                    and you have a powerful tool to monitor hot blog topics (the more comments the hotter
                    the topic…) </li>
            </ul>
        </li>
        <li><strong>Manual Tracking Features</strong>
            <br />
            We can’t think of everything so we allow you to manually add any data you desire
            to chart:
            <ul style="list-style-position: outside; list-style-type: circle">
                <li>Generic Data
                    <br />
                    Import any comma or tab separated file (.csv) to add data to RankTrend.com – for
                    instance, grab your AWStats data and import it in to RankTrend.com.</li><li>Manual “Events”
                        <ul>
                            <li>Did you:
                                <ul style="list-style-position: outside;">
                                    <li style="list-style-type: none">Run an ad for your site? </li>
                                    <li style="list-style-type: none">Implement a new Google AdWords campaign?</li><li
                                        style="list-style-type: none">Change the layout of your home page? </li>
                                    <li style="list-style-type: none">Run a special? </li>
                                    <li style="list-style-type: none">Distribute an article?</li></ul>
                            </li>
                            <br />
                            <img src="Images/GoogleRankWithEvents.jpg" style="float: none; text-align: center" /><br />
                            <br />
                            <li>What ever actions you take, you can track these “events” and then compare their
                                relationship to other data over time. For example, what happened to your backlinks
                                after you posted that article to iSnare.com? Or what happened to your AdSense revenue
                                after you changed the layout and copy on your home page? </li>
                        </ul>
                    </li>
                <li>Manual Event Categories
                    <br />
                    Track and display types of events separately. Track your article submissions in
                    one category and your blog posts in another. This gives you full control over what
                    types of events you want to compare against other data. </li>
            </ul>
        </li>
        <li><strong>Multiple Plans</strong><br />
            From small to large, we have a plan that will work for you. See the <a runat="server"
                href="~/Plans/">Plans</a> tab for more details.
            <ul>
                <li>Free<br />
                    We remember. Starting out with just a site or 2 just doesn’t leave a lot of profit
                    to buy fancy tools. No problem, we’ve got you covered with our Free plan. </li>
                <li>Paid<br />
                    Now that you are growing you will probably want to change to one of our paid packages
                    that allow you far more flexibility. </li>
            </ul>
        </li>
    </ul>
    <p>
        &nbsp;</p>
</asp:Content>
