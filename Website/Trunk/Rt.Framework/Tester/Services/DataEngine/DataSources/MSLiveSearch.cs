using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[TestFixture]
	public class MSLiveSearch_Tester
	{
		MSLiveSearch m;

		[SetUp]
		public void Setup()
		{
			m = new MSLiveSearch();
		}

		[Test]
		public void GetSearchPosition()
		{
			int? searchPosition;

			searchPosition = MSLiveSearch.GetSearchPosition(readEmbeddedText("MSLiveSearch_Sample1.txt"), "http://www.speakeasy.net/speedtest/");

			Assert.AreEqual(3, searchPosition);
		}

		[Test]
		public void GetSearchPosition2()
		{
			int? searchPosition;

			searchPosition = MSLiveSearch.GetSearchPosition(readEmbeddedText("MSLiveSearch_NoResults.txt"), "http://www.speakeasy.net/speedtest/");

			Assert.AreEqual(0, searchPosition);
		}

		[Test]
		public void GetNextRequest()
		{
			SerializableWebRequest req;

			m.Parameters = new Dictionary<int, object>();
			m.Parameters.Add(1, "test search");

			req = m.GetNextRequest();
			Assert.AreEqual("http://search.live.com/results.aspx?q=test search&format=rss&count=100", req.Url);
			Assert.AreEqual(null, req.PostData);
		}

		/// <summary>
		///		Verify that requesting the request after it's been made results in NULL being returned.
		/// </summary>
		[Test]
		public void GetNextRequest2()
		{
			m.Parameters = new Dictionary<int, object>();
			m.Parameters.Add(1, "test search");

			m.GetNextRequest();
			Assert.AreEqual(null, m.GetNextRequest());
		}

		[Test]
		public void SetResponse()
		{
			SerializableWebResponse resp;
			RawDataValue[] values;

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("MSLiveSearch_Sample1.txt");

			m.Parameters = new Dictionary<int, object>();
			m.Parameters.Add(1, "test search");
			m.Url = "http://www.speakeasy.net/speedtest/";

			m.GetNextRequest();

			m.SetResponse(resp);

			values = m.Values;
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual(3, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.IsTrue(values[0].Timestamp > DateTime.UtcNow.AddSeconds(-10) && values[0].Timestamp <= DateTime.UtcNow);
		}

		[Test]
		public void Url()
		{
			m.Url = "asdt";
			Assert.AreEqual("asdt", m.Url);
		}

		private static string readEmbeddedText(string fileName)
		{
			Stream readStream;
			StreamReader reader;
			string namespaceName;
			Assembly asm;

			namespaceName = typeof(GoogleAdsense_Tester).Namespace;
			asm = typeof(GoogleAdsense_Tester).Assembly;

			readStream = asm.GetManifestResourceStream(namespaceName + "." + fileName);

			if (readStream == null)
				throw new FileNotFoundException("Could Not Find Embedded File");

			reader = new StreamReader(readStream);

			return reader.ReadToEnd();
		}
	}
}
