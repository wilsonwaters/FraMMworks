using System;
using System.Collections.Generic;
using System.Text;

using FraMMWorks.Core;

using NUnit.Framework;

namespace NUnitTests.Core
{
   [TestFixture]
   public class PluginManager
   {
      public PluginManager()
      {
      }

      [Test]
      public void scanForPlugins()
      {
         // get the manager
         FraMMWorks.Core.PluginManager pm = FraMMWorks.Core.PluginManager.Instance;

         Assert.GreaterOrEqual(pm.AvailablePlugins.Length, 4, "Couldn't find all expeced plugins");
      }

      [Test]
      public void getNewPluginInstance()
      {
         // get the manager
         FraMMWorks.Core.PluginManager pm = FraMMWorks.Core.PluginManager.Instance;

         foreach (FraMMWorks.Core.PluginManager.PluginInfo info in pm.AvailablePlugins)
         {

            FraMMWorks.Interfaces.IPlugin p = pm.getNewPluginInstance(info);

            // make sure it's not null
            Assert.IsNotNull(p, "returned a null plugin");

            bool same = true;

            // Check the basic fields
            if (!(info.type == p.GetType() && info.name.Equals(p.Name) && info.description.Equals(p.Description)))
               same = false;

            // make sure the input capabilities are exactly the same
            if (typeof(FraMMWorks.Interfaces.ISink).IsAssignableFrom(info.type))
            {
               FraMMWorks.Interfaces.ISink ip = (FraMMWorks.Interfaces.ISink)p;
               for (int i = 0; i < info.inputCapabilities.Length; i++)
                  if (info.inputCapabilities[i] != ip.InputCapabilities[i])
                     same = false;
            }

            // make sure the output capabilities are exactly the same
            if (typeof(FraMMWorks.Interfaces.ISource).IsAssignableFrom(info.type))
            {
               FraMMWorks.Interfaces.ISource op = (FraMMWorks.Interfaces.ISource)p;
               for (int i = 0; i < info.outputCapabilities.Length; i++)
                  if (info.outputCapabilities[i] != op.OutputCapabilities[i])
                     same = false;
            }

            // If we made it here, they are "the same"
            Assert.That(same, "requested plugin was different from returned plugin for "+p.Name);
         }
      }
   }
}
