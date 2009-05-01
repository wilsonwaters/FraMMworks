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
   /// Abstract Base class for implementing a Sink plugin. Sink plugins receive
   /// frames only. Some examples would be a video display sink, or an audio
   /// writer.
   /// 
   /// Plugins must implement the following:
   /// default constructor with no parameters
   /// inputCapabilities field - describes what types of frames the plugin handles
   /// sendFrame() - will be called with an incoming frame
   /// 
   /// Plugins may optionally override the following methods:
   /// getSettings() - advertise modifyable settings particular to this plugin
   /// getDisplayControl() - for displaying information about the plugin on the main GUI
   /// </remarks>
   public abstract class Sink : ISink
   {
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      public virtual String Name
      {
         get { return this.GetType().Name; }
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
      /// For example, a basic <I>VideoDisplay</I> implementation takes a single video
      /// channel and draws it to the screen. The capabilities for this class would
      /// return a single item is the list of type <I>VideoFrame</I>
      /// </summary>
      /// <returns>A List of IFrame types this implemntation will accept</returns>
      public abstract Type[] InputCapabilities
      {
         get;
      }

      /// <summary>
      /// Provides a list of settings applicable to this implementation. The returned
      /// types must be <see cref="Setting"/> objects which provide a direct reference
      /// of the value to be changed.
      /// Settings may be basic types (Int32, Double, String, etc) or more complex types
      /// (such as <I>VideoFrameSelector</I>) which must implement <see cref="System.Windows.Forms.Control"/> 
      /// to describe how to draw itself.
      /// 
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
      /// Passes a list of <see cref="IFrame"/> to this sink for processing. The Frames
      /// should corrospond to the types indicated by getInputCapabilities().
      /// This method should block while the frame is being processed. 
      /// </summary>
      /// <param name="frame">the input frames for this sink class</param>
      /// <param name="pin">the "pin" we want to send a frame to - 
      /// corresponds to a position in the InputCapabilities array.
      /// </param>
      public abstract void sendFrame(IFrame frame, int pin);

      /// <summary>
      /// Provides a <see cref="System.Windows.Forms.Control"/> object which displays
      /// information about this frame sink on the GUI.
      /// This is optional and may return null if not required (default).
      /// </summary>
      /// <return>a control object which draws informaion about this plugin
      /// or null to indcate there is no associated drawable control.
      /// </return>
      public virtual System.Windows.Forms.Control getDisplayControl()
      {
         return null;
      }
   }
}
