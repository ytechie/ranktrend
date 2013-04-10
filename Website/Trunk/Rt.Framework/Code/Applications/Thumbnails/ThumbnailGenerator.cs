using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using YTech.General.Charting.Controls;
using YTech.General.Drawing;
using YTech.General.Mathematics;

namespace Rt.Framework.Applications.Thumbnails
{
	public class ThumbnailGenerator
	{
		private Database _db;

		public ThumbnailGenerator(Database db)
		{
			_db = db;
		}

		public string Generate(int configuredDatasourceId, int? subTypeId, DateTime start, DateTime end)
		{
			DataTable rawData;
			byte[] image;
			ThumbnailChart chart;

			rawData = _db.QueryRawData(start, end, configuredDatasourceId, subTypeId);
			//using (chart = new ThumbnailChart())
			//{
				Bitmap b;
				DateTime[] timestamps;
				double?[] values, timestamps2;
				double slope, yIntercept;
				
				bool reverse;
				ConfiguredDatasource cds = _db.ORManager.Get<ConfiguredDatasource>(configuredDatasourceId);
				reverse = cds.DatasourceType.Reversed;

				processRawData(rawData, out timestamps, out values);

				//LineChart lc = new LineChart(175, 100, LineChartType.SingleDataSet);

			//	float[] chartVals = new float[values.Length];
				//for (int i = 0; i < values.Length; i++)
				//	if(values[i] != null)
				//		chartVals[i] = (float)values[i].Value;

				//lc.SetData(chartVals);

			

			double? min, max;
			YTech.General.Charting.ChartMath.GetDataBounds(values, out min, out max);

			if (min == null)
				return "http://chart.apis.google.com/chart?cht=lxy&chs=175x100";

			string dataString = GetDataString(timestamps, values, min.Value - 2);

			Color backgroundColor = getChartBackground(timestamps, values, reverse);
			string bgHex = ColorToGoogleHex(backgroundColor);

			return
				string.Format("http://chart.apis.google.com/chart?cht=lxy&chs=175x100&chd=t:{0}|{1}&chds=0,100,{1},{2}&chf=bg,s,{3}&chco=000000", dataString,
				              min.Value - 1, max.Value + 1, bgHex);
				/*
				timestamps2 = new double?[timestamps.Length];
				for (int i = 0; i < timestamps.Length; i++)
					if (values[i] != null) timestamps2[i] = timestamps[i].ToOADate();
				MathFunctions.TrendFunction(values, timestamps2, out slope, out yIntercept);
				if (double.IsNaN(slope)) slope = 0;
				if (reverse) slope *= -1;
				alpha = (int) ((Math.Abs(Math.Atan(slope))/(Math.PI/2))*191.0) + 64;

				chart.InverseScale = reverse;
				chart.Height = 100;
				chart.Width = 175;

				chart.Timestamps = timestamps;
				chart.YValues = values;
				if (slope == 0)
					chart.BackgroundColor = Color.White;
				else if (slope > 0)
					chart.BackgroundColor = Color.FromArgb(alpha, Color.Green);
				else
					chart.BackgroundColor = Color.FromArgb(alpha, Color.Red);
				b = chart.GenerateBitmap();
				image = ImageUtilities.BitmapToBytes(b, ImageFormat.Png);
			}
				
			return image;
			*/
//return lc.GetUrl();
		}

		public static string ColorToGoogleHex(Color color)
		{
			string toReturn = string.Format("0x{0:X8}", color.ToArgb());
			//note: The alpha value is at the end
			toReturn = toReturn.Substring(toReturn.Length - 6, 6) + toReturn.Substring(toReturn.Length - 8, 2);
			return toReturn;
		}

		private static Color getChartBackground(DateTime[] timestamps, double?[] values, bool reversed)
		{
			double?[] timestampValues;
			double slope, yIntercept;
			int alpha;

			timestampValues = new double?[timestamps.Length];
			for (int i = 0; i < timestamps.Length; i++)
				if (values[i] != null) timestampValues[i] = timestamps[i].ToOADate();
			MathFunctions.TrendFunction(values, timestampValues, out slope, out yIntercept);
			if (double.IsNaN(slope)) slope = 0;
			if (reversed) slope *= -1;
			alpha = (int)((Math.Abs(Math.Atan(slope)) / (Math.PI / 2)) * 191.0) + 64;

			if (slope == 0)
				return Color.White;
			else if (slope > 0)
				return Color.FromArgb(alpha, Color.Green);
			else
				return Color.FromArgb(alpha, Color.Red);
		}

		private static string GetDataString(DateTime[] timestamps, double?[] values, double nullValue)
		{
			StringBuilder ts = new StringBuilder();
			StringBuilder vs = new StringBuilder();
		
			if(values.Length < 1)
				return "";

			//TODO: Check if the data points are evenly spaced to avoid sending the timestamps

			DateTime minTime = new DateTime();
			DateTime maxTime = new DateTime();
			for (int i = 0; i < timestamps.Length; i++)
			{
				if (i == 0 || timestamps[i] < minTime)
					minTime = timestamps[i];
				if (i == 0 || timestamps[i] > maxTime)
					maxTime = timestamps[i];
			}

				for (int i = 0; i < values.Length; i++)
				{
					ts.AppendFormat("{0:00.##}", ((double)(timestamps[i].Ticks - minTime.Ticks) / (double)(maxTime.Ticks - minTime.Ticks)) * 100);

					if (values[i] == null)
						vs.Append(nullValue);
					else
						vs.Append(values[i]);

					if (i != values.Length - 1)
					{
						ts.Append(",");
						vs.Append(",");
					}
				}

			return string.Format("{0}|{1}", ts, vs);
		}

		private static void processRawData(DataTable rawData, out DateTime[] timestamps, out double?[] values)
		{
			timestamps = new DateTime[rawData.Rows.Count];
			values = new double?[timestamps.Length];

			for (int i = 0; i < rawData.Rows.Count; i++)
			{
				timestamps[i] = (DateTime) rawData.Rows[i]["Timestamp"];
				if (rawData.Rows[i]["FloatValue"] == DBNull.Value || (double) rawData.Rows[i]["FloatValue"] == -1)
					values[i] = null;
				else
					values[i] = (double) rawData.Rows[i]["FloatValue"];
			}
		}
	}
}