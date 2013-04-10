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
	public class Digg_Tester
	{
		private Digg d;
		private SerializableWebRequest req;
		private SerializableWebResponse resp;

		[SetUp]
		public void Setup()
		{
			d = new Digg();
			d.Parameters = new Dictionary<int, object>();
		}

		[Test]
		public void RequestUrl()
		{
			d.Parameters.Add(1, "http://www.digg.com/asdfasdf");
			req = d.GetNextRequest();
			Assert.AreEqual("http://www.digg.com/asdfasdf", req.Url);
			Assert.AreEqual(null, req.PostData);
		}

		[Test]
		public void Diggs()
		{
			RawDataValue[] values;

			d.Parameters.Add(1, "http://www.digg.com/asdfasdf");
			req = d.GetNextRequest();
			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("Digg_Sample2.txt");
			d.SetResponse(resp);

			values = d.Values;
			Assert.AreEqual(2, values.Length);

			Assert.AreEqual(5, values[0].DatasourceSubTypeId);
			Assert.AreEqual(100, values[0].FloatValue);
			Assert.AreEqual(true, values[0].Success);
			Assert.IsTrue(values[0].Timestamp > DateTime.UtcNow.AddSeconds(-10) && values[0].Timestamp <= DateTime.UtcNow);
		}

		[Test]
		public void Comments()
		{
			RawDataValue[] values;

			d.Parameters.Add(1, "http://www.digg.com/asdfasdf");
			req = d.GetNextRequest();
			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("Digg_Sample2.txt");
			d.SetResponse(resp);

			values = d.Values;
			Assert.AreEqual(2, values.Length);

			Assert.AreEqual(6, values[1].DatasourceSubTypeId);
			Assert.AreEqual(16, values[1].FloatValue);
			Assert.AreEqual(true, values[1].Success);
			Assert.IsTrue(values[1].Timestamp > DateTime.UtcNow.AddSeconds(-10) && values[1].Timestamp <= DateTime.UtcNow);
		}

		[Test]
		public void ErrorHandling()
		{
			RawDataValue[] values;

			d.Parameters.Add(1, "http://www.digg.com/asdfasdf");
			req = d.GetNextRequest();
			resp = new SerializableWebResponse();
			resp.Content = "asdfasdf";
			d.SetResponse(resp);

			values = d.Values;

			Assert.AreEqual(2, values.Length);
			Assert.AreEqual(false, values[0].Success);
			Assert.AreEqual(false, values[1].Success);
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
