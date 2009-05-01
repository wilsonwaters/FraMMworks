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

namespace FraMMWorks.PluginBase
{
   /// <remarks>
   /// Abstract Base class for implementing a source plugin. Source plugins generate
   /// frames only. Some examples would be a random noise generator, a black video frame
   /// generator or a live video streem. These tyoes of sources generally don't have an
   /// preferred frame rate and frame should be read as fast as they can be generated (or
   /// required).
   /// 
   /// Use <seealso cref="ControllableSource"/> for sources which may be controlled
   /// (through seek and pause methods) and contain a defined number of frames.
   /// An example of this type would be a video file. These sources should also
   /// have a "preferred" frame rate.
   /// 
   /// Plugins must implement the following:
   /// default constructor with no parameters
   /// outputCapabilities field - describes what types of frames the plugin provides
   /// getFrame() - will be called to obtain the next frame.
   /// 
   /// Plugins may optionally override the following:
   /// name field - a descriptive name for the plugin
   /// description field - a verbose description of the plugin
   /// getSettings() - advertise modifyable settings particular to this plugin
   /// getDisplayControl() - for displaying information about the plugin on the main GUI
   /// </remarks>
   public abstract class Source : ISource
   {
      // ----------------------- private data members -------------------------
      /// <summary>
      /// Number of frames we've read from this source
      /// </summary>
      protected uint currentFrameNum;

      // ----------------------- public data members --------------------------
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      public virtual String Name
      {
         get { return this.GetType().Name;  }
      }

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      public virtual String Description
      {
         get { return ""; }
      }

      /// <summary>
      /// Provides a list of <see cref="IFrame"/> types which this implemtation can handle.
      /// For example, a basic <I>VideoFileReader</I> implementation provides frames from
      /// a single video channel. The capabilities for this class would
      /// return a single item is the list of type <I>VideoFrame</I>
      /// </summary>
      /// <returns>A List of IFrame types this implemntation can provide</returns>
      public abstract Type[] OutputCapabilities
      {
         get;
      }

      /// <summary>
      /// Number of frames we've read from this source
      /// </summary>
      public uint CurrentFrameNum
      {
         get { return currentFrameNum; }
      }

      // ------------------------ public abstract methods ---------------------
      /// <summary>
      /// Provides a list of settings applicable to this implementation. The returned
      /// types must be <see cref="Setting"/> objects which provide a direct reference
      /// of the value to be changed.
      /// Settings may be basic types (Int32, Double, String, etc) or more complex types
      /// (such as <I>VideoFrameSelector</I>) which must implement <see cref="System.Windows.Forms.Control"/> 
      /// to describe how to draw itself.
      /// This funtion returns null by default, indicating there are no modifyable settings.
      /// </summary>
      /// <returns>The <see cref="Setting"/>s which may be modified for this implementation
      /// or null to indicate no settings are available.
      /// </returns>
      public virtual List<Setting> getSettings()
      {
         return null;
      }

      /// <summary>
      /// Obtains a list of <see cref="IFrame"/> from this source. The Frames
      /// will corrospond to the types indicated by getOutputCapabilities().
      /// This method may block until a frame is available.
      /// 
      /// To indicate that no frames are available (i.e. source stopped)
      /// this method may return null.
      /// </summary>
      /// <return>the frame just processed/obtained, or null to indicate EOF</return>
      /// <param name="pin">the "pin" we want to get a frame from - 
      /// corresponds to a position in the OutputCapabilities array
      /// </param>
      public abstract IFrame getFrame(int pin);

      /// <summary>
      /// Provides a <see cref="System.Windows.Forms.Control"/> object which displays
      /// information about this frame source on the GUI.
      /// This is optional and may return null if not required.
      /// </summary>
      /// <return>a control object which draws informaion about this plugin
      /// or null to indcate there is no associated drawable control.
      /// </return>
      public virtual System.Windows.Forms.Control getDisplayControl()
      {
         return null;
      }

      /// <summary>
      /// Start the source.
      /// </summary>
      public abstract void start();

      /// <summary>
      /// Stop the source
      /// </summary>
      public abstract void stop();

   }
}
