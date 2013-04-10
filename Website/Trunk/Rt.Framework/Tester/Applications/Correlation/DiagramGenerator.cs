using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NUnit.Framework;

namespace Rt.Framework.Applications.Correlation
{
	[TestFixture]
	public class DiagramGenerator_Tester
	{
		DiagramGenerator gen;

		[Test]
		public void Test()
		{
			List<DiagramNode> nodes;
			List<DiagramNodeConnection> connections;
			DiagramNode newNode;
			DiagramNodeConnection newConnection;

			nodes = new List<DiagramNode>();

			newNode = new DiagramNode();
			newNode.Text = "Node A";
			nodes.Add(newNode);

			newNode = new DiagramNode();
			newNode.Text = "Node B";
			nodes.Add(newNode);

			newNode = new DiagramNode();
			newNode.Text = "Node C";
			nodes.Add(newNode);

			connections = new List<DiagramNodeConnection>();

			//Connect A and B
			newConnection = new DiagramNodeConnection();
			newConnection.FirstNode = nodes[0];
			newConnection.SecondNode = nodes[1];
			connections.Add(newConnection);

			//Connect B and C
			newConnection = new DiagramNodeConnection();
			newConnection.FirstNode = nodes[1];
			newConnection.SecondNode = nodes[2];
			connections.Add(newConnection);

			//Connect A and C
			newConnection = new DiagramNodeConnection();
			newConnection.FirstNode = nodes[0];
			newConnection.SecondNode = nodes[2];
			connections.Add(newConnection);

			gen = new DiagramGenerator(nodes, connections, 500);
			Bitmap b;
			b = gen.Generate();
			
			//We can't really look at the image, so just make sure we got one without a crash
			Assert.IsTrue(b != null);
		}

		[Test]
		public void WrapText()
		{
			string s = "this is a test";
			string o;

			o = DiagramGenerator.WrapText(s, 5);
			Assert.AreEqual("this\r\nis a\r\ntest\r\n", o);
		}

		[Test]
		public void WrapText2()
		{
			string s = "this is a test";
			string o;

			o = DiagramGenerator.WrapText(s, 4);
			Assert.AreEqual("this\r\nis \r\na t\r\ntest\r\n", o);
		}

		[Test]
		public void WrapText3()
		{
			string s = "this is a test";
			string o;

			o = DiagramGenerator.WrapText(s, 3);
			Assert.AreEqual("thi\r\ns\r\nis\r\na\r\ntes\r\nt\r\n", o);
		}

		[Test]
		public void GetConnectionGroups()
		{
			List<List<DiagramNodeConnection>> groups;
			List<DiagramNodeConnection> connections;
			DiagramNode nodeA = new DiagramNode(), nodeB = new DiagramNode(), nodeC = new DiagramNode(), nodeD = new DiagramNode(), nodeE = new DiagramNode();
			DiagramNodeConnection connA, connB, connC;

			connections = new List<DiagramNodeConnection>();
			//This is all one group
			connA = new DiagramNodeConnection(nodeA, nodeB);
			connB = new DiagramNodeConnection(nodeB, nodeC);
			connections.Add(connA);
			connections.Add(connB);
			//Another unconnected group
			connC = new DiagramNodeConnection(nodeD, nodeE);
			connections.Add(connC);

			groups = DiagramGenerator.GetConnectionGroups(connections);

			Assert.AreEqual(2, groups.Count);

			Assert.AreEqual(2, groups[0].Count);
			Assert.AreEqual(connA, groups[0][0]);
			Assert.AreEqual(connB, groups[0][1]);

			Assert.AreEqual(1, groups[1].Count);
			Assert.AreEqual(connC, groups[1][0]);
		}
	}
}
