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
   /// A form for building and displaying a Topology graph
   /// 
   /// Wilson Waters frammworks-mail@alintech.com.au 20100526
   /// </remarks>
   public partial class TopologyEditor : Form
   {
      //----------------------- public structs --------------------------------


      //----------------------- public data members --------------------------
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

      /// <summary>
      /// The Topology data model this GUI operates on
      /// </summary>
      public ITopology Topology
      {
         get { return this.topology; }
         set 
         { 
            topology = value;
            if (this.topologyControl1 != null)
               this.topologyControl1.Topology = this.topology;
         }
      }

      //----------------------- private data members --------------------------
      /// <summary>
      /// True if this form is being used as a standalone topology creator.
      /// Some of the functinality will be disabled.
      /// </summary>
      private bool standalone;

      /// <summary>
      /// The data model this GUI operates on
      /// </summary>
      private ITopology topology;

      //----------------------- Constructors ----------------------------------
      /// <summary>
      /// Default constructor
      /// </summary>
      public TopologyEditor()
      {
         InitializeComponent();
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
            throw new NotImplementedException();
         }
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
         dialog.Filter = "FraMMWorks Model File (*.fxm) | *.fxm";
         dialog.ShowDialog();
         try
         {
            load(dialog.FileName);
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error loading FMX model file: " + ex.Message, "FraMMWorks Topology Model Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
