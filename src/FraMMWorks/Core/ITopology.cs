using System;
using System.Collections.Generic;
using System.Text;

using FraMMWorks.Interfaces;

namespace FraMMWorks.Core
{
   /// <summary>
   /// a generic FraMMWorks Topology definition
   /// </summary>
   public interface ITopology
   {
      /// <summary>
      /// Get all plugins which are active on the topology
      /// (but no necessarily connected to anything)
      /// </summary>
      List<IPlugin> GetActivePlugins();
   }
}
