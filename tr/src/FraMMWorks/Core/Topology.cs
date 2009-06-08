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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FraMMWorks.Interfaces;

namespace FraMMWorks.Core
{
   /// <remarks>
   /// Manages the plugin chain topology. Provides a way to graphically
   /// build the topology, save and load it to a file and access information
   /// about the plugins (such as their drawable component).
   /// 
   /// Future work will allow this class to draw itself as a control and
   /// be modified graphically by users.
   /// 
   /// Wilson Waters W.Waters@curtin.edu.au 20080115
   /// </remarks>
   public partial class Topology : Form
   {
      //----------------------- public structs --------------------------------
      /// <summary>
      /// Represents an edge (link) on the topology graph.
      /// Each vertex is an actual plugin instance and the "pins" show which of
      /// ths input/output capabilities the link is connected to (i.e. the index
      /// of the array returned by getInput/OutputCapability())
      /// </summary>
      public struct GraphEntry
      {
         /// <summary>
         /// reference to the source plugin of this edge
         /// </summary>
         public ISource vertex1;

         /// <summary>
         /// reference to the sink plugin of this edge
         /// </summary>
         public ISink vertex2;

         /// <summary>
         /// Which pin his edge is connected to on vertex1
         /// </summary>
         public int vertex1Pin;

         /// <summary>
         /// Which pin this edge is connected to in vertex2
         /// </summary>
         public int vertex2Pin;

         /// <summary>
         /// Overridden Equals for GrapgEntries
         /// </summary>
         /// <param name="obj"></param>
         /// <returns></returns>
         public override bool Equals(object obj)
         {
            if (!(obj is GraphEntry))
               return false;
            else
               return Equals((GraphEntry)obj);
         }

         /// <summary>
         /// Two GraphEntries are the same if each vertex references the *same* object and
         /// the connected pins are the same.
         /// </summary>
         /// <param name="ge">the entry we are comparing</param>
         /// <returns></returns>
         public bool Equals(GraphEntry ge)
         {
            return (this.vertex1 == ge.vertex1 && this.vertex2 == ge.vertex2 && this.vertex1Pin == ge.vertex1Pin && this.vertex2Pin == ge.vertex2Pin);
         }

         /// <summary>
         /// Gets the hash code of this object
         /// </summary>
         /// <returns></returns>
         //public override int GetHashCode()
         //{
         //    return base.GetHashCode();
         //}
      }

      //----------------------- public data members --------------------------
      /// <summary>
      /// get the current state of the topology.
      /// </summary>
      public List<GraphEntry> TopologyGraph
      {
         get { lock (this) { return topologyGraph; }; }
      }

      /// <summary>
      /// Get all plugins which are active on the topology
      /// (but no necessarily connected to anything)
      /// </summary>
      public List<IPlugin> ActivePlugins
      {
         get { lock (this) { return activePlugins; }; }
      }

      /// <summary>
      /// Determines how this form closes and disables some features.
      /// </summary>
      public bool Standalone
      {
         get { return standalone; }
         set
         { 
            standalone = value;
            this.closeToolStripMenuItem.Text = (standalone ? "Exit" : "Hide");
         }
      }

      //----------------------- private data members --------------------------
      /// <summary>
      /// List of plugins can we know about.
      /// </summary>
      private PluginManager.PluginInfo[] availablePlugins;

      /// <summary>
      /// The actual Incidence list graph of this topology.
      /// Each entry is an edge on the topology graph.
      /// Each vertex within the graph is an actual instances a plugin which
      /// may have its properties (i.e. settings) modified,
      /// </summary>
      private List<GraphEntry> topologyGraph;

      /// <summary>
      /// Plugins which have been placed on the topology. This doesn't mean
      /// they are connected to anything, just that they are in use.
      /// for example, when a plugin is initially added (dragged from available
      /// plugins) it will not be connected to anything (i.e. not it topologyGraph)
      /// but it will a live object.
      /// 
      /// Every time a plugin is obtained from the plugin manager, it should also
      /// be added here.
      /// </summary>
      private List<IPlugin> activePlugins;

      /// <summary>
      /// True if this form is being used as a standalone topology creator.
      /// Some of the functinality will be disabled.
      /// </summary>
      private bool standalone;



      //----------------------- Constructors ----------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public Topology()
      {
         InitializeComponent();

         standalone = true; // default

         availablePlugins = PluginManager.Instance.AvailablePlugins;
         topologyGraph = new List<GraphEntry>();
         activePlugins = new List<IPlugin>();
      }

      //----------------------- public processing members ---------------------
      /// <summary>
      /// Saves this current topology to an XML file
      /// </summary>
      /// <param name="filename">file to save to</param>
      /// <param name="overwrite">whether to throw exception if file exists, or
      /// overwrite it.</param>
      public void save(String filename, bool overwrite)
      {
         lock (this)
         {
            throw new NotImplementedException();
         }
      }

