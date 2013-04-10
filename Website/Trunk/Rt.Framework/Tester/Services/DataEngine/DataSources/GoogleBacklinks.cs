using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[TestFixture]
	public class GoogleBacklinks_Tester
	{
		private GoogleBacklinks gb;

		[SetUp]
		public void Setup()
		{
			gb = new GoogleBacklinks();
		}

		[Test]
		public void GetSearchUrl()
		{
			string searchUrl;

			searchUrl = GoogleBacklinks.GetSearchUrl("http://www.Simpletracking.com");
			Assert.AreEqual("http://www.google.com/search?q=link%3Ahttp%3a%2f%2fwww.Simpletracking.com&num=1", searchUrl);
		}

		[Test]
		public void GetBacklinkCount()
		{
			int? backlinks;

			backlinks = GoogleBacklinks.GetBacklinkCount(readEmbeddedText("GoogleBacklinks_Sample1.txt"));
			Assert.AreEqual(86, backlinks);
		}

		[Test]
		public void GetBacklinkCount2()
		{
			int? backlinks;

			backlinks = GoogleBacklinks.GetBacklinkCount(readEmbeddedText("GoogleBacklinks_NoResults.txt"));
			Assert.AreEqual(0, backlinks);
		}

		/// <summary>
		///		Verify bogus results result in NULL.
		/// </summary>
		[Test]
		public void GetBacklinkCount3()
		{
			int? backlinks;

			backlinks = GoogleBacklinks.GetBacklinkCount("asdfasdge");
			Assert.AreEqual(null, backlinks);
		}

		[Test]
		public void GetBacklinkCount4()
		{
			int? backlinks;

			backlinks = GoogleBacklinks.GetBacklinkCount(null);
			Assert.AreEqual(null, backlinks);
		}

		[Test]
		public void GetNextRequest()
		{
			SerializableWebRequest req;

			gb.Url = "http://www.Simpletracking.com";
			req = gb.GetNextRequest();
			Assert.AreEqual(null, req.PostData);
			Assert.AreEqual("http://www.google.com/search?q=link%3Ahttp%3a%2f%2fwww.Simpletracking.com&num=1", req.Url);
		}

		/// <summary>
		///		Verify that the request is only returned 1 time.
		/// </summary>
		[Test]
		public void GetNextRequest2()
		{
			SerializableWebRequest req;

			gb.Url = "http://www.Simpletracking.com";
			req = gb.GetNextRequest();
			req = gb.GetNextRequest();
		}

		[Test]
		public void SetResponse()
		{
			SerializableWebResponse resp;
			RawDataValue[] values;

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("GoogleBacklinks_Sample1.txt");

			gb.SetResponse(resp);
			values = gb.Values;
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual(null, values[0].DatasourceSubTypeId);
			Assert.AreEqual(86, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.IsTrue(values[0].Timestamp > DateTime.UtcNow.AddSeconds(-10) && values[0].Timestamp <= DateTime.UtcNow);
		}

		/// <summary>
		///		Verify a bad response results in the value having a success = false.
		/// </summary>
		[Test]
		public void SetResponse2()
		{
			SerializableWebResponse resp;
			RawDataValue[] values;

			resp = new SerializableWebResponse();
			resp.Content = "asdfasd";

			gb.SetResponse(resp);
			values = gb.Values;
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual(null, values[0].DatasourceSubTypeId);
			Assert.AreEqual(null, values[0].FloatValue);
			Assert.AreEqual(false, values[0].Success);
			Assert.IsTrue(values[0].Timestamp > DateTime.UtcNow.AddSeconds(-10) && values[0].Timestamp <= DateTime.UtcNow);
		}

		[Test]
		public void Url()
		{
			gb.Url = "asdf";
			Assert.AreEqual("asdf", gb.Url);
		}

		[Test]
		public void Parameters()
		{
			Dictionary<int, object> d = new Dictionary<int, object>();

			gb.Parameters = d;
			Assert.AreEqual(d, gb.Parameters);
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
