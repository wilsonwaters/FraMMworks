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
   /// Definition for a basic Frame Source. This provides functionality for classes
   /// which generate Frames of any type. This may be a video file reader class
   /// (which will only implelement ISink) or a processing class (which will implement
   /// both ISink and ISource).
   /// 
   /// Author: Wilson Waters
   /// Date: 20080107
   /// </remarks>
   public interface ISource : IPlugin
   {
      /// <summary>
      /// Provides a list of <see cref="IFrame"/> types which this implemtation can handle.
      /// For example, a basic <I>VideoFileReader</I> implementation provides frames from
      /// a single video channel. The capabilities for this class would
      /// return a single item is the list of type <I>VideoFrame</I>
      /// </summary>
      /// <returns>A List of IFrame types this implemntation can provide</returns>
      Type[] OutputCapabilities { get;}

      /// <summary>
      /// Obtains a list of <see cref="IFrame"/> from this source. The Frames
      /// will corrospond to the types indicated by getOutputCapabilities().
      /// This method may block until a frame is available.
      /// </summary>
      /// <return>the frame just processed/obtained.</return>
      /// <param name="pin">the "pin" we want to get a frame from - 
      /// corresponds to a position in the OutputCapabilities array
      /// </param>
      IFrame getFrame(int pin);


   }
}
