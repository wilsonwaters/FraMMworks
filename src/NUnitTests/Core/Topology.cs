using System;
using System.Collections.Generic;
using System.Text;

using FraMMWorks.Core;

using NUnit.Framework;

namespace NUnitTests.Core
{
   [TestFixture]
   public class Topology
   {
      const String TEST_TOPOLOGY_FILE = @"..\..\testData\testTopology.xml";

      public Topology()
      {
      }

      [SetUp]
      public void InitTest()
      {
         FraMMWorks.Core.ControlAPI.Instance.OnDebugMessage += new ControlAPI.DebugMessageHandler(message);
         FraMMWorks.Core.ControlAPI.Instance.OnErrorMessage += new ControlAPI.ErrorMessageHandler(message);
      }

      [Ignore, Test]
      public void save()
      {
      }

      [Test]
      public void load()
      {
         FraMMWorks.Core.Topology t = new FraMMWorks.Core.Topology();
         t.load(TEST_TOPOLOGY_FILE);

         Assert.IsNotNull(t.TopologyGraph, "topology graph was null");

         Assert.AreEqual(4, t.TopologyGraph.Count, "There was a different number of edges(links) in the topology then expected");
      }

      public void message(String message)
      {
         System.Console.WriteLine(message);
      }

   }
}