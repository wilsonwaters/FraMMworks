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

namespace FraMMWorks.FrameTypes
{
   /// <remarks>
   /// Holds a generic "image" of some sort. Uses the .net Image class
   /// as the internal storage, so the image could be of any type
   /// </remarks>
   public abstract class ImageFrame : Frame
   {
      /*--------------------------private data members------------------------*/
      /// <summary>
      /// The actual internal storage of the image.
      /// </summary>
      protected Image image;


      /*--------------------------public Properties --------------------------*/
      /// <summary>
      /// width of this image in pixels
      /// </summary>
      public int Width
      {
         get { return image.Width; }
      }

      /// <summary>
      /// height of this image in pixels
      /// </summary>
      public int Height
      {
         get { return image.Height; }
      }

      /// <summary>
      /// Get or set the internal image
      /// </summary>
      public virtual Image Image
      {
         get { return image; }
         set { image = value; }
      }

      /*--------------------------Constructors --------------------------------*/
      /// <summary>
      /// Default contructor
      /// </summary>
      public ImageFrame()
      {
         image = null;
      }

      /*--------------------------public processing methods -------------------*/

   }
}
