using System;
using System.Collections.Generic;
using System.Text;

using FraMMWorks.Core;

using NUnit.Framework;

namespace NUnitTests.Core
{
   [TestFixture]
   public class ProcessingPipeline
   {
      const String TEST_TOPOLOGY_FILE = @"..\..\testData\testTopology.xml";

      public ProcessingPipeline()
      {
         //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
      }

      [Test]
      public void updateTopology()
      {
         FraMMWorks.Core.Topology topology = new FraMMWorks.Core.Topology();
         topology.load(TEST_TOPOLOGY_FILE);
         FraMMWorks.Core.ProcessingPipeline pipeline = new FraMMWorks.Core.ProcessingPipeline();
         pipeline.updateTopology(topology);
      }

      [Test]
      public void pause()
      {
         FraMMWorks.Core.ProcessingPipeline p = new FraMMWorks.Core.ProcessingPipeline();
         p.pause();
      }

      [Test]
      public void unpause()
      {
         FraMMWorks.Core.ProcessingPipeline p = new FraMMWorks.Core.ProcessingPipeline();
         p.unpause();
      }

   }
}