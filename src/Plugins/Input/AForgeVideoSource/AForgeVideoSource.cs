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
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

using FraMMWorks.Core;
using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;

using AForge.Video;
using AForge.Video.VFW;
using AForge.Video.DirectShow;

namespace AForgeVideoSource
{
   /// <remarks>
   /// Usees the AForge video input classes as a video source plugin. This extends
   /// ControllableSource, but it isnt really controlable. This is simply
   /// for testing initially. Change it to a standard Source later.
   /// </remarks>
   public class AForgeVideoSource : Source
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
      /// What type of output does this class handle
      /// </summary>
      public override Type[] OutputCapabilities
      {
         get { return outputCapabilities; }
      }

      //------------------------- private data memebers------------------------
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      private String name = "AForge Video Source";

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      private String description = "Obtain Video from a file, MJPEG/JPEG stream, capture device or directshow " +
                   "through the AForge libraries http://code.google.com/p/aforge/. " +
                   "The source type and source location must be specified.";

      /// <summary>
      /// What type of output does this class handle
      /// </summary>
      private Type[] outputCapabilities = new Type[] { typeof(FraMMWorksImageFrame) };

      /// <summary>
      /// The settings list for this plugin.
      /// </summary>
      private List<Setting> settings;

      /// <summary>
      /// The actual AForge source object;
      /// </summary>
      AForge.Video.IVideoSource source;

      /// <summary>
      /// The next frame to be read by getFrame()
      /// </summary>
      IFrame nextFrame;

      /// <summary>
      /// Signals when a frame is ready to be read.
      /// </summary>
      private AutoResetEvent frameReadyEvent;

      /// <summary>
      /// Signals when a completed frame has been taken
      /// </summary>
      private AutoResetEvent frameTakenEvent;

      /// <summary>
      /// If this is not null, throw an exception in getFrame
      /// </summary>
      private String lastVideoSourceError;



      //------------------------- Constructors ---------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public AForgeVideoSource()
      {
         // Set up the default settings for this plugin
         initSettings();

         source = null;
         lastVideoSourceError = null;

         frameReadyEvent = new AutoResetEvent(false);
         frameTakenEvent = new AutoResetEvent(true);
      }

      //------------------------- public access memebers------------------------

      /// <summary>
      /// The setings for this plugin
      /// setting 0: String   - URL/filename of input file
      /// setting 1: ComboBox - The source type setting
      /// </summary>
      /// <returns>The <see cref="Setting"/>s for this plugin
      /// </returns>
      public override List<Setting> getSettings()
      {
         return settings;
      }

      /// <summary>
      /// Gets the next frame from the AForge library. On the is the first time
      /// the method is called, open the source and start grabbing.
      /// </summary>
      /// <return>the frame just processed/obtained.</return>
      /// <param name="pin">the "pin" we want to get a frame from - 
      /// corresponds to a position in the OutputCapabilities array
      /// </param>
      public override IFrame getFrame(int pin)
      {
         // Check for errors
         if (lastVideoSourceError != null)
         {
            string temp = lastVideoSourceError;
            lastVideoSourceError = null;
            throw new Exception(temp);
         }

         // If the source hasn't been started, throw an error
         if (source == null)
            throw new ProcessingException("Source hasn't been started. call start() before readig a frame.");

         // check if there is a frame ready
         frameReadyEvent.WaitOne();

         // If we get here, there must be a frame ready. But make sure it isn't some error
         if (nextFrame == null)
            throw new ProcessingException("AForgeVideoSource error. Expected a frame but it was null", true);

         // save a reference so another thread can re-use the completedFrame variable
         IFrame tempFrame = nextFrame;
         nextFrame = null;

         // increment the current frame number
         currentFrameNum++;

         // Signal that the class is ready to process another frame
         frameTakenEvent.Set();

         return tempFrame;
      }

      /// <summary>
      /// Start the source.
      /// </summary>
      public override void start()
      {
         if (source != null)
            stop();

         openSource();
      }

      /// <summary>
      /// Stop the source
      /// </summary>
      public override void stop()
      {
         if (source != null)
         {
            source.SignalToStop();
            source.WaitForStop();
            source = null;
         }
      }


