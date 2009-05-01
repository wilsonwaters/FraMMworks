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

namespace FraMMWorks.PluginBase
{
   /// <remarks>
   /// Abstract Base class for implementing a controllable source plugin.
   /// Controllable sources are similar to Sources, except they may be controled through
   /// seek(). They also provide the preferred frame rate for
   /// the source through the SourceFrameRate property.
   /// Some examples of this plugin would be a video or audio file reader.
   /// This is not suitable for live streams where the number of frames is not known.
   /// You must use a standard source in this case.
   /// 
   /// Plugins must implement the following methods:
   /// getOutputCapabilities() - describes what types of frames the plugin provides
   /// getFrame() - will be called to obtain the next frame.
   /// start() - called to start accessing the source. Current frame will be reset to 0
   /// stop() - called to stop accessing the source. Current frame will be reset to 0
   /// seek() - move to a new position within the source
   /// 
   /// Plugins may optionally override the following methods:
   /// getSettings() - advertise modifyable settings particular to this plugin
   /// getDisplayControl() - for displaying information about the plugin on the main GUI
   /// </remarks>
   public abstract class ControllableSource : Source
   {
      // ----------------------- private data members -------------------------
      /// <summary>Internal access for NumFrames</summary>
      protected uint numFrames;

      /// <summary>Internal access for SourceFrameRate</summary>
      protected double sourceFrameRate;

      // ----------------------- public data members --------------------------
      /// <summary>
      /// The number of frames this source contains. A value of 0 indicates there
      /// are no frames in this source.
      /// </summary>
      public uint NumFrames
      {
         get { return numFrames; }
      }

      /// <summary>
      /// Provides the expected rate this source will be reproduced at. (i.e. frames
      /// per second). A value of 0.0 means the source frame rate is unknown and should
      /// be read as fast as possible.
      /// </summary>
      /// <returns></returns>
      public double SourceFrameRate
      {
         get { return sourceFrameRate; }
      }


      // ------------------------ Constructors --------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public ControllableSource()
      {
         numFrames = 0;
         sourceFrameRate = 0.0;
      }


      // ------------------------ public abstract methods ---------------------
      /// <summary>
      /// Move to a new position in the source represented by the number of
      /// frame from the beginning of the source.
      /// </summary>
      public abstract void seek(uint frameNum);

   }
}
