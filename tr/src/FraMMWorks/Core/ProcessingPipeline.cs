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
using System.Diagnostics;

using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;

namespace FraMMWorks.Core
{
   /// <remarks>
   /// Joins together plugins as defined by a <seealso cref="Topology"/> and 
   /// handles passing of frames through the pipeline.
   /// 
   /// Designed to operate on multithreaded machines, this pipeline joins
   /// each plugin with a separate thread allowing concurrent processing
   /// for each plugin while still handling framerate throttling and
   /// minimal frame buffering.
   /// 
   /// Assuming unlimited number of threads (on individual processors):
   /// 1) time for frame to travel through pipeline = time to process frame through
   ///    slowest plugin * number of plugins
   /// 2) interframe delay on final sink = time to process frame through slowest plugin
   ///
   /// Wilson Waters W.Waters@curtin.edu.au 20080115
   /// </remarks>
   public class ProcessingPipeline
   {
      //----------------------- consts ----------------------------------------
      /// <summary>
      /// This forces a garbage collection whenever a sink finished with a frame.
      /// It generally shouldn't be required, and performance will suffer quite
      /// a bit when on.
      /// The advantage with leaving it on is memory use will stay fairly consistantly
      /// low.
      /// Turned off will mean memory usage will get quite high before the garbage
      /// collector kicks in.
      /// </summary>
      const bool REGULAR_GARBAGE_COLLECTION = false;

      //----------------------- private structs -------------------------------
      /// <summary>
      /// Provides info about each running thread
      /// </summary>
      private struct GraphJoinerThreadInfo
      {
         /// <summary>
         /// The actual thread handling this edge on the graph
         /// </summary>
         public Thread thread;

         /// <summary>
         /// Information about how this edge joins into the topology
         /// </summary>
         public Topology.GraphEntry edge;
      }

      //----------------------- private data members --------------------------
      /// <summary>
      /// The currently running topology graph
      /// </summary>
      private List<GraphJoinerThreadInfo> currentTopologyGraph;

      /// <summary>
      /// If we want to pause execution thorough the pipeline
      /// </summary>
      private bool _pause;

      /// <summary>
      /// use this for locking when we're pausing.
      /// </summary>
      private Object pauseLock;

      /// <summary>
      /// Allow all the threads to wake
      /// </summary>
      private ManualResetEvent unpauseEvent;

      /// <summary>
      /// True if the pipeline is currently active (i.e. all threads are active
      /// and not dead, or in the process of shutting down)
      /// </summary>
      private bool active;

      //----------------------- Constructors ----------------------------------
      /// <summary>
      /// default constructor
      /// </summary>
      public ProcessingPipeline()
      {
         // Make sure the stopwatch is high res. 
         if (!Stopwatch.IsHighResolution)
            ControlAPI.Instance.nonFatalError(this, "the System.Diagnostics.Stopwatch is not high resolution. framerates may not be accurate.");

         // default with no entries so it is created from scratch
         currentTopologyGraph = new List<GraphJoinerThreadInfo>();

         // start any new topology paused.
         _pause = true;
         pauseLock = new Object();
         unpauseEvent = new ManualResetEvent(false);

         active = false;
      }

      //----------------------- public processing members ---------------------
      /// <summary>
      /// This causes all the threads to be killed and the pipeline put back
      /// to it's initial state. Call updatePipeline with a new topology
      /// to get it going again.
      /// </summary>
      public void shutdown()
      {
         // lock to prevent shutdown() and updateTopology() interfering with each other.
         lock (this)
         {
            active = false;
            // If it was paused, unpause it to let the plugins continue and exit
            if (_pause)
            {
               this.unpause();
            }

            // Stop all the threads
            for (int i = 0; i < currentTopologyGraph.Count; i++)
            {
               currentTopologyGraph[i].thread.Interrupt();
               currentTopologyGraph[i].thread.Abort();
               currentTopologyGraph[i].thread.Join(1000);
               if (currentTopologyGraph[i].thread.ThreadState != System.Threading.ThreadState.Stopped)
               {
                  // mmm... for some reason this thread isn't stopping in a reasonable time.
                  ControlAPI.Instance.nonFatalError(this, "Couldn't stop the joiner thread between {0}:{1} -> {2}:{3}", currentTopologyGraph[i].edge.vertex1.Name, currentTopologyGraph[i].edge.vertex1.OutputCapabilities[currentTopologyGraph[i].edge.vertex1Pin].Name, currentTopologyGraph[i].edge.vertex2.Name, currentTopologyGraph[i].edge.vertex2.InputCapabilities[currentTopologyGraph[i].edge.vertex2Pin].Name);
               }
            }

            // clear the graph
            currentTopologyGraph.Clear();

            // make sure we start paused next time
            pause();
         }
      }

