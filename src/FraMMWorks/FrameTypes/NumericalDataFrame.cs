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

namespace FraMMWorks.FrameTypes
{
   /// <remarks>
   /// Holds a 2D array of numerical data with labels to describe each field.
   /// TODO: implement
   /// </remarks>
   public class NumericalDataFrame : Frame
   {
      /// <summary>
      /// Obtain a byte buffer representing the raw data held by this frame.
      /// 
      /// *** Should this perhaps return a reference to an internal byte buffer?
      /// </summary>
      /// <returns></returns>
      public override byte[] getRawData()
      {
         return new byte[1];
      }

      /// <summary>
      /// Performs a shallow copy so far
      /// </summary>
      /// <returns></returns>
      public override Object Clone()
      {
         return this.MemberwiseClone();
      }
   }
}
