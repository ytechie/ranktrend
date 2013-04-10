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
	public class Delicious_Tester
	{
		private Delicious d;

		[SetUp]
		public void Setup()
		{
			d = new Delicious();
		}

		[Test]
		public void GetRequest()
		{
			SerializableWebRequest req;

			d.Url = "http://pajhome.org.uk/crypt/md5/";
			req = d.GetNextRequest();
			Assert.AreEqual(null, req.PostData);
			Assert.AreEqual("http://del.icio.us/url/0d96ac703118f0df9da6f1515cb1a0a1", req.Url);
		}

		[Test]
		public void GetRequest2()
		{
			SerializableWebRequest req;

			d.Url = "http://www.simpletracking.com/";
			req = d.GetNextRequest();
			Assert.AreEqual(null, req.PostData);
			Assert.AreEqual("http://del.icio.us/url/48c7077d723347d56051e7ae1a749bff", req.Url);
		}

		/// <summary>
		///		Verify that requests after the first return NULL.
		/// </summary>
		[Test]
		public void GetRequest3()
		{
			SerializableWebRequest req;

			d.Url = "http://pajhome.org.uk/crypt/md5/";
			d.GetNextRequest();
			req = d.GetNextRequest();
			Assert.AreEqual(null, req);
		}

		[Test]
		public void GetRequest4()
		{
			SerializableWebRequest req;

			d.Url = "http://www.ranktrend.com";
			req = d.GetNextRequest();
			Assert.AreEqual("http://del.icio.us/url/39e95426f371796c946bb1854648f899", req.Url);
		}

		[Test]
		public void GetRequest5()
		{
			SerializableWebRequest req;

			d.Url = "http://www.ranktrend.com/";
			req = d.GetNextRequest();
			Assert.AreEqual("http://del.icio.us/url/39e95426f371796c946bb1854648f899", req.Url);
		}

		[Test]
		public void FormatUrl()
		{
			Assert.AreEqual("http://pajhome.org.uk/crypt/md5/", Delicious.FormatUrl("http://pajhome.org.uk/crypt/md5/"));
		}

		[Test]
		public void FormatUrl2()
		{
			Assert.AreEqual("http://test.com/", Delicious.FormatUrl("http://test.com"));
		}

		[Test]
		public void FormatUrl3()
		{
			Assert.AreEqual("http://test.co.uk/", Delicious.FormatUrl("http://test.co.uk"));
		}

		[Test]
		public void FormatUrl4()
		{
			Assert.AreEqual("http://www.obishawn.com/", Delicious.FormatUrl("http://www.obishawn.com/"));
		}

		[Test]
		public void GetLinkCount()
		{
			int? linkCount;

			linkCount = Delicious.GetLinkCount(readEmbeddedText("Delicious_Sample1.txt"));

			Assert.AreEqual(369, linkCount);
		}

		[Test]
		public void GetLinkCount2()
		{
			int? linkCount;

			linkCount = Delicious.GetLinkCount(readEmbeddedText("Delicious_NoLinks.txt"));

			Assert.AreEqual(0, linkCount);
		}

		[Test]
		public void GetLinkCount3()
		{
			int? linkCount;

			linkCount = Delicious.GetLinkCount("this is bad text");

			Assert.AreEqual(null, linkCount);
		}

		[Test]
		public void GetLinkCount4()
		{
			int? linkCount;

			linkCount = Delicious.GetLinkCount(readEmbeddedText("Delicious_1Link.txt"));

			Assert.AreEqual(1, linkCount);
		}

		[Test]
		public void SetResponse()
		{
			SerializableWebResponse resp;

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("Delicious_Sample1.txt");

			d.SetResponse(resp);
		}

		[Test]
		public void Values()
		{
			SerializableWebResponse resp;
			RawDataValue[] values;

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("Delicious_Sample1.txt");

			d.SetResponse(resp);

			values = d.Values;
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual(null, values[0].DatasourceSubTypeId);
			Assert.AreEqual(369, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.IsTrue(values[0].Timestamp > DateTime.UtcNow.AddSeconds(-10) && values[0].Timestamp <= DateTime.UtcNow);
		}

		[Test]
		public void Values2()
		{
			SerializableWebResponse resp;
			RawDataValue[] values;

			resp = new SerializableWebResponse();
			resp.Content = "adsfasdf";

			d.SetResponse(resp);

			values = d.Values;
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual(null, values[0].DatasourceSubTypeId);
			Assert.AreEqual(null, values[0].FloatValue);
			Assert.AreEqual(false, values[0].Success);
			Assert.IsTrue(values[0].Timestamp > DateTime.UtcNow.AddSeconds(-10) && values[0].Timestamp <= DateTime.UtcNow);
		}

		[Test]
		public void Url()
		{
			d.Url = "asdk";
			Assert.AreEqual("asdk", d.Url);
		}

		[Test]
		public void Parameters()
		{
			Dictionary<int, object> dict = new Dictionary<int, object>();

			d.Parameters = dict;
			Assert.AreEqual(dict, d.Parameters);
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
