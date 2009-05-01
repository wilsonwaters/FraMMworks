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
using System.IO;

using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;
using FraMMWorks.Core;

namespace FilmstripNavigation
{
   /// <remarks>
   /// Allows navigation in FraMMWorks through a "Filestrip" style frame display.
   /// Important frames are extracted from the video and displayed on the screen,
   /// which can be clicked to navegate to that point.
   /// 
   /// At the moment "important frames" are simply read from a file.
   /// 
   /// Wilson Waters 20080224
   /// </remarks>
   public class FilmstripNavigation : Sink
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
      private String name = "Filmstrip Navigation";

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      private String description = "Navigate a video source through a filmstrip style display.";

      /// <summary>
      /// What type of output does this class handle
      /// </summary>
      private Type[] inputCapabilities = new Type[] { typeof(FraMMWorksImageFrame) };

      /// <summary>
      /// The settings list for this plugin.
      /// </summary>
      private List<Setting> settings;

      /// <summary>
      /// The control that is drawn to the screen
      /// </summary>
      private FilmstripControl control;

      /// <summary>
      /// The current size of the video frames
      /// </summary>
      private Size frameSize;

      /// <summary>
      /// deligate the displayArea.Items modfication call to the GUI thread.
      /// </summary>
      /// <param name="image"></param>
      private delegate void OnSetDisplayItemsDelegate(FilmstripControl.FrameInfo [] frames);



      //------------------------- Constructors ---------------------------------
      public FilmstripNavigation()
      {
         initSettings();

         frameSize = new Size(384, 288); // default size

         control = new FilmstripControl();
         control.ImageHeight = (int)settings[1].value;
         control.AllowSelection = false;

         // be notified on a frame click in the control
         control.FrameClicked += new FilmstripControl.FrameClickedHandler(control_FrameClicked);

         updateSettings();
      }

      //------------------------- public access memebers------------------------
      /// <summary>
      /// The setings for this plugin
      /// setting 0: String   - Frame list file
      /// setting 1: int      - Filmstrip image height
      /// </summary>
      /// <returns>The <see cref="Setting"/>s for this plugin
      /// </returns>
      public override List<Setting> getSettings()
      {
         return settings;
      }

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

         // check if we need to re-read the input file
         if (settings[0].isDirty || settings[1].isDirty)
            updateSettings();

         // display it.
         FraMMWorksImageFrame f = (FraMMWorksImageFrame)frame;
         if (f.Width != frameSize.Width || f.Height != frameSize.Height)
         {
            frameSize.Width = f.Width;
            frameSize.Height = f.Height;
         }
         //updateDisplay(f.Image);

      }

      /// <summary>
      /// The control which should be drawn the the main GUI to display this
      /// video display.
      /// </summary>
      /// <return>a control object which draws each video frame to the screen
      /// </return>
      public override System.Windows.Forms.Control getDisplayControl()
      {
         return control;
      }

      private void setDisplayItems(FilmstripControl.FrameInfo [] frames)
      {
         // make sure the control is actually being drawn (i.e. it has a parent)
         if (this.control.Parent == null)
            return;

         // need to marshal this call back to the UI thread, as it id called from a worker thread
         if (this.control.InvokeRequired)
         {
            control.Parent.BeginInvoke(new OnSetDisplayItemsDelegate(setDisplayItems), new object[] { frames });
         }
         else
         {
            this.control.Items.Clear();
            this.control.Items.AddRange(frames);
            this.control.Invalidate();
         }
      }

      private void initSettings()
      {
         settings = new List<Setting>();

         // setting 0: frfame list filename
         Setting setting = new Setting();
         setting.name = "Frame list file";   // The display name of this setting
         setting.minValue = 0;                  // Min length of field
         setting.maxValue = 1024;               // Max length of field 
         setting.type = typeof(String);         // It's a string setting
         setting.value = "";                    // default value is empty string
         setting.isDirty = true;
         settings.Add(setting);

         // setting 1: the height to display images in the filmstrip (will
         // also determine the length of the strip)
         setting = new Setting();
         setting.name = "Filmstrip image height";
         setting.minValue = 0;
         setting.maxValue = 2048;
         setting.type = typeof(Int32);
         setting.value = 100;
         setting.isDirty = true;
         settings.Add(setting);

      }

      private void updateSettings()
      {
         if (settings[0].isDirty)
         {
            lock (settings[0])
            {
               readFrameFile((string)settings[0].value);
               settings[0].isDirty = false;
            }
         }
         if (settings[1].isDirty)
         {
            lock (settings[1])
            {
               control.ImageHeight = (int)settings[1].value;
            }
         }
      }

      /// <summary>
      /// Reads the specified frame file and updates the control with
      /// these items.
      /// Frame files are in the format of
      /// sceneStartFrame sceneEndFrame
      /// where each line corresponds to a paricular scene.
      /// i.e.
      /// 1 5 c:\images\1.jpg
      /// 7 8 c:\images\7.jpg
      /// 105 160 c:\images\ble bla bloot.jpg
      /// 12345 421213 c:\images\12345.jpg
      /// </summary>
      /// <param name="filename"></param>
      private void readFrameFile(string filename)
      {
         List<FilmstripControl.FrameInfo> frames = new List<FilmstripControl.FrameInfo>();
         TextReader file;
         try
         {
             file = new StreamReader(filename);
         }
         catch (Exception ex)
         {
            ControlAPI.Instance.debugMessage(this, "error opening input file: "+filename + ". "+ex.Message);
            return;
         }

         String line = null;
         int lineNum = 1;
         try
         {
            while ((line = file.ReadLine()) != null)
            {
               string[] parts = line.Split(' ');
               FilmstripControl.FrameInfo frame = new FilmstripControl.FrameInfo();
               frame.startFrame = UInt32.Parse(parts[0]);
               frame.endFrame = UInt32.Parse(parts[1]);
               frame.selected = false;

               // load cached image
               if ((parts[0].Length + parts[1].Length + 2) < line.Length)
               {
                  string cachedImageFile = "";
                  try
                  {
                     cachedImageFile = line.Substring(parts[0].Length + parts[1].Length + 2, line.Length - parts[0].Length - parts[1].Length - 2);
                     frame.image = new Bitmap(cachedImageFile);
                  }
                  catch (Exception ex)
                  {
                     ControlAPI.Instance.nonFatalError(this, "error loading cached image file " + cachedImageFile + ". Error: " + ex.Message);
                     frame.image = null;
                  }
               }
               else
               {
                  frame.image = null;
               }

               frames.Add(frame);
               lineNum++;
            }
         }
         catch (Exception ex)
         {
            ControlAPI.Instance.nonFatalError(this, "error in input file on line " + lineNum + ". line: " + line + ". file: " + filename + ". Error: " + ex.Message);
         }
         file.Close();

         FilmstripControl.FrameInfo[] frameArray = new FilmstripControl.FrameInfo[frames.Count];
         frames.CopyTo(frameArray);
         setDisplayItems(frameArray);
      }

      /// <summary>
      /// Saves a cached image to represent the scene. Modifies the frame file
      /// to point to the cached image and writes the file.
      /// </summary>
      /// <param name="cacheImage"></param>
      /// <param name="sceneNum"></param>
      private void writeCachedImage(Image sceneImage, int sceneNum)
      {
         //TODO: implement
         return;
      }

      /// <summary>
      /// Handle a fram clicked even by seeking to that frame
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="frame"></param>
      void control_FrameClicked(object sender, FilmstripControl.FrameInfo frame)
      {
         ControlAPI.Instance.seek(frame.startFrame);
      }
   }
}
