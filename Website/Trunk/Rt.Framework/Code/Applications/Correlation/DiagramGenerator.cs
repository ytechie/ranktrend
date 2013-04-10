using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using log4net;
using Netron.Diagramming.Core.Layout.Force;
using YTech.General.Collections;

namespace Rt.Framework.Applications.Correlation
{
	public class DiagramGenerator
	{
		private const int DIAGRAM_MARGIN = 50;
		private const int SIMULATION_ITERATIONS = 2000;

		/// <summary>
		///		Create and declare our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private List<DiagramNodeConnection> _connections;
		private List<HotSpot> _hotspots;
		private int _imageWidth;
		private Dictionary<DiagramNode, ForceItem> _nodeMap;
		private List<DiagramNode> _nodes;
		private ForceSimulator _simulator;

		public DiagramGenerator(List<DiagramNodeConnection> connections, int imageWidth)
		{
			//Build a node list from the connections
			_nodes = new List<DiagramNode>();
			foreach (DiagramNodeConnection currConnection in connections)
			{
				if (!_nodes.Contains(currConnection.FirstNode))
					_nodes.Add(currConnection.FirstNode);
				if (!_nodes.Contains(currConnection.SecondNode))
					_nodes.Add(currConnection.SecondNode);
			}

			_connections = connections;
			_simulator = new ForceSimulator();
			_simulator.AddForce(new NBodyForce());
			_simulator.AddForce(new SpringForce());
			_simulator.AddForce(new DragForce());
			_imageWidth = imageWidth;
		}

		public DiagramGenerator(List<DiagramNode> nodes, List<DiagramNodeConnection> connections, int imageWidth)
		{
			_nodes = nodes;
			_connections = connections;
			_simulator = new ForceSimulator();
			_simulator.AddForce(new NBodyForce());
			_simulator.AddForce(new SpringForce());
			_simulator.AddForce(new DragForce());
			_imageWidth = imageWidth;
		}

		public List<HotSpot> Hotspots
		{
			get { return _hotspots; }
		}

		public Bitmap Generate()
		{
			initNodesAndConnections();
			calculateNodePositions();
			return drawDiagram();
		}

		private void initNodesAndConnections()
		{
			Random rand;

			_nodeMap = new Dictionary<DiagramNode, ForceItem>();

			//Use a pre-defined seed so that the "random" numbers are always the same.
			//This will ensure an even distribution, yet the layout will not change
			//each time it is rendered.
			rand = new Random(42346234);

			//Add the nodes
			foreach (DiagramNode currDiagramNode in _nodes)
			{
				ForceItem forceNode;

				forceNode = new ForceItem();
				forceNode.Location = new float[] {rand.Next(500), rand.Next(500)};
				forceNode.Mass = (float) 1.0;

				//Add the node to our map so we can look up the force item by node
				_nodeMap.Add(currDiagramNode, forceNode);

				_simulator.addItem(forceNode);
			}

			//Add the springs/connections
			foreach (DiagramNodeConnection currConnection in _connections)
			{
				_simulator.addSpring(_nodeMap[currConnection.FirstNode], _nodeMap[currConnection.SecondNode]);
			}
		}

		private void calculateNodePositions()
		{
			Stopwatch stopwatch;

			stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < SIMULATION_ITERATIONS; i++)
			{
				long timestep = 1000;
				long step;

				timestep *= (long) (1.0 - ((double) i/(double) SIMULATION_ITERATIONS));
				step = timestep + 50;

				_simulator.RunSimulator(step);
			}

			stopwatch.Stop();

			_log.DebugFormat("Node layout completed in {0} milliseconds", stopwatch.ElapsedMilliseconds);

			//Now we have to map the force items back to the nodes
			foreach (DiagramNode currNode in _nodeMap.Keys)
			{
				ForceItem fi;

				fi = _nodeMap[currNode];
				currNode.Coordinates.X = (int) Math.Round(fi.Location[0]);
				currNode.Coordinates.Y = (int) Math.Round(fi.Location[1]);
			}

