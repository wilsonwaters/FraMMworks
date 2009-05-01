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

using FraMMWorks.Interfaces;

namespace FraMMWorks.FrameTypes
{
   /// <remarks>
   /// Abstract Base class for implementing a Frame class. A frame is the basic
   /// way in which data is passed around FraMMWorks.
   /// 
   /// Examples of Frame inplementations are VideoFrame and AudioFrame. A
   /// plugin may implement its own Frame classes is required, though the
   /// provided implementaions (VideoFrame and AudioFrame etc.)
   /// should be suitable for most plugins.
   /// 
   /// Plugins classes must implement the following methods:
   /// getRawData() - provide a byte array representation of the data.
   /// 
   /// </remarks>
   public abstract class Frame : IFrame
   {
      /*--------------------------private data members------------------------*/

      private uint frameNum;
      /// <summary>
      /// The sequential number of this particular frame.
      /// </summary>
      /// <returns>unique frame number (0-4billion)</returns>
      public uint FrameNum
      {
         get {return frameNum;}
         set {frameNum = value;}
      }

      //----------------------abstract methods-------------------------------

      /// <summary>
      /// Obtain a byte buffer representing the raw data held by this frame.
      /// 
      /// *** Should this perhaps return a reference to an internal byte buffer?
      /// </summary>
      /// <returns></returns>
      public abstract byte[] getRawData();

      /// <summary>
      /// Create an identical frame to this one.
      /// </summary>
      /// <returns></returns>
      public abstract Object Clone();


      //----------------------public methods---------------------------------

      /// <summary>
      /// Implementation of frame number getter
      /// </summary>
      /// <returns></returns>
      public uint getFrameNum()
      {
         return frameNum;
      }
   }
}