      /// <summary>
      /// pause this pipeline by stopping frames passing between
      /// all plugins.
      /// </summary>
      public void pause()
      {
         lock (pauseLock)
         {
            unpauseEvent.Reset();
            _pause = true;
         }
      }

      /// <summary>
      /// unpause this pipeline
      /// </summary>
      public void unpause()
      {
         lock (pauseLock)
         {
            _pause = false;
            unpauseEvent.Set();
         }
      }

      public bool isPaused()
      {
         return _pause;
      }

      /// <summary>
      /// Updates the internal pricessing pipeline with a new topology.
      /// This means all plugins will be connected by a thread and the pipeline
      /// is ready to accept frames.
      /// This is used to create the initial topology AND update the pipeline
      /// as the topology changes. It can change the topology "on-the-fly"
      /// while frames aree still passing through the pipeline (there will be
      /// a (probably un-noticable) brief pause as links are re-routed).
      /// </summary>
      public void updateTopology(Topology topology)
      {
         // lock to prevent other threads updating the processing pipeline at the same time
         // and to prevent shutdown() and updateTopology() from interfering with each other.
         lock (this)
         {
            // if it's not paused, do it now
            bool wasPaused = _pause;
            if (!_pause)
            {
               this.pause();
            }

            // note which edges have/haven't changed
            bool[] found = new bool[currentTopologyGraph.Count];
            for (int i = 0; i < found.Length; i++) found[i] = false;

            // This will be the currentTopologyGraph at the end of this operation
            List<GraphJoinerThreadInfo> newTopologyGraph = new List<GraphJoinerThreadInfo>(topology.TopologyGraph.Count);

            // We're now set to active - not really true, but each thread is started in an active state
            // This is done so we are notified of any thread startup errrors
            active = true;

            // check each of the new topology edges and create new links as needed
            for (int i = 0; i < topology.TopologyGraph.Count; i++)
            {
               // Check if this edge already exists in the processing chain.
               int foundIndex = searchForEdge(topology.TopologyGraph[i]);
               if (foundIndex < 0)
               {
                  // This link hasn't been created yet, so start it now
                  Thread t = new Thread(new ParameterizedThreadStart(graphJoinerThread));
                  t.Start(topology.TopologyGraph[i]);
                  GraphJoinerThreadInfo info = new GraphJoinerThreadInfo();
                  info.edge = topology.TopologyGraph[i];
                  info.thread = t;
                  newTopologyGraph.Insert(i, info);
               }
               else
               {
                  // This plugin is already processing nicely, so just leave it as it is
                  GraphJoinerThreadInfo info = new GraphJoinerThreadInfo();
                  info.edge = currentTopologyGraph[foundIndex].edge;
                  info.thread = currentTopologyGraph[foundIndex].thread;
                  newTopologyGraph.Insert(i, info);
                  found[foundIndex] = true;
               }
            }

            // remove old links which are no longer needed.
            for (int i = 0; i < found.Length; i++)
            {
               if (!found[i])
               {
                  // don't need this joiner thread any more
                  currentTopologyGraph[i].thread.Abort();
                  currentTopologyGraph[i].thread.Join(1000);
                  if (currentTopologyGraph[i].thread.ThreadState != System.Threading.ThreadState.Stopped)
                  {
                     // mmm... for some reason this thread isn't stopping in a reasonable time.
                     ControlAPI.Instance.nonFatalError(this, "Couldn't stop the joiner thread between {0}:{1} -> {2}:{3}", currentTopologyGraph[i].edge.vertex1.Name, currentTopologyGraph[i].edge.vertex1.OutputCapabilities[currentTopologyGraph[i].edge.vertex1Pin].Name, currentTopologyGraph[i].edge.vertex2.Name, currentTopologyGraph[i].edge.vertex2.InputCapabilities[currentTopologyGraph[i].edge.vertex2Pin].Name);
                  }
               }
            }
            // copy over the old topology graph
            currentTopologyGraph.Clear();
            currentTopologyGraph = newTopologyGraph;

            // resume as we were
            if (!wasPaused)
               this.unpause();
         }
      }

      //----------------------- private processing members --------------------
      /// <summary>
      /// Used to determine if the given graph edge is currently being used.
      /// This operation is performed by linerarly searching through the current
      /// topology and calling the Equals method on each graph entry.
      /// The position in the currentTopologyGraph list is returned.
      /// </summary>
      /// <param name="e">the Graph Edge to search for</param>
      /// <returns>index in the currentTopologyGraph for this entry, or -1 for not found.</returns>
      int searchForEdge(Topology.GraphEntry e)
      {
         for(int i = 0 ; i< currentTopologyGraph.Count ; i++)
         {
            if (currentTopologyGraph[i].edge.Equals(e))
               return i;
         }
         return -1;
      }

