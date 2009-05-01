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
//using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using FraMMWorks.Core;
using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;

namespace ImageFileSource
{
   /// <remarks>
   /// Reads all images from a given directory in alphanumerical order
   /// </remarks>
   public class ImageFileSource : ControllableSource
   {
      //------------------------- constants -----------------------------------
      /// <summary>
      /// Gives an indication of the average number of images a directory may 
      /// contain.
      /// </summary>
      private const int DEFAULT_CAPACITY = 1000;

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
      private String name = "Image File Source";

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      private String description = "Scans a directory for all image files and sequentially outputs them " +
                                   "as a video sequence. The file listing is alphanumerically ordered";

      /// <summary>
      /// What type of output does this class handle
      /// </summary>
      private Type[] outputCapabilities = new Type[] { typeof(FraMMWorksImageFrame) };

      /// <summary>
      /// The settings list for this plugin.
      /// </summary>
      private List<Setting> settings;

      /// <summary>
      /// An oedered listing of file names in the directory of interest
      /// (or just one image)
      /// </summary>
      private List<String> imageFiles;

      /// <summary>
      /// The actual images. These corrospond to the imageFile listing at all times.
      /// </summary>
      private List<FraMMWorksImageFrame> images;

      /// <summary>
      /// flag to indicate the imageFiles and images list needs to be rebuilt.
      /// </summary>
      private bool isListingDirty;


      //------------------------- Constructors ---------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public ImageFileSource()
      {
         // Set up the default settings for this plugin
         initSettings();

         imageFiles = new List<String>(DEFAULT_CAPACITY);
         images = new List<FraMMWorksImageFrame>(DEFAULT_CAPACITY);
         isListingDirty = true;
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
         // make sure the pin is correct - we can only handle one connection
         if (pin > 0)
            throw new ProcessingException("Image File Source error: can't handle more than one output. You can only connect to pin 0.", false);

         // check if the filename/directory setitng has changed or the precached setting.
         if (settings[0].isDirty || settings[2].isDirty)
            isListingDirty = true;

         // check if the framerate setting has changed
         if (settings[1].isDirty)
            sourceFrameRate = (int)settings[1].value;

         // check if we need to rebuild the file list
         if (isListingDirty)
            rebuildFileList();

         // Need to lock, otherwise currentFrameNum may be changed between operations.
         lock (this)
         {
            // make sure we're not at the end of the source now that we've locked currentFrameNum
            if (currentFrameNum >= numFrames)
            {
               return null;
            }  

            // if the image hasn't been loaded, do it now
            if (images[(int)currentFrameNum] == null)
            {
               //load image
               Bitmap b = null;
               try
               {
                  b = new Bitmap(imageFiles[(int)currentFrameNum]);
               }
               catch (Exception ex)
               {
                  // ignore this frame, and move to the next
                  currentFrameNum++;
                  throw new ProcessingException("Couldn't load an image file named: "+imageFiles[(int)currentFrameNum]+". Error: "+ex.Message,true);
               }
               FraMMWorksImageFrame f = new FraMMWorksImageFrame(b);
               f.FrameNum = currentFrameNum;
               images.Insert((int)currentFrameNum, f);
            }

            // This is a bit inefficent, but is needed for seeking/reuse.
            // The frame you pass out here will actually be modified.
            return (IFrame)images[(int)currentFrameNum++].Clone();
         }
      }

      /// <summary>
      /// Start the source.
      /// </summary>
      public override void start()
      {
         // just reset the currnet frame number
         currentFrameNum = 0;
      }

      /// <summary>
      /// Stop the source
      /// </summary>
      public override void stop()
      {
         // just reset the currnet frame number
         currentFrameNum = 0;
      }

      /// <summary>
      /// Move to a new position in the source represented by the number of
      /// frame from the beginning of the source.
      /// </summary>
      public override void seek(uint frameNum)
      {
         // need to lock, otherwse value may change in the middle of getFrame() operations.
         lock (this)
         {
            // set the currnet frame number
            currentFrameNum = frameNum;
         }
      }


      //------------------------- private processing memebers------------------------

      private void initSettings()
      {
         settings = new List<Setting>();

         // setting 0: image filename or directory of images
         Setting setting = new Setting();
         setting.name = "Input filename/directory";   // The display name of this setting
         setting.minValue = 0;                  // Min length of field
         setting.maxValue = 1024;               // Max length of field 
         setting.type = typeof(String);         // It's a string setting
         setting.value = ".";                  // default value is current directory
         setting.isDirty = false;
         settings.Add(setting);

         // setting 1: the frame rate to display these images at
         setting = new Setting();
         setting.name = "Source framerate";
         setting.minValue = 0;
         setting.maxValue = 256;
         setting.type = typeof(Int32);
         setting.value = 25;
         setting.isDirty = false;
         settings.Add(setting);

         // setting 1: the frame rate to display these images at
         setting = new Setting();
         setting.name = "Precache images";
         setting.minValue = 0;
         setting.maxValue = 1;
         setting.type = typeof(Boolean);
         setting.value = false;
         setting.isDirty = false;
         settings.Add(setting);
      }

      /// <summary>
      /// Scans the current directory for files and re-creates the ordered filename
      /// list. If the setting refers to a single image, then just place that
      /// in the file list
      /// </summary>
      private void rebuildFileList()
      {
         // Lock to handle multiple threads trying to rebuild he list at the same
         // time. This is also re-entrant safe.
         lock (this)
         {
            // if the file list is actually clean, just exit
            if (!isListingDirty)
               return;

            imageFiles.Clear();
            imageFiles.AddRange(Directory.GetFiles((String)settings[0].value));
            imageFiles.Sort();

            // clear the existing images list and fill it with nulls
            images.Clear();
            images.Capacity = imageFiles.Count;
            for (int i = 0; i < imageFiles.Count; i++)
            {
               images.Insert(i, null);
            }

            // set some values
            this.numFrames = (uint)imageFiles.Count;

            // do we want to pre-cache all the images?
            if ((bool)settings[2].value)
            {
               int i = 0;
               foreach (String imageFile in imageFiles)
               {
                  try
                  {
                     Bitmap b = new Bitmap(imageFile);
                     FraMMWorksImageFrame frame = new FraMMWorksImageFrame(b);
                     images.Insert(i++, frame);
                  }
                  catch (Exception ex)
                  {
                     ControlAPI.Instance.nonFatalError(this, "Couldn't load an image file named: {0}. Error: {1}", imageFile, ex.Message);
                  }
               }
            }

            isListingDirty = false;
         }
      }

   }
}