      //------------------------- private processing memebers------------------------
      private void initSettings()
      {
         settings = new List<Setting>();

         // setting 0: URL/filename of input file
         Setting setting = new Setting();
         setting.name = "Input filename/URL";   // The display name of this setting
         setting.minValue = 0;                  // Min length of field
         setting.maxValue = 1024;               // Max length of field 
         setting.type = typeof(String);         // It's a string setting
         setting.value = "";                    // default value is empty string
         setting.isDirty = false;
         settings.Add(setting);

         // setting 1: The source type setting - a combo box
         setting = new Setting();
         setting.name = "Input Type";
         setting.minValue = 0;
         setting.maxValue = 1;
         setting.type = typeof(SourceSelectionSetting);
         setting.value = SourceSelectionSetting.DefaultItems[0];
         setting.isDirty = false;
         settings.Add(setting);
      }

      /// <summary>
      /// Opens the source as indicated in the settings.
      /// </summary>
      private void openSource()
      {
         // make sure everything is stopped
         stop();

         // Get the settings
         if (!(settings[0].value is String))
            throw new Exception("AForgeVidoeSource internal error. The \"Input filename/URL\" setting wasn't as expected");
         String inputURL = settings[0].value as String;

         if (!(settings[1].value is String))
            throw new Exception("AForgeVidoeSource internal error. The \"Input Type\" setting wasn't as expected");
         String inputType = settings[1].value as String;

         // Work out what type of source it is
         switch (inputType)
         {
            case "File":
               source = new AVIFileVideoSource(inputURL);
               break;
            case "JPEG URL":
               source = new JPEGStream(inputURL);
               break;
            case "MJPEG URL":
               source = new MJPEGStream(inputURL);
               break;
            case "Local video capture device":
               source = new VideoCaptureDevice(inputURL);
               break;
            case "Open video file (using direct show)":
               source = new FileVideoSource(inputURL);
               break;
            default:
               throw new Exception("AForgeVidoeSource internal error. Didn't recognise the \"Input Type\" value of " + inputType);
         }

         // Set it all up
         source.NewFrame += new NewFrameEventHandler(aforgeNewFrame_cb);
         source.VideoSourceError += new VideoSourceErrorEventHandler(aforgeError_cb);

         // set some variables about this source
         currentFrameNum = 0;   // start at the beginning

         // Now actually start grabbing
         source.Start();

      }

      /// <summary>
      /// Process a new frame. We can block in here which will cause the source
      /// to either wait for the next frame(in the case of a file) or drop
      /// frames (in the case of a live stream).
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void aforgeNewFrame_cb(object sender, NewFrameEventArgs e)
      {
         try
         {
            // If there's still an old frame, wait until it is taken
            // before adding this one
            frameTakenEvent.WaitOne();

            // Make sure there wasn't some sort of problem reading the frame
            // if so, just delete it.
            if (nextFrame != null)
               nextFrame = null;

            // reset error
            lastVideoSourceError = null;

            // get new frame
            FraMMWorksImageFrame frame = new FraMMWorksImageFrame();
            frame.Image = (Bitmap)e.Frame.Clone(); //TODO: do we really need to clone this???
            frame.FrameNum = currentFrameNum;
            nextFrame = frame;
         }
         catch (Exception ex)
         {
            if (lastVideoSourceError != null)
               lastVideoSourceError = "Error in aforge video callback: "+lastVideoSourceError;
            else
               lastVideoSourceError = "Error in aforge video callback: "+ex.Message;
         }
         finally
         {
            frameReadyEvent.Set();
         }

      }

      /// <summary>
      /// notify app of some sort of AForge error
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void aforgeError_cb(object sender, VideoSourceErrorEventArgs e)
      {
         // save video source error's description
         if (e.Description.Contains("not load file or assembly"))
         {
            ControlAPI.Instance.fatalError(this, e.Description);
         }
         else
         {
            ControlAPI.Instance.nonFatalError(this, e.Description);
         }
         lastVideoSourceError = e.Description;
      }

   }
}