			scaleNodePositions();
		}

		private void scaleNodePositions()
		{
			//Get the min and max values for x and y
			float minX = 0, maxX = 0, minY = 0;

			for (int i = 0; i < _nodes.Count; i++)
			{
				if (i == 0)
				{
					minX = _nodes[i].Coordinates.X;
					maxX = minX;
					minY = _nodes[i].Coordinates.Y;
				}
				else
				{
					minX = Math.Min(minX, _nodes[i].Coordinates.X);
					maxX = Math.Max(maxX, _nodes[i].Coordinates.X);
					minY = Math.Min(minY, _nodes[i].Coordinates.Y);
				}
			}

			//Determine the scaling factor
			float scalingFactor;
			scalingFactor = (float) (_imageWidth - (float) DIAGRAM_MARGIN*2.0)/(maxX - minX);

			//Scale all of the nodes
			for (int i = 0; i < _nodes.Count; i++)
			{
				double x, y;

				x = _nodes[i].Coordinates.X;
				x -= minX;
				x *= scalingFactor;
				x += DIAGRAM_MARGIN;
				_nodes[i].Coordinates.X = (int) Math.Round(x);

				y = _nodes[i].Coordinates.Y;
				y -= minY;
				y *= scalingFactor;
				y += DIAGRAM_MARGIN;
				_nodes[i].Coordinates.Y = (int) Math.Round(y);
			}
		}

		private Bitmap drawDiagram()
		{
			Bitmap b;
			int maxY = 0;

			//Get the max y coordinate
			for (int i = 0; i < _nodes.Count; i++)
			{
				if (i == 0)
					maxY = _nodes[i].Coordinates.Y;
				else
					maxY = Math.Max(maxY, _nodes[i].Coordinates.Y);
			}

			b = new Bitmap(_imageWidth, maxY + (DIAGRAM_MARGIN*2));
			using (Graphics graphics = Graphics.FromImage(b))
			{
				//Let's make this look nice
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

				Pen blackPen = new Pen(Color.Black);
				SolidBrush whiteBrush = new SolidBrush(Color.White);
				SolidBrush blackBrush = new SolidBrush(Color.Black);
				Font textFont = new Font("Arial", 8, GraphicsUnit.Point);

				//Draw the connections
				for (int i = 0; i < _connections.Count; i++)
				{
					blackPen.Width = (float) _connections[i].ConnectorWidth;
					graphics.DrawLine(blackPen, _connections[i].FirstNode.Coordinates, _connections[i].SecondNode.Coordinates);
				}

				_hotspots = new List<HotSpot>();

				//Draw the rectangles & text
				for (int i = 0; i < _nodes.Count; i++)
				{
					SizeF textSize;
					Rectangle textRect;
					RectangleHotSpot spot;

					textSize = graphics.MeasureString(_nodes[i].Text, textFont);

					textRect = new Rectangle();
					textRect.X = (int) Math.Round(_nodes[i].Coordinates.X - (textSize.Width/2F)) - 1;
					textRect.Y = (int) Math.Round(_nodes[i].Coordinates.Y - (textSize.Height/2F)) - 1;
					textRect.Height = (int) Math.Round(textSize.Height + 2F);
					textRect.Width = (int) Math.Round(textSize.Width + 2F);

					spot = new RectangleHotSpot();
					spot.Left = textRect.X - 1;
					spot.Top = textRect.Y - 1;
					spot.Right = textRect.X + textRect.Width + 1;
					spot.Bottom = textRect.Y + textRect.Height + 1;

					//Pass back the datasource list in the navigate URL.  The caller will have to fill in the entire URL.
					spot.NavigateUrl = getConnectedDatasourceList(_nodes[i]);
					_hotspots.Add(spot);

					graphics.DrawRectangle(blackPen, textRect);

					graphics.FillRectangle(whiteBrush, textRect.X + 1, textRect.Y + 1, textRect.Width - 2, textRect.Height - 2);

					graphics.DrawString(_nodes[i].Text, textFont, blackBrush,
					                    textRect.X + 1, textRect.Y + 1);
				}
			}

			return b;
		}

