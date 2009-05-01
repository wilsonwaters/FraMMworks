using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using FraMMWorks.Interfaces;
using FraMMWorks.FrameTypes;
using FraMMWorks.PluginBase;
using FraMMWorks.Core;

namespace Splitter
{
   /// <remarks>
   /// Takes a single frame of any type and duplicates it 'n' times on the output.
   /// the number of outputs the frame is to split is configurable.
   /// 
   /// Wilson Waters 20080221
   /// </remarks>
   public class Splitter: Filter
   {
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      public override String Name
      {
         get { return name; }
      }

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      public override String Description
      {
         get { return description; }
      }

      /// <summary>
      /// Queries the plugin for what type of trame it handles and returns it in
      /// the correct format.
      /// </summary>
      /// <returns>A List with a single IFrame type this implemntation will accept</returns>
      public override Type[] InputCapabilities
      {
         get { return new Type[] { capability }; }
      }

      /// <summary>
      /// Queries the plugin for what type of trame it handles and returns it in
      /// the correct format.
      /// </summary>
      /// <returns>A List with a single IFrame type this implemntation can provide</returns>
      public override Type[] OutputCapabilities
      {
         get
         {
            lock (settings[0])
            {
               Type[] ret = new Type[(int)settings[0].value];
               for (int i = 0; i < (int)settings[0].value; i++)
               {
                  ret[i] = capability;
               }
               return ret;
            }
         }
      }


      //------------------------- constants ------------------------------------
      /// <summary>
      /// The default setting for the number of ways to split.
      /// </summary>
      const int DEFAULT_SPLIT_WAYS = 2;


      //------------------------- private data members--------------------------

      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      private String name = "Frame Splitter";

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      private String description = "Split a single input frame to 'n' outputs.";

      /// <summary>
      /// The splitter isn't interested in what type of frame it's splitting, so
      /// just base it off the frame Interface
      /// </summary>
      private Type capability = typeof(IFrame);

      /// <summary>
      /// The settings list for this plugin.
      /// </summary>
      private List<Setting> settings;

      /// <summary>
      /// temp storage for the output frames once they've been processed
      /// </summary>
      private IFrame [] completedFrames;

      /// <summary>
      /// array of events which will cause each chennel's getFrame() to block
      /// until the existing frame is taken.
      /// </summary>
      private AutoResetEvent [] completedFrameEvents;

      /// <summary>
      /// Signals when a completed frame has been taken
      /// </summary>
      private AutoResetEvent [] frameTakenEvents;

      /// <summary>
      /// The current number of output pins we're dealing with.
      /// </summary>
      private int numOutputPins;


      //------------------------- Constructors ---------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public Splitter()
      {
         // Set up the default settings for this plugin
         initSettings();
         updateSettings();
      }

      //------------------------- public access memebers------------------------

      /// <summary>
      /// This method will be called by external classes with a frame to be
      /// split.
      /// 
      /// </summary>
      /// <param name="frameList">the input frame to operate on</param>
      /// <param name="pin">the "pin" we want to send a frame to - 
      /// corresponds to a position in the InputCapabilities array.
      /// </param>
      public override void sendFrame(IFrame frame, int pin)
      {
         if (frame == null)
            throw new ArgumentException("SimpleProcessingFilter was sent a null frame", "frame");

         // Make sure there is only one frame
         if (pin != 0)
            throw new ArgumentException("SimpleProcessingFilters can only handle one input frame", "pin");

         // Wait until all the frames have been taken by the getFrame methods
         System.Threading.WaitHandle.WaitAll(frameTakenEvents);

         // reuse the first frame
         completedFrames[0] = frame;

         // Split the rest of them
         for (int i = 1 ; i<numOutputPins ; i++)
         {
            completedFrames[i] = (IFrame)frame.Clone();
         }

         // signal everything that we're ready to take frames!
         for (int i=0; i<numOutputPins ; i++)
            completedFrameEvents[i].Set();
      }

      /// <summary>
      /// Obtains a <see cref="IFrame"/> from this source. The Frames
      /// will corrospond to the types indicated by getOutputCapabilities().
      /// This method may block until a frame is available.
      /// </summary>
      /// <return>the frame just processed/obtained.</return>
      /// <param name="pin">the "pin" we want to get a frame from - 
      /// corresponds to a position in the OutputCapabilities array
      /// </param>
      public override IFrame getFrame(int pin)
      {
         // Check if the settings have changed
         if (settings[0].isDirty)
         {
            updateSettings();
         }

         // make sure this pin exists
         if (pin >= numOutputPins)
            throw new ProcessingException("Splitter plugin error: tried to read from a pin that doesn't exist", false);

         // wait until there is a frame ready
         completedFrameEvents[pin].WaitOne();

         // If we get here, there must be a frame ready. But make sure it isn't some error
         if (completedFrames[pin] == null)
            throw new ProcessingException("The Splitter plugin was woken while waiting for a new frame to arrive.", true);

         // save a reference so another thread can re-use the completedFrame variable
         IFrame tempFrame = completedFrames[pin];
         completedFrames[pin] = null;

         // Signal that the class is ready to process another frame
         frameTakenEvents[pin].Set();

         return tempFrame;
      }

