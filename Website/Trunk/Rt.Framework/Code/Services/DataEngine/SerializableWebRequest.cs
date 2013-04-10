using System;
using System.Collections.Generic;
using System.Text;

namespace Rt.Framework.Services.DataEngine
{
	/// <summary>
	///		A wrapper for web requests so that it can be
	///		serialized for easy transfer to a remote proxy.
	/// </summary>
	/// <remarks>
	///		This is used so that remote websites can be accessed by
	///		sending requests to a remote server, which will pass
	///		back the results as a <see cref="SerializableWebResponse" />.
	/// </remarks>
	[Serializable]
	public class SerializableWebRequest
	{
		private string _url;
		private string _postData;

		/// <summary>
		///		Gets or sets the URL of the remote website that will
		///		be requested.
		/// </summary>
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		//TODO: support this
		public string PostData
		{
			get { return _postData; }
			set { _postData = value; }
		}

		//public SerializableWebRequest()
		//{
		//  //System.Net.HttpWebRequest w;
		//  //w.
		//}
	}
}
