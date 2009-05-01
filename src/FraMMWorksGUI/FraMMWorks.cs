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

using FraMMWorks.Core;

namespace FraMMWorksGUI
{
   public partial class FraMMWorks : Form
   {
      //------------------------- private class fields ------------------------
      /// <summary>
      /// The topology form
      /// </summary>
      private Topology topology;

      /// <summary>
      /// The debuc console form
      /// </summary>
      private DebugConsole debugConsole;

      /// <summary>
      /// Deligate GUI thread to re-draw the controls
      /// </summary>
      /// <param name="controls"></param>
      private delegate void OnUpdateControls(List<Control> controls);

      /// <summary>
      /// Deligate GUI thread to change the controls at the bottom of the screen
      /// (hiding play/stop buttons, moving seek bar etc).
      /// </summary>
      /// <param name="controls"></param>
      private delegate void OnUpdateControlAction(ControlAPI.ControlAction action);

      /// <summary>
      /// true when the mouse is being dragged along the trackbar
      /// </summary>
      private bool trackBarMouseDown;


      //------------------------- Constructors --------------------------------
      /// <summary>
      /// Construct the GUI object
      /// </summary>
      public FraMMWorks()
      {
         InitializeComponent();

         // set up error and debug message passing
         ControlAPI.Instance.OnErrorMessage += new ControlAPI.ErrorMessageHandler(displayError);
         ControlAPI.Instance.OnDebugMessage += new ControlAPI.DebugMessageHandler(displayStatusMessage);

         // set up notification when the plugin drawable controls change
         ControlAPI.Instance.OnDisplayControlChange += new ControlAPI.DisplayControlHandler(updateControls);

         // set up noification when of source control changes (play pause etc)
         ControlAPI.Instance.OnControlAction +=new ControlAPI.ControlActionHandler(controlActionNotification);

         trackBarMouseDown = false;

         // debuging - remove me
         //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
      }


      //------------------------- FraMMWorks callbacks ------------------------
      private void displayError(String message)
      {
         if (debugConsole != null && !debugConsole.IsDisposed)
            debugConsole.updateMessages(ControlAPI.Instance.DebugMessages, ControlAPI.Instance.ErrorMessages);

         DialogResult res = MessageBox.Show(message+"\r\nContinue processing?", "FraMMWorks error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
         if (res == DialogResult.No)
            ControlAPI.Instance.stop();
      }

      private void displayStatusMessage(String message)
      {
         //this.toolStripStatusLabel_statusMessage.Text = message;

         if (debugConsole != null && !debugConsole.IsDisposed)
            debugConsole.updateMessages(ControlAPI.Instance.DebugMessages, ControlAPI.Instance.ErrorMessages);
      }

      /// <summary>
      /// Re-draws all the controls on the displayable panel.
      /// TODO: only re-draw if there have been changes
      /// </summary>
      /// <param name="controls"></param>
      private void updateControls(List<Control> controls)
      {
         // need to marshal this call back to the UI thread, as it id called from a worker thread
         if (this.flowLayoutPanel_pluginControlArea.InvokeRequired)
         {
            BeginInvoke(new OnUpdateControls(updateControls), new object[] { controls });
         }
         else
         {
            // the actual update
            // first remove all existing controls
            this.flowLayoutPanel_pluginControlArea.Controls.Clear();

            // now, add them all back!
            /*
            foreach (Control control in controls)
            {
               // place each one in a GroupBox
               GroupBox box = new GroupBox();
               box.FlatStyle = FlatStyle.Standard;
               control.Location = new Point(5, 20);
               box.Controls.Add(control);
               box.AutoSize = true;
               box.Text = control.Name;
               this.flowLayoutPanel_pluginControlArea.Controls.Add(box);
            }
             */ 
            this.flowLayoutPanel_pluginControlArea.Controls.AddRange(controls.ToArray());
         }
      }

      /// <summary>
      /// Update the GUI control buttons when an action is performed(i.e. play, pause)
      /// </summary>
      /// <param name="action"></param>
      private void controlActionNotification(ControlAPI.ControlAction action)
      {
         // need to marshal this call back to the UI thread, as it id called from a worker thread
         if (this.timeBar.InvokeRequired)
         {
            BeginInvoke(new OnUpdateControlAction(controlActionNotification), new object[] { action });
         }
         else
         {
            switch (action)
            {
               case ControlAPI.ControlAction.play:
                  this.button_play.Enabled = false;
                  this.button_stop.Enabled = true;
                  this.button_pause.Enabled = true;
                  this.button_seek_forward.Enabled = true;
                  this.button_seek_back.Enabled = true;
                  this.button_seek_forwardFrame.Enabled = false;
                  this.button_seek_backFrame.Enabled = false;
                  break;

               case ControlAPI.ControlAction.stop:
                  this.button_play.Enabled = true;
                  this.button_stop.Enabled = false;
                  this.button_pause.Enabled = false;
                  this.button_seek_forward.Enabled = false;
                  this.button_seek_back.Enabled = false;
                  this.button_seek_forwardFrame.Enabled = false;
                  this.button_seek_backFrame.Enabled = false;
                  break;

               case ControlAPI.ControlAction.pause:
                  this.button_play.Enabled = false;
                  this.button_stop.Enabled = true;
                  this.button_pause.Enabled = true;
                  this.button_seek_forward.Enabled = true;
                  this.button_seek_back.Enabled = true;
                  this.button_seek_forwardFrame.Enabled = true;
                  this.button_seek_backFrame.Enabled = true;
                  break;

               case ControlAPI.ControlAction.unpause:
                  this.button_play.Enabled = false;
                  this.button_stop.Enabled = true;
                  this.button_pause.Enabled = true;
                  this.button_seek_forward.Enabled = true;
                  this.button_seek_back.Enabled = true;
                  this.button_seek_forwardFrame.Enabled = false;
                  this.button_seek_backFrame.Enabled = false;
                  break;

               case ControlAPI.ControlAction.seek:
                  // TODO: put a timer on this, so it's only updated every second or so.
                  if (!trackBarMouseDown)
                  {
                     this.timeBar.Maximum = (int)ControlAPI.Instance.getNumFrames();
                     this.timeBar.Value = Math.Min((int)ControlAPI.Instance.getCurrentFrameNum(), this.timeBar.Maximum);
                  }
                  break;

               default:
                  ControlAPI.Instance.nonFatalError(this, "Unknown control action: {0}", action);
                  break;
            }
         }
      }


      //------------------------- GUI callbacks -------------------------------
      private void button_stop_Click(object sender, EventArgs e)
      {
         ControlAPI.Instance.stop();
      }

      private void button_play_Click(object sender, EventArgs e)
      {
         ControlAPI.Instance.play();
      }

      private void button_pause_Click(object sender, EventArgs e)
      {
         if (ControlAPI.Instance.isPaused())
         {
            ControlAPI.Instance.unpause();
         }
         else
         {
            ControlAPI.Instance.pause();
            this.button_play.Enabled = false;
         }
      }

      private void button_seek_start_Click(object sender, EventArgs e)
      {
         ControlAPI.Instance.seek(0);
      }

      private void button_seek_back_Click(object sender, EventArgs e)
      {
         ControlAPI.Instance.seek(ControlAPI.Instance.getCurrentFrameNum()-1);
      }

      private void button_seek_forward_Click(object sender, EventArgs e)
      {
         ControlAPI.Instance.seek(ControlAPI.Instance.getCurrentFrameNum() + 1);
      }

      private void button_seek_end_Click(object sender, EventArgs e)
      {
         ControlAPI.Instance.seek(uint.MaxValue);
      }

      private void layoutManagerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // Load the topology form if it hasn't been loaded before, otherwise just show it
         if (topology == null || topology.IsDisposed)
         {
            topology = new Topology();
            topology.Standalone = false;
         }

         topology.Show();

      }

