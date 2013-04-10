namespace Rt.Framework.Applications.Correlation
{
	public class DiagramNodeConnection
	{
		public int ConnectorWidth;
		public DiagramNode FirstNode;
		public DiagramNode SecondNode;

		public DiagramNodeConnection()
		{
		}

		public DiagramNodeConnection(DiagramNode firstNode, DiagramNode secondNode)
		{
			FirstNode = firstNode;
			SecondNode = secondNode;
		}

		public bool SharesNode(DiagramNodeConnection otherConnection)
		{
			return FirstNode == otherConnection.FirstNode
			       || SecondNode == otherConnection.FirstNode
			       || FirstNode == otherConnection.SecondNode
			       || SecondNode == otherConnection.SecondNode;
		}
	}
}