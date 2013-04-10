using System.Collections.Generic;

namespace Rt.Framework.Members.WhatToTrack
{
	public interface IKeywordManager
	{
		List<string> GetKeywordList(string url);
		void SaveKeywordList(string url, IEnumerable<string> keywords);
	}
}
