using System;
using System.Collections.Generic;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Members.WhatToTrack;
using Spring.Context;
using Spring.Context.Support;

public partial class Members_what_to_track : Page
{
	private KeywordManager _keywordManager;

	protected void Page_Load(object sender, EventArgs e)
	{
		IApplicationContext context = ContextRegistry.GetContext();
		_keywordManager = (KeywordManager) context.GetObject("keywordManager");

		cmdSave.Click += cmdSave_Click;
		cmdSave2.Click += cmdSave_Click;

		siteList.SelectedItemChanged += siteList_SelectedItemChanged;

		if (!IsPostBack)
		{
			siteList.PopulatePageList();
			populateInitialKeywordList();
		}
	}

	private void cmdSave_Click(object sender, EventArgs e)
	{
		string[] keywordArr = txtKeywords.Text.Split(new[] {"" + Convert.ToChar(13) + Convert.ToChar(10)},
		                                             StringSplitOptions.RemoveEmptyEntries);
		var keywords = new List<string>();

		//Copy the items to a list, without copying duplicates
		foreach (string currKeyword in keywordArr)
		{
			if (!keywords.Contains(currKeyword))
				keywords.Add(currKeyword);
		}

		_keywordManager.SaveKeywordList(siteList.GetSelectedSite().Url, keywords);
	}

	private void siteList_SelectedItemChanged(object sender, EventArgs e)
	{
		populateInitialKeywordList();
	}

	private void populateInitialKeywordList()
	{
		UrlClass selectedSite = siteList.GetSelectedSite();

		//TODO: Handle this failure mode better
		if (selectedSite == null)
			return;

		List<string> keywords = _keywordManager.GetKeywordList(selectedSite.Url);
		txtKeywords.Text = string.Join(Convert.ToChar(10).ToString(), keywords.ToArray());
	}
}