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
using System.Windows.Forms;

using FraMMWorks.PluginBase;
using FraMMWorks.Interfaces;

namespace FraMMWorks.Core
{
   /// <remarks>
   /// External API to the core functionality of FraMMWorks.
   /// Provides ways to handle topology, navigation, plugin information
   /// debugging etc.
   /// 
   /// This API implements a Singleton pattern and may be used by a large
   /// number of plugins and clients to control the system.
   /// 
   /// Wilson Waters W.Waters@curtin.edu.au 20080115
   /// </remarks>
   public class ControlAPI
   {
      #region ----------------------- constants -------------------------------------
      /// <summary>
      /// Maximum number of messages stored in error and debug history.
      /// </summary>
      private const int MAX_MESSAGE_HISTORY = 100;
      #endregion

      #region ----------------------- public enums ----------------------------------
      /// <summary>
      /// Holds an action related to source control, such as play, stop, pause
      /// </summary>
      public enum ControlAction
      {
         play,
         stop,
         pause,
         unpause,
         seek
      };
      #endregion

      #region ----------------------- private data members --------------------------
      /// <summary>
      /// The actual singleton instance
      /// </summary>
      private static volatile ControlAPI instance;

      /// <summary>
      /// Thread lock for multi-threaded access to object.
      /// </summary>
      private static object syncRoot = new Object();

      /// <summary>
      /// The actual processing pipeline which does the grunt of the work.
      /// </summary>
      private ProcessingPipeline pipeline;

      /// <summary>
      /// The current topology of the system.
      /// </summary>
      private Topology topology;

      /// <summary>
      /// Holds a history of debug messages.
      /// </summary>
      private LinkedList<String> debugMessages;

      /// <summary>
      /// Holds a history of error messages.
      /// </summary>
      private LinkedList<String> errorMessages;

      /// <summary>
      /// Store the "current frame number". This is simply the maximum
      /// frame number from all the sources.
      /// </summary>
      private uint currentFrameNumber;

      /// <summary>
      /// Store the "max frame number". This is simply the maximum
      /// numFrames of all the sources.
      /// </summary>
      private uint maxFrameNumber;

      private Object frameNumberLock;
      #endregion


      #region ----------------------- public properties -----------------------------
      /// <summary>
      /// A list of recent error messages (note, only the last 100 messages are
      /// stored). This returns a copy of the internal error list.
      /// </summary>
      public ICollection<String> ErrorMessages
      {
         get
         {
            lock (errorMessages)
            {
               return new LinkedList<String>(errorMessages);
            }
         }
      }

      /// <summary>
      /// A list of recent debug messages (note, only the last 100 messages are
      /// stored). This returns a copy of the internal error list.
      /// </summary>
      public ICollection<String> DebugMessages
      {
         get
         {
            lock (debugMessages)
            {
               return new LinkedList<String>(debugMessages);
            }
         }
      }
      #endregion


      #region ----------------------- public delegate declarations ------------------
      /// <summary>
      /// format for ErrorMessageHandler functions
      /// </summary>
      /// <param name="message"></param>
      public delegate void ErrorMessageHandler(String message);

      /// <summary>
      /// format for ErrorMessageHandler functions
      /// </summary>
      /// <param name="message"></param>
      public delegate void DebugMessageHandler(String message);

      /// <summary>
      /// Format for OnDisplayControlChange event handler
      /// </summary>
      /// <param name="controls"></param>
      public delegate void DisplayControlHandler(List<Control> controls);

      /// <summary>
      /// format for ControlActionHandler event handlers
      /// </summary>
      /// <param name="action"></param>
      public delegate void ControlActionHandler(ControlAction action);

      #endregion


      #region ----------------------- public events ---------------------------------
      /// <summary>
      /// Client applications can connect to this to be notified of debug
      /// massages.
      /// </summary>
      public event DebugMessageHandler OnDebugMessage;

      /// <summary>
      /// Client applications (or even plugins) can connect to this to be
      /// notified of errors.
      /// </summary>
      public event ErrorMessageHandler OnErrorMessage;

      /// <summary>
      /// Connect to this deligate to be notified of new or changed GUI
      /// display controls from the plugins.
      /// </summary>
      public event DisplayControlHandler OnDisplayControlChange;

      /// <summary>
      /// Connect to this to be notified when the source controls are accessed
      /// (i.e. play, stop, pause, unpause, seek). The access may be from the GUI
      /// button presses, or other plugins.
      /// </summary>
      public event ControlActionHandler OnControlAction;
      #endregion


      #region ----------------------- Constructors ----------------------------------
      /// <summary>
      /// default constructor. Will be called internally on first access to
      /// this singleton class.
      /// </summary>
      private ControlAPI()
      {
         pipeline = new ProcessingPipeline();
         debugMessages = new LinkedList<String>();
         errorMessages = new LinkedList<String>();

         frameNumberLock = new Object();
      }

