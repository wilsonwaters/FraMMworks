namespace FraMMWorks.Core
{
   /// <remarks>
   /// Topology window
   /// </remarks>
   partial class Topology
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
         this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
         this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.statusStrip_bottom = new System.Windows.Forms.StatusStrip();
         this.toolStripStatusLabel_statusMessages = new System.Windows.Forms.ToolStripStatusLabel();
         this.splitContainer_main = new System.Windows.Forms.SplitContainer();
         this.label_temp_pluginarea = new System.Windows.Forms.Label();
         this.label_temp_drawingarea = new System.Windows.Forms.Label();
         this.menuStrip1.SuspendLayout();
         this.statusStrip_bottom.SuspendLayout();
         this.splitContainer_main.Panel1.SuspendLayout();
         this.splitContainer_main.Panel2.SuspendLayout();
         this.splitContainer_main.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(981, 26);
         this.menuStrip1.TabIndex = 0;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 22);
         this.fileToolStripMenuItem.Text = "File";
         // 
         // newToolStripMenuItem
         // 
         this.newToolStripMenuItem.Name = "newToolStripMenuItem";
         this.newToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.newToolStripMenuItem.Text = "New";
         // 
         // openToolStripMenuItem
         // 
         this.openToolStripMenuItem.Name = "openToolStripMenuItem";
         this.openToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.openToolStripMenuItem.Text = "Open";
         this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.saveToolStripMenuItem.Text = "Save";
         // 
         // saveAsToolStripMenuItem
         // 
         this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
         this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.saveAsToolStripMenuItem.Text = "Save As";
         // 
         // toolStripMenuItem1
         // 
         this.toolStripMenuItem1.Name = "toolStripMenuItem1";
         this.toolStripMenuItem1.Size = new System.Drawing.Size(141, 6);
         // 
         // closeToolStripMenuItem
         // 
         this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
         this.closeToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.closeToolStripMenuItem.Text = "Close";
         this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
         // 
         // helpToolStripMenuItem
         // 
         this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
         this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         this.helpToolStripMenuItem.Size = new System.Drawing.Size(48, 22);
         this.helpToolStripMenuItem.Text = "Help";
         // 
         // aboutToolStripMenuItem
         // 
         this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
         this.aboutToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
         this.aboutToolStripMenuItem.Text = "About";
         // 
         // statusStrip_bottom
         // 
         this.statusStrip_bottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_statusMessages});
         this.statusStrip_bottom.Location = new System.Drawing.Point(0, 597);
         this.statusStrip_bottom.Name = "statusStrip_bottom";
         this.statusStrip_bottom.Size = new System.Drawing.Size(981, 23);
         this.statusStrip_bottom.TabIndex = 1;
         this.statusStrip_bottom.Text = "statusStrip1";
         // 
         // toolStripStatusLabel_statusMessages
         // 
         this.toolStripStatusLabel_statusMessages.Name = "toolStripStatusLabel_statusMessages";
         this.toolStripStatusLabel_statusMessages.Size = new System.Drawing.Size(220, 18);
         this.toolStripStatusLabel_statusMessages.Text = "FraMMWorks Topology Designer";
         // 
         // splitContainer_main
         // 
         this.splitContainer_main.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer_main.Location = new System.Drawing.Point(0, 26);
         this.splitContainer_main.Name = "splitContainer_main";
         // 
         // splitContainer_main.Panel1
         // 
         this.splitContainer_main.Panel1.AutoScroll = true;
         this.splitContainer_main.Panel1.Controls.Add(this.label_temp_pluginarea);
         // 
         // splitContainer_main.Panel2
         // 
         this.splitContainer_main.Panel2.AutoScroll = true;
         this.splitContainer_main.Panel2.Controls.Add(this.label_temp_drawingarea);
         this.splitContainer_main.Size = new System.Drawing.Size(981, 571);
         this.splitContainer_main.SplitterDistance = 151;
         this.splitContainer_main.TabIndex = 2;
         // 
         // label_temp_pluginarea
         // 
         this.label_temp_pluginarea.AutoSize = true;
         this.label_temp_pluginarea.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label_temp_pluginarea.Location = new System.Drawing.Point(0, 0);
         this.label_temp_pluginarea.Name = "label_temp_pluginarea";
         this.label_temp_pluginarea.Size = new System.Drawing.Size(111, 17);
         this.label_temp_pluginarea.TabIndex = 0;
         this.label_temp_pluginarea.Text = "AvailablePlugins";
         // 
         // label_temp_drawingarea
         // 
         this.label_temp_drawingarea.AutoSize = true;
         this.label_temp_drawingarea.Dock = System.Windows.Forms.DockStyle.Fill;
         this.label_temp_drawingarea.Location = new System.Drawing.Point(0, 0);
         this.label_temp_drawingarea.Name = "label_temp_drawingarea";
         this.label_temp_drawingarea.Size = new System.Drawing.Size(153, 17);
         this.label_temp_drawingarea.TabIndex = 0;
         this.label_temp_drawingarea.Text = "Topology drawing area";
         this.label_temp_drawingarea.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // Topology
         // 
         this.ClientSize = new System.Drawing.Size(981, 620);
         this.Controls.Add(this.splitContainer_main);
         this.Controls.Add(this.statusStrip_bottom);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "Topology";
         this.Text = "FraMMWorks Topology Designer";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Topology_FormClosing);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.statusStrip_bottom.ResumeLayout(false);
         this.statusStrip_bottom.PerformLayout();
         this.splitContainer_main.Panel1.ResumeLayout(false);
         this.splitContainer_main.Panel1.PerformLayout();
         this.splitContainer_main.Panel2.ResumeLayout(false);
         this.splitContainer_main.Panel2.PerformLayout();
         this.splitContainer_main.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
      private System.Windows.Forms.StatusStrip statusStrip_bottom;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_statusMessages;
      private System.Windows.Forms.SplitContainer splitContainer_main;
      private System.Windows.Forms.Label label_temp_drawingarea;
      private System.Windows.Forms.Label label_temp_pluginarea;
   }
}