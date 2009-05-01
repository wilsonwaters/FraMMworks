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

namespace FraMMWorks.Interfaces
{
   /// <remarks>
   /// Definition of a basic frame. This is the data which will be passed through
   /// the processing chain. Implementaion of this class may be a VideoFrame,
   /// AudioFrame, NumericalDataFrame etc.
   /// 
   /// A frame does not need to be thread safe. There should only ever be a single
   /// thread accessing the frame as it passes through the pipeline.
   /// 
   /// Author: Wilson Waters
   /// Date: 20080107
   /// </remarks>
   /// <seealso cref="FrameTypes.VideoFrame"/>
   public interface IFrame:ICloneable
   {
      /// <summary>
      /// The sequential number of this particular frame.
      /// </summary>
      /// <returns>unique frame number (0-4billion)</returns>
      uint getFrameNum();

      /// <summary>
      /// Obtain a byte buffer representing the raw data held by this frame.
      /// 
      /// *** Should this perhaps return a reference to an internal byte buffer?
      /// </summary>
      /// <returns></returns>
      byte[] getRawData();

      /// <summary>
      /// Creates an identical frame to the current one.
      /// Should be a deep copy where appropriate, as we want two completely
      /// separate frames.
      /// </summary>
      /// <returns></returns>
      new Object Clone();

   }
}