      /// <summary>
      /// Get the PluginManager singleton class. This is thread safe and can
      /// handle two threads calling Instance() at the same time (especially
      /// important when creating the luginManager for the first time).
      /// 
      /// See http://msdn2.microsoft.com/en-us/library/ms998558.aspx for
      /// implementation example.
      /// </summary>
      public static ControlAPI Instance
      {
         get
         {
            if (instance == null)
            {
               lock (syncRoot)
               {
                  if (instance == null)
                     instance = new ControlAPI();
               }
            }

            return instance;
         }
      }
      #endregion


      #region ----------------------- public processing members ---------------------
      /// <summary>
      /// Write a debug message for the user if enabled
      /// </summary>
      /// <param name="message"></param>
      public void debugMessage(object source, string format, params object[] args)
      {
         String message = source.GetType().Name + ": ";
         message += String.Format(format, args);

         lock (debugMessages)
         {
            // save it
            if (debugMessages.Count == MAX_MESSAGE_HISTORY)
            {
               // remove oldest message
               debugMessages.RemoveLast();
            }
            debugMessages.AddFirst(message);
         }

         // inform any clients
         if (OnDebugMessage != null)
            OnDebugMessage(message);
      }

      /// <summary>
      /// Display an error to the user (probably through a message box if running
      /// in GUI mode) and continue processing
      /// </summary>
      /// <param name="message"></param>
      public void nonFatalError(object source, string format, params object[] args)
      {
         String message = source.GetType().Name + ": ";
         message += String.Format(format, args);

         // save it
         lock (errorMessages)
         {
            if (errorMessages.Count == MAX_MESSAGE_HISTORY)
            {
               // remove oldest message
               errorMessages.RemoveLast();
            }
            errorMessages.AddFirst(message);
         }

         // inform any clients
         if (OnErrorMessage != null)
            OnErrorMessage(message);
      }

