using System.Collections.Generic;
using Rt.Framework.Api;

namespace Rt.Framework.Members.WhatToTrack
{
	public class KeywordManager : IKeywordManager
	{
		private readonly IDatasourceRepository _datasourceRepo;

		public KeywordManager(IDatasourceRepository datasourceRepo)
		{
			_datasourceRepo = datasourceRepo;
		}

		public List<string> GetKeywordList(string url)
		{
			var datasources = _datasourceRepo.GetDatasources(url, true, new[] {1, 2, 7});
			var keywords = new List<string>();

			foreach(var currDatasource in datasources)
			{
				var keyword = currDatasource.Parameters[0].Value;
				if(!keywords.Contains(keyword))
					keywords.Add(keyword);
			}

			return keywords;
		}

		public void SaveKeywordList(string url, IEnumerable<string> keywords)
		{
			var keywordList = new List<string>();
			foreach(var currKeyword in keywords)
				keywordList.Add(currKeyword);

			_datasourceRepo.ReplaceKeywords(url, keywordList.ToArray());
		}
	}
}
