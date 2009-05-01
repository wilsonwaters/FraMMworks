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

using FraMMWorks.Core;

namespace FraMMWorks.FrameTypes
{
   /// <remarks>
   /// The standard image format for FraMMWorks.
   /// Holds an image frame as a 24 bit RGB bitmap.
   /// 
   /// NOTE: this class is not thread safe (particularly the Lock() and Unlock()
   /// functions. Infact, all IFrames do not need to be thread safe.
   /// </remarks>
   public class FraMMWorksImageFrame : ImageFrame
   {
      /*--------------------------private data members------------------------*/
      /// <summary>
      /// Used to lock/unlock the image. If this is not null, the image is locked
      /// </summary>
      private BitmapData lockData;


      /*--------------------------public Properties --------------------------*/
      /// <summary>
      /// the number of bytes contained in the raw data
      /// </summary>
      public int BitmapSize
      {
         get { return (image.Width * image.Height); }
      }

      /// <summary>
      /// The actual video frame represented as a Bitmap internally.
      /// You can pass in any type of Image to this, but for optimal
      /// performance pass a Format24bppRgb bitmap. Otherwise we convert
      /// the image to a Format24bppRgb Bitmap.
      /// </summary>
      public override Image Image
      {
         get { return image; }
         set
         {
            if (value is Bitmap && value.PixelFormat == PixelFormat.Format24bppRgb)
               image = value;
            else
            {
               ControlAPI.Instance.debugMessage(this, "warning: input image isn't optimal format (Format24bppRgb). Conversion to this format happens automatically, but is slow).");
               image = convertImage(value);
            }
         } 
      }

      /*--------------------------Constructors --------------------------------*/
      /// <summary>
      /// Default contructor
      /// </summary>
      public FraMMWorksImageFrame()
      {
         image = null;
         lockData = null;
      }

      /// <summary>
      /// takes an image to use as the internal bitmap
      /// </summary>
      public FraMMWorksImageFrame(Image image)
      {
         this.Image = image; // converts if neccecery.
         lockData = null;
      }

      /// <summary>
      /// Performs a deep copy of this frame.
      /// </summary>
      /// <returns></returns>
      public override Object Clone()
      {
         // First, make sure the image is not locked. We can't clone a locked bitmap.
         if (lockData != null)
            throw new InvalidOperationException("Can not clone a locked FraMMWorks image file. This frame is currently being used. frame=" + FrameNum);

         FraMMWorksImageFrame f = new FraMMWorksImageFrame();
         f.image = (Image)this.image.Clone();
         return f;
      }

      /*--------------------------public processing methods -------------------*/
      /// <summary>
      /// Locks the internal image and returns a pointer to the raw bitmap data.
      /// Ensure you call UnlockBitmap() after using this.
      /// 
      /// TODO: implement locking to make thread safe (i.e. multiple calls to lockBitmap
      /// will wait until the previous has finished.) Bad idea? This may cause deadlock
      ///
      /// </summary>
      /// <returns></returns>
      public IntPtr LockBitmap()
      {
         // make sure it's not already locked. If it is, just wait until it's available
         if (lockData != null)
            throw new InvalidOperationException("Tried to lock a bitmap, but it is already locked. frame="+FrameNum);

         lockData = (image as Bitmap).LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, image.PixelFormat);

         // Get the address of the first line.
         return lockData.Scan0;
      }

      /// <summary>
      /// Unlocks the bitmap after LockBitmap has been used to modify the frame.
      /// </summary>
      public void UnlockBitmap()
      {
         if (lockData == null)
            throw new InvalidOperationException("Tried to unlock a bitmap, but it was not locked. frame=" + FrameNum);

         (image as Bitmap).UnlockBits(lockData);
         lockData = null;
      }

      /// <summary>
      /// Obtain a byte buffer representing the raw data held by this frame.
      /// 
      /// This method copies the raw data to a new array, so operations on it
      /// will not affect the original bitmap.
      /// You may be better off accessing the image.LockBits funciton directly
      /// if you want to modify this frame.
      /// 
      /// </summary>
      /// <returns>RGB based array</returns>
      public override byte[] getRawData()
      {
         // This is a faster way, but doesn't handle conversion of types
         BitmapData data = (image as Bitmap).LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, image.PixelFormat);

         // Get the address of the first line.
         IntPtr ptr = data.Scan0;

         // Declare an array to hold the bytes of the bitmap.
         int bytes = data.Stride * image.Height;
         byte[] rgbValues = new byte[bytes];

         // Copy the RGB values into the array.
         System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
 
         // Unlock the bits.
         (image as Bitmap).UnlockBits(data);

         /*
         // Slow way - getPixel
         int bytes = image.Width * image.Height * 3;
         byte[] rgbValues = new byte[bytes];

         for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
               Color pix = image.GetPixel(x, y);
               rgbValues[(x * y) + (x * 3)] = pix.R;
               rgbValues[(x * y) + (x * 3) + 1] = pix.G;
               rgbValues[(x * y) + (x * 3) + 2] = pix.B;
            }
          */

         return rgbValues;
      }

      /// <summary>
      /// Takes any image and converts it to the format suitable for a VideoFrame
      /// i.e. a PixelFormat.Format24bppRgb Bitmap.
      /// </summary>
      /// <param name="i"></param>
      /// <returns></returns>
      private static Bitmap convertImage(Image i)
      {
         Bitmap b = new Bitmap(i.Width, i.Height, PixelFormat.Format24bppRgb);
         b.SetResolution(i.HorizontalResolution, i.VerticalResolution);
         Graphics g = Graphics.FromImage(b);
         g.DrawImage(i, 0, 0);
         g.Dispose();
         return b;
      }
   }
}