      /// <summary>
      /// Display an error to the user and quit the application.
      /// This should only be called by extreem data corruption and otherwise
      /// uncorrectable errors (i.e. out of memory)
      /// </summary>
      /// <param name="message"></param>
      public void fatalError(object source, string format, params object[] args)
      {
         String message = "****FATAL ERROR*** ";
         message += source.GetType().Name + ": ";
         message += String.Format(format, args);

         // save it
         lock (errorMessages)
         {
            if (errorMessages.Count == MAX_MESSAGE_HISTORY)
            {
               // remove oldest message
               errorMessages.RemoveLast();
            }
            errorMessages.AddFirst(message);
         }

         // inform any clients
         if (OnErrorMessage != null)
            OnErrorMessage(message);

         DialogResult result = MessageBox.Show("FraMMWorks experienced a fatal error and needs to exit.", "FraMMWorks Fatal Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
         if (result == DialogResult.OK)
            Environment.Exit(1);

      }  

      /// <summary>
      /// Updates the processing chain with either a completly new topology object
      /// or a modified one.
      /// </summary>
      /// <param name="topology"></param>
      public void updateTopology(Topology topology)
      {
         // update the processing pipeline
         lock (this)
         {
            pipeline.updateTopology(topology);
            this.topology = topology;
         }

         // notify any listening clients that the plugin drawing controls have changed
         // only do this if there is anyone listening though
         if (OnDisplayControlChange != null)
         {
            List<Control> controls = new List<Control>();
            foreach (Interfaces.IPlugin plugin in topology.ActivePlugins)
            {
               Control control = plugin.getDisplayControl();
               if (control != null)
               {
                  controls.Add(control);
               }
            }
            OnDisplayControlChange.Invoke(controls);
         }
      }

      /// <summary>
      /// Start processing chain. Creates new processing pipeline and
      /// corrosponding threads. Will start proessing at frame 0;
      /// </summary>
      public void play()
      {
         // first, make sure everything is back to the start
         stop();

         // If there's no topology, we can't do much
         if (topology == null)
            return;

         // make sure the pipeline is ready
         updateTopology(topology);

         // now start all the souces and reset any controllable sources to the first frame
         foreach (IPlugin plugin in this.topology.ActivePlugins)
         {
            if (plugin is ControllableSource)
            {
               ControllableSource source = plugin as ControllableSource;
               source.seek(0);
            }

            if (plugin is Source)
            {
               Source source = plugin as Source;
               source.start();
            }
         }

         // finally unpause the pipeline and it should all go!
         this.pipeline.unpause();
         if (OnControlAction != null)
         {
            OnControlAction.Invoke(ControlAction.play);
            OnControlAction.Invoke(ControlAction.seek);
         }
      }

      /// <summary>
      /// Stops all processing and kills the pipeline thereads
      /// </summary>
      public void stop()
      {
         // set the currentFrameNum to this value 1st
         currentFrameNumber = 0;
         maxFrameNumber = 0;

         if (this.topology != null)
         {
            // first, stop all the source plugins
            foreach (IPlugin plugin in this.topology.ActivePlugins)
            {
               if (plugin is Source)
               {
                  Source source = plugin as Source;
                  source.stop();
               }

               if (plugin is Filter)
               {
                  // throw out any existing data in filters
                  Filter filter = plugin as Filter;
                  filter.flush();
               }
            }
         }

         // and destroy the processing pipeline.
         pipeline.shutdown();
         if (OnControlAction != null)
            OnControlAction.Invoke(ControlAction.stop);
      }

      /// <summary>
      /// pause processing chan at the current frame
      /// </summary>
      public void pause()
      {
         pipeline.pause();
         if (OnControlAction != null)
            OnControlAction.Invoke(ControlAction.pause);
      }

      /// <summary>
      /// resume processing from where it was last paused.
      /// </summary>
      public void unpause()
      {
         pipeline.unpause();
         if (OnControlAction != null)
            OnControlAction.Invoke(ControlAction.unpause);
      }

      /// <summary>
      /// Determines whether the processing pipeline is currently in a paused state
      /// </summary>
      /// <returns></returns>
      public bool isPaused()
      {
         return pipeline.isPaused();
      }

      /// <summary>
      /// Move all seekable sources to a to a specified frame number. If out of
      /// range for a source, go the the limit closest to the specified
      /// frame number.
      /// </summary>
      /// <param name="frameNum"></param>
      public void seek(uint frameNum)
      {
         // set the currentFrameNum to this value 1st
         currentFrameNumber = frameNum;

         foreach (IPlugin plugin in this.topology.ActivePlugins)
         {
            if (plugin is ControllableSource)
            {
               ControllableSource source = plugin as ControllableSource;
               if (source.NumFrames > frameNum)
                  source.seek(frameNum);
            }
         }
         if (OnControlAction != null)
            OnControlAction.Invoke(ControlAction.seek);
      }

      /// <summary>
      /// Gets the current frame number that we are processing
      /// (simply picks the largest source frame num)
      /// </summary>
      /// <returns>the frame we're currently processing</returns>
      public uint getCurrentFrameNum()
      {
         return currentFrameNumber;
         /*
         uint currentMaxFrame = 0;
         foreach (IPlugin plugin in this.topology.ActivePlugins)
         {
            if (plugin is Source)
            {
               Source source = plugin as Source;
               if (source.CurrentFrameNum > currentMaxFrame)
                  currentMaxFrame = source.CurrentFrameNum;
            }
         }
         return currentMaxFrame;
          */
      }

      /// <summary>
      /// Gets the total number of frames we will be processing if known.
      /// Uses the maximum of all ControlableSources NumFrames properties.
      /// </summary>
      /// <returns>total number of frames we'll be processing or 0 for unknown</returns>
      public uint getNumFrames()
      {
         return maxFrameNumber;

         /*
         uint currentMaxFrames = 0;
         foreach (IPlugin plugin in this.topology.ActivePlugins)
         {
            if (plugin is ControllableSource)
            {
               ControllableSource source = plugin as ControllableSource;
               if (source.NumFrames > currentMaxFrames)
                  currentMaxFrames = source.NumFrames;
            }
         }
         return currentMaxFrames;
          */
      }

      /// <summary>
      /// Stops processing and closes the processing pipeline.
      /// </summary>
      public void shutdown()
      {
         stop();
      }

      /// <summary>
      /// Provides a way for Source plugins to notify the ControlAPI that
      /// it has reached EOF. Useful for stopping a processing chain when
      /// all the frames have been read and updating the  GUI to indicate
      /// the source has stopped.
      /// </summary>
      /// <param name="source"></param>
      public void notifySourceStopped(Source source)
      {
         if (OnControlAction != null)
            OnControlAction.Invoke(ControlAction.stop);
      }

      /// <summary>
      /// Privides a way for Source plugins to notify the ControlAPI that
      /// a source frame has been read/generated.
      /// This is useful for updating the trackbar on a GUI.
      /// Passes framenumber rather than the actual Frame. This is to prevent
      /// multithreaded locking issues with the frames (frames aren't thread
      /// safe).
      /// </summary>
      /// <param name="source">The actual source.</param>
      public void notifySourceFrameGrabbed(Source source)
      {
         // NOTE: is may be worth only doing this once every second for efficiency.
         uint frameNum = source.CurrentFrameNum;
         uint numFrames = frameNum;
         if (source is ControllableSource)
            numFrames = (source as ControllableSource).NumFrames;

         lock (frameNumberLock)
         {
            if (frameNum > currentFrameNumber)
               currentFrameNumber = frameNum;

            if (numFrames > maxFrameNumber)
               maxFrameNumber = numFrames;
         }

         if (OnControlAction != null)
            OnControlAction.Invoke(ControlAction.seek);
      }

      #endregion


      #region ----------------------- private processing members --------------------

      #endregion
   }
}
