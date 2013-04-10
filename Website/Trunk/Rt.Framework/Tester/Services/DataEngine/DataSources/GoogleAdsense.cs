using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[TestFixture]
	public class GoogleAdsense_Tester
	{
		[Test]
		public void ParseAdsenseCsvReport()
		{
			string report;
			RawDataValue[] reportValues;

			report = readEmbeddedText("SampleAdsenseReportCsv.txt");
			reportValues = GoogleAdsense.ParseAdsenseCsvReport(report, null);
			
			//Earnings
			Assert.AreEqual(6, reportValues.Length);
			Assert.AreEqual(1, reportValues[0].DatasourceSubTypeId);
			Assert.AreEqual(0.90, reportValues[0].FloatValue);
			Assert.AreEqual(DateTime.Parse("2006-10-21"), reportValues[0].Timestamp);

			Assert.AreEqual(1, reportValues[1].DatasourceSubTypeId);
			Assert.AreEqual(1.01, reportValues[1].FloatValue);
			Assert.AreEqual(DateTime.Parse("2006-10-22"), reportValues[1].Timestamp);

			//Impressions
			Assert.AreEqual(2, reportValues[2].DatasourceSubTypeId);
			Assert.AreEqual(268, reportValues[2].FloatValue);
			Assert.AreEqual(DateTime.Parse("2006-10-21"), reportValues[2].Timestamp);

			Assert.AreEqual(2, reportValues[3].DatasourceSubTypeId);
			Assert.AreEqual(143, reportValues[3].FloatValue);
			Assert.AreEqual(DateTime.Parse("2006-10-22"), reportValues[3].Timestamp);

			//Clicks
			Assert.AreEqual(4, reportValues[4].DatasourceSubTypeId);
			Assert.AreEqual(11, reportValues[4].FloatValue);
			Assert.AreEqual(DateTime.Parse("2006-10-21"), reportValues[4].Timestamp);

			Assert.AreEqual(4, reportValues[5].DatasourceSubTypeId);
			Assert.AreEqual(5, reportValues[5].FloatValue);
			Assert.AreEqual(DateTime.Parse("2006-10-22"), reportValues[5].Timestamp);
		}
		
		/// <summary>
		///		Verify that filtering by channel works
		/// </summary>
		[Test]
		public void ParseAdsenseCsvReport2()
		{
			//TODO!!
		}

		[Test]
		public void Sample1()
		{
			GoogleAdsense ga;
			SerializableWebRequest req;
			SerializableWebResponse resp;

			ga = new GoogleAdsense();
			ga.Parameters = new Dictionary<int, object>();
			ga.Parameters.Add(1, "testUser");
			ga.Parameters.Add(2, "testPass");

			//Stage 1
			req = ga.GetNextRequest();
			Assert.AreEqual(null, req.PostData);
			Assert.AreEqual("https://www.google.com/accounts/ServiceLoginAuth?service=adsense&hl=en-US&ltmpl=login&ifr=true&passive=true&rm=hide&nui=3&alwf=true&continue=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth&followup=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth", req.Url);

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("GoogleAdsense_Sample1_Stage1.txt");

			ga.SetResponse(resp);


			//Stage 2
			req = ga.GetNextRequest();
			Assert.AreEqual("https://www.google.com/accounts/ServiceLoginAuth", req.Url);
			Assert.AreEqual("ltmpl=login&continue=https%3a%2f%2fwww.google.com%2fadsense%2fgaiaauth&followup=https%3a%2f%2fwww.google.com%2fadsense%2fgaiaauth&service=adsense&nui=3&ifr=true&rm=hide&ltmpl=login&hl=en-US&alwf=true&Email=testUser&Passwd=testPass&null=Sign+in", req.PostData);

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("GoogleAdsense_Sample1_Stage2.txt");

			ga.SetResponse(resp);

			//Stage 3
			req = ga.GetNextRequest();
			Assert.AreEqual("https://www.google.com/accounts/CheckCookie?continue=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth&followup=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth&service=adsense&hl=en-US&chtml=LoginDoneHtml", req.Url);
			Assert.AreEqual(null, req.PostData);

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("GoogleAdsense_Sample1_Stage3.txt");

			ga.SetResponse(resp);

			//Stage 4
			req = ga.GetNextRequest();
			Assert.AreEqual("https://www.google.com/adsense/report/aggregate?dateRange.dateRangeType=custom&dateRange.customDate.start.month=" + DateTime.Now.AddDays(-1).Date.Month + "&dateRange.customDate.start.day=" + DateTime.Now.AddDays(-1).Date.Day + "&dateRange.customDate.start.year=" + DateTime.Now.AddDays(-1).Date.Year + "&dateRange.customDate.end.month=" + DateTime.Now.AddDays(-1).Month + "&dateRange.customDate.end.day=" + DateTime.Now.AddDays(-1).Date.Day + "&dateRange.customDate.end.year=" + DateTime.Now.AddDays(-1).Date.Year + "&outputFormat=TSV_EXCEL", req.Url);
			Assert.AreEqual(null, req.PostData);

			resp = new SerializableWebResponse();
			resp.Content = readEmbeddedText("GoogleAdsense_Sample1_Stage4.txt");

			ga.SetResponse(resp);

			//Check data
			RawDataValue[] values;
			values = ga.Values;
			Assert.AreEqual(3, values.Length);
			Assert.AreEqual(1, values[0].DatasourceSubTypeId);
			Assert.AreEqual(true, values[0].Success);
			Assert.AreEqual(1.64, values[0].FloatValue);
			Assert.AreEqual(DateTime.Parse("2-22-07"), values[0].Timestamp);

			Assert.AreEqual(2, values[1].DatasourceSubTypeId);
			Assert.AreEqual(true, values[1].Success);
			Assert.AreEqual(616, values[1].FloatValue);
			Assert.AreEqual(DateTime.Parse("2-22-07"), values[1].Timestamp);

			Assert.AreEqual(4, values[2].DatasourceSubTypeId);
			Assert.AreEqual(true, values[2].Success);
			Assert.AreEqual(16, values[2].FloatValue);
			Assert.AreEqual(DateTime.Parse("2-22-07"), values[2].Timestamp);
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
