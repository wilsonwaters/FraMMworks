using System;
using System.Collections.Generic;
using System.Text;
using FraMMWorks.PluginBase;

namespace FraMMWorks.Interfaces 
{
   /// <remarks>
   /// Base interface for all plugins.
   /// </remarks>
   public interface IPlugin
   {
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      String Name { get;}

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      String Description { get;}

      /// <summary>
      /// Provides a <see cref="System.Windows.Forms.Control"/> object which displays
      /// information about this frame sink on the GUI.
      /// This is optional and may return null if not required.
      /// </summary>
      /// <returns></returns>
      System.Windows.Forms.Control getDisplayControl();

      /// <summary>
      /// Provides a list of settings applicable to this implementation. The returned
      /// types must be <see cref="PluginBase.Setting"/> objects which provide a direct reference
      /// of the value to be changed.
      /// Settings may be basic types (Int32, Double, String, etc) or more complex types
      /// (such as <I>VideoFrameSelector</I>) which must implement <see cref="System.Windows.Forms.Control"/> 
      /// to describe how to draw itself.
      /// </summary>
      /// <returns>The <see cref="PluginBase.Setting"/>s which may be modified for this implementation</returns>
      List<Setting> getSettings();
   }
}
