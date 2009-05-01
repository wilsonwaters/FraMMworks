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
   /// Definition for a basic Frame Sink. This provides functionality for classes
   /// which receive Frames of any type. This may be a screen display class
   /// (which will only implelement <see cref="ISink"/>) or a processing class
   /// (which will implement both <see cref="ISink"/> and <see cref="ISource"/>).
   /// 
   /// Author: Wilson Waters
   /// Date: 20080107
   /// </remarks>
   public interface ISink : IPlugin
   {
      /// <summary>
      /// Provides a list of <see cref="IFrame"/> types which this implemtation can handle.
      /// For example, a basic <I>VideoDisplay</I> implementation takes a single video
      /// channel and draws it to the screen. The capabilities for this class would
      /// return a single item is the list of type <I>VideoFrame</I>
      /// </summary>
      /// <returns>A List of IFrame types this implemntation will accept</returns>
      Type[] InputCapabilities { get;}

      /// <summary>
      /// Passes a list of <see cref="IFrame"/> to this sink for processing. The Frames
      /// should corrospond to the types indicated by getInputCapabilities().
      /// This method should block while the frame is being processed. 
      /// </summary>
      /// <param name="frame">the input frames for this sink class</param>
      /// <param name="pin">the "pin" we want to send a frame to - 
      /// corresponds to a position in the InputCapabilities array.
      /// </param>
      void sendFrame(IFrame frame, int pin);


   }
}
