namespace FraMMWorksGUI
{
   /// <remarks>
   /// The GUI drawing class
   /// </remarks>
   partial class FraMMWorks
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.layoutManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.debugConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.processingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.liveModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.proccessingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.statusStrip1 = new System.Windows.Forms.StatusStrip();
         this.toolStripStatusLabel_statusMessage = new System.Windows.Forms.ToolStripStatusLabel();
         this.timeBar = new System.Windows.Forms.TrackBar();
         this.panel_controlButtons = new System.Windows.Forms.Panel();
         this.button_seek_forwardFrame = new System.Windows.Forms.Button();
         this.button_seek_forward = new System.Windows.Forms.Button();
         this.button_seek_back = new System.Windows.Forms.Button();
         this.button_seek_backFrame = new System.Windows.Forms.Button();
         this.button_pause = new System.Windows.Forms.Button();
         this.button_play = new System.Windows.Forms.Button();
         this.button_stop = new System.Windows.Forms.Button();
         this.flowLayoutPanel_pluginControlArea = new System.Windows.Forms.FlowLayoutPanel();
         this.menuStrip1.SuspendLayout();
         this.statusStrip1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
         this.panel_controlButtons.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.processingToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(1142, 26);
         this.menuStrip1.TabIndex = 0;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 22);
         this.fileToolStripMenuItem.Text = "File";
         // 
         // openToolStripMenuItem
         // 
         this.openToolStripMenuItem.Name = "openToolStripMenuItem";
         this.openToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
         this.openToolStripMenuItem.Text = "Open";
         this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
         // 
         // exitToolStripMenuItem
         // 
         this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
         this.exitToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
         this.exitToolStripMenuItem.Text = "Exit";
         // 
         // editToolStripMenuItem
         // 
         this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layoutManagerToolStripMenuItem,
            this.debugConsoleToolStripMenuItem});
         this.editToolStripMenuItem.Name = "editToolStripMenuItem";
         this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 22);
         this.editToolStripMenuItem.Text = "View";
         // 
         // layoutManagerToolStripMenuItem
         // 
         this.layoutManagerToolStripMenuItem.Name = "layoutManagerToolStripMenuItem";
         this.layoutManagerToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
         this.layoutManagerToolStripMenuItem.Text = "Topology Desinger";
         this.layoutManagerToolStripMenuItem.Click += new System.EventHandler(this.layoutManagerToolStripMenuItem_Click);
         // 
         // debugConsoleToolStripMenuItem
         // 
         this.debugConsoleToolStripMenuItem.Name = "debugConsoleToolStripMenuItem";
         this.debugConsoleToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
         this.debugConsoleToolStripMenuItem.Text = "Debug Console";
         this.debugConsoleToolStripMenuItem.Click += new System.EventHandler(this.debugConsoleToolStripMenuItem_Click);
         // 
         // processingToolStripMenuItem
         // 
         this.processingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.liveModeToolStripMenuItem,
            this.proccessingModeToolStripMenuItem});
         this.processingToolStripMenuItem.Name = "processingToolStripMenuItem";
         this.processingToolStripMenuItem.Size = new System.Drawing.Size(56, 22);
         this.processingToolStripMenuItem.Text = "Mode";
         // 
         // liveModeToolStripMenuItem
         // 
         this.liveModeToolStripMenuItem.Name = "liveModeToolStripMenuItem";
         this.liveModeToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
         this.liveModeToolStripMenuItem.Text = "Live Mode";
         // 
         // proccessingModeToolStripMenuItem
         // 
         this.proccessingModeToolStripMenuItem.Name = "proccessingModeToolStripMenuItem";
         this.proccessingModeToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
         this.proccessingModeToolStripMenuItem.Text = "Proccessing Mode";
         // 
         // helpToolStripMenuItem
         // 
         this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         this.helpToolStripMenuItem.Size = new System.Drawing.Size(48, 22);
         this.helpToolStripMenuItem.Text = "Help";
         // 
         // statusStrip1
         // 
         this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_statusMessage});
         this.statusStrip1.Location = new System.Drawing.Point(0, 687);
         this.statusStrip1.Name = "statusStrip1";
         this.statusStrip1.Size = new System.Drawing.Size(1142, 23);
         this.statusStrip1.TabIndex = 1;
         this.statusStrip1.Text = "statusStrip1";
         // 
         // toolStripStatusLabel_statusMessage
         // 
         this.toolStripStatusLabel_statusMessage.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
         this.toolStripStatusLabel_statusMessage.Name = "toolStripStatusLabel_statusMessage";
         this.toolStripStatusLabel_statusMessage.Size = new System.Drawing.Size(49, 18);
         this.toolStripStatusLabel_statusMessage.Text = "Status";
         // 
         // timeBar
         // 
         this.timeBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.timeBar.Location = new System.Drawing.Point(0, 602);
         this.timeBar.Name = "timeBar";
         this.timeBar.Size = new System.Drawing.Size(1142, 53);
         this.timeBar.TabIndex = 2;
         this.timeBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.timeBar_MouseDown);
         this.timeBar.ValueChanged += new System.EventHandler(this.timeBar_ValueChanged);
         this.timeBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.timeBar_MouseUp);
         // 
         // panel_controlButtons
         // 
         this.panel_controlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.panel_controlButtons.Controls.Add(this.button_seek_forwardFrame);
         this.panel_controlButtons.Controls.Add(this.button_seek_forward);
         this.panel_controlButtons.Controls.Add(this.button_seek_back);
         this.panel_controlButtons.Controls.Add(this.button_seek_backFrame);
         this.panel_controlButtons.Controls.Add(this.button_pause);
         this.panel_controlButtons.Controls.Add(this.button_play);
         this.panel_controlButtons.Controls.Add(this.button_stop);
         this.panel_controlButtons.Location = new System.Drawing.Point(3, 661);
         this.panel_controlButtons.Name = "panel_controlButtons";
         this.panel_controlButtons.Size = new System.Drawing.Size(1142, 25);
         this.panel_controlButtons.TabIndex = 3;
         // 
         // button_seek_forwardFrame
         // 
         this.button_seek_forwardFrame.Image = global::FraMMWorksGUI.Properties.Resources.frameForwardButton;
         this.button_seek_forwardFrame.Location = new System.Drawing.Point(185, 0);
         this.button_seek_forwardFrame.Name = "button_seek_forwardFrame";
         this.button_seek_forwardFrame.Size = new System.Drawing.Size(25, 25);
         this.button_seek_forwardFrame.TabIndex = 6;
         this.button_seek_forwardFrame.Text = ">";
         this.button_seek_forwardFrame.UseVisualStyleBackColor = true;
         this.button_seek_forwardFrame.Click += new System.EventHandler(this.button_seek_end_Click);
         // 
         // button_seek_forward
         // 
         this.button_seek_forward.Image = global::FraMMWorksGUI.Properties.Resources.fastForwardButton;
         this.button_seek_forward.Location = new System.Drawing.Point(159, 0);
         this.button_seek_forward.Name = "button_seek_forward";
         this.button_seek_forward.Size = new System.Drawing.Size(25, 25);
         this.button_seek_forward.TabIndex = 5;
         this.button_seek_forward.Text = ">>";
         this.button_seek_forward.UseVisualStyleBackColor = true;
         this.button_seek_forward.Click += new System.EventHandler(this.button_seek_forward_Click);
         // 
         // button_seek_back
         // 
         this.button_seek_back.Image = global::FraMMWorksGUI.Properties.Resources.rewindButton;
         this.button_seek_back.Location = new System.Drawing.Point(128, 0);
         this.button_seek_back.Name = "button_seek_back";
         this.button_seek_back.Size = new System.Drawing.Size(25, 25);
         this.button_seek_back.TabIndex = 4;
         this.button_seek_back.Text = "<<";
         this.button_seek_back.UseVisualStyleBackColor = true;
         this.button_seek_back.Click += new System.EventHandler(this.button_seek_back_Click);
         // 
         // button_seek_backFrame
         // 
         this.button_seek_backFrame.Image = global::FraMMWorksGUI.Properties.Resources.frameBackButton;
         this.button_seek_backFrame.Location = new System.Drawing.Point(102, 0);
         this.button_seek_backFrame.Name = "button_seek_backFrame";
         this.button_seek_backFrame.Size = new System.Drawing.Size(25, 25);
         this.button_seek_backFrame.TabIndex = 3;
         this.button_seek_backFrame.Text = "<";
         this.button_seek_backFrame.UseVisualStyleBackColor = true;
         this.button_seek_backFrame.Click += new System.EventHandler(this.button_seek_start_Click);
         // 
         // button_pause
         // 
         this.button_pause.Image = global::FraMMWorksGUI.Properties.Resources.pauseButton;
         this.button_pause.Location = new System.Drawing.Point(53, 0);
         this.button_pause.Name = "button_pause";
         this.button_pause.Size = new System.Drawing.Size(25, 25);
         this.button_pause.TabIndex = 2;
         this.button_pause.UseVisualStyleBackColor = true;
         this.button_pause.Click += new System.EventHandler(this.button_pause_Click);
         // 
         // button_play
         // 
         this.button_play.Image = global::FraMMWorksGUI.Properties.Resources.playButton;
         this.button_play.Location = new System.Drawing.Point(26, 0);
         this.button_play.Name = "button_play";
         this.button_play.Size = new System.Drawing.Size(25, 25);
         this.button_play.TabIndex = 1;
         this.button_play.UseVisualStyleBackColor = true;
         this.button_play.Click += new System.EventHandler(this.button_play_Click);
         // 
         // button_stop
         // 
         this.button_stop.Image = global::FraMMWorksGUI.Properties.Resources.stopButton;
         this.button_stop.Location = new System.Drawing.Point(0, 0);
         this.button_stop.Name = "button_stop";
         this.button_stop.Size = new System.Drawing.Size(25, 25);
         this.button_stop.TabIndex = 0;
         this.button_stop.UseVisualStyleBackColor = true;
         this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
         // 
         // flowLayoutPanel_pluginControlArea
         // 
         this.flowLayoutPanel_pluginControlArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.flowLayoutPanel_pluginControlArea.AutoScroll = true;
         this.flowLayoutPanel_pluginControlArea.Location = new System.Drawing.Point(0, 29);
         this.flowLayoutPanel_pluginControlArea.Name = "flowLayoutPanel_pluginControlArea";
         this.flowLayoutPanel_pluginControlArea.Size = new System.Drawing.Size(1142, 567);
         this.flowLayoutPanel_pluginControlArea.TabIndex = 4;
         // 
         // FraMMWorks
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1142, 710);
         this.Controls.Add(this.flowLayoutPanel_pluginControlArea);
         this.Controls.Add(this.panel_controlButtons);
         this.Controls.Add(this.timeBar);
         this.Controls.Add(this.statusStrip1);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "FraMMWorks";
         this.Text = "FraMMWorks";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FraMMWorks_FormClosing);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.statusStrip1.ResumeLayout(false);
         this.statusStrip1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.timeBar)).EndInit();
         this.panel_controlButtons.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem processingToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem liveModeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem proccessingModeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
      private System.Windows.Forms.StatusStrip statusStrip1;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_statusMessage;
      private System.Windows.Forms.TrackBar timeBar;
      private System.Windows.Forms.Panel panel_controlButtons;
      private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_pluginControlArea;
      private System.Windows.Forms.Button button_stop;
      private System.Windows.Forms.Button button_seek_forwardFrame;
      private System.Windows.Forms.Button button_seek_forward;
      private System.Windows.Forms.Button button_seek_back;
      private System.Windows.Forms.Button button_seek_backFrame;
      private System.Windows.Forms.Button button_pause;
      private System.Windows.Forms.Button button_play;
      private System.Windows.Forms.ToolStripMenuItem layoutManagerToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem debugConsoleToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
   }
}