      /// <summary>
      /// Load topology from an XML file.
      /// TODO: implement file reading.
      /// </summary>
      /// <param name="filename">the topology to load</param>
      public void load(String filename)
      {
         lock (this)
         {
            topologyGraph.Clear();

            // This is just a fixed topology for testing purposes so far.
            // The control would display something like this.
            /* 
             * +---------------------+       +-----------------+        +--------------+       +------------------------------+
             * | AForge Video Source |-------| GreyscaleFilter |--------| Video Mirror |-------| FraMMWorks GUI Video Display |
             * +---------------------+       +-----------------+        +--------------+       +------------------------------+
             */

            // Work out which plugin is what - this would normally be done through
            // the GUI initially, then streight from XML files on load(), hence the names
            IPlugin[] plugins = new IPlugin[10]; // need this to store the order
            foreach (PluginManager.PluginInfo info in availablePlugins)
            {
               IPlugin plugin; // temp storage;
               List<FraMMWorks.PluginBase.Setting> settings;
               switch (info.name)
               {
                  case "AForge Video Source":
                     plugin = PluginManager.Instance.getNewPluginInstance(info);
                     activePlugins.Add(plugin); plugins[0] = plugin;
                      settings = plugin.getSettings();
                     //settings[0].value = @"http://192.168.0.14:8080/mjpg/video.mjpg";
                      settings[0].value = @"C:\Documents and Settings\wilson\Desktop\FraMMWorks2\testData\test.avi";
                     //settings[0].value = @"S:\FraMMWorks\testData\testVideo-MJPEG.avi";
                     //settings[0].value = @"S:\FraMMWorks\testData\20-XViD_MPEG4.avi";
                     //settings[0].value = @"@device:sw:{860BB310-5D01-11D0-BD3B-00A0C911CE86}\\wwigo";

                     //settings[1].value = "MJPEG URL";
                     settings[1].value = "File";
                     break;

                  /*
                  case "Image File Source":
                     plugin = PluginManager.Instance.getNewPluginInstance(info);
                     activePlugins.Add(plugin); plugins[0] = plugin;
                     settings = plugin.getSettings();
                     settings[0].value = @"S:\FraMMWorks\testData\imageFileSource-testImages";
                     //settings[0].value = @"S:\FraMMWorks\testData\images2";
                     //settings[0].value = @"C:\testData";
                     settings[1].value = 10;
                     settings[2].value = false;
                     break;
                   */ 

                  case "Frame Splitter":
                     plugin = PluginManager.Instance.getNewPluginInstance(info);
                     activePlugins.Add(plugin); plugins[1] = plugin;
                     settings = plugin.getSettings();
                     settings[0].value = 2; // two way splitter
                     break;

                  //case "Video Mirror":
                  //   plugins[2] = PluginManager.Instance.getNewPluginInstance(info);
                  //   activePlugins.Add(plugins[2]);
                  //   break;

                  case "OpenCV Face Detector":
                        plugin = PluginManager.Instance.getNewPluginInstance(info);
                     activePlugins.Add(plugin); plugins[2] = plugin;
                     settings = plugin.getSettings();
                     lock (settings[0])
                     {
                        //settings[0].value = @"C:\testData\haarcascade_frontalface_alt.xml";
                        settings[0].value = @".\haarcascade_frontalface_alt.xml";
                        settings[0].isDirty = true;
                     }
                     break;

                  case "Filmstrip Navigation":
                     plugin = PluginManager.Instance.getNewPluginInstance(info);
                     activePlugins.Add(plugin); plugins[3] = plugin;
                     settings = plugin.getSettings();
                     lock (settings[0])
                     {
                        settings[0].value = @"S:\FraMMWorks\testData\testFilmstripIndex.txt";
                        //settings[0].value = @"C:\testData\testFilmstripIndex.txt";
                        settings[0].isDirty = true;
                     }
                     break;

                  case "FraMMWorks GUI Video Display":
                     plugin = PluginManager.Instance.getNewPluginInstance(info);
                     activePlugins.Add(plugin); plugins[4] = plugin;
                     break;
               }
            }

            // greate the graph, joining the plugins together
            // AForge Video Source -> Splitter
            GraphEntry e = new GraphEntry();
            e.vertex1 = (ISource)plugins[0];
            e.vertex1Pin = 0;
            e.vertex2 = (ISink)plugins[1];
            e.vertex2Pin = 0;
            topologyGraph.Add(e);

            // Splitter:0 -> OpenCV Face Detector
            e = new GraphEntry();
            e.vertex1 = (ISource)plugins[1];
            e.vertex1Pin = 0;
            e.vertex2 = (ISink)plugins[2];
            e.vertex2Pin = 0;
            topologyGraph.Add(e);

            // Splitter:1 -> Filmstrip Navigation
            e = new GraphEntry();
            e.vertex1 = (ISource)plugins[1];
            e.vertex1Pin = 1;
            e.vertex2 = (ISink)plugins[3];
            e.vertex2Pin = 0;
            topologyGraph.Add(e);

            // OpenCV Face Detector -> FraMMWorks GUI Video Display
            e = new GraphEntry();
            e.vertex1 = (ISource)plugins[2];
            e.vertex1Pin = 0;
            e.vertex2 = (ISink)plugins[4];
            e.vertex2Pin = 0;
            topologyGraph.Add(e);
         }

         ControlAPI.Instance.updateTopology(this);
      }

      //----------------------- private processing members --------------------
      private void closeMe()
      {
         // If this is a subform, just hide it.
         // Otherwise, we assume it's a standalone application and close the form
         if (standalone)
            this.Close();
         else
            this.Hide();
      }

      //----------------------- GUI Drawing members ---------------------------



      private void openToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FileDialog dialog = new OpenFileDialog();
         dialog.Filter = "FraMMWorks XML File (*.xml) | *.xml";
         dialog.ShowDialog();
         try
         {
            load(dialog.FileName);
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error loading XML topology file: " + ex.Message, "FraMMWorks Topology Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
         toolStripStatusLabel_statusMessages.Text = "Loaded " + dialog.FileName;
      }

      private void closeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void Topology_FormClosing(object sender, FormClosingEventArgs e)
      {
         // If this is a subform, just hide it.
         // Otherwise, we assume it's a standalone application and close the form
         if (!standalone)
         {
            e.Cancel = true;
            this.Hide();
         }
      }



   }
}
