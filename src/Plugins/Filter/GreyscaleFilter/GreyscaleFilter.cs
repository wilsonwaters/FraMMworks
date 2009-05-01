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

namespace GreyscaleFilter
{
   /// <remarks>
   /// Convert a frame to Greyscale
   /// Simple demonstration filter to show how to write filter plugins.
   /// 
   /// This is the *most* basic type of plugin, you should probably also
   /// implement the Name and Description properties
   /// </remarks>
   public class GreyscaleFilter : SimpleProcessingFilter
   {
      //------------------------- public properties ---------------------------
      /// <summary>
      /// What type of output does this class handle
      /// </summary>
      public override Type Capability
      {
         get { return typeof(FraMMWorksImageFrame); }
      }

      //------------------------- private data members--------------------------

      //------------------------- Constructors ---------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public GreyscaleFilter()
      {
      }

      //------------------------- public access memebers------------------------

      /// <summary>
      /// Process this frame by converting every pixel to grey
      /// </summary>
      /// <param name="inputFrame">The frame to operate on</param>
      protected override void processFrame(IFrame inputFrame)
      {
         // Make sure the frame is as expected
         if (inputFrame == null || !(inputFrame is FraMMWorksImageFrame))
            throw new ApplicationException("Video mirror plugin - was passed an invalid frame");

         // Set some variables for easy use
         Bitmap image = (Bitmap)((FraMMWorksImageFrame)inputFrame).Image;
         /*
         // convert to greyscale - simple but hilariously inefficient.
         for (int x = 0; x < image.Width / 2; x++)
         {
            for (int y = 0; y < image.Height / 5; y++)
            {
               Color pix = image.GetPixel(x, y);
               byte grey = (byte)(((int)pix.R + (int)pix.G + (int)pix.B) / 3);
               image.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
            }
         }
          */
      }


      // ----------------------private processing methods ---------------------


   }
}
