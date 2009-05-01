using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AForgeVideoSource
{
   /// <remarks>
   /// Contains a setting for selecting the source type for the AForgeVideoSource
   /// plugin.
   /// 
   /// This example class shows how to create settings for non-primitave types,
   /// in this case a combo box. As a combo box extents the Control class
   /// it is seen as a setting by the FraMMWorks GUI drawer.
   /// </remarks>
   public class SourceSelectionSetting : ComboBox
   {
      /// <summary>
      /// The selectable items which will be hard coded into this control
      /// </summary>
      private static string [] items = new string []{
            "File",
            "JPEG URL",
            "MJPEG URL",
            "Local video capture device",
            "Open video file (using direct show)"};

      public static String[] DefaultItems
      {
         get { return items; }
      }


      /// <summary>
      /// Pre-set the options in this combo box and sets the default selection
      /// </summary>
      public SourceSelectionSetting()
      {
         this.Items.AddRange(items);
         this.SelectedItem = items[0];
      }

   }
}
