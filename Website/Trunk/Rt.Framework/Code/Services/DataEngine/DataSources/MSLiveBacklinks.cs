using System;
using System.Collections.Generic;
using System.Text;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	public class MSLiveBacklinks
	{
		/// <summary>
		///		The URL to use for checking backlinks with MS live search.
		/// </summary>
		/// <remarks>
		///		{0} = The encoded URL to check backlinks for.
		/// </remarks>
		private const string BACKLINK_URL = "http://search.live.com/results.aspx?q=link%3A{0}&count=1";

		/// <summary>
		///		A message that is part of the page that is displayed if there are no
		///		backlinks registered for the site.
		/// </summary>
		private const string NO_RESULTS_MESSAGE = "We did not find any results for";

		/// <summary>
		///		The regular expression for the number of backlinks for the site.
		/// </summary>
		private const string REGEX_BACKLINK_COUNT = @"Page \d+ of (\d+) results";
	}
}
