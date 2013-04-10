using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[TestFixture]
	public class GoogleRank_Tester
	{
		GoogleRank gr;
		SerializableWebRequest req;
		RawDataValue[] values;

		[SetUp]
		public void Setup()
		{
			gr = new GoogleRank();
			gr.Parameters = new Dictionary<int, object>();
		}

		/// <summary>
		///		Verify that the correct request URL is created.
		/// </summary>
		[Test]
		public void GetNextRequest()
		{
			gr.Parameters.Add(1, "test phrase");
			gr.Url = "http://www.Test.com";

			req = gr.GetNextRequest();
			Assert.IsNull(req.PostData);
			Assert.AreEqual("http://www.google.com/search?q=test phrase&num=100", req.Url);
		}

		[Test]
		public void SetResponse()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults.txt");

			gr.Url = "http://www.blackwell-synergy.com";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(8, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse_NoResults()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults_NoResults.txt");

			gr.Url = "http://www.Test.com";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.IsNull(values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse_8Results()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults_8Results.txt");

			gr.Url = "http://www.ObiShawn.com";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(1, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse_BadResults()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults_Bad.txt");

			gr.Url = "http://www.blackwell-synergy.com";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(null, values[0].FloatValue);
			Assert.AreEqual(false, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse2()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults2.txt");

			gr.Url = "http://www.humanmetrics.com/cgi-win/JTypes1.htm";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(4, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse3()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults3.txt");

			gr.Url = "http://www.kbalertz.com/kb_933568.aspx";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(2, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse4()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults4.txt");

			gr.Url = "http://www.obishawn.com/archive/2006/02/19/4.aspx";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(7, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse5()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults5.txt");

			gr.Url = "http://en.wikipedia.org/wiki/Scuba_diving";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(5, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse6()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults6.txt");

			gr.Url = "maps.caribseek.com/";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(4, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		/// <summary>
		///		Tests search results that have unsafe page warnings.
		/// </summary>
		[Test]
		public void SetResponse7()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults7.txt");

			gr.Url = "http://find.yuku.com/find/communities?q=survivorsucks+teens";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(11, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		[Test]
		public void SetResponse8()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults8.txt");

			gr.Url = "blah";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		/// <summary>
		///		This is to test the new onmousedown stuff that can be
		///		in the links.
		/// </summary>
		/// <remarks>
		///		Sample: onmousedown="return clk(0,'','','res','7','')"
		/// </remarks>
		[Test]
		public void SetResponse9()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults9.txt");

			gr.Url = "blah";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		/// <summary>
		///		This is to test the fact that size=-1 can appear in a font tag
		///		after the color info, instead of before.
		/// </summary>
		[Test]
		public void SetResponse10()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults10.txt");

			gr.Url = "blah";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(false, values[0].Fuzzy);
		}

		/// <summary>
		///		Verify that if google says there are 102 matches, but we
		///		only find 100, the result is still valid, but fuzzy.
		/// </summary>
		[Test]
		public void SetResponse_Fuzzy()
		{
			SerializableWebResponse sr;

			sr = new SerializableWebResponse();
			sr.Content = readEmbeddedText("GoogleRankResults_Fuzzy.txt");

			gr.Url = "blah";
			gr.SetResponse(sr);

			values = gr.Values;

			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(true, values[0].Fuzzy);
		}

		private string readEmbeddedText(string fileName)
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
