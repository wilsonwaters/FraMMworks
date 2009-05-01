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
using System.Threading;

using FraMMWorks.Interfaces;
using FraMMWorks.FrameTypes;


namespace FraMMWorks.PluginBase
{
   /// <summary>
   /// Abstract base class for implementing a simple filter.
   /// It allows basic frame processing plugins to be created by hiding
   /// details of threading, receiving and sending frames through provision
   /// of the processFrame(IFrame) method. The simple filter only has capability
   /// to deal with one type of frame, one at a time.
   /// 
   /// Plugins must implement the following
   /// processFrame() - perform some operation on a frame and return.
   /// capability field - to show what type of frame it handles.
   /// 
   /// Plugins may optionally override the following methods:
   /// getSettings() - advertise modifyable settings particular to this plugin
   /// getDisplayControl() - for displaying information about the plugin on the main GUI
   /// </summary>
   public abstract class SimpleProcessingFilter : Filter
   {
      /*--------------------------protected data members------------------------*/
      /// <summary>
      /// Plugins must override this property to notify what types of IFrame they
      /// handle. i.e.
      /// </summary>
      public abstract Type Capability
      {
         get;
      }


      /*--------------------------private data members------------------------*/
      /// <summary>
      /// temp storage for the frame once it's been processed
      /// </summary>
      private IFrame completedFrame;

      /// <summary>
      /// Signals when a frame is ready to be accessed.
      /// </summary>
      private AutoResetEvent completedFrameEvent;

      /// <summary>
      /// Signals when a completed frame has been taken
      /// </summary>
      private AutoResetEvent frameTakenEvent;

      /// <summary>
      /// Simplified method for implementing filters. Plugins simply implement
      /// this method without concern for accessing and returning frames through
      /// sentFrame and getFrame.
      /// </summary>
      /// <param name="inputFrame">The frame to operate on</param>
      protected abstract void processFrame(IFrame inputFrame);

      /*--------------------------Constructor---------------------------------*/
      /// <summary>
      /// DefaultConstructor
      /// </summary>
      public SimpleProcessingFilter()
      {
         completedFrame = null;
         completedFrameEvent = new AutoResetEvent(false);
         frameTakenEvent = new AutoResetEvent(true);
      }

      /*--------------------------Public processing methods-------------------*/

      /// <summary>
      /// Queries the plugin for what type of trame it handles and returns it in
      /// the correct format.
      /// </summary>
      /// <returns>A List with a single IFrame type this implemntation will accept</returns>
      public override Type[] InputCapabilities
      {
         get { return new Type[] { Capability }; }
      }

      /// <summary>
      /// Queries the plugin for what type of trame it handles and returns it in
      /// the correct format.
      /// </summary>
      /// <returns>A List with a single IFrame type this implemntation can provide</returns>
      public override Type[] OutputCapabilities
      {
         get { return new Type[] { Capability }; }
      }

      /// <summary>
      /// This method will be called by external classes with a frame to be
      /// processed.
      /// 
      /// As this a SimpleProcessingFilter can only ever handle one frame at
      /// a time, it is an error to pass in multiple frames.
      /// </summary>
      /// <param name="frame">the input frame to operate on</param>
      public override void sendFrame(IFrame frame, int pin)
      {
         if (frame == null)
            throw new ArgumentException("SimpleProcessingFilter was sent a null frame", "frame");

         // Make sure there is only one frame
         if (pin != 0)
            throw new ArgumentException("SimpleProcessingFilters can only handle one input frame", "pin");

         // Wait until the last frame has been taken by the getFrame method
         frameTakenEvent.WaitOne();

         // Pass this frame to the processFrame method
         processFrame(frame);

         // place the frame into the output buffer to be retrived by something else in the getFrame method.
         completedFrame = frame;
         completedFrameEvent.Set();
      }

      /// <summary>
      /// Obtains a list of <see cref="IFrame"/> from this source. The Frames
      /// will corrospond to the types indicated by getOutputCapabilities().
      /// This method may block until a frame is available.
      /// </summary>
      /// <return>the frame just processed/obtained.</return>
      /// <param name="pin">must always be 0 on SimpleProcessingFilters
      /// </param>
      public override IFrame getFrame(int pin)
      {
         // Make sure there is only one frame
         if (pin != 0)
            throw new ArgumentException("SimpleProcessingFilters only provide one output frame", "pin");

         // check if there is a frame ready
         completedFrameEvent.WaitOne();

         // If we get here, there must be a frame ready. But make sure it isn't some error
         if (completedFrame == null)
            throw new ApplicationException("A filter was woken while waiting for a frame to finish processing.");

         // save a reference so another thread can re-use the completedFrame variable
         IFrame tempFrame = completedFrame;
         completedFrame = null;

         // Signal that the class is ready to process another frame
         frameTakenEvent.Set();

         return tempFrame;
      }

      /// <summary>
      /// throw out any waiting frames and unblock all calls.
      /// </summary>
      public override void flush()
      {
         this.completedFrame = null;

         completedFrameEvent.Set();
         completedFrameEvent.Reset();
         frameTakenEvent.Set();
      }
   }
}