		private string getConnectedDatasourceList(DiagramNode node)
		{
			DelimitedList list;
			List<DiagramNode> connectedNodes;

			list = new DelimitedList();
			connectedNodes = new List<DiagramNode>();

			//Make sure we include this node
			connectedNodes.Add(node);

			foreach (DiagramNodeConnection currConnection in _connections)
			{
				if (currConnection.FirstNode == node && !connectedNodes.Contains(currConnection.SecondNode))
					connectedNodes.Add(currConnection.SecondNode);
				if (currConnection.SecondNode == node && !connectedNodes.Contains(currConnection.FirstNode))
					connectedNodes.Add(currConnection.FirstNode);
			}

			foreach (DiagramNode currNode in connectedNodes)
			{
				string datasourceString;

				datasourceString = currNode.ConfiguredDatasourceId.ToString();
				if (currNode.SubTypeId != null)
					datasourceString += "." + currNode.SubTypeId.Value.ToString();

				list.Append(datasourceString);
			}

			return list.ToString();
		}

		public static string WrapText(string text, int lineSize)
		{
			StringBuilder output;
			int i = 0;

			output = new StringBuilder();

			while (i < text.Length)
			{
				string chunk;

				chunk = text.Substring(i, Math.Min(lineSize, text.Length - i));
				//if it starts with a space, remove it
				chunk = chunk.TrimStart(new char[] {' '});

				//Check if there are enough characters for a full line
				if (chunk.Length < lineSize)
				{
					//The whole chunk will fit
					output.AppendLine(chunk);
					i += chunk.Length;
				}
				else
				{
					//Check if we can break on a space to make it look nice
					if (chunk.Contains(" "))
					{
						int lastSpaceIndex = chunk.LastIndexOf(' ');

						output.AppendLine(chunk.Substring(0, lastSpaceIndex));
						i += lastSpaceIndex + 1;
					}
					else
					{
						//We have no choice but to split up the text
						output.AppendLine(chunk.Substring(0, lineSize));
						i += lineSize;
					}
				}
			}

			return output.ToString();
		}

		private static string splitText(string text, int lineSize)
		{
			StringBuilder output;

			output = new StringBuilder();

			for (int i = 0; i < text.Length; i += lineSize)
			{
				if (lineSize > text.Length - i)
					output.Append(text.Substring(i, text.Length - i));
				else
					output.AppendLine(text.Substring(i, lineSize));
			}

			return output.ToString();
		}

		/// <summary>
		///		Uses a list of connections and separates them into groups of connections.
		///		Each group has no connections to any other group.
		/// </summary>
		/// <returns></returns>
		public static List<List<DiagramNodeConnection>> GetConnectionGroups(List<DiagramNodeConnection> connections)
		{
			List<DiagramNodeConnection> connectionBucket;
			List<List<DiagramNodeConnection>> groups;

			groups = new List<List<DiagramNodeConnection>>();

			connectionBucket = new List<DiagramNodeConnection>(connections);

			//Loop through the connections, and turn them into separate groups
			while (connectionBucket.Count > 0)
			{
				List<DiagramNodeConnection> currNodeGroup;

				currNodeGroup = new List<DiagramNodeConnection>();
				//Just add a single connection to start the group
				currNodeGroup.Add(connectionBucket[0]);
				connectionBucket.Remove(connectionBucket[0]);

				//Process every item in the group
				for (int i = 0; i < currNodeGroup.Count; i++)
				{
					//For the current item, find any items linked to it
					for (int j = connectionBucket.Count - 1; j >= 0; j--)
					{
						if (connectionBucket[j].SharesNode(currNodeGroup[i]))
						{
							//Move the node from the bucket to the current group
							currNodeGroup.Add(connectionBucket[j]);
							connectionBucket.Remove(connectionBucket[j]);
						}
					}
				}

				groups.Add(currNodeGroup);
			}

			return groups;
		}
	}
}