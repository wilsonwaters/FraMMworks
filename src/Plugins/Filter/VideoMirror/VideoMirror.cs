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

using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;

namespace VideoMirror
{
   //------------------------- public properties ---------------------------
   /// <remarks>
   /// A basic filter to flip the video about the vertical or horizontal
   /// axis.
   /// </remarks>
   public class VideoMirror : SimpleProcessingFilter
   {
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
      public override Type Capability
      {
         get { return capability; }
      }


      //------------------------- private data members--------------------------
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      private String name = "Video Mirror";

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      private String description = "Mirror a video signal about some axis. The axis, position and source " +
                                         "side are configurable.";

      /// <summary>
      /// What type of output does this class handle
      /// </summary>
      protected Type capability = typeof(FraMMWorksImageFrame);

      /// <summary>
      /// The settings list for this plugin.
      /// </summary>
      private List<Setting> settings;


      //------------------------- Constructors ---------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public VideoMirror()
      {
         // Set up the default settings for this plugin
         initSettings();
      }

      //------------------------- public access memebers------------------------

      /// <summary>
      /// Process this frame by mirroring about some point.
      /// </summary>
      /// <param name="inputFrame">The frame to operate on</param>
      protected unsafe override void processFrame(IFrame inputFrame)
      {
         // Make sure the frame is as expected
         if (inputFrame == null || !(inputFrame is FraMMWorksImageFrame))
            throw new ApplicationException("Video mirror plugin - was passed an invalid frame");

         // Set some variables for easy use
         Bitmap image = (Bitmap)((FraMMWorksImageFrame)inputFrame).Image;
         int width = image.Width;
         int height = image.Height;
         bool isHorizontal = (bool)settings[0].value;
         double mirrorLocation = (double)settings[1].value;
         bool isBottomRightSource = (bool)settings[2].value;

         BitmapData imageData = null;

         // lock source image
         imageData = image.LockBits(
             new Rectangle(0, 0, width, height),
             ImageLockMode.ReadWrite,
             PixelFormat.Format24bppRgb);

         // get pointers to the image data
         byte* src = (byte*)imageData.Scan0.ToPointer();
         int srcOffset = imageData.Stride - width * 3;
         int lineWidth = (width * 3) + srcOffset;

         // The actual processing.
         if (!isHorizontal)
         {
            int xFlipPoint = (int)(width * mirrorLocation);
            if (!isBottomRightSource)
            {
               // Flip each line about the center
               for (int y = 0; y < height; y++)
               {
                  // Save a reference to this line
                  byte* lineStart = src;
                  // for each pixel
                  for (int x = 0; x < xFlipPoint; x++, src += 3)
                  {
                     // copy this pixel to the destination
                     //memcpy(lineStart + ((width - x - (width-xFlipPoint)) * 3), src, 3);
                     int newLoc = (((xFlipPoint * 2) - x)-1) * 3;
                     if (newLoc <= lineWidth)
                        memcpy(lineStart + newLoc, src, 3);
                  }
                  src += (width - xFlipPoint) * 3; // ignore the rest of this line
                  src += srcOffset;       // add any offset for this line
               }
            }
            else
            {
               for (int y = 0; y < height; y++)
               {
                  byte* lineStart = src;

                  for (int x = 0; x < (width / 2); x++, src += 3)
                     memcpy(src, lineStart + ((width - x) * 3), 3);

                  src += (width / 2) * 3;
                  src += srcOffset;
               }
            }
         }
         else
         {
            // Copy top line to bottom, second line to bottom-1... etc...
            if (!isBottomRightSource)
               for (int y = 0; y < (height / 2); y++)
                  memcpy(src + (lineWidth * (height - y)), src + (lineWidth * y), lineWidth);
            else
               for (int y = 0; y < (height / 2); y++)
                  memcpy(src + (lineWidth * y), src + (lineWidth * (height - y)), lineWidth);
         }

         // unlock source image
         image.UnlockBits(imageData);
      }

      /// <summary>
      /// The setings for this plugin
      /// setting 0: bool     - whether the mirror is verrial or horizontal
      /// setting 1: double   - location in the frame for the centre of the mirror
      /// setting 2: bool     - Whether to use bottom (or right) or top (or left) as the source
      /// </summary>
      /// <returns>The <see cref="Setting"/>s for this plugin
      /// </returns>
      public new List<Setting> getSettings()
      {
         return settings;
      }



      // ----------------------private processing methods ---------------------
      /// <summary>
      /// set up the settings object
      /// setting 0: bool     - whether the mirror is verrial or horizontal
      /// setting 1: double   - location in the frame for the centre of the mirror
      /// setting 2: bool     - Whether to use bottom (or right) or top (or left) as the source
      /// </summary>
      private void initSettings()
      {
         settings = new List<Setting>();

         // setting 0: whether the mirror is verrial or horizontal
         Setting setting = new Setting();
         setting.name = "Horizontal mirror";    // The display name of this setting
         setting.minValue = 0;                  // Min length of field
         setting.maxValue = 1;                  // Max length of field 
         setting.type = typeof(bool);           // It's a string setting
         setting.value = (bool)false;            // default value is empty string
         settings.Add(setting);

         // setting 1: location of the mirror
         setting = new Setting();
         setting.name = "Mirror location";    // The display name of this setting
         setting.minValue = 0;                // Min length of field
         setting.maxValue = 1;                // Max length of field 
         setting.type = typeof(double);       // It's a string setting
         setting.value = (double)0.5;         // default value is empty string
         settings.Add(setting);

         // setting 2: Whether to use bottom (or right) or top (or left) as the source
         setting = new Setting();
         setting.name = "Bottom/Right source";  // The display name of this setting
         setting.minValue = 0;                  // Min length of field
         setting.maxValue = 1;                  // Max length of field 
         setting.type = typeof(bool);           // It's a string setting
         setting.value = (bool)false;           // default value is empty string
         settings.Add(setting);
      }

      /// <summary>
      /// Horribly in-efficient implemntation of memcpy for plain old unsafe byte arrays
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="len"></param>
      private unsafe void memcpy(byte* dest, byte* src, int len)
      {
         for (int x = 0; x < len; x++)
            dest[x] = src[x];
      }

   }
}