      /// <summary>
      /// throw out any waiting frames and unblock all calls.
      /// </summary>
      public override void flush()
      {
         for (int i = 0; i < completedFrames.Length; i++)
         {
            completedFrames[i] = null;
            completedFrameEvents[i].Set();
            completedFrameEvents[i].Reset();
            frameTakenEvents[i].Set();
         }
      }

      /// <summary>
      /// The setings for this plugin
      /// setting 0: int - the number of ways to split the input frame.
      /// </summary>
      /// <returns>The <see cref="Setting"/>s for this plugin
      /// </returns>
      public override List<Setting> getSettings()
      {
         return settings;
      }



      // ----------------------private processing methods ---------------------
      /// <summary>
      /// set up the settings object
      /// setting 0: int - the number of ways to split the input frame.
      /// </summary>
      private void initSettings()
      {
         settings = new List<Setting>();

         // setting 0: how many ways to split the frame
         Setting setting = new Setting();
         setting.name = "Split times";
         setting.minValue = 1;
         setting.maxValue = 256;
         setting.type = typeof(int);
         setting.value = DEFAULT_SPLIT_WAYS;
         settings.Add(setting);

      }

      /// <summary>
      /// Updates the internal state of this class to reflect the new settings.
      /// 
      /// This may be called multiple times (i.e. if multiple plugins try to
      /// read frames at once). The setting will only be updated once.
      /// </summary>
      private void updateSettings()
      {
         lock (settings[0])
         {
            int newNumWays = (int)settings[0].value;
            if (completedFrames == null)
            {
               // Frst time the completedFrames array is used.
               completedFrames = new IFrame[newNumWays];
               completedFrameEvents = new AutoResetEvent[newNumWays];
               frameTakenEvents = new AutoResetEvent[newNumWays];

               // add the pins / events
               for (int i = 0; i < newNumWays; i++)
               {
                  completedFrames[i] = null;
                  completedFrameEvents[i] = new AutoResetEvent(false);
                  frameTakenEvents[i] = new AutoResetEvent(true);
               }
            }
            else if (newNumWays > completedFrames.Length)
            {
               // The settings have changed to make this split MORE ways
               // add more pins to the end and copy the frames to these new pins (if there are any to copy)

               // Create a temp array
               IFrame [] tempCompletedFrames = new IFrame[newNumWays];

               // check if there's anything to copy
               IFrame newFrame = null;
               foreach (IFrame frame in completedFrames)
               {
                  if (frame != null)
                     newFrame = (IFrame)frame.Clone();
                  break;
               }

               // copy the existing frames
               for (int i = 0; i < completedFrames.Length; i++)
               {
                  tempCompletedFrames[i] = completedFrames[i];
               }

               // add the new frames/events
               for (int i = completedFrames.Length; i < newNumWays; i++)
               {
                  tempCompletedFrames[i] = newFrame;
                  completedFrameEvents[i] = new AutoResetEvent(false);
                  frameTakenEvents[i] = new AutoResetEvent(true);
               }

               // clober the old array.
               completedFrames = tempCompletedFrames;

            }
            else if (newNumWays < completedFrames.Length)
            {
               // the settings have been changed to make this split less ways
               // remove the last pins and simply scrap the frames.
               // Also have to notify any threads waiting on frames that we're
               // about to scrap

               // notify any waiting threads.
               // They will probably throow a recoverable ProcessingException
               // if they're currently waiting.
               for (int i = newNumWays; i < completedFrames.Length; i++)
               {
                  completedFrames[i] = null;
                  completedFrameEvents[i].Set();
               }

               // Create a temp array
               IFrame[] tempCompletedFrames = new IFrame[newNumWays];

               // copy the existing frames
               for (int i = 0; i < newNumWays; i++)
               {
                  tempCompletedFrames[i] = completedFrames[i];
               }

               // clober the old array.
               completedFrames = tempCompletedFrames;

            }
            else // nothing has changed
            {
            }

            // save the number of pins
            numOutputPins = newNumWays;
            settings[0].isDirty = false;
         }
      }
   }
}