      private void FraMMWorks_FormClosing(object sender, FormClosingEventArgs e)
      {
         ControlAPI.Instance.shutdown();
      }

      private void debugConsoleToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // Load the topology form if it hasn't been loaded before, otherwise just show it
         if (debugConsole == null || debugConsole.IsDisposed)
         {
            debugConsole = new DebugConsole();
         }

         debugConsole.Show();
         debugConsole.updateMessages(ControlAPI.Instance.DebugMessages, ControlAPI.Instance.ErrorMessages);
      }

      /// <summary>
      /// opens a topology file without going through the Topology editor.
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void openToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (topology != null && !topology.IsDisposed)
         {
            DialogResult res = MessageBox.Show("Save existing topology?", "Save topology", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
               FileDialog dialog = new SaveFileDialog();
               dialog.Filter = "FraMMWorks XML File (*.xml) | *.xml";
               dialog.ShowDialog();
               if (dialog.FileName.Length > 0)
               {
                  try
                  {
                     topology.save(dialog.FileName, false);
                  }
                  catch (Exception ex)
                  {
                     MessageBox.Show("Error saving topology: "+ex.Message, "Topology save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     return;
                  }
               }
               else if(res == DialogResult.No)
               {
                  topology = null;
               }
               else
               {
                  return;
               }
            }

         }

         // make sure we;re all stopped
         ControlAPI.Instance.stop();

         // create the new one.
         topology = new Topology();
         topology.Standalone = false;

         FileDialog openDialog = new OpenFileDialog();
         openDialog.Filter = "FraMMWorks XML File (*.xml) | *.xml";
         openDialog.ShowDialog();
         if (openDialog.FileName.Length > 0)
         {
            try
            {
               topology.load(openDialog.FileName);
               this.toolStripStatusLabel_statusMessage.Text = "Loaded " + openDialog.FileName;
            }
            catch (Exception ex)
            {
               MessageBox.Show("Error loading XML topology file: " + ex.Message, "FraMMWorks Topology Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
      }

      private void timeBar_ValueChanged(object sender, EventArgs e)
      {
         if (trackBarMouseDown)
         {
            ControlAPI.Instance.seek((uint)(sender as TrackBar).Value);
         }
      }

      private void timeBar_MouseUp(object sender, MouseEventArgs e)
      {
         trackBarMouseDown = false;
      }

      private void timeBar_MouseDown(object sender, MouseEventArgs e)
      {
         trackBarMouseDown = true;
      }

   }
}