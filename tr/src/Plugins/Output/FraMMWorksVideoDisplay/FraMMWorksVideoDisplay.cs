/*
    FraMMWorks - Multimedia Processing and Diagnostic Framework.
    Copyright (C) 2008  Wilson Waters

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;
using FraMMWorks.Core;

namespace FraMMWorksVideoDisplay
{
   /// <remarks>
   /// A basic video display class for FraMMWorks. Inputs a single chanel of
   /// video and displays it to a window in the main GUI.
   /// </remarks>
   public class FraMMWorksVideoDisplay : Sink
   {
      //------------------------- public properties ---------------------------
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      public override String Name
      {
         get { return name; }
      }

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      public override String Description
      {
         get { return description; }
      }

      /// <summary>
      /// What type of input can this class handle
      /// </summary>
      public override Type[] InputCapabilities
      {
         get { return inputCapabilities; }
      }

      //------------------------- private data memebers------------------------
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      private String name = "FraMMWorks GUI Video Display";

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      private String description = "Display a video stream directly to the FraMMWorks GUI window.";

      /// <summary>
      /// What type of output does this class handle
      /// </summary>
      private Type[] inputCapabilities = new Type[] { typeof(FraMMWorksImageFrame) };

      /// <summary>
      /// The control that is drawn to the screen
      /// </summary>
      private Control displayArea;

      /// <summary>
      /// The current size of the video frames
      /// </summary>
      private Size size;

      /// <summary>
      /// deligate the displayArea.Image modfication call to the GUI thread.
      /// </summary>
      /// <param name="image"></param>
      private delegate void OnImageChangeDelegate(Bitmap image);



      //------------------------- Constructors ---------------------------------
      public FraMMWorksVideoDisplay()
      {
         size = new Size(384, 288); // default size

         PictureBox displayAreaImage = new PictureBox();
         displayAreaImage.Image = new Bitmap(384, 288);
         displayAreaImage.ClientSize = size;

         GroupBox box = new GroupBox();
         box.FlatStyle = FlatStyle.Standard;
         displayAreaImage.Location = new Point(5, 20);
         box.Controls.Add(displayAreaImage);
         box.AutoSize = true;
         box.Text = this.name;

         displayArea = box;

         Graphics g = Graphics.FromImage(displayAreaImage.Image);
         g.DrawLine(new Pen(Brushes.Red), 0, 0, 384, 288);
         g.DrawLine(new Pen(Brushes.Red), 384, 0, 0, 288);
         g.Dispose();
      }

      //------------------------- public access memebers------------------------

      /// <summary>
      /// Display a frame to the screen.
      /// </summary>
      /// <param name="frame">the input frames to display</param>
      /// <param name="pin">the "pin" we want to send a frame to - 
      /// corresponds to a position in the InputCapabilities array.
      /// </param>
      public override void sendFrame(IFrame frame, int pin)
      {
         // make sure the pin is correct - we can only handle one connection
         if (pin > 0)
            throw new ProcessingException("FraMMWorks Video Display error: can't handle more than one input. You can only connect to pin 0.", false);

         // Make sure the input frame is in expected format
         if ((frame == null) || !(frame is FraMMWorksImageFrame))
            throw new ApplicationException("FraMMWorksVideoDisplay input image was null or in an incorrect format");

         // display it.
         FraMMWorksImageFrame f = (FraMMWorksImageFrame)frame;
         if (f.Width != size.Width || f.Height != size.Height)
         {
            size.Width = f.Width;
            size.Height = f.Height;
         }
         updateDisplay(f.Image);

      }

      /// <summary>
      /// The control which should be drawn the the main GUI to display this
      /// video display.
      /// </summary>
      /// <return>a control object which draws each video frame to the screen
      /// </return>
      public override System.Windows.Forms.Control getDisplayControl()
      {
         return displayArea;
      }

      private void updateDisplay(Image image)
      {
         // make sure the control is actually being drawn (i.e. it has a parent)
         if (this.displayArea.Parent == null)
            return;

         // need to marshal this call back to the UI thread, as it id called from a worker thread
         if (this.displayArea.InvokeRequired)
         {
            displayArea.Parent.BeginInvoke(new OnImageChangeDelegate(updateDisplay), new object[] { image });
         }
         else
         {
            PictureBox box = this.displayArea.Controls[0] as PictureBox;
            box.Image = image;
            box.ClientSize = size;
            box.Invalidate();
         }
      }
   }
}