      /// <summary>
      /// Each edge in a topology graph is implemented as a Thread to handle
      /// passing of frames between threads and blocking between multiple
      /// plugins.
      /// </summary>
      /// <param name="edge">The edge from teh topology we're implementing</param>
      private void graphJoinerThread(object edge)
      {
         // set up a timer to detrmine the processing time of the sink
         Stopwatch timer = new Stopwatch();
         Topology.GraphEntry e = (Topology.GraphEntry)edge;
         try
         {
            while (true)
            {
               try
               {
                  // handle the pause
                  while (isPaused())
                     unpauseEvent.WaitOne();

                  // wait for a frame from the source vertex
                  IFrame frame = e.vertex1.getFrame(e.vertex1Pin);

                  // If the is ISource was an actual Source (i.e. it's *generating* frames, not
                  // modifying them) we need to notify the ControlAPI of some stuff
                  if (e.vertex1 is Source)
                  {
                     if (frame == null)
                     {
                        // Source has read until EOF - no more, so pause this link.
                        pause();
                        ControlAPI.Instance.notifySourceStopped(e.vertex1 as Source);
                        continue;
                     }
                     else
                     {
                        ControlAPI.Instance.notifySourceFrameGrabbed(e.vertex1 as Source);
                     }
                  }

                  // pass it to the sink, recording how long processing takes.
                  timer.Start();
                  e.vertex2.sendFrame(frame, e.vertex2Pin);
                  timer.Stop();

                  double processingTime = ((double)timer.ElapsedTicks*1000.0) / (double)Stopwatch.Frequency;
                  timer.Reset();

                  // if we just passed the frame to a sink, do a garbage collection now if required
                  if (REGULAR_GARBAGE_COLLECTION && e.vertex2 is Sink)
                  {
                     timer.Start();
                     GC.Collect();
                     GC.WaitForPendingFinalizers();
                     timer.Stop();
                     double gcTime = ((double)timer.ElapsedTicks*1000.0) / (double)Stopwatch.Frequency;
                     timer.Reset();
                     ControlAPI.Instance.debugMessage(e.vertex2, "Garbage collection time: {0}ms", gcTime);
                  }

                  ControlAPI.Instance.debugMessage(e.vertex2, "Processing time: {0}ms.", processingTime);
               }
               catch (ProcessingException pe)
               {
                  if (active && pe.Recoverable)
                  {
                     ControlAPI.Instance.nonFatalError(this, "Recoverable Error in plugin between: \"{0}\":{1}({2}) -> \"{3}\":{4}({5}). \r\n{6}", e.vertex1.Name, e.vertex1Pin, e.vertex1.OutputCapabilities[e.vertex1Pin].Name, e.vertex2.Name, e.vertex2Pin, e.vertex2.InputCapabilities[e.vertex2Pin].Name, pe.Message);
                  }
                  else
                  {
                     throw pe;
                  }

               }
            }
         }
         catch (ThreadAbortException tae)
         {
            ControlAPI.Instance.debugMessage(this, "edge thread exiting for \"{0}\":{1}({2}) -> \"{3}\":{4}({5})", e.vertex1.Name, e.vertex1Pin, e.vertex1.OutputCapabilities[e.vertex1Pin].Name, e.vertex2.Name, e.vertex2Pin, e.vertex2.InputCapabilities[e.vertex2Pin].Name);
         }
         catch (ThreadInterruptedException tie)
         {
            ControlAPI.Instance.debugMessage(this, "edge thread exiting for \"{0}\":{1}({2}) -> \"{3}\":{4}({5})", e.vertex1.Name, e.vertex1Pin, e.vertex1.OutputCapabilities[e.vertex1Pin].Name, e.vertex2.Name, e.vertex2Pin, e.vertex2.InputCapabilities[e.vertex2Pin].Name);
         }
         catch (Exception ex)
         {
            // We,re only interested in errors if the pipeline is active.
            if (active)
            {
               ControlAPI.Instance.nonFatalError(this, "Error in plugin. thread exiting for \"{0}\":{1}({2}) -> \"{3}\":{4}({5}). \r\n{6}", e.vertex1.Name, e.vertex1Pin, e.vertex1.OutputCapabilities[e.vertex1Pin].Name, e.vertex2.Name, e.vertex2Pin, e.vertex2.InputCapabilities[e.vertex2Pin].Name, ex.Message);
            }
         }
      }
   }
}
