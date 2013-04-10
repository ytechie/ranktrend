using System.Drawing;

namespace Rt.Framework.Applications.Correlation
{
	public class DiagramNode
	{
		public int ConfiguredDatasourceId;
		public Point Coordinates;
		public int? SubTypeId;
		public string Text;

		public DiagramNode()
		{
		}

		public DiagramNode(string text)
		{
			Text = text;
		}

		public DiagramNode(string text, int configuredDatasourceId, int? subTypeId)
		{
			Text = text;
			ConfiguredDatasourceId = configuredDatasourceId;
			SubTypeId = subTypeId;
		}
	}
